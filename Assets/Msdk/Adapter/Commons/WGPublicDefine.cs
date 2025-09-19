using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using LitJson;
using Msdk;

namespace Msdk
{
    public delegate void LoginDelegate(LoginRet ret);
    public delegate void ChannelPermissionAuthDelegate(LoginRet ret);
    public delegate void CheckULDelegate(CheckULRet ret);

    public delegate void WakeupDelegate(WakeupRet ret);
    public delegate void ShareDelegate(ShareRet ret);
    public delegate void RelationDelegate(RelationRet ret);
    public delegate void NearbyDelegate(RelationRet ret);
    public delegate void LocateDelegate(LocationRet ret);
    public delegate void FeedbackDelegate(int flag, String desc);
    public delegate String CrashReportMessageDelegate();
    public delegate byte[] CrashReportDataDelegate();
    public delegate void ShowADDelegate(ADRet ret);
    public delegate void QueryGroupDelegate(GroupRet ret);
    public delegate void CreateWXGroupDelegate(GroupRet ret);
    public delegate void JoinWXGroupDelegate(GroupRet ret);
    public delegate void RealNameAuthDelegate(RealNameAuthRet ret);
    public delegate void JoinQQGroupDelegate(GroupRet ret);
    public delegate void QueryWXGroupStatusDelegate(GroupRet ret);
    public delegate void WebviewDelegate(WebviewRet ret);

    #region EmbeddedWebView回调
    public delegate void OnJsCallbackDelegate(String jsParams);
    public delegate void OnWebProgressChangedDelegate(String url, float progress);
    public delegate void OnWebCloseDelegate();
    #endregion


    #region 加绑群v2回调
    public delegate void OnCreateGroupV2Delegate(GroupRet ret);
    public delegate void OnJoinGroupV2Delegate(GroupRet ret);
    public delegate void OnQueryGroupInfoV2Delegate(GroupRet ret);
    public delegate void OnUnbindGroupV2Delegate(GroupRet ret);
    public delegate void OnGetGroupCodeV2Delegate(GroupRet ret);
    public delegate void OnQueryBindGuildV2Delegate(GroupRet ret);
    public delegate void OnBindExistGroupV2Delegate(GroupRet ret);
    public delegate void OnGetGroupListV2Delegate(GroupRet ret);
    public delegate void OnRemindGuildLeaderV2Delegate(GroupRet ret);
    #endregion


    public delegate void AddWXCardDelegate(CardRet ret);
    public delegate void ADBackPressedDelegate(ADRet ret);
    public delegate void BindGroupDelegate(GroupRet ret);
    public delegate void UnbindGroupDelegate(GroupRet ret);
    public delegate void QueryQQGroupKeyDelegate(GroupRet ret);
    public delegate void CheckUpdateDelegate(long newApkSize, string newFeature, long patchSize,
        int status, string updateDownloadUrl, int updateMethod);
    public delegate void DownloadAppProgressDelegate(long receiveDataLen, long totalDataLen);
    public delegate void DownloadAppStateDelegate(int state, int errorCode, string errorMsg);
    public delegate void DownloadYYBProgressDelegate(string url, long receiveDataLen, long totalDataLen);
    public delegate void DownloadYYBStateDelegate(string url, int state, int errorCode, string errorMsg);

    public delegate void OnLocationGotCountryFromIPDelegate(CountryFromIPRet ret);
    public delegate void OnLocationGetIPInfoDelegate(GetIPInfoRet ret);

    #region TPNS相关回调
    public delegate void OnReceiveNotificationResponseDelegate(string userInfo);// TPNS点击消息的回调
    #endregion



    public enum ePlatform
    {
        ePlatform_None = 0,
        ePlatform_Weixin = 1,
        ePlatform_QQ = 2,
        ePlatform_WTLogin = 3,
        ePlatform_QQHall = 4,
        ePlatform_Guest = 5,
        ePlatform_Apple = 7,
        ePlatform_WeixinVideoLive = 8 //微信视频号授权
    }
    /***
     * eWakeupPlatform为拉起游戏平台，指唤醒游戏的平台，由于历史原因，其中的微信，手Q枚举值需要与
     * ePlatform中的枚举值保持一致，后续新增的拉起平台枚举值不再使用0，3，5，6.
     */
    public enum eWakeupPlatform {
	    eWakeupPlatform_Weixin = 1,
	    eWakeupPlatform_QQ = 2,
	    eWakeupPlatform_QQHall = 4,
        eWakeupPlatform_TencentMsdk = 7 
    };


    public enum eULCheckStep
    {
        eULCheckStepParams = 0,
        eULCheckStepSystemVersion = 1,
        eULCheckStepWechatVersion = 2,
        eULCheckStepSDKInnerOperation = 3,
        eULCheckStepLaunchWechat = 4,
        eULCheckStepBackToCurrentApp = 5,
        eULCheckStepFinal = 6,
    };
   
    public enum eWechatScene
    {
        WechatScene_Session = 0,            //聊天界面
        WechatScene_Timeline = 1,           //朋友圈
        WechatScene_Favorite = 2,           //收藏
        WechatScene_SpecifiedSession = 3,   //指定联系人
        WechatScene_State = 4               //状态
    }
    public enum eMiniProgramType
    {
        ProgramType_Release = 0,
        ProgramType_Test = 1,
        ProgramType_Preview = 2,
    }


    public enum eQQScene
    {
        QQScene_None = 0,
        QQScene_QZone = 1,
        QQScene_Session = 2
    }

    public enum eADType
    {
        Type_Pause = 1,
        Type_Stop = 2
    }



    public enum eApiName
    {
        eApiName_WGSendToQQWithPhoto = 0,
        eApiName_WGJoinQQGroup = 1,
        eApiName_WGAddGameFriendToQQ = 2,
        eApiName_WGBindQQGroup = 3
    }

    public enum eMSDK_SCREENDIR
    {
        eMSDK_SCREENDIR_SENSOR = 0, /* 横竖屏 */
        eMSDK_SCREENDIR_PORTRAIT = 1, /* 竖屏 */
        eMSDK_SCREENDIR_LANDSCAPE = 2 /* 横屏 */
    }

    public enum QQVersion
    {
        kQQUninstall,
        kQQVersion3_0,
        kQQVersion4_0,
        kQQVersion4_2_1,
        kQQVersion4_5,
        kQQVersion4_6,
        kQQVersion4_7,
    }

    /* 上报日志到bugly的日志级别 */
    public enum eBuglyLogLevel
    {
        eBuglyLogLevel_S = 0, /* Silent */
        eBuglyLogLevel_E = 1, /* Error */
        eBuglyLogLevel_W = 2, /* Warnning */
        eBuglyLogLevel_I = 3, /* Info */
        eBuglyLogLevel_D = 4, /* Debug */
        eBuglyLogLevel_V = 5, /* Verbose */
    }

    public enum eIDType
    {
        /*0:身份证
          1:港澳通行证
          2:港澳台身份证
          3:护照
          4:军警证*/
        eIDType_IDCards = 0,
        eIDType_HKMacaoPass = 1,
        eIDType_HKMacaoTaiwanID = 2,
        eIDType_Passport = 3,
        eIDType_PoliceCertificate = 4,
    }

    public enum eStatusType
    {
        ISCREATED = 0, // 是否建群
        ISJOINED = 1,  // 是否加群
    }

    public enum eExceptionType
    {
        eException_CSharp = 4,  // C#异常
        eException_JS = 5, //js异常
        eException_Lua = 6 //lua异常
    }

    public class eFlag
    {
        public const int eFlag_Succ = 0;
        public const int eFlag_Cloud_NoMatch = 1;              // 云控不匹配
        public const int eFlag_QQ_NoAcessToken = 1000;          // QQ&QZone login fail and can't get accesstoken
        public const int eFlag_QQ_UserCancel = 1001;            // 用户取消
        public const int eFlag_QQ_LoginFail = 1002;             // 登录失败
        public const int eFlag_QQ_NetworkErr = 1003;            // 网络错误
        public const int eFlag_QQ_NotInstall = 1004;            // QQ未安装
        public const int eFlag_QQ_NotSupportApi = 1005;         // QQ版本不支持此api
        public const int eFlag_QQ_AccessTokenExpired = 1006;    // accesstoken过期
        public const int eFlag_QQ_PayTokenExpired = 1007;       // paytoken过期
        public const int eFlag_QQ_UnRegistered = 1008;          // 没有在qq注册

