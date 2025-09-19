using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using LitJson;
using System.Collections.Generic;
namespace Msdk
{
	// 与手机平台消息交中心
    public class MessageCenter : MonoBehaviour, IMSDKLisener
	{
        /**仅仅将json的外层键值对进行解析，方便对外层键值对进行查找和比对**/
        public static Dictionary<String, String> simpleParseJson(String json) {
            Dictionary<string, string> keyValueDic = new Dictionary<string, string>();
            if (String.IsNullOrEmpty(json)) {
                return keyValueDic;
            }
            //剥离json外部的{}
            if (json.StartsWith("{"))
            {
                json = json.Substring(json.IndexOf('{') + 1);
            }
            if (json.EndsWith("}"))
            {
                json = json.Substring(0, json.LastIndexOf("}"));
            }
            String[] comaSplit = json.Split(',');
            try
            {
                foreach (String keyValue in comaSplit)
                {
                    String[] colonSplit = keyValue.Split(':');
                    if (colonSplit.Length > 1)
                    {
                        //去除key value外层的引号
						String key = colonSplit[0].Replace("\"","").Trim();
						String value = colonSplit[1].Replace("\"", "").Trim();
                        if (!keyValueDic.ContainsKey(key))
                        {
                            keyValueDic.Add(key, value);
                        } 
                    }
                }
            }
            catch (Exception e) {
                String exception = "{'MessageComsumerExpInfo':'" + e.Message + "'}";
                resultQueue.Enqueue(exception);
            }

            return keyValueDic;
        }
        // LPStr:单字节、空终止的 ANSI 字符串
        public delegate String SendToUnity([MarshalAs(UnmanagedType.LPStr)] String jsonStr);

        // 非托管代码回调到Unity的特性
        // iOS的Full AOP编译要求：1.使用MonoPInvokeCallback标签 2.使用静态方法
        public class MonoPInvokeCallbackAttribute : System.Attribute
        {
            private Type type;
            public MonoPInvokeCallbackAttribute(Type t) { type = t; }
        }

        // android UnitySendMessage to this method
 		public void OnMSDKMessage(string result)
 		{
            MsdkUtil.Log("OnMSDKMessage receiev succeed");
            MessageConsumer(result);
 		}

[MonoPInvokeCallback(typeof(SendToUnity))]
        public static String MessageConsumer([MarshalAs(UnmanagedType.LPStr)] String jsonStr)
        {
            lock (queueLock)
            {
                try
                {
                    if (String.IsNullOrEmpty(jsonStr))
                    {
                        return "";
                    }

                    String methodName = "";
                    Dictionary<String, String> jsonDic = simpleParseJson(jsonStr);
                    //判断传递数据是否合法
                    if (jsonDic.ContainsKey("MsdkMethod"))
                    {
                        methodName = jsonDic["MsdkMethod"];
                    }
                    else
                    {
                        return "";
                    }
                    if (String.IsNullOrEmpty(methodName))
                    {
                        return "";
                    }
                    // 如果是Crash回调，为保证额外数据上报正常，保持非UnityMain线程调用
                    if (methodName.Equals("OnCrashExtMessageNotify"))
                    {
                        String toNative = OnCrashExtMessageNotify(jsonStr);
                        return toNative;
                    }
                    else if (methodName.Equals("OnCrashExtDataNotify"))
                    {
                        String toNative = OnCrashExtDataNotify(jsonStr);
                        return toNative;
                    }
                    else
                    {
                        // 非Crash回调，则将结果string添加到队列
                        resultQueue.Enqueue(jsonStr);
                        return "";
                    }
                }
                catch (Exception e)
                {
                    String exception = "{'MessageComsumerExpInfo':'" + e.Message + "'}";
                    resultQueue.Enqueue(exception);
                }
                return "";
            }
        }

		static MessageCenter instance = null;
		static GameObject bridgeGameObject = null;

        private static System.Object queueLock = new System.Object();
        private static Queue resultQueue = new Queue(10);

		public static MessageCenter Instance
		{
			get
			{
				if(instance == null){
					if (bridgeGameObject == null) {
						bridgeGameObject = new GameObject();
						bridgeGameObject.name = "MSDKCallbackBridge";
						DontDestroyOnLoad(bridgeGameObject);
					}
					instance = bridgeGameObject.AddComponent(typeof(MessageCenter)) as MessageCenter;
				}
				return instance;
			}
		}

