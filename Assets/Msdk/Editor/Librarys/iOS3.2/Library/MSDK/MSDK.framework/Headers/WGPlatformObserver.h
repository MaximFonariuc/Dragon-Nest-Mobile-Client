//
//  WGPlatformObserver.h
//  WGPlatform
//
//  Created by fly chen on 2/22/13.
//  Copyright (c) 2013 tencent.com. All rights reserved.
//

#ifndef WGPlatform_WGPlatformObserver_h
#define WGPlatform_WGPlatformObserver_h

#include "WGProxy.h"

/*! @brief WGPlatform通知类
 *
 * SDK通过通知类和外部调用者通讯
 */
class WGPlatformObserver : public WGBaseObserver
{
   public:
    /*! @brief 登录回调
     *
     * 登录时通知上层App，并传递结果
     * @param loginRet 参数
     */
    virtual void OnLoginNotify(LoginRet& loginRet) = 0;
    /*! @brief 视频号授权回调
     *
     * 登录时通知上层App，并传递结果
     * @param loginRet 参数
     */
    virtual void OnChannelPermissionAuthNotify(LoginRet& loginRet) {};
#ifdef __APPLE__
    /*! @brief 微信 CheckUniversalLink 回调
    *
    * WX checkUL 检查结果
    * @param CheckULRet 参数
    */
    virtual void OnCheckULNotify(CheckULRet& checkUlRet) = 0;

    /*! @brief TPNS点击消息 回调
    *
    * 在用户点击TPNS推送时传递所需信息
    * @param userInfo 参数
    */
    virtual void OnReceiveNotificationResponseNotify(const char *userInfo){};
#endif
    /*! @brief 分享结果
     *
     * 将分享的操作结果通知上层App
     * @param shareRet 分享结果
     */
    virtual void OnShareNotify(ShareRet& shareRet) = 0;

    /*! @brief 被其他应用拉起
     *
     *被其他平台拉起
     * @param wakeupRet  唤起参数
     */
    virtual void OnWakeupNotify(WakeupRet& wakeupRet) = 0;

    virtual void OnRelationNotify(RelationRet& relationRet) = 0;

    virtual void OnLocationNotify(RelationRet& relationRet) = 0;

    virtual void OnLocationGotNotify(LocationRet& locationRet) = 0;

    virtual void OnLocationGotCountryFromIPNotify(GetCountryFromIPRet& fromIPRet) = 0;
    
    virtual void OnLocationGetIPInfoNotify(GetIPInfoRet& ipInfoRet) = 0;

    virtual void OnFeedbackNotify(int flag, std::string desc) = 0;
    
    /*! @brief Crash回调
    *
    * Crash时通知上层App，并传递Crash堆栈
    * @param crashType Crash类型，仅Android端有效
    *      0:Java crash 1:Java caught exception 2:Native crash 3:Unity error 4:ANR 5:Cocos JS error 6:Cocos Lua error
    * @param errType 错误类型，仅Android端有效
    * @param errMsg 错误信息，仅Android端有效
    * @param errStack 错误堆栈
    */
    virtual void OnCrashNotify(int crashType, const char *errType, const char *errMsg, const char *errStack) {}
    
    virtual std::string OnCrashExtMessageNotify()
    {
        return "";
    }
    // TODO 异帐号逻辑先注释
    /*virtual bool OnDiffAccountAlert(){
        return true;
    }*/

#ifdef ANDROID
    virtual void OnAddWXCardNotify(CardRet& cardRet) {}
    virtual std::vector<char> OnCrashExtDataNotify()
    {
        std::vector<char> empty_result;
        return empty_result;
    }
#endif

    virtual ~WGPlatformObserver(){};

#ifdef MSDK_DEBUG
    virtual void OnMachineTestResult(bool isSuccess, std::string& desc){};
#endif

    
public:
    WGPlatformObserver* platform_observer_;
};

#endif
