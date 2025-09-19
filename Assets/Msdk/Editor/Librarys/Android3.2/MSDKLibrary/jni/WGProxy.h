/******************************************************************************
 * Filename： WGProxy.h
 * Description：
 * Date：2017/6/29
 * Author：linkxzhou
 * Version： 
 * Copyright 1998 - 2017 TENCENT Inc. All Rights Reserved
 * History：
 		<Author>		<Date>    <Version>  	<Description>
 ******************************************************************************/
#ifndef ANDROID_WGPROXY_H
#define ANDROID_WGPROXY_H

#include "MSDKStructs.h"
#include "msdk_convert_type.h"

class WGBaseObserver;
class WGProxy
{
public:
    virtual ~WGProxy() {}
public:
    // WGPlatformObserver 对应的observer
    virtual void _OnLoginNotify(WGBaseObserver *observer, ConvertLoginRet&loginRet) = 0;
    virtual void _OnChannelPermissionAuthNotify(WGBaseObserver *observer, ConvertLoginRet&loginRet) = 0;
    virtual void _OnShareNotify(WGBaseObserver *observer, ConvertShareRet&shareRet) = 0;
    virtual void _OnWakeupNotify(WGBaseObserver *observer, ConvertWakeupRet&wakeupRet) = 0;
    virtual void _OnRelationNotify(WGBaseObserver *observer, ConvertRelationRet&relationRet) = 0;
    virtual void _OnLocationNotify(WGBaseObserver *observer, ConvertRelationRet&relationRet) = 0;
    virtual void _OnLocationGotNotify(WGBaseObserver *observer, ConvertLocationRet&locationRet) = 0;
    virtual void _OnLocationGotCountryFromIPNotify(WGBaseObserver *observer, ConvertGetCountryFromIPRet&fromIPRet) = 0;
    virtual void _OnLocationGetIPInfoNotify(WGBaseObserver *observer, ConvertGetIPInfoRet&ipInfoRet) = 0;
    virtual void _OnFeedbackNotify(WGBaseObserver *observer, int flag, MSDKString desc) = 0;
    virtual MSDKString _OnCrashExtMessageNotify(WGBaseObserver *observer) = 0;
    virtual void _OnCrashNotify(WGBaseObserver *observer, int crashType, MSDKString errType, MSDKString errMsg, MSDKString errStack) = 0;


#ifdef ANDROID
    virtual void _OnAddWXCardNotify(WGBaseObserver *observer, ConvertCardRet&cardRet) = 0;
    virtual MSDKVector<char> _OnCrashExtDataNotify(WGBaseObserver *observer) = 0;
#endif

    virtual void _OnMachineTestResult(WGBaseObserver *observer, bool isSuccess, MSDKString& desc) = 0;

public:
    // 实名制的observer
    virtual void _OnRealNameAuthNotify(WGBaseObserver *observer, ConvertRealNameAuthRet&realNameAuthRet) = 0;

public:
    // 应用宝更新的observer
    virtual void _OnCheckNeedUpdateInfo(WGBaseObserver *observer,
                                        long newApkSize,
                                        MSDKString newFeature,
                                        long patchSize,
                                        int status,
                                        MSDKString updateDownloadUrl,
                                        int updateMethod) = 0;
    virtual void _OnDownloadAppProgressChanged(WGBaseObserver *observer, long
    receiveDataLen, long totalDataLen) = 0;

    virtual void _OnDownloadAppStateChanged(WGBaseObserver *observer, int state, int
    errorCode, MSDKString errorMsg) = 0;
    virtual void _OnDownloadYYBProgressChanged(WGBaseObserver *observer, MSDKString
    url, long receiveDataLen, long totalDataLen) = 0;
    virtual void _OnDownloadYYBStateChanged(WGBaseObserver *observer, MSDKString url,
                                            int state, int errorCode, MSDKString errorMsg) = 0;

public:
    // 浏览器回调
    virtual void _OnWebviewNotify(WGBaseObserver *observer, ConvertWebviewRet&webviewRet) = 0;

public:
    // 群功能的observer
    virtual void _OnQueryGroupInfoNotify(WGBaseObserver *observer, ConvertGroupRet&groupRet) = 0;
//    virtual void _OnBindGroupNotify(WGBaseObserver *observer, ConvertGroupRet& groupRet) = 0;
    virtual void _OnUnbindGroupNotify(WGBaseObserver *observer, ConvertGroupRet& groupRet) = 0;
//    virtual void _OnQueryGroupKeyNotify(WGBaseObserver *observer, ConvertGroupRet&groupRet) = 0;
    virtual void _OnCreateWXGroupNotify(WGBaseObserver *observer, ConvertGroupRet&groupRet) = 0;
    virtual void _OnJoinWXGroupNotify(WGBaseObserver *observer, ConvertGroupRet&groupRet) = 0;
    virtual void _OnQueryWXGroupStatusNotify(WGBaseObserver *observer, ConvertGroupRet&groupRet) = 0;
//    virtual void _OnJoinQQGroupNotify(WGBaseObserver *observer, ConvertGroupRet&groupRet) = 0;
    virtual void _OnCreateGroupV2Notify(WGBaseObserver *observer, ConvertGroupRet&groupRet)= 0;
    virtual void _OnJoinGroupV2Notify(WGBaseObserver *observer, ConvertGroupRet&groupRet)= 0;
    virtual void _OnQueryGroupInfoV2Notify(WGBaseObserver *observer, ConvertGroupRet&groupRet)= 0;
    virtual void _OnUnbindGroupV2Notify(WGBaseObserver *observer, ConvertGroupRet&groupRet)= 0;
    virtual void _OnGetGroupCodeV2Notify(WGBaseObserver *observer, ConvertGroupRet&groupRet)= 0;
    virtual void _OnQueryBindGuildV2Notify(WGBaseObserver *observer, ConvertGroupRet&groupRet) = 0;
    virtual void _OnBindExistGroupV2Notify(WGBaseObserver *observer, ConvertGroupRet&groupRet)= 0;
    virtual void _OnGetGroupListV2Notify(WGBaseObserver *observer, ConvertGroupRet&groupRet)= 0;
    virtual void _OnRemindGuildLeaderV2Notify(WGBaseObserver *observer, ConvertGroupRet&groupRet)= 0;
};

