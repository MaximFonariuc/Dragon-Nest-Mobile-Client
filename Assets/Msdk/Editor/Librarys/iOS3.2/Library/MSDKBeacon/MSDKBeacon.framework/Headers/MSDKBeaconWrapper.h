//
//  MSDKBeaconWrapper.h
//  UMSDK
//
//  Created by MikeFu on 2017/2/16.
//  Copyright © 2017年 Tencent. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <MSDK/MSDKBaseBeacon.h>

@interface MSDKBeaconWrapper : MSDKBaseBeacon

@property (nonatomic, copy) NSString * appkey;

+ (id)sharedInstance;

- (void)initBeacon:(NSString *)appid openId:(NSString *)openId;

- (void)enableAudit:(BOOL)audit;

- (void)handleOpenURL:(NSURL *)url;

- (NSString *)getBeaconVersion;

- (void)setLoginStateToBeasonSDK:(NSString *)openId;

- (void)report:(NSString *)eventName params:(NSDictionary *)params isRealTime:(BOOL)isRealTime;

- (void)report:(BOOL)isOk eventName:(NSString *)eventName params:(NSDictionary *)params isRealTime:(BOOL)isRealTime;

- (NSString *)getQIMEI;

- (NSString *)getQIMEINew;

@end
