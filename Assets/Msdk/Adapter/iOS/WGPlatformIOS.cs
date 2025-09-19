#if UNITY_IPHONE
using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;
using Msdk;

namespace Msdk
{

	public class WGPlatformIOS : WGPlatformUnity, IMsdk
	{

		// MSDK Unity 初始化
		public new void Init ()
		{
			base.Init();
            // 注册C++回调
            iOSConnector.setBridge(MessageCenter.MessageConsumer);
			reportData ();
		}

#region 登录相关
		public void WGLogin (ePlatform platform)
		{
			iOSConnector.Login ((int) platform);
		}

        public void WGChannelPermissionAuth(ePlatform platform, string permissions)
        {
            iOSConnector.ChannelPermissionAuth ((int) platform, permissions);
        }

		public int WGLoginOpt(ePlatform platform, int overtime)
		{
			return iOSConnector.LoginOpt ((int)platform, overtime);
		}
		public void WGQrCodeLogin (ePlatform platform)
		{
			iOSConnector.WGQrCodeLogin ((int) platform);
		}

		public bool WGLogout ()
		{
			return iOSConnector.Logout ();
		}

        public void WGCheckWXUniversalLink()
        {
            iOSConnector.WGCheckWXUniversalLink();
        }

        public LoginRet WGGetLoginRecord ()
		{
			string loginRetStr = iOSConnector.GetLoginRecord();
			if (loginRetStr != null)
			{
				return LoginRet.ParseJson(loginRetStr);
			}
			else
			{
				return new LoginRet();
			}

		}

		public bool WGSwitchUser (bool flag)
		{
			return iOSConnector.SwitchUser (flag);
		}

		public void WGSetPermission (int permissions)
		{
			iOSConnector.SetPermission (permissions);
		}

		public void WGRefreshWXToken ()
		{
			iOSConnector.RefreshWXToken ();
		}

        public void WGRealNameAuth(RealNameAuthInfo info)
        {
            string authInfoJsonStr = JsonMapper.ToJson(info);
            iOSConnector.RealNameAuth(authInfoJsonStr);
        }
#endregion

#region QQ相关
		public void WGSendToQQ (eQQScene scene, string title, string desc, string url, byte[] imgData, int imgDataLen, string messageExt, string gameTag)
		{
			iOSConnector.SendToQQ ((int)scene, title, desc, url, imgData, imgDataLen, messageExt, gameTag);
		}

		public void WGSendToQQWithPhoto (eQQScene scene, byte[] imgData, int imgDataLen)
		{
			iOSConnector.SendToQQWithPhoto ((int)scene, imgData, imgDataLen);
		}

        public void WGSendToQQWithCommonShare(string serviceID, string extraJson)
        {
            iOSConnector.SendToQQWithCommonShare(serviceID, extraJson);
        }

		public void WGSendToQQWithMusic (eQQScene scene, string title, string desc, string musicUrl, string musicDataUrl, string imgUrl)
		{
			iOSConnector.SendToQQWithMusic((int) scene, title, desc, musicUrl, musicDataUrl, imgUrl);
		}

		public bool WGSendToQQGameFriend (int act, string fopenid, string title, string summary, string targetUrl, string imgUrl, string previewText, string gameTag, string msdkExtInfo)
		{
			return iOSConnector.SendToQQGameFriend (act, fopenid, title, summary, targetUrl, imgUrl, previewText, gameTag, msdkExtInfo);
		}

		public bool WGQueryQQMyInfo ()
		{
			return iOSConnector.QueryQQMyInfo ();
		}

        public bool WGQueryAppleMyInfo()
        {
            return iOSConnector.QueryAppleMyInfo();
        }

        public bool WGQueryQQGameFriendsInfo ()
		{
			return iOSConnector.QueryQQGameFriendsInfo ();
		}

		//public void WGBindQQGroup (string unionid, string union_name, string zoneid, string signature)
		//{
		//	iOSConnector.BindQQGroup (unionid, union_name, zoneid, signature);
		//}

        //public void WGJoinQQGroup(string groupNum, string groupKey)
        //{
        //    iOSConnector.JoinQQGroup (groupNum, groupKey);
        //}

		public void WGAddGameFriendToQQ (string fopenid, string desc, string message)
		{
			iOSConnector.AddGameFriendToQQ (fopenid, desc, message);
		}
		public void WGCreateQQGroupV2(GameGuild gameGuild)
		{
            iOSConnector.CreateQQGroupV2(
                gameGuild.guildId, 
                gameGuild.guildName,
                gameGuild.leaderOpenId,
                gameGuild.leaderRoleId,
                gameGuild.leaderZoneId, 
                gameGuild.zoneId,
                gameGuild.partition, 
                gameGuild.roleId,
                gameGuild.roleName, 
                gameGuild.userZoneId,
                gameGuild.userLabel,
                gameGuild.nickName,
                gameGuild.type,
                gameGuild.areaId);
		}

