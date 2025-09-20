#ifndef __WGPLATFORM_H__
#define __WGPLATFORM_H__

#include <string>
#include "MSDKEnums.h"
#include "MSDKStructs.h"
#include "msdk_convert_type.h"
#include "WGGroupObserver.h"
#include "WGPlatformObserver.h"
#include "WGRealNameAuthObserver.h"
#include "WGWebviewObserver.h"
#include "WGEmbeddedWebViewObserver.h"

#ifdef ANDROID
#include <jni.h>
#include "WGSaveUpdateObserver.h"
#ifndef MSDK_INNER_COMPILE
#include "WGObserverProxy.h"
#endif
#endif

#ifdef _WIN32
#define WG_DEPRECATED(_version)
#else
#define WG_DEPRECATED(_version) __attribute__((deprecated))
#endif


/**
 * WeGame接口函数
 *
 * 该类封装了WeGame的外部接口
 */
class MSDKEXPORT WGPlatform{
public:
    static WGPlatform *GetInstance();
    WGPlatformObserver *GetObserver() const;
    WGGroupObserver *GetGroupObserver() const;
    WGRealNameAuthObserver *GetRealNameAuthObserver() const;
    WGWebviewObserver *GetWebviewObserver() const;
    void WGSetEmbeddedWebViewObserver(WGEmbeddedWebViewObserver *pEmWebObserver);
    
#ifdef ANDROID

public:
    void init(JavaVM *pVM);
    WGSaveUpdateObserver *GetSaveUpdateObserver() const;

    /**
     *   OnLoginNotify: 登陆回调
     *   OnShareNotify: 分享回调
     *   OnWakeupNotify: 被唤起回调
     *   OnRelationNotify: 关系链查询回调
     * @param pObserver 游戏传入的全局回调对象
     */
    void WGSetObserver(WGPlatformObserver *pObserver)
    {
#ifndef MSDK_INNER_COMPILE
        if (pObserver->proxy_ == NULL)
        {
            pObserver->proxy_ = new WGObserverProxy();
        }
#endif
        this->_WGSetObserver(pObserver);
    }
    /**
     * 实名认证回调设置
     */
    void WGSetRealNameAuthObserver(WGRealNameAuthObserver *pRealNameAuthObserver)
    {
#ifndef MSDK_INNER_COMPILE
        if (pRealNameAuthObserver->proxy_ == NULL)
        {
            pRealNameAuthObserver->proxy_ = new WGObserverProxy();
        }
#endif
        this->_WGSetRealNameAuthObserver(pRealNameAuthObserver);
    }
    /**
     * 浏览器回调设置
     */
    void WGSetWebviewObserver(WGWebviewObserver *pWebviewObserver)
    {
#ifndef MSDK_INNER_COMPILE
        if (pWebviewObserver->proxy_ == NULL)
        {
            pWebviewObserver->proxy_ = new WGObserverProxy();
        }
#endif
        this->_WGSetWebviewObserver(pWebviewObserver);
    }

    /**
     * 加群加好友回调
     */
    void WGSetGroupObserver(WGGroupObserver *pGroupObserver)
    {
#ifndef MSDK_INNER_COMPILE
        if (pGroupObserver->proxy_ == NULL)
        {
            pGroupObserver->proxy_ = new WGObserverProxy();
        }
#endif
        this->_WGSetGroupObserver(pGroupObserver);
    }
    /**
     * @param saveUpdateObserver 省流量更新全局回调,
     * 和更新相关的所有回调都会通过此对象回调
     */
    void WGSetSaveUpdateObserver(WGSaveUpdateObserver *saveUpdateObserver)
    {
#ifndef MSDK_INNER_COMPILE
        if (saveUpdateObserver->proxy_ == NULL)
        {
            saveUpdateObserver->proxy_ = new WGObserverProxy();
        }
#endif
        this->_WGSetSaveUpdateObserver(saveUpdateObserver);
    }

    /**
    * @param loginRet 返回的记录
    * @return 返回值为平台id, 类型为ePlatform, 返回ePlatform_None表示没有登陆记录
    *   loginRet.platform(类型为ePlatform)表示平台id, 可能值为ePlatform_QQ,
    ePlatform_Weixin, ePlatform_None.
    *   loginRet.flag(类型为eFlag)表示当前本地票据的状态, 可能值及说明如下:
    *     eFlag_Succ: 授权票据有效
    *     eFlag_QQ_AccessTokenExpired: 手Q accessToken已经过期, 显示授权界面,
    引导用户重新授权
    *     eFlag_WX_AccessTokenExpired:
    微信accessToken票据过期，需要调用WGRefreshWXToken刷新
    *     eFlag_WX_RefreshTokenExpired: 微信refreshToken, 显示授权界面,
    引导用户重新授权
    *   ret.token是一个Vector<TokenRet>, 其中存放的TokenRet有type和value,
    通过遍历Vector判断type来读取需要的票据. type类型定义如下:
    *       eToken_QQ_Access = 1,
           eToken_QQ_Pay,
           eToken_WX_Access,
           eToken_WX_Refresh,
    *
    * 注意: 游戏通过此接口获取到的票据以后必须传到游戏Server,
    通过游戏Server调用MSDK后端验证票据接口验证票据有效以后才能让用户进入游戏.
    */
    int WGGetLoginRecord(LoginRet &loginRet)
    {
        ConvertLoginRet msdkloginret = MsdkCovertTypeUtils::LoginRetToMSDKLoginRet(loginRet);
        int ret = this->_WGGetLoginRecord(msdkloginret);
        loginRet = MsdkCovertTypeUtils::MSDKLoginRetToLoginRet(msdkloginret);

        return ret;
    }
    
    /**
     * 游戏群绑定：游戏公会/联盟内，公会会长可通过点击“绑定”按钮，拉取会长自己创建的群，绑定某个群作为该公会的公会群
     * 绑定结果结果会通过WGGroupObserver的OnBindGroupNotify回调给游戏。
     * 由于目前手Q
     * SDK尚不支持绑群的回调，因此从MSDK2.7.0a开始无论绑定是否成功，MSDK都会给游戏一个成功的回调。
     * 游戏收到回调以后需要调用查询接口确认绑定是否成功
     * @param unionid 公会ID
     * @param union_name 公会名称
     * @param zoneid 区域ID
     * @param signature      游戏盟主身份验证签名，从游戏后台获取
     */
//    void WGBindQQGroup(
//                       unsigned char *unionid, unsigned char *union_name, unsigned char *zoneid, unsigned char *signature);
//    /**
//     * 通过游戏加群
//     * @param qqGroupKey 需要添加群的ID
//     */
//    void WGJoinQQGroup(unsigned char *qqGroupKey);
//    /**
//     * 查询公会绑定的群的信息，查询结果会通过WGGroupObserver的OnQueryGroupInfoNotify回调给游戏
//     * @param unionid 公会ID
//     * @param zoneid 大区ID
//     */
//    void WGQueryQQGroupInfo(unsigned char *cUnionid, unsigned char *cZoneid);
//    /**
//     * 解绑公会当前绑定的QQ群，结果会通过WGGroupObserver的OnUnbindGroupNotify回调给游戏
//     * @param cGroupOpenid 公会绑定的群的群openid
//     * @param cUnionid 公会ID
//     */
//
//    void WGUnbindQQGroup(unsigned char *cGroupOpenid, unsigned char *cUnionid);

    /**
     * 通过游戏加好友
     * @param fopenid 要添加好友的openid
     * @param desc 要添加好友的备注
     * @param message 验证信息
     */
    void WGAddGameFriendToQQ(unsigned char *fopenid, unsigned char *desc, unsigned char *message);
    /**
      * 将微信卡券插入到微信卡包
      * @param cardId 卡券ID
      * @param timestamp 计算签名的时间戳
      * @param sign 签名
      */
    void WGAddCardToWXCardPackage(unsigned char *cardId, unsigned char *timestamp, unsigned char *sign);

    /**
    * @param scene 指定分享到朋友圈, 或者微信会话, 可能值和作用如下:
    *   WechatScene_Session: 分享到微信会话
    *   WechatScene_Timeline: 分享到微信朋友圈
    * @param mediaTagName (必填)请根据实际情况填入下列值的一个,
      此值会传到微信供统计用, 在分享返回时也会带回此值, 可以用于区分分享来源
     "MSG_INVITE";                   // 邀请
     "MSG_SHARE_MOMENT_HIGH_SCORE";    //分享本周最高到朋友圈
     "MSG_SHARE_MOMENT_BEST_SCORE";    //分享历史最高到朋友圈
     "MSG_SHARE_MOMENT_CROWN";         //分享金冠到朋友圈
     "MSG_SHARE_FRIEND_HIGH_SCORE";     //分享本周最高给好友
     "MSG_SHARE_FRIEND_BEST_SCORE";     //分享历史最高给好友
     "MSG_SHARE_FRIEND_CROWN";          //分享金冠给好友
     "MSG_friend_exceed"         // 超越炫耀
     "MSG_heart_send"            // 送心
     * @param imgPath 本地图片的路径(建议图片小于3MB)
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过
        OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
     * @param messageAction scene为1(分享到微信朋友圈)的情况下才起作用
     *   WECHAT_SNS_JUMP_SHOWRANK       跳排行
     *   WECHAT_SNS_JUMP_URL            跳链接
     *   WECHAT_SNS_JUMP_APP           跳APP
     * @return void
     *   通过游戏设置的全局回调的OnShareNotify(ShareRet&
         shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
    */

    void WGSendToWeixinWithPhotoPath(const eWechatScene &scene, unsigned char *mediaTagName, unsigned char *imgPath,
                                     unsigned char *messageExt, unsigned char *messageAction);