        public const int eFlag_QQ_MessageTypeErr = 1009;        // QQ消息类型错误
        public const int eFlag_QQ_MessageContentEmpty = 1010;   // QQ消息为空
        public const int eFlag_QQ_MessageContentErr = 1011;     // QQ消息不可用（超长或其他）

        public const int eFlag_WX_NotInstall = 2000;            // Weixin is not installed
        public const int eFlag_WX_NotSupportApi = 2001;         // Weixin don't support api
        public const int eFlag_WX_UserCancel = 2002;            // Weixin user has cancelled
        public const int eFlag_WX_UserDeny = 2003;              // Weixin user has denys
        public const int eFlag_WX_LoginFail = 2004;             // Weixin login faild
        public const int eFlag_WX_RefreshTokenSucc = 2005;      // Weixin 刷新票据成功
        public const int eFlag_WX_RefreshTokenFail = 2006;      // Weixin 刷新票据失败
        public const int eFlag_WX_AccessTokenExpired = 2007;    // Weixin accessToken过期
        public const int eFlag_WX_RefreshTokenExpired = 2008;   // Weixin refresh过期

        public const int eFlag_WX_Group_HasNoAuthority = 2009;   //游戏没有建群权限
        public const int eFlag_WX_Group_ParameterError = 2010;   //参数检查错误
        public const int eFlag_WX_Group_HadExist = 2011;         //微信群已存在
        public const int eFlag_WX_Group_AmountBeyond = 2012;     //建群数量超过上限
        public const int eFlag_WX_Group_IDNotExist = 2013;       //群ID不存在
        public const int eFlag_WX_Group_IDHadCreatedToday = 2014;//群ID今天已经建过群，每天每个ID只能创建一次群聊
        public const int eFlag_WX_Group_JoinAmountBeyond = 2015; //加群数量超限，每天每个ID最多可加2个群
        public const int eFlag_WX_Group_NotInGroup = 2016;       //不在群中，发送群消息时若不在群中(被群主踢出群等)
        public const int eFlag_Error = -1;

        /** 自动登录失败, 需要重新授权, 包含本地票据过期, 刷新失败登所有错误 */
        public const int eFlag_Local_Invalid = -2;
        /** 不在白名单 */
        public const int eFlag_NotInWhiteList = -3;
        /** 需要引导用户开启定位服务 */
        public const int eFlag_LbsNeedOpenLocationService = -4;
        /** 定位失败，原因待确认	 */
        public const int eFlag_LbsLocateFail = -5;
        /*for WGOpenUrl*/
        public const int eFlag_UrlTooLong = -6;
        /* 该功能在Guest模式下不可使用 */
        public const int eFlag_InvalidOnGuest = -7;
        /*网络请求出错(发送失败，超时，无回包等*/
        public const int eFlag_NetworkError = -8;
        /*无操作权限*/
        public const int inteFlag_UnPermission = -9;
        /*非法参数*/
        public const int eFlag_IllegalParams = -10;
        /*定位失败（超时）*/
        public const int eFlag_LbsLocateTimeout = -11;

        /* 快速登陆相关返回值 */
        /**需要进入登陆页 */
        public const int eFlag_NeedLogin = 3001;
        /**使用URL登陆成功 */
        public const int eFlag_UrlLogin = 3002;
        /**需要弹出异帐号提示 */
        public const int eFlag_NeedSelectAccount = 3003;
        /**通过URL将票据刷新 */
        public const int eFlag_AccountRefresh = 3004;
        /**需要实名认证**/
        public const int eFlag_Need_Realname_Auth = 3005;
		/**正在进行MSDK实名认证，请勿调用游戏自定义的实名认证界面**/
        public const int eFlag_Need_MSDK_Realname_Auth = 3006;

        /* Guest的票据失效 */
        public const int eFlag_Guest_AccessTokenInvalid = 4001;
        /* Guest模式登录失败 */
        public const int eFlag_Guest_LoginFailed = 4002;
        /* Guest模式注册失败 */
        public const int eFlag_Guest_RegisterFailed = 4003;
        
        /*添加正在检查token的逻辑 */
        public const int eFlag_Checking_Token = 5001;
        public const int eFlag_Logining = 5002;            //正在登录中，请等待登录回调后再操作
        public const int eFlag_Login_Timeout = 5003;       //正在登录中，请等待登录回调后再操作
        public const int eFlag_RequestTooFrequently = 5004;//调用接口太频繁

        /* 关闭内置浏览器*/
        public const int eFlag_Webview_closed = 6001;
        /*打开注销账号的url无效*/
        public const int eFlag_Invalid_Delete_Account_URL = 6002;
        /*注销账号操作时未登录或渠道不支持*/
        public const int eFlag_Invalid_Login_Channel = 6003;
        /* 传递的js消息*/
        public const int eFlag_Webview_page_event = 7000;

        public const int eFlag_CloudGame_Params_Error = 7001;   // 云游戏参数错误

        /* 未使用Xcode 11 编译的 MSDK (for iOS) */
        public const int eFlag_Apple_NeedCompileXcode11 = 8001;
        /* iOS 12 及以下版本不支持 Apple 登录 (for iOS) */
        public const int eFlag_Apple_NotSupport = 8002;
        /* Apple 登录用户取消 (for iOS) */
        public const int eFlag_Apple_UserCancel = 8003;
        /* Apple 登录失败 (for iOS) */
        public const int eFlag_Apple_LoginFailed = 8004;
        /*  Apple access token 过期 (for iOS) */
        public const int eFlag_Apple_AccessTokenExpired = 8005;
        /*  Apple 登录,用户撤销了该授权 (for iOS) */
        public const int eFlag_Apple_Revoked = 8006;
        /* Apple 登录，用户没有对其授权 (for iOS) */
        public const int eFlag_Apple_NotFound = 8007;
        /* Apple 登录，用户已经授权 (for iOS, 内部使用) */
        public const int eFlag_Apple_Authorized = 8008;
        /* Apple 登录，未知错误 */
        public const int eFlag_Apple_Unknown = 8009;
        /*  Apple 登录，网络错误 */
        public const int eFlag_Apple_NetworkError = 8010; 


    }

    public class RealNameAuthErrCode
    {
        public const int eErrCode_SystemError = 1;          //系统错误
        public const int eErrCode_NoAuth = 2;               //没有权限
        public const int eErrCode_RequestFrequently = 3;    //访问频繁，稍后再试
        public const int eErrCode_NoRecord = 4;             //没有该记录
        public const int eErrCode_AlreadyRegisted = 5;      //用户已注册
        public const int eErrCode_BindCountLimit = 6;       //证件绑定超过限制
        public const int eErrCode_UserNoRegist = 7;         //用户没注册(修改时可能返回)
        public const int eErrCode_InvalidParam = 100;       //非法参数输入
        public const int eErrCode_InvalidIDCard = 101;      //非法的身份证，当证件类型为身份证时会校验
        public const int eErrCode_InvalidBirth = 102;       //非法的生日，当证件为非身份证时会校验
        public const int eErrCode_InvalidChineseName = 103; //非法的中文名字
        public const int eErrCode_InvalidToken = 104;       //token过期或不合法
    }

    public class RealNameAuthInfo
    {
        public String name { get; set; }                    //用户名称
        public eIDType identityType { get; set; }           //ID类型
        public String identityNum { get; set; }             //ID号码
        public int provinceID { get; set; }                 //省份ID，选填
        public String city { get; set; }                    //城市
    }

    public class eRelationRetType
    {
        public const int eRet_QueryMyInfo = 0;          // 查询个人信息
        public const int eRet_QueryGameFriends = 1;     //查询同玩好友
        public const int eRet_QueryUnionID = 2;         // 查询unionID
    }

    public class WXMessageTypeInfo
    {
        public string type { get; set; }

        public WXMessageTypeInfo()
        {
            type = "";
        }
    }

    public class TypeInfoImage : WXMessageTypeInfo
    {
        public string pictureUrl { get; set; }
        public int height { get; set; }
        public int width { get; set; }

        public TypeInfoImage()
        {
            type = "TypeInfoImage";
            pictureUrl = "";
        }
    }

    public class TypeInfoVideo : WXMessageTypeInfo
    {
        public string pictureUrl { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string mediaUrl { get; set; }

        public TypeInfoVideo()
        {
            type = "TypeInfoVideo";
            pictureUrl = "";
            mediaUrl = "";
        }
    }