		public void Init()
		{
            MsdkUtil.Log("Init MSDKCallbackBridge");
		}

        void Start()
        {
            // 若开启PC虚拟环境，检查是否需要触发WakeupEvent
            #if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
            if (WGPlatform.GetPCDebug()) {
                if (DataGenerator.Instance.WakeupData != null) {
                    Invoke("NotifyWakeup", 0.1f);
                    if (DataGenerator.Instance.WakeupData.flag == eFlag.eFlag_UrlLogin) {
                        Invoke("NotifyLogin", 0.5f);
                    }
                }
            }
            #endif
        }

        // Update is called once per frame
        void Update ()
        {
            // 队列中有值，则取出并在UnityMain线程回调到游戏
            string jsonStr = "";
            lock (queueLock) {
                if (resultQueue.Count > 0) {
                    jsonStr  = (String)resultQueue.Dequeue();
                }
            }
            if (String.IsNullOrEmpty(jsonStr)) {
                return;
            }
            JsonData data = null;
            try {
                data = JsonMapper.ToObject(jsonStr);
                String methodName = (string)data["MsdkMethod"];
                if (String.IsNullOrEmpty(methodName)) {
					MsdkUtil.Log("Process message error! MsdkMethod is null.");
					MsdkUtil.Log(jsonStr);
                    return;
                }
				MsdkUtil.Log("MessageCenter receive : " + jsonStr);
                MethodInfo methodInfo = Instance.GetType().GetMethod(methodName, new [] {typeof(string)});
                methodInfo.Invoke(Instance, new object[] { jsonStr });
            } catch (Exception e) {
				MsdkUtil.Log("update json parse exception");
				MsdkUtil.Log(jsonStr);
                MsdkUtil.Log(e.Message);
            }
        }

        private void NotifyWakeup()
        {
            MsdkEvent.Instance.HandleWakeupNofify(DataGenerator.Instance.WakeupData);
        }

        private void NotifyLogin()
        {
            MsdkEvent.Instance.HandleLoginNotify(DataGenerator.Instance.LoginData);
        }

		void OnDestroy()
		{
			MsdkUtil.Log(bridgeGameObject.name + " OnDestroy");
		}

		#region 回调处理
		public void OnLoginNotify (string jsonRet)
		{
			LoginRet ret = LoginRet.ParseJson (jsonRet);
			MsdkEvent.Instance.HandleLoginNotify(ret);
		}

        public void OnChannelPermissionAuthNotify(string jsonRet)
        {
            LoginRet ret = LoginRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleChannelPermissionAuthNotify(ret);
        }

        public void OnCheckULNotify(string jsonRet)
        {
            MsdkUtil.Log("OnCheckULNotify " );
            CheckULRet ret = CheckULRet.ParseJson(jsonRet);
            MsdkUtil.Log("OnCheckULNotify " + ret.ToString());
            MsdkEvent.Instance.HandleCheckULNotify(ret);
        }

        public void OnWakeupNotify (string jsonRet)
		{
			WakeupRet ret = WakeupRet.ParseJson (jsonRet);
			MsdkEvent.Instance.HandleWakeupNofify(ret);
		}

		public void OnShareNotify (string jsonRet)
		{
			ShareRet ret = ShareRet.ParseJson (jsonRet);
			MsdkEvent.Instance.HandleShareNofify(ret);
		}

		public void OnRelationNotify (string jsonRet)
		{
			RelationRet ret = RelationRet.ParseJson (jsonRet);
			MsdkEvent.Instance.HandleRelationNofify(ret);
		}

		public void OnLocationNotify (string jsonRet)
		{
			RelationRet ret = RelationRet.ParseJson (jsonRet);
			MsdkEvent.Instance.HandleLocationNotify(ret);
		}

		public void OnLocationGotNotify (string jsonRet)
		{
			LocationRet ret = LocationRet.ParseJson (jsonRet);
			MsdkEvent.Instance.HandleLocationGotNotify(ret);
		}

        public void OnLocationGotCountryFromIPNotify(string jsonRet)
        {
            CountryFromIPRet ret = CountryFromIPRet.ParseJson (jsonRet);
            MsdkEvent.Instance.HandleLocationGotCountryFromIP (ret);
        }

        public void OnLocationGetIPInfoNotify(string jsonRet)
        {
            GetIPInfoRet ret = GetIPInfoRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleGetIPInfo(ret);
        }