    /**
     * @param scene 标识发送手Q会话或者Qzone
     *         eQQScene.QQScene_QZone: 分享到空间(4.5以上版本支持)
     *         eQQScene.QQScene_Session: 分享到手Q会话
     * @param title 结构化消息的标题
     * @param desc 结构化消息的概要信息
     * @param url  内容的跳转url，填游戏对应游戏中心详情页,
     * 可以在此链接中添加参数名为gamedata的自定义字段.
     *     例如http://example.com/detail.html?gamedata=game_custom_data,
     * 此链接中带的自定义数据"game_custom_data"会在游戏被平台拉起的时候带回,
     *     游戏可以在onWakeup回调的WakeupRet.extInfo中接收此数据
     * @param imgUrl
     * 分享消息缩略图URL，可以为本地路径或者网络路径，本地路径要放在sdcard
     * @param imgUrlLen 分享消息说略图URL长度
     * @param messageExt：游戏自定义字段，点击分享消息回到游戏时会透传回游戏，不需要的话可以填写空串
     * @param gameTag 此值会传到QQ供统计使用
     * @return void
     *   通过游戏设置的全局回调的OnShareNotify(ShareRet&
     * shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
     *
     *     @return void
     * 通过游戏设置的全局回调的OnShareNotify(ShareRet&
     * shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
     * 注意:
     *     如果分享的是本地图片，则需要放置到sdcard分区,
     * 或其他外部程序有权限访问之处
     *     由于手Q客户端部分的版本返回的回调是有问题的,
     * 故建议不要依赖此回调做其他逻辑。
     */
    void WGSendToQQ(const eQQScene &scene,
                    unsigned char *title,
                    unsigned char *desc,
                    unsigned char *url,
                    unsigned char *imgUrl,
                    const int &imgUrlLen,
                    unsigned char *messageExt = (unsigned char *)"",
                    unsigned char *gameTag = (unsigned char *)"");
    /**
     * 分享纯图到手Q或者QQ空间
     * @param scene 标识发送手Q会话或者Qzone
     *         eQQScene.QQScene_QZone: 分享到空间
     *         eQQScene.QQScene_Session: 分享到手Q会话
     * @param imgFilePath 需要分享图片的本地文件路径, 图片需放在sd卡
     *     每次分享的图片路径不能相同，相同会导致图片显示有问题，游戏需要自行保证每次分享图片的地址不相同
     * @return void
     *   通过游戏设置的全局回调的OnShareNotify(ShareRet&
     * shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
     *   注意:
     *    1. 由于手Q客户端部分的版本返回的回调是有问题的,
     * 故不要依赖此回调做其他逻辑. (当前flag全都返回均为eFlag_Succ)
     *    2. 图片路径需要使用sdcard的路径, 不能分享保存在应用目录下的图片
     *
     */
    void WGSendToQQWithPhoto(const eQQScene &scene, unsigned char *imgFilePath);


    /**
     *     地址形如: http://180.153.81.37/monitor/monitor.jsp, IP如:
     * 119.147.19.241:80 (需要端口号)
     * @param 需要测速的地址(IP)列表
     */
    void WGTestSpeed(std::vector<std::string> &addrList)
    {
        MSDKVector<MSDKString> tmp_vc = MsdkCovertTypeUtils::VStringToVMSDKString(addrList);
        this->_WGTestSpeed(tmp_vc);
    }

    /**
     * @return 接口的支持情况
     * @param
     * eApiName_WGSendToQQWithPhoto = 0,
     * eApiName_WGJoinQQGroup = 1,
     * eApiName_WGAddGameFriendToQQ = 2,
     * eApiName_WGBindQQGroup = 3
     */
    bool WGCheckApiSupport(eApiName);
    /**
     * 如果手机上没有安装应用宝则此接口会自动下载应用宝,
     * 并通过OnDownloadYYBProgressChanged和OnDownloadYYBStateChanged两个接口分别回调
     * 如果手机上已经安装应用宝则此接口会根据参数选择是否拉起应用宝
     *         下载进度和状态变化会通过OnDownloadAppProgressChanged和OnDownloadAppStateChanged回调给游戏
     * @para  isUseYYB:是否拉起应用宝更新游戏，如果选否，会直接在游戏内完成更新
     */
    void WGStartSaveUpdate(bool isUseYYB);
    /**
     * @return void
     *      查询结果回调到由WGSetSaveUpdateObserver接口设置的回调对象的OnCheckNeedUpdateInfo方法
     */
    void WGCheckNeedUpdate();
    /**
     * @return 返回值为 TMYYBInstallState(WGPublicDefine.h中定义)
     */
    int WGCheckYYBInstalled();
    /**
        *  信鸽本地推送
        *
        *
        */
    long WGAddLocalNotification(LocalMessage &msg);

    /**
     * @param name 事件名称
     * @param eventList 事件内容, 一个key-value形式的vector
     * @param isRealTime 是否实时上报
     *
     */
    void WGReportEvent(unsigned char *name, std::vector<KVPair> &eventList, bool isRealTime)
    {
        MSDKVector<MSDKKVPair> tmp_vc = MsdkCovertTypeUtils::VKVPairToMSDKVKVPair(eventList);
        this->_WGReportEvent(name, tmp_vc, isRealTime);
    }
    
    /**
     * 上报C#、JS、Lua等异常
     * exception_type：异常类型
     * excetipon_name：异常名称
     * excetipon_name：异常信息
     * exception_stack：异常堆栈
     * ext_info：附加信息
     */
    void WGReportException(eExceptionType exception_type,
                           unsigned char *exception_name,
                           unsigned char *exception_msg,
                           unsigned char *exception_stack,
                           std::vector<KVPair> &ext_info)
    {
        MSDKVector<MSDKKVPair> tmp_vc = MsdkCovertTypeUtils::VKVPairToMSDKVKVPair(ext_info);
        this->_WGReportException(exception_type, exception_name, exception_msg, exception_stack, tmp_vc);
    }

    /**
     * 区别于小程序，微信提供基于原生的游戏直播/围观功能
     * @param scene 朋友圈或者好友列表
     * @param title 标题
     * @param desc 描述
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过 OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
     * @param extInfo 直播/围观功能的专用参数
     */
    void WGSendToWXGameLive(const eWechatScene &scene, unsigned char *title, unsigned char *desc, unsigned char *messageExt, std::vector<KVPair> &extInfo) {
        MSDKVector<MSDKKVPair> tmp_extInfo = MsdkCovertTypeUtils::VKVPairToMSDKVKVPair(extInfo);
        this->_WGSendToWXGameLive(scene, title, desc, messageExt, tmp_extInfo);
    }


private:

    const std::vector<NoticeInfo> InnerWGGetNoticeData(unsigned char *scene)
    {
        MSDKVector<MSDKNoticeInfo> vc = this->_InnerWGGetNoticeData(scene);
        this->_LOG_InnerWGGetNoticeData(vc);  // 打印数据
        // 返回vector数据
        return MsdkCovertTypeUtils::MSDKVNoticeInfoToVNoticeInfo(vc);
    }

    // 代理的设置observer
    void _WGSetObserver(WGPlatformObserver *pObserver);
    void _WGSetRealNameAuthObserver(WGRealNameAuthObserver *pRealNameAuthObserver);
    void _WGSetWebviewObserver(WGWebviewObserver *pWebviewObserver);
    void _WGSetGroupObserver(WGGroupObserver *pGroupObserver);
    void _WGSetSaveUpdateObserver(WGSaveUpdateObserver *saveUpdateObserver);

    int _WGGetLoginRecord(ConvertLoginRet &loginRet);
#endif

#ifdef __APPLE__

    public:
    ePlatform m_currentPlatformType;

    void WGSetObserver(WGPlatformObserver *pObserver);
    void WGSetRealNameAuthObserver(WGRealNameAuthObserver *pRealNameAuthObserver);
    void WGSetWebviewObserver(WGWebviewObserver *pWebviewObserver);
    void WGSetGroupObserver(WGGroupObserver *pGroupObserver);
        

    /**
     * @param loginRet 返回的记录
     * @return 返回值为平台id, 类型为ePlatform, 返回ePlatform_None表示没有登陆记录
     *   loginRet.platform(类型为ePlatform)表示平台id, 可能值为ePlatform_QQ,
     ePlatform_Weixin, ePlatform_None.
     *   loginRet.flag(类型为eFlag)表示当前本地票据的状态, 可能值及说明如下:
     *     eFlag_Succ: 授权票据有效
     *     eFlag_QQ_AccessTokenExpired: 手Q accessToken已经过期, 显示授权界面,
     引导用户重新授权
     *     eFlag_WX_AccessTokenExpired:
     微信accessToken票据过期，需要调用WGRefreshWXToken刷新
     *     eFlag_WX_RefreshTokenExpired: 微信refreshToken, 显示授权界面,
     引导用户重新授权
     *   ret.token是一个Vector<TokenRet>, 其中存放的TokenRet有type和value,
     通过遍历Vector判断type来读取需要的票据. type类型定义如下:
     *       eToken_QQ_Access = 1,
     eToken_QQ_Pay,
     eToken_WX_Access,
     eToken_WX_Refresh,
     *
     * 注意: 游戏通过此接口获取到的票据以后必须传到游戏Server,
     通过游戏Server调用MSDK后端验证票据接口验证票据有效以后才能让用户进入游戏.
     */
    int WGGetLoginRecord(LoginRet& loginRet);

    /**
     * 游戏群绑定：游戏公会/联盟内，公会会长可通过点击“绑定”按钮，拉取会长自己创建的群，绑定某个群作为该公会的公会群
     * @param cUnionid 公会ID，opensdk限制只能填数字，字符可能会导致绑定失败
     * @param cUnion_name 公会名称
     * @param cZoneid 大区ID，opensdk限制只能填数字，字符可能会导致绑定失败
     * @param cSignature
     * 游戏盟主身份验证签名，生成算法为openid_appid_appkey_公会id_区id 做md5.
     *                        如果按照该方法仍然不能绑定成功，可RTX 咨询
     * OpenAPIHelper
     *
     */
//    void WGBindQQGroup(
//                       unsigned char *cUnionid, unsigned char *cUnion_name, unsigned char *cZoneid, unsigned char *cSignature);

    /**
     * 游戏内加群,公会成功绑定qq群后，公会成员可通过点击“加群”按钮，加入该公会群
     * @param cQQGroupNum 公会群号，绑群成功后游戏(工会会长)可到 http://qun.qq.com
     * 获取
     * @param cQQGroupKey 需要添加的QQ群对应的key，绑群成功后游戏(工会会长)可到
     * http://qun.qq.com 获取
     */
//    void WGJoinQQGroup(unsigned char *cQQGroupNum, unsigned char *cQQGroupKey);

    /**
     * 游戏内加好友
     * @param cFopenid 需要添加好友的openid
     * @param cDesc 要添加好友的备注
     * @param cMessage 添加好友时发送的验证信息
     */
    void WGAddGameFriendToQQ(unsigned char *cFopenid, unsigned char *cDesc, unsigned char *cMessage);

