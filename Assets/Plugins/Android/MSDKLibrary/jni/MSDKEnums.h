//
//  MSDKEnums.h
//  MSDKFoundation
//
//  Created by Jason on 14/11/6.
//  Copyright (c) 2014年 Tencent. All rights reserved.
//

#ifndef MSDKFoundation_MSDKEnums_h
#define MSDKFoundation_MSDKEnums_h

#define DEFAUTL_LOGIN_OVERTIME 20
#define MSDK_TEST "MSDK_TEST"
/***
 * ePlatform为登录平台，由于历史原因，后续新增的登录平台枚举值不再使用3和4。
 */
typedef enum _ePlatform {
    ePlatform_None,
    ePlatform_Weixin,
    ePlatform_QQ
#ifdef ANDROID
//    ,
//    ePlatform_WTLogin = 3,
//    ePlatform_QQHall = 4
#endif
    ,
    ePlatform_Guest = 5,
    ePlatform_Auto = 6,  //自动登录，使用上一次的平台
    ePlatform_Apple = 7,  //Apple 登录
    ePlatform_WeixinVideoLive = 8 //微信视频号授权
} ePlatform;

typedef enum _eULCheckStep {
    eULCheckStepParams,
    eULCheckStepSystemVersion,
    eULCheckStepWechatVersion,
    eULCheckStepSDKInnerOperation,
    eULCheckStepLaunchWechat,
    eULCheckStepBackToCurrentApp,
    eULCheckStepFinal,
} eULCheckStep;
/***
 * eWakeupPlatform为拉起游戏平台，指唤醒游戏的平台，由于历史原因，其中的微信，手Q枚举值需要与
 * ePlatform中的枚举值保持一致，后续新增的拉起平台枚举值不再使用0，3，5，6.
 */
typedef enum _eWakeupPlatform {
	eWakeupPlatform_Weixin = 1,
	eWakeupPlatform_QQ = 2,
	eWakeupPlatform_QQHall = 4,
	eWakeupPlatform_TencentMsdk = 7 //msdk scheme 拉起
}eWakeupPlatform;