		public void OnFeedbackNotify (string jsonString)
		{
            try {
                JsonData jd = JsonMapper.ToObject(jsonString);
                int flag = (int)jd["flag"];
                string desc = (string)jd["desc"];
                MsdkEvent.Instance.HandleFeedbackNotify(flag, desc);
            }  catch (Exception e)  {
                Debug.LogError(e.Message);
            }
		}

        public static string OnCrashExtMessageNotify(string jsonString)
        {
            return MsdkEvent.Instance.HandleCrashMessageNotify();
        }

        public string OnCrashExtMessageNotify()
        {
            return MsdkEvent.Instance.HandleCrashMessageNotify();
        }

        public static string OnCrashExtDataNotify(string jsonString)
        {
            try {
                var tmp = MsdkEvent.Instance.HandleCrashDataNotify();
                string ret = System.Text.Encoding.Default.GetString(tmp);
                return ret;
            } catch (Exception e) {
                return "";
            }
        }

        public byte[] OnCrashExtDataNotify()
        {
            return MsdkEvent.Instance.HandleCrashDataNotify();
        }

		public void OnADNotify (string jsonRet)
		{
            ADRet ret = ADRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleShowADNotify(ret);
		}

		public void OnQueryGroupInfoNotify (string jsonRet)
		{
			GroupRet ret = GroupRet.ParseJson (jsonRet);
			MsdkEvent.Instance.HandleQueryGroupInfoNotify(ret);
		}

		public void OnJoinWXGroupNotify (string jsonRet)
		{
			GroupRet ret = GroupRet.ParseJson (jsonRet);
			MsdkEvent.Instance.HandleJoinWXGroupNotify(ret);
		}

		public void OnCreateWXGroupNotify (string jsonRet)
		{
			GroupRet ret = GroupRet.ParseJson (jsonRet);
			MsdkEvent.Instance.HandleCreateWXGroupNotify(ret);
		}

        public void OnRealNameAuthNotify(string jsonRet)
        {
            RealNameAuthRet ret = RealNameAuthRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleRealNameAuthNotify(ret);
        }

        public void OnJoinQQGroupNotify(string jsonRet)
        {
            GroupRet ret = GroupRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleJoinQQGroupNotify(ret);
        }

        public void OnQueryWXGroupStatusNotify(string jsonRet)
        {
            GroupRet ret = GroupRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleQueryWXGroupStatusNotify(ret);
        }
        public void OnWebviewNotify(string jsonRet)
        {
            WebviewRet ret = WebviewRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleWebviewNotify(ret);
        }
        public void OnQueryGroupKeyNotify(string jsonRet)
        {
            GroupRet ret = GroupRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleQueryGroupKeyNotify(ret);
        }
        public void OnBindGroupNotify(string jsonRet)
        {
            GroupRet ret = GroupRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleBindGroupNotify(ret);
        }
        public void OnUnbindGroupNotify(string jsonRet)
        {
            GroupRet ret = GroupRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleUnbindGroupNotify(ret);
        }

        //加绑群v2回调
        public void OnCreateGroupV2Notify(string jsonRet)
        {
            GroupRet ret = GroupRet.ParseJson(jsonRet);
            Debug.Log("groupret is :"+ret);
            MsdkEvent.Instance.HandleCreateGroupV2Notify(ret);
        }

        public void OnJoinGroupV2Notify(string jsonRet)
        {
            GroupRet ret = GroupRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleJoinGroupV2Notify(ret);
        }

        public void OnQueryGroupInfoV2Notify(string jsonRet)
        {
            GroupRet ret = GroupRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleQueryGroupInfoV2Notify(ret);
        }

        public void OnUnbindGroupV2Notify(string jsonRet)
        {
            GroupRet ret = GroupRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleUnbindGroupV2Notify(ret);
        }

        public void OnGetGroupCodeV2Notify(string jsonRet)
        {
            GroupRet ret = GroupRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleGetGroupCodeV2Notify(ret);
        }

        public void OnQueryBindGuildV2Notify(string jsonRet)
        {
            GroupRet ret = GroupRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleQueryBindGuildV2Notify(ret);
        }

        public void OnBindExistGroupV2Notify(string jsonRet)
        {
            GroupRet ret = GroupRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleBindExistGroupV2Notify(ret);
        }

        public void OnGetGroupListV2Notify(string jsonRet)
        {
            GroupRet ret = GroupRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleGetGroupListV2Notify(ret);
        }

