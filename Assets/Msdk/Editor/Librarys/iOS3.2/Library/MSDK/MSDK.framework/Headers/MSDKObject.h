//
//  MSDKObject.h
//  MSDKFoundation
//
//  Created by fu chunhui on 14-11-18.
//  Copyright (c) 2014年 Tencent. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MSDKStructs.h"

@interface MSDKObject : NSObject

@end

@interface MSDKRealNameAuthRet : MSDKObject

@property (nonatomic, assign) int flag;                       //0成功
@property (nonatomic, assign) int errorCode;
@property (nonatomic, assign) int platform;
@property (nonatomic, copy) NSString *desc;
- (instancetype)initWithRet:(RealNameAuthRet &)ret;
- (void)getRet:(RealNameAuthRet &)ret;

@end

