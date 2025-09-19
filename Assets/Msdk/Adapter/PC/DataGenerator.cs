using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using Msdk;

class DataGenerator
{
    LoginRet loginSucc, loginFail;
    RealNameAuthRet authSucc, authFail;
    ShareRet shareSucc, shareFail;
    RelationRet relationSucc, relationFail;
    WakeupRet wakeupRet;
    GroupRet groupSucc, groupFail, groupUnbind;
    CardRet cardSucc, cardFail;
    LocationRet locationSucc, locationFail;
    RelationRet nearbySucc, nearbyFail, relationCleanLoc;
    CallbackStates callback;
    WebviewRet webviewRet;


    static DataGenerator instance;

    public static DataGenerator Instance
    {
        get
        {
            if (instance == null) {
                instance = new DataGenerator();
            }

            return instance;
        }
    }

    private DataGenerator()
    {
        // 读取游戏对事件的配置
        callback = CallbackStates.Load();
        if (callback == null) {
            callback = new CallbackStates();
        }

        // 登录数据
        loginSucc = new LoginRet();
        loginSucc.flag = (int)eFlag.eFlag_Succ;
        loginSucc.desc = ":-) , QQ授权成功 cb: content: success statusCode: 200";
        loginSucc.platform = (int)ePlatform.ePlatform_QQ;
        loginSucc.pf_key = "e9638c4ea7295ae02ebf1f529239bd65";
        loginSucc.open_id = "D51F963BA3E2571ABD8244D95F9B9AD0";
        loginSucc.pf = "desktop_m_qq-73213123-android-00000000-qq-100703379-D51F963BA3E2571ABD8244D95F9B9AD0";
        loginSucc.extra_json = "{\"scope\":\"snsapi_userinfo,snsapi_friend,snsapi_message\"}";
        List<TokenRet> token = new List<TokenRet>();
        token.Add(new TokenRet(eTokenType.eToken_QQ_Access, "B8B01FC091389DF970D534AE6003B54D", 1465110708));
        token.Add(new TokenRet(eTokenType.eToken_QQ_Pay, "48E5BC4FC984CDF842CDC64D32B669E8", 1457939508));
        loginSucc.token = token;

        loginFail = new LoginRet();
        loginFail.flag = (int)eFlag.eFlag_Error;
        loginFail.desc = "Login fail";
        loginFail.platform = (int)ePlatform.ePlatform_QQ;
        loginFail.open_id = "D51F963BA3E2571ABD8244D95F9B9AD0";

        // 实名认证
        authSucc = new RealNameAuthRet();
        authSucc.flag = (int)eFlag.eFlag_Succ;
        authSucc.desc = "实名认证成功";
        authSucc.platform = (int)ePlatform.ePlatform_QQ;

        authFail = new RealNameAuthRet();
        authFail.flag = (int)eFlag.eFlag_Error;
        authFail.desc = "实名认证失败";
        authFail.platform = (int)ePlatform.ePlatform_QQ;
        authFail.errorCode = RealNameAuthErrCode.eErrCode_InvalidIDCard;

        // 分享数据
        shareSucc = new ShareRet();
        shareSucc.flag = (int)eFlag.eFlag_Succ;
        shareSucc.desc = "Share success";
        shareSucc.platform = (int)ePlatform.ePlatform_QQ;
        shareSucc.extInfo = "QQ share extInfo";

        shareFail = new ShareRet();
        shareFail.flag = (int)eFlag.eFlag_Error;
        shareFail.desc = "Share fail";
        shareFail.platform = (int)ePlatform.ePlatform_QQ;
        shareFail.extInfo = "QQ share extInfo";

        // 关系链数据
        relationSucc = new RelationRet();
        relationSucc.flag = (int)eFlag.eFlag_Succ;
        relationSucc.desc = "com.tencent.msdk.remote.api.QueryQQFriends ret: 0 msg: success";
        relationSucc.platform = (int)ePlatform.ePlatform_QQ;
        PersonInfo personInfo = new PersonInfo();
        personInfo.nickName = "demo玩家Blwn2🕶⚽ ➕ ❤ = I love football";
        personInfo.openId = "D51F963BA3E2571ABD8244D95F9B9AD0";
        personInfo.pictureSmall = "http://q.qlogo.cn/qqapp/100703379/D51F963BA3E2571ABD8244D95F9B9AD0/40";
        personInfo.pictureMiddle = "http://q.qlogo.cn/qqapp/100703379/D51F963BA3E2571ABD8244D95F9B9AD0/40";
        personInfo.pictureLarge = "http://q.qlogo.cn/qqapp/100703379/D51F963BA3E2571ABD8244D95F9B9AD0/100";
        personInfo.isFriend = false;
        personInfo.gender = "男";
        relationSucc.persons.Add(personInfo);

        relationCleanLoc = new RelationRet();
        relationCleanLoc.flag = (int)eFlag.eFlag_Succ;
        relationCleanLoc.desc = "{\"msg\":\"success\",\"ret\":\"0\"}";

        relationFail = new RelationRet();
        relationFail.flag = (int)eFlag.eFlag_Error;
        relationFail.desc = "Query relation fail";

        // 第三方拉起游戏数据
        wakeupRet = new WakeupRet();
        wakeupRet.flag = eFlag.eFlag_NeedSelectAccount;
        wakeupRet.platform = (int)ePlatform.ePlatform_QQ;
        wakeupRet.open_id = "7675FCD4A008A6BBA75809D54CFC9BE2";
        wakeupRet.extInfo.Add(new KVPair("platform", "qq_m"));
        wakeupRet.extInfo.Add(new KVPair("leftViewText", "游戏详情"));
        wakeupRet.extInfo.Add(new KVPair("launchfrom", "sq_gamecenter"));
        wakeupRet.extInfo.Add(new KVPair("fling_action_key", "2"));
        wakeupRet.extInfo.Add(new KVPair("current_uin", "7675FCD4A008A6BBA75809D54CFC9BE2"));
        wakeupRet.extInfo.Add(new KVPair("openid", "7675FCD4A008A6BBA75809D54CFC9BE2"));
        wakeupRet.extInfo.Add(new KVPair("fling_code_key", "1099963824"));
        wakeupRet.extInfo.Add(new KVPair("atoken", "B8B01FC091389DF970D534AE6003B54D"));
        wakeupRet.extInfo.Add(new KVPair("ptoken", "48E5BC4FC984CDF842CDC64D32B669E8"));

        // 群相关数据
        groupSucc = new GroupRet();
        groupSucc.flag = (int)eFlag.eFlag_Succ;
        groupSucc.platform = (int)ePlatform.ePlatform_QQ;
        groupSucc.desc = "call WGBindQQGroup finished ，but not sure succ or failed";
        groupSucc.errorCode = 0;
        QQGroup qqgroup = new QQGroup();
        qqgroup._groupId = "groupId0";
        qqgroup._groupName = "_groupName0";
        QQGroup qqgroup1 = new QQGroup();
        qqgroup1._groupId = "groupId1";
        qqgroup1._groupName = "_groupName1";
        List<QQGroup> qqGroups = new List<QQGroup>();
        qqGroups.Add(qqgroup);
        qqGroups.Add(qqgroup1);
        QQGroupInfoV2 qqGroupInfoV2 = new QQGroupInfoV2();
        qqGroupInfoV2._guildId = "1"; qqGroupInfoV2._guildName = "name";
        qqGroupInfoV2._relation = 3; qqGroupInfoV2._qqGroups = qqGroups;
        groupSucc.mQQGroupInfoV2 = qqGroupInfoV2;

        groupFail = new GroupRet();
        groupFail.flag = (int)eFlag.eFlag_Error;
        groupFail.desc = "com.tencent.msdk.qq.group.QueryQQGroup ret: -10000 msg: 2002,没有绑定记录，请检查公会id和区id。";
        groupFail.errorCode = 2002;

        // 微信卡卷数据
        cardSucc = new CardRet();
        cardSucc.flag = (int)eFlag.eFlag_Succ;
        cardSucc.platform = (int)ePlatform.ePlatform_None;
        cardSucc.open_id = "oGRTijiYT4CaRfydUbDFR25kAmwQ";
        cardSucc.extInfo.Add(new KVPair("wxapi_add_card_to_wx_card_list", "{\"card_list\":[{\"card_id\":\"pe7Gmjg3EpKtnwzNAGHMGJhNKSo4\",\"" +
                "card_ext\":\"{\\\"code\\\":\\\"\\\",\\\"openid\\\":\\\"\\\",\\\"timestamp\\\":\\\"1111111111\\\"," + 
                "\\\"signature\\\":\\\"sdffsfffff\\\"}\",\"is_succ\":0}]}"));

        cardFail = new CardRet();
        cardFail.flag = (int)eFlag.eFlag_Error;
        cardFail.platform = (int)ePlatform.ePlatform_None;
        cardFail.desc = "Bind card fail";

        // Lbs数据
        locationSucc = new LocationRet();
        locationSucc.flag = (int)eFlag.eFlag_Succ;
        locationSucc.platform = (int)ePlatform.ePlatform_QQ;
        locationSucc.desc = "com.tencent.msdk.remote.api.GetLocationInfo ret: 0 msg: success";
        locationSucc.latitude = 22.547855;
        locationSucc.longitude = 113.94498;

        locationFail = new LocationRet();
        locationFail.flag = (int)eFlag.eFlag_LbsNeedOpenLocationService;
        locationFail.platform = (int)ePlatform.ePlatform_None;
        locationFail.desc = "location service is closed!";
        locationFail.latitude = 0;
        locationFail.longitude = 0;

        nearbySucc = new RelationRet();
        relationSucc.flag = (int)eFlag.eFlag_Succ;
        relationSucc.desc = "com.tencent.msdk.remote.api.QueryNearbyPlayer ret: 0 msg: success";
        relationSucc.platform = (int)ePlatform.ePlatform_QQ;
        personInfo = new PersonInfo();
        personInfo.nickName = "demo玩家B";
        personInfo.openId = "D51F963BA3E2571ABD8244D95F9B9AD0";
        personInfo.distance = 2;
        personInfo.timestamp = 1458728556;
        personInfo.isFriend = true;
        personInfo.gender = "男";
        personInfo.pictureSmall = "http://q.qlogo.cn/qqapp/100703379/D51F963BA3E2571ABD8244D95F9B9AD0/40";
        personInfo.pictureMiddle = "http://q.qlogo.cn/qqapp/100703379/D51F963BA3E2571ABD8244D95F9B9AD0/40";
        personInfo.pictureLarge = "http://q.qlogo.cn/qqapp/100703379/D51F963BA3E2571ABD8244D95F9B9AD0/100";
        relationSucc.persons.Add(personInfo);
        personInfo = new PersonInfo();
        personInfo.nickName = "MSDK-qingcui2";
        personInfo.openId = "57388C7E931F9D12AC8C86E9C78655C8";
        personInfo.distance = 2;
        personInfo.timestamp = 1458728597;
        personInfo.isFriend = true;
        personInfo.gender = "女";
        personInfo.pictureSmall = "http://q.qlogo.cn/qqapp/100703379/57388C7E931F9D12AC8C86E9C78655C8/40";
        personInfo.pictureMiddle = "http://q.qlogo.cn/qqapp/100703379/57388C7E931F9D12AC8C86E9C78655C8/40";
        personInfo.pictureLarge = "http://q.qlogo.cn/qqapp/100703379/57388C7E931F9D12AC8C86E9C78655C8/100";
        relationSucc.persons.Add(personInfo);

        nearbyFail = new RelationRet();
        nearbyFail.flag = (int)eFlag.eFlag_LbsNeedOpenLocationService;
        nearbyFail.platform = (int)ePlatform.ePlatform_None;
        nearbyFail.desc = "location service is closed!";

        //web View 数据
        webviewRet = new WebviewRet();
        webviewRet.flag = 0;
        webviewRet.desc = "浏览器打开关闭回调，0"+eFlag.eFlag_Succ+"为打开回调，"+eFlag.eFlag_Webview_closed+"为关闭回调";
    	
		groupUnbind = new GroupRet ();
		groupUnbind.flag = 0;
		groupUnbind.desc = "解绑成功";
	}