    /**
     * 获取手Q版本号
     */
    int WGGetIphoneQQVersion() WG_DEPRECATED(3.0.0);

    /**
      *  获取游客模式下的id
      *
      *
      */
    std::string WGGetGuestID()
    {
        return _WGGetGuestID().c_str();
    }

    /**
     *  刷新游客模式下的id
     *
     *
     */
    void WGResetGuestID();

    /**
     * 添加本地推送，仅当游戏在后台时有效
     * @param localMessage 推送消息结构体，其中该接口
     *  必填参数为：
     *  fireDate;        //本地推送触发的时间
     *  alertBody;        //推送的内容
     *  badge;          //角标的数字
     *  可选参数为：
     *  alertAction;    //替换弹框的按钮文字内容（默认为"启动"）
     *  userInfo;       //自定义参数，可以用来标识推送和增加附加信息
     *  无用参数为：
     *  userInfoKey;    //本地推送在前台推送的标识Key
     *  userInfoValue;  //本地推送在前台推送的标识Key对应的值
     */
    long WGAddLocalNotification(LocalMessage &localMessage);

    /**
     * 添加本地推送，游戏在前台有效
     * @param localMessage 推送消息结构体，其中该接口
     *  必填参数为：
     *  alertBody;        //推送的内容
     *  userInfoKey;    //本地推送在前台推送的标识Key
     *  userInfoValue;  //本地推送在前台推送的标识Key对应的值
     *  无用参数为：
     *  fireDate;        //本地推送触发的时间
     *  badge;          //角标的数字
     *  alertAction;    //替换弹框的按钮文字内容（默认为"启动"）
     *  userInfo;       //自定义参数，可以用来标识推送和增加附加信息
     */
    long WGAddLocalNotificationAtFront(LocalMessage &localMessage);

    /**
     * 删除本地前台推送
     * @param localMessage 推送消息结构体，其中该接口
     *  必填参数为：
     *  userInfoKey;    //本地推送在前台推送的标识Key
     *  userInfoValue;  //本地推送在前台推送的标识Key对应的值
     *  无用参数为：
     *  fireDate;        //本地推送触发的时间
     *  alertBody;        //推送的内容
     *  badge;          //角标的数字
     *  alertAction;    //替换弹框的按钮文字内容（默认为"启动"）
     *  userInfo;       //自定义参数，可以用来标识推送和增加附加信息
     */
    void WGClearLocalNotification(LocalMessage &localMessage);


    /**
     * 平台拉起事件
     * @param urlStr 平台拉起的url
     */
    bool WGHandleOpenUrl(unsigned char *urlStr);
    
    /**
     * 平台拉起事件
     * @param urlStr 平台拉起的url
     */
    bool WGHandleOpenUniversalLink(unsigned char *urlStr);

    /**  分享内容到QQ
     *
     * 分享时调用
     * @param scene QQScene_QZone:空间，默认弹框 QQScene_Session:好友
     * @param title 标题
     * @param desc 内容
     * @param url  内容的跳转url，填游戏对应游戏中心详情页
     * @param imgData 图片文件数据
     * @param imgDataLen 数据长度
     * @param messageExt：游戏自定义字段，点击分享消息回到游戏时会透传回游戏，不需要的话可以填写空串
     * @param gameTag 此值会传到QQ供统计使用
     */
    void WGSendToQQ(const eQQScene &scene,
                    unsigned char *title,
                    unsigned char *desc,
                    unsigned char *url,
                    unsigned char *imgData,
                    const int &imgDataLen,
                    unsigned char *messageExt = (unsigned char *)"",
                    unsigned char *gameTag = (unsigned char *)"");

    /*
     * 分享时调用
     * @param QQScene_QZone:空间, 默认弹框 QQScene_Session:好友
     * @param imgData 图片文件数据
     * @param imgDataLen 数据长度
     */
    void WGSendToQQWithPhoto(const eQQScene &scene, unsigned char *imgData, const int &imgDataLen);


	 /**
     * @param name 事件名称
     * @param eventList 事件内容, 一个key-value形式的vector
     * @param isRealTime 是否实时上报
     *
     */
    void WGReportEvent(unsigned char *name, std::vector<KVPair> &eventList, bool isRealTime);
    
    /**
     * 上报C#、JS、Lua等异常
     * exception_type：异常类型
     * exception_name：异常名称
     * excetipon_name：异常信息
     * exception_stack：异常堆栈
     * ext_info：附加信息
     */
    void WGReportException(eExceptionType exception_type,
                           unsigned char *exception_name,
                           unsigned char *exception_msg,
                           unsigned char *exception_stack,
                           std::vector<KVPair> &ext_info);
    /**
       * 区别于小程序，微信提供基于原生的游戏直播/围观功能
       * @param scene 朋友圈或者好友列表
       * @param title 标题
       * @param desc 描述
       * @param messageExt 好友的openid
       * @param extInfo 好友的openid
       */
       void WGSendToWXGameLive(const eWechatScene &scene, unsigned char *title, unsigned char *desc, unsigned char *messageExt, std::vector<KVPair> &extInfo);
    /**
     * 注册消息推送
     * @param launchOptions ,"-application:didFinishLaunchingWithOptions:"中的(NSDictionary *)launchOptions参数转换为json字符串
     *
     *
     */
    void WGRegisterAPNSPushNotification(unsigned char *launchOptions);
    
    /**
     * 成功注册推送并拿到deviceToken
     * @param deviceToken ，"-application:didRegisterForRemoteNotificationsWithDeviceToken:"中的(NSData *)deviceToken参数转换为字符串
     *
     *
     */
    void WGSuccessedRegisterdAPNSWithToken(unsigned char *deviceToken);
    
    /**
     * 注册推送失败
     *
     *
     *
     */
    void WGFailedRegisteredAPNS();
    
    /**
     * 清理推送角标数字
     *
     *
     *
     */
    void WGCleanBadgeNumber();
    
    /**
     * 接收到推送消息
     * @param userInfo ，"-application:didReceiveRemoteNotification:"中的(NSDictionary *)userInfo参数转换为json字符串
     *
     *
     */
    void WGReceivedMSGFromAPNS(unsigned char *userInfo);
  
private:
	const std::vector<NoticeInfo> InnerWGGetNoticeData(unsigned char *scene);

#endif


public:
    /**
  * 获取paytoken有效期
  *
  * @return paytoken有效期 seconds
  */
    int WGGetPaytokenValidTime();
    /**
     * @param platform 游戏传入的平台类型, 可能值为: ePlatform_QQ,
     * ePlatform_Weixin
     * 通过游戏设置的全局回调的OnLoginNotify(LoginRet&
     * loginRet)方法返回数据给游戏
     *     loginRet.platform表示当前的授权平台, 值类型为ePlatform,
     * 可能值为ePlatform_QQ, ePlatform_Weixin
     *     loginRet.flag值表示返回状态, 可能值(eFlag枚举)如下：
     *       eFlag_Succ: 返回成功,
     * 游戏接收到此flag以后直接读取LoginRet结构体中的票据进行游戏授权流程.
     *       eFlag_QQ_NoAcessToken: 手Q授权失败,
     * 游戏接收到此flag以后引导用户去重新授权(重试)即可.
     *       eFlag_QQ_UserCancel: 用户在授权过程中
     *       eFlag_QQ_LoginFail: 手Q授权失败,
     * 游戏接收到此flag以后引导用户去重新授权(重试)即可.
     *       eFlag_QQ_NetworkErr: 手Q授权过程中出现网络错误,
     * 游戏接收到此flag以后引导用户去重新授权(重试)即可.
     *     loginRet.token是一个Vector<TokenRet>, 其中存放的TokenRet有type和value,
     * 通过遍历Vector判断type来读取需要的票据. type(TokenType)类型定义如下:
     *       eToken_QQ_Access,
     *       eToken_QQ_Pay,
     *       eToken_WX_Access,
     *       eToken_WX_Refresh
     */
    void WGLogin(ePlatform platform = ePlatform_None);
    
    /******************************************************************************
     * Description：微信二次授权登录（视频号授权）
     * Input：platform：授权平台，ePlatform_WeixinVideoLive
     * permissions: 授权所需权限
     * Output：
     * Return：调用成功返回eFlag_Succ，如果正在登录中返回eFlag_Logining
     ******************************************************************************/
    void WGChannelPermissionAuth(ePlatform platform = ePlatform_None, unsigned char *permissions  = (unsigned char *)"");

    /******************************************************************************
     * Description：授权登录(具有防重复点击，防超时功能)
     * Input：platform：登录平台, overtime：超时时间(秒)
     * Output：
     * Return：调用成功返回eFlag_Succ，如果正在登录中返回eFlag_Logining
     ******************************************************************************/
    eFlag WGLoginOpt(ePlatform platform = ePlatform_None, int overtime = DEFAUTL_LOGIN_OVERTIME);

    /**
     * @param platform 游戏传入的平台类型, 可能值为: ePlatform_QQ,
     * ePlatform_Weixin
     * 通过游戏设置的全局回调的OnLoginNotify(LoginRet&
     * loginRet)方法返回数据给游戏
     *     loginRet.platform表示当前的授权平台, 值类型为ePlatform,
     * 可能值为ePlatform_QQ, ePlatform_Weixin
     *     loginRet.flag值表示返回状态, 可能值(eFlag枚举)如下：
     *       eFlag_Succ: 返回成功,
     * 游戏接收到此flag以后直接读取LoginRet结构体中的票据进行游戏授权流程.
     *       eFlag_QQ_NoAcessToken: 手Q授权失败,
     * 游戏接收到此flag以后引导用户去重新授权(重试)即可.
     *       eFlag_QQ_UserCancel: 用户在授权过程中
     *       eFlag_QQ_LoginFail: 手Q授权失败,
     * 游戏接收到此flag以后引导用户去重新授权(重试)即可.
     *       eFlag_QQ_NetworkErr: 手Q授权过程中出现网络错误,
     * 游戏接收到此flag以后引导用户去重新授权(重试)即可.
     *     loginRet.token是一个Vector<TokenRet>, 其中存放的TokenRet有type和value,
     * 通过遍历Vector判断type来读取需要的票据. type(TokenType)类型定义如下:
     *       eToken_QQ_Access,
     *       eToken_QQ_Pay,
     *       eToken_WX_Access,
     *       eToken_WX_Refresh
     */
    void WGQrCodeLogin(ePlatform platform);
    /**
     * @return bool 返回值已弃用, 全都返回true
     */
    bool WGLogout();
    /**
     * @param permissions ePermission枚举值 或 运算的结果, 表示需要的授权项目
     *
     */
    void WGSetPermission(int permissions);

#pragma mark - WeixinShare
    // Weixin Share
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
    void WGSendToWeixin(unsigned char *title, unsigned char *desc, unsigned char *mediaTagName,
                        unsigned char *thumbImgData, const int &thumbImgDataLen, unsigned char *messageExt ,
                        unsigned char *userOpenId  = (unsigned char *)"");

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
    void WGSendToWeixinWithUrl(const eWechatScene &scene, unsigned char *title, unsigned char *desc, unsigned char *url,
                               unsigned char *mediaTagName, unsigned char *thumbImgData, const int &thumbImgDataLen,
                               unsigned char *messageExt , unsigned char *userOpenId = (unsigned char *)"");
    
