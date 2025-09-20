//
//  WGWebviewObserver.h
//  MSDK
//
//  Created by Mike on 4/12/16.
//  Copyright © 2016 Tencent. All rights reserved.
//

#ifndef WGWebviewObserver_h
#define WGWebviewObserver_h

#include "MSDKStructs.h"
#include "msdk_convert_type.h"
/*! @brief Webview类
 *
 * SDK通过通知类和外部调用者通讯
 */
class WGWebviewObserver : public WGBaseObserver
{
public:
    /*! @brief Webview回调
     *
     * 将创建的操作结果通知上层App
     * @param webviewRet 创建结果
     */
    virtual void OnWebviewNotify(WebviewRet& webviewRet) = 0;
    virtual ~WGWebviewObserver() { }

public:
    WGWebviewObserver* webview_observer_;
};

#endif /* WGWebviewObserver_h */