    private int ParseConfig(string value)
    {
        try {
            if (String.IsNullOrEmpty(value) || !value.StartsWith("eFlag")) {
                return eFlag.eFlag_Succ;
            }
            Type t = typeof(eFlag);
            System.Reflection.FieldInfo field = t.GetField(value);
            int flag = (int)field.GetValue(new eFlag());
            return flag;
        } catch (Exception e) {
            Debug.LogWarning(e.Message);
            return eFlag.eFlag_Succ;
        }
    }

    public LoginRet LoginData
    {
        get
        {
            int flag = ParseConfig(callback.loginState);
            if (flag == eFlag.eFlag_Succ) {
                return loginSucc;
            } else {
                loginFail.flag = flag;
                return loginFail;
            }
        }
    }

    public RealNameAuthRet AuthData
    {
        get
        {
            int flag = ParseConfig(callback.authState);
            if (flag == eFlag.eFlag_Succ) {
                authSucc.platform = LoginData.platform;
                return authSucc;
            } else {
                authFail.platform = LoginData.platform;
                authFail.flag = flag;
                return authFail;
            }
        }
    }

    public ShareRet ShareData
    {
        get
        {
            int flag = ParseConfig(callback.shareState);
            if (flag == eFlag.eFlag_Succ) {
                return shareSucc;
            } else {
                shareFail.flag = flag;
                return shareFail;
            }
        }
    }

