/******************************************************************************
 * Filename： WGObserverProxy.h
 * Description：
 * Date：2017/07/04
 * Author：qingcuilu
 * Version： 
 * Copyright 1998 - 2017 TENCENT Inc. All Rights Reserved
 * History：
 		<Author>		<Date>    <Version>  	<Description>
 ******************************************************************************/
#ifndef ANDROID_WGOBSERVERPROXY_H
#define ANDROID_WGOBSERVERPROXY_H

#include "WGProxy.h"

class WGObserverProxy : public WGProxy
{
public:
    // WGPlatformObserver 对应的observer
    virtual void _OnLoginNotify(WGBaseObserver *observer, ConvertLoginRet&loginRet)
    {
        LoginRet _loginRet = loginRet.ToLoginRet();
        observer->OnLoginNotify(_loginRet);
    }
    virtual void _OnChannelPermissionAuthNotify(WGBaseObserver *observer, ConvertLoginRet&loginRet)
    {
        LoginRet _loginRet = loginRet.ToLoginRet();
        observer->OnChannelPermissionAuthNotify(_loginRet);
    }
    virtual void _OnShareNotify(WGBaseObserver *observer, ConvertShareRet& shareRet)
    {
        ShareRet _shareRet = shareRet.ToShareRet();
        observer->OnShareNotify(_shareRet);
    }
    virtual void _OnWakeupNotify(WGBaseObserver *observer, ConvertWakeupRet& wakeupRet)
    {
        WakeupRet _wakeupRet = wakeupRet.ToWakeupRet();
        observer->OnWakeupNotify(_wakeupRet);
    }
    virtual void _OnRelationNotify(WGBaseObserver *observer, ConvertRelationRet&relationRet)
    {
        RelationRet _relationRet = relationRet.ToRelationRet();
        observer->OnRelationNotify(_relationRet);
    }
    virtual void _OnLocationNotify(WGBaseObserver *observer, ConvertRelationRet&relationRet)
    {
        RelationRet _relationRet = relationRet.ToRelationRet();
        observer->OnLocationNotify(_relationRet);
    }
    virtual void _OnLocationGotNotify(WGBaseObserver *observer, ConvertLocationRet&locationRet)
    {
        LocationRet _locationRet = locationRet.ToLocationRet();
        observer->OnLocationGotNotify(_locationRet);
    }
    virtual void _OnLocationGotCountryFromIPNotify(WGBaseObserver *observer, ConvertGetCountryFromIPRet&fromIPRet)
    {
        GetCountryFromIPRet _ipRet = fromIPRet.ToGetCountryFromIPRet();
        observer->OnLocationGotCountryFromIPNotify(_ipRet);
    }
    virtual void _OnLocationGetIPInfoNotify(WGBaseObserver *observer, ConvertGetIPInfoRet&getIpInfoRet)
    {
        GetIPInfoRet _ipInfoRet = getIpInfoRet.ToGetIPInfoRet();
        observer->OnLocationGetIPInfoNotify(_ipInfoRet);
    }
    virtual void _OnFeedbackNotify(WGBaseObserver *observer, int flag, MSDKString desc)
    {
        std::string _desc = desc.c_str();
        observer->OnFeedbackNotify(flag, _desc);
    }
    virtual void _OnCrashNotify(WGBaseObserver *observer, int crashType, MSDKString errType, MSDKString errMsg, MSDKString errStack)
    {
        std::string _errType = errType.c_str();
        std::string _errMsg = errMsg.c_str();
        std::string _errStack = errStack.c_str();
        observer->OnCrashNotify(crashType, _errType.c_str(), _errMsg.c_str(), _errStack.c_str());
    }

    virtual MSDKString _OnCrashExtMessageNotify(WGBaseObserver *observer)

