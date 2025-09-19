//
//  MSDKBugly.h
//  UMSDK
//
//  Created by SherylPang on 2023/8/16.
//  Copyright © 2023 Tencent. All rights reserved.
//

#ifndef MSDKBugly_h
#define MSDKBugly_h

#import <Foundation/Foundation.h>
#import "MSDKEnums.h"
#import <string>
@interface MSDKBugly : NSObject

+(id)sharedInstance;

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
- (std::string)onMSDKInnerCrashExtMessageNotify;
- (void)onMSDKInnerCrashNotify:(int)crashType errType:(std::string)errType errMsg:(std::string)errMsg errStack:(std::string)errStack;
@end

#endif /* MSDKBugly_h */
