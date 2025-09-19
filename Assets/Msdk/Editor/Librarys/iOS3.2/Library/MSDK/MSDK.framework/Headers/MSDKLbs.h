//
//  MSDKLbs.h
//  UMSDK
//
//  Created by MikeFu on 2019/8/14.
//  Copyright © 2019 Tencent. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef void(*MSDKLbsHandler)(int type, int flag, double longitude, double latitude);

@interface MSDKLbs : NSObject

+ (id)getMSDKLbs;

- (void)getLocationInfo:(int)type compileHandler:(MSDKLbsHandler)compileHandler;

@end
