using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

namespace com.tencent.pandora
{
    public class CSharpInterface
    {
        public static UInt64 NowMilliseconds()
        {
            return TimeHelper.NowMilliseconds();
        }

        public static void PlaySound(string id)
        {
            Pandora.Instance.PlaySound(id);
        }

        public static void Report(string content, int id, int type)
        {
            Pandora.Instance.Report(content, id, type);
        }

        public static void ReportError(string error, int id)
        {

            Pandora.Instance.ReportError(error, id);
        }

        public static GameObject CloneAndAddToParent(GameObject template, string name, GameObject parent)
        {
            if(template == null)
            {
                Logger.LogError("源节点不存在");
                return null;
            }

            GameObject item = GameObject.Instantiate(template) as GameObject;
            item.name = name;
            SetParent(item, parent);
            return item;
        }

        public static void SetParent(GameObject child, GameObject parent)
        {
            if(child == null)
            {
                Logger.LogError("源节点不存在");
                return;
            }

            if (parent == null)
            {
                Logger.LogError("父节点不存在");
                return;
            }
            child.transform.SetParent(parent.transform);
            child.transform.localPosition = Vector3.zero;
            child.transform.localScale = Vector3.one;
            child.transform.localRotation = Quaternion.identity;
        }

        public static void SetPanelParent(string panelName, GameObject panelParent)
        {
            Pandora.Instance.SetPanelParent(panelName, panelParent);
        }


        public static GameObject GetPanelParent(string moduleName)
        {
            return PanelManager.GetPanelParent(moduleName);
        }

        public static bool IsDebug()
        {
            return Pandora.Instance.IsDebug;
        }

        public static bool IsIOSPlatform
        {
            get
            {
                return PandoraSettings.IsIOSPlatform;
            }
        }

        public static string GetPlatformDescription()
        {
            return PandoraSettings.GetPlatformDescription();
        }

        public static string GetSDKVersion()
        {
            return Pandora.Instance.GetUserData().sSdkVersion;
        }

        /// <summary>
        /// Resources.UnloadUnusedAssets();该接口只应该由游戏方调用
        /// Pandora慎用该接口，可能导致游戏卡顿，在确认必要性很高时才能使用该接口
        /// </summary>
        public static void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }

        public static bool PauseDownloading
        {
            get
            {
                return Pandora.Instance.PauseDownloading;
            }
            set
            {
                Pandora.Instance.PauseDownloading = value;
            }
        }

        public static bool PauseSocketSending
        {
            get
            {
                return Pandora.Instance.PauseSocketSending;
            }
            set
            {
                Pandora.Instance.PauseSocketSending = value;
            }
        }

        public static bool WriteCookie(string fileName, string content)
        {
            return CookieHelper.Write(fileName, content);
        }

        public static string ReadCookie(string fileName)
        {
            return CookieHelper.Read(fileName);
        }

        public static UserData GetUserData()
        {
            return Pandora.Instance.GetUserData();
        }

        public static RemoteConfig GetRemoteConfig()
        {
            return Pandora.Instance.GetRemoteConfig();
        }

        public static void GetIconSprite(string type, string path, Action<Sprite> callback)
        {
            if(Pandora.Instance.GetIconSprite != null)
            {
                Pandora.Instance.GetIconSprite(type, path, callback);
            }
        }

        public static void ShowImage(string panelName, string url, GameObject go, bool isCacheInMemory, uint callId)
        {
            PanelManager.ShowImage(panelName, url, go, isCacheInMemory, delegate (int returnCode) { OnShowImage(panelName, url, callId, returnCode); });
        }

        private static void OnShowImage(string panelName, string url, uint callId, int returnCode)
        {
            Dictionary<string, System.Object> dict = new Dictionary<string, System.Object>();
            dict["PanelName"] = panelName;
            dict["Url"] = url;
            dict["RetCode"] = returnCode.ToString();
            ExecuteLuaCallback(callId, dict);
        }

        public static void ShowPortriat(string panelName, string url, GameObject go, bool isCacheInMemory, uint callId)
        {
            PanelManager.ShowPortrait(panelName, url, go, isCacheInMemory, delegate (int returnCode) { OnShowImage(panelName, url, callId, returnCode); });
        }

        public static void CacheImage(string url)
        {
            CacheManager.LoadAsset(url, null);
        }
        
        public static bool IsImageCached(string url)
        {
            return CacheManager.IsAssetExists(url);
        }

        public static void LoadAssetBundle(string url, uint callId)
        {
            AssetManager.GetAssetBundle(url, delegate (AssetBundle assetBundle) { OnLoadAsset(url, assetBundle, callId); });
        }

        public static void LoadGameObject(string url, bool isCacheInMemory, uint callId)
        {
            AssetManager.GetGameObject(url, isCacheInMemory, delegate (GameObject gameObject) { OnLoadAsset(url, gameObject, callId); });
        }