    public RelationRet RelationData
    {
        get
        {
            int flag = ParseConfig(callback.relationState);
            if (flag == eFlag.eFlag_Succ) {
                return relationSucc;
            } else {
                relationFail.flag = flag;
                return relationFail;
            }
        }
    }

    public WakeupRet WakeupData
    {
        get
        {
            string wakeup = callback.wakeupState;
            if (!String.IsNullOrEmpty(wakeup) && wakeup.StartsWith("eFlag")) {
                int flag = ParseConfig(wakeup);
                wakeupRet.flag = flag;
                return wakeupRet;
            } else {
                return null;
            }
        }
    }

    public GroupRet GroupData
    {
        get
        {
            int flag = ParseConfig(callback.groupState);
            if (flag == eFlag.eFlag_Succ) {
                return groupSucc;
            } else {
                groupFail.flag = flag;
                return groupFail;
            }
        }
    }
		

    public CardRet CardData
    {
        get
        {
            int flag = ParseConfig(callback.cardState);
            if (flag == eFlag.eFlag_Succ) {
                return cardSucc;
            } else {
                cardFail.flag = flag;
                return cardFail;
            }
        }
    }

    public LocationRet LocationData
    {
        get
        {
            int flag = ParseConfig(callback.locationState);
            if (flag == eFlag.eFlag_Succ) {
                return locationSucc;
            } else {
                locationFail.flag = flag;
                return locationFail;
            }
        }
    }

    public RelationRet nearbyData
    {
        get
        {
            int flag = ParseConfig(callback.nearbyState);
            if (flag == eFlag.eFlag_Succ) {
                return nearbySucc;
            } else {
                nearbyFail.flag = flag;
                return nearbyFail;
            }
        }
    }
    public RelationRet cleanLocationData
    {
        get {
            return relationCleanLoc;
        }
    }
    public WebviewRet openwebviewRet
    {
        get
        {
            return webviewRet;
        }
    }
}