class WGBaseObserver
{
public:
    WGBaseObserver() : proxy_(NULL)
    { }
    virtual ~WGBaseObserver()
    {
        if (proxy_ != NULL) {
            delete proxy_;
            proxy_ = NULL;
        }
    }

public:
    // WGPlatformObserver 对应的observer
    virtual void OnLoginNotify(LoginRet& loginRet) {}
    virtual void OnChannelPermissionAuthNotify(LoginRet& loginRet) {}
#ifdef __APPLE__
    virtual void OnCheckULNotify(CheckULRet& loginRet) {}
#endif
    virtual void OnShareNotify(ShareRet& shareRet) {}
    virtual void OnWakeupNotify(WakeupRet& wakeupRet) {}
    virtual void OnRelationNotify(RelationRet& relationRet) {}
    virtual void OnLocationNotify(RelationRet& relationRet) {}
    virtual void OnLocationGotNotify(LocationRet& locationRet) {}
    virtual void OnLocationGotCountryFromIPNotify(GetCountryFromIPRet& fromIPRet) {}
    virtual void OnLocationGetIPInfoNotify(GetIPInfoRet& ipInfoRet) {}
    virtual void OnFeedbackNotify(int flag, std::string desc) {}
    virtual void OnCrashNotify(int crashType, const char *errType, const char *errMsg, const char *errStack) {}
    virtual std::string OnCrashExtMessageNotify()
    {
        return "";
    }

#ifdef ANDROID
    virtual void OnAddWXCardNotify(CardRet& cardRet) {}
    virtual std::vector<char> OnCrashExtDataNotify()
    {
        return std::vector<char>();
    }
#endif

    virtual void OnMachineTestResult(bool isSuccess, std::string& desc) {}

public:
    virtual void OnWebviewNotify(WebviewRet& webviewRet) {}

public:
    // 群对应的observer
    virtual void OnQueryGroupInfoNotify(GroupRet& groupRet) {}
//    virtual void OnBindGroupNotify(GroupRet& groupRet) {}
    virtual void OnUnbindGroupNotify(GroupRet& groupRet) {}
//    virtual void OnQueryGroupKeyNotify(GroupRet& groupRet) {}
    virtual void OnCreateWXGroupNotify(GroupRet& groupRet) {}
    virtual void OnJoinWXGroupNotify(GroupRet& groupRet) {}
    virtual void OnQueryWXGroupStatusNotify(GroupRet& groupRet) {}
//    virtual void OnJoinQQGroupNotify(GroupRet& groupRet) {}
    virtual void OnCreateGroupV2Notify(GroupRet& groupRet) {}
    virtual void OnJoinGroupV2Notify(GroupRet& groupRet) {}
    virtual void OnQueryGroupInfoV2Notify(GroupRet& groupRet) {}
    virtual void OnUnbindGroupV2Notify(GroupRet& groupRet) {}
    virtual void OnGetGroupCodeV2Notify(GroupRet& groupRet) {}
    virtual void OnQueryBindGuildV2Notify(GroupRet& groupRet) {}
    virtual void OnBindExistGroupV2Notify(GroupRet& groupRet) {}
    virtual void OnGetGroupListV2Notify(GroupRet& groupRet) {}
    virtual void OnRemindGuildLeaderV2Notify(GroupRet& groupRet) {}

public:
    virtual void OnRealNameAuthNotify(RealNameAuthRet& realNameAuthRet) {}

public:
    virtual void OnCheckNeedUpdateInfo(long newApkSize, std::string newFeature, long patchSize, int status,
                                       std::string updateDownloadUrl, int updateMethod) {}
    virtual void OnDownloadAppProgressChanged(long receiveDataLen, long totalDataLen) {}
    virtual void OnDownloadAppStateChanged(int state, int errorCode, std::string errorMsg) {}
    virtual void OnDownloadYYBProgressChanged(std::string url, long receiveDataLen, long totalDataLen) {}
    virtual void OnDownloadYYBStateChanged(std::string url, int state, int errorCode, std::string errorMsg) {}

public:
    WGProxy* proxy_;
};

#endif //ANDROID_WGPROXY_H