        /**
         *
         *加入手q群
         *GameGuild：
         * guildId：公会id（必填）
         * zoneId:区服ID（必填）
         * roleId:角色ID（必填）
         * partition：小区区服ID，可不填（选填）
         * userZoneId：用户的区服ID，会长可能转让给非本区分的人，所以公会区服不一定是用户区服。（选填）
         * userLabel：修改群名片，不填为不修改群名片，规则"【YYYY】zzzz" YYYY指用户的游戏数据，zzzz指用户游戏内的昵称（选填）
         * nickName：用户昵称（选填）
         * type："0"公会，"1"队伍，"2"赛事（选填）
         * areaId:游戏大区ID，"1"qq,"2"微信 （选填）
         *
         * groupId：手q群ID（原gc）
         */
		public void WGJoinQQGroupV2(GameGuild gameGuild,string groupId)
		{
        iOSConnector.JoinQQGroupV2(
                gameGuild.guildId,
                gameGuild.guildName,
                gameGuild.leaderOpenId,
                gameGuild.leaderRoleId,
                gameGuild.leaderZoneId,
                gameGuild.zoneId,
                gameGuild.partition,
                gameGuild.roleId,
                gameGuild.roleName,
                gameGuild.userZoneId,
                gameGuild.userLabel,
                gameGuild.nickName,
                gameGuild.type,
                gameGuild.areaId,
                groupId);
		}


		public void WGUnbindQQGroupV2(GameGuild gameGuild)
		{
            iOSConnector.UnbindQQGroupV2(
                gameGuild.guildId,
                gameGuild.guildName,
                gameGuild.leaderOpenId,
                gameGuild.leaderRoleId,
                gameGuild.leaderZoneId,
                gameGuild.zoneId,
                gameGuild.partition,
                gameGuild.roleId,
                gameGuild.roleName,
                gameGuild.userZoneId,
                gameGuild.userLabel,
                gameGuild.nickName,
                gameGuild.type,
                gameGuild.areaId);
		}

		public void WGQueryQQGroupInfoV2(string groupId)
		{
			iOSConnector.QueryQQGroupInfoV2(groupId);
		}

		//public void WGQueryQQGroupKey(string groupOpenId)
		//{
		//	iOSConnector.QueryQQGroupKey(groupOpenId);
		//}

        /**
         *
         *绑定已存在的手q群
         *GameGuild：
         * guildId：公会id（必填）
         * zoneId:区服ID（必填）
         * roleId:角色ID（必填）
         * userZoneId：用户的区服ID，会长可能转让给非本区分的人，所以公会区服不一定是用户区服。（选填）
         * type："0"公会，"1"队伍，"2"赛事（选填）
         * areaId:游戏大区ID，"1"qq,"2"微信 （选填）
         *
         * groupId：手q群ID（原gc）
         * groupName:手q群名称
         */
		public void WGBindExistQQGroupV2(GameGuild gameGuild, string groupId, string groupName)
		{
            iOSConnector.BindExistQQGroupV2(
                gameGuild.guildId,
                gameGuild.guildName,
                gameGuild.leaderOpenId,
                gameGuild.leaderRoleId,
                gameGuild.leaderZoneId,
                gameGuild.zoneId,
                gameGuild.partition,
                gameGuild.roleId,
                gameGuild.roleName,
                gameGuild.userZoneId,
                gameGuild.userLabel,
                gameGuild.nickName,
                gameGuild.type,
                gameGuild.areaId,
                groupId,
                groupName);
		}

		public void WGGetQQGroupCodeV2(GameGuild gameGuild)
		{
			iOSConnector.GetQQGroupCodeV2(
                gameGuild.guildId,
                gameGuild.guildName,
                gameGuild.leaderOpenId,
                gameGuild.leaderRoleId,
                gameGuild.leaderZoneId,
                gameGuild.zoneId,
                gameGuild.partition,
                gameGuild.roleId,
                gameGuild.roleName,
                gameGuild.userZoneId,
                gameGuild.userLabel,
                gameGuild.nickName,
                gameGuild.type,
                gameGuild.areaId);
		}

		public void WGQueryBindGuildV2(string groupId, int type = 0)
		{
			iOSConnector.QueryBindGuildV2(groupId, type);
		}

		public void WGGetQQGroupListV2()
		{
			iOSConnector.GetQQGroupListV2();
		}
		public void WGRemindGuildLeaderV2(GameGuild gameGuild)
		{
			iOSConnector.RemindGuildLeaderV2(
                gameGuild.guildId,
                gameGuild.guildName,
                gameGuild.leaderOpenId,
                gameGuild.leaderRoleId,
                gameGuild.leaderZoneId,
                gameGuild.zoneId,
                gameGuild.partition,
                gameGuild.roleId,
                gameGuild.roleName,
                gameGuild.userZoneId,
                gameGuild.userLabel,
                gameGuild.nickName,
                gameGuild.type,
                gameGuild.areaId);
		}

        public void WGSendToQQWithArk(eQQScene scene, string title, string desc, string url, string imgUrl, string jsonString, string messageExt, string gameTag)
        {
            iOSConnector.SendToQQWithArk((int)scene, title, desc, url, imgUrl, jsonString, messageExt, gameTag);
        }


