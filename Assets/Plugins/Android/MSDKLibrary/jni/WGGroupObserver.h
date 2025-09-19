//
//  WGGroupObserver.h
//  MSDK
//
//  Created by 付亚明 on 7/30/15.
//  Copyright (c) 2015 Tencent. All rights reserved.
//

#ifndef MSDK_WGGroupObserver_h
#define MSDK_WGGroupObserver_h

#include <stdio.h>
#include <string>
#include "MSDKStructs.h"
#include "msdk_convert_type.h"
#include "WGProxy.h"

/*! @brief 加群绑群类
 *
 * SDK通过通知类和外部调用者通讯
 */
class WGGroupObserver : public WGBaseObserver
{
   public:
    /*! @brief 查询群成员回调
     *
     * 将查询的操作结果通知上层App
     * @param groupRet 查询结果
     */
    virtual void OnQueryGroupInfoNotify(GroupRet& groupRet) = 0;
//
//    /*! @brief QQ群与工会绑定接口回调
//     *
//     * 将QQ群与工会绑定接口的结果通知上层App
//     * @param groupRet 加群结果
//     */
//    virtual void OnBindGroupNotify(GroupRet& groupRet) = 0;

    /*! @brief 解绑群
     *
     * 将解绑群操作结果通知上层App
     * @param groupRet 操作群结果
     */
    virtual void OnUnbindGroupNotify(GroupRet& groupRet) = 0;
//
//    /*! @brief 查询qq加群用的GroupKey回调
//     *
//     * 将查询qq加群用的GroupKey的结果通知上层App
//     * @param groupRet 加群结果
//     */
//    virtual void OnQueryGroupKeyNotify(GroupRet& groupRet) = 0;

    /*! @brief 微信建群回调
     *
     * 将创建的操作结果通知上层App
     * @param groupRet 创建结果
     */
    virtual void OnCreateWXGroupNotify(GroupRet& groupRet) = 0;

    /*! @brief 微信加群回调
     *
     * 将加群的操作结果通知上层App
     * @param groupRet 加群结果
     */
    virtual void OnJoinWXGroupNotify(GroupRet& groupRet) = 0;

    /*! @brief 微信查询群状态
     *
     * 将微信查询群状态的操作结果通知上层App
     * @param groupRet 操作群结果
     */
    virtual void OnQueryWXGroupStatusNotify(GroupRet& groupRet) = 0;

//    /*! @brief 游戏内新QQ加群接口回调
//     *
//     * 将游戏内新QQ加群结果通知上层App
//     * @param groupRet 加群结果
//     */
//    virtual void OnJoinQQGroupNotify(GroupRet& groupRet) = 0;


    virtual void OnCreateGroupV2Notify(GroupRet& groupRet){};

    virtual void OnJoinGroupV2Notify(GroupRet& groupRet){};

    virtual void OnQueryGroupInfoV2Notify(GroupRet& groupRet){};

    virtual void OnUnbindGroupV2Notify(GroupRet& groupRet){};

    virtual void OnGetGroupCodeV2Notify(GroupRet& groupRet){};

    virtual void OnQueryBindGuildV2Notify(GroupRet& groupRet){};

    virtual void OnBindExistGroupV2Notify(GroupRet& groupRet){};

    virtual void OnGetGroupListV2Notify(GroupRet& groupRet){};

    virtual void OnRemindGuildLeaderV2Notify(GroupRet& groupRet){};

    //注：发送群消息回调走WGPlatformObserver OnShareNotify(ShareRet& shareRet);
    virtual ~WGGroupObserver() {}

public:
    WGGroupObserver* group_observer_;
};

#endif