typedef enum _eFlag {
    eFlag_Succ = 0,
    eFlag_Cloud_NoMatch = 1,              // 云控不匹配
    eFlag_QQ_NoAcessToken = 1000,         // QQ&QZone login fail and can't get accesstoken
    eFlag_QQ_UserCancel = 1001,           // QQ&QZone user has cancelled login process (tencentDidNotLogin)
    eFlag_QQ_LoginFail = 1002,            // QQ&QZone login fail (tencentDidNotLogin)
    eFlag_Login_NetworkErr = 1003,        // QQ&QZone&wx login networkErr
    eFlag_QQ_NetworkErr = 1003,           // 兼容Android老版本
    eFlag_QQ_NotInstall = 1004,           // QQ is not install
    eFlag_QQ_NotSupportApi = 1005,        // QQ don't support open api
    eFlag_QQ_AccessTokenExpired = 1006,   // QQ Actoken失效, 需要重新登陆
    eFlag_QQ_PayTokenExpired = 1007,      // QQ Pay token过期
    eFlag_QQ_UnRegistered = 1008,         // 没有在qq注册
    eFlag_QQ_MessageTypeErr = 1009,       // QQ消息类型错误
    eFlag_QQ_MessageContentEmpty = 1010,  // QQ消息为空
    eFlag_QQ_MessageContentErr = 1011,    // QQ消息不可用（超长或其他）

    eFlag_WX_NotInstall = 2000,               // Weixin is not installed
    eFlag_WX_NotSupportApi = 2001,            // Weixin don't support api
    eFlag_WX_UserCancel = 2002,               // Weixin user has cancelled
    eFlag_WX_UserDeny = 2003,                 // Weixin User has deny
    eFlag_WX_LoginFail = 2004,                // Weixin login fail
    eFlag_WX_RefreshTokenSucc = 2005,         // Weixin 刷新票据成功
    eFlag_WX_RefreshTokenFail = 2006,         // Weixin 刷新票据失败
    eFlag_WX_AccessTokenExpired = 2007,       // Weixin AccessToken失效, 此时可以尝试用refreshToken去换票据
    eFlag_WX_RefreshTokenExpired = 2008,      // Weixin refresh token 过期, 需要重新授权
    eFlag_WX_Group_HasNoAuthority = 2009,     //游戏没有建群权限
    eFlag_WX_Group_ParameterError = 2010,     //参数检查错误
    eFlag_WX_Group_HadExist = 2011,           //微信群已存在
    eFlag_WX_Group_AmountBeyond = 2012,       //建群数量超过上限
    eFlag_WX_Group_IDNotExist = 2013,         //群ID不存在
    eFlag_WX_Group_IDHadCreatedToday = 2014,  //群ID今天已经建过群，每天每个ID只能创建一次群聊
    eFlag_WX_Group_JoinAmountBeyond = 2015,   //加群数量超限，每天每个ID最多可加2个群
    eFlag_WX_Group_NotInGroup = 2016,         //不在群中，发送群消息时若不在群中(被群主踢出群等)
    eFlag_Error = -1,
    eFlag_Local_Invalid = -2,               // 本地票据无效, 要游戏现实登陆界面重新授权
    eFlag_NotInWhiteList = -3,              // 不在白名单(for Android)
    eFlag_LbsNeedOpenLocationService = -4,  // 需要引导用户开启定位服务
    eFlag_LbsLocateFail = -5,               // 定位失败，原因待确认
	eFlag_LbsNeedOpenLocationSwitch = -6,// 需要引导用户开启定位开关
    eFlag_UrlTooLong = -6,                  // for WGOpenUrl
	eFlag_InvalidOnGuest = -7,              // 该功能在Guest模式下不可使用(for ios)
    eFlag_NetworkError = -8,                // 网络请求出错(发送失败，超时，无回包等)
    eFlag_UnPermission = -9,                // 无操作权限
    eFlag_IllegalParams = -10,              //非法参数
	eFlag_LbsLocateTimeout = -11,			//定位失败（超时）

    eFlag_NeedLogin = 3001,           //需要进入登陆页
    eFlag_UrlLogin = 3002,            //使用URL登陆成功
    eFlag_NeedSelectAccount = 3003,   //需要弹出异帐号提示
    eFlag_AccountRefresh = 3004,      //通过URL将票据刷新
    eFlag_NeedRealNameAuth = 3005,    //需要实名认证
    eFlag_Need_Realname_Auth = 3005,  //需要实名认证 ，兼容之前的版本的错误码(for Android)
	eFlag_Need_MSDK_Realname_Auth = 3006,  // 正在进行MSDK实名认证，请忽调用游戏自定义的实名认证界面
    eFlag_WebviewClosed = 6001,       //内置浏览器关闭
    eFlag_Invalid_Delete_Account_URL = 6002, //打开注销账号的url无效
    eFlag_Invalid_Login_Channel = 6003, //注销账号操作时未登录或渠道不支持
    eFlag_Webview_page_event = 7000,  //webview js给native传递参数的事件
    eFlag_Checking_Token = 5001,      //正在检查票据
    eFlag_Logining = 5002,            //正在登录中，请等待登录回调后再操作
    eFlag_Login_Timeout = 5003,       //正在登录中，请等待登录回调后再操作
    eFlag_RequestTooFrequently = 5004,//调用接口太频繁

    eFlag_Guest_AccessTokenInvalid = 4001,  // Guest的票据失效(for ios)
    eFlag_Guest_LoginFailed = 4002,         // Guest模式登录失败(for ios)
    eFlag_Guest_RegisterFailed = 4003,      // Guest模式注册失败(for ios)
    


	eFlag_CloudGame_Params_Error = 7001,	// 云游戏参数错误
    
    eFlag_Apple_NeedCompileXcode11 = 8001,    // 未使用Xcode 11 编译的 MSDK (for iOS)
    eFlag_Apple_NotSupport = 8002,    // iOS 12 及以下版本不支持 Apple 登录 (for iOS)
    eFlag_Apple_UserCancel = 8003,    // Apple 登录用户取消 (for iOS)
    eFlag_Apple_LoginFailed = 8004,    // Apple 登录失败 (for iOS)
    eFlag_Apple_AccessTokenExpired = 8005,      // Apple access token 过期 (for iOS)
    eFlag_Apple_Revoked = 8006,      // Apple 登录,用户撤销了该授权 (for iOS)
    eFlag_Apple_NotFound = 8007, // Apple 登录，用户没有对其授权 (for iOS)
    eFlag_Apple_Authorized = 8008, // Apple 登录，用户已经授权 (for iOS, 内部使用)
    eFlag_Apple_Unknown = 8009, // Apple 登录，未知错误
    eFlag_Apple_NetworkError = 8010, // Apple 登录，网络错误

} eFlag;