        public void WGSendToQQWithRichPhoto(string summary, ArrayList imgParams, string extraScene, string messageExt)
        {
            // 在这里拆分结构体
            //List<string> iosImgData_list = new List<string>();
            //List<int> iosImgDataLen_list = new List<int>();
            //Debug.Log("WGPlatformAndroid 在这个拆分结构体");
            //for (int i = 0; i < imgParams.Count; i++)
            //{
            //    ImageParams tmp_ImageParams = new ImageParams();
            //    tmp_ImageParams.ios_imageData = imgParams[i].ios_imageData;
            //    tmp_ImageParams.ios_imageDataLen = imgParams[i].ios_imageDataLen;
            //    iosImgData_list.Add(tmp_ImageParams.ios_imageData.ToString());
            //    iosImgDataLen_list.Add(tmp_ImageParams.ios_imageDataLen);
            //    Debug.Log("androidPath_list [" + i + "] " + iosImgData_list[i]);
            //}
            //iOSConnector.SendToQQWithRichPhoto(summary, iosImgData_list, iosImgDataLen_list, extraScene, messageExt);

            List<string> tmp_imgData = new List<string>();
            List<string> tmp_imgDataLen = new List<string>();
            List<string> imgP = new List<string>();
            for (int i = 0; i < imgParams.Count; i++)
            {
                //tmp_imgData.Add(((ImageParams)imgParams[i]).ios_imageData.ToString());
                //tmp_imgDataLen.Add(((ImageParams)imgParams[i]).ios_imageDataLen.ToString());
                // 如果通过base64 传递就不需要传 lenth了
                string base64_imgData = Convert.ToBase64String(((ImageParams)imgParams[i]).ios_imageData);
                tmp_imgData.Add(base64_imgData);
            }


            // 把 ios_imageData 放入 write中存储传到适配层
            StringBuilder sb_Data = new StringBuilder();
            JsonWriter writer_Data = new JsonWriter(sb_Data);
            writer_Data.WriteArrayStart();
            foreach (string imgD in tmp_imgData)
            {
                writer_Data.Write(imgD);
            }
            writer_Data.WriteArrayEnd();

            //// 把 ios_imageDataLen 放入 write中存储传到适配层
            //StringBuilder sb_DataLen = new StringBuilder();
            //JsonWriter writer_DataLen = new JsonWriter(sb_DataLen);
            //writer_DataLen.WriteArrayStart();
            //foreach (string imgD in tmp_imgData)
            //{
            //    writer_DataLen.Write(imgD);
            //}
            //writer_DataLen.WriteArrayEnd();


            Debug.Log("输出 richPhoto img:" + sb_Data);
            iOSConnector.SendToQQWithRichPhoto(summary, sb_Data.ToString(), extraScene, messageExt);
        }

        public void WGSendToQQWithText(string text, string extraScene, string messageExt)
        {
            iOSConnector.SendToQQWithText(text, extraScene, messageExt);
        }


        public void WGQueryUnionID()
        {
            iOSConnector.QueryUnionID();
        }

        /**
         * 分享视频到空间
         * @param summary 分享的正文
         * @param videoParams 分享的视频路径，Android支持本地地址，iOS支持视频数据
         * @param extraScene 区分分享的场景，用于异化小尾巴展示和feeds点击行为，支持iOS
         * @param messageExt 游戏自定义字段，点击分享消息回到游戏时会透传回游戏，不需要的话可以填写空串，支持iOS
         * 注意: 此功能只在手Q5.9.5及其以上版本支持
         */
        public void WGSendToQQWithVideo(string summary, VideoParams videoParams, string extraScene, string messageExt)
        {
            iOSConnector.SendToQQWithVideo(summary, videoParams.ios_videoData, videoParams.ios_videoDataLen, extraScene, messageExt);
        }

#endregion QQ相关

#region 微信相关
        public void WGSendToWeixin (string title, string desc, string mediaTagName, byte[] thumbImgData, int thumbImgDataLen, string messageExt,string userOpenId)
		{
            iOSConnector.SendToWeixin (title, desc, mediaTagName, thumbImgData, thumbImgDataLen, messageExt, userOpenId);
		}

		public void WGSendToWeixinWithUrl (eWechatScene scene, string title, string desc, string url, string mediaTagName, byte[] thumbImgData, int thumbImgDataLen, string messageExt, string userOpenId)
		{
            iOSConnector.SendToWeixinWithUrl ((int)scene, title, desc, url, mediaTagName, thumbImgData, thumbImgDataLen, messageExt, userOpenId);
		}

        public void WGSendToWXNativeGamePage(eWechatScene scene, string title, string desc, string mediaTagName, byte[] thumbImgData, int thumbImgDataLen, bool isVideo, int videoDuration, string shareData, string messageExt, string messageAction, string userOpenId)
        {
            iOSConnector.SendToWXNativeGamePage((int)scene, title, desc, mediaTagName, thumbImgData, thumbImgDataLen, isVideo, videoDuration, shareData, messageExt, messageAction, userOpenId);
        }

		public void WGSendToWeixinWithPhoto (eWechatScene scene, string mediaTagName, byte[] imgData, int imgDataLen, string messageExt, string messageAction)
		{
			iOSConnector.SendToWeixinWithPhoto ((int)scene, mediaTagName, imgData, imgDataLen, messageExt, messageAction);
		}

