//
//  MSDKInfoTool.h
//  MSDK
//
//  Created by Jason on 14/11/7.
//  Copyright (c) 2014年 Tencent. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface MSDKInfoTool : NSObject

+ (NSDictionary *)getDeviceInfo:(int)sence;
+ (NSString *)getCurrentDeviceModel;
+ (NSString *)getAPN;
+ (NSString *)idfaString;
+ (NSString *)tgTokenString;
+ (NSString *)qimeiString;
+ (NSString *)qimeiNewString;
+ (NSString *)stringWithUUID;
+ (long long)getRamSize;
+ (long long)getRomSize;
+ (float)getTotalSpace;
+ (NSString *)getCpuTypeStr;
+ (NSString *)getRosultion;

@end

