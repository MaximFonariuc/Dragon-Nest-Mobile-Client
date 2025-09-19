//
//  iOSConnector.h
//  MSDKDemo
//
//  Created by 付亚明 on 1/20/15.
//  Modefied by qingcuilu on 11/10/15.
//  Copyright (c) 2015 Tencent. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <MSDK/MSDK.h>
#import <MSDK/MSDKPublicDefine.h>
#define kMsdkMethod         @"MsdkMethod"

#ifdef __cplusplus
extern "C" {
#endif
    
    extern char* iOSConnectorMakeStringCopy(const char* string);
    /**
     * 辅助Unity进行MSDK日志打印
     */
    extern void iosLog(char* msg);
     
    /**
     * 广告回调设置
     */
    extern void SetADObserver(WGADObserver* pADObserver);


    /**
     *   OnLoginNotify: 登陆回调
     *   OnShareNotify: 分享回调
     *   OnWakeupNotify: 被唤起回调
     *   OnRelationNotify: 关系链查询回调
     *   OnReceiveNotificationResponseNotify:tpns点击消息回调
     * @param pObserver 游戏传入的全局回调对象
     */
    extern void SetObserver(WGPlatformObserver* pObserver);

    /**
     *   OnCreateWXGroupNotify: 创建微信群回调
     *   OnQueryGroupInfoNotify: 查询群信息回调
     *   OnJoinWXGroupNotify: 加入微信群回调
     * @param pGroupObserver 游戏传入的全局回调对象
     */
    extern void SetGroupObserver(WGGroupObserver* pGroupObserver);

    /**
     * @param loginRet 返回的记录
     * @return 返回值为平台id, 类型为ePlatform, 返回ePlatform_None表示没有登陆记录
     *   loginRet.platform(类型为ePlatform)表示平台id, 可能值为ePlatform_QQ, ePlatform_Weixin, ePlatform_None.
     *   loginRet.flag(类型为eFlag)表示当前本地票据的状态, 可能值及说明如下:
     *     eFlag_Succ: 授权票据有效
     *     eFlag_QQ_AccessTokenExpired: 手Q accessToken已经过期, 显示授权界面, 引导用户重新授权
     *     eFlag_WX_AccessTokenExpired: 微信accessToken票据过期，需要调用WGRefreshWXToken刷新
     *     eFlag_WX_RefreshTokenExpired: 微信refreshToken, 显示授权界面, 引导用户重新授权
     *   ret.token是一个Vector<TokenRet>, 其中存放的TokenRet有type和value, 通过遍历Vector判断type来读取需要的票据. type类型定义如下:
     *   	eToken_QQ_Access = 1,
     eToken_QQ_Pay,
     eToken_WX_Access,
     eToken_WX_Refresh,
     *
     * 注意: 游戏通过此接口获取到的票据以后必须传到游戏Server, 通过游戏Server调用MSDK后端验证票据接口验证票据有效以后才能让用户进入游戏.
     */
    extern char* GetLoginRecord();

    /**
     * @param platform 游戏传入的平台类型, 可能值为: ePlatform_QQ = 2, ePlatform_Weixin = 1
     * @return void
     *   通过游戏设置的全局回调的OnLoginNotify(LoginRet& loginRet)方法返回数据给游戏
     *     loginRet.platform表示当前的授权平台, 值类型为ePlatform, 可能值为ePlatform_QQ, ePlatform_Weixin
     *     loginRet.flag值表示返回状态, 可能值(eFlag枚举)如下：
     *       eFlag_Succ: 返回成功, 游戏接收到此flag以后直接读取LoginRet结构体中的票据进行游戏授权流程.
     *       eFlag_QQ_NoAcessToken: 手Q授权失败, 游戏接收到此flag以后引导用户去重新授权(重试)即可.
     *       eFlag_QQ_UserCancel: 用户在授权过程中
     *       eFlag_QQ_LoginFail: 手Q授权失败, 游戏接收到此flag以后引导用户去重新授权(重试)即可.
     *       eFlag_QQ_NetworkErr: 手Q授权过程中出现网络错误, 游戏接收到此flag以后引导用户去重新授权(重试)即可.
     *     loginRet.token是一个Vector<TokenRet>, 其中存放的TokenRet有type和value, 通过遍历Vector判断type来读取需要的票据. type(TokenType)类型定义如下:
     *       eToken_QQ_Access = 1,
     *       eToken_QQ_Pay = 2,
     *       eToken_WX_Access = 3,
     *       eToken_WX_Refresh = 5
     */
    extern void Login(int platform);

    /**
     * @brief 微信视频号授权
     * 
     * @param platform 
     * @param permissions 
     */
    extern void ChannelPermissionAuth(int platform, unsigned char * permissions);
    
    extern int LoginOpt(int platform,int overtime);
    /**
     * @param platform 游戏传入的平台类型, 可能值为: ePlatform_QQ, ePlatform_Weixin
     * @return void
     * 通过游戏设置的全局回调的OnLoginNotify(LoginRet& loginRet)方法返回数据给游戏
     */
    extern void WGQrCodeLogin(int platform);

  /**
    * 帮助开发者检查微信 Universal Link 配置是否正确的函数
    * @return voidUL
    * 通过游戏设置的全局回调的OnChekULNotify(CheckULRet& checkULRet)方法返回数据给游戏
    */
   extern void WGCheckWXUniversalLink();

    /**
     * @return bool 返回值已弃用, 全都返回true
     */
    extern bool Logout();

    /**
     * @param permissions ePermission枚举值 或 运算的结果, 表示需要的授权项目
     * @return void
     */
    extern void SetPermission(int permissions);

    /**
     * @param title 结构化消息的标题
     * @param desc 结构化消息的概要信息
     * @param mediaTagName 请根据实际情况填入下列值的一个, 此值会传到微信供统计用, 在分享返回时也会带回此值, 可以用于区分分享来源
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
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
     * @return void
     *   通过游戏设置的全局回调的OnShareNotify(ShareRet& shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
     */
    extern void SendToWeixin(
                                  char* title,
                                  char* desc,
                                  char* mediaTagName,
                                  Byte* thumbImgData,
                                  int thumbImgDataLen,
                                  char* messageExt,
                                  char *userOpenId = (char  *)""
                                  );


    /**
     * @param scene 指定分享到朋友圈, 或者微信会话, 可能值和作用如下:
     *   0: 分享到微信会话
     *   1: 分享到微信朋友圈
     * @param title 结构化消息的标题
     * @param desc 结构化消息的概要信息
     * @param url 分享的URL
     * @param mediaTagName 请根据实际情况填入下列值的一个, 此值会传到微信供统计用, 在分享返回时也会带回此值, 可以用于区分分享来源
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
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
     * @return void
     *   通过游戏设置的全局回调的OnShareNotify(ShareRet& shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
     */
    extern void SendToWeixinWithUrl(
                                         int scene,
                                         char* title,
                                         char* desc,
                                         char* url,
                                         char* mediaTagName,
                                         Byte* thumbImgData,
                                         int thumbImgDataLen,
                                         char* messageExt,
                                         char *userOpenId = (char *)""
                                         );

    /**
     * 游戏原生页分享
     * @param scene 指定分享到朋友圈, 或者微信会话
     * @param title 结构化消息的标题
     * @param desc 结构化消息的概要信息
     * @param thumbImgData 结构化消息的缩略图
     * @param mediaTagName 供微信统计使用
     * @param thumbImgDataLen 结构化消息的缩略图数据长度
     * @param isVideo 是否是视频类型，非必要
     * @param videoDuration 视频时长，单位秒(视频类型时，必要)
     * @param shareData 透传字段
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过
     * OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
     * @param messageAction 暂时不用可以空字串
     * @param userOpenId 指定分享给特定的好友
     */
    extern void SendToWXNativeGamePage(int scene,
                                unsigned char *title,
                                unsigned char *desc,
                                unsigned char *mediaTagName,
                                unsigned char *thumbImgData,
                                int thumbImgDataLen,
                                bool isVideo,
                                int videoDuration,
                                unsigned char *shareData,
                                unsigned char *messageExt,
                                unsigned char *messageAction,
                                unsigned char *userOpenId = (unsigned char *)""
                                );

    /*
     * @param scene 指定分享到朋友圈, 或者微信会话, 可能值和作用如下:
     *   0: 分享到微信会话
     *   1: 分享到微信朋友圈
     * @param mediaTagName 请根据实际情况填入下列值的一个, 此值会传到微信供统计用, 在分享返回时也会带回此值, 可以用于区分分享来源
     "MSG_INVITE";                   // 邀请
     "MSG_SHARE_MOMENT_HIGH_SCORE";    //分享本周最高到朋友圈
     "MSG_SHARE_MOMENT_BEST_SCORE";    //分享历史最高到朋友圈
     "MSG_SHARE_MOMENT_CROWN";         //分享金冠到朋友圈
     "MSG_SHARE_FRIEND_HIGH_SCORE";     //分享本周最高给好友
     "MSG_SHARE_FRIEND_BEST_SCORE";     //分享历史最高给好友
     "MSG_SHARE_FRIEND_CROWN";          //分享金冠给好友
     "MSG_friend_exceed"         // 超越炫耀
     "MSG_heart_send"            // 送心
     * @param imgData 原图文件数据
     * @param imgDataLen 原图文件数据长度(图片大小不能超过10M)
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
     * @param messageAction scene为1(分享到微信朋友圈)的情况下才起作用
     *   WECHAT_SNS_JUMP_SHOWRANK       跳排行
     *   WECHAT_SNS_JUMP_URL            跳链接
     *   WECHAT_SNS_JUMP_APP           跳APP
     * @return void
     *   通过游戏设置的全局回调的OnShareNotify(ShareRet& shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
     */
    extern void SendToWeixinWithPhoto(
                                                     int scene,
                                                     char *mediaTagName,
                                                     Byte *imgData,
                                                     int imgDataLen,
                                                     char *messageExt,
                                                     char *messageAction
                                                     );

    //微信小程序分享接口
    /*
     webpageUrl 旧版本微信打开该小程序分享时，兼容跳转的普通页面url(必填，可任意url，用于老版本兼容)
     userName 小程序username，如gh_d43f693ca31f
     path 小程序path，可通过该字段指定跳转小程序的某个页面（若不传，默认跳转首页，此处path游戏可线下向微信申请小程序页面）
     withShareTicket 是否带shareTicket转发（如果小程序页面要展示用户维度的数据，并且小程序可能分享到群，需要设置为YES）
     */
    extern void SendToWXWithMiniApp(int scene,
                             unsigned char *title,
                             unsigned char *desc,
                             unsigned char *thumbImgData,
                             int thumbImgDataLen,
                             unsigned char *webpageUrl,
                             unsigned char *userName,
                             unsigned char *path,
                             bool withShareTicket,
                             unsigned char *messageExt,
                             unsigned char *messageAction,
                             const unsigned char *mediaTagName = (const unsigned char *)"",
                             int type = 0,
                             const unsigned char *userOpenId  = (const unsigned char *)"");

     /**
     * 启动小程序
     * @param userName  //  填小程序原始id
     * @param path      //  拉起小程序页面的可带参路径，不填默认拉起小程序首页
     * @param type      //  preview = 2 ; test = 1 ; release = 0;
     */
    extern void LaunchMiniApp(const unsigned char *userName,
                         const unsigned char *path,
                         int type
                            );
     /**
     * 区别于小程序，微信提供基于原生的游戏直播/围观功能
     * @param scene 朋友圈或者好友列表
     * @param title 标题
     * @param desc 描述
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
     * @param extInfo  直播/围观功能的专用参数 一个key-value形式的vector
     * 例：[{"key":"xxx","value":"xxx"},{"key":"yyy","value":"yyyy"}]
     */
    void WGSendToWXGameLive(int scene, unsigned char *title, unsigned char *desc, unsigned char *messageExt, char *extInfo);


     /**  分享ARK到QQ
     *
     * 分享时调用
     * @param scene QQScene_QZone:空间，默认弹框 QQScene_Session:好友
     * @param title 标题
     * @param desc 内容
     * @param url  内容的跳转url，填游戏对应游戏中心详情页
     * @param imgUrl 图片url
     * @param jsonString ARK分享jsonString（此处由业务侧与手q ARK开发时约定如何配置，msdk仅透传）
     * @return void
     */
    extern void SendToQQWithArk(
                           int scene,
                           unsigned char* title,
                           unsigned char* desc,
                           unsigned char* url,
                           unsigned char* imgUrl,
                           unsigned char* jsonString,
                           unsigned char* messageExt,
                           unsigned char* gameTag
                           );

    /**  分享纯文字到QQ空间
     *
     * 分享时调用
     * @param text 内容
     * @return 
     */
    extern void SendToQQWithText(unsigned char* text, unsigned char* extraScene, unsigned char* messageExt);


    /**
     * 上报C#、JS、Lua等异常
     * exception_type：异常类型
     * exception_name：异常名称
     * excetipon_name：异常信息
     * exception_stack：异常堆栈
     * ext_info：附加信息
     */
    extern void ReportException(int exception_type,
                         char* exception_name,
                         char* exception_msg,
                         char* exception_stack,
                         char* ext_info);
    
    //视频分享到朋友圈接口
    extern void SendToWeixinWithVideo(int scene,
                               unsigned char *title,
                               unsigned char *desc,
                               /*unsigned char *thumbUrl,
                               unsigned char *videoUrl,*/
                               unsigned char *ios_videoData,
                               int ios_videoDataLen,
                               unsigned char *mediaTagName,
                               unsigned char *messageAction,
                               unsigned char *messageExt);

    /**
     *
     * 分享图片信息至微信状态
     * 参数说明：
     * stateId：状态ID，选填，不同的状态ID会显示不同的Icon
     * stateTitle：状态标题，选填，限制32 个字符以内
     * imageData：图片数据，必填，图片比例为9:16
     * imageDataLen：图片数据长度
     * jumpUrl：跳转URL，必填，点击来源小尾巴后跳转的页面url，游戏先支持跳到“微信游戏圈”页面
     *
     */
    extern void SendToWXStateWithPhoto(const unsigned char *stateId,
                                       const unsigned char *stateTitle,
                                       const unsigned char *imageData,
                                       const int imageDataLen,
                                       const unsigned char *jumpUrl);


    extern void SendToWXStateWithPhotoOpenMiniApp(const unsigned char*stateId,
                                            const unsigned char*stateTitle, 
                                            const unsigned char* imageData, 
                                            const int imageDataLen, 
                                            const unsigned char* userName, 
                                            const unsigned char* path, 
                                            const int type);

    /**
     *
     * 分享视频至微信视频号
     * 参数说明：
     * localIdentify：视频信息，必填，填写系统相册中视频标识符PHAsset localIdentifier，视频格式目前仅支持MP4，时长不超过30分钟，大小不超过450M，宽高比在1:3 - 3:1之间
     * messageExt：拓展字段，选填，携带额外信息，当前版本暂未使⽤
     *
     */
    extern void SendToWXChannelShareVideo(const unsigned char *localIdentify, const unsigned char *messageExt);
    
    /**
     * 微信视频号直播
     * 参数说明：
     * channelLiveJson 游戏拉起视频号直播透传json数据，包括liveJsonInfo和openID字段
     * messageExt 拓展字段，选填，携带额外信息，当前版本暂未使⽤
     */
    extern void SendToWXChannelStartLive(unsigned char * channelLiveJson, unsigned char * messageExt);

    /**
     * 用户反馈接口, 反馈内容查看链接(Tencent内网):
     * 		http://mcloud.ied.com/queryLogSystem/ceQuery.html?token=545bcbcfada62a4d84d7b0ee8e4b44bf&gameid=0&projectid=ce
     * @param body 反馈的内容, 内容由游戏自己定义格式, SDK对此没有限制
     * @return 通过OnFeedbackNotify回调反馈接口调用结果
     */
    extern void FeedbackWithBody(char* body);


    /**
     * @param bRDMEnable 是否开启RDM的crash异常捕获上报
     * @param bMTAEnable 是否开启MTA的crash异常捕获上报
     */
    extern void EnableCrashReport(bool bRDMEnable, bool bMTAEnable);


    /**
     * @param name 事件名称
     * @param eventList 事件内容, 一个key-value形式的vector
     * 例：[{"key":"xxx","value":"xxx"},{"key":"yyy","value":"yyyy"}]
     * @param isRealTime 是否实时上报
     * @return void
     */
    extern void ReportEvent(char *name, char *eventList, bool isRealTime);

    /**
     *  设置 MSDK 日志
     *  @param enabled true:打开 false:关闭
     */
    void OpenMSDKLog(bool enabled);

    /**
     * 返回MSDK版本号
     * @return MSDK版本号
     */
    extern char* GetVersion();


    /**
     * 如果没有再读assets/channel.ini中的渠道号, 故游戏测试阶段可以自己写入渠道号到assets/channel.ini用于测试.
     * IOS返回plist中的CHANNEL_DENGTA字段
     * @return 安装渠道
     */
    extern char* GetChannelId();


    /**
     * @return APP版本号
     */
    extern char* GetPlatformAPPVersion(int platform);


    /**
     * @return 注册渠道
     */
    extern char* GetRegisterChannelId();


    /**
     * 此接口用于刷新微信的accessToken
     * refreshToken的用途就是刷新accessToken, 只要refreshToken不过期就可以通过refreshToken刷新accessToken。
     * 有两种情况需要刷新accessToken,
     * @return void
     *   通过游戏设置的全局回调的OnLoginNotify(LoginRet& loginRet)方法返回数据给游戏
     *     因为只有微信平台有refreshToken, loginRet.platform的值只会是ePlatform_Weixin
     *     loginRet.flag值表示返回状态, 可能值(eFlag枚举)如下：
     *       eFlag_WX_RefreshTokenSucc: 刷新票据成功, 游戏接收到此flag以后直接读取LoginRet结构体中的票据进行游戏授权流程.
     *       eFlag_WX_RefreshTokenFail: WGRefreshWXToken调用过程中网络出错, 刷新失败, 游戏自己决定是否需要重试 WGRefreshWXToken
     */
    extern void RefreshWXToken();


    /**
     * @param platformType 游戏传入的平台类型, 可能值为: ePlatform_QQ, ePlatform_Weixin
     * @return 平台的支持情况, false表示平台不支持授权, true则表示支持
     */
    extern bool IsPlatformInstalled(int platformType);


    /**
     * 检查平台是否支持SDK API接口
     * @param platformType 游戏传入的平台类型, 可能值为: ePlatform_QQ, ePlatform_Weixin
     * @return 平台的支持情况, false表示平台不支持授权, true则表示支持
     */
    extern bool IsPlatformSupportApi(ePlatform platformType);


    /**
     * 获取pfkey，pfKey由msdk 服务器加密生成，支付过程校验
     * @return 返回当前pf加密后对应fpKey字符串
     */
    extern char* GetPfKey();


    /**
     *  输出msdk用到的各sdk版本号
     */
    extern void LogPlatformSDKVersion();


    /**
     * 获取自己的QQ资料
     * @return void
     *   此接口的调用结果通过OnRelationNotify(RelationRet& relationRet) 回调返回数据给游戏,
     *   RelationRet对象的persons属性是一个Vector<PersonInfo>, 取第0个即是用户的个人信息.
     *   手Q授权的用户可以获取到的个人信息包含:
     *   nickname, openId, gender, pictureSmall, pictureMiddle, pictureLarge, gpsCity, 其他字段为空.
     *   其中gpsCity字段为玩家所在城市信息，只有游戏调用过 WGGetNearbyPersonInfo 或者 WGGetLocationInfo
     *   接口后，这个字段才有相应信息。
     */
    extern bool QueryQQMyInfo();


    /**
     * 获取QQ好友信息, 回调在OnRelationNotify中,
     * 其中RelationRet.persons为一个Vector, Vector中的内容即使好友信息, QQ好友信息里面province和city为空
     * @return void
     * 此接口的调用结果通过OnRelationNotify(RelationRet& relationRet)
     *   回调返回数据给游戏, RelationRet对象的persons属性是一个Vector<PersonInfo>,
     *   其中的每个PersonInfo对象即是好友信息,
     *   好友信息包含: nickname, openId, gender, pictureSmall, pictureMiddle, pictureLarge
     */
    extern bool QueryQQGameFriendsInfo();


    /**
     *   回调在OnRelationNotify中,其中RelationRet.persons为一个Vector, Vector的第一项即为自己的资料
     *   个人信息包括nickname, openId, gender, pictureSmall, pictureMiddle, pictureLarge, provice, city, gpsCity
     *   其中gpsCity字段为玩家所在城市信息，只有游戏调用过 WGGetNearbyPersonInfo 或者 WGGetLocationInfo
     *   接口后，这个字段才有相应信息。
     */
    extern bool QueryWXMyInfo();


    /**
     * 获取微信好友信息, 回调在OnRelationNotify中,
     *   回调在OnRelationNotify中,其中RelationRet.persons为一个Vector, Vector中的内容即为好友信息
     *   好友信息包括nickname, openId, gender, pictureSmall, pictureMiddle, pictureLarge, provice, city
     */
    extern bool QueryWXGameFriendsInfo();


    /**
    * 获取unionid(qq用户唯一标识), 回调在OnRelationNotify中
    * 其中RelationRet.persons为一个Vector，Vector的第一项即为自己的资料
     * 个人信息中包含unionID，其类型为一个string
    */
    extern void QueryUnionID();

    /**
     * 创建微信群聊
     * @param unionid 公会ID
     * @param chatRoomName 公会名称(群名称)
     * @param chatRoomNickName 玩家名称(群里展示)
     */
    extern void CreateWXGroup(
                         unsigned char* unionid,
                         unsigned char* chatRoomName,
                         unsigned char* chatRoomNickName
                         );


    /**
     * 加入微信群聊
     * @param unionid 公会ID
     * @param chatRoomNickName 玩家名称(群里展示)
     */
    extern void JoinWXGroup(
                       unsigned char* unionid,
                       unsigned char* chatRoomNickName
                       );


    /**
     * 查询微信群聊成员信息
     * @param unionID 公会ID
     * @param openIdLists 公会成员openid,以","分隔
     */
    extern void QueryWXGroupInfo(
                            unsigned char* unionID,
                            unsigned char* openIdLists
                            );

    /**
     * 发送微信群消息
     * @param msgType 消息类型：
     1:open
     2:link(未实现)
     3:voice(未实现)
     4:text(未实现)
     * @param subType 子类型：
     1:邀请(填1)
     2:炫耀,
     3:赠送
     4:索要
     * @param unionID 公会ID
     * @param title 标题
     * @param desc 描述
     * @param mediaTagName 请根据实际情况填入下列值的一个, 此值会传到微信供统计用
     "MSG_INVITE";                   // 邀请
     "MSG_SHARE_MOMENT_HIGH_SCORE";    //分享本周最高到朋友圈
     "MSG_SHARE_MOMENT_BEST_SCORE";    //分享历史最高到朋友圈
     "MSG_SHARE_MOMENT_CROWN";         //分享金冠到朋友圈
     "MSG_SHARE_FRIEND_HIGH_SCORE";     //分享本周最高给好友
     "MSG_SHARE_FRIEND_BEST_SCORE";     //分享历史最高给好友
     "MSG_SHARE_FRIEND_CROWN";          //分享金冠给好友
     "MSG_friend_exceed"         // 超越炫耀
     "MSG_heart_send"            // 送心
     * @param imgUrl 图片CDN url
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
     * @param msdkExtInfo 游戏自定义透传字段，通过分享结果shareRet.extInfo返回给游戏
     */
    extern void SendToWXGroup(
                         int msgType,
                         int subType,
                         unsigned char* unionID,
                         unsigned char* title,
                         unsigned char* desc,
                         unsigned char* messageExt,
                         unsigned char* mediaTagName,
                         unsigned char* imgUrl,
                         unsigned char* msdkExtInfo
                         );

    /**
     * 打开微信deeplink（deeplink功能的开通和配置请联系微信游戏中心）
     * @param link 具体跳转deeplink，可填写为：
     *             INDEX：跳转微信游戏中心首页
     *             DETAIL：跳转微信游戏中心详情页
     *             LIBRARY：跳转微信游戏中心游戏库
     *             具体跳转的url （需要在微信游戏中心先配置好此url）
     */
    extern void OpenWeiXinDeeplink(unsigned char* link);

    /**
     * @param act 好友点击分享消息拉起页面还是直接拉起游戏, 传入 1 拉起游戏, 传入 0, 拉起targetUrl
     * @param fopenid 好友的openId
     * @param title 分享的标题
     * @param summary 分享的简介
     * @param targetUrl 内容的跳转url，填游戏对应游戏中心详情页，
     * @param imageUrl 分享缩略图URL
     * @param previewText 可选, 预览文字
     * @param gameTag 可选, 此参数必须填入如下值的其中一个
				 MSG_INVITE                //邀请
				 MSG_FRIEND_EXCEED       //超越炫耀
				 MSG_HEART_SEND          //送心
				 MSG_SHARE_FRIEND_PVP    //PVP对战
     * @param extMsdkInfo 游戏自定义透传字段，通过分享结果shareRet.extInfo返回给游戏，游戏可以用extInfo区分request
     */
    extern bool SendToQQGameFriend(
                                                   int act,
                                                   char* fopenid,
                                                   char *title,
                                                   char *summary,
                                                   char *targetUrl,
                                                   char *imgUrl,
                                                   char* previewText,
                                                   char* gameTag,
                                                   char* msdkExtInfo
                                                   );


    /**
     * 此接口类似WGSendToQQGameFriend, 此接口用于分享消息到微信好友, 分享必须指定好友openid
     * @param fOpenId 好友的openid
     * @param title 分享标题
     * @param description 分享描述
     * @param mediaId 图片的id 通过uploadToWX接口获取
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
     * @param mediaTagName 请根据实际情况填入下列值的一个, 此值会传到微信供统计用, 在分享返回时也会带回此值, 可以用于区分分享来源
     "MSG_INVITE";                   // 邀请
     "MSG_SHARE_MOMENT_HIGH_SCORE";    //分享本周最高到朋友圈
     "MSG_SHARE_MOMENT_BEST_SCORE";    //分享历史最高到朋友圈
     "MSG_SHARE_MOMENT_CROWN";         //分享金冠到朋友圈
     "MSG_SHARE_FRIEND_HIGH_SCORE";     //分享本周最高给好友
     "MSG_SHARE_FRIEND_BEST_SCORE";     //分享历史最高给好友
     "MSG_SHARE_FRIEND_CROWN";          //分享金冠给好友
     "MSG_friend_exceed"         // 超越炫耀
     "MSG_heart_send"            // 送心
     * @param msdkExtInfo 游戏自定义透传字段，通过分享结果shareRet.extInfo返回给游戏
     */
    extern bool SendToWXGameFriend(
                                                   char* fOpenId,
                                                   char* title,
                                                   char* description,
                                                   char* mediaId,
                                                   char* messageExt,
                                                   char* mediaTagName,
                                                   char* msdkExtInfo
                                                   );


    /**
     *  @since 2.0.0
     *  此接口用于已经登录过的游戏, 在用户再次进入游戏时使用, 游戏启动时先调用此接口, 此接口会尝试到后台验证票据
     *  此接口会通过OnLoginNotify将结果回调给游戏, 本接口只会返回两种flag, eFlag_Local_Invalid和eFlag_Succ,
     *  如果本地没有票据或者本地票据验证失败返回的flag为eFlag_Local_Invalid, 游戏接到此flag则引导用户到授权页面授权即可.
     *  如果本地有票据并且验证成功, 则flag为eFlag_Succ, 游戏接到此flag则可以直接使用sdk提供的票据, 无需再次验证.
     *  @return void
     *   Callback: 验证结果通过我OnLoginNotify返回
     */
    extern void LoginWithLocalInfo();


    /*
     * @param scene 公告场景ID，不能为空
     */
    extern void ShowNotice(char* scene);


    /**
     * 隐藏滚动公告
     */
    extern void HideScrollNotice();


    /**
     *  @param openUrl 要打开的url
     */
    extern void OpenUrl(char* openUrl, const char *algorithm = "v2");
    

    /**
     *  @param openUrl 要打开的url
     *  @param screenDir 内置浏览器支持的屏幕方向,0横竖屏,1竖屏,2横屏
     */
    extern void OpenUrlWithScreenDir(unsigned char * openUrl, int screenDir, const char *algorithm = "v2");

    
    /**
     *  @param openUrl 要打开的url
     *  @param screenDir 内置浏览器支持的屏幕方向,0横竖屏,1竖屏,2横屏
     */
    extern void OpenUrlWithScreenDirAndFullScreen(unsigned char * openUrl, int screenDir, bool fullScreen, const char *algorithm = "v2");


    /*
     * 加密Url，返回票据加密后的url
     * @param openUrl 需要增加加密参数的url
     */
    extern char* GetEncodeUrl(unsigned char * openUrl, const char *algorithm = "v2");

    /*
     * 从本地数据库读取指定scene下指定type的当前有效公告
     * @param sence 这个参数和公告管理端的“公告栏”对应
     * @return NoticeInfo结构的数组，NoticeInfo结构如下：
     typedef struct
     {
     std::string msg_id;			//公告id
     std::string open_id;		//用户open_id
     std::string msg_url;		//公告跳转链接
     eMSG_NOTICETYPE msg_type;	//公告类型，eMSG_NOTICETYPE
     std::string msg_scene;		//公告展示的场景，管理端后台配置
     std::string start_time;		//公告有效期开始时间
     std::string end_time;		//公告有效期结束时间
     eMSG_CONTENTTYPE content_type;	//公告内容类型，eMSG_CONTENTTYPE
     //网页公告特殊字段
     std::string content_url;     //网页公告URL
     //图片公告特殊字段
     std::vector<PicInfo> picArray;    //图片数组
     //文本公告特殊字段
     std::string msg_title;		//公告标题
     std::string msg_content;	//公告内容
     }NoticeInfo;
     */
    extern char* GetNoticeData(char* scene);

    /**
    *  打开AMS营销活动中心
    *
    *  @param gameName 业务代码缩写，每个业务不同，各业务在接入AMS平台时由系统分配。
    *  @param actChannelId 活动渠道ID，由活动管理端后台分配，各业务登录[互娱AMS]管理端自行获取。
    *  @param zoneId 用户区服信息，大区 1手Q，2微信
    *  @param platformId 平台ID：iOS（0），安卓（1）
    *  @param partitionId 小区ID
    *  @param roleId 角色ID
    *  @param extra 额外扩展参数，会以&extra=xxx形式附加在AMS活动中心url上，注意做好Url Encode
    *  @param screenDir 屏幕方向
    *
    */
    extern void OpenAmsCenter(unsigned char *gameName,
                              unsigned char *actChannelId,
                              unsigned char *zoneId,
                              unsigned char *platformId,
                              unsigned char *partitionId,
                              unsigned char *roleId,
                              unsigned char *extra,
                              int screenDir);

    /**
     *  获取附近人的信息
     *  @return 回调到OnLocationNotify
     *  @return void
     *   通过游戏设置的全局回调的OnLocationNofity(RelationRet& rr)方法返回数据给游戏
     *     rr.platform表示当前的授权平台, 值类型为ePlatform, 可能值为ePlatform_QQ, ePlatform_Weixin
     *     rr.flag值表示返回状态, 可能值(eFlag枚举)如下：
     * 			eFlag_LbsNeedOpenLocationService: 需要引导用户开启定位服务
     *  		eFlag_LbsLocateFail: 定位失败, 可以重试
     *  		eFlag_Succ: 获取附近的人成功
     *  		eFlag_Error:  定位成功, 但是请求附近的人失败, 可重试
     *     rr.persons是一个Vector, 其中保存了附近玩家的信息
     */
    extern void GetNearbyPersonInfo();


    /**
     *  @return 回调到OnLocationNotify
     *  @return void
     *   通过游戏设置的全局回调的OnLocationNofity(RelationRet& rr)方法返回数据给游戏
     *     rr.platform表示当前的授权平台, 值类型为ePlatform, 可能值为ePlatform_QQ, ePlatform_Weixin
     *     rr.flag值表示返回状态, 可能值(eFlag枚举)如下：
     *  		eFlag_Succ: 清除成功
     *  		eFlag_Error: 清除失败
     */
    extern bool CleanLocation();


    /**
     *  获取当前玩家位置信息。
     *  @return 回调到OnLocationGotNotify
     *  @return void
     *   通过游戏设置的全局回调的OnLocationGotNotify(LocationRet& rr)方法返回数据给游戏
     *     rr.platform表示当前的授权平台, 值类型为ePlatform, 可能值为ePlatform_QQ, ePlatform_Weixin
     *     rr.flag值表示返回状态, 可能值(eFlag枚举)如下：
     *  		eFlag_Succ: 获取成功
     *  		eFlag_Error: 获取失败
     *     rr.longitude 玩家位置经度，double类型
     *     rr.latitude 玩家位置纬度，double类型
     */
    extern bool GetLocationInfo();
    
    /**
     *  获取当前玩家的国籍信息(根据接入的IP)。回调到OnLocationGotCountryFromIPNotify
     *  通过游戏设置的全局回调的OnLocationGotCountryFromIPNotify(GetCountryFromIPRet &fromIPRet)
     *  方法返回数据给游戏fromIPRet
     *     fromIPRet.country 客户端IP对应的ISO_3166-1标准中的国家或地区中文名称
     *     fromIPRet.isQueryByRequestHeader  false: 通过查询缓存中的映射关系获得结果；true：通过请求投中的信息获得结果
     *     fromIPRet.ret 值表示返回状态,ret为0才表示成功
     *     fromIPRet.msg 表示传递的消息ret不为0时错误原因
     */
    extern void GetCountryFromIP();

    /**
     *  获取当前玩家的国家及省市信息
     *  通过游戏设置的全局回调的OnLocationGetIPInfoNotify(GetIPInfoRet &ipInfoRet)
     *  方法返回数据给游戏ipInfoRet
     */
    extern void GetIPInfo();

    /**
     * 此接口会分享消息到微信游戏中心内的消息中心，这种消息主要包含两部分，消息体和附加按钮，消息体主要包含展示内容
     * 附加按钮主要定义了点击以后的跳转动作（拉起APP，拉起页面、拉起排行榜），消息类型和按钮类型可以任意组合
     * @param fopenid 好友的openid
     * @param title 游戏消息中心分享标题
     * @param content 游戏消息中心分享内容
     * @param pTypeInfo 消息体key-value形式的json字符串，这里可以传入四种消息类型，均为WXMessageTypeInfo的子类：
     * 		TypeInfoImage: 图片消息（下面的几种属性全都要填值）
     * 			std::string pictureUrl; // 图片缩略图
     * 			int height; // 图片高度
     * 			int width; // 图片宽度
     *      Json字符串：{"type":"TypeInfoImage","pictureUrl":"xxx","height":"xxx","width":"xxx"}
     *
     * 		TypeInfoVideo: 视频消息（下面的几种属性全都要填值）
     * 			std::string pictureUrl; // 视频缩略图
     * 			int height; // 视频高度
     * 			int width; // 视频宽度
     * 			std::string mediaUrl; // 视频链接
     *      Json字符串：{"type":"TypeInfoVideo","pictureUrl":"xxx","height":"xxx","width":"xxx","mediaUrl":"xxx"}
     *
     * 		TypeInfoLink: 链接消息（下面的几种属性全都要填值）
     * 			std::string pictureUrl; // 在消息中心的消息图标Url（图片消息中，此链接则为图片URL)
     * 			std::string targetUrl; // 链接消息的目标URL，点击消息拉起此链接
     *      Json字符串：{"type":"TypeInfoLink","pictureUrl":"xxx","targetUrl":"xxx"}
     *
     * 		TypeInfoText: 文本消息
     *      Json字符串：{"type":"TypeInfoText"}
     *
     *
     * @param pButtonInfo 按钮效果key-value形式的json字符串，这里可以传入三种按钮类型，均为WXMessageButton的子类：
     * 		ButtonApp: 拉起应用（下面的几种属性全都要填值）
     * 			std::string name; // 按钮名称
     * 			std::string messageExt; // 附加自定义信息，通过按钮拉起应用时会带回游戏
     *      Json字符串：{"type":"ButtonApp","name":"xxx","messageExt":"xxx"}
     *
     * 		ButtonWebview: 拉起web页面（下面的几种属性全都要填值）
     * 			std::string name; // 按钮名称
     * 			std::string webViewUrl; // 点击按钮后要跳转的页面
     *      Json字符串：{"type":"ButtonWebview","name":"xxx","webViewUrl":"xxx"}
     *
     * 		ButtonRankView: 拉起排行榜（下面的几种属性全都要填值）
     * 			std::string name; // 按钮名称
     * 			std::string title; // 排行榜名称
     * 			std::string rankViewButtonName; // 排行榜中按钮的名称
     * 			std::string messageExt; // 附加自定义信息，通过排行榜中按钮拉起应用时会带回游戏
     *      Json字符串：{"type":"ButtonRankView","name":"xxx","title":"xxx","rankViewButtonName":"xxx","messageExt":"xxx"}
     *
     * @param msdkExtInfo 游戏自定义透传字段，通过分享结果shareRet.extInfo返回给游戏
     *  @return 参数异常或未登陆
     */
    extern bool SendMessageToWechatGameCenter(
                                                   char* fOpenid,
                                                   char* title,
                                                   char* content,
                                                   char* pTypeInfo,
                                                   char* pButtonInfo,
                                                   char* msdkExtInfo
                                                   );


    /**
     * 把音乐消息分享给微信好友
     * @param scene 指定分享到朋友圈, 或者微信会话, 可能值和作用如下:
     *   WechatScene_Session: 分享到微信会话
     *   WechatScene_Timeline: 分享到微信朋友圈 (此种消息已经限制不能分享到朋友圈)
     * @param title 音乐消息的标题
     * @param desc	音乐消息的概要信息
     * @param musicUrl	音乐消息的目标URL
     * @param musicDataUrl	音乐消息的数据URL
     * @param mediaTagName 请根据实际情况填入下列值的一个, 此值会传到微信供统计用, 在分享返回时也会带回此值, 可以用于区分分享来源
     "MSG_INVITE";                   // 邀请
     "MSG_SHARE_MOMENT_HIGH_SCORE";    //分享本周最高到朋友圈
     "MSG_SHARE_MOMENT_BEST_SCORE";    //分享历史最高到朋友圈
     "MSG_SHARE_MOMENT_CROWN";         //分享金冠到朋友圈
     "MSG_SHARE_FRIEND_HIGH_SCORE";     //分享本周最高给好友
     "MSG_SHARE_FRIEND_BEST_SCORE";     //分享历史最高给好友
     "MSG_SHARE_FRIEND_CROWN";          //分享金冠给好友
     "MSG_friend_exceed"         // 超越炫耀
     "MSG_heart_send"            // 送心
     * @param imgData 原图文件数据
     * @param imgDataLen 原图文件数据长度(图片大小不z能超过10M)
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
     * @param messageAction scene为WechatScene_Timeline(分享到微信朋友圈)的情况下才起作用
     *   WECHAT_SNS_JUMP_SHOWRANK       跳排行,查看排行榜
     *   WECHAT_SNS_JUMP_URL            跳链接,查看详情
     *   WECHAT_SNS_JUMP_APP            跳APP,玩一把
     * @return void
     *   通过游戏设置的全局回调的OnShareNotify(ShareRet& shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
     */
    extern void SendToWeixinWithMusic(
                                           int scene,
                                           char* title,
                                           char* desc,
                                           char* musicUrl,
                                           char* musicDataUrl,
                                           char* mediaTagName,
                                           Byte* imgData,
                                           int imgDataLen,
                                           char* messageExt,
                                           char* messageAction
                                           );


    /**
     * 把音乐消息分享到手Q会话
     * @param scene eQQScene:
     * 			QQScene_QZone : 分享到空间
     * 			QQScene_Session：分享到会话
     * @param title 结构化消息的标题
     * @param desc 结构化消息的概要信息
     * @param musicUrl      点击消息后跳转的URL
     * @param musicDataUrl  音乐数据URL（例如http:// ***.mp3）
     * @param imgUrl 		分享消息缩略图URL，可以为本地路径(直接填路径，例如：/sdcard/ ***test.png)或者网络路径(例如：http:// ***.jpg)，本地路径要放在sdcard
     * @return void
     *   通过游戏设置的全局回调的OnShareNotify(ShareRet& shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
     */
    extern void SendToQQWithMusic(
                                       int scene,
                                       char* title,
                                       char* desc,
                                       char* musicUrl,
                                       char* musicDataUrl,
                                       char* imgUrl
                                       );

     /**
      * 手Q通用分享
      * @param serviceID 互联分配给业务的服务id
      * @param extraJson 可扩展参数，透传至QQ
      *
      */
    extern void SendToQQWithCommonShare(unsigned char *serviceID, unsigned char *extraJson);


     /**
     * 分享丰富的图片到空间
     * @param summary 分享的正文(无最低字数限制，最高1w字)
     * @param imageDatas 分享的图片的本地路径，可支持多张图片(<=9
     * 张图片为发表说说，>9 张图片为上传图片到相册)，只支持本地图片
     * 注意: 此功能只在手Q5.9.5及其以上版本支持
     */
    extern void SendToQQWithRichPhoto(unsigned char *summary, const char* imgParamsData , const unsigned char *extraScene,
                                 const unsigned char *messageExt);

    /**
     *  通过外部拉起的URL登陆。该接口用于异帐号场景发生时，用户选择使用外部拉起帐号时调用。
     *  登陆成功后通过onLoginNotify回调
     *
     *  @param flag 为YES时表示用户需要切换到外部帐号，此时该接口会使用上一次保存的异帐号登陆数据登陆。登陆成功后通过onLoginNotify回调；如果没有票据，或票据无效函数将会返回NO，不会发生onLoginNotify回调。
     *              为NO时表示用户继续使用原帐号，此时删除保存的异帐号数据，避免产生混淆。
     *
     *  @return 如果没有票据，或票据无效将会返回NO；其它情况返回YES
     */
    extern bool SwitchUser(bool flag);



    /**  分享内容到QQ
     *
     * 分享时调用
     * @param scene QQScene_QZone:空间，默认弹框 QQScene_Session:好友
     * @param title 标题
     * @param desc 内容
     * @param url  内容的跳转url，填游戏对应游戏中心详情页
     * @param imgData 图片文件数据
     * @param imgDataLen 数据长度
     * @return void
     */
    extern void SendToQQ(
                              int scene,
                              char* title,
                              char* desc,
                              char* url,
                              Byte* imgData,
                              int imgDataLen,
                              char* messageExt,
                              char* gameTag
                              );


    /*
     *
     * 分享时调用
     * @param QQScene_QZone:空间, 默认弹框 QQScene_Session:好友
     * @param imgData 图片文件数据
     * @param imgDataLen 数据长度
     */
    extern void SendToQQWithPhoto(
                                       int scene,
                                       Byte* imgData,
                                       int imgDataLen
                                       );
    /*
     *
     * 自定义参数分享图片到空间
     * @param QQScene_QZone:空间, 默认弹框 QQScene_Session:好友
     * @param imgData 图片文件数据
     * @param imgDataLen 数据长度
     * @param extraScene 自定义场景
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
     */
    extern void SendToQQWithPhotoWithParams(
                           int scene,
                           Byte* imgData,
                           int imgDataLen,
                           const unsigned char *extraScene,
                           const unsigned char *messageExt,
                           const unsigned char *gameTag
                           );
    
    //手q大图分享带自定义参数接口
    extern void WGSendToQQWithPhoto(const eQQScene &scene,
                             ImageParams &imageParams,
                             const unsigned char *extraScene,
                             const unsigned char *messageExt);
    /**
     * 获取pf, 用于支付, 和pfKey配对使用
     * @return pf
     */
    extern char* GetPf();


    /**
     *  获取paytoken有效期
     *
     *  @return paytoken有效期 seconds
     */
    extern int GetPaytokenValidTime();

    /**
     *
     *  @return 手Q版本号
     typedef enum QQVersion
     {
     kQQVersion3_0,
     kQQVersion4_0,      //支持sso登陆
     kQQVersion4_2_1,    //ios7兼容
     kQQVersion4_5,      //4.5版本，wpa会话
     kQQVersion4_6,      //4.6版本，sso登陆信令通道切换
     } QQVersion;
     */
    extern int GetIphoneQQVersion();

    /**
     * 游戏群绑定：游戏公会/联盟内，公会会长可通过点击“绑定”按钮，拉取会长自己创建的群，绑定某个群作为该公会的公会群
     * @param cUnionid 公会ID，opensdk限制只能填数字，字符可能会导致绑定失败
     * @param cUnion_name 公会名称
     * @param cZoneid 大区ID，opensdk限制只能填数字，字符可能会导致绑定失败
     * @param cSignature 游戏盟主身份验证签名，生成算法为openid_appid_appkey_公会id_区id 做md5.
     * 					   如果按照该方法仍然不能绑定成功，可RTX 咨询 OpenAPIHelper
     *
     */
    /*extern void BindQQGroup(
                                 char* cUnionid,
                                 char* cUnion_name,
                                 char* cZoneid,
                                 char* cSignature);
    */
    /**
     * 公会成员加QQ群接口
     * @param groupNum 必填参数 公会群号(非工会ID)，绑群成功后游戏(工会会长)可到 http://qun.qq.com 获取
     * @param groupKey 必填参数 需要添加的QQ群对应的key，绑群成功后游戏(工会会长)可到
     */
   // extern void JoinQQGroup(unsigned char* cQQGroupNum, unsigned char* cQQGroupKey);
    
    /**
     *
     *获取绑定手q群的群号
     * guildId：公会id（必填）
     * zoneId:区服ID（必填）
     * type："0"公会，"1"队伍，"2"赛事（必填）
     */
    extern void GetQQGroupCodeV2(
                                unsigned char*guildId,
                                unsigned char*guildName, 
                                unsigned char* leaderOpenId, 
                                unsigned char* leaderRoleId, 
                                unsigned char* leaderZoneId,
                                unsigned char* zoneId,
                                unsigned char* partition,
                                unsigned char* roleId,
                                unsigned char* roleName,
                                unsigned char* userZoneId,
                                unsigned char* userLabel,
                                unsigned char* nickName,
                                unsigned char* type,
                                unsigned char* areaId);
    
    /**
     *
     *查询绑定的公会信息
     *
     * groupId：手q群ID（原gc）
     * type:"0"公会，"1"队伍，"2"赛事
     */
    extern void QueryBindGuildV2(unsigned char *groupId, unsigned int type = 0);
    
    /**
     *
     *查询创建的qq群列表（与是否绑定无关）
     *
     */
    extern void GetQQGroupListV2();
    
    
    extern void RemindGuildLeaderV2(
                                unsigned char*guildId,
                                unsigned char*guildName, 
                                unsigned char* leaderOpenId, 
                                unsigned char* leaderRoleId, 
                                unsigned char* leaderZoneId,
                                unsigned char* zoneId,
                                unsigned char* partition,
                                unsigned char* roleId,
                                unsigned char* roleName,
                                unsigned char* userZoneId,
                                unsigned char* userLabel,
                                unsigned char* nickName,
                                unsigned char* type,
                                unsigned char* areaId);
    extern void BindExistQQGroupV2(
                                unsigned char*guildId,
                                unsigned char*guildName, 
                                unsigned char* leaderOpenId, 
                                unsigned char* leaderRoleId, 
                                unsigned char* leaderZoneId,
                                unsigned char* zoneId,
                                unsigned char* partition,
                                unsigned char* roleId,
                                unsigned char* roleName,
                                unsigned char* userZoneId,
                                unsigned char* userLabel,
                                unsigned char* nickName,
                                unsigned char* type,
                                unsigned char* areaId,
                                unsigned char* groupId,
                                unsigned char* groupName);

     
     
     
     
    
    /**
     * 游戏内加好友
     * @param cFopenid 需要添加好友的openid
     * @param cDesc 要添加好友的备注
     * @param cMessage 添加好友时发送的验证信息
     */
    extern void AddGameFriendToQQ(
                                       char* cFopenid,
                                       char* cDesc,
                                       char* cMessage);


    /**
     *  获取游客模式下的id
     */
    extern char* GetGuestID();


    /**
     *  刷新游客模式下的id
     */
    extern void ResetGuestID();

    extern void iOSCrashTest();
    
    /**
     * 设置游戏当前所处的场景开始点
     * @param cGameStatus 场景值，MSDK提供的场景值请参考GameStatus的定义，游戏也可以自定义场景参数
     */
    extern void StartGameStatus(unsigned char* cGameStatus);

    /**
     * 设置游戏当前所处的场景结束点
     * @param cGameStatus 场景值，MSDK提供的场景值请参考GameStatus的定义，游戏也可以自定义场景参数
     * @param succ 游戏对该场景执行结果的定义，例如成功、失败、异常等。
     * @param errorCode 游戏该场景异常的错误码，用户标识或者记录该场景失败具体是因为什么原因
     */
    extern void EndGameStatus(unsigned char* cGameStatus, int succ, int errorCode);

    /**
     * 添加本地推送，仅当游戏在后台时有效
     * @param LocalMessage 推送消息结构体，其中该接口
     *  必填参数为：
     *  fireDate;		//本地推送触发的时间
     *  alertBody;		//推送的内容
     *  badge;          //角标的数字
     *  可选参数为：
     *  alertAction;	//替换弹框的按钮文字内容（默认为"启动"）
     *  userInfo;       //自定义参数，可以用来标识推送和增加附加信息
     *  无用参数为：
     *  userInfoKey;	//本地推送在前台推送的标识Key
     *  userInfoValue;  //本地推送在前台推送的标识Key对应的值
     */
    extern long AddLocalNotification(char* localMessage);

    /**
     * 添加本地推送，游戏在前台有效
     * @param LocalMessage 推送消息结构体，其中该接口
     *  必填参数为：
     *  alertBody;		//推送的内容
     *  userInfoKey;	//本地推送在前台推送的标识Key
     *  userInfoValue;  //本地推送在前台推送的标识Key对应的值
     *  无用参数为：
     *  fireDate;		//本地推送触发的时间
     *  badge;          //角标的数字
     *  alertAction;	//替换弹框的按钮文字内容（默认为"启动"）
     *  userInfo;       //自定义参数，可以用来标识推送和增加附加信息
     */
    extern long AddLocalNotificationAtFront(char* localMessage);

    /**
     * 删除本地前台推送
     * @param LocalMessage 推送消息结构体，其中该接口
     *  必填参数为：
     *  userInfoKey;	//本地推送在前台推送的标识Key
     *  userInfoValue;  //本地推送在前台推送的标识Key对应的值
     *  无用参数为：
     *  fireDate;		//本地推送触发的时间
     *  alertBody;		//推送的内容
     *  badge;          //角标的数字
     *  alertAction;	//替换弹框的按钮文字内容（默认为"启动"）
     *  userInfo;       //自定义参数，可以用来标识推送和增加附加信息
     */
    extern void ClearLocalNotification(char* localMessage);

    /**
     * 清空所有未生效的本地推送
     */
    extern void ClearLocalNotifications();

    /**
     * 设置标签
     * @param tag 待设置的标签名称，不能为null或空。
     */
    extern void SetPushTag(char* tag);

    /**
     * 删除标签
     * @param tag 待删除的标签名称，不能为null或空
     */
    extern void DeletePushTag(char* tag);

    /**
     * 上报日志到bugly
     * @param level 日志级别，默认eBuglyLogLevel_I
     *  eBuglyLogLevel_S (0), //Silent
     *  eBuglyLogLevel_E (1), //Error
     *  eBuglyLogLevel_W (2), //Warning
     *  eBuglyLogLevel_I (3), //Info
     *  eBuglyLogLevel_D (4), //Debug
     *  eBuglyLogLevel_V (5); //Verbose
     *
     * @param log 日志内容
     *
     *  该日志会在crash发生时进行上报，上报日志最大30K
     */
    extern void BuglyLog(int level, unsigned char* log);
    
    /**
     * 关闭bugly上报 
     */
    extern void CloseCrashReport();

    /**
     * 实名认证
     * @param RealNameAuthInfo 实名认证信息结构体
     *  typedef struct
     *   {
     *       std::string name;           //姓名
     *       eIDType identityType;       //证件类型
     *       std::string identityNum;    //证件号码
     *       int province;               //省份
     *       std::string city;           //城市
     *   }RealNameAuthInfo;
     *  其中该接口必填参数为：
     *  name;           //姓名
     *  identityType;   //证件类型
     *  identityNum;    //证件号码
     *  选填参数为：
     *  province;		//省份
     *  city;           //城市
     */
    extern void RealNameAuth(char* authInfo);
    
    /**
     *　QQ游戏公会获取加群用的groupKey接口
     * @param groupOpenId 和游戏公会ID绑定的QQ群的groupOpenid，来自于公会会长绑群时获得
     *
     *
     */
    //extern void QueryQQGroupKey(unsigned char* groupOpenId);
    
    /**
     *　QQ群与工会绑定接口（新）(使用时需要申请权限)
     * @param guildId 工会ID
     * @param zoneId 大区ID
     * @param guildName 工会名称
     * @param roleId 角色id
     * @param partition 小区）区服id
     *
     */

    extern void CreateQQGroupV2(
                                unsigned char*guildId,
                                unsigned char*guildName, 
                                unsigned char* leaderOpenId, 
                                unsigned char* leaderRoleId, 
                                unsigned char* leaderZoneId,
                                unsigned char* zoneId,
                                unsigned char* partition,
                                unsigned char* roleId,
                                unsigned char* roleName,
                                unsigned char* userZoneId,
                                unsigned char* userLabel,
                                unsigned char* nickName,
                                unsigned char* type,
                                unsigned char* areaId);
    
    /**
     *　游戏内查询用户与群关系接口（新）(使用时需要申请权限)
     * @param guildId 	群id
     *
     */
    extern void QueryQQGroupInfoV2(unsigned char* groupId);
    
    /**
     *　游戏内与QQ加群接口（新）(使用时需要申请权限)
     * @param guildId 工会ID
     * @param zoneId 大区ID
     * @param groupId 	群号
     * @param roleId 角色id
     * @param partition 小区）区服id
     *
     */
    extern void JoinQQGroupV2(
                                unsigned char*guildId,
                                unsigned char*guildName, 
                                unsigned char* leaderOpenId, 
                                unsigned char* leaderRoleId, 
                                unsigned char* leaderZoneId,
                                unsigned char* zoneId,
                                unsigned char* partition,
                                unsigned char* roleId,
                                unsigned char* roleName,
                                unsigned char* userZoneId,
                                unsigned char* userLabel,
                                unsigned char* nickName,
                                unsigned char* type,
                                unsigned char* areaId,
                                unsigned char* groupId);
    
    /**
     *　游戏内与QQ解绑群接口（新）(使用时需要申请权限)
     * @param guildId 工会ID
     * @param zoneId 大区ID
     * @param guildName 	工会名称
     *
     */
    extern void UnbindQQGroupV2(
                                unsigned char*guildId,
                                unsigned char*guildName, 
                                unsigned char* leaderOpenId, 
                                unsigned char* leaderRoleId, 
                                unsigned char* leaderZoneId,
                                unsigned char* zoneId,
                                unsigned char* partition,
                                unsigned char* roleId,
                                unsigned char* roleName,
                                unsigned char* userZoneId,
                                unsigned char* userLabel,
                                unsigned char* nickName,
                                unsigned char* type,
                                unsigned char* areaId);
    
    /**
     *　微信解绑群
     * @param groupId 工会ID
     *
     */
    extern void UnbindWeiXinGroup(unsigned char* groupId);
    
    /**
     *　微信查询群状态
     * @param groupId 工会ID
     * @param opType  0：查询是否建群 1：查询是否加群
     *
     */
    extern void QueryWXGroupStatus(unsigned char* groupId,int opType);
    
    /**
     *
     *微信游戏圈传图功能
     *目前暂时对图片大小有限制，最好不能超过512K
     *@param imgData 图片数据
     *@param imgDataLen 图片数据长度
     *@param gameExtra 额外数据
     */
    extern void ShareToWXGameline(unsigned char* imgData,const int imgDataLen,unsigned char* gameExtra);
    /**
     *
     *上报unity版本数据
     *@param reportName 事件名称
     *@param reportData json字符串
     *
     */
    extern void ReportUnityData(char *reportName, char *reportData);

     /**
     *  输出msdk依赖平台版本号
     */
    extern void LogPlatformSDKVersion();  // log出msdk用到的各sdk版本号


    /**
     * 协议形式打开浏览器
     * jsonStr： json格式的协议，可以控制浏览器 全屏/非全屏 是否有返回按钮等
     */
    extern void OpenFullScreenWebViewWithJson(const char *jsonStr);

    /**
     * 上报prajna
     * serialNumber：上报prajna的串号
     */
    extern void ReportPrajna(const char *serialNumber);


     /**
     * 注册消息推送
     * @param launchOptions ,"-application:didFinishLaunchingWithOptions:"中的(NSDictionary *)launchOptions参数转换为json字符串
     *
     *
     */
    extern void RegisterAPNSPushNotification(unsigned char *launchOptions);
    
    /**
     * 成功注册推送并拿到deviceToken
     * @param deviceToken ，"-application:didRegisterForRemoteNotificationsWithDeviceToken:"中的(NSData *)deviceToken参数转换为字符串
     *
     *
     */
    extern void SuccessedRegisterdAPNSWithToken(unsigned char *deviceToken);
    
    /**
     * 注册推送失败
     *
     *
     *
     */
    extern void FailedRegisteredAPNS();
    
    /**
     * 清理推送角标数字
     *
     *
     *
     */
    extern void CleanBadgeNumber();
    
    /**
     * 接收到推送消息
     * @param userInfo ，"-application:didReceiveRemoteNotification:"中的(NSDictionary *)userInfo参数转换为json字符串
     *
     *
     */
    extern void ReceivedMSGFromAPNS(unsigned char *userInfo);

    /**
     * 在游戏窗口中打开嵌入式浏览器页面
     * url 要打开的页面链接
     * 返回值 1表示打开成功，0或者负数表示打开失败
     */
    extern int OpenEmbeddedWebView(const char* url);

    /**
     * 在游戏窗口中以数据的形式打开浏览器页面
     * charset 数据的编码格式（目前只支持UTF-8编码的数据）
     * data 传递给页面的数据
     * lens 传递的数据长度
     * 返回值 1表示打开成功，0或者负数表示打开失败
     */
    extern int OpenEmbeddedWebViews(const char* charset , const char* data , int lens);

    /**
     * 在游戏窗口中关闭浏览器页面
     * 返回值 1表示关闭成功，0或者负数表示关闭失败
     */
    extern int CloseEmbeddedWebView();

    /**
     * 在游戏中发送信息到页面js函数
     * params 要传递给js函数的函数名、参数信息
     */
    extern void CallToEmbeddedWebView(const char* params);

    /**
     * 设置webview的背景图片
     * data 图片数据
     * lens 图片数据的长度
     */
    extern bool SetEmbeddedWebViewBackground(const char* imageData , int lens);

    /**
     *
     * 微信OpenBusinessView分享功能
     * @param businessType 业务内容 必填
     * @param query 业务参数 选填
     * @param extInfo 额外信息(Json格式)
     * @param extData iOS专用，指iOS本地沙盒路径视频文件的地址
     */
    extern void SendToWXWithOpenBusinessView(const unsigned char *businessType,
                                        const unsigned char *query,
                                        const unsigned char *extInfo,
                                        const unsigned char *extData);
    /**
     * 平台拉起事件
     * @param urlStr 平台拉起的url
     */
    extern bool HandleOpenUrl(unsigned char *urlStr);
    
    /**
     * 平台拉起事件
     * @param urlStr 平台拉起的url
     */
    extern bool HandleOpenUniversalLink(unsigned char *urlStr);
    
    /*
        *启动QQ小程序
        * @param miniAppID 必填，小程序的AppID（注：必须在QQ互联平台中，将该小程序与分享的App绑定）
        * @param miniPath  非必填，小程序的展示路径，不填展示默认小程序首页
        * @param type      非必填，小程序的类型，默认正式版(0)，可选测试版(1)、开发版(2)
        */
    extern void LaunchQQMiniApp(const unsigned char *miniAppID, const unsigned char *miniPath, int type);

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
    extern void SendToQQWithMiniApp(
                               int scene,
                               unsigned char *title,
                               unsigned char *desc,
                               unsigned char *url,
                               Byte* imgData,
                               int imgDataLen,
                               unsigned char *miniProgramAppid,
                               unsigned char *miniProgramPath,
                               int type);
    
    /**
     * 分享视频到空间
     * @param summary 分享的正文(无最低字数限制，最高1w字)
     * @param videoParams 分享的视频路径，Android支持本地地址，iOS支持视频数据
     * @param extraScene 区分分享的场景，用于异化小尾巴展示和feeds点击行为，支持iOS
     * @param messageExt 游戏自定义字段，点击分享消息回到游戏时会透传回游戏，不需要的话可以填写空串，支持iOS
     * 注意: 此功能只在手Q5.9.5及其以上版本支持
     */
    extern void SendToQQWithVideo(unsigned char *summary, unsigned char *ios_videoData,
                                  int ios_videoDataLen, unsigned char *extraScene, unsigned char *messageExt);

    /**
     * 设置推送账号
     * @param account 推送账号
     */
    extern void SetPushAccount(char* account);
    
    /**
     * 删除推送账号
     * @param account 推送账号
     */
    extern void DeletePushAccount(char* account);
    
    /**
     * 停止推送服务
     */
    extern void UnregisterPush();

    /**
     * 设置是否允许收集敏感信息
    */
    extern void SetCouldCollectSensitiveInfo(bool couldCollect);
    
    /**
     * 以json形式设置敏感信息字段至各个组件SDK，目前支持{\"AndroidID\":\"xxx\", \"WiFiMacAddress\":\"xxx\", \"Imei\":\"xxx\", \"Model\":\"xxx\", \"Oaid\":\"xxx\", \"Imsi\":\"xxx\", \"Cid\":\"xxx\"}
    */
    extern void SetSensitiveInfo(const char *jsonInfo);

    /**
     * 以json形式设置敏感信息单字段开关，优先级小于总开关，目前支持设置AndroidID、Apn、Imei，QImei, QImei36
     * 参数示例{"AndroidID":true,"Apn":true,"Imei":false,"QImei":true,"QImei36":true}
     */
    extern void SetCollectSensitiveInfo(const char *jsonInfo);

    /**
     *  @param openUrl 要打开的url
     *  @param screenDir 内置浏览器支持的屏幕方向,0横竖屏,1竖屏,2横屏
     *  @param fullScreen 是否全屏
     *  @param algorithm 加密算法标识
     */
    extern void OpenGameDataAuthCenter(unsigned char *openUrl, int screenDir, bool fullScreen, const char *algorithm);

    /**
     * @brief 设置url后带着协议所需参数拉起销号页面
     * 
     * @param screenDir 内置浏览器支持的屏幕方向,0横竖屏,1竖屏,2横屏
     * @param isFullScreen 是否全屏
     * @param algorithm 加密算法标识
     */
    extern void OpenDeleteAccountUrl(int screenDir, bool isFullScreen, const char *algorithm);

#ifdef __cplusplus
}
#endif

