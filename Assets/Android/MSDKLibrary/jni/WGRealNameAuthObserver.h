//
//  WGRealNameAuthObserver.h
//  MSDK
//
//  Created by Mike on 5/26/16.
//  Copyright © 2016 Tencent. All rights reserved.
//

#ifndef WGRealNameAuthObserver_h
#define WGRealNameAuthObserver_h

#include "MSDKStructs.h"
#include "msdk_convert_type.h"
/*! @brief RealNameAuth类
 *
 * SDK通过通知类和外部调用者通讯
 */
class WGRealNameAuthObserver : public WGBaseObserver
{
public:
    /*! @brief 实名认证回调
     *
     * 将实名认证的操作结果通知上层App
     * @param realNameAuthRet 实名认证结果
     */
    virtual void OnRealNameAuthNotify(RealNameAuthRet& realNameAuthRet) = 0;
    virtual ~WGRealNameAuthObserver() { }

public:
    WGRealNameAuthObserver* real_name_auth_observer_;
};

#endif /* WGRealNameAuthObserver_h */
