//
//  BeaconAuditInterface.h
//  Beacon
//
//  Created by jerry on 2017/5/19.
//  Copyright © 2017年 tencent. All rights reserved.
//

@interface BeaconAuditInterface : NSObject

/*
 * 开启稽核模块，默认关闭
 */
+ (void)setAuditEnable:(BOOL)enable;
+ (void)handleOpenURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication;

@end