@interface iOSConnector : NSObject

+ (NSString *)loginRetToResponseJsonString:(LoginRet&)loginRet method:(NSString*)method;
+ (NSString *)checkULRetToResponseJsonString:(CheckULRet&)checkULRet method:(NSString*)method;
+ (NSString *)shareRetToResponseJsonString:(ShareRet&)shareRet method:(NSString*)method;
+ (NSString *)wakeupRetToResponseJsonString:(WakeupRet&)wakeupRet method:(NSString*)method;
+ (NSString *)relationRetToResponseJsonString:(RelationRet&)relationRet method:(NSString*)method;
+ (NSString *)locationRetToResponseJsonString:(LocationRet&)locationRet method:(NSString*)method;
+ (NSString *)feedbackToResponseJsonString:(int)flag desc:(std::string)desc method:(NSString*)method;
+ (NSString *)authRetToResponseJsonString:(RealNameAuthRet&)authRet method:(NSString*)method;
+ (NSString *)adRetToResponseJsonString:(ADRet&)adRet method:(NSString*)method;
+ (NSString *)groupRetToResponseJsonString:(GroupRet&)groupRet method:(NSString*)method;
+ (NSString *)webviewRetToResponseJsonString:(WebviewRet &)webviewRet method:(NSString*)method;
+ (NSString *)jsCallbackToResponseJsonString:(std::string)params method:(NSString*)method;
+ (NSString *)webProgressChangedToResponseJsonString:(std::string)url desc:(float)progress method:(NSString*)method;
+ (NSString *)getCountryFromIPRetToResponseJsonString:(GetCountryFromIPRet&)fromIPRet method:(NSString*)method;
+ (NSString *)getIPInfoRetToResponseJsonString:(GetIPInfoRet&)ipInfoRet method:(NSString*)method;
+ (NSString *)receiveNotificationToResponseJsonString:(std::string)userInfo method:(NSString*)method;

@end
