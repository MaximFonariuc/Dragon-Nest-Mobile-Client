//
//  MSDKLocalConfig.h
//  MSDKFoundation
//
//  Created by MikeFu on 16/9/21.
//  Copyright © 2016年 Tencent. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MSDKEnums.h"

@interface MSDKLocalConfig : NSObject

+ (id)shareInstance;

#pragma mark - plist配置

- (NSString *)getAppIdByPlatform:(ePlatform)platform;

- (NSString *)getChannelId;

- (NSString *)getOfferId;

- (BOOL)isMSDKTestEnv;

- (NSString *)getMSDKEnv;

- (NSString *)getMSDKServerUrl;

- (NSString *)getMSDKKey;

- (NSString *)getAppKeyByPlatform:(ePlatform)platform;

- (NSString *)getPlatformIdByPlatform:(ePlatform)platform;

- (NSString *)getPlatformStrByPlatform:(ePlatform)platform;

- (NSString *)getGuestAppIdWithoutPrefix;

- (NSString *)getAppVersion;

- (NSString *)getUniversalLinkByPlatform:(ePlatform)platform;

- (BOOL)needNotice;

- (BOOL)needHttpIPv6Support;

- (NSInteger)getNoticeRefreshTime;

- (BOOL)needStartAutoRefreshJob;

- (int)getQQSdkWebviewScreenDir;

- (int)getRealNameAuthSwitch;

- (NSString *)getDeleteAccountUrl;

#pragma mark - 模块开关

- (BOOL)needPush;

- (BOOL)needPushAtForeground;

- (BOOL)needBeacon;

- (BOOL)needBugly;

- (BOOL)needHttpDns;

- (BOOL)needBeaconEvent;

- (BOOL)isWGOpenUrlNeedUrlEncode;

- (NSString *)getWXScopeExtraParams;

- (BOOL)needWebviewForceAdaptBangScreen;

- (BOOL)GetLoginCheckPaytoken;

- (NSString *)GetWebViewLoadingBackgroundHexColor;

- (NSString *)getCenterControlWebViewLoadingBackgroundHexColor;

- (BOOL)getAppTrackingEnable;

@end