    /**
     * @param scene 指定分享到朋友圈, 或者微信会话, 可能值和作用如下:
     *   WechatScene_Session: 分享到微信会话
     *   WechatScene_Timeline: 分享到微信朋友圈
     * @param mediaTagName (必填)请根据实际情况填入下列值的一个,
     此值会传到微信供统计用, 在分享返回时也会带回此值, 可以用于区分分享来源
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
     * @param imgDataLen 原图文件数据长度(建议图片小于1MB)
     *   通过游戏设置的全局回调的OnShareNotify(ShareRet&
     shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
     */
    void WGSendToWeixinWithPhoto(
        const eWechatScene &scene, unsigned char *mediaTagName, unsigned char *imgData, const int &imgDataLen);
    /**
     * @param scene 指定分享到朋友圈, 或者微信会话, 可能值和作用如下:
     *   WechatScene_Session: 分享到微信会话
     *   WechatScene_Timeline: 分享到微信朋友圈
     * @param mediaTagName (必填)请根据实际情况填入下列值的一个,
     此值会传到微信供统计用, 在分享返回时也会带回此值, 可以用于区分分享来源
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
     * @param imgDataLen 原图文件数据长度(建议图片小于1MB)
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过
     OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
     * @param messageAction scene为1(分享到微信朋友圈)的情况下才起作用
     *   WECHAT_SNS_JUMP_SHOWRANK       跳排行
     *   WECHAT_SNS_JUMP_URL            跳链接
     *   WECHAT_SNS_JUMP_APP           跳APP
     *   通过游戏设置的全局回调的OnShareNotify(ShareRet&
     shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
     */
    void WGSendToWeixinWithPhoto(const eWechatScene &scene, unsigned char *mediaTagName, unsigned char *imgData,
                                 const int &imgDataLen, unsigned char *messageExt, unsigned char *messageAction);
    
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
    void WGSendToWXNativeGamePage(const eWechatScene &scene, unsigned char *title, unsigned char *desc, unsigned char *mediaTagName,
                                  unsigned char *thumbImgData, const int &thumbImgDataLen,
                                  const bool isVideo, const int &videoDuration, unsigned char *shareData,
                                  unsigned char *messageExt, unsigned char *messageAction, unsigned char *userOpenId = (unsigned char *)"");

    /**
     * 把音乐消息分享给微信好友
     * @param scene 指定分享到朋友圈, 或者微信会话, 可能值和作用如下:
     *   WechatScene_Session: 分享到微信会话
     *   WechatScene_Timeline: 分享到微信朋友圈 (此种消息已经限制不能分享到朋友圈)
     * @param title 音乐消息的标题
     * @param desc    音乐消息的概要信息
     * @param musicUrl    音乐消息的目标URL
     * @param musicDataUrl    音乐消息的数据URL
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
     * @param imgData 原图文件数据
     * @param imgDataLen 原图文件数据长度(图片大小不z能超过10M)
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过
     OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
     * @param messageAction
     scene为WechatScene_Timeline(分享到微信朋友圈)的情况下才起作用
     *   WECHAT_SNS_JUMP_SHOWRANK       跳排行,查看排行榜
     *   WECHAT_SNS_JUMP_URL            跳链接,查看详情
     *   WECHAT_SNS_JUMP_APP            跳APP,玩一把
     *
     *   通过游戏设置的全局回调的OnShareNotify(ShareRet&
     shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
     */
    void WGSendToWeixinWithMusic(const eWechatScene &scene, unsigned char *title, unsigned char *desc,
                                 unsigned char *musicUrl, unsigned char *musicDataUrl, unsigned char *mediaTagName, unsigned char *imgData,
                                 const int &imgDataLen, unsigned char *messageExt, unsigned char *messageAction);

    /**
     * 此接口类似WGSendToQQGameFriend, 此接口用于分享消息到微信好友,
     分享必须指定好友openid
     * @param fOpenId 好友的openid
     * @param title 分享标题
     * @param description 分享描述
     * @param mediaId 图片的id 通过uploadToWX接口获取
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过
     OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
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
     */
    bool WGSendToWXGameFriend(unsigned char *fOpenId, unsigned char *title, unsigned char *description,
                              unsigned char *mediaId, unsigned char *messageExt, unsigned char *mediaTagName);
    /**
     * 此接口类似WGSendToQQGameFriend, 此接口用于分享消息到微信好友,
     分享必须指定好友openid
     * @param fOpenId 好友的openid
     * @param title 分享标题
     * @param description 分享描述
     * @param mediaId 图片的id 通过uploadToWX接口获取
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过
     OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
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
     * @param msdkExtInfo
     游戏自定义透传字段，通过分享结果shareRet.extInfo返回给游戏
     */
    bool WGSendToWXGameFriend(unsigned char *fOpenId, unsigned char *title, unsigned char *description,
                              unsigned char *mediaId, unsigned char *messageExt, unsigned char *mediaTagName, unsigned char *msdkExtInfo);

    /**
     * 此接口会分享消息到微信游戏中心内的消息中心，这种消息主要包含两部分，消息体和附加按钮，消息体主要包含展示内容
     * 附加按钮主要定义了点击以后的跳转动作（拉起APP，拉起页面、拉起排行榜），消息类型和按钮类型可以任意组合
     * @param fOpenid 好友的openid
     * @param title 游戏消息中心分享标题
     * @param content 游戏消息中心分享内容
     * @param pInfo
     * 消息体，这里可以传入四种消息类型，均为WXMessageTypeInfo的子类：
     *         TypeInfoImage: 图片消息（下面的几种属性全都要填值）
     *             std::string pictureUrl; // 图片缩略图
     *             int height; // 图片高度
     *             int width; // 图片宽度
     *         TypeInfoVideo: 视频消息（下面的几种属性全都要填值）
     *             std::string pictureUrl; // 视频缩略图
     *             int height; // 视频高度
     *             int width; // 视频宽度
     *             std::string mediaUrl; // 视频链接
     *         TypeInfoLink: 链接消息（下面的几种属性全都要填值）
     *             std::string pictureUrl; //
     * 在消息中心的消息图标Url（图片消息中，此链接则为图片URL)
     *             std::string targetUrl; // 链接消息的目标URL，点击消息拉起此链接
     *         TypeInfoText: 文本消息
     *
     * @param pButton
     * 按钮效果，这里可以传入三种按钮类型，均为WXMessageButton的子类：
     *         ButtonApp: 拉起应用（下面的几种属性全都要填值）
     *             std::string name; // 按钮名称
     *             std::string messageExt; //
     * 附加自定义信息，通过按钮拉起应用时会带回游戏
     *         ButtonWebview: 拉起web页面（下面的几种属性全都要填值）
     *             std::string name; // 按钮名称
     *             std::string webViewUrl; // 点击按钮后要跳转的页面
     *         ButtonRankView: 拉起排行榜（下面的几种属性全都要填值）
     *             std::string name; // 按钮名称
     *             std::string title; // 排行榜名称
     *             std::string rankViewButtonName; // 排行榜中按钮的名称
     *             td::string messageExt; //
     * 附加自定义信息，通过排行榜中按钮拉起应用时会带回游戏
     * @param msdkExtInfo
     * 游戏自定义透传字段，通过分享结果shareRet.extInfo返回给游戏
     *  @return 参数异常或未登陆
     */
    bool WGSendMessageToWechatGameCenter(unsigned char *fOpenid, unsigned char *title, unsigned char *content,
                                         WXMessageTypeInfo *pInfo, WXMessageButton *pButton, unsigned char *msdkExtInfo);


// Weixin Share End

#pragma mark - QQShare
    // QQ Share
    /**
     * @param act 好友点击分享消息拉起页面还是直接拉起游戏, 传入 1 拉起游戏, 传入
     0, 拉起targetUrl
     * @param fopenid 好友的openId
     * @param title 分享的标题
     * @param summary 分享的简介
     * @param targetUrl 跳转目标页面, 通常为游戏的详情页
     * @param imgUrl 分享缩略图URL
     * @param previewText 可选, 预览文字
     * @param gameTag 可选, 此参数必须填入如下值的其中一个
     MSG_INVITE                //邀请
     MSG_FRIEND_EXCEED       //超越炫耀
     MSG_HEART_SEND          //送心
     MSG_SHARE_FRIEND_PVP    //PVP对战
     */
    bool WGSendToQQGameFriend(int act, unsigned char *fopenid, unsigned char *title, unsigned char *summary,
                              unsigned char *targetUrl, unsigned char *imgUrl, unsigned char *previewText, unsigned char *gameTag);
    /**
     * @param act 好友点击分享消息拉起页面还是直接拉起游戏, 传入 1 拉起游戏, 传入
     0, 拉起targetUrl
     * @param fopenid 好友的openId
     * @param title 分享的标题
     * @param summary 分享的简介
     * @param targetUrl 内容的跳转url，填游戏对应游戏中心详情页，
     * @param imgUrl 分享缩略图URL
     * @param previewText 可选, 预览文字
     * @param gameTag 可选, 此参数必须填入如下值的其中一个
     MSG_INVITE                //邀请
     MSG_FRIEND_EXCEED       //超越炫耀
     MSG_HEART_SEND          //送心
     MSG_SHARE_FRIEND_PVP    //PVP对战
     * @param msdkExtInfo
     游戏自定义透传字段，通过分享结果shareRet.extInfo返回给游戏，游戏可以用extInfo区分request
     */
    bool WGSendToQQGameFriend(int act, unsigned char *fopenid, unsigned char *title, unsigned char *summary,
                              unsigned char *targetUrl, unsigned char *imgUrl, unsigned char *previewText, unsigned char *gameTag,
                              unsigned char *msdkExtInfo);

