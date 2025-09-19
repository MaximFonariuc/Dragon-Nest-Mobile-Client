//
//  MSDKXG.h
//  UMSDK
//
//  Created by junhui on 2020/4/9.
//  Copyright © 2020年 Tencent. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "MSDKStructs.h"

@interface MSDKXGPush : NSObject
+ (id)getMSDKXGPush;
- (void)registerAPNSPushNotification:(NSDictionary *)dict;
- (void)successedRegisterdAPNSWithToken:(NSData *)data;
- (void)failedRegisteredAPNS;
- (void)cleanBadgeNumber;
- (void)receivedMSGFromAPNSWithDict:(NSDictionary *)userInfo;
    
- (long)localNotification:(NSDate *)fireDate
            alertBody:(NSString *)alertBody
                badge:(int)badge
            alertAction:(NSString *)alertAction
                userInfo:(NSDictionary *)userInfo;
- (long)localNotificationAtFrontEnd:(UILocalNotification *)notification
                    userInfoKey:(NSString *)userInfoKey
                    userInfoValue:(NSString *)userInfoValue;
- (void)delLocalNotification:(NSString *)userInfoKey userInfoValue:(NSString *)userInfoValue;
- (NSString *)xgSdkVersion;
- (void)setPushTag:(NSString *)tag;
- (void)deletePushTag:(NSString *)tag;
- (void)clearLocalNotifications;
- (void)clearLocalNotification:(LocalMessage &)localMessage;
- (long)addLocalNotificationAtFront:(LocalMessage &)localMessage;
- (long)addLocalNotification:(LocalMessage &)localMessage;
- (void)handleMSDKOnLoginSuccessNotify:(NSString *)openId;
- (void)setPushAccount:(NSString *)account;
- (void)deletePushAccount:(NSString *)account;
- (void)unregisterPush;



@end