		public void WGSendToWeixinWithMusic (eWechatScene scene, string title, string desc, string musicUrl, string musicDataUrl, string mediaTagName, byte[] imgData, int imgDataLen, string messageExt, string messageAction)
		{
			iOSConnector.SendToWeixinWithMusic ((int)scene, title, desc, musicUrl, musicDataUrl, mediaTagName, imgData, imgDataLen, messageExt, messageAction);
		}

		public void WGSendToWeixinWithVideo(eWechatScene scene, string title, string desc,/* string thumbUrl, string videoUrl, */VideoParams videoParams, string mediaTagName, string messageAction, string messageExt)
		{
			iOSConnector.SendToWeixinWithVideo((int) scene, title, desc,/* thumbUrl, videoUrl,*/ videoParams.ios_videoData,videoParams.ios_videoDataLen, mediaTagName, messageAction, messageExt);
		}

        /**
        *
        * 分享图片信息至微信状态
        * 参数说明：
        * stateId：状态ID，选填，不同的状态ID会显示不同的Icon
        * stateTitle：状态标题，选填，限制32 个字符以内
        * imageParams：图片信息，必填，图片比例为9:16
        * 此struct在android、ios上对应的字段不同。android需要填写图片路径：android_imagePath，ios需要自行读取填写图片数据ios_imageData和图片数据长度ios_imageDataLen
        * jumpUrl：跳转URL，必填，点击来源小尾巴后跳转的页面url，游戏先支持跳到“微信游戏圈”页面
        *
        */
        public void WGSendToWXStateWithPhoto(string stateId,
                                             string stateTitle,
                                             ImageParams imageParams,
                                             string jumpUrl)
        {
            iOSConnector.SendToWXStateWithPhoto(stateId, stateTitle, imageParams.ios_imageData, imageParams.ios_imageDataLen, jumpUrl);
        }

        public void WGSendToWXStateWithPhotoOpenMiniApp(string stateId,
                                                        string stateTitle,
                                                        ImageParams imageParams,
                                                        string userName,
                                                        string path,
                                                        eMiniProgramType type)
        {
               iOSConnector.SendToWXStateWithPhotoOpenMiniApp(stateId, stateTitle, imageParams.ios_imageData, imageParams.ios_imageDataLen,
                                                                userName, path, (int)type);                                             
        }

        /**
        *
        * 分享视频至微信视频号
        * 参数说明：
        * videoParams：视频信息，必填，视频格式目前仅支持MP4，时长不超过30分钟，大小不超过450M，宽高比在1:3 - 3:1之间
        * 此struct在android、ios上对应的字段不同。android需要填写视频路径：android_videoPath，ios需要填写系统相册中视频标识符PHAsset localIdentifier：ios_videoLocalIdentifier
        * messageExt：拓展字段，选填，携带额外信息，当前版本暂未使⽤
        *
        */
        public void WGSendToWXChannelShareVideo(VideoParams videoParams, string messageExt)
        {
            iOSConnector.SendToWXChannelShareVideo(videoParams.ios_videoLocalIdentifier, messageExt);
        }

        /**
         * 微信视频号直播
         * 参数说明：
         * channelLiveJson 游戏拉起视频号直播透传json数据，包括liveJsonInfo和openID字段
         * messageExt 拓展字段，选填，携带额外信息，当前版本暂未使⽤
         */
        public void WGSendToWXChannelStartLive(string channelLiveJson, string messageExt)
        {
            iOSConnector.SendToWXChannelStartLive(channelLiveJson, messageExt);
        }

		public bool WGSendToWXGameFriend (string fOpenId, string title, string description, string mediaId, string messageExt, string mediaTagName, string msdkExtInfo)
		{
			return iOSConnector.SendToWXGameFriend (fOpenId, title, description, mediaId, messageExt, mediaTagName, msdkExtInfo);
		}

		//public void WGSendToWXWithMiniApp(eWechatScene scene, string title, string desc, byte[] thumbImgData, int thumbImgDataLen, string webpageUrl, string userName, string path, bool withShareTicket, string messageExt, string messageAction)
		//{
		//	iOSConnector.SendToWXWithMiniApp((int)scene,title,desc, thumbImgData, thumbImgDataLen, webpageUrl, userName, path, withShareTicket, messageExt, messageAction);
		//}

        public void WGSendToWXWithMiniApp(eWechatScene scene, string title, string desc, byte[] thumbImgData, int thumbImgDataLen, string webpageUrl, string userName, string path, bool withShareTicket, string messageExt, string messageAction, string mediaTagName, eMiniProgramType type, string userOpenId)
        {
            iOSConnector.SendToWXWithMiniApp((int)scene, title, desc, thumbImgData, thumbImgDataLen, webpageUrl, userName, path, withShareTicket, messageExt, messageAction, mediaTagName, (int)type, userOpenId);
        }

        public void WGLaunchMiniApp(string userName, string path, eMiniProgramType type)
        {
            iOSConnector.LaunchMiniApp(userName, path, (int)type);
        }

        