    /**
     * 把音乐消息分享到手Q会话
     * @param scene eQQScene:
     *             QQScene_QZone : 分享到空间
     *             QQScene_Session：分享到会话
     * @param title 结构化消息的标题
     * @param desc 结构化消息的概要信息
     * @param musicUrl      点击消息后跳转的URL
     * @param musicDataUrl  音乐数据URL（例如http:// ***.mp3）
     * @param imgUrl
     * 分享消息缩略图URL，可以为本地路径(直接填路径，例如：/sdcard/
     * ***test.png)或者网络路径(例如：http:// ***.jpg)，本地路径要放在sdcard
     *
     * 通过游戏设置的全局回调的OnShareNotify(ShareRet&
     * shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
     * 注意:
     *     由于手Q客户端部分的版本返回的回调是有问题的,
     * 故建议不要依赖此回调做其他逻辑。
     */
    void WGSendToQQWithMusic(const eQQScene &scene, unsigned char *title, unsigned char *desc, unsigned char *musicUrl,
                             unsigned char *musicDataUrl, unsigned char *imgUrl);

    /**
     * 手Q通用分享
     * @param serviceID 互联分配给业务的服务id
     * @param extraJson 可扩展参数，透传至QQ
     */
    void WGSendToQQWithCommonShare(unsigned char *serviceID, unsigned char *extraJson);
    
    /**
     * 分享丰富的图片到空间
     * @param summary 分享的正文(无最低字数限制，最高1w字)
     * @param imageDatas 分享的图片的本地路径，可支持多张图片(<=9
     * 张图片为发表说说，>9 张图片为上传图片到相册)，只支持本地图片
     * 注意: 此功能只在手Q5.9.5及其以上版本支持
     */
    void WGSendToQQWithRichPhoto(unsigned char *summary, std::vector<ImageParams> &imgParams , const unsigned char *extraScene,
                                 const unsigned char *messageExt)
    {

        MSDKVector<ImageParams> tmp_vc;
        for (int i = 0 ; i < imgParams.size(); i++) {
            tmp_vc.push_back(imgParams[i]);
        }

        this->_WGSendToQQWithRichPhoto(summary, tmp_vc, extraScene , messageExt);
    }
    
    /**
     * 分享视频到空间
     * @param summary 分享的正文(无最低字数限制，最高1w字)
     * @param videoParams 分享的视频路径，Android支持本地地址，iOS支持视频数据
     * @param extraScene 区分分享的场景，用于异化小尾巴展示和feeds点击行为，支持iOS
     * @param messageExt 游戏自定义字段，点击分享消息回到游戏时会透传回游戏，不需要的话可以填写空串，支持iOS
     * 注意: 此功能只在手Q5.9.5及其以上版本支持
     */
    void WGSendToQQWithVideo(unsigned char *summary, VideoParams &videoParams, unsigned char *extraScene, unsigned char *messageExt);
    
    // QQ Share End

    /**
     * 用户反馈接口, 反馈内容查看链接(Tencent内网):
     *         http://mcloud.ied.com/queryLogSystem/ceQuery.html?token=545bcbcfada62a4d84d7b0ee8e4b44bf&gameid=0&projectid=ce
     * @param body 反馈的内容, 内容由游戏自己定义格式, SDK对此没有限制
     *
     */
    void WGFeedback(unsigned char *body);
    /**
     * @param bRDMEnable 是否开启RDM的crash异常捕获上报
     * @param bMTAEnable 是否开启MTA的crash异常捕获上报
     */
    void WGEnableCrashReport(bool bRDMEnable, bool bMTAEnable) WG_DEPRECATED(3.0.0);
    /**
     * 自定义数据上报, 此接口仅支持一个key-value的上报, 从1.3.4版本开始,
     * 建议使用void WGReportEvent( unsigned char* name, std::vector<KVPair>&
     * eventList, bool isRealTime)
     * @param name 事件名称
     * @param body 事件内容
     * @param isRealTime 是否实时上报
     *
     */
    void WGReportEvent(unsigned char *name, unsigned char *body, bool isRealTime) WG_DEPRECATED(1.3.4);

    /**
     *  设置 MSDK 日志
     *  @param enabled true:打开 false:关闭
     */
    void WGOpenMSDKLog(bool enabled);

    /**
     * 返回MSDK版本号
     * @return MSDK版本号
     */
    const std::string WGGetVersion()
    {
        return this->_WGGetVersion().c_str();
    }
    /**
     * 如果没有再读assets/channel.ini中的渠道号,
     * 故游戏测试阶段可以自己写入渠道号到assets/channel.ini用于测试.
     * IOS返回plist中的CHANNEL_DENGTA字段
     * @return 安装渠道
     */
    const std::string WGGetChannelId()
    {
        return this->_WGGetChannelId().c_str();
    }
    /**
     * @return APP版本号
     */
    const std::string WGGetPlatformAPPVersion(ePlatform platform)
    {
        return this->_WGGetPlatformAPPVersion(platform).c_str();
    }

    /**
     * @return 注册渠道
     */
    const std::string WGGetRegisterChannelId()
    {
        return this->_WGGetRegisterChannelId().c_str();
    }
    /**
     * 此接口用于刷新微信的accessToken
     * refreshToken的用途就是刷新accessToken,
     * 只要refreshToken不过期就可以通过refreshToken刷新accessToken。
     * 有两种情况需要刷新accessToken,
     * 通过游戏设置的全局回调的OnLoginNotify(LoginRet&
     * loginRet)方法返回数据给游戏
     *     因为只有微信平台有refreshToken,
     * loginRet.platform的值只会是ePlatform_Weixin
     *     loginRet.flag值表示返回状态, 可能值(eFlag枚举)如下：
     *       eFlag_WX_RefreshTokenSucc: 刷新票据成功,
     * 游戏接收到此flag以后直接读取LoginRet结构体中的票据进行游戏授权流程.
     *       eFlag_WX_RefreshTokenFail: WGRefreshWXToken调用过程中网络出错,
     * 刷新失败, 游戏自己决定是否需要重试 WGRefreshWXToken
     */
    void WGRefreshWXToken();
    /**
     * @param platformType 游戏传入的平台类型, 可能值为: ePlatform_QQ,
     * ePlatform_Weixin
     * @return 平台的支持情况, false表示平台不支持授权, true则表示支持
     */
    bool WGIsPlatformInstalled(ePlatform platformType);
    /**
     * 检查平台是否支持SDK API接口
     * @param platformType 游戏传入的平台类型, 可能值为: ePlatform_QQ,
     * ePlatform_Weixin
     * @return 平台的支持情况, false表示平台不支持授权, true则表示支持
     */
    bool WGIsPlatformSupportApi(ePlatform platformType);

    /**
     * 获取pf, 用于支付, 和pfKey配对使用
     * @param cGameCustomInfo 默认可以填空,
     * 部分游戏经分有特殊需求可以通过此自定义字段传入特殊需求数据
     * @return pf
     */
    const std::string WGGetPf(unsigned char *cGameCustomInfo = NULL)
    {
        return this->_WGGetPf(cGameCustomInfo).c_str();
    }
    /**
     * 获取pfkey，pfKey由msdk 服务器加密生成，支付过程校验
     * @return 返回当前pf加密后对应fpKey字符串
     */
    const std::string WGGetPfKey()
    {
        return this->_WGGetPfKey().c_str();
    }

    /**
     *  输出msdk依赖平台版本号
     */
    void WGLogPlatformSDKVersion();  // log出msdk用到的各sdk版本号

    /**
     * 获取自己的QQ资料
     * @return void
     *   此接口的调用结果通过OnRelationNotify(RelationRet& relationRet)
     * 回调返回数据给游戏,
     *   RelationRet对象的persons属性是一个Vector<PersonInfo>,
     * 取第0个即是用户的个人信息.
     *   手Q授权的用户可以获取到的个人信息包含:
     *   nickname, openId, gender, pictureSmall, pictureMiddle, pictureLarge,
     * gpsCity, 其他字段为空.
     *   其中gpsCity字段为玩家所在城市信息，只有游戏调用过 WGGetNearbyPersonInfo
     * 或者 WGGetLocationInfo
     *   接口后，这个字段才有相应信息。
     */
    bool WGQueryQQMyInfo();

#ifdef __APPLE__
    /**
     * 获取自己的 Apple 登录资料
     * @return void
     *   此接口的调用结果通过OnRelationNotify(RelationRet& relationRet)
     * 回调返回数据给游戏,
     *   RelationRet对象的persons属性是一个Vector<PersonInfo>,
     * 取第0个即是用户的个人信息.
     *   APple 登录的用户可以获取到的个人信息包含:
     *   apple_email, apple_namePrefix，apple_givenName，apple_middleName，apple_familyName，apple_nameSuffix，apple_nickname 字段, 其他字段为空
     */
    bool WGQueryAppleMyInfo();
#endif
    /**
     * 获取QQ好友信息, 回调在OnRelationNotify中,
     * 其中RelationRet.persons为一个Vector, Vector中的内容即使好友信息,
     * QQ好友信息里面province和city为空
     * @return void
     * 此接口的调用结果通过OnRelationNotify(RelationRet& relationRet)
     *   回调返回数据给游戏, RelationRet对象的persons属性是一个Vector<PersonInfo>,
     *   其中的每个PersonInfo对象即是好友信息,
     *   好友信息包含: nickname, openId, gender, pictureSmall, pictureMiddle,
     * pictureLarge
     */
    bool WGQueryQQGameFriendsInfo();

    /**
    * 获取unionid(qq用户唯一标识), 回调在OnRelationNotify中
    * 其中RelationRet.persons为一个Vector，Vector的第一项即为自己的资料
     * 个人信息中包含unionID，其类型为一个string
    */
    void WGQueryUnionID();

