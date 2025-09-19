//
//  MSDKSensitivity.h
//  UMSDK
//
//  Created by MikeFu on 2020/7/29.
//  Copyright © 2020 Tencent. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface MSDKSensitivity : NSObject

+ (id)getMSDKSensitivity;

- (NSString *)idfaString:(BOOL)trackingEnable;

@end