        /**
         *
         * 微信OpenBusinessView分享功能
         * @param businessType 业务内容 必填
         * @param query 业务参数 选填
         * @param extInfo 额外信息(Json格式)
         * @param extData iOS专用，指iOS本地沙盒路径视频文件的地址
         */
        public void WGSendToWXWithOpenBusinessView(string businessType,
                                        string query,
                                        string extInfo,
                                        string extData)
        {
            iOSConnector.SendToWXWithOpenBusinessView(businessType, query, extInfo, extData);
        }

        /**
         * 区别于小程序，微信提供基于原生的游戏直播/围观功能
         * @param scene 朋友圈或者好友列表
         * @param title 标题
         * @param desc 描述
         * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
         * @param extInfo  直播/围观功能的专用参数 
        */
        public void WGSendToWXGameLive(
            eWechatScene scene,
            string title,
            string desc,
            string messageExt,
            Dictionary<string, string> extInfo) {
            JsonData json = new JsonData ();
	        foreach (string key in extInfo.Keys) {
		        json[key] = extInfo[key];
	        }
	        string jsonData = json.ToJson ();
            Debug.Log("WGSendToWXGameLive extInfo to json : " + jsonData);
            iOSConnector.WGSendToWXGameLive((int)scene, title, desc, messageExt, jsonData);
        }

        public bool WGSendMessageToWechatGameCenter (string fOpenid, string title, string content, WXMessageTypeInfo pInfo, WXMessageButton pButton, string msdkExtInfo)
		{
			string pInfoJsonString = JsonMapper.ToJson (pInfo);
			string pButtonJsonString = JsonMapper.ToJson (pButton);
			return iOSConnector.SendMessageToWechatGameCenter (fOpenid, title, content, pInfoJsonString, pButtonJsonString, msdkExtInfo);
		}

		public bool WGQueryWXMyInfo ()
		{
			return iOSConnector.QueryWXMyInfo ();
		}

		public bool WGQueryWXGameFriendsInfo ()
		{
			return iOSConnector.QueryWXGameFriendsInfo ();
		}

		public void WGCreateWXGroup (string unionid, string chatRoomName, string chatRoomNickName)
		{
			iOSConnector.CreateWXGroup (unionid, chatRoomName, chatRoomNickName);
		}

		public void WGJoinWXGroup (string unionid, string chatRoomNickName)
		{
			iOSConnector.JoinWXGroup (unionid, chatRoomNickName);
		}

		public void WGQueryWXGroupInfo (string unionid, string openIdList)
		{
			iOSConnector.QueryWXGroupInfo (unionid, openIdList);
		}

		public void WGSendToWXGroup (int msgType, int subType, string unionid, string title, string description, string messageExt, string mediaTagName, string imgUrl, string msdkExtInfo)
		{
			iOSConnector.SendToWXGroup (msgType, subType, unionid, title, description, messageExt, mediaTagName, imgUrl, msdkExtInfo);
		}
		public void WGQueryWXGroupStatus (string unionid, eStatusType opType)
		{
			iOSConnector.QueryWXGroupStatus (unionid, (int)opType);
		}
		public void WGOpenWeiXinDeeplink (string link)
		{
			iOSConnector.OpenWeiXinDeeplink (link);
		}

		public void WGShareToWXGameline(byte[] data, string gameExtra)
		{
			int lens = 0;
			if (data != null) {
				lens = data.Length;
			}
			iOSConnector.ShareToWXGameline(data, lens, gameExtra);
		}

		public void WGUnbindWeiXinGroup(string unionid)
		{
			iOSConnector.UnbindWeiXinGroup(unionid);
		}
#endregion 微信相关

#region 通用接口
		public void WGFeedback (string body)
		{
			iOSConnector.FeedbackWithBody (body);
		}

		public void WGEnableCrashReport (bool bRDMEnable, bool bMTAEnable)
		{
			iOSConnector.EnableCrashReport (bRDMEnable, bMTAEnable);
		}

		public void WGReportEvent (string name, Dictionary<string, string> eventList, bool isRealTime)
		{
			try
			{
				JsonData json = new JsonData ();
				foreach (string key in eventList.Keys)
				{
					json[key] = eventList[key];
				}
				string jsonString = json.ToJson ();
				iOSConnector.ReportEvent (name, jsonString, isRealTime);
			}
			catch
			{
				Debug.LogError ("解析json失败，具体请看log日志");
			}
		}

		public string WGGetVersion ()
		{
			return iOSConnector.GetVersion ();
		}

		public string WGGetChannelId ()
		{
			return iOSConnector.GetChannelId ();
		}

		public string WGGetPlatformAPPVersion (ePlatform platform)
		{
			return iOSConnector.GetPlatformAPPVersion ((int) platform);
		}

		public string WGGetRegisterChannelId ()
		{
			return iOSConnector.GetRegisterChannelId ();
		}

		public bool WGIsPlatformInstalled (ePlatform platformType)
		{
			return iOSConnector.IsPlatformInstalled ((int)platformType);
		}

		public string WGGetPfKey ()
		{
			return iOSConnector.GetPfKey ();
		}

		public string WGGetPf ()
		{
			return iOSConnector.GetPf ();
		}