        public void OnRemindGuildLeaderV2Notify(string jsonRet)
        {
            GroupRet ret = GroupRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleRemindGuildLeaderV2Notify(ret);
        }


        public void OnJsCallback(string jsonString)
        {
            try
            {
                JsonData jd = JsonMapper.ToObject(jsonString);
                string jsParams = (string)jd["params"];
                MsdkEvent.Instance.HandleJsCallbackEvent(jsParams);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void OnWebProgressChanged(string jsonString)
        {
            try
            {
                JsonData jd = JsonMapper.ToObject(jsonString);
                string url = (string)jd["url"];
                #if UNITY_ANDROID
                double progress = (double)jd["progress"];
                #elif UNITY_IPHONE || UNITY_IOS
                long progress = (long)jd["progress"];
                #else
                float progress = 0;
                #endif
                MsdkEvent.Instance.HandleWebProgressChanged(url, (float)progress);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

 

        public void OnWebClose(string jsonRet)
        {
            MsdkEvent.Instance.HandleWebClose();
        }

		#if UNITY_ANDROID
		public void OnADBackPressedNotify (string jsonRet)
		{
            ADRet ret = ADRet.ParseJson(jsonRet);
            MsdkEvent.Instance.HandleADBackPressedNotify(ret);
		}

		public void OnAddWXCardNotify (string jsonRet)
		{
			CardRet ret = CardRet.ParseJson (jsonRet);
			MsdkEvent.Instance.HandleAddWXCardNotify(ret);
		}

		public void OnCheckNeedUpdateInfo (string jsonString)
		{
			try {
				JsonData jd = JsonMapper.ToObject (jsonString);
                long newApkSize = (long)jd["newApkSize"];
				string newFeature = (String)jd ["newFeature"];
                long patchSize = (long)jd["patchSize"];
				int status = (int)jd ["status"];
				string updateDownloadUrl = (string)jd ["updateDownloadUrl"];
				int updateMethod = (int)jd ["updateMethod"];
				MsdkEvent.Instance.HandleCheckNeedUpdateInfo(newApkSize, newFeature, patchSize, status,
				                                    updateDownloadUrl, updateMethod);
			} catch (Exception e) {
				Debug.LogError(e.Message);
			}
		}

		public void OnDownloadAppProgressChanged (string jsonString)
		{
			try {
				JsonData jd = JsonMapper.ToObject (jsonString);
                long receiveDataLen = (long)jd["receiveDataLen"];
                long totalDataLen = (long)jd["totalDataLen"];
				MsdkEvent.Instance.HandleDownloadAppProgressChanged(receiveDataLen, totalDataLen);
			} catch (Exception e) {
				Debug.LogError(e.Message);
			}
		}

		public void OnDownloadAppStateChanged (string jsonString)
		{
			try {
				JsonData jd = JsonMapper.ToObject (jsonString);
				int state = (int)jd ["state"];
				int errorCode = (int)jd ["errorCode"];
				string errorMsg = (string)jd ["errorMsg"];
				MsdkEvent.Instance.HandleDownloadAppStateChanged(state, errorCode, errorMsg);
			} catch (Exception e) {
				Debug.LogError(e.Message);
			}
		}

		public void OnDownloadYYBProgressChanged (string jsonString)
		{
			try {
				JsonData jd = JsonMapper.ToObject (jsonString);
				string url = (string) jd["url"];
                long receiveDataLen = (long)jd["receiveDataLen"];
                long totalDataLen = (long)jd["totalDataLen"];
				MsdkEvent.Instance.HandleDownloadYYBProgressChanged(url, receiveDataLen, totalDataLen);
			} catch (Exception e) {
				Debug.LogError(e.Message);
			}
		}

		public void OnDownloadYYBStateChanged (string jsonString)
		{
			try {
				JsonData jd = JsonMapper.ToObject (jsonString);
				string url = (string) jd["url"];
				int state = (int)jd ["state"];
				int errorCode = (int)jd ["errorCode"];
				string errorMsg = (string)jd ["errorMsg"];
				MsdkEvent.Instance.HandleDownloadYYBStateChanged(url, state, errorCode, errorMsg);
			} catch (Exception e) {
				Debug.LogError(e.Message);
			}
		}

 

#endif

        #if UNITY_IPHONE || UNITY_IOS
        public void OnReceiveNotificationResponseNotify(string userInfo)
        {
            MsdkEvent.Instance.HandleReceiveNotificationResponse(userInfo);
        }

        #endif
		#endregion
	}
}