    public class TypeInfoLink : WXMessageTypeInfo
    {
        public string pictureUrl { get; set; }
        public string targetUrl { get; set; }

        public TypeInfoLink()
        {
            type = "TypeInfoLink";
            pictureUrl = "";
            targetUrl = "";
        }
    }

    public class TypeInfoText : WXMessageTypeInfo
    {
        public TypeInfoText()
        {
            type = "TypeInfoText";
        }
    }

    public class WXMessageButton
    {
        public string type { get; set; }

        public WXMessageButton()
        {
            type = "";
        }
    }

    public class ButtonApp : WXMessageButton
    {
        public string name { get; set; }
        public string messageExt { get; set; }

        public ButtonApp()
        {
            type = "ButtonApp";
            name = "";
            messageExt = "";
        }
    }

    public class ButtonWebview : WXMessageButton
    {
        public string name { get; set; }
        public string webViewUrl { get; set; }

        public ButtonWebview()
        {
            type = "ButtonWebview";
            name = "";
            webViewUrl = "";
        }
    }

    public class ButtonRankView : WXMessageButton
    {
        public string name { get; set; }
        public string title { get; set; }
        public string rankViewButtonName { get; set; }
        public string messageExt { get; set; }

        public ButtonRankView()
        {
            type = "ButtonRankView";
            name = "";
            title = "";
            messageExt = "";
        }
    }


    public abstract class CallbackRet
    {
        public int flag = eFlag.eFlag_Error;
        public string desc = "";
        public int platform = 0;

        /* 以下方法是为了能使用LitJson的自动将Json映射为Object方法 */
        public int _flag
        {
            get { return flag; }
            set { flag = value; }
        }
        public string _desc
        {
            get { return desc; }
            set { desc = value; }
        }
        public int _platform
        {
            get { return platform; }
            set { platform = value; }
        }

        public CallbackRet(int platform, int flag, string desc)
        {
            this.platform = platform;
            this.flag = flag;
            this.desc = desc;
        }
        public CallbackRet()
        {
        }

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine("flag:" + flag);
            msg.AppendLine("desc:" + desc);
            msg.AppendLine("platform:" + platform);
            return msg.ToString();
        }
    }


    public class eTokenType
    {
        public const int eToken_QQ_Access = 1;
        public const int eToken_QQ_Pay = 2;
        public const int eToken_WX_Access = 3;
        public const int eToken_WX_Code = 4;
        public const int eToken_WX_Refresh = 5;
        public const int eToken_Guest_Access = 6;
        public const int eToken_QQ_Refresh = 7;
        public const int eToken_Apple_Access = 8;
        public const int eToken_Apple_Refresh = 9;
    }

    public class ePermission
    {
        public const int eOPEN_NONE = 0;
        public const int eOPEN_PERMISSION_GET_USER_INFO = 0x2;
        public const int eOPEN_PERMISSION_GET_SIMPLE_USER_INFO = 0x4;
        public const int eOPEN_PERMISSION_ADD_ALBUM = 0x8;
        public const int eOPEN_PERMISSION_ADD_IDOL = 0x10;
        public const int eOPEN_PERMISSION_ADD_ONE_BLOG = 0x20;
        public const int eOPEN_PERMISSION_ADD_PIC_T = 0x40;
        public const int eOPEN_PERMISSION_ADD_SHARE = 0x80;
        public const int eOPEN_PERMISSION_ADD_TOPIC = 0x100;
        public const int eOPEN_PERMISSION_CHECK_PAGE_FANS = 0x200;
        public const int eOPEN_PERMISSION_DEL_IDOL = 0x400;
        public const int eOPEN_PERMISSION_DEL_T = 0x800;
        public const int eOPEN_PERMISSION_GET_FANSLIST = 0x1000;
        public const int eOPEN_PERMISSION_GET_IDOLLIST = 0x2000;
        public const int eOPEN_PERMISSION_GET_INFO = 0x4000;
        public const int eOPEN_PERMISSION_GET_OTHER_INFO = 0x8000;
        public const int eOPEN_PERMISSION_GET_REPOST_LIST = 0x10000;
        public const int eOPEN_PERMISSION_LIST_ALBUM = 0x20000;
        public const int eOPEN_PERMISSION_UPLOAD_PIC = 0x40000;
        public const int eOPEN_PERMISSION_GET_VIP_INFO = 0x80000;
        public const int eOPEN_PERMISSION_GET_VIP_RICH_INFO = 0x100000;
        public const int eOPEN_PERMISSION_GET_INTIMATE_FRIENDS_WEIBO = 0x200000;
        public const int eOPEN_PERMISSION_MATCH_NICK_TIPS_WEIBO = 0x400000;
        public const int eOPEN_PERMISSION_GET_APP_FRIENDS = 0x800000;
        public const int eOPEN_ALL = 0xffffff;
    }


    public class eMSG_NOTICETYPE
    {
        public const int eMSG_NOTICETYPE_ALERT = 0;
        public const int eMSG_NOTICETYPE_SCROLL = 1;
        public const int eMSG_NOTICETYPE_ALL = 2;
    }

    public class eMSG_CONTENTTYPE
    {
        public const int eMSG_CONTENTTYPE_TEXT = 0; /* 文本公告 */
        public const int eMSG_CONTENTTYPE_IMAGE = 1; /* 图片公告 */
        public const int eMSG_CONTENTTYPE_WEB = 2; /* 网页公告 */
    }

    public class TMSelfUpdateUpdateInfo
    {
        public const int STATUS_OK = 0;
        public const int STATUS_CHECKUPDATE_FAILURE = 1;
        public const int STATUS_CHECKUPDATE_RESPONSE_IS_NULL = 2;

        public const int UpdateMethod_NoUpdate = 0;
        public const int UpdateMethod_Normal = 1;
        public const int UpdateMethod_ByPatch = 2;
    }

    public class TMAssistantDownloadTaskState
    {
        public const int DownloadSDKTaskState_WAITING = 1;
        public const int DownloadSDKTaskState_DOWNLOADING = 2;
        public const int DownloadSDKTaskState_SUCCEED = 4;
        public const int DownloadSDKTaskState_PAUSED = 3;
        public const int DownloadSDKTaskState_FAILED = 5;
        public const int DownloadSDKTaskState_DELETE = 6;
    }

    public class TMSelfUpdateTaskState
    {
        public const int SelfUpdateSDKTaskState_SUCCESS = 0;
        public const int SelfUpdateSDKTaskState_DOWNLOADING = 1;
        public const int SelfUpdateSDKTaskState_FAILURE = 2;
    }

    public class TMYYBInstallState
    {
        public const int LOWWER_VERSION_INSTALLED = 2;
        public const int UN_INSTALLED = 1;
        public const int ALREADY_INSTALLED = 0;
    }

    public class TMAssistantDownloadErrorCode
    {
        public const int DownloadSDKErrorCode_CLIENT_PROTOCOL_EXCEPTION = 732;
        public const int DownloadSDKErrorCode_CONNECT_TIMEOUT = 601;
        public const int DownloadSDKErrorCode_HTTP_LOCATION_HEADER_IS_NULL = 702;
        public const int DownloadSDKErrorCode_INTERRUPTED = 600;
        public const int DownloadSDKErrorCode_IO_EXCEPTION = 606;
        public const int DownloadSDKErrorCode_NONE = 0;
        public const int DownloadSDKErrorCode_PARSER_CONTENT_FAILED = 704;
        public const int DownloadSDKErrorCode_RANGE_NOT_MATCH = 706;
        public const int DownloadSDKErrorCode_REDIRECT_TOO_MANY_TIMES = 709;
        public const int DownloadSDKErrorCode_RESPONSE_IS_NULL = 701;
        public const int DownloadSDKErrorCode_SET_RANGE_FAILED = 707;
        public const int DownloadSDKErrorCode_SOCKET_EXCEPTION = 605;
        public const int DownloadSDKErrorCode_SOCKET_TIMEOUT = 602;
        public const int DownloadSDKErrorCode_SPACE_NOT_ENOUGH = 730;
        public const int DownloadSDKErrorCode_TOTAL_SIZE_NOT_SAME = 705;
        public const int DownloadSDKErrorCode_UNKNOWN_EXCEPTION = 604;
        public const int DownloadSDKErrorCode_UNKNOWN_HOST = 603;
        public const int DownloadSDKErrorCode_URL_EMPTY = 731;
        public const int DownloadSDKErrorCode_URL_HOOK = 708;
        public const int DownloadSDKErrorCode_URL_INVALID = 700;
        public const int DownloadSDKErrorCode_WRITE_FILE_FAILED = 703;
        public const int DownloadSDKErrorCode_WRITE_FILE_NO_ENOUGH_SPACE = 710;
        public const int DownloadSDKErrorCode_WRITE_FILE_SDCARD_EXCEPTION = 711;
    }

