#if UNITY_IPHONE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Msdk
{

	public class iOSConnector {

        [DllImport("__Internal")]
        public static extern void setBridge(MessageCenter.SendToUnity bridge);

		[DllImport("__Internal")]
		public static extern string GetLoginRecord();

		[DllImport("__Internal")]
		public static extern void Login(int platform);

		[DllImport("__Internal")]
		public static extern void ChannelPermissionAuth(int platform, string permissions);

		[DllImport("__Internal")]
		public static extern int LoginOpt(int platform,int overtime);

		[DllImport("__Internal")]
		public static extern void WGQrCodeLogin(int platform);

		[DllImport("__Internal")]
		public static extern bool Logout();

        [DllImport("__Internal")]
		public static extern bool WGCheckWXUniversalLink();

        [DllImport("__Internal")]
        public static extern void RealNameAuth(string infoStr);

		[DllImport("__Internal")]
		public static extern void SetPermission(int permissions);

		[DllImport("__Internal")]
		public static extern void SendToWeixin(
			string title,
			string desc,
			string mediaTagName,
			byte[] thumbImgData,
			int thumbImgDataLen,
			string messageExt,
            string userOpenId = ""
			);
			

		[DllImport("__Internal")]
		public static extern void SendToWeixinWithUrl(
			int scene,
			string title,
			string desc,
			string url,
			string mediaTagName,
			byte[] thumbImgData,
			int thumbImgDataLen,
			string messageExt,
            string userOpenId = ""
			);

		[DllImport("__Internal")]
		public static extern void SendToWXNativeGamePage(
            int scene,
            string title,
            string desc,
            string mediaTagName,
            byte[] thumbImgData,
            int thumbImgDataLen,
            bool isVideo,
            int videoDuration,
            string shareData,
            string messageExt,
			string messageAction,
			string userOpenId = ""
			);

		[DllImport("__Internal")]
		public static extern void SendToWeixinWithPhoto(
			int scene,
			string mediaTagName,
			byte[] imgData,
			int imgDataLen,
			string messageExt,
			string messageAction
			);

		[DllImport("__Internal")]
		public static extern void SendToQQWithCommonShare(
			string serviceID,
			string extraJson);

		[DllImport("__Internal")]
		public static extern void SendToWeixinWithVideo(
			int scene, 
			string title, 
			string desc, 
			/*string thumbUrl, 
			string videoUrl, */
			byte[] videoParamsData,
			int videoParamsLen,
			string mediaTagName, 
			string messageAction, 
			string messageExt);

		[DllImport("__Internal")]
		public static extern void SendToWXStateWithPhoto(string stateId,
														 string stateTitle,
														 byte[] imageData,
														 int imageDataLen,
														 string jumpUrl);

		[DllImport("__Internal")]
        public static extern void SendToWXStateWithPhotoOpenMiniApp(string stateId,
                                                         string stateTitle,
                                                         byte[] imageData,
														 int imageDataLen,
                                                         string userName,
                                                         string path,
                                                         int type);  

		[DllImport("__Internal")]
		public static extern void SendToWXChannelShareVideo(string localIdentify, string messageExt);

        [DllImport("__Internal")]
		public static extern void SendToWXChannelStartLive(string channelLiveJson, string messageExt);
		
		[DllImport("__Internal")]
		public static extern void FeedbackWithBody(string body);

		[DllImport("__Internal")]
		public static extern void EnableCrashReport(bool bRDMEnable, bool bMTAEnable);

		[DllImport("__Internal")]
		public static extern void ReportEvent(
			string name,
			string eventList,
			bool isRealTime
			);

		[DllImport("__Internal")]
		public static extern string GetVersion();

		[DllImport("__Internal")]
		public static extern string GetChannelId();

		[DllImport("__Internal")]
		public static extern string GetPlatformAPPVersion(int platform);

		[DllImport("__Internal")]
		public static extern string GetRegisterChannelId();

		[DllImport("__Internal")]
		public static extern void RefreshWXToken();

		[DllImport("__Internal")]
		public static extern bool IsPlatformInstalled(int platformType);

		[DllImport("__Internal")]
		public static extern string GetPfKey();

		[DllImport("__Internal")]
		public static extern void LogPlatformSDKVersion();

		[DllImport("__Internal")]
		public static extern bool QueryQQMyInfo();

        [DllImport("__Internal")]
        public static extern bool QueryAppleMyInfo();

        [DllImport("__Internal")]
		public static extern bool QueryQQGameFriendsInfo();

		[DllImport("__Internal")]
		public static extern bool QueryWXMyInfo();

		[DllImport("__Internal")]
		public static extern bool QueryWXGameFriendsInfo();

		[DllImport("__Internal")]
		public static extern void CreateWXGroup (string unionid, string chatRoomName, string chatRoomNickName);

		[DllImport("__Internal")]
		public static extern void JoinWXGroup (string unionid, string chatRoomNickName);

  //      [DllImport("__Internal")]
		//public static extern void JoinQQGroup (string groupNum, string groupKey);

		[DllImport("__Internal")]
		public static extern void QueryWXGroupInfo (string unionid, string openIdList);

		[DllImport("__Internal")]
		public static extern void SendToWXGroup (
			int msgType,
			int subType,
			string unionid,
			string title,
			string description,
			string messageExt,
			string mediaTagName,
			string imgUrl,
			string msdkExtInfo);

		[DllImport("__Internal")]
		public static extern bool SendToQQGameFriend(
			int act,
			string fopenid,
			string title,
			string summary,
			string targetUrl,
			string imgUrl,
			string previewText,
			string gameTag,
			string msdkExtInfo
			);

		[DllImport("__Internal")]
		public static extern void OpenWeiXinDeeplink (string link);

		[DllImport("__Internal")]
		public static extern bool SendToWXGameFriend(
			string fOpenId,
			string title,
			string description,
			string mediaId,
			string messageExt,
			string mediaTagName,
			string msdkExtInfo
			);

		[DllImport("__Internal")]
		//public static extern bool SendToWXWithMiniApp(
			//int scene, 
			//string title, 
			//string desc, 
			//byte[] thumbImgData, 
			//int thumbImgDataLen, 
			//string webpageUrl, 
			//string userName, 
			//string path, 
			//bool withShareTicket, 
			//string messageExt, 
			//string messageAction);
        public static extern void SendToWXWithMiniApp(
            int scene,
            string title,
            string desc,
            byte[] thumbImgData,
            int thumbImgDataLen,
            string webpageUrl,
            string userName,
            string path,
            bool withShareTicket,
            string messageExt,
            string messageAction,
            string mediaTagName = "",
            int type = 0,
            string userOpenId = "");
        
        [DllImport("__Internal")]
        public static extern void LaunchMiniApp(
            string userName,
            string path,
            int type);

		[DllImport("__Internal")]	
		public static extern void WGSendToWXGameLive(
            int scene,
            string title,
            string desc,
            string messageExt,
            string extInfo);


		[DllImport("__Internal")]
		public static extern void LoginWithLocalInfo();

		[DllImport("__Internal")]
		public static extern void ShowNotice(string scene);

		[DllImport("__Internal")]
		public static extern void HideScrollNotice();

		[DllImport("__Internal")]
		public static extern void OpenUrl(string openUrl, string algorithm = "v2");

		[DllImport("__Internal")]
		public static extern void OpenUrlWithScreenDir(string openUrl, int screenDir, string algorithm = "v2");

		[DllImport("__Internal")]
		public static extern void OpenUrlWithScreenDirAndFullScreen(string openUrl, int screenDir, bool fullScreen, string algorithm = "v2");

		[DllImport("__Internal")]
		public static extern string GetEncodeUrl (string openUrl, string algorithm = "v2");

		[DllImport("__Internal")]
		public static extern string GetNoticeData(string scene);

		[DllImport("__Internal")]
		public static extern void OpenAmsCenter(string gameName, string actChannelId, string zoneId, string platformId, string partitionId, string roleId, string extra, int screenDir);

		[DllImport("__Internal")]
		public static extern void GetNearbyPersonInfo();

		[DllImport("__Internal")]
		public static extern bool CleanLocation();

		[DllImport("__Internal")]
		public static extern bool GetLocationInfo();

		[DllImport("__Internal")]
		public static extern bool SendMessageToWechatGameCenter(
			string fOpenid,
			string title,
			string content,
			string pTypeInfo,
			string pButtonInfo,
			string msdkExtInfo
		);

		[DllImport("__Internal")]
		public static extern void SendToWeixinWithMusic(
			int scene,
			string title,
			string desc,
			string musicUrl,
			string musicDataUrl,
			string mediaTagName,
			byte[] imgData,
			int imgDataLen,
			string messageExt,
			string messageAction
			);

		[DllImport("__Internal")]
		public static extern void SendToQQWithMusic(
			int scene,
			string title,
			string desc,
			string musicUrl,
			string musicDataUrl,
			string imgUrl
			);

		[DllImport("__Internal")]
		public static extern bool SwitchUser(bool flag);


		[DllImport("__Internal")]
		public static extern void SendToQQ(
			int scene,
			string title,
			string desc,
			string url,
			byte[] imgData,
			int imgDataLen,
			string messageExt = "",
			string gameTag = ""
			);

		[DllImport("__Internal")]
		public static extern void SendToQQWithPhoto(
			int scene,
			byte[] imgData,
			int imgDataLen
			);

		[DllImport("__Internal")]
		public static extern void SendToQQWithPhotoWithParams(
			int scene,
			byte[] imgData,
			int imgDataLen,
			string extraScene,
			string messageExt,
			string gameTag = ""
			);
		[DllImport("__Internal")]
		public static extern string GetPf();

		[DllImport("__Internal")]
		public static extern int GetPaytokenValidTime();


		[DllImport("__Internal")]
		public static extern void OpenMSDKLog(bool enabled);

		//[DllImport("__Internal")]
		//public static extern void BindQQGroup(
           //string unionid,
           //string union_name,
           //string zoneid,
           //string signature);

		[DllImport("__Internal")]
		public static extern void AddGameFriendToQQ(
			string fopenid,
			string desc,
			string message);

		[DllImport("__Internal")]
		public static extern string GetGuestID();

		[DllImport("__Internal")]
		public static extern void ResetGuestID();

		[DllImport("__Internal")]
		public static extern void StartGameStatus (string cGameStatus);

		[DllImport("__Internal")]
		public static extern void EndGameStatus (string cGameStatus, int succ, int errorCode);

		[DllImport("__Internal")]
		public static extern void ClearLocalNotifications ();

		[DllImport("__Internal")]
		public static extern long AddLocalNotification(string localMsgJsonString);

		[DllImport("__Internal")]
		public static extern long AddLocalNotificationAtFront(string localMsgJsonString);

		[DllImport("__Internal")]
		public static extern void ClearLocalNotification(string localMsgJsonString);

		[DllImport("__Internal")]
		public static extern void SetPushTag (string tag);

		[DllImport("__Internal")]
		public static extern void DeletePushTag (string tag);

		[DllImport("__Internal")]
		public static extern void SetPushAccount (string account);

		[DllImport("__Internal")]
		public static extern void DeletePushAccount (string account);

		[DllImport("__Internal")]
		public static extern void UnregisterPush ();

        [DllImport("__Internal")]
        public static extern void BuglyLog (int level, string log);

		[DllImport("__Internal")]
        public static extern void CloseCrashReport();

		[DllImport("__Internal")]
		public static extern bool QueryWXGroupStatus(string unionid, int type);

		[DllImport("__Internal")]
		public static extern void CreateQQGroupV2(
			string guildId,
			string guildName,
            string leaderOpenId,
            string leaderRoleId,
            string leaderZoneId,
            string zoneId,
            string partition,
			string roleId,
            string roleName,
            string userZoneId,
            string userLabel,
            string nickName,
            string type,
            string areaId
		);

		[DllImport("__Internal")]
		public static extern void JoinQQGroupV2(
            string guildId,
            string guildName,
            string leaderOpenId,
            string leaderRoleId,
            string leaderZoneId,
            string zoneId,
            string partition,
            string roleId,
            string roleName,
            string userZoneId,
            string userLabel,
            string nickName,
            string type,
            string areaId,
            string groupId);

		[DllImport("__Internal")]
		public static extern void UnbindQQGroupV2(
            string guildId,
            string guildName,
            string leaderOpenId,
            string leaderRoleId,
            string leaderZoneId,
            string zoneId,
            string partition,
            string roleId,
            string roleName,
            string userZoneId,
            string userLabel,
            string nickName,
            string type,
            string areaId);

		[DllImport("__Internal")]
		public static extern void QueryQQGroupInfoV2(string groupId);

		[DllImport("__Internal")]
		public static extern void GetQQGroupCodeV2(
            string guildId,
            string guildName,
            string leaderOpenId,
            string leaderRoleId,
            string leaderZoneId,
            string zoneId,
            string partition,
            string roleId,
            string roleName,
            string userZoneId,
            string userLabel,
            string nickName,
            string type,
            string areaId);

		//[DllImport("__Internal")]
		//public static extern void QueryQQGroupKey(string groupId);

		[DllImport("__Internal")]
        public static extern void BindExistQQGroupV2(
            string guildId,
            string guildName,
            string leaderOpenId,
            string leaderRoleId,
            string leaderZoneId,
            string zoneId,
            string partition,
            string roleId,
            string roleName,
            string userZoneId,
            string userLabel,
            string nickName,
            string type,
            string areaId,
            string groupId,
            string groupName);

		[DllImport("__Internal")]
        public static extern void QueryBindGuildV2(string groupId, int type = 0);

		[DllImport("__Internal")]
		public static extern void GetQQGroupListV2();

		[DllImport("__Internal")]
		public static extern void RemindGuildLeaderV2(
            string guildId,
            string guildName,
            string leaderOpenId,
            string leaderRoleId,
            string leaderZoneId,
            string zoneId,
            string partition,
            string roleId,
            string roleName,
            string userZoneId,
            string userLabel,
            string nickName,
            string type,
            string areaId);

		[DllImport("__Internal")]
		public static extern void ShareToWXGameline(byte[] data, int lens, string gameExtra);

		[DllImport("__Internal")]
		public static extern void UnbindWeiXinGroup(string groupId);

		[DllImport("__Internal")]
		public static extern void ReportUnityData(string name, string jsonData);

		[DllImport("__Internal")]
		public static extern void OpenFullScreenWebViewWithJson(string jsonStr);

		[DllImport("__Internal")]
		public static extern void ReportPrajna(string serialNumber);

        [DllImport("__Internal")]
        public static extern void SendToQQWithArk(int scene, string title, string desc, string url, string imgUrl, string jsonString, string messageExt = "", string gameTag = "");

        [DllImport("__Internal")]
        public static extern void ReportException(int exception_type,
                               string exception_name,
                               string exception_msg,
                               string exception_stack,
                               string ext_info);

        [DllImport("__Internal")]
        public static extern void SendToQQWithRichPhoto(string summary, string imgParamsData, string extraScene, string messageExt);

        [DllImport("__Internal")]
        public static extern void QueryUnionID();


        [DllImport("__Internal")]
        public static extern void SendToQQWithText(string text, string extraScene, string messageExt);


        [DllImport("__Internal")]
        public static extern void RegisterAPNSPushNotification(string launchOptions);

        [DllImport("__Internal")]
        public static extern void SuccessedRegisterdAPNSWithToken(string deviceToken);

        [DllImport("__Internal")]
        public static extern void FailedRegisteredAPNS();

        [DllImport("__Internal")]
        public static extern void CleanBadgeNumber();

        [DllImport("__Internal")]
        public static extern void ReceivedMSGFromAPNS(string userInfo);

        [DllImport("__Internal")]
        public static extern int OpenEmbeddedWebView(string url);

        [DllImport("__Internal")]
        public static extern int OpenEmbeddedWebViews(string charset, string data, int lens);

        [DllImport("__Internal")]
        public static extern int CloseEmbeddedWebView();

        [DllImport("__Internal")]
        public static extern void CallToEmbeddedWebView(string embeddedParams);

        [DllImport("__Internal")]
        public static extern bool SetEmbeddedWebViewBackground(byte[] imageData, int lens);

        [DllImport("__Internal")]
        public static extern bool SendToWXWithOpenBusinessView(string businessType,
                                        string query,
                                        string extInfo,
                                        string extData);
        [DllImport("__Internal")]
		public static extern void GetCountryFromIP();

		[DllImport("__Internal")]
		public static extern void GetIPInfo();
		
        [DllImport("__Internal")]
        public static extern void LaunchQQMiniApp(
            string miniAppID,
            string miniPath,
            int type);

		
		[DllImport("__Internal")]
        public static extern void SendToQQWithMiniApp(
                int scene,
                string title,
                string desc,
                string url,
                byte[] imgData,
                int imgDataLen,
                string miniProgramAppid,
                string miniProgramPath,
                int type = 0);

        [DllImport("__Internal")]
        public static extern bool HandleOpenUrl(string urlStr);

        [DllImport("__Internal")]
        public static extern bool HandleOpenUniversalLink(string urlStr);

        [DllImport("__Internal")]
        public static  extern void SendToQQWithVideo(string summary, byte[] videoParamsData, int videoParamsLen, string extraScene, string messageExt);

		[DllImport("__Internal")]
        public static extern void SetCouldCollectSensitiveInfo(bool couldCollect);

		[DllImport("__Internal")]
		public static extern void SetCollectSensitiveInfo(string jsonInfo);
    
		[DllImport("__Internal")]
        public static extern void SetSensitiveInfo(string jsonInfo);

		[DllImport("__Internal")]
		public static extern void OpenGameDataAuthCenter(string openUrl, int screenDir, bool fullScreen, string algorithm);

		[DllImport("__Internal")]
		public static extern void OpenDeleteAccountUrl(int screenDir, bool isFullScreen, string algorithm);

    }

}
#endif