typedef enum _eShare {
    eShare_Succ = 0,
} eShare;

typedef enum _eTokenType {
    eToken_QQ_Access = 1,  // 手Q accessToken
    eToken_QQ_Pay,         // 手Q payToken
    eToken_WX_Access,      // 微信accessToken
    eToken_WX_Code,        // 微信code, 已弃用
    eToken_WX_Refresh,     // 微信refreshToken
    eToken_Guest_Access,   // Guest模式下的票据
	eToken_QQ_Refresh,		// 手Q refreshToken
    eToken_Apple_Access,    // Apple登录下的票据
    eToken_Apple_Refresh    // Apple登录下的票据
} eTokenType;

typedef enum _eWXMessageType {
    eWXMessageType_Text = 1,
    eWXMessageType_Image,  //图片
    eWXMessageType_Video,  //视频
    eWXMessageType_Link,   //链接
} eWXMessageType;          // WX GameCenter 消息类型

typedef enum _eWXButtonType {
    eWXButtonType_Rank = 1,  //跳排行榜
    eWXButtonType_App,       //唤起app
    eWXButtonType_Web,       //跳网页
} eWXButtonType;             // button响应类型

typedef enum _ePermission {
    eOPEN_NONE = 0,
    eOPEN_PERMISSION_GET_USER_INFO = 0x2,
    eOPEN_PERMISSION_GET_SIMPLE_USER_INFO = 0x4,
    eOPEN_PERMISSION_ADD_ALBUM = 0x8,
    eOPEN_PERMISSION_ADD_IDOL = 0x10,
    eOPEN_PERMISSION_ADD_ONE_BLOG = 0x20,
    eOPEN_PERMISSION_ADD_PIC_T = 0x40,
    eOPEN_PERMISSION_ADD_SHARE = 0x80,
    eOPEN_PERMISSION_ADD_TOPIC = 0x100,
    eOPEN_PERMISSION_CHECK_PAGE_FANS = 0x200,
    eOPEN_PERMISSION_DEL_IDOL = 0x400,
    eOPEN_PERMISSION_DEL_T = 0x800,
    eOPEN_PERMISSION_GET_FANSLIST = 0x1000,
    eOPEN_PERMISSION_GET_IDOLLIST = 0x2000,
    eOPEN_PERMISSION_GET_INFO = 0x4000,
    eOPEN_PERMISSION_GET_OTHER_INFO = 0x8000,
    eOPEN_PERMISSION_GET_REPOST_LIST = 0x10000,
    eOPEN_PERMISSION_LIST_ALBUM = 0x20000,
    eOPEN_PERMISSION_UPLOAD_PIC = 0x40000,
    eOPEN_PERMISSION_GET_VIP_INFO = 0x80000,
    eOPEN_PERMISSION_GET_VIP_RICH_INFO = 0x100000,
    eOPEN_PERMISSION_GET_INTIMATE_FRIENDS_WEIBO = 0x200000,
    eOPEN_PERMISSION_MATCH_NICK_TIPS_WEIBO = 0x400000,
    eOPEN_PERMISSION_GET_APP_FRIENDS = 0x800000,
    eOPEN_ALL = 0xffffff,
} ePermission;

