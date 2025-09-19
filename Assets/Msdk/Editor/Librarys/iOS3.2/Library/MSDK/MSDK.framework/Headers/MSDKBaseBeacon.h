//
//  MSDKBaseBeacon.h
//  UMSDK
//
//  Created by luosong on 2022/3/30.
//  Copyright © 2022 Tencent. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface MSDKBaseBeacon : NSObject

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