    /**
     *   回调在OnRelationNotify中,其中RelationRet.persons为一个Vector,
     * Vector的第一项即为自己的资料
     *   个人信息包括nickname, openId, gender, pictureSmall, pictureMiddle,
     * pictureLarge, provice, city, gpsCity
     *   其中gpsCity字段为玩家所在城市信息，只有游戏调用过 WGGetNearbyPersonInfo
     * 或者 WGGetLocationInfo
     *   接口后，这个字段才有相应信息。
     */
    bool WGQueryWXMyInfo();
    /**
     * 获取微信好友信息, 回调在OnRelationNotify中,
     *   回调在OnRelationNotify中,其中RelationRet.persons为一个Vector,
     * Vector中的内容即为好友信息
     *   好友信息包括nickname, openId, gender, pictureSmall, pictureMiddle,
     * pictureLarge, provice, city
     */
    bool WGQueryWXGameFriendsInfo();

    /**
    *  @since 2.0.0
    *  此接口用于已经登录过的游戏, 在用户再次进入游戏时使用,
    * 游戏启动时先调用此接口, 此接口会尝试到后台验证票据
    *  此接口会通过OnLoginNotify将结果回调给游戏, 本接口会返回flag,
    * eFlag_Local_Invalid和eFlag_Succ,及eFlag_WX_RefreshTokenSucc。
    *  如果本地没有票据或者本地票据验证失败返回的flag为eFlag_Local_Invalid,
    * 游戏接到此flag则引导用户到授权页面授权即可.
    *  如果本地有票据并且验证成功, 则flag为eFlag_Succ,
    * 游戏接到此flag则可以直接使用sdk提供的票据, 无需再次验证.
    *   Callback: 验证结果通过我OnLoginNotify返回
    */
    void WGLoginWithLocalInfo();
    /*
     * 展示对应类型指定公告栏下的公告
     * @param scene 公告栏ID，不能为空, 这个参数和公告管理端的“公告栏”设置对应
     */
    void WGShowNotice(unsigned char *scene);
    /**
     * 隐藏滚动公告
     */
    void WGHideScrollNotice();
    /**
     *  @param openUrl 要打开的url
     */
    void WGOpenUrl(unsigned char *openUrl, const char *algorithm = "v2");
    /**
     *  @param openUrl 要打开的url
     */
    void WGOpenUrl(unsigned char *openUrl, const eMSDK_SCREENDIR &screendir, const char *algorithm = "v2");
    /**
     *  @param openUrl 要打开的url
     */
    void WGOpenUrl(unsigned char *openUrl, const eMSDK_SCREENDIR &screenDir, const bool fullScreen, const char *algorithm = "v2");
    /**
     * 在游戏中发送信息到页面js函数
     * @param params
     * 要传递给js函数的参数
     */
    void WGSendToWebJs(unsigned char *params);
    /**
     * @param openUrl 需要增加加密参数的url
     */
    const std::string WGGetEncodeUrl(unsigned char *openUrl, const char *algorithm = "v2")
    {
        return this->_WGGetEncodeUrl(openUrl, algorithm).c_str();
    }


    /*
     * 从本地数据库读取指定scene下指定type的当前有效公告
     * @param sence 公告栏ID，不能为空, 这个参数和公告管理端的“公告栏”设置对应
     * @return NoticeInfo结构的数组，NoticeInfo结构如下：
     typedef struct
     {
     std::string msg_id;            //公告id
     std::string open_id;        //用户open_id
     std::string msg_url;        //公告跳转链接
     eMSG_NOTICETYPE msg_type;    //公告类型，eMSG_NOTICETYPE
     std::string msg_scene;        //公告展示的场景，管理端后台配置
     std::string start_time;        //公告有效期开始时间
     std::string end_time;        //公告有效期结束时间
     eMSG_CONTENTTYPE content_type;    //公告内容类型，eMSG_CONTENTTYPE
     //网页公告特殊字段
     std::string content_url;     //网页公告URL
     //图片公告特殊字段
     std::vector<PicInfo> picArray;    //图片数组
     //文本公告特殊字段
     std::string msg_title;        //公告标题
     std::string msg_content;    //公告内容
     }NoticeInfo;
     */
    inline std::vector<NoticeInfo> WGGetNoticeData(unsigned char *scene)
    {
        return InnerWGGetNoticeData(scene);
    }

    /**
     *  打开AMS营销活动中心
     *
     *  @param params
     * 可传入附加在URL后的参数，长度限制256.格式为"key1=***&key2=***",注意特殊字符需要urlencode。
     *                不能和MSDK将附加在URL的固定参数重复，列表如下：
     *                参数名            说明            值
     *                timestamp           请求的时间戳
     *                appid            游戏ID
     *                algorithm           加密算法标识    v1
     *                msdkEncodeParam  密文
     *                version           MSDK版本号        例如1.6.2i
     *                sig               请求本身的签名
     *                encode           编码参数        1
     *  @return eFlag
     * 返回值，正常返回eFlag_Succ；如果params超长返回eFlag_UrlTooLong
     */
    bool WGOpenAmsCenter(unsigned char *params) WG_DEPRECATED(1.3.4);
    
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
    *  @param screenDir 屏幕方向，默认横竖屏
    *
    */
    void WGOpenAmsCenter(unsigned char *gameName,
                         unsigned char *actChannelId,
                         unsigned char *zoneId,
                         unsigned char *platformId,
                         unsigned char *partitionId,
                         unsigned char *roleId,
                         unsigned char *extra,
                         const eMSDK_SCREENDIR &screenDir = eMSDK_SCREENDIR_SENSOR);
    /**
     *  获取附近人的信息
     *  回调到OnLocationNotify
     *
     *  通过游戏设置的全局回调的OnLocationNofity(RelationRet&
     * rr)方法返回数据给游戏
     *     rr.platform表示当前的授权平台, 值类型为ePlatform, 可能值为ePlatform_QQ,
     * ePlatform_Weixin
     *     rr.flag值表示返回状态, 可能值(eFlag枚举)如下：
     *          eFlag_LbsNeedOpenLocationService: 需要引导用户开启定位服务
     *          eFlag_LbsLocateFail: 定位失败, 原因待确认，可重试
     *          eFlag_Succ: 获取附近的人成功
     *          eFlag_Error:  定位成功, 但是请求附近的人失败, 可重试
     *          eFlag_LbsLocateTimeout: 定位失败(超时)
     *     rr.persons是一个Vector, 其中保存了附近玩家的信息
     */
    void WGGetNearbyPersonInfo();
    /**
     *  @return 回调到OnLocationNotify
     *  @return void
     *   通过游戏设置的全局回调的OnLocationNofity(RelationRet&
     * rr)方法返回数据给游戏
     *     rr.platform表示当前的授权平台, 值类型为ePlatform, 可能值为ePlatform_QQ,
     * ePlatform_Weixin
     *     rr.flag值表示返回状态, 可能值(eFlag枚举)如下：
     *          eFlag_Succ: 清除成功
     *          eFlag_Error: 清除失败
     */
    bool WGCleanLocation();
    /**
     *  获取当前玩家位置信息。
     *  @return 回调到OnLocationGotNotify
     *  @return void
     *   通过游戏设置的全局回调的OnLocationGotNotify(LocationRet&
     * rr)方法返回数据给游戏
     *     rr.platform表示当前的授权平台, 值类型为ePlatform, 可能值为ePlatform_QQ,
     * ePlatform_Weixin
     *     rr.flag值表示返回状态, 可能值(eFlag枚举)如下：
     *          eFlag_Succ: 获取成功
     *          eFlag_Error: 获取失败
     *     rr.longitude 玩家位置经度，double类型
     *     rr.latitude 玩家位置纬度，double类型
     */
    bool WGGetLocationInfo();

    /**
     *  获取当前玩家的国籍信息(根据接入的IP)。回调到OnLocationGotCountryFromIPNotify
     *  通过游戏设置的全局回调的OnLocationGotCountryFromIPNotify(GetCountryFromIPRet &fromIPRet)
     *  方法返回数据给游戏fromIPRet
     *     fromIPRet.country 客户端IP对应的ISO_3166-1标准中的国家或地区中文名称
     *     fromIPRet.isQueryByRequestHeader  false: 通过查询缓存中的映射关系获得结果；true：通过请求投中的信息获得结果
     *     fromIPRet.ret 值表示返回状态,ret为0才表示成功
     *     fromIPRet.msg 表示传递的消息ret不为0时错误原因
     */
    void WGGetCountryFromIP();
    
    /**
     *  获取当前玩家的国家及省市信息(根据接入的IP)。
     *  通过游戏设置的全局回调的OnLocationGetIPNotify(GetIPInfoRet &ipInfoRet)
     */
    void WGGetIPInfo();
    /**
     * 查询公会绑定群加群时的GroupKey的信息，查询结果会通过WGGroupObserver的OnQueryGroupKeyNotify回调给游戏
     * @param cGroupOpenid 群openID
     */
//    void WGQueryQQGroupKey(unsigned char *cGroupOpenid);

    /**
     * 游戏内创建公会微信群，调用结果会通过WGGroupObserver的OnCreateWXGroupNotify回调给游戏，
     *         一般会返回一个建群的链接，游戏用内置浏览器打开该链接即可实现加建群。
     * @param unionid 工会ID
     * @param chatRoomName 聊天群名称
     * @param chatRoomNickName 用户在聊天群的自定义昵称
     */
    void WGCreateWXGroup(unsigned char *unionid, unsigned char *chatRoomName, unsigned char *chatRoomNickName);
    /**
     * 游戏内加入公会微信群，调用结果会通过WGGroupObserver的OnJoinWXGroupNotify回调给游戏，
     *         一般会返回一个加群的链接，游戏用内置浏览器打开该链接即可实现加加群。
     * @param unionid 工会ID
     * @param chatRoomNickName 用户在聊天群的自定义昵称
     */
    void WGJoinWXGroup(unsigned char *unionid, unsigned char *chatRoomNickName);
    /**
     * 游戏内查询公会微信群信息，用于检查是否创建微信公会群以及对应用户是否加入群
     *         调用结果会通过WGGroupObserver的OnQueryGroupInfoNotify回调给游戏，
     * @param unionid 工会ID
     * @param openIdList 待检查是否在群里的用户
     */
    void WGQueryWXGroupInfo(unsigned char *unionid, unsigned char *openIdList);
    /**
     * 分享结构化消息到微信公会群
     * @param msgType 消息类型，目前传1
     * @param subType 分享类型，邀请填1，炫耀填2，赠送填3，索要填4
     * @param unionid 工会ID
     * @param title 分享的标题
     * @param description 分享的简介
     * @param messageExt 游戏分享是传入字符串，通过此消息拉起游戏会通过
     OnWakeUpNotify(WakeupRet ret)中ret.messageExt回传给游戏
     * @param mediaTagName 请根据实际情况填入下列值的一个, 此值会传到微信供统计用,
     在分享返回时也会带回此值, 可以用于区分分享来源
     MSG_INVITE                //邀请
     MSG_FRIEND_EXCEED       //超越炫耀
     MSG_HEART_SEND          //送心
     MSG_SHARE_FRIEND_PVP    //PVP对战
     * @param imgUrl 分享缩略图URL
     * @param msdkExtInfo
     游戏自定义透传字段，通过分享结果shareRet.extInfo返回给游戏
     */