    public class TokenRet
    {
        public int type = 0;
        public string value = "";
        public long expiration = 0;

        public int _type
        {
            get { return type; }
            set { type = value; }
        }
        public string _value
        {
            get { return value; }
            set { this.value = value; }
        }
        public long _expiration
        {
            get { return expiration; }
            set { expiration = (long)value; }
        }
        public TokenRet()
        {

        }

        public TokenRet(int type, string value, long expiration)
            : base()
        {
            this.type = type;
            this.value = value;
            this.expiration = expiration;
        }
    }

    public class RealNameAuthRet : CallbackRet
    {
        public int errorCode;

        public int _errorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }

        public static RealNameAuthRet ParseJson(string json)
        {
            try
            {
                RealNameAuthRet ret = JsonMapper.ToObject<RealNameAuthRet>(json);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.Log("errro:" + ex.Message + "\n" + ex.StackTrace);
            }
            return new RealNameAuthRet();
        }

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder(base.ToString());
            msg.AppendLine("errorCode:" + errorCode);
            return msg.ToString();
        }
    }

    public class LoginRet : CallbackRet
    {
        public string open_id = "";
        public string user_id = "";
        public string pf = "";
        public string pf_key = "";
        public List<TokenRet> token = new List<TokenRet>();
        public string prajna_ext = "";
        public string extra_json = "";

        public string _open_id
        {
            get { return open_id; }
            set { open_id = value; }
        }
        public string _user_id
        {
            get { return user_id; }
            set { user_id = value; }
        }
        public string _pf
        {
            get { return pf; }
            set { pf = value; }
        }
        public string _pf_key
        {
            get { return pf_key; }
            set { pf_key = value; }
        }

        public List<TokenRet> _token
        {
            get { return token; }
            set { token = value; }
        }

        public string _prajna_ext
        {
            get { return prajna_ext; }
            set { prajna_ext = value; }
        }

        public string _extra_json
        {
            get { return extra_json; }
            set { extra_json = value; }
        }

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder(base.ToString());
            msg.AppendLine("open_id:" + this.open_id);
            msg.AppendLine("pf:" + this.pf);
            msg.AppendLine("pf_key:" + this.pf_key);
            msg.AppendLine("user_id:" + this.user_id);
            for (int i = 0; i < this.token.Count; i++)
            {
                string type = "";
                switch (this.token[i].type)
                {
                    case eTokenType.eToken_QQ_Access:
                        type = "eToken_QQ_Access";
                        break;
                    case eTokenType.eToken_QQ_Pay:
                        type = "eToken_QQ_Pay";
                        break;
                    case eTokenType.eToken_WX_Access:
                        type = "eToken_WX_Access";
                        break;
                    case eTokenType.eToken_WX_Code:
                        type = "eToken_WX_Code";
                        break;
                    case eTokenType.eToken_WX_Refresh:
                        type = "eToken_WX_Refresh";
                        break;
                    case eTokenType.eToken_Guest_Access:
                        type = "eToken_Guest_Access";
                        break;
                    case eTokenType.eToken_Apple_Access:
                        type = "eToken_Apple_Access";
                        break;
                    case eTokenType.eToken_Apple_Refresh:
                        type = "eToken_Apple_Refresh";
                        break;
                    default:
                        type = "errorType";
                        break;
                }
                msg.AppendLine(type + ":" + this.token[i].value);
                msg.AppendLine("expiration" + ":" + this.token[i].expiration);
            }
            msg.AppendLine ("prajna_ext:" + this.prajna_ext);
            msg.AppendLine("extra_json:" + this.extra_json);
            return msg.ToString();
        }

        public string GetTokenByType(int type)
        {
            for (int i = 0; i < this.token.Count; i++)
            {
                if (this.token[i].type == type)
                {
                    return this.token[i].value;
                }
            }
            return "";
        }

        public string GetAccessToken()
        {
            int plat = this.platform;
            string accessToken = "";
            if (plat == (int)ePlatform.ePlatform_QQ || plat == (int)ePlatform.ePlatform_QQHall)
            {
                accessToken = this.GetTokenByType(eTokenType.eToken_QQ_Access);
            }
            else if (plat == (int)ePlatform.ePlatform_Weixin)
            {
                accessToken = this.GetTokenByType(eTokenType.eToken_WX_Access);
            }
            else if (plat == (int)ePlatform.ePlatform_Guest)
            {
                accessToken = this.GetTokenByType(eTokenType.eToken_Guest_Access);
            }
            else if (plat == (int)ePlatform.ePlatform_Apple)
            {
                accessToken = this.GetTokenByType(eTokenType.eToken_Apple_Access);
            }
            return accessToken;

        }

        public long GetTokenExpireByType(int type)
        {
            for (int i = 0; i < this.token.Count; i++)
            {
                if (this.token[i].type == type)
                {
                    return this.token[i].expiration;
                }
            }
            return 0;
        }
        /// <summary>
        /// 解析json串，返回一个LoginRet.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="json">Json.</param>
        public static LoginRet ParseJson(string json)
        {
            try
            {
                LoginRet ret = JsonMapper.ToObject<LoginRet>(json);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.Log("errro:" + ex.Message + "\n" + ex.StackTrace);
            }
            return new LoginRet();
        }

        public LoginRet()
        {
        }
    }

    public class KVPair
    {
        public string key = "";
        public string value = "";

        public string _key
        {
            get { return key; }
            set { key = value; }
        }
        public string _value
        {
            get { return value; }
            set { this.value = value; }
        }

        public KVPair() { }

        public KVPair(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public override string ToString()
        {
            return key + ":" + value;
        }
    }

    public class CheckULRet : CallbackRet
    {
        public int step;          //检查步骤
        public bool success;  // 检测是否成功
        public string errorInfo; // 检测信息
        public string suggestion; //出错时的建议

        public int _step
        {
            get { return step; }
            set { step = value; }
        }
        public bool _success
        {
            get { return success; }
            set { success = value; }
        }
        public string _errorInfo
        {
            get { return errorInfo; }
            set { errorInfo = value; }
        }
        public string _suggestion
        {
            get { return suggestion; }
            set { suggestion = value; }
        }
        public override string ToString()
        {
            StringBuilder msg = new StringBuilder(base.ToString());
            msg.AppendLine("step:" + step);
            msg.AppendLine("success:" + success);
            msg.AppendLine("errorInfo:" + errorInfo);
            msg.AppendLine("suggestion:" + suggestion);
            return msg.ToString();
        }
        public static CheckULRet ParseJson(string json)
        {
            try
            {
                CheckULRet ret = JsonMapper.ToObject<CheckULRet>(json);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.Log("errro:" + ex.Message + "\n" + ex.StackTrace);
            }
            return new CheckULRet();
        }
    }


    public class WakeupRet : CallbackRet
    {
        /** 传递的openid */
        public string open_id = "";
        /** 对应微信消息中的mediaTagName */
        public string media_tag_name = "";
        /** 扩展消息,唤醒游戏时带给游戏 */
        public string messageExt = "";
        /** 语言     目前只有微信5.1以上用，手Q不用 */
        public string lang = "";
        /** 国家     目前只有微信5.1以上用，手Q不用 */
        public string country = "";
        public List<KVPair> extInfo = new List<KVPair>();

        public string _open_id
        {
            get { return open_id; }
            set { open_id = value; }
        }
        public string _media_tag_name
        {
            get { return media_tag_name; }
            set { media_tag_name = value; }
        }
        public string _messageExt
        {
            get { return messageExt; }
            set { messageExt = value; }
        }
        public string _lang
        {
            get { return lang; }
            set { lang = value; }
        }
        public string _country
        {
            get { return country; }
            set { country = value; }
        }
        public override string ToString()
        {
            StringBuilder msg = new StringBuilder(base.ToString());
            msg.AppendLine("openid:" + open_id);
            msg.AppendLine("mediaTagName:" + media_tag_name);
            msg.AppendLine("messageExt:" + messageExt);
            msg.AppendLine("lang:" + lang);
            msg.AppendLine("country:" + country);
            foreach (KVPair pair in extInfo)
            {
                msg.AppendLine(pair.ToString());
            }
            return msg.ToString();
        }

        public List<KVPair> _extInfo
        {
            get { return extInfo; }
            set { extInfo = value; }
        }

        public WakeupRet()
        {
        }

        /// <summary>
        /// 解析json串，返回一个 WakeupRet.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="json">Json.</param>
        public static WakeupRet ParseJson(string json)
        {
            try
            {
                WakeupRet ret = JsonMapper.ToObject<WakeupRet>(json);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.Log("errro:" + ex.Message + "\n" + ex.StackTrace);
            }
            return new WakeupRet();
        }
    }

    public class ShareRet : CallbackRet
    {
        public string extInfo = "";

        public string _extInfo
        {
            get { return extInfo; }
            set { extInfo = value; }
        }
        public ShareRet()
        {
        }

        public static ShareRet ParseJson(string json)
        {
            try
            {
                ShareRet ret = JsonMapper.ToObject<ShareRet>(json);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.Log("errro:" + ex.Message + "\n" + ex.StackTrace);
            }
            return new ShareRet();
        }

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder(base.ToString());
            msg.AppendLine("extInfo:" + extInfo);
            return msg.ToString();
        }
    }

    public class CardRet : CallbackRet
    {
        public String open_id = ""; /* 传递的openid */
        public String wx_card_list = ""; /* 对应微信消息中的mediaTagName */
        public List<KVPair> extInfo = new List<KVPair>(); /* 存放所有平台过来信息。 */

        public string _open_id
        {
            get { return open_id; }
            set { open_id = value; }
        }
        public string _wx_card_list
        {
            get { return wx_card_list; }
            set { wx_card_list = value; }
        }
        public List<KVPair> _extInfo
        {
            get { return extInfo; }
            set { extInfo = value; }
        }

        public static CardRet ParseJson(string json)
        {
            try
            {
                CardRet ret = JsonMapper.ToObject<CardRet>(json);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.Log("errro:" + ex.Message + "\n" + ex.StackTrace);
            }
            return new CardRet();
        }

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder(base.ToString());
            msg.AppendLine("openid:" + open_id);
            msg.AppendLine("wxCardList:" + wx_card_list);
            foreach (KVPair pair in extInfo)
            {
                msg.AppendLine(pair.ToString());
            }
            return msg.ToString();
        }
    }

    public class ADRet
    {
        public string viewTag = "";
        public int scene;

        public ADRet()
        {
        }

        public string _viewTag
        {
            get { return viewTag; }
            set { viewTag = value; }
        }

        public int _scene
        {
            get { return scene; }
            set { scene = value; }
        }

        public override System.String ToString()
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine("viewTag:" + viewTag);
            msg.AppendLine("scene:" + scene);
            return msg.ToString();
        }

        /// <summary>
        /// 解析json串，返回一个ADRet.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="json">Json.</param>
        public static ADRet ParseJson(string json)
        {
            try
            {
                ADRet ret = JsonMapper.ToObject<ADRet>(json);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.Log("errro:" + ex.Message + "\n" + ex.StackTrace);
            }
            return new ADRet();
        }

    }

    public class PersonInfo
    {
        public string nickName = "";
        public string openId = "";
        public string gender = "";
        public string pictureSmall = "";
        public string pictureMiddle = "";
        public string pictureLarge = "";
        public string province = "";
        public string city = "";
        public string unionID = "";        //用户unionID
        public string gpsCity = "";

        public int distance = 0;
        public bool isFriend = false;
        public long timestamp = 0;


        public string lang = "";
        public string country = "";

        // apple 登录新增
        public string apple_email; // apple 登录邮箱
        public string apple_namePrefix; // apple 返回的 fullname 的一部分
        public string apple_nameSuffix; // apple 返回的 fullname 的一部分
        public string apple_familyName; // apple 返回的 fullname 的一部分
        public string apple_middleName; // apple 返回的 fullname 的一部分
        public string apple_givenName; // apple 返回的 fullname 的一部分
        public string apple_nickname; // apple 返回的 fullname 的一部分

        //wx关系链
        public string friendRemark = ""; //好友备注

        public string _nickName
        {
            get { return nickName; }
            set { nickName = value; }
        }
        public string _openId
        {
            get { return openId; }
            set { openId = value; }
        }
        public string _gender
        {
            get { return gender; }
            set { gender = value; }
        }
        public string _pictureSmall
        {
            get { return pictureSmall; }
            set { pictureSmall = value; }
        }
        public string _pictureMiddle
        {
            get { return pictureMiddle; }
            set { pictureMiddle = value; }
        }
        public string _pictureLarge
        {
            get { return pictureLarge; }
            set { pictureLarge = value; }
        }
        public string _provice
        {
            get { return province; }
            set { province = value; }
        }
        public string _city
        {
            get { return city; }
            set { city = value; }
        }
        public string _unionID
        {
            get { return unionID; }
            set { unionID = value; }
        }
        public string _gpsCity
        {
            get { return gpsCity; }
            set { gpsCity = value; }
        }
        public int _distance
        {
            get { return distance; }
            set { distance = value; }
        }
        public bool _isFriend
        {
            get { return isFriend; }
            set { isFriend = value; }
        }
        public long _timestamp
        {
            get { return timestamp; }
            set { timestamp = (long)value; }
        }
        public string _lang
        {
            get { return lang; }
            set { lang = value; }
        }
        public string _country
        {
            get { return country; }
            set { country = value; }
        }

        public string _apple_email
        {
            get { return apple_email; }
            set { apple_email = value; }
        }

        public string _apple_namePrefix
        {
            get { return apple_namePrefix; }
            set { apple_namePrefix = value; }
        }

        public string _apple_nameSuffix
        {
            get { return apple_nameSuffix; }
            set { apple_nameSuffix = value; }
        }

        public string _apple_familyName
        {
            get { return apple_familyName; }
            set { apple_familyName = value; }
        }

        public string _apple_middleName
        {
            get { return apple_middleName; }
            set { apple_middleName = value; }
        }

        public string _apple_givenName
        {
            get { return apple_givenName; }
            set { apple_givenName = value; }
        }

        public string _apple_nickname
        {
            get { return apple_nickname; }
            set { apple_nickname = value; }
        }

        public string _friendRemark
        {
            get { return friendRemark; }
            set { friendRemark = value;}
        }

        public PersonInfo()
        {

        }

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine("nickName:" + nickName);
            msg.AppendLine("openId:" + openId);
            msg.AppendLine("gender:" + gender);
            msg.AppendLine("pictureSmall:" + pictureSmall);
            msg.AppendLine("pictureMiddle:" + pictureMiddle);
            msg.AppendLine("pictureLarge:" + pictureLarge);
            msg.AppendLine("province:" + province);
            msg.AppendLine("city:" + city);
            msg.AppendLine("unionID:" + unionID);
            msg.AppendLine("gpsCity:" + gpsCity);
            msg.AppendLine("distance:" + distance);
            msg.AppendLine("isFriend:" + isFriend);
            msg.AppendLine("timestamp:" + timestamp);
            msg.AppendLine("lang:" + lang);
            msg.AppendLine("country:" + country);
            msg.AppendLine("apple_email:" + apple_email);
            msg.AppendLine("apple_namePrefix:" + apple_namePrefix);
            msg.AppendLine("apple_nameSuffix:" + apple_nameSuffix);
            msg.AppendLine("apple_familyName:" + apple_familyName);
            msg.AppendLine("apple_middleName:" + apple_middleName);
            msg.AppendLine("apple_givenName:" + apple_givenName);
            msg.AppendLine("apple_nickname:" + apple_nickname);
            msg.AppendLine("friendRemark:" + friendRemark);
            return msg.ToString();
        }
    }

    public class RelationRet : CallbackRet
    {
        public List<PersonInfo> persons = new List<PersonInfo>();
        public int type;
        public int _type
        {
            get { return type; }
            set { type = value; }
        }

        public List<PersonInfo> _persons
        {
            get { return persons; }
            set { persons = value; }
        }
        /// <summary>
        /// 解析json串，返回一个RelationRet.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="json">Json.</param>
        public static RelationRet ParseJson(string json)
        {
            try
            {
                RelationRet ret = JsonMapper.ToObject<RelationRet>(json);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.Log("errro:" + ex.Message + "\n" + ex.StackTrace);
            }
            return new RelationRet();
        }
        public override string ToString()
        {
            StringBuilder msg = new StringBuilder(base.ToString());
            foreach (PersonInfo person in persons)
            {
                msg.AppendLine(person.ToString());
            }
            msg.AppendLine("type:" + this.type);
            return msg.ToString();
        }

        public RelationRet()
        {
        }
    }

    public class LocationRet : CallbackRet
    {
        public double longitude;    // 经度
        public double latitude;     // 纬度

        public double _longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }
        public double _latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }
        public LocationRet()
        {
        }

        public static LocationRet ParseJson(string json)
        {
            try
            {
                LocationRet ret = JsonMapper.ToObject<LocationRet>(json);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.Log("errro:" + ex.Message + "\n" + ex.StackTrace);
            }
            return new LocationRet();
        }

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder(base.ToString());
            msg.AppendLine("longitude:" + longitude);
            msg.AppendLine("latitude:" + latitude);
            return msg.ToString();
        }
    }
