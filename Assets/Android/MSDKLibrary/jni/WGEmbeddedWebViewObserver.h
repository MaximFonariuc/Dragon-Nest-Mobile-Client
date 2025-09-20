//
// Created by qingcuilu on 2018/7/25.
//

#ifndef WGEMBEDDEDWEBVIEWOBSERVER_H
#define WGEMBEDDEDWEBVIEWOBSERVER_H

#include "MSDKStructs.h"

/*! @brief Webview类
 *
 * SDK通过通知类和外部调用者通讯
 * 因使用的是基础类型，不需要做跨so保护
 */
class WGEmbeddedWebViewObserver
{
public:
    /**
     * js调用的回调
     * @param params 网页js接口传递过来的参数
     */
    virtual void OnJsCallback(const char* params) = 0;

    /**
     * 浏览器打开界面的进度
     * @param url 打开的url
     * @param progress 打开url的进度，范围[0,1]
     */
    virtual void OnWebProgressChanged(const char* url, float progress) = 0;

    /**
     * 浏览器关闭的回调
     */
    virtual void OnWebClose() = 0;

    virtual ~WGEmbeddedWebViewObserver() { }
};

#endif //WGEMBEDDEDWEBVIEWOBSERVER_H