    void WGSendToWXGroup(int msgType, int subType, unsigned char *unionid, unsigned char *title,
                         unsigned char *description, unsigned char *messageExt, unsigned char *mediaTagName, unsigned char *imgUrl,
                         unsigned char *msdkExtInfo);
    /**
     * 设置游戏当前所处的场景开始点
     * @param cGameStatus
     * 场景值，MSDK提供的场景值请参考GameStatus的定义，游戏也可以自定义场景参数
     */
    void WGStartGameStatus(unsigned char *cGameStatus);
    /**
     * 设置游戏当前所处的场景结束点
     * @param cGameStatus
     * 场景值，MSDK提供的场景值请参考GameStatus的定义，游戏也可以自定义场景参数
     * @param succ 游戏对该场景执行结果的定义，例如成功、失败、异常等。
     * @param errorCode
     * 游戏该场景异常的错误码，用户标识或者记录该场景失败具体是因为什么原因
     */
    void WGEndGameStatus(unsigned char *cGameStatus, int succ, int errorCode);
    /**
     *  通过外部拉起的URL登陆。该接口用于异帐号场景发生时，用户选择使用外部拉起帐号时调用。
     *  登陆成功后通过onLoginNotify回调
     *
     *  @param flag
     * 为YES时表示用户需要切换到外部帐号，此时该接口会使用上一次保存的异帐号登陆数据登陆。登陆成功后通过onLoginNotify回调；如果没有票据，或票据无效函数将会返回NO，不会发生onLoginNotify回调。
     *              为NO时表示用户继续使用原帐号，此时删除保存的异帐号数据，避免产生混淆。
     *
     *  @return 如果没有票据，或票据无效将会返回NO；其它情况返回YES
     */

    bool WGSwitchUser(bool flag);

    /**
  * 可以针对不同的用户设置标签，如性别、年龄、学历、爱好等
  * @param tag 用户标签
  */
    void WGSetPushTag(unsigned char *tag);
    
    /**
    * 设置用户账号
    * @param account 用户账号
    */
    void WGSetPushAccount(unsigned char *account);

    /**
     * 删除设置的标签
     * @param tag 用户标签
     */
    void WGDeletePushTag(unsigned char *tag);
    

   /**
    * 删除设置的标签
    * @param account 用户标签
    */
    void WGDeletePushAccount(unsigned char *account);

    /**
    * 终止推送服务后，将无法通过腾讯移动推送服务向设备推送消息
    * 如再次需要再次消息推送，则必须需要再次调用 WGRegisterAPNSPushNotification 方法重启腾讯移动推送服务。
    *
    */
    void WGUnregisterPush();

    void WGClearLocalNotifications();



    /**
     * 打开微信deeplink（deeplink功能的开通和配置请联系微信侧）
     * @param link 具体跳转deeplink，可填写为：
     *             INDEX：跳转微信游戏中心首页
     *             DETAIL：跳转微信游戏中心详情页
     *             LIBRARY：跳转微信游戏中心游戏库
     *             具体跳转的url （需要在微信侧先配置好此url）
     */
    void WGOpenWeiXinDeeplink(unsigned char *link);
    /**
     * 上报日志到bugly
     * @param level 日志级别:
     *     eBuglyLogLevel_S (0), //Silent
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
    void WGBuglyLog(eBuglyLogLevel level, unsigned char *log);

    /**
     * 关闭bugly上报 
     */
    void WGCloseCrashReport();
    
    /**
     * 实名认证
     * @param authInfo 实名认证信息结构体
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
     *  province;        //省份
     *  city;           //城市
     */
    void WGRealNameAuth(RealNameAuthInfo &authInfo);

    /**
     *　微信解绑群
     * @param groupId 工会ID
     *
     */
    void WGUnbindWeiXinGroup(unsigned char *groupId);

    /**
     *　微信查询群状态
     * @param groupId 工会ID
     * @param opType  0：查询是否建群 1：查询是否加群
     *
     */
    void WGQueryWXGroupStatus(unsigned char *groupId, eStatusType opType);

    /**
     *
     *微信游戏圈传图功能
     目前暂时对图片大小有限制，最好不能超过512K
     */
    void WGShareToWXGameline(unsigned char *imgData, const int &imgDataLen, unsigned char *gameExtra);


    /**
     *
     *微信朋友圈圈视频分享功能
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
     videoParams: * 此struct在android、ios上对应的字段不同。android需要填写视频路径：android_videoPath，ios需要自行读取填写视频数据ios_videoData和视频数据长度ios_videoDataLen,对应的ios上字段:
     videoParams.videoData： 视频文件数据，sdk限制不能超过10M大小，
     videoParams.videoDataLen：视频文件数据长度，sdk限制不能超过10M大小，
     mediaTagName：数据统计用，建议传MSG_SHARE_MOMENT_VIDEO
     mediaAction： 透传到微信游戏后台，可使用该字段透传信息以便后台设置该条朋友圈的一些字段，格式为action_type#param
     mediaExt：启动游戏时透传回游戏侧
     * @return void
     *   通过游戏设置的全局回调的OnShareNotify(ShareRet&
     * shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
     *
     */
    void WGSendToWeixinWithVideo(const eWechatScene &scene,
                                 const unsigned char *title,
                                 const unsigned char *desc,
                                 VideoParams &videoParams,
                                 const unsigned char *mediaTagName,
                                 const unsigned char *messageAction,
                                 const unsigned char *messageExt);
    
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
    void WGSendToWXStateWithPhoto(const unsigned char *stateId,
                                  const unsigned char *stateTitle,
                                  ImageParams &imageParams,
                                  const unsigned char *jumpUrl);
    
    void WGSendToWXStateWithPhotoOpenMiniApp(const unsigned char *stateId,
                                  const unsigned char *stateTitle,
                                  ImageParams &imageParams,
                                  const unsigned char *userName,
                                  const unsigned char *path = (const unsigned char *)"",
                                  const eMiniProgramType &type = ProgramType_Release);
    
    /**
     *
     * 分享视频至微信视频号
     * 参数说明：
     * videoParams：视频信息，必填，视频格式目前仅支持MP4，时长不超过30分钟，大小不超过450M，宽高比在1:3 - 3:1之间
     * 此struct在android、ios上对应的字段不同。android需要填写视频路径：android_videoPath，ios需要填写系统相册中视频标识符PHAsset localIdentifier：ios_videoLocalIdentifier
     * messageExt：拓展字段，选填，携带额外信息，当前版本暂未使⽤
     *
     */
    void WGSendToWXChannelShareVideo(VideoParams &videoParams, const unsigned char *messageExt);

    /**
     *  视频号直播
     * @param channelLiveJson 游戏拉起视频号直播透传json数据，包括liveJsonInfo和openID字段
     * @param messageExt 拓展字段，选填，携带额外信息，当前版本暂未使⽤
     */
    void WGSendToWXChannelStartLive(unsigned char *channelLiveJson, unsigned char *messageExt);
    
    /* 手q大图分享带自定义参数接口
     * 大图分享到空间能力，手Q空间侧需求针对传入内容，个性化匹配用户看到的小尾巴内容。实现个性化的操作结果。
     * 参数说明：
     * scene：分享的场景，目前只支持qq空间QQScene_QZone
     * ImageParams：
     * 此struct在android、ios上对应的字段不同。android需要填写图片路径：android_imagePath，ios需要自行读取填写图片数据ios_imageData和图片数据长度ios_imageDataLen
     * 注意如果使用的是android版本的java接口对应的函数为 WGSendToQQWithPhoto(eQQScene scene,String imgFilePath,String extraScene,String messageExt),其中imgFilePath请填写图片路径
     * extraScene：区分分享的场景，用于异化小尾巴展示和feeds点击行为
     * messageExt：游戏自定义字段，点击分享消息回到游戏时会透传回游戏，不需要的话可以填写空串
     * gameTag 此值会传到QQ供统计使用
     * @return void
     *   通过游戏设置的全局回调的OnShareNotify(ShareRet&
     * shareRet)回调返回数据给游戏, shareRet.flag值表示返回状态, 可能值及说明如下:
     *     eFlag_Succ: 分享成功
     *     eFlag_Error: 分享失败
     *
     */
    void WGSendToQQWithPhoto(const eQQScene &scene,
                             ImageParams &imageParams,
                             const unsigned char *extraScene,
                             const unsigned char *messageExt,
                             const unsigned char *gameTag = (const unsigned char *)"");

    /*
     *启动QQ小程序
     * @param miniProgramAppid 必填，小程序的AppID（注：必须在QQ互联平台中，将该小程序与分享的App绑定）
     * @param miniProgramPath  非必填，小程序的展示路径，不填展示默认小程序首页
     * @param type             非必填，小程序的类型，默认正式版(ProgramType_Release)
     */
    void WGLaunchQQMiniApp(const unsigned char *miniProgramAppid,
                           const unsigned char *miniProgramPath,
                           const eMiniProgramType &type = ProgramType_Release);
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
    void WGSendToQQWithMiniApp(
            const eQQScene &scene,
            unsigned char *title,
            unsigned char *desc,
            unsigned char *url,
            ImageParams &imageParams,
            unsigned char *miniProgramAppid,
            unsigned char *miniProgramPath,
            const eMiniProgramType &type = ProgramType_Release);

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
    void WGSendToWXWithMiniApp(const eWechatScene &scene,
                               const unsigned char *title,
                               const unsigned char *desc,
                               const unsigned char *thumbImgData,
                               const int &thumbImgDataLen,
                               const unsigned char *webpageUrl,
                               const unsigned char *userName,
                               const unsigned char *path,
                               bool withShareTicket,
                               const unsigned char *messageExt,
                               const unsigned char *messageAction,
                               const unsigned char *mediaTagName = (const unsigned char *)"",
                               const eMiniProgramType &type = ProgramType_Release,
                               const unsigned char *userOpenId  = (const unsigned char *)"");