typedef enum _eWechatScene {
    WechatScene_Session = 0,            //聊天界面
    WechatScene_Timeline = 1,           //朋友圈
    WechatScene_Favorite = 2,           //收藏
    WechatScene_SpecifiedSession = 3,   //指定联系人
    WechatScene_State = 4               //状态
} eWechatScene;

typedef enum _eMiniProgramType{
	ProgramType_Release = 0,
	ProgramType_Test = 1,
	ProgramType_Preview = 2,
}eMiniProgramType;


typedef enum _eQQScene {
    QQScene_QZone = 1,    //默认弹出分享到Qzone的弹框
    QQScene_Session = 2,  //默认弹出分享给好友的弹框
} eQQScene;

typedef enum _eADType {
    Type_Pause = 1,  // 暂停位广告
    Type_Stop = 2,   // 退出位广告
} eADType;

typedef enum _eMSG_NOTICETYPE {
    eMSG_NOTICETYPE_ALERT = 0,
    eMSG_NOTICETYPE_SCROLL = 1,
    eMSG_NOTICETYPE_ALL = 2,
} eMSG_NOTICETYPE;

typedef enum _eMSG_CONTENTTYPE {
    eMSG_CONTENTTYPE_TEXT = 0,   //文本公告
    eMSG_CONTENTTYPE_IMAGE = 1,  //图片公告
    eMSG_CONTENTTYPE_WEB = 2,    //网页公告
} eMSG_CONTENTTYPE;


typedef enum _eMSDK_SCREENDIR {
    eMSDK_SCREENDIR_SENSOR = 0,     //横竖屏
    eMSDK_SCREENDIR_PORTRAIT = 1,   //竖屏
    eMSDK_SCREENDIR_LANDSCAPE = 2,  //横屏
} eMSDK_SCREENDIR;

typedef enum _ePicType {
    ePicType_NONE = 0,
    ePicType_PNG = 1,
    ePicType_JPG = 2,
    ePicType_GIF = 3,
} ePicType;

typedef enum _eMSDK_NOTIFYTYPE {
    eMSDK_NOTIFYTYPE_ONLOGINNOTIFY = 0,
    eMSDK_NOTIFYTYPE_ONSHARENOTIFY = 1,
    eMSDK_NOTIFYTYPE_ONWAKEUPNOTIFY = 2,
    eMSDK_NOTIFYTYPE_ONRELATIONNOTIFY = 3,
    eMSDK_NOTIFYTYPE_ONLOCATIONNOTIFY = 4,
    eMSDK_NOTIFYTYPE_ONLOCATIONGOTNOTIFY = 5,
    eMSDK_NOTIFYTYPE_ONFEEDBACKNOTIFY = 6,
    eMSDK_NOTIFYTYPE_ONCRASHEXTMESSAGENOTIFY = 7,
    eMSDK_NOTIFYTYPE_OADNOTIFY = 8,
    eMSDK_NOTIFYTYPE_ONCREATEWXGROUPNOTIFY,
    eMSDK_NOTIFYTYPE_OnQUERYGROUPINFONOTIFY,
    eMSDK_NOTIFYTYPE_OnJOINWXGROUPNOTIFY,
    eMSDK_NOTIFYTYPE_OnWEBVIEWNOTIFY,
    eMSDK_NOTIFYTYPE_OnREALNAMEAUTHNOTIFY,
    eMSDK_NOTIFYTYPE_OnQUERYWXGROUPSTATUSNOTIFY,
    eMSDK_NOTIFYTYPE_OnUNLINKWXGROUPNOTIFY,
    eMSDK_NOTIFYTYPE_OnGETQQGROUPKEYNOTIFY,
    eMSDK_NOTIFYTYPE_OnCREATEBINDQQGROUPNOTIFY,
    eMSDK_NOTIFYTYPE_OnJOINQQGROUPNOTIFY,
    eMSDK_NOTIFYTYPE_OnUNLINKQQGROUPNOTIFY
} eMSDK_NOTIFYTYPE;

