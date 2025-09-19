using System;
using UnityEngine;

namespace Msdk
{
	public class MsdkEvent
	{
		private static MsdkEvent instance = null;

		private MsdkEvent() {}

		public static MsdkEvent Instance
		{
			get
			{
				if (instance == null) {
					instance = new MsdkEvent();
				}
				return instance;
			}
		}

		public event LoginDelegate LoginEvent;                // 登录回调
        public event ChannelPermissionAuthDelegate ChannelPermissionAuthEvent; //视频号授权回调
        public event CheckULDelegate CheckULEvent;            // 检查 UL 回调

        public event WakeupDelegate WakeupEvent;              // 第三方唤醒游戏的回调
		public event ShareDelegate ShareEvent;                // 分享回调
		public event RelationDelegate RelationEvent;          // 查询关系链回调
        public event NearbyDelegate NearbyEvent;              // 查询附近的人的回调
        public event LocateDelegate LocateEvent;              // 查询自己位置信息的回调
        public event FeedbackDelegate FeedbackEvent;          // 玩家反馈信息的回调
        public event CrashReportMessageDelegate CrashReportMessageEvent; // Crash时上报额外信息---字符串
        public event CrashReportDataDelegate CrashReportDataEvent;       // Crash时上报额外信息---字节数据
        public event ShowADDelegate ShowADEvent;              // 展示广告回调
        public event QueryGroupDelegate QueryGroupEvent;      // 查询群信息的回调
        public event CreateWXGroupDelegate CreateWXGroupEvent;// 加入微信群信息的回调
        public event JoinWXGroupDelegate JoinWXGroupEvent;    // 创建微信群信息的回调
        public event RealNameAuthDelegate RealNameAuthEvent;  // 实名认证回调
        public event JoinQQGroupDelegate JoinQQGroupEvent;    // 绑定QQ群回调
        public event WebviewDelegate WebviewEvent;            // 打开关闭浏览器回调
        public event QueryWXGroupStatusDelegate QueryWXGroupStatusEvent;// 查询微信群状态回调

        public event AddWXCardDelegate AddWXCardEvent;        // 微信插入卡卷的回调
        public event ADBackPressedDelegate ADBackPressedEvent;// 广告界面按返回键时的回调
        public event BindGroupDelegate BindGroupEvent;        // 绑定QQ群的回调
        public event UnbindGroupDelegate UnbindGroupEvent;    // 解绑QQ群的回调
        public event QueryQQGroupKeyDelegate QueryQQGroupKeyEvent;          // 查询QQ群Key的回调

        public event OnCreateGroupV2Delegate OnCreateGroupV2Event;//创建绑定群回调
        public event OnJoinGroupV2Delegate OnJoinGroupV2Event;
        public event OnQueryGroupInfoV2Delegate QueryGroupInfoV2Event;
        public event OnUnbindGroupV2Delegate UnbindGroupV2Event;
        public event OnGetGroupCodeV2Delegate GetGroupCodeV2Event;
        public event OnQueryBindGuildV2Delegate QueryBindGuildV2Event;
        public event OnBindExistGroupV2Delegate BindExistGroupV2Event;
        public event OnGetGroupListV2Delegate GetGroupListV2Event;
        public event OnRemindGuildLeaderV2Delegate RemindGuildLeaderV2Event;

        public event CheckUpdateDelegate CheckUpdateEvent;                  // 查询游戏更新信息
        public event DownloadAppProgressDelegate DownloadAppProgressEvent;  // 下载游戏的进度
        public event DownloadAppStateDelegate DownloadAppStateEvent;        // 下载游戏的状态
        public event DownloadYYBProgressDelegate DownloadYYBProgressEvent;  // 下载应用宝的进度
        public event DownloadYYBStateDelegate DownloadYYBStateEvent;        // 下载应用宝的状态

        public event OnJsCallbackDelegate OnJsCallbackEvent;                  // js调用的回调
        public event OnWebProgressChangedDelegate OnWebProgressChangedEvent;  // 浏览器打开界面的进度
        public event OnWebCloseDelegate OnWebCloseEvent;                      // 浏览器关闭的回调

        public event OnLocationGotCountryFromIPDelegate OnLocationGotCountryFromIPEvent;    // 根据 IP 获取当前玩家的国籍信息
        public event OnLocationGetIPInfoDelegate OnLocationGetIPInfoEvent;      //根据 IP 获取当前玩家的国家及省市信息