        public static void LoadImage(string url, bool isCacheInMemory, uint callId)
        {
            AssetManager.GetImage(url, isCacheInMemory, delegate (Texture2D texture) { OnLoadAsset(url, texture, callId); });
        }

        private static void OnLoadAsset(string url, UnityEngine.Object obj, uint callId)
        {
            UnityEngine.Object localObj = obj;
            Dictionary<string, System.Object> dict = new Dictionary<string, System.Object>();
            dict["Type"] = localObj.GetType().ToString();
            dict["Url"] = url;
            dict["RetCode"] = (localObj != null) ? 0 : 1;
            ExecuteLuaCallback(callId, dict);
        }

        public static UnityEngine.Object GetAsset(string url)
        {
            return AssetManager.GetAsset(url);
        }

        public static void CacheAsset(string url)
        {
            CacheManager.LoadAsset(url, null);
        }

        public static bool IsAssetCached(string url)
        {
            return CacheManager.IsAssetExists(url);
        }

        public static void ReleaseAsset(string url)
        {
            AssetManager.ReleaseAsset(url);
        }

        /// <summary>
        /// iOS强制需要使用https
        /// requestJson应该是一个Dictionary序列化后的字符串
        /// 可能需要根据业务的具体情况调整WwwLoader中的封包细节
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestJson"></param>
        /// <param name="isPost"></param>
        /// <param name="callId"></param>
        public static void LoadWww(string url, string requestJson, bool isPost, uint callId)
        {
            AssetManager.LoadWww(url, requestJson, isPost, delegate (string response) { OnLoadWww(response, callId); });
        }

        private static void OnLoadWww(string response, uint callId)
        {
            Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();
            result["Resp"] = response;
            result["RetCode"] = string.IsNullOrEmpty(response) ? "-1" : "0";
            ExecuteLuaCallback(callId, result);
        }

        public static void CreatePanel(uint callId, string panelName)
        {
            PanelManager.CreatePanel(panelName, delegate (int returnCode) { OnCreatePanel(panelName, callId, returnCode); });
        }

        private static void OnCreatePanel(string panelName, uint callId, int returnCode)
        {
            Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();
            result["PanelName"] = panelName;
            result["RetCode"] = returnCode.ToString();
            ExecuteLuaCallback(callId, result);
        }

        public static GameObject GetPanel(string panelName)
        {
            return PanelManager.GetPanel(panelName);
        }

        public static void HidePanel(string panelName)
        {
            PanelManager.Hide(panelName);
        }

        public static void HideAllPanel()
        {
            PanelManager.HideAll();
        }

        public static void DestroyPanel(string panelName)
        {
            PanelManager.Destroy(panelName);
        }

        public static void DestroyAllPanel()
        {
            PanelManager.DestroyAll();
        }

        public static bool GetTotalSwitch()
        {
            return Pandora.Instance.GetRemoteConfig().totalSwitch;
        }

        public static bool GetFunctionSwitch(string functionName)
        {
            return Pandora.Instance.GetRemoteConfig().GetFunctionSwitch(functionName);
        }

        /// <summary>
        /// Lua发送消息给游戏
        /// </summary>
        /// <param name="jsonStr"></param>
        public static void LuaCallGame(string jsonStr)
        {
            Pandora.Instance.CallGame(jsonStr);
        }

        /// <summary>
        /// 游戏发送消息给Lua
        /// </summary>
        /// <param name="jsonStr"></param>
        internal static void GameCallLua(string jsonStr)
        {
            try
            {
                if(LuaStateManager.IsInitialized == true)
                {
                    LuaStateManager.CallLuaFunction("Common.CommandFromGame", jsonStr);
                }
            }
            catch(Exception e)
            {
                string error = "Lua执行游戏发送过来的消息失败, " + jsonStr + " " + e.Message;
                Logger.LogError(error);
                Pandora.Instance.ReportError(error, ErrorCodeConfig.GAME_2_PANDORA_EXCEPTION);
            }
        }

        /// <summary>
        /// callId为Common.lua中生成的回调唯一id
        /// CommandId为Broker命令字
        /// </summary>
        /// <param name="callId"></param>
        /// <param name="jsonStr"></param>
        /// <param name="commandId"></param>
        public static void CallBroker(uint callId, string jsonStr, int commandId)
        {
            Pandora.Instance.BrokerSocket.Send(callId, jsonStr, commandId);
        }

        internal static void ExecuteLuaCallback(uint callId, Dictionary<string, System.Object> result)
        {
            string jsonStr = MiniJSON.Json.Serialize(result);
            ExecuteLuaCallback(callId, jsonStr);
        }

