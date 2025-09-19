//
// MSDKPublicDefine.h
// MSDKFoundation
//
// Created by Jason on 14/11/25.
// Copyright (c) 2014年 Tencent. All rights reserved.
//

#ifndef MSDKFoundation_MSDKPublicDefine_h
#define MSDKFoundation_MSDKPublicDefine_h

#define MSDK_VERSION @"3.3.30i"
#define MSDK_SVN_CODE @""
//crash场景
#pragma mark - crash scene
/************* 应用状态 *************/
//前台运行中
#define MSDK_FRONT @"MSDKFront"
//后台运行中
#define MSDK_BACK @"MSDKBack"
//切换到后台中
#define MSDK_GOBACK @"MSDKGoBack"
//切换回前台中
#define MSDK_GOFRONT @"MSDKGoFront"
//退出游戏
#define MSDK_EXIST @"MSDKExist"

/************* MSDK相关场景 *************/
//默认状态
#define MSDK_DEFAULT @"MSDKDefault"
//初始化MSDK中
#define MSDK_INIT @"MSDKInit"
//设置回调
#define MSDK_OBSERVER @"MSDKObserver"
//登陆中
#define MSDK_LOGIN @"MSDKLogin"
//登出中
#define MSDK_LOGOUT @"MSDKLogout"
//更新中
#define MSDK_UPDATE @"MSDKUpdate"
//分享中
#define MSDK_SHARE @"MSDKShare"

#pragma mark - Constants
#define kOneDaySeconds 86400
#define kCNULL ""
#define kNSNULL @""

#pragma mark - Network Return Json Keys
#define kFlagError -1

#pragma mark - Platform Id
#define kWenXin @"wechat" //微信
#define kQQ @"qq_m" //手机QQ
#define kQzone @"qzone_m" //手机Qzone
#define kMobile @"mobile" //手机qq游戏大厅
#define kDesktop @"desktop_m" //默认桌面启动
#define kApple @"apple" // apple 登录

#pragma mark - Platform Id Suffix
#define kQQAcount @"_qq"
#define kWXAcount @"_wx"
#define kGuestAcount @"_guest"
#define kAppleAccount @"_apple"


#pragma mark - Important For All Modules
#define kopenId @"openId"
#define kplatform @"platform"
#define kpf @"pf"
#define kpfKey @"pfKey"
#define kflag @"flag"
#define kaccessTokenType @"accessTokenType"
#define kaccessTokenValue @"accessTokenValue"
#define kaccessTokenExpiration @"accessTokenExpiration"
#define krefreshTokenType @"refreshTokenType"
#define krefreshTokenValue @"refreshTokenValue"
#define krefreshTokenExpiration @"refreshTokenExpiration"
#define kupdateTime @"updateTime"
#define kuser_id @"user_id"
#define kpayTokenType @"payTokenType"
#define kpayTokenValue @"payTokenValue"
#define kpayTokenExpiration @"payTokenExpiration"
#define CHANNEL_DENGTA @"CHANNEL_DENGTA"

#define kOsVersion @"osVersion"
#define kMSDK_OfferIdKey @"MSDK_OfferId"

#pragma mark - Local Setting Keys
#define kMSDK_LocalSetting_Key @"key"
#define kMSDK_LocalSetting_Value @"value"
#define kMSDK_LocalSetting_Http_DNS @"http_dns"
#define kMSDK_LocalSetting_Current_Platform @"cur_Platform"
#define kMSDK_LocalSetting_Last_Platform @"last_Platform"
#define kMSDK_LocalSetting_Last_Openid @"last_Openid"
#define kMSDK_LocalSetting_Permission @"Permission"

#pragma mark - Login Identity
#define kMSDK_Jwt_Token @"jwt_token"
#endif