typedef enum _eMSDK_SHARETYPE { MSDK_SHARETYPE_QQ = 0, MSDK_SHARETYPE_WX = 1 } MSDK_SHARETYPE;

typedef enum _eStatusType {
    eStatusType_ISCREATED = 0,  // 查询是否建群
    eStatusType_ISJONINED = 1,  // 查询是否加群
} eStatusType;

typedef enum _eMSDK_SUBSHARETYPE {
    MSDK_SUBSHARETYPE_NORMAL = 0,  //结构化消息
    MSDK_SUBSHARETYPE_LINK,        // url消息
    MSDK_SUBSHARETYPE_LARGEPHOTO,  //大图消息
    MSDK_SUBSHARETYPE_MUSIC,       //音乐消息
} MSDK_SUBSHARETYPE;

typedef enum _eGuestData_MigrateStatus {
    eGuestData_BeforeMigrate = 0,
    eGuestData_AfterMigrate,
    eGuestData_GuestRegister
} eGuestData_MigrateStatus;

typedef enum _eBuglyLogLevel {
    eBuglyLogLevel_S = 0,  // Silent
    eBuglyLogLevel_E = 1,  // Error
    eBuglyLogLevel_W = 2,  // Warning
    eBuglyLogLevel_I = 3,  // Info
    eBuglyLogLevel_D = 4,  // Debug
    eBuglyLogLevel_V = 5   // Verbose
} eBuglyLogLevel;

typedef enum _eIDType {
    eIDType_IDCards = 0,           //身份证
    eIDType_HKMacaoPass = 1,       //港澳台居民往内地通行证
    eIDType_HKMacaoTaiwanID = 2,   //港澳台身份证
    eIDType_Passport = 3,          //护照
    eIDType_PoliceCertificate = 4  //军人/警察身份证
} eIDType;

typedef enum _eRealNameAuthErrCode {
    eErrCode_SystemError = 1,           //系统错误
    eErrCode_NoAuth = 2,                //没有权限
    eErrCode_RequestFrequently = 3,     //访问频繁，稍后再试
    eErrCode_NoRecord = 4,              //没有该记录
    eErrCode_AlreadyRegisted = 5,       //用户已注册
    eErrCode_BindCountLimit = 6,        //证件绑定超过限制
    eErrCode_UserNoRegist = 7,          //用户没注册(修改时可能返回)
    eErrCode_InvalidParam = 100,        //非法参数输入
    eErrCode_InvalidIDCard = 101,       //非法的身份证，当证件类型为身份证时会校验
    eErrCode_InvalidBirth = 102,        //非法的生日，当证件为非身份证时会校验
    eErrCode_InvalidChineseName = 103,  //非法的中文名字
    eErrCode_InvalidToken = 104         // token过期或不合法
} eRealNameAuthErrCode;

typedef enum _eRelationRetType
{
    eRet_QueryMyInfo = 0,  // 查询个人信息
    eRet_QueryGameFriends = 1, //查询同玩好友
	eRet_QueryUnionID = 2  // 查询unionID
}eRelationRetType;

typedef enum _eExceptionType
{
    eException_CSharp = 4,  // C#异常
    eException_JS = 5, //js异常
    eException_Lua = 6 //lua异常
}eExceptionType;

#ifdef ANDROID
typedef enum _ApiName {
    eApiName_WGSendToQQWithPhoto = 0,
    eApiName_WGJoinQQGroup = 1,
    eApiName_WGAddGameFriendToQQ = 2,
    eApiName_WGBindQQGroup = 3
} eApiName;