        public event OnReceiveNotificationResponseDelegate ReceiveNotificationResponseEvent; // TPNS点击消息的回调





        public void HandleLoginNotify(LoginRet ret)
		{
            try {
                if (LoginEvent != null) {
                    LoginEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}

        public void HandleChannelPermissionAuthNotify(LoginRet ret)
        {
            try
            {
                if (ChannelPermissionAuthEvent != null)
                {
                    ChannelPermissionAuthEvent(ret);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void HandleCheckULNotify(CheckULRet ret)
        {
            MsdkUtil.Log("HandleCheckULNotify ");
            try
            {
                if (CheckULEvent != null)
                {
                    MsdkUtil.Log("CheckULEvent");
                    CheckULEvent(ret);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void HandleWakeupNofify(WakeupRet ret)
		{
            try {
                if (WakeupEvent != null) {
                    WakeupEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}

		public void HandleShareNofify(ShareRet ret)
		{
            try {
                if (ShareEvent != null) {
                    ShareEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}

		public void HandleRelationNofify(RelationRet ret)
		{
            try {
                if (RelationEvent != null) {
                    RelationEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}

		public void HandleLocationNotify(RelationRet ret)
		{
            try {
                if (NearbyEvent != null) {
                    NearbyEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}

		public void HandleLocationGotNotify(LocationRet ret)
		{
            try {
                if (LocateEvent != null) {
                    LocateEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}

        // 根据 IP 获取当前玩家的国籍信息
        public void HandleLocationGotCountryFromIP(CountryFromIPRet ret)
        {
            try {
                if (OnLocationGotCountryFromIPEvent != null) {
                    OnLocationGotCountryFromIPEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError (e.Message);
            }
        }

        //根据 IP 获取当前玩家的国家及省市信息
        public void HandleGetIPInfo(GetIPInfoRet ret)
        {
            try {
                if (OnLocationGetIPInfoEvent != null) {
                    OnLocationGetIPInfoEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError (e.Message);
            }
        }

		public void HandleFeedbackNotify(int flag, String desc)
        {
            try {
                if (FeedbackEvent != null) {
                    FeedbackEvent(flag, desc);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        public String HandleCrashMessageNotify()
        {
            try {
                if (CrashReportMessageEvent != null) {
                    return CrashReportMessageEvent();
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
            return "";
        }

        public byte[] HandleCrashDataNotify()
        {
            try {
                if (CrashReportDataEvent != null) {
                    return CrashReportDataEvent();
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
            return null;
        }

        public void HandleShowADNotify(ADRet ret)
        {
            try {
                if (ShowADEvent != null) {
                    ShowADEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

		public void HandleQueryGroupInfoNotify(GroupRet ret)
        {
            try {
                if (QueryGroupEvent != null) {
                    QueryGroupEvent(ret);
                } 
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

		public void HandleJoinWXGroupNotify(GroupRet ret)
        {
            try {
                if (JoinWXGroupEvent != null) {
                    JoinWXGroupEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

		public void HandleCreateWXGroupNotify(GroupRet ret)
        {
            try {
                if (CreateWXGroupEvent != null) {
                    CreateWXGroupEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        public void HandleRealNameAuthNotify(RealNameAuthRet ret)
        {
            try {
                if (RealNameAuthEvent != null) {
                    RealNameAuthEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        public void HandleJoinQQGroupNotify(GroupRet ret) 
        {
            try
            {
                if (JoinQQGroupEvent != null) {
                    JoinQQGroupEvent(ret);
                }
            }
            catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }
        public void HandleQueryWXGroupStatusNotify(GroupRet ret)
        {
            try
            {
                if (QueryWXGroupStatusEvent != null)
                {
                    QueryWXGroupStatusEvent(ret);
                }
            }
            catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }
        public void HandleWebviewNotify(WebviewRet ret)
        {
            try
            {
                if (WebviewEvent != null)
                {
                    WebviewEvent(ret);
                }
            }
            catch (Exception e) {
                Debug.LogError(e.Message);
            }
        } 


        public void HandleJsCallbackEvent(string jsParams) // js调用的回调 
        {
            try
            {
                if (OnJsCallbackEvent != null)
                {
                    OnJsCallbackEvent(jsParams);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }


        public void HandleWebProgressChanged(string url, float progress) // 浏览器打开界面的进度
        {
            try
            {
                if (OnWebProgressChangedEvent != null)
                {
                    OnWebProgressChangedEvent(url, progress);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void HandleWebClose() // 浏览器关闭的回调
        {
            try
            {
                if (OnWebCloseEvent != null)
                {
                    OnWebCloseEvent();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }



        #region Android 特有回调
		public void HandleAddWXCardNotify(CardRet ret)
		{
            try {
                if (AddWXCardEvent != null) {
                    AddWXCardEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}

        public void HandleADBackPressedNotify(ADRet ret)
        {
            try {
                if (ADBackPressedEvent != null) {
                    ADBackPressedEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

		public void HandleBindGroupNotify(GroupRet ret)
		{
            try {
                if (BindGroupEvent != null) {
                    BindGroupEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}
		
		public void HandleUnbindGroupNotify(GroupRet ret)
		{
            try {
                if (UnbindGroupEvent != null) {
                    UnbindGroupEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}

		public void HandleQueryGroupKeyNotify(GroupRet ret)
		{
            try {
                if (QueryQQGroupKeyEvent != null) {
                    QueryQQGroupKeyEvent(ret);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}


        public void HandleCreateGroupV2Notify(GroupRet ret)
        {
            try{
                if (OnCreateGroupV2Event != null) {
                    OnCreateGroupV2Event(ret);
                }
            }
            catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        public void HandleJoinGroupV2Notify(GroupRet ret)
        {
            try
            {
                if (OnJoinGroupV2Event != null)
                {
                    OnJoinGroupV2Event(ret);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        public void HandleBindExistGroupV2Notify(GroupRet ret)
        {
            try
            {
                if (BindExistGroupV2Event != null)
                {
                    BindExistGroupV2Event(ret);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        public void HandleQueryGroupInfoV2Notify(GroupRet ret)
        {
            try
            {
                if (QueryGroupInfoV2Event != null)
                {
                    QueryGroupInfoV2Event(ret);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void HandleUnbindGroupV2Notify(GroupRet ret)
        {
            try
            {
                if (UnbindGroupV2Event != null)
                {
                    UnbindGroupV2Event(ret);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void HandleGetGroupCodeV2Notify(GroupRet ret)
        {
            try
            {
                if (GetGroupCodeV2Event != null)
                {
                    GetGroupCodeV2Event(ret);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void HandleQueryBindGuildV2Notify(GroupRet ret)
        {
            try
            {
                if (QueryBindGuildV2Event != null)
                {
                    QueryBindGuildV2Event(ret);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }


        public void HandleGetGroupListV2Notify(GroupRet ret)
        {
            try
            {
                if (GetGroupListV2Event != null)
                {
                    GetGroupListV2Event(ret);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void HandleRemindGuildLeaderV2Notify(GroupRet ret)
        {
            try
            {
                if (RemindGuildLeaderV2Event != null)
                {
                    RemindGuildLeaderV2Event(ret);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

		public void HandleCheckNeedUpdateInfo(long newApkSize, string newFeature, long patchSize,
				int status, string updateDownloadUrl, int updateMethod)
		{
            try {
                if (CheckUpdateEvent != null) {
                    CheckUpdateEvent(newApkSize, newFeature, patchSize, status, updateDownloadUrl, updateMethod);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}

		public void HandleDownloadAppProgressChanged(long receiveDataLen, long totalDataLen)
		{
            try {
                if (DownloadAppProgressEvent != null) {
                    DownloadAppProgressEvent(receiveDataLen, totalDataLen);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}

		public void HandleDownloadAppStateChanged(int state, int errorCode, string errorMsg)
		{
            try {
                if (DownloadAppStateEvent != null) {
                    DownloadAppStateEvent(state, errorCode, errorMsg);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}

		public void HandleDownloadYYBProgressChanged(string url, long receiveDataLen, long totalDataLen)
		{
            try {
                if (DownloadYYBProgressEvent != null) {
                    DownloadYYBProgressEvent(url, receiveDataLen, totalDataLen);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}

		public void HandleDownloadYYBStateChanged(string url, int state, int errorCode, string errorMsg)
		{
            try {
                if (DownloadYYBStateEvent != null) {
                    DownloadYYBStateEvent(url, state, errorCode, errorMsg);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
		}

        #endregion

        #region ios特有回调
        public void HandleReceiveNotificationResponse(string userInfo)
        {
            try {
                if (ReceiveNotificationResponseEvent != null) {
                    ReceiveNotificationResponseEvent(userInfo);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }
        #endregion
	}
}