		public void WGLoginWithLocalInfo ()
		{
			iOSConnector.LoginWithLocalInfo ();
		}

		public void WGShowNotice (string scene)
		{
			iOSConnector.ShowNotice (scene);
		}

		public void WGHideScrollNotice ()
		{
			iOSConnector.HideScrollNotice ();
		}

		public void WGOpenUrl (string openUrl, string algorithm)
		{
			iOSConnector.OpenUrl (openUrl, algorithm);
		}

		public void WGOpenUrl (string openUrl, eMSDK_SCREENDIR screendir, string algorithm)
		{
            MsdkUtil.Log("Warnning : This api was deprecated. Please use 'WGOpenUrl(string openUrl)' instead.");
			iOSConnector.OpenUrlWithScreenDir (openUrl, (int)screendir, algorithm);
		}

        public void WGOpenUrl (string openUrl, eMSDK_SCREENDIR screendir, bool fullScreen, string algorithm)
        {
            iOSConnector.OpenUrlWithScreenDirAndFullScreen (openUrl, (int)screendir, fullScreen, algorithm);
        }

		public string WGGetEncodeUrl (string openUrl, string algorithm)
		{
			return iOSConnector.GetEncodeUrl (openUrl, algorithm);
		}



        // 嵌入式浏览器
        public int WGOpenEmbeddedWebView(string url)
        {
            return  iOSConnector.OpenEmbeddedWebView(url);
        }

        public int WGOpenEmbeddedWebViews(string charset, string data, int lens)
        {
            return iOSConnector.OpenEmbeddedWebViews(charset, data, lens);
        }

        public int WGCloseEmbeddedWebView()
        {
            return iOSConnector.CloseEmbeddedWebView();
        }

        public void WGCallToEmbeddedWebView(string enbeddedParams)
        {
            iOSConnector.CallToEmbeddedWebView(enbeddedParams);
        }

        public bool WGSetEmbeddedWebViewBackground(byte[] imageData, int lens)
        {
            return iOSConnector.SetEmbeddedWebViewBackground(imageData, lens);
        }

		public List<NoticeInfo> WGGetNoticeData (string scene)
		{
			string noticeDataStr = iOSConnector.GetNoticeData(scene);
			if (noticeDataStr != null)
			{
				return NoticeInfoList.ParseJson(noticeDataStr);
			}
			else
			{
				return new List<NoticeInfo>();
			}
		}

        public void WGOpenAmsCenter(string gameName, string actChannelId, string zoneId, string platformId, string partitionId, string roleId, string extra, eMSDK_SCREENDIR screenDir)
        {
            iOSConnector.OpenAmsCenter(gameName, actChannelId, zoneId, platformId, partitionId, roleId, extra, (int)screenDir);
        }

		public void WGGetNearbyPersonInfo ()
		{
			iOSConnector.GetNearbyPersonInfo ();
		}

		public bool WGCleanLocation ()
		{
			return iOSConnector.CleanLocation ();
		}

		public bool WGGetLocationInfo ()
		{
			return iOSConnector.GetLocationInfo ();
		}

        // 获取当前玩家的国籍信息(根据接入的IP)
        public void WGGetCountryFromIP()
        {
            iOSConnector.GetCountryFromIP ();
        }

        public void WGGetIPInfo()
        {
            iOSConnector.GetIPInfo();
        }

		public void WGStartGameStatus (string cGameStatus)
		{
			iOSConnector.StartGameStatus (cGameStatus);
		}

		public void WGEndGameStatus (string cGameStatus, int succ, int errorCode)
		{
			iOSConnector.EndGameStatus (cGameStatus, succ, errorCode);
		}

		public int WGGetPaytokenValidTime ()
		{
			return iOSConnector.GetPaytokenValidTime ();
		}
			

		public void WGClearLocalNotifications ()
		{
			iOSConnector.ClearLocalNotifications ();
		}

		public void WGOpenMSDKLog (bool enabled)
		{
			iOSConnector.OpenMSDKLog (enabled);
		}

		public string WGGetGuestID ()
		{
			return iOSConnector.GetGuestID ();
		}

		public void WGResetGuestID ()
		{
			iOSConnector.ResetGuestID ();
		}


		public long WGAddLocalNotification (LocalMessageIOS localMessage)
		{
			string localMsgJsonString = JsonMapper.ToJson (localMessage);
			return iOSConnector.AddLocalNotification (localMsgJsonString);
		}

		public long WGAddLocalNotificationAtFront (LocalMessageIOS localMessage)
		{
			string localMsgJsonString = JsonMapper.ToJson (localMessage);
			return iOSConnector.AddLocalNotificationAtFront (localMsgJsonString);
		}

		public void WGClearLocalNotification (LocalMessageIOS localMessage)
		{
			string localMsgJsonString = JsonMapper.ToJson (localMessage);
			iOSConnector.ClearLocalNotification (localMsgJsonString);
		}

		public void WGSetPushTag (string tag)
		{
			iOSConnector.SetPushTag (tag);
		}

		public void WGDeletePushTag (string tag)
		{
			iOSConnector.DeletePushTag (tag);
		}
        public void WGSetPushAccount (string account)
		{
			iOSConnector.SetPushAccount (account);
		}