enum TMSelfUpdateUpdateInfo
{
    STATUS_OK = 0,                            // 检查成功
    STATUS_CHECKUPDATE_FAILURE = 1,           // 查询更新失败
    STATUS_CHECKUPDATE_RESPONSE_IS_NULL = 2,  // 查询响应为空

    UpdateMethod_NoUpdate = 0,  // 无更新包
    UpdateMethod_Normal = 1,    // 有全量更新包
    UpdateMethod_ByPatch = 2    // 有省流量更新包
};

enum TMAssistantDownloadTaskState
{
    DownloadSDKTaskState_WAITING = 1,
    DownloadSDKTaskState_DOWNLOADING = 2,
    DownloadSDKTaskState_PAUSED = 3,
    DownloadSDKTaskState_SUCCEED = 4,
    DownloadSDKTaskState_FAILED = 5,
    DownloadSDKTaskState_DELETE = 6,
    // LOWWER_VERSION_INSTALLED  = 2,
    // UN_INSTALLED  = 1,
    // ALREADY_INSTALLED = 0
};

enum TMSelfUpdateTaskState
{
    SelfUpdateSDKTaskState_SUCCESS = 0,
    SelfUpdateSDKTaskState_DOWNLOADING = 1,
    SelfUpdateSDKTaskState_FAILURE = 2
};
enum TMYYBInstallState
{
    LOWWER_VERSION_INSTALLED = 2,
    UN_INSTALLED = 1,
    ALREADY_INSTALLED = 0
};

enum TMAssistantDownloadErrorCode
{
    // DownloadSDK_START_FAILED_BUSY = 5,
    // DownloadSDK_START_FAILED_EXISTED  = 4,
    // DownloadSDK_START_FAILED_NETWORK_NOT_CONNECTED    = 1,
    // DownloadSDK_START_FAILED_ONLY_FOR_WIFI    = 2,
    // DownloadSDK_START_FAILED_PARAMETERS_INVALID   = 3,
    // DownloadSDK_START_SUCCEED = 0,
    DownloadSDKErrorCode_CLIENT_PROTOCOL_EXCEPTION = 732,
    DownloadSDKErrorCode_CONNECT_TIMEOUT = 601,
    DownloadSDKErrorCode_HTTP_LOCATION_HEADER_IS_NULL = 702,
    DownloadSDKErrorCode_INTERRUPTED = 600,
    DownloadSDKErrorCode_IO_EXCEPTION = 606,
    DownloadSDKErrorCode_NONE = 0,
    DownloadSDKErrorCode_PARSER_CONTENT_FAILED = 704,
    DownloadSDKErrorCode_RANGE_NOT_MATCH = 706,
    DownloadSDKErrorCode_REDIRECT_TOO_MANY_TIMES = 709,
    DownloadSDKErrorCode_RESPONSE_IS_NULL = 701,
    DownloadSDKErrorCode_SET_RANGE_FAILED = 707,
    DownloadSDKErrorCode_SOCKET_EXCEPTION = 605,
    DownloadSDKErrorCode_SOCKET_TIMEOUT = 602,
    DownloadSDKErrorCode_SPACE_NOT_ENOUGH = 730,
    DownloadSDKErrorCode_TOTAL_SIZE_NOT_SAME = 705,
    DownloadSDKErrorCode_UNKNOWN_EXCEPTION = 604,
    DownloadSDKErrorCode_UNKNOWN_HOST = 603,
    DownloadSDKErrorCode_URL_EMPTY = 731,
    DownloadSDKErrorCode_URL_HOOK = 708,
    DownloadSDKErrorCode_URL_INVALID = 700,
    DownloadSDKErrorCode_WRITE_FILE_FAILED = 703,
    DownloadSDKErrorCode_WRITE_FILE_NO_ENOUGH_SPACE = 710,
    DownloadSDKErrorCode_WRITE_FILE_SDCARD_EXCEPTION = 711
};
#endif

#endif
