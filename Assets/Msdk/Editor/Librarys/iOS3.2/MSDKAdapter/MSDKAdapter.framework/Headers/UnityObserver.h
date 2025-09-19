//
//  UnityObserver.h
//  MSDKDevDemo
//
//  Created by 付亚明 on 3/6/15.
//  Copyright (c) 2015 Tencent. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <MSDK/MSDK.h>


#ifndef WGFrameworkDemo_UnityObserver_h
#define WGFrameworkDemo_UnityObserver_h

class UnityObserver : public WGPlatformObserver,
                      public WGADObserver,
                      public WGGroupObserver,
                      public WGRealNameAuthObserver,
                      public WGWebviewObserver,
                      public WGEmbeddedWebViewObserver
{
   private:
    static UnityObserver* m_pInst;

   public:
    static UnityObserver* GetInstance();

#pragma mark - WGPlatformObserver
    // 登陆回调
    void OnLoginNotify(LoginRet& loginRet);
    //视频号授权回调
    void OnChannelPermissionAuthNotify(LoginRet& loginRet);
    // 检查 微信 UL 接入情况结果回调
    void OnCheckULNotify(CheckULRet& checkULRet);
    // 分享回调
    void OnShareNotify(ShareRet& shareRet);
    // 唤醒回调
    void OnWakeupNotify(WakeupRet& wakeupRet);
    // 关系链回调
    void OnRelationNotify(RelationRet& relationRet);
    // 附近的人回调
    void OnLocationNotify(RelationRet& relationRet);
    // 玩家当前位置回调
    void OnLocationGotNotify(LocationRet& locationRet);
    // 反馈相关回调
    void OnFeedbackNotify(int flag, std::string desc);

    // crash时的处理
    std::string OnCrashExtMessageNotify();
    
    // 根据 IP 获取玩家国籍信息回调
    void OnLocationGotCountryFromIPNotify(GetCountryFromIPRet &fromIPRet);
    //根据 IP 获取玩家国家及省市信息回调
    void OnLocationGetIPInfoNotify(GetIPInfoRet &ipInfoRet);

#pragma mark - WGRealNameAuthObserver
    // 实名认证回调
    void OnRealNameAuthNotify(RealNameAuthRet& realNameAuthRet);

#pragma mark - WGADObserver
    // 广告回调
    void OnADNotify(ADRet& adRet);

#pragma mark - WGGroupObserver
    // 微信建群回调
    void OnCreateWXGroupNotify(GroupRet& groupRet);
    // 查询群成员回调
    void OnQueryGroupInfoNotify(GroupRet& groupRet);
    // 微信加群回调
    void OnJoinWXGroupNotify(GroupRet& groupRet);
    // 查询群key回调
    void OnQueryGroupKeyNotify(GroupRet& groupRet);
    // 解绑群回调
    void OnUnbindGroupNotify(GroupRet& groupRet);
    // 获取微信群状态回调
    void OnQueryWXGroupStatusNotify(GroupRet& groupRet);
    // 绑定群回调
    void OnBindGroupNotify(GroupRet& groupRet);
    // 加入QQ群回调
    void OnJoinQQGroupNotify(GroupRet& groupRet);
    
    // 加绑群V2回调
    // 创建并绑定群回调
    void OnCreateGroupV2Notify(GroupRet& groupRet);
    // 加入群回调
    void OnJoinGroupV2Notify(GroupRet& groupRet);
    // 查询群回调
    void OnQueryGroupInfoV2Notify(GroupRet& groupRet);
    // 解绑回调
    void OnUnbindGroupV2Notify(GroupRet& groupRet);
    // 查询工会绑定的群回调
    void OnGetGroupCodeV2Notify(GroupRet& groupRet) ;
    // 查询群绑定的工会回调
    void OnQueryBindGuildV2Notify(GroupRet& groupRet);
    // 绑定群回调
    void OnBindExistGroupV2Notify(GroupRet& groupRet);
    // 获取创建的群回调
    void OnGetGroupListV2Notify(GroupRet& groupRet);
    // 提醒会长绑群回调
    void OnRemindGuildLeaderV2Notify(GroupRet& groupRet);
    
#pragma mark - WGWebviewObserver
    void OnWebviewNotify(WebviewRet& webviewRet);

#pragma mark - WGEmbeddedWebViewObserver
    void OnJsCallback(const char* params);

    void OnWebProgressChanged(const char* url, float progress);

    void OnWebClose();
#pragma mark - TPNSObserver
    void OnReceiveNotificationResponseNotify(const char *userInfo);
};

#endif