public class CountryFromIPRet: CallbackRet
    {
        public string country;  // 国家
        public bool isQueryByRequestHeader; // 标志是否请求头查询

        public string _country
        {
            get { return country; }
            set { country = value; }
        }

        public bool _isQueryByRequestHeader
        {
            get { return isQueryByRequestHeader; }
            set { isQueryByRequestHeader = value; }
        }

        public CountryFromIPRet()
        {
        }

        public static CountryFromIPRet ParseJson(string json)
        {
            try
            {
                CountryFromIPRet ret = JsonMapper.ToObject<CountryFromIPRet>(json);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.Log("errro:" + ex.Message + "\n" + ex.StackTrace);
            }
            return new CountryFromIPRet();
        }

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder(base.ToString());
            msg.AppendLine("country:" + country);
            msg.AppendLine("isQueryByRequestHeader:" + isQueryByRequestHeader);
            return msg.ToString();
        }
        
    }

 public class GetIPInfoRet: CallbackRet
 {
    public int country_no;  //国家编号
    public int province_no; //省份编号
    public int city_no;     //城市编号
    public string country;  //国家
    public string province; //省份
    public string city;     //城市
    public bool isQueryByRequestHeader; //标志是否请求头查询

    public int _country_no
    {
        get { return country_no; }
        set { country_no = value; }
    }

    public int _province_no
    {
        get { return province_no; }
        set { province_no = value; }
    }

    public int _city_no
    {
        get { return city_no; }
        set { city_no = value; }
    }
    
    public string _country
    {
        get { return country; }
        set { country = value; }
    }

    public string _province
    {
        get { return province; }
        set { province = value; }
    }

    public string _city
    {
        get { return city; }
        set { city = value; }
    }

    public bool _isQueryByRequestHeader
    {
        get { return isQueryByRequestHeader; }
        set { isQueryByRequestHeader = value; }
    }

    public GetIPInfoRet() { }

    public static GetIPInfoRet ParseJson(string jsonRet)
    {
        try
            {
                GetIPInfoRet ret = JsonMapper.ToObject<GetIPInfoRet>(jsonRet);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.Log("error:" + ex.Message + "\n" + ex.StackTrace);
            }
            return new GetIPInfoRet();
    }

    public override string ToString()
        {
            StringBuilder msg = new StringBuilder(base.ToString());
            msg.AppendLine("country_no:" + country_no);
            msg.AppendLine("province_no:" + province_no);
            msg.AppendLine("city_no:" + city_no);
            msg.AppendLine("country:" + country);
            msg.AppendLine("province:" + province);
            msg.AppendLine("city:" + city);
            msg.AppendLine("isQueryByRequestHeader:" + isQueryByRequestHeader);
            return msg.ToString();
        }
 }   

    public class PicInfo
    {
        public string picPath;
        public eMSDK_SCREENDIR screenDir;
        public string hashValue;

        public string _picPath
        {
            get { return picPath; }
            set { picPath = value; }
        }

        public string _hashValue
        {
            get { return hashValue; }
            set { hashValue = value; }
        }

        public eMSDK_SCREENDIR _screenDir
        {
            get { return screenDir; }
            set { screenDir = value; }
        }

        public PicInfo()
        {
            screenDir = eMSDK_SCREENDIR.eMSDK_SCREENDIR_SENSOR;
        }

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine("picPath:" + picPath);
            msg.AppendLine("hashValue:" + hashValue);
            string screen = "";
            if (screenDir == eMSDK_SCREENDIR.eMSDK_SCREENDIR_LANDSCAPE)
            {
                screen = "LANDSCAPE";
            }
            else if (screenDir == eMSDK_SCREENDIR.eMSDK_SCREENDIR_PORTRAIT)
            {
                screen = "PORTRAIT";
            }
            else
            {
                screen = "SENSOR";
            }
            msg.AppendLine("screenDir:" + screen);
            return msg.ToString();
        }
    }

    public class NoticeInfo
    {
        public string msg_id; /* 公告id */
        //public string mAppId ; /* appid */
        public string open_id; /* 用户open_id */
        public string msg_url; /* 公告跳转链接 */
        public int msg_type; /* 公告展示类型，滚动弹出 */
        public string msg_scene; /* 公告展示的场景，管理端后台配置 */
        public string start_time; /* 公告有效期开始时间 */
        public string end_time; /* 公告有效期结束时间 */
        public string update_time; /* 公告更新时间 */
        public int content_type;/* 公告内容类型，eMSG_NOTICETYPE */
        #if UNITY_ANDROID
        public string msg_order;/*公告优先级 */
        #else
        public int msg_order;/*公告优先级 */
        #endif

        /*文本公告特殊字段 */
        public string msg_title; /* 公告标题 */
        public string msg_content;/* 公告内容 */

        /*网页公告特殊字段 */
        public string content_url;

        public string msg_custom;/*自定义参数*/
        public List<PicInfo> picArray = new List<PicInfo>();

        /* 以下方法是为了能使用LitJson的自动将Json映射为Object方法 */
        public string _update_time
        {
            get { return update_time; }
            set { update_time = value; }
        }
        public string _msg_title
        {
            get { return msg_title; }
            set { msg_title = value; }
        }
        public int _content_type
        {
            get { return content_type; }
            set { content_type = value; }
        }
        #if UNITY_ANDROID
        public string _msg_order
        {
            get { return msg_order; }
            set { msg_order = value; }
        }
        #else
        public int _msg_order
        {
            get { return msg_order; }
            set { msg_order = value; }
        }
        #endif
        public string _end_time
        {
            get { return end_time; }
            set { end_time = value; }
        }
        public string _start_time
        {
            get { return start_time; }
            set { start_time = value; }
        }
        public string _msg_scene
        {
            get { return msg_scene; }
            set { msg_scene = value; }
        }
        public int _msg_type
        {
            get { return msg_type; }
            set { msg_type = value; }
        }
        public string _msg_url
        {
            get { return msg_url; }
            set { msg_url = value; }
        }
        public string _content_url
        {
            get { return content_url; }
            set { content_url = value; }
        }
        public string _msg_id
        {
            get { return msg_id; }
            set { msg_id = value; }
        }
        public string _open_id
        {
            get { return open_id; }
            set { open_id = value; }
        }
        public string _msg_content
        {
            get { return msg_content; }
            set { msg_content = value; }
        }

        public string _msg_custom
        {
            get { return msg_custom; }
            set { msg_custom = value; }
        }

        public List<PicInfo> _picArray
        {
            get { return picArray; }
            set { picArray = value; }
        }

        public NoticeInfo()
        {
        }

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine("msg_id:" + msg_id);
            msg.AppendLine("open_id:" + open_id);
            msg.AppendLine("msg_url:" + msg_url);
            msg.AppendLine("msg_type:" + msg_type);
            msg.AppendLine("msg_scene:" + msg_scene);
            msg.AppendLine("start_time:" + start_time);
            msg.AppendLine("end_time:" + end_time);
            msg.AppendLine("update_time:" + update_time);
            msg.AppendLine("content_type:" + content_type);
            msg.AppendLine("msg_order:" + msg_order);
            msg.AppendLine("msg_title:" + msg_title);
            msg.AppendLine("msg_content:" + msg_content);
            msg.AppendLine("content_url:" + content_url);
            msg.AppendLine("msg_custom:" + msg_custom);
            foreach (PicInfo info in picArray)
            {
                msg.AppendLine(info.ToString());
            }
            return msg.ToString();
        }
    }

    /// <summary>
    /// 方便解析json的类
    /// </summary>
    public class NoticeInfoList
    {
        public List<NoticeInfo> list = new List<NoticeInfo>();

        public List<NoticeInfo> _notice_list
        {
            get { return list; }
            set { list = value; }
        }

        public NoticeInfoList()
        {
        }

        public static List<NoticeInfo> ParseJson(string json)
        {
            try
            {
                JsonData datas = JsonMapper.ToObject(json);
                if (datas["_notice_list"].IsArray)
                {
                    NoticeInfoList noticeList = JsonMapper.ToObject<NoticeInfoList>(json);
                    return noticeList.list;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("errro:" + ex.Message + "\n" + ex.StackTrace);
            }
            return new List<NoticeInfo>();
        }
    }

    public class QQGroupInfo
    {
        public string groupName = ""; /*群名称 */
        public string fingerMemo = "";/*群的相关简介 */
        public string memberNum = ""; /*群成员数 */
        public string maxNum = ""; /*该群可容纳的最多成员数 */
        public string ownerOpenid = ""; /*群主openid */
        public string unionid = ""; /*与该QQ群绑定的公会ID */
        public string zoneid = ""; /*大区ID */
        public string adminOpenids = ""; /*管理员openid。如果管理员有多个的话，用“,”隔开，例如0000000000000000000000002329FBEF,0000000000000000000000002329FAFF */
        public string groupOpenid = "";  /*和游戏公会ID绑定的QQ群的groupOpenid */
        public string groupKey = "";   /*需要添加的QQ群对应的key */
        public string relation = "";/*显示成员和群的关系1:群主，2:管理员，3:普通成员，4:非成员 */
        public string _groupName
        {
            get { return groupName; }
            set { groupName = value; }
        }
        public string _fingerMemo
        {
            get { return fingerMemo; }
            set { fingerMemo = value; }
        }
        public string _memberNum
        {
            get { return memberNum; }
            set { memberNum = value; }
        }
        public string _maxNum
        {
            get { return maxNum; }
            set { maxNum = value; }
        }
        public string _ownerOpenid
        {
            get { return ownerOpenid; }
            set { ownerOpenid = value; }
        }
        public string _unionid
        {
            get { return unionid; }
            set { unionid = value; }
        }
        public string _zoneid
        {
            get { return zoneid; }
            set { zoneid = value; }
        }
        public string _adminOpenids
        {
            get { return adminOpenids; }
            set { adminOpenids = value; }
        }
        public string _groupOpenid
        {
            get { return groupOpenid; }
            set { groupOpenid = value; }
        }
        public string _groupKey
        {
            get { return groupKey; }
            set { groupKey = value; }
        }
        public string _relation
        {
            get { return relation; }
            set { relation = value; }
        }
        public QQGroupInfo()
        {

        }

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine("groupName:" + groupName);
            msg.AppendLine("fingerMemo:" + fingerMemo);
            msg.AppendLine("memberNum:" + memberNum);
            msg.AppendLine("maxNum:" + maxNum);
            msg.AppendLine("ownerOpenid:" + ownerOpenid);
            msg.AppendLine("unionid:" + unionid);
            msg.AppendLine("zoneid:" + zoneid);
            msg.AppendLine("adminOpenids:" + adminOpenids);
            msg.AppendLine("groupOpenid:" + groupOpenid);
            msg.AppendLine("groupKey:" + groupKey);
            msg.AppendLine("relation:" + relation);
            return msg.ToString();
        }
    }

    public class WXGroupInfo
    {
        public string openIdList = "";		/* 群成员openID */
        public string memberNum = "";		/* 群成员数 */
        public string chatRoomURL = "";		/*假如群聊URL */
        public int status;
        public string _openIdList
        {
            get { return openIdList; }
            set { openIdList = value; }
        }

        public string _memberNum
        {
            get { return memberNum; }
            set { memberNum = value; }
        }

        public string _chatRoomURL
        {
            get { return chatRoomURL; }
            set { chatRoomURL = value; }
        }
        public int _status
        {
            get { return status; }
            set { status = value; }
        }
        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine("openIdList:" + openIdList);
            msg.AppendLine("memberNum:" + memberNum);
            msg.AppendLine("chatRoomURL:" + chatRoomURL);
            msg.AppendLine("status:" + status);
            return msg.ToString();
        }
    }
    public class QQGroup
    {
        string groupId;  //群号
        string groupName; // 群名称
        public string _groupId{
            get { return groupId;}
            set { groupId = value;}
        }
        public string _groupName{
            get { return groupName;}
            set { groupName = value;}
        }
        public override string ToString()
        {
 	        StringBuilder msg = new StringBuilder();
            msg.AppendLine("groupId:" + groupId);
            msg.AppendLine("groupName:" + groupName);
            return msg.ToString();
        }
    }
    public class QQGroupInfoV2
    {
        int relation;
        string guildId;
        string guildName;
        List<QQGroup> qqGroups = new List<QQGroup>();
        public int _relation{
            get { return relation;}
            set { relation = value;}
        }
        public string _guildId{
            get { return guildId;}
            set { guildId = value;}
        }
        public string _guildName{
            get { return guildName;}
            set { guildName = value;}
        }
        public List<QQGroup> _qqGroups{
            get { return qqGroups; }
            set { qqGroups = value; }
        }
        
        public QQGroupInfoV2(){
            relation = -1;
            guildId = "";
            guildName = "";
            qqGroups = new List<QQGroup>();
        }
        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine("relation:" + relation);
            msg.AppendLine("guildId:" + guildId);
            msg.AppendLine("guildName:" + guildName);
            foreach(QQGroup qqGroup in qqGroups){
                 msg.AppendLine( qqGroup.ToString());
            }
            return msg.ToString();
        }
    }
    public class GroupRet : CallbackRet
    {
        public int errorCode = 0;
        public QQGroupInfo mQQGroupInfo = new QQGroupInfo();
        public WXGroupInfo mWXGroupInfo = new WXGroupInfo();
        public QQGroupInfoV2 mQQGroupInfoV2 = new QQGroupInfoV2();
        public int _errorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }

        public QQGroupInfoV2 _mQQGroupInfoV2
        {
            get { return mQQGroupInfoV2; }
            set { mQQGroupInfoV2 = value; }
        }

        public QQGroupInfo _mQQGroupInfo
        {
            get { return mQQGroupInfo; }
            set { mQQGroupInfo = value; }
        }
        public WXGroupInfo _mWXGroupInfo
        {
            get { return mWXGroupInfo; }
            set { mWXGroupInfo = value; }
        }

        public static GroupRet ParseJson(string json)
        {
            try
            {
                GroupRet ret = JsonMapper.ToObject<GroupRet>(json);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.Log("errro:" + ex.Message + "\n" + ex.StackTrace);
            }
            return new GroupRet();
        }

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder(base.ToString());
            msg.AppendLine("errorCode:" + errorCode);

            msg.AppendLine("mQQGroupInfoV2:\n"+mQQGroupInfoV2.ToString());
            msg.AppendLine("mQQGroupInfo:\n" + mQQGroupInfo.ToString());
            msg.AppendLine("mWXGroupInfo:\n" + mWXGroupInfo.ToString());
            return msg.ToString();
        }
    }

    public class WebviewRet : CallbackRet
    {
        public string msgData;
        public string _msgData
        {
            get { return msgData; }
            set { msgData = value; }
        }
        public static WebviewRet ParseJson(string json)
        {
            try
            {
                WebviewRet ret = JsonMapper.ToObject<WebviewRet>(json);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.Log("errro:" + ex.Message + "\n" + ex.StackTrace);
            }
            return new WebviewRet();
        }

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder(base.ToString());
            msg.AppendLine("msgData:" + msgData);
            return msg.ToString();
        }
    }



    public class LocalMessageAndroid
    {
        public int type { get; set; }					/* 消息类型type： 1:通知，2:消息 */
        public int action_type { get; set; }			/* 动作类型，1打开activity或app本身，2打开浏览器，3打开Intent ，4通过包名打开应用。默认为1 */
        public String title { get; set; }				/* 消息标题title： 消息标题 */
        public String content { get; set; }				/* 消息内容content 消息內容 */
        public String custom_content { get; set; }		/* 消息自定义内容 */
        public int icon_type { get; set; }				/* 通知栏图标是应用内图标还是上传图标(0是应用内图标，1是上传图标,默认0) */
        public int lights { get; set; }					/* 是否呼吸灯(0否，1是,默认1) */
        public int ring { get; set; }					/* 是否响铃(0否，1是,默认1) */
        public int vibrate { get; set; }				/* 是否振动(0否，1是,默认1) */
        public int style_id { get; set; }				/* Web端设置是否覆盖编号build_id的通知样式，默认1，0否，1是 */
        public long builderId { get; set; }				/* 消息样式，默认为0或不设置 */
        public String activity { get; set; }			/* 打开指定Activity,例如：com.qq.xgdemo.AboutActivity。当动作类型为1或4时有效 */
        public String packageDownloadUrl { get; set; }
        public String packageName { get; set; }			/* 拉起其他应用的包名，当动作类型为4时有效 */
        public String icon_res { get; set; }			/* 应用内图标文件名（xg.png）或者下载图标的url地址,例如:xg或者图片url */
        public String date { get; set; }				/* 弹出通知的日期，格式为”yyyyMMdd” */
        public String hour { get; set; }				/* 弹出通知的日期的小时数 */
        public String min { get; set; }					/* 弹出通知的日期的分钟数 */
        public String intent { get; set; }				/* 设置打开intent,例如(10086拨号界面)intent:10086#Intent;scheme=tel;action=android.intent.action.DIAL;S.key=value;end当动作类型为3时有效 */
        public String url { get; set; }					/* 打开url,例如：http://www.qq.com。当动作类型为2时有效 */
        public String ring_raw { get; set; }			/* 指定应用内的声音（ring.mp3）,例如:ring */
        public String small_icon { get; set; }			/* 指定状态栏的小图片(xg.png),例如：xg */

        public LocalMessageAndroid()
        {
            type = 1;
            action_type = -1;
            icon_type = -1;
            lights = -1;
            ring = -1;
            vibrate = -1;
            style_id = -1;
            builderId = -1;
            content = "";
            custom_content = "";
            activity = "";
            packageDownloadUrl = "";
            packageName = "";
            icon_res = "";
            date = "";
            hour = "";
            intent = "";
            min = "";
            title = "";
            url = "";
            ring_raw = "";
            small_icon = "";
        }
    }

    public class LocalMessageIOS
    {
        public string fireDate { get; set; }			/*本地推送触发的时间,格式yyyy-MM-dd HH:mm:ss */
        public string alertBody { get; set; }			/*推送的内容 */
        public int badge { get; set; }                  /*角标的数字 */
        public string alertAction { get; set; }			/*替换弹框的按钮文字内容（默认为"启动"） */
        public List<KVPair> userInfo { get; set; }		/*自定义参数，可以用来标识推送和增加附加信息 */
        public string userInfoKey { get; set; }			/*本地推送在前台推送的标识Key */
        public string userInfoValue { get; set; } 		/*本地推送在前台推送的标识Key对应的值 */

        public LocalMessageIOS()
        {
            fireDate = "";
            alertBody = "";
            alertAction = "";
            userInfoKey = "";
            userInfoValue = "";
        }
    }

    public class GameStatus
    {
        public const int GAME_STATUS_TYPE_APP = 1;
        public const int GAME_STATUS_TYPE_MSDK = 2;
        public const int GAME_STATUS_TYPE_GAME = 3;
        public const int GAME_STATUS_TYPE_GAME_EXT = 4;


        /************* 应用状态 *************/
        //
        public const string MSDK_FRONT = "MSDKFront";
        //后台运行中
        public const string MSDK_BACK = "MSDKBack";
        //切换到后台中
        public const string MSDK_GOBACK = "MSDKGoBack";
        //切换回前台中
        public const string MSDK_GOFRONT = "MSDKGoFront";
        //退出游戏中
        public const string MSDK_EXIT = "MSDKExit";

        /************* MSDK相关场景 *************/
        //默认状态
        public const string MSDK_DEFAULT = "MSDKDefault";
        //初始化MSDK中
        public const string MSDK_INIT = "MSDKInit";
        //回调中
        public const string MSDK_OBSERVER = "MSDKObserver";
        //登陆中
        public const string MSDK_LOGIN = "MSDKLogin";
        //登出中
        public const string MSDK_LOGOUT = "MSDKLogout";
        //更新中
        public const string MSDK_UPDATE = "MSDKUpdate";
        //分享中
        public const string MSDK_SHARE = "MSDKShare";

        /************* 支付相关场景 *************/
        //拉起支付
        public const string PAY_LAUNCHER = "MSDKPayLauncher";
        //付款
        public const string PAY_ING = "MSDKPayIng";
        //支付结束，回调游戏
        public const string PAY_NOTIFY = "MSDKPayNotify";

        /************* 登陆相关场景 *************/
        public const string GAME_LOGIN = "MSDKGameLogin";

        /************* 游戏游戏对局场景场景 *************/
        //单局游戏开始前准备
        public const string GAME_PRE = "MSDKGamePre";
        //单局游戏游戏进行中
        public const string GAME_ING = "MSDKGameIng";
        //单局游戏游戏暂停中
        public const string GAME_PAUSE = "MSDKGamePause";
        //单局游戏结束结算中
        public const string GAME_CALCULATE = "MSDKGameCalculate";

        /************* 游戏购买道具场景 *************/
        //购买道具
        public const string ITEM_PURCHASE = "MSDKItemPurchase";

        /************* 游戏联网对战场景 *************/
        //联网对战准备
        public const string PV_PRE = "MSDKPvPre";
        //联网对战进行中
        public const string PV_ING = "MSDKPvIng";
        //联网对战进行中
        public const string PV_PAUSE = "MSDKPvPause";
        //联网对战结束结算中
        public const string PV_CALCULATE = "MSDKPvCalculate";
    }
    public class ImageParams
    {
#if UNITY_IPHONE
        public byte[] ios_imageData { get; set; }
        public int ios_imageDataLen { get; set; }
#endif
#if UNITY_ANDROID
        public string android_imagePath { get; set; }
#endif
    }

    public class VideoParams
    {
#if UNITY_IPHONE
        public byte[] ios_videoData { get; set; }
        public int ios_videoDataLen { get; set; }
        public string ios_videoLocalIdentifier { get; set; }
#endif
#if UNITY_ANDROID
        public string android_videoPath { get; set; }
#endif
    }

        public class GameGuild
    {
        // 工会id
        public string  guildId{ get; set; }
        // 工会名称
        public string guildName { get; set; }
        //公会会长的openId
        public string leaderOpenId { get; set; }
        //公会会长的roleid
        public string leaderRoleId { get; set; }
        //会长区服信息,会长可能转让给非本区服的人，与公会区服一样时可不填
        public string leaderZoneId { get; set; }
        // 区id
        public string zoneId { get; set; }
        //（小区）区服id，可以不填写，暂时无用
        public string  partition{ get; set; }
        // 角色id
        public string roleId { get; set; }
        public string roleName { get; set; }
        //用户区服ID，王者的会长可能转让给非本区服的人，所以公会区服不一定是用户区服。与公会区服一样时可不填
        public string userZoneId { get; set; }
        //新增修改群名片功能，全不填为不修改群名片；任一有内容为需要修改；样式规则：【YYYY】ZZZZ,ZZZZ指用户的游戏内昵称
        public string  userLabel{ get; set; }
        //用户昵称
        public string  nickName{ get; set; }
        //0公会(或不填),1队伍，2赛事
        public string  type{ get; set; }
        //测试环境使用：游戏大区ID，理论上只有1:QQ,2:微信，但是测试环境有很多虚拟的
        public string  areaId{ get; set; }
        public GameGuild(){
            guildId = "";
            guildName = "";
            leaderOpenId = "";
            leaderRoleId = "";
            leaderZoneId = "";
            zoneId = "";
            partition = "";
            roleId = "";
            roleName = "";
            userZoneId = "";
            userLabel = "";
            nickName = "";
            type = "0";
            areaId = "1";
        }
           
    } 
}