		public void WGDeletePushAccount (string account)
		{
			iOSConnector.DeletePushAccount (account);
		}
        public void WGUnregisterPush()
		{
			iOSConnector.UnregisterPush ();
		}


#region ios Only
        // 信鸽推送相关
        public void WGRegisterAPNSPushNotification(string launchOptions)
        {
            iOSConnector.RegisterAPNSPushNotification(launchOptions);
        }

        public void WGSuccessedRegisterdAPNSWithToken(string deviceToken)
        {
            iOSConnector.SuccessedRegisterdAPNSWithToken(deviceToken);
        }

        public void WGFailedRegisteredAPNS()
        {
            iOSConnector.FailedRegisteredAPNS();
        }

        public void WGCleanBadgeNumber()
        {
            iOSConnector.CleanBadgeNumber();
        }

        public void WGReceivedMSGFromAPNS(string userInfo)
        {
            iOSConnector.ReceivedMSGFromAPNS(userInfo);
        }

        /**
         * 平台拉起事件
         * @param urlStr 平台拉起的url
         */
        public bool WGHandleOpenUrl(string urlStr)
        {
            return iOSConnector.HandleOpenUrl(urlStr);
        }

        /**
         * 平台拉起事件
         * @param urlStr 平台拉起的url
         */
        public bool WGHandleOpenUniversalLink(string urlStr)
        {
            return iOSConnector.HandleOpenUniversalLink(urlStr);
        }
#endregion iOS only



        public void WGBuglyLog(eBuglyLogLevel level, string log)
        {
            iOSConnector.BuglyLog((int) level, log);
        }

        /**
        * 关闭bugly上报 
        */
        public void WGCloseCrashReport()
        {
            iOSConnector.CloseCrashReport();
        }

		public void WGSendToQQWithPhoto(eQQScene scene,ImageParams imageParams,string extraScene,string messageExt, string gameTag)
        {
			iOSConnector.SendToQQWithPhotoWithParams((int)scene,imageParams.ios_imageData, imageParams.ios_imageDataLen, extraScene, messageExt, gameTag);
        }
        

        public void WGOpenFullScreenWebViewWithJson(string jsonStr)
        {
            iOSConnector.OpenFullScreenWebViewWithJson(jsonStr);
        }

        public void WGReportPrajna(string serialNumber)
        {
            iOSConnector.ReportPrajna(serialNumber);
        }


        public void WGReportException(eExceptionType exception_type,
                       string exception_name,
                       string exception_msg,
                       string exception_stack,
                       Dictionary<string, string> ext_info)
        {
            try
            {
                JsonData json = new JsonData ();
                foreach (string key in ext_info.Keys)
                {
                    json[key] = ext_info[key];
                }
                string jsonString = json.ToJson ();
                iOSConnector.ReportException((int)exception_type, exception_name, exception_msg, exception_stack, jsonString);
            }
            catch
            {
                Debug.LogError ("解析json失败，具体请看log日志");
            }

            

        }

        public void WGLogPlatformSDKVersion()
        {
           iOSConnector.LogPlatformSDKVersion();
        }

        /*
        *启动QQ小程序
        * @param miniAppID 必填，小程序的AppID（注：必须在QQ互联平台中，将该小程序与分享的App绑定）
        * @param miniPath  非必填，小程序的展示路径，不填展示默认小程序首页
        * @param type      非必填，小程序的类型，默认正式版(0)，可选测试版(1)、开发版(2)
        */
        public void WGLaunchQQMiniApp(string miniAppID, string miniPath, eMiniProgramType type)
        {
            iOSConnector.LaunchQQMiniApp(miniAppID, miniPath, (int)type);
        }
        
        /**
         *
         * 手Q小程序分享功能
         * @param scene 标识发送手Q会话或者Qzone
         *         eQQScene.QQScene_QZone: 分享到空间(4.5以上版本支持)
         *         eQQScene.QQScene_Session: 分享到手Q会话
         * @param title 结构化消息的标题
         * @param desc 结构化消息的概要信息
         * @param url  兼容低版本的网页链接
         * ImageParams：
         * 此struct在android、ios上对应的字段不同。android需要填写图片路径：android_imagePath，ios需要自行读取填写图片数据ios_imageData和图片数据长度ios_imageDataLen
         * @param miniProgramAppid 游戏绑定的小程序appid.在connect.qq.com操作绑定小程序，游戏和小程序必须是同一主体才能绑定
         * @param miniProgramPath 小程序页面的path，必须
         * @param type 小程序类型，分为正式版、体验版
         * @return void
         */
        public void WGSendToQQWithMiniApp(
                eQQScene scene,
                string title,
                string desc,
                string url,
                ImageParams imageParams,
                string miniProgramAppid,
                string miniProgramPath,
                eMiniProgramType type)
        {
            iOSConnector.SendToQQWithMiniApp((int)scene, title, desc, url, imageParams.ios_imageData, imageParams.ios_imageDataLen, miniProgramAppid, miniProgramPath, (int)type);
        }

