//
//  MSDKBuglyWrapper.h
//  UMSDK
//
//  Created by MikeFu on 2017/2/16.
//  Copyright © 2017年 Tencent. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MSDK/MSDKBugly.h"
#import "MSDK/MSDKObjectExtension.h"
#import "MSDK/MSDKLocalConfig.h"


@interface MSDKBuglyWrapper : MSDKBugly

+ (id)sharedInstance;

- (void)initBugly:(NSString *)appid openId:(NSString *)openId channelId:(NSString *)channelId qIMEI:(NSString *)qIMEI;

- (NSString *)getBuglyVersion;

- (void)setLoginStateToBuglySDK:(NSString *)openId;

- (void)closeCrashReport;

- (void)buglyLog:(eBuglyLogLevel)level log:(NSString *)log;

- (void)setGameStatus:(NSString *)gameStatus statusKey:(NSString *)statusKey;

- (void)removeGameStatus:(NSString *)statusKey;

- (void)reportException:(NSUInteger)exceptionType
          exceptionName:(NSString *)exceptionName
           exceptionMsg:(NSString *)exceptionMsg
         exceptionStack:(NSString *)exceptionStack
                extInfo:(NSDictionary *)extInfo;

@end