        internal static void ExecuteLuaCallback(uint callId, string jsonStr)
        {
            try
            {
                if(Pandora.Instance.IsDebug == true)
                {
                    Logger.Log("回调Lua, callId " + callId + ": ");
                    Logger.Log(jsonStr);
                }
                if(LuaStateManager.IsInitialized == true)
                {
                    LuaStateManager.CallLuaFunction("Common.ExecuteCallback", callId, jsonStr);
                }
            }
            catch(Exception e)
            {
                string error = "回调Lua出错了, jsonStr: " + jsonStr + " " + e.Message;
                Logger.LogError(error);
                Pandora.Instance.ReportError(error, ErrorCodeConfig.EXECUTE_LUA_CALLBACK_EXCEPTION);
            }
        }

        public static bool IOSPay(string jsonStr)
        {
            Logger.Log("IOS米大师支付： " + jsonStr);
#if UNITY_IOS
            return false;
#else
            return false;
#endif
        }

        public static bool AndroidPay(string jsonStr)
        {
            Logger.Log("安卓米大师支付： " + jsonStr);
#if UNITY_ANDROID
            return false;
#else
            return false;
#endif
        }

        internal static void IOSPayFinish(string jsonStr)
        {
            try
            {
                if (LuaStateManager.IsInitialized == true)
                {
                    Logger.Log("IOS米大师支付回调Lua层: " + jsonStr);
                    LuaStateManager.CallLuaFunction("Common.IOSPayFinish", jsonStr);
                }
            }
            catch (Exception e)
            {
                string error = "IOS米大师支付回调Lua层发生错误\n" + e.Message;
                Logger.LogError(error);
                Pandora.Instance.ReportError(error);
            }
        }

        internal static void AndroidPayFinish(string jsonStr)
        {
            try
            {
                if(LuaStateManager.IsInitialized == true)
                {
                    Logger.Log("安卓米大师支付回调Lua层: " + jsonStr);
                    LuaStateManager.CallLuaFunction("Common.AndroidPayFinish", jsonStr);
                }
            }
            catch(Exception e)
            {
                string error = "安卓米大师支付回调Lua层发生错误\n" + e.Message;
                Logger.LogError(error);
                Pandora.Instance.ReportError(error);
            }
        }
        #region WebView
        public static void OpenPage(string zipFileName, string url, bool openInOtherProcess, string color, bool delayShow)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                // 本地url，补全路径
                url = GetFullUrl(zipFileName, url);
            }

            if (url != "")
            {
                Pandora.Instance.OpenPage(url, openInOtherProcess, color, delayShow);
            }
        }

        public static void ClosePage()
        {
            Pandora.Instance.ClosePage();
        }

        public static void ShowPage()
        {
            Pandora.Instance.ShowPage();
        }

        public static void HidePage()
        {
            Pandora.Instance.HidePage();
        }

        public static void WritePageMessage(string message)
        {
            Pandora.Instance.WritePageMessage(message);
        }
        
        public static void OnPageLoaded()
        {
            try
            {
                if (LuaStateManager.IsInitialized == true)
                {
                    LuaStateManager.CallLuaFunction("Common.OnPageLoaded");
                }
            }
            catch(Exception e)
            {
                Logger.LogError("WebView 页面加载Lua回调执行发生异常, " + e.Message);
            }
        }

        public static void OnPageMessage(string message)
        {
            try
            {
                if (LuaStateManager.IsInitialized == true)
                {
                    LuaStateManager.CallLuaFunction("Common.OnPageMessage", message);
                }
            }
            catch (Exception e)
            {
                Logger.LogError("WebView 页面消息回调异常, " + e.Message);
            }
        }

        public static void OnPageBack(string isDown)
        {
            try
            {
                if (LuaStateManager.IsInitialized == true)
                {
                    LuaStateManager.CallLuaFunction("Common.OnPageBack", isDown);
                }
            }
            catch (Exception e)
            {
                Logger.LogError("WebView 页面回退回调异常, " + e.Message);
            }
        }


        public static string GetFullUrl(string zipFileName, string relativeUrl)
        {
            try
            {
                // 文件在Assets\CACHE\Pandora\program中
                string fullUrl = CacheManager.GetCachedProgramAssetUrl(relativeUrl);
                string fullPath = CacheManager.GetCachedProgramAssetPath(relativeUrl);
                if (File.Exists(fullPath) == true)
                {
                    // html已经下载成功，返回路径
                    return fullUrl;
                }
                else
                {
                    // html下载失败，使用游戏预置的
                    fullUrl = LocalDirectoryHelper.GetStreamingAssetsUrl() + "/" + relativeUrl;
                    return fullUrl;
                }
            }
            catch (Exception e)
            {
                Logger.LogError("GetFullUrl Exception, " + e.Message);
                return "";
            }

            return "";
        }
        #endregion

    }

}