        /**
         * 设置是否允许收集敏感信息
         */
        public void WGSetCouldCollectSensitiveInfo(bool couldCollect)
        {
            iOSConnector.SetCouldCollectSensitiveInfo(couldCollect);
        }

         /**
         * 以json形式设置敏感信息单字段开关，优先级小于总开关，目前支持设置AndroidID、Apn、Imei，QImei，QImei36
         * 参数示例{"AndroidID":true,"Apn":true,"Imei":false,"QImei":true,"QImei36":true}
         */
        public void WGSetCollectSensitiveInfo(string jsonInfo)
        {
            iOSConnector.SetCollectSensitiveInfo(jsonInfo);
        }


        /**
        * 以json形式设置敏感信息字段至各个组件SDK，目前支持{\"AndroidID\":\"xxx\", \"WiFiMacAddress\":\"xxx\", \"Imei\":\"xxx\", \"Model\":\"xxx\", \"Oaid\":\"xxx\", \"Imsi\":\"xxx\", \"Cid\":\"xxx\"}
        */
        public void WGSetSensitiveInfo(string jsonInfo)
        {
            iOSConnector.SetSensitiveInfo(jsonInfo);
        }

        public void WGOpenGameDataAuthCenter(string openUrl, eMSDK_SCREENDIR screendir, bool fullScreen, string algorithm)
        {
            iOSConnector.OpenGameDataAuthCenter(openUrl, (int)screendir, fullScreen, algorithm);
        }

        /**
        * 在配置文件中设置对应注销账号界面的url，即可根据环境按协议带参数拉起对应页面
        */
        public void WGOpenDeleteAccountUrl(eMSDK_SCREENDIR screenDir, bool isFullScreen, string algorithm)
        {
            iOSConnector.OpenDeleteAccountUrl((int)screenDir, isFullScreen, algorithm);
        }
#endregion 通用接口

#region Android平台接口
        //public void WGJoinQQGroup(string qqGroupKey)
        //{
        //    MsdkUtil.Log("Error : This interface is only use in Android!");
        //    throw new NotImplementedException();
        //}

        //public void WGQueryQQGroupInfo(string cUnionid, string cZoneid)
        //{
        //    MsdkUtil.Log("Error : This interface is only use in Android!");
        //    throw new NotImplementedException();
        //}

        //public void WGUnbindQQGroup(string cGroupOpenid, string cUnionid)
        //{
        //    MsdkUtil.Log("Error : This interface is only use in Android!");
        //    throw new NotImplementedException();
        //}

        //        public void WGQueryQQGroupKey(string cGroupOpenid)
        //        {
        //            MsdkUtil.Log("Error : This interface is only use in Android!");
        //            throw new NotImplementedException();
        //        }

        public void WGAddCardToWXCardPackage(string cardId, string timestamp, string sign)
        {
            MsdkUtil.Log("Error : This interface is only use in Android!");
            throw new NotImplementedException();
        }

        public void WGSendToWeixinWithPhotoPath(eWechatScene scene, string mediaTagName, string imgPath, string messageExt, string messageAction)
        {
            MsdkUtil.Log("Error : This interface is only use in Android!");
            throw new NotImplementedException();
        }

        public void WGSendToQQ(eQQScene scene, string title, string desc, string url, string imgUrl, int imgUrlLen, string messageExt, string gameTag)
        {
            MsdkUtil.Log("Error : This interface is only use in Android!");
            throw new NotImplementedException();
        }

        public void WGSendToQQWithPhoto(eQQScene scene, string imgFilePath)
        {
            MsdkUtil.Log("Error : This interface is only use in Android!");
            throw new NotImplementedException();
        }

        //public void WGSendToQQWithRichPhoto(string summary, ArrayList imgs)
        //{
        //    MsdkUtil.Log("Error : This interface is only use in Android!");
        //    throw new NotImplementedException();
        //}

        public void WGSendToQQWithVideo(string summary, string videoPath)
        {
            MsdkUtil.Log("Error : This interface is only use in Android!");
            throw new NotImplementedException();
        }

        public bool WGCheckApiSupport(eApiName apiName)
        {
            MsdkUtil.Log("Error : This interface is only use in Android!");
            throw new NotImplementedException();
        }

        public void WGStartSaveUpdate(bool isUseYYB)
        {
            MsdkUtil.Log("Error : This interface is only use in Android!");
            throw new NotImplementedException();
        }

        public void WGCheckNeedUpdate()
        {
            MsdkUtil.Log("Error : This interface is only use in Android!");
            throw new NotImplementedException();
        }

        public int WGCheckYYBInstalled()
        {
            MsdkUtil.Log("Error : This interface is only use in Android!");
            throw new NotImplementedException();
        }

        public long WGAddLocalNotification(LocalMessageAndroid msg)
        {
            MsdkUtil.Log("Error : This interface is only use in Android!");
            throw new NotImplementedException();
        }
		public void reportData()
		{
			string cmd = "unity_version_report";
			JsonData jsonData = new JsonData ();
			jsonData ["msdkUnityVersion"] = WGPlatform.Version;

			iOSConnector.ReportUnityData (cmd,jsonData.ToJson());
		}

      


#endregion


	}
}
#endif
