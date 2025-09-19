#if UNITY_ANDROID
using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace Msdk
{

	public class WGPlatformAndroid : WGPlatformUnity, IMsdk
	{
		// MSDK Unity 初始化
		public new void Init()
		{
			base.Init();
            // 注册C++回调
            AndroidConnector.setBridge(MessageCenter.MessageConsumer);
            // 上报Unity版本
            //reportData(WGPlatform.Version);
            reportData();
		}

        private void reportData(string message)
        {
            try {
                LoginRet loginRet = WGGetLoginRecord();
                
                message += "_" + loginRet.open_id;
                using (AndroidJavaObject report = new AndroidJavaObject("com.tencent.msdk.request.MsdkDataReport"))
                {
                    // operate 设置为1001，用以后台过滤
                    report.Call("reportData", message, (int)ePlatform.ePlatform_QQ, 1001);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        public void reportData()
        {
            string cmd = "unity_version_report";
            JsonData jsonData = new JsonData();
            jsonData["msdkUnityVersion"] = WGPlatform.Version;

            AndroidConnector.ReportUnityData(cmd, jsonData.ToJson());
        }
        public static AndroidJavaObject dicToMap(Dictionary<string, string> dictionary)
        {
            if (dictionary == null)
            {
                return null;
            }
            AndroidJavaObject map = new AndroidJavaObject("java.util.HashMap");
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                map.Call<string>("put", pair.Key, pair.Value);
            }
            return map;
        }
#region 登录相关接口
		public void WGLogin (ePlatform platform)
		{
            AndroidConnector.Login((int)platform);
		}

        public void WGChannelPermissionAuth(ePlatform platform, string permissions)
        {
            AndroidConnector.ChannelPermissionAuth ((int) platform, permissions);
        }

        public void WGCheckWXUniversalLink()
        {
            Debug.Log("Android no support WGCheckWXUniversalLink");
        }

		public int WGLoginOpt (ePlatform platform,int overtime)
		{
			return AndroidConnector.LoginOpt((int)platform,overtime);
		}
		public void WGQrCodeLogin (ePlatform platform)
		{
            AndroidConnector.QrCodeLogin((int)platform);
		}

		public bool WGLogout ()
		{
            return AndroidConnector.Logout();
		}

		public LoginRet WGGetLoginRecord ()
		{
            string loginRetStr = AndroidConnector.GetLoginRecord();
			if (loginRetStr != null) {
				return LoginRet.ParseJson (loginRetStr);
			} else {
				return new LoginRet();
			}
		}

		public void WGSetPermission (int permissions)
		{
            AndroidConnector.SetPermission(permissions);
		}

		public void WGLoginWithLocalInfo ()
		{
            AndroidConnector.LoginWithLocalInfo();
		}

		public bool WGSwitchUser (bool switchToLaunchUser)
		{
            return AndroidConnector.SwitchUser(switchToLaunchUser);
		}

		public int WGGetPaytokenValidTime ()
		{
            return AndroidConnector.GetPaytokenValidTime();
		}

        public void WGRealNameAuth(RealNameAuthInfo info)
        {
            string authInfoJsonStr = JsonMapper.ToJson(info);
            AndroidConnector.RealNameAuth(authInfoJsonStr);
        }
#endregion 登录相关接口

#region QQ相关接口
		/**
		 * 查询关系链
		 */
		public bool WGQueryQQMyInfo ()
		{
            return AndroidConnector.QueryQQMyInfo();
		}

		public bool WGQueryQQGameFriendsInfo ()
		{
            return AndroidConnector.QueryQQGameFriendsInfo();
		}

		/**
		 * 加绑群，加好友
		 */
		//public void WGBindQQGroup (string unionid, string union_name, string zoneid, string signature)
		//{
  //          AndroidConnector.BindQQGroup(unionid, union_name, zoneid, signature);
		//}

		public void WGAddGameFriendToQQ (string fopenid, string desc, string message)
		{
            AndroidConnector.AddGameFriendToQQ(fopenid, desc, message);
		}

		/**
		 * 分享
		 */
		public void WGSendToQQ (eQQScene scene, string title, string desc, string url, string imgUrl, int imgUrlLen, string messageExt, string gameTag)
		{
            AndroidConnector.SendToQQ((int)scene, title, desc, url, imgUrl, imgUrlLen, messageExt, gameTag);
		}

		public void WGSendToQQWithPhoto (eQQScene scene, string imgFilePath)
		{
            AndroidConnector.SendToQQWithPhoto((int)scene, imgFilePath);
		}

        public void WGSendToQQWithCommonShare(string serviceID, string extraJson)
        {
            AndroidConnector.SendToQQWithCommonShare(serviceID, extraJson);
        }

		public void WGSendToQQWithMusic (eQQScene scene, string title, string desc, string musicUrl, string musicDataUrl, string imgUrl)
		{
            AndroidConnector.SendToQQWithMusic((int)scene, title, desc, musicUrl, musicDataUrl, imgUrl);
		}

		public bool WGSendToQQGameFriend (int act, string fopenid, string title, string summary,
                string targetUrl, string imgUrl, string previewText, string gameTag, string msdkExtInfo)
		{
             return AndroidConnector.SendToQQGameFriend(act, fopenid, title, summary, targetUrl, imgUrl,
                 previewText, gameTag, msdkExtInfo);
		}

		//public void WGSendToQQWithRichPhoto (string summary, ArrayList imgs)
		//{
		//	StringBuilder sb = new StringBuilder();
		//	JsonWriter writer = new JsonWriter(sb);
		//	writer.WriteArrayStart();
		//	foreach (string img in imgs) {
		//		writer.Write(img);
		//	}
		//	writer.WriteArrayEnd();
  //          AndroidConnector.SendToQQWithRichPhoto(summary, sb.ToString());
		//}
        /**
         * 分享丰富的图片到空间
         * @param summary 分享的正文(无最低字数限制，最高1w字)
         * @param imageDatas 分享的图片的本地路径，可支持多张图片(<=9
         * 张图片为发表说说，>9 张图片为上传图片到相册)，只支持本地图片
         * 注意: 此功能只在手Q5.9.5及其以上版本支持
         */
        public void WGSendToQQWithRichPhoto(string summary, ArrayList imgParams, string extraScene, string messageExt)
        {
            // 在这里拆分结构体
            List<string> imgP = new List<string>();
            for (int i = 0; i < imgParams.Count; i++)
            {
                imgP.Add(((ImageParams)imgParams[i]).android_imagePath);
            }
            StringBuilder sb = new StringBuilder();
            JsonWriter writer = new JsonWriter(sb);
            writer.WriteArrayStart();
            foreach (string img in imgP) {
                writer.Write(img);
            }
            writer.WriteArrayEnd();
            Debug.Log("输出 richPhoto img:" + sb.ToString());
            AndroidConnector.SendToQQWithRichPhoto(summary, sb.ToString(),extraScene, messageExt);
            //List<string> androidPath_list = new List<string>();
            //Debug.Log("WGPlatformAndroid 在这个拆分结构体");
            //for (int i = 0; i < imgParams.Count; i++)
            //{
            //    ImageParams tmp_ImageParams = new ImageParams();
            //    tmp_ImageParams.android_imagePath = imgParams[i].android_imagePath;
            //    androidPath_list.Add(tmp_ImageParams.android_imagePath);
            //    Debug.Log("androidPath_list [" + i + "] " + androidPath_list[i]);
            //}
            //Debug.Log("WGPlatformAndroid 拆分完毕");
            //AndroidConnector.SendToQQWithRichPhoto(summary, androidPath_list, extraScene, messageExt);
        }

		/**
		 * QQ群
		 */
		//public void WGJoinQQGroup (string qqGroupKey)
		//{
  //          AndroidConnector.JoinQQGroup(qqGroupKey);
		//}

        //public void WGJoinQQGroup(string groupNum, string groupKey)
        //{
        //    //MsdkUtil.Log("Error : This interface is only use in iOS!");
        //    //throw new NotImplementedException();
        //    WGJoinQQGroup(groupKey);
        //}

		//public void WGQueryQQGroupInfo (string unionId, string zoneId)
		//{
  //          AndroidConnector.QueryQQGroupInfo(unionId, zoneId);
		//}

		//public void WGUnbindQQGroup (string groupOpenid, string unionId)
		//{
  //          AndroidConnector.UnbindQQGroup(groupOpenid, unionId);
		//}

		//public void WGQueryQQGroupKey (string groupOpenid)
		//{
  //          AndroidConnector.QueryQQGroupKey(groupOpenid);
		//}

        //public void WGCreateQQGroupV2(
        //    string guildId,
        //    string guildName, 
        //    string zoneId,
        //    string roleId, 
        //    string partition
        //    )
        //{
        //    AndroidConnector.CreateQQGroupV2(guildId, guildName, zoneId, roleId, partition);
        //}
        public void WGCreateQQGroupV2(GameGuild gameGuild)
        {
            AndroidConnector.CreateQQGroupV2(
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

        //public void WGJoinQQGroupV2(
        //    string guildId,
        //    string zoneId,
        //    string groupId,
        //    string roleId,
        //    string partition)
        //{
        //    AndroidConnector.JoinQQGroupV2(guildId, zoneId,groupId, roleId, partition);
        //}

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
            AndroidConnector.JoinQQGroupV2(
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
            AndroidConnector.UnbindQQGroupV2(
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
            AndroidConnector.QueryQQGroupInfoV2(groupId);
        }
        public void WGSendToQQWithPhoto(eQQScene scene, ImageParams imageParams, string extraScene, string messageExt, string gameTag) 
        {
            AndroidConnector.SendToQQWithPhotoWithParams((int)scene, imageParams.android_imagePath, extraScene, messageExt, gameTag);
        }
        //public void WGSendToWXWithMiniApp(eWechatScene scene, string title, string desc, byte[] thumbImgData, int thumbImgDataLen, string webpageUrl, string userName, string path, bool withShareTicket, string messageExt, string messageAction)
        //{
        //    AndroidConnector.SendToWXWithMiniApp((int)scene, title, desc, thumbImgData, thumbImgDataLen, webpageUrl, userName, path, withShareTicket, messageExt, messageAction);
        //}
        /**
         *
         *微信小程序分享功能
         参数定义：
         scene：分享场景（会话/朋友圈），目前只支持分享到会话Session
         title：分享标题，长度不超过512字节
         desc：描述内容，长度不超过1K字节
         thumbImgData： 小程序缩略图， IOS: 不超过32K（旧版本使用）,不超过128K（新版本） Android: 不超过32K（旧版本）
         webpageUrl：旧版本微信打开该小程序分享时，兼容跳转的普通页面url(必填，可任意url，用于老版本兼容)
         userName：小程序username，如gh_d43f693ca31f(必填，可任意url，用于老版本兼容)
         path：小程序path，可通过该字段指定跳转小程序的某个页面（若不传，默认跳转首页）
         withShareTicket 是否带shareTicket转发（如果小程序页面要展示用户维度的数据，并且小程序可能分享到群，需要设置为YES）
         mediaTagName 请根据实际情况填入下列值的一个, 此值会传到微信供统计用,
         在分享返回时也会带回此值, 可以用于区分分享来源
             "MSG_INVITE";                   // 邀请
             "MSG_SHARE_MOMENT_HIGH_SCORE";    //分享本周最高到朋友圈
             "MSG_SHARE_MOMENT_BEST_SCORE";    //分享历史最高到朋友圈
             "MSG_SHARE_MOMENT_CROWN";         //分享金冠到朋友圈
             "MSG_SHARE_FRIEND_HIGH_SCORE";     //分享本周最高给好友
             "MSG_SHARE_FRIEND_BEST_SCORE";     //分享历史最高给好友
             "MSG_SHARE_FRIEND_CROWN";          //分享金冠给好友
             "MSG_friend_exceed"         // 超越炫耀
             "MSG_heart_send"            // 送心
         messageExt:暂时不用可以空字串
         messageAction：暂时不用可以空字串
         * @return void
         *   通过游戏设置的全局回调的OnShareNotify(ShareRet&
         * shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
         *     eFlag_Succ: 分享成功
         *     eFlag_Error: 分享失败
         *
         *
         */
     
        public void WGSendToWXWithMiniApp(
            eWechatScene scene,
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
            string mediaTagName,
            eMiniProgramType type,
            string userOpenId)
        {
            AndroidConnector.SendToWXWithMiniApp((int)scene, title, desc, thumbImgData, thumbImgDataLen, webpageUrl, userName, path, withShareTicket, messageExt, messageAction, mediaTagName, (int)type, userOpenId);
        }

        /**
         * 启动小程序
         * @param userName  //  填小程序原始id
         * @param path      //  拉起小程序页面的可带参路径，不填默认拉起小程序首页
         * @param type      //  preview = 2 ; test = 1 ; release = 0;
         */
        public void WGLaunchMiniApp(string userName, string path, eMiniProgramType type)
        {
            AndroidConnector.LaunchMiniApp(userName, path, (int)type);
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
            AndroidConnector.SendToWXWithOpenBusinessView(businessType, query, extInfo, extData);
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
            AndroidConnector.WGSendToWXGameLive((int)scene, title, desc, messageExt, jsonData);
        }
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
            AndroidConnector.BindExistQQGroupV2(
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
            AndroidConnector.GetQQGroupCodeV2(
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
            AndroidConnector.QueryBindGuildV2(groupId, type);
        }

        public void WGGetQQGroupListV2()
        {
            AndroidConnector.GetQQGroupListV2();
        }

        public void WGRemindGuildLeaderV2(GameGuild gameGuild)
        {
             AndroidConnector.RemindGuildLeaderV2(
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

        //public void WGSendToWeixinWithVideo(eWechatScene scene, string title, string desc, string thumbUrl, string videoUrl, VideoParams videoParams, string mediaTagName, string messageAction, string messageExt)
        //{
        //    Debug.Log("scene:" + scene + ",title:" + title + ",desc:" + desc + ",thumbUrl:" + thumbUrl + ",videoUrl:" + videoUrl +
        //        ",videoParams.android_videoPath:" + videoParams.android_videoPath + ",mediaTagName:" + mediaTagName + ",messageAction:" + messageAction + ",messageExt:" + messageExt);
        //    AndroidConnector.SendToWeixinWithVideo((int)scene, title, desc, thumbUrl, videoUrl, videoParams.android_videoPath, mediaTagName, messageAction, messageExt);
        //}
        /* *
         *微信游戏圈视频分享功能
         *1.视频格式目前只支持mp4，不超过10M大小，时间不超过10秒，Android目前限制不超过3M
         *2.宽高比在1:2到3:1之间，FPS不超过30
         *3.视频只能分享到朋友圈(scene值为WXSceneTimeLine)
         **
         **结果通过分享的回调返回 OnShareNotify
         参数说明：
         scene：分享的场景，目前只支持微信朋友圈：WechatScene_Timeline
         title：分享标题，长度不超过512字节
         desc：描述内容，长度不超过1K字节
         thumbUrl：视频的缩略图URL
         videoUrl：分享视频的URL
         videoData： 视频文件数据，sdk限制不能超过10M大小，
         videoDataLen：视频文件数据长度，sdk限制不能超过10M大小，
         mediaTagName：数据统计用，建议传MSG_SHARE_MOMENT_VIDEO
         mediaAction： 透传到微信游戏后台，可使用该字段透传信息以便后台设置该条朋友圈的一些字段，格式为action_type#param
         mediaExt：启动游戏时透传回游戏侧
         **/
        public void WGSendToWeixinWithVideo(eWechatScene scene, string title, string desc, VideoParams videoParams, string mediaTagName, string messageAction, string messageExt)
        {
            Debug.Log("scene:" + scene + ",title:" + title + ",desc:" + desc + 
                ",videoParams.android_videoPath:" + videoParams.android_videoPath + ",mediaTagName:" + mediaTagName + ",messageAction:" + messageAction + ",messageExt:" + messageExt);
            AndroidConnector.SendToWeixinWithVideo((int)scene, title, desc, videoParams.android_videoPath, mediaTagName, messageAction, messageExt);
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
            AndroidConnector.SendToWXStateWithPhoto(stateId, stateTitle, imageParams.android_imagePath, jumpUrl);
        }

        /**
        *
        * 分享图片信息至微信状态-支持跳转到小程序
        * 参数说明：
        * stateId：状态ID，选填，不同的状态ID会显示不同的Icon
        * stateTitle：状态标题，选填，限制32 个字符以内
        * imageParams：图片信息，必填，图片比例为9:16
        * 此struct在android、ios上对应的字段不同。android需要填写图片路径：android_imagePath，ios需要自行读取填写图片数据ios_imageData和图片数据长度ios_imageDataLen
        * userName:点击状态跳转小程序的usernmae，不可为空，必填
        * path: 点击状态跳转小程序的path，选填
        * type: 小程序类型，0:正式版，1: 测试版，2：预览版
        */
        public void WGSendToWXStateWithPhotoOpenMiniApp(string stateId,string stateTitle, ImageParams imageParams, string userName, string path, eMiniProgramType type)
        {
            AndroidConnector.SendToWXStateWithPhotoOpenMiniApp(stateId, stateTitle, imageParams.android_imagePath, userName, path, (int)type);
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
            AndroidConnector.SendToWXChannelShareVideo(videoParams.android_videoPath, messageExt);
        }

        /**
         * 微信视频号直播
         * 参数说明：
         * channelLiveJson 游戏拉起视频号直播透传json数据，包括liveJsonInfo和openID字段
         * messageExt 拓展字段，选填，携带额外信息，当前版本暂未使⽤
         */
        public void WGSendToWXChannelStartLive(string channelLiveJson, string messageExt)
        {
            AndroidConnector.SendToWXChannelStartLive(channelLiveJson, messageExt);
        }

        public void WGSendToQQWithArk(eQQScene scene, string title, string desc, string url, string imgUrl, string jsonString, string messageExt, string gameTag)
        {
            AndroidConnector.SendToQQWithArk((int)scene, title, desc, url, imgUrl, jsonString, messageExt, gameTag);
        }

        public void WGSendToQQWithText(string text, string extraScene, string messageExt)
        {
            AndroidConnector.SendToQQWithText(text, extraScene, messageExt);
        }

        /*
        *启动QQ小程序
        * @param miniAppID 必填，小程序的AppID（注：必须在QQ互联平台中，将该小程序与分享的App绑定）
        * @param miniPath  非必填，小程序的展示路径，不填展示默认小程序首页
        * @param type      非必填，小程序的类型，默认正式版(0)，可选测试版(1)、开发版(2)
        */
        public void WGLaunchQQMiniApp(string miniAppID, string miniPath, eMiniProgramType type)
        {
            AndroidConnector.LaunchQQMiniApp(miniAppID, miniPath, (int)type);
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
            AndroidConnector.SendToQQWithMiniApp((int)scene, title, desc, url, imageParams.android_imagePath, miniProgramAppid, miniProgramPath, (int)type);
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
            AndroidConnector.SendToQQWithVideo(summary, videoParams.android_videoPath, extraScene, messageExt);
        }

#endregion QQ相关接口

#region 微信相关接口
		public void WGOpenWeiXinDeeplink (string link)
		{
            AndroidConnector.OpenWeiXinDeeplink(link);
		}

		public void WGRefreshWXToken ()
		{
            AndroidConnector.RefreshWXToken();
		}

		/**
		 * 关系链
		 */
		public bool WGQueryWXMyInfo ()
		{
            return AndroidConnector.QueryWXMyInfo();
		}


		public bool WGQueryWXGameFriendsInfo ()
		{
            return AndroidConnector.QueryWXGameFriendsInfo();
		}

        /**
        * 获取unionid(qq用户唯一标识), 回调在OnRelationNotify中
        * 其中RelationRet.persons为一个Vector，Vector的第一项即为自己的资料
         * 个人信息中包含unionID，其类型为一个string
        */
        public void WGQueryUnionID()
        {
            AndroidConnector.QueryUnionID();
        }

		/**
		 * 分享
		 */
        /**
         * @param title 结构化消息的标题
         * @param desc 结构化消息的概要信息
         * @param mediaTagName 请根据实际情况填入下列值的一个, 此值会传到微信供统计用,
         在分享返回时也会带回此值, 可以用于区分分享来源
             "MSG_INVITE";                   // 邀请
             "MSG_SHARE_MOMENT_HIGH_SCORE";    //分享本周最高到朋友圈
             "MSG_SHARE_MOMENT_BEST_SCORE";    //分享历史最高到朋友圈
             "MSG_SHARE_MOMENT_CROWN";         //分享金冠到朋友圈
             "MSG_SHARE_FRIEND_HIGH_SCORE";     //分享本周最高给好友
             "MSG_SHARE_FRIEND_BEST_SCORE";     //分享历史最高给好友
             "MSG_SHARE_FRIEND_CROWN";          //分享金冠给好友
             "MSG_friend_exceed"         // 超越炫耀
             "MSG_heart_send"            // 送心
         * @param thumbImgData 结构化消息的缩略图
         * @param thumbImgDataLen 结构化消息的缩略图数据长度
         * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过
         OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
         *   通过游戏设置的全局回调的OnShareNotify(ShareRet&
         shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
         *     eFlag_Succ: 分享成功
         *     eFlag_Error: 分享失败
         * @param userOpenId 指定分享给特定的好友
         */
		public void WGSendToWeixin (string title, string desc, string mediaTagName, byte[] imgData,
                                    int imgDataLen, string messageExt, string userOpenId)
		{
            AndroidConnector.SendToWeixin(title, desc, mediaTagName, imgData, imgDataLen, messageExt, userOpenId);
		}

        /**
         * @param title 结构化消息的标题
         * @param desc 结构化消息的概要信息
         * @param url 分享的URL
         * @param mediaTagName 请根据实际情况填入下列值的一个, 此值会传到微信供统计用,
         在分享返回时也会带回此值, 可以用于区分分享来源
         "MSG_INVITE";                   // 邀请
         "MSG_SHARE_MOMENT_HIGH_SCORE";    //分享本周最高到朋友圈
         "MSG_SHARE_MOMENT_BEST_SCORE";    //分享历史最高到朋友圈
         "MSG_SHARE_MOMENT_CROWN";         //分享金冠到朋友圈
         "MSG_SHARE_FRIEND_HIGH_SCORE";     //分享本周最高给好友
         "MSG_SHARE_FRIEND_BEST_SCORE";     //分享历史最高给好友
         "MSG_SHARE_FRIEND_CROWN";          //分享金冠给好友
         "MSG_friend_exceed"         // 超越炫耀
         "MSG_heart_send"            // 送心
         * @param thumbImgData 结构化消息的缩略图
         * @param thumbImgDataLen 结构化消息的缩略图数据长度
         * @param messageExt 游戏分享时传入字符串，通过此消息拉起游戏会通过
         OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
         *   通过游戏设置的全局回调的OnShareNotify(ShareRet&
         shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
         *     eFlag_Succ: 分享成功
         *     eFlag_Error: 分享失败
         * @param userOpenId 指定分享给特定的好友
         */
		public void WGSendToWeixinWithUrl (eWechatScene scene, string title, string desc, string url,
                                           string mediaTagName, byte[] imgData, int imgDataLen, string messageExt, string userOpenId)
		{
            AndroidConnector.SendToWeixinWithUrl((int)scene, title, desc, url, mediaTagName, imgData,
                                                 imgDataLen, messageExt, userOpenId);
		}

        public void WGSendToWXNativeGamePage(eWechatScene scene, string title, string desc,
            string mediaTagName, byte[] thumbImgData, int thumbImgDataLen,
            bool isVideo, int videoDuration, string shareData, string messageExt, string messageAction, string userOpenId)
        {
            AndroidConnector.SendToWXNativeGamePage((int)scene, title, desc, mediaTagName, thumbImgData, thumbImgDataLen, isVideo, videoDuration, shareData, messageExt, messageAction, userOpenId);
        }

		public void WGSendToWeixinWithPhoto (eWechatScene scene, string mediaTagName, byte[] imgData,
                int imgDataLen, string messageExt, string messageAction)
		{
            AndroidConnector.SendToWeixinWithPhoto((int)scene, mediaTagName, imgData, imgDataLen,
                messageExt, messageAction);
		}

		public void WGSendToWeixinWithPhotoPath (eWechatScene scene, string mediaTagName, string imgPath,
                string messageExt, string messageAction)
		{
            AndroidConnector.SendToWeixinWithPhotoPath((int)scene, mediaTagName, imgPath, messageExt,
                messageAction);
		}

		public void WGSendToWeixinWithMusic (eWechatScene scene, string title, string desc, string musicUrl,
                string musicDataUrl, string mediaTagName, byte[] imgData, int imgDataLen, string messageExt,
                string messageAction)
		{
            AndroidConnector.SendToWeixinWithMusic((int)scene, title, desc, musicUrl, musicDataUrl,
                    mediaTagName, imgData, imgDataLen, messageExt, messageAction);
		}

		public bool WGSendToWXGameFriend (string fOpenId, string title, string description, string mediaId,
                string messageExt, string mediaTagName, string msdkExtInfo)
		{
            return AndroidConnector.SendToWXGameFriend(fOpenId, title, description, mediaId,
                    messageExt, mediaTagName, msdkExtInfo);
		}

		public bool WGSendMessageToWechatGameCenter (string fOpenid, string title, string content,
                WXMessageTypeInfo pInfo, WXMessageButton pButton, string msdkExtInfo)
		{
			string pInfoJsonString = JsonMapper.ToJson (pInfo);
			string pButtonJsonString = JsonMapper.ToJson (pButton);
            return AndroidConnector.SendMessageToWechatGameCenter(fOpenid, title, content,
                    pInfoJsonString, pButtonJsonString, msdkExtInfo);
		}

        public void WGShareToWXGameline(byte[] data, string gameExtra)
        {
            int lens = 0;
            if (data != null) {
                lens = data.Length;
            }
            AndroidConnector.ShareToWXGameline(data, lens, gameExtra);
        }
		/**
		 * 微信群，卡卷
		 */
		public void WGCreateWXGroup (string unionId, string chatRoomName, string chatRoomNickName)
		{
            AndroidConnector.CreateWXGroup(unionId, chatRoomName, chatRoomNickName);
		}

		public void WGJoinWXGroup (string unionId, string chatRoomNickName)
		{
            AndroidConnector.JoinWXGroup(unionId, chatRoomNickName);
		}

		public void WGQueryWXGroupInfo (string unionId, string openIdList)
		{
            AndroidConnector.QueryWXGroupInfo(unionId, openIdList);
		}

		public void WGSendToWXGroup (int msgType, int subType, string unionId, string title,
                string description, string messageExt, string mediaTagName, string imgUrl,
                string msdkExtInfo)
		{
            AndroidConnector.SendToWXGroup(msgType, subType, unionId, title, description,
                    messageExt, mediaTagName, imgUrl, msdkExtInfo);
		}

		public void WGAddCardToWXCardPackage (string cardId, string timestamp, string sign)
		{
            AndroidConnector.AddCardToWXCardPackage(cardId, timestamp, sign);
		}
		public void WGQueryWXGroupStatus (string unionid, eStatusType opType)
		{
			AndroidConnector.QueryWXGroupStatus (unionid, (int)opType);
		}
		public void WGUnbindWeiXinGroup(string unionid)
		{
            AndroidConnector.UnbindWeiXinGroup(unionid);
		}
#endregion 微信相关接口

#region 其他接口
        public string WGGetPf ()
        {
            return AndroidConnector.GetPf();
        }

        public bool WGCheckApiSupport (eApiName apiName)
        {
            return AndroidConnector.CheckApiSupport((int)apiName);
        }

        public void WGFeedback (string body)
        {
            AndroidConnector.FeedbackWithBody(body);
        }

        public void WGEnableCrashReport (bool bRDMEnable, bool bMTAEnable)
        {
            AndroidConnector.EnableCrashReport(bRDMEnable, bMTAEnable);
        }

        public void WGReportEvent (string name, Dictionary<string, string> eventList, bool isRealTime)
        {
	        JsonData json = new JsonData ();
	        foreach (string key in eventList.Keys) {
		        json[key] = eventList[key];
	        }
	        string jsonData = json.ToJson ();
            AndroidConnector.ReportEvent(name, jsonData, isRealTime);
        }

        public void WGOpenMSDKLog(bool enabled)
        {
            AndroidConnector.OpenMSDKLog(enabled);
        }

        public string WGGetVersion ()
        {
            return AndroidConnector.GetVersion();
        }

        public string WGGetChannelId ()
        {
            return AndroidConnector.GetChannelId();
        }

        public string WGGetPlatformAPPVersion (ePlatform platform)
        {
            return AndroidConnector.GetPlatformAPPVersion((int)platform);
        }

        public string WGGetRegisterChannelId ()
        {
            return AndroidConnector.GetRegisterChannelId();
        }

        public bool WGIsPlatformInstalled (ePlatform platform)
        {
            return AndroidConnector.IsPlatformInstalled((int)platform);
        }

        public string WGGetPfKey ()
        {
            return AndroidConnector.GetPfKey();
        }

        /**
         * 游戏场景
         */
        public void WGStartGameStatus (string status)
        {
            AndroidConnector.StartGameStatus(status);
        }

        public void WGEndGameStatus(string status, int succ, int errorCode)
        {
            AndroidConnector.EndGameStatus(status, succ, errorCode);
        }

        /**
         * 公告
         */
        public void WGShowNotice (string scene)
        {
            AndroidConnector.ShowNotice(scene);
        }

        public void WGHideScrollNotice ()
        {
            AndroidConnector.HideScrollNotice();
        }

        public List<NoticeInfo> WGGetNoticeData (string scene)
        {
            string noticeDataStr = AndroidConnector.GetNoticeData(scene);
	        if (noticeDataStr != null) {
		        return NoticeInfoList.ParseJson(noticeDataStr);
	        } else {
		        return new List<NoticeInfo>();
	        }
        }

        /**
         * 内置浏览器
         */
        public void WGOpenUrl (string openUrl, string algorithm)
        {
            AndroidConnector.OpenUrl(openUrl, algorithm);
        }

        public void WGOpenUrl (string openUrl, eMSDK_SCREENDIR screendir, string algorithm)
        {
            AndroidConnector.OpenUrlWithScreenDir(openUrl, (int)screendir, algorithm);
        }

        public void WGOpenUrl (string openUrl, eMSDK_SCREENDIR screendir, bool fullScreen, string algorithm)
        {
            AndroidConnector.OpenUrlWithScreenDirAndFullScreen (openUrl, (int)screendir, fullScreen, algorithm);
        }

        public string WGGetEncodeUrl (string openUrl, string algorithm)
        {
            return AndroidConnector.GetEncodeUrl(openUrl, algorithm);
        }

        public void WGOpenAmsCenter(string gameName, string actChannelId, string zoneId, string platformId, string partitionId, string roleId, string extra, eMSDK_SCREENDIR screenDir)
        {
            AndroidConnector.OpenAmsCenter(gameName, actChannelId, zoneId, platformId, partitionId, roleId, extra, (int)screenDir);
        }

        /**
         * 位置信息
         */
        public void WGGetNearbyPersonInfo ()
        {
            AndroidConnector.GetNearbyPersonInfo();
        }

        public bool WGCleanLocation ()
        {
            return AndroidConnector.CleanLocation();
        }

        public bool WGGetLocationInfo ()
        {
            return AndroidConnector.GetLocationInfo();
        }

        // 获取当前玩家的国籍信息(根据接入的IP)
        public void WGGetCountryFromIP()
        {
            AndroidConnector.GetCountryFromIP ();
        }

        //获取玩家所在的国家及省市信息
        public void WGGetIPInfo()
        {
            AndroidConnector.GetIPInfo();
        }

        /**
         * 广告
         */
        public void WGShowAD (eADType adType)
        {
            AndroidConnector.ShowAD((int)adType);
        }

        public void WGCloseAD (eADType adType)
        {
            AndroidConnector.CloseAD((int)adType);
        }

        /**
         * 应用宝更新
         */
        public void WGStartSaveUpdate (bool isUseYYB)
        {
            AndroidConnector.StartSaveUpdate(isUseYYB);
        }

        public void WGCheckNeedUpdate ()
        {
            AndroidConnector.CheckNeedUpdate();
        }

        public int WGCheckYYBInstalled ()
        {
            return AndroidConnector.CheckYYBInstalled();
        }

        public long WGAddLocalNotification (LocalMessageAndroid msg)
        {
	        string jsonString = JsonMapper.ToJson (msg);
            return AndroidConnector.AddLocalNotification(jsonString);
        }

        public void WGClearLocalNotifications ()
        {
            AndroidConnector.ClearLocalNotifications();
        }

        public void WGSetPushTag (string tag)
        {
            AndroidConnector.SetPushTag(tag);
        }

        public void WGDeletePushTag (string tag)
        {
            AndroidConnector.DeletePushTag(tag);
        }

        public void WGBuglyLog (eBuglyLogLevel level, string log)
        {
            AndroidConnector.BuglyLog((int)level, log);
        }
        
        /**
        * 关闭bugly上报 
        */
        public void WGCloseCrashReport()
        {
            AndroidConnector.CloseCrashReport();
        }
        
        public void WGOpenFullScreenWebViewWithJson (string jsonStr)
        {
            AndroidConnector.OpenFullScreenWebViewWithJson(jsonStr);
        }

        public void WGReportPrajna (string serialNumber)
        {
            AndroidConnector.ReportPrajna(serialNumber);
        }

        /**
         * 在游戏窗口中打开嵌入式浏览器页面
         * url 要打开的页面链接
         * 返回值 1表示打开成功，0或者负数表示打开失败
         */
        public int WGOpenEmbeddedWebView(string url)
        {
            return AndroidConnector.OpenEmbeddedWebView(url);
        }

        /**
         * 在游戏窗口中以数据的形式打开浏览器页面
         * charset 数据的编码格式（目前只支持UTF-8编码的数据）
         * data 传递给页面的数据
         * lens 传递的数据长度
         * 返回值 1表示打开成功，0或者负数表示打开失败
         */
        public int WGOpenEmbeddedWebViews(string charset, string data, int lens)
        {
            return AndroidConnector.OpenEmbeddedWebViews(charset, data, lens);
        }

        /**
         * 在游戏窗口中关闭浏览器页面
         * 返回值 1表示关闭成功，0或者负数表示关闭失败
         */
        public int WGCloseEmbeddedWebView()
        {
            return AndroidConnector.CloseEmbeddedWebView();
        }

        /**
         * 在游戏中发送信息到页面js函数
         * params 要传递给js函数的函数名、参数信息
         */
        public void WGCallToEmbeddedWebView(string embeddedParams)
        {
            AndroidConnector.CallToEmbeddedWebView(embeddedParams);
        }

        /**
         * 设置webview的背景图片
         * data 图片数据
         * lens 图片数据的长度
         */
        public bool WGSetEmbeddedWebViewBackground(byte[] imageData, int lens)
        {
            return AndroidConnector.SetEmbeddedWebViewBackground(imageData, lens);
        }


        public void WGReportException(eExceptionType exception_type,
                       string exception_name,
                       string exception_msg,
                       string exception_stack,
                       Dictionary<string, string> ext_info)
        {
            JsonData json = new JsonData();
            foreach (string key in ext_info.Keys)
            {
                json[key] = ext_info[key];
            }
            string jsonData = json.ToJson();
            AndroidConnector.ReportException((int)exception_type, exception_name, exception_msg, exception_stack, jsonData);

        }

        public void WGLogPlatformSDKVersion()
        {
            AndroidConnector.LogPlatformSDKVersion();
        }

        public void WGSetPushAccount(string account) 
        {
            AndroidConnector.SetPushAccount(account);
        }

        public void WGDeletePushAccount(string account) 
        {
            AndroidConnector.DeletePushAccount(account);
        }

        public void WGUnregisterPush() 
        {
            AndroidConnector.UnregisterPush();
        }

        public void WGSetCouldCollectSensitiveInfo(bool couldCollect)
        {
            AndroidConnector.SetCouldCollectSensitiveInfo(couldCollect);
        }

        public void WGSetCollectSensitiveInfo(string jsonInfo)
        {
            AndroidConnector.SetCollectSensitiveInfo(jsonInfo);
        }
    
        public void WGSetSensitiveInfo(string jsonInfo)
        {
            AndroidConnector.SetSensitiveInfo(jsonInfo);
        }

        public void WGOpenGameDataAuthCenter(string openUrl, eMSDK_SCREENDIR screendir, bool fullScreen, string algorithm)
        {
            AndroidConnector.OpenGameDataAuthCenter(openUrl, (int)screendir, fullScreen, algorithm);
        }

        public void WGOpenDeleteAccountUrl(eMSDK_SCREENDIR screenDir, bool isFullScreen, string algorithm)
        {
            AndroidConnector.OpenDeleteAccountUrl((int)screenDir, isFullScreen, algorithm);
        }
        
#endregion 其他接口
        public bool WGQueryAppleMyInfo()
        {
            return false;
        }
#region iOS平台接口

        public void WGSendToQQ(eQQScene scene, string title, string desc, string url, byte[] imgData, int imgDataLen, string messageExt, string gameTag)
        {
            MsdkUtil.Log("Error : This interface is only use in iOS!");
            throw new NotImplementedException();
        }

        public void WGSendToQQWithPhoto(eQQScene scene, byte[] imgData, int imgDataLen)
        {
            MsdkUtil.Log("Error : This interface is only use in iOS!");
            throw new NotImplementedException();
        }

        public long WGAddLocalNotification(LocalMessageIOS localMessage)
        {
            MsdkUtil.Log("Error : This interface is only use in iOS!");
            throw new NotImplementedException();
        }

        public long WGAddLocalNotificationAtFront(LocalMessageIOS localMessage)
        {
            MsdkUtil.Log("Error : This interface is only use in iOS!");
            throw new NotImplementedException();
        }

        public void WGClearLocalNotification(LocalMessageIOS localMessage)
        {
            MsdkUtil.Log("Error : This interface is only use in iOS!");
            throw new NotImplementedException();
        }

        public string WGGetGuestID()
        {
            MsdkUtil.Log("Error : This interface is only use in iOS!");
            throw new NotImplementedException();
        }

        public void WGResetGuestID()
        {
            MsdkUtil.Log("Error : This interface is only use in iOS!");
            throw new NotImplementedException();
        }

        
#endregion iOS平台接口
	}
}   
#endif