    {
        return observer->OnCrashExtMessageNotify();
    }

#ifdef ANDROID
    virtual void _OnAddWXCardNotify(WGBaseObserver *observer, ConvertCardRet&cardRet)
    {
        CardRet _cardRet = cardRet.ToCardRet();
        observer->OnAddWXCardNotify(_cardRet);
    }
    virtual MSDKVector<char> _OnCrashExtDataNotify(WGBaseObserver *observer)
    {
        std::vector<char> v = observer->OnCrashExtDataNotify();
        return MsdkCovertTypeUtils::VCharToMSDKVChar(v);
    }
#endif

    virtual void _OnMachineTestResult(WGBaseObserver *observer, bool isSuccess,
                              MSDKString& desc)
    {
        std::string _desc = desc.c_str();
        observer->OnMachineTestResult(isSuccess, _desc);
    }

public:
    // 实名制的observer
    virtual void _OnRealNameAuthNotify(WGBaseObserver *observer, ConvertRealNameAuthRet&
    realNameAuthRet)
    {
        RealNameAuthRet _realNameAuthRet = realNameAuthRet.ToRealNameAuthRet();
        observer->OnRealNameAuthNotify(_realNameAuthRet);
    }

public:
    // 应用宝更新的observer
    virtual void _OnCheckNeedUpdateInfo(WGBaseObserver *observer,
                                long newApkSize,
                                MSDKString newFeature,
                                long patchSize,
                                int status,
                                MSDKString updateDownloadUrl,
                                int updateMethod)
    {
        std::string _newFeature = newFeature.c_str();
        std::string _updateDownloadUrl = updateDownloadUrl.c_str();
        observer->OnCheckNeedUpdateInfo(newApkSize, _newFeature, patchSize,
            status, _updateDownloadUrl, updateMethod);
    }
    virtual void _OnDownloadAppProgressChanged(WGBaseObserver *observer, long receiveDataLen, long totalDataLen)
    {
        observer->OnDownloadAppProgressChanged(receiveDataLen, totalDataLen);
    }
    virtual void _OnDownloadAppStateChanged(WGBaseObserver *observer, int state, int errorCode, MSDKString errorMsg)
    {
        std::string _errorMsg = errorMsg.c_str();
        observer->OnDownloadAppStateChanged(state, errorCode, _errorMsg);
    }
	virtual void _OnDownloadYYBProgressChanged(WGBaseObserver *observer, MSDKString url, long receiveDataLen, long totalDataLen)
    {
        std::string _url = url.c_str();
        observer->OnDownloadYYBProgressChanged(_url, receiveDataLen, totalDataLen);
    }
    virtual void _OnDownloadYYBStateChanged(WGBaseObserver *observer, MSDKString url, int state, int errorCode, MSDKString errorMsg)
    {
        std::string _url = url.c_str();
        std::string _errorMsg = errorMsg.c_str();
        observer->OnDownloadYYBStateChanged(_url, state, errorCode, _errorMsg);
    }

public:
    // 浏览器更新
    virtual void _OnWebviewNotify(WGBaseObserver *observer, ConvertWebviewRet& webviewRet)
    {
        WebviewRet _webviewRet = webviewRet.ToWebviewRet();
        observer->OnWebviewNotify(_webviewRet);
    }

public:
    // 群功能的observer
    virtual void _OnQueryGroupInfoNotify(WGBaseObserver *observer, ConvertGroupRet&groupRet)
    {
        GroupRet _groupRet = groupRet.ToGroupRet();
        observer->OnQueryGroupInfoNotify(_groupRet);
    }
//    virtual void _OnBindGroupNotify(WGBaseObserver *observer, ConvertGroupRet& groupRet)
//    {
//        GroupRet _groupRet = groupRet.ToGroupRet();
//        observer->OnBindGroupNotify(_groupRet);
//    }
    virtual void _OnUnbindGroupNotify(WGBaseObserver *observer, ConvertGroupRet& groupRet)
    {
        GroupRet _groupRet = groupRet.ToGroupRet();
        observer->OnUnbindGroupNotify(_groupRet);
    }
//    virtual void _OnQueryGroupKeyNotify(WGBaseObserver *observer, ConvertGroupRet&groupRet)
//    {
//        GroupRet _groupRet = groupRet.ToGroupRet();
//        observer->OnQueryGroupKeyNotify(_groupRet);
//    }
    virtual void _OnCreateWXGroupNotify(WGBaseObserver *observer, ConvertGroupRet&groupRet)
    {
        GroupRet _groupRet = groupRet.ToGroupRet();
        observer->OnCreateWXGroupNotify(_groupRet);
    }
    virtual void _OnJoinWXGroupNotify(WGBaseObserver *observer, ConvertGroupRet& groupRet)
    {
        GroupRet _groupRet = groupRet.ToGroupRet();
        observer->OnJoinWXGroupNotify(_groupRet);
    }
    virtual void _OnQueryWXGroupStatusNotify(WGBaseObserver *observer, ConvertGroupRet&groupRet)
    {
        GroupRet _groupRet = groupRet.ToGroupRet();
        observer->OnQueryWXGroupStatusNotify(_groupRet);
    }
//    virtual void _OnJoinQQGroupNotify(WGBaseObserver *observer, ConvertGroupRet& groupRet)
//    {
//        GroupRet _groupRet = groupRet.ToGroupRet();
//        observer->OnJoinQQGroupNotify(_groupRet);
//    }
    virtual void _OnCreateGroupV2Notify(WGBaseObserver *observer, ConvertGroupRet& groupRet)
    {
        GroupRet _groupRet = groupRet.ToGroupRet();
        observer->OnCreateGroupV2Notify(_groupRet);
    }
    virtual void _OnJoinGroupV2Notify(WGBaseObserver *observer, ConvertGroupRet& groupRet)
    {
        GroupRet _groupRet = groupRet.ToGroupRet();
        observer->OnJoinGroupV2Notify(_groupRet);
    }
    virtual void _OnQueryGroupInfoV2Notify(WGBaseObserver *observer, ConvertGroupRet& groupRet)
    {
        GroupRet _groupRet = groupRet.ToGroupRet();
        observer->OnQueryGroupInfoV2Notify(_groupRet);
    }
    virtual void _OnUnbindGroupV2Notify(WGBaseObserver *observer, ConvertGroupRet& groupRet)
    {
        GroupRet _groupRet = groupRet.ToGroupRet();
        observer->OnUnbindGroupV2Notify(_groupRet);
    }
    virtual void _OnGetGroupCodeV2Notify(WGBaseObserver *observer, ConvertGroupRet& groupRet)
    {
        GroupRet _groupRet = groupRet.ToGroupRet();
        observer->OnGetGroupCodeV2Notify(_groupRet);
    }
    virtual void _OnQueryBindGuildV2Notify(WGBaseObserver *observer, ConvertGroupRet& groupRet)
    {
        GroupRet _groupRet = groupRet.ToGroupRet();
        observer->OnQueryBindGuildV2Notify(_groupRet);
    }
    virtual void _OnBindExistGroupV2Notify(WGBaseObserver *observer, ConvertGroupRet& groupRet)
    {
        GroupRet _groupRet = groupRet.ToGroupRet();
        observer->OnBindExistGroupV2Notify(_groupRet);
    }
    virtual void _OnGetGroupListV2Notify(WGBaseObserver *observer, ConvertGroupRet& groupRet)
    {
        GroupRet _groupRet = groupRet.ToGroupRet();
        observer->OnGetGroupListV2Notify(_groupRet);
    }
    virtual void _OnRemindGuildLeaderV2Notify(WGBaseObserver *observer, ConvertGroupRet& groupRet)
    {
        GroupRet _groupRet = groupRet.ToGroupRet();
        observer->OnRemindGuildLeaderV2Notify(_groupRet);
    }
};

#endif //ANDROID_WGOBSERVERPROXY_H