    /**
     * 启动小程序
     * @param userName  //  填小程序原始id
     * @param path      //  拉起小程序页面的可带参路径，不填默认拉起小程序首页
     * @param type      //  preview = 2 ; test = 1 ; release = 0;
     */
    void WGLaunchMiniApp(const unsigned char *userName,
                         const unsigned char *path,
                         const eMiniProgramType &type
                            );

    /**
     *
     * 微信OpenBusinessView分享功能
     * @param businessType 业务内容 必填
     * @param query 业务参数 选填
     * @param extInfo 额外信息(Json格式)
     * @param extData iOS专用，指iOS本地沙盒路径视频文件的地址
     */
    void WGSendToWXWithOpenBusinessView(const unsigned char *businessType,
                                        const unsigned char *query,
                                        const unsigned char *extInfo,
                                        const unsigned char *extData);

    /**  分享ARK到QQ
     *
     * 分享时调用
     * @param scene QQScene_QZone:空间，默认弹框 QQScene_Session:好友
     * @param title 标题
     * @param desc 内容
     * @param url  内容的跳转url，填游戏对应游戏中心详情页
     * @param imgUrl 图片url
     * @param jsonString ARK分享jsonString（此处由业务侧与手q ARK开发时约定如何配置，msdk仅透传）
     * @param messageExt：游戏自定义字段，点击分享消息回到游戏时会透传回游戏，不需要的话可以填写空串
     * @param gameTag 此值会传到QQ供统计使用
     * @return void
     */
    void WGSendToQQWithArk(
                           const eQQScene& scene,
                           unsigned char* title,
                           unsigned char* desc,
                           unsigned char* url,
                           unsigned char* imgUrl,
                           unsigned char* jsonString,
                           unsigned char* messageExt = (unsigned char *)"",
                           unsigned char* gameTag = (unsigned char *)""
                           );

    /**  分享纯文字到QQ空间
     *
     * 分享时调用
     * @param text 内容
     * @return 
     */
    void WGSendToQQWithText(unsigned char* text, unsigned char* extraScene, unsigned char* messageExt);

    /**
     *
     *创建并绑定手q群
     *GameGuild：
     * guildId：公会id（必填）
     * guildName:公会名称（必填）
     * zoneId:区服ID（必填）
     * roleId:角色ID（必填）
     * partition：小区区服ID，可不填（选填）
     * userZoneId：用户的区服ID，会长可能转让给非本区分的人，所以公会区服不一定是用户区服。（选填）
     * userLabel：修改群名片，不填为不修改群名片，规则"【YYYY】zzzz" YYYY指用户的游戏数据，zzzz指用户游戏内的昵称（选填）
     * nickName：用户昵称（选填）
     * type："0"公会，"1"队伍，"2"赛事（选填）
     * areaId:游戏大区ID，"1"qq,"2"微信 （选填）
     */
    void WGCreateQQGroupV2(GameGuild &gameGuild);

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
    void WGJoinQQGroupV2(GameGuild &gameGuild,const char *groupId);
    /**
     *　游戏内查询用户与群关系
     * @param groupId 	群id
     *
     */
    void WGQueryQQGroupInfoV2(const char *groupId);

    /**
     *
     *解绑手q群
     *GameGuild：
     * guildId：公会id（必填）
     * guildName:公会名称（必填）
     * zoneId:区服ID（必填）
     * userZoneId：用户的区服ID，会长可能转让给非本区分的人，所以公会区服不一定是用户区服。（选填）
     * type："0"公会，"1"队伍，"2"赛事（选填）
     * areaId:游戏大区ID，"1"qq,"2"微信 （选填）
     */
    void WGUnbindQQGroupV2(GameGuild &gameGuild);

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
    void WGBindExistQQGroupV2(GameGuild &gameGuild,const char *groupId,const char *groupName);

    /**
     *
     *获取绑定手q群的群号
     *GameGuild：
     * guildId：公会id（必填）
     * zoneId:区服ID（必填）
     * type："0"公会，"1"队伍，"2"赛事（选填）
     */
    void WGGetQQGroupCodeV2(GameGuild &gameGuild);

    /**
     *
     *查询绑定的公会信息
     *
     * groupId：手q群ID（原gc）
     * type:"0"公会，"1"队伍，"2"赛事
     */
    void WGQueryBindGuildV2(const char *groupId, const int type = 0);

    /**
     *
     *查询创建的qq群列表（与是否绑定无关）
     *
     */
    void WGGetQQGroupListV2();

    /**
     *
     *提醒公会会长绑定手q群
     *GameGuild：
     * guildId：公会id（必填）
     * zoneId:区服ID（必填）
     * roleId:角色ID（必填）
     * roleName:角色名称（必填）
     * userZoneId：用户的区服ID，会长可能转让给非本区分的人，所以公会区服不一定是用户区服。（选填）
     * type："0"公会，"1"队伍，"2"赛事（选填）
     * leaderOpenid：公会会长的openid（必填）
     * leaderRoleId：公会会长的roleid（必填）
     * leaderZoneId:会长区服信息，会长可以转让给非本区服的人（选填）
     * areaId:游戏大区ID，"1"qq,"2"微信 （选填）
     */
    void WGRemindGuildLeaderV2(GameGuild &gameGuild);

    /**
     * 协议形式打开浏览器
     * jsonStr： json格式的协议，可以控制浏览器 全屏/非全屏 是否有返回按钮等
     */
    void WGOpenFullScreenWebViewWithJson(const char *jsonStr);

    /**
     * 上报prajna
     * serialNumber：上报prajna的串号
     */
    void WGReportPrajna(const char *serialNumber);
    
    /**
	 * 在游戏窗口中打开嵌入式浏览器页面
	 * url 要打开的页面链接
	 * 返回值 1表示打开成功，0或者负数表示打开失败
	 */
    int WGOpenEmbeddedWebView(const char* url);

    /**
	 * 在游戏窗口中以数据的形式打开浏览器页面
	 * charset 数据的编码格式（目前只支持UTF-8编码的数据）
	 * data 传递给页面的数据
	 * lens 传递的数据长度
	 * 返回值 1表示打开成功，0或者负数表示打开失败
	 */
    int WGOpenEmbeddedWebView(const char* charset , const char* data , int lens);

    /**
     * 在游戏窗口中关闭浏览器页面
     * 返回值 1表示关闭成功，0或者负数表示关闭失败
     */
    int WGCloseEmbeddedWebView();

    /**
     * 在游戏中发送信息到页面js函数
     * params 要传递给js函数的函数名、参数信息
     */
    void WGCallToEmbeddedWebView(const char* params);

    /**
     * 设置webview的背景图片
     * data 图片数据
     * lens 图片数据的长度
     */
    bool WGSetEmbeddedWebViewBackground(const char* imageData , int lens);
    
    /**
     * 设置是否允许收集敏感信息
     *
     *
    */
    void WGSetCouldCollectSensitiveInfo(bool couldCollect);

    /**
    * 以json形式设置敏感信息单字段开关，优先级小于总开关，目前支持设置AndroidID、Apn、Imei，QImei
    * 参数示例{"AndroidID":true,"Apn":true,"Imei":false,"QImei":true,"QImei36":true}
    *
    */
    void WGSetCollectSensitiveInfo(const char *jsonInfo);

    /**
     * 以json形式设置敏感信息字段至各个组件SDK，目前支持{"AndroidID":"xxx","WiFiMacAddress":"xxx","Imei":"xxx"}
     *
     *
    */
    void WGSetSensitiveInfo(const char *jsonInfo);
    
    /**
     * 打开游戏数据授权页
     *
     * @param openUrl 游戏数据授权页网址链接
     * @param screenDir 屏幕方向
     * @param isFullScreen 是否以全屏的方式打开
     * @param algorithm 加密算法标识
     */
    void WGOpenGameDataAuthCenter(unsigned char *openUrl,
                                  const eMSDK_SCREENDIR &screenDir = eMSDK_SCREENDIR_SENSOR,
                                  const bool isFullScreen = false,
                                  const char *algorithm = "v2");
    
    /*
     * @param screenDir 屏幕方向
     * @param isFullScreen 是否以全屏的方式打开
     * @param algorithm 加密算法标识
     */
    void WGOpenDeleteAccountUrl(const eMSDK_SCREENDIR &screenDir = eMSDK_SCREENDIR_SENSOR,
                               const bool isFullScreen = false,
                               const char *algorithm = "v2");
    
    // MSDK内部使用的测试接口
    void WGMachineTest();
    // MSDK内部使用的测试接口
    void WGSingleTest();

    void WGStartSingleTest(bool isMainThread);

    void WGStartSingleTestMultiThread();
    
    void WGCheckWXUniversalLink();
    
private:
    WGPlatform();
    virtual ~WGPlatform();

private:
    const MSDKString _WGGetVersion();
    const MSDKString _WGGetChannelId();
    const MSDKString _WGGetPlatformAPPVersion(ePlatform platform);
    const MSDKString _WGGetRegisterChannelId();
    const MSDKString _WGGetPf(unsigned char *cGameCustomInfo);
    const MSDKString _WGGetPfKey();
    const MSDKString _WGGetEncodeUrl(unsigned char *openUrl, const char *algorithm = "v2");
    const MSDKString _WGGetGuestID();
    const MSDKVector<MSDKNoticeInfo> _InnerWGGetNoticeData(unsigned char *scene);
    void _WGSendToQQWithRichPhoto(unsigned char *summary, MSDKVector<ImageParams> &imgParams ,const unsigned char *extraScene,
                                  const unsigned char *messageExt);
    void _WGTestSpeed(MSDKVector<MSDKString> &addrList);
    void _WGReportEvent(unsigned char *name, MSDKVector<MSDKKVPair> &eventList, bool isRealTime);
    
    void _WGReportException(eExceptionType exception_type,
                            unsigned char *exception_name,
                            unsigned char *exception_msg,
                            unsigned char *exception_stack,
                            MSDKVector<MSDKKVPair> &ext_info);
    void _LOG_InnerWGGetNoticeData(MSDKVector<MSDKNoticeInfo> &v);
    void _WGSendToWXGameLive(const eWechatScene &scene, unsigned char *title, unsigned char *desc, unsigned char *messageExt, MSDKVector<MSDKKVPair> &extInfo);

};

#endif
