//
//  TGPATokenManager.h
//
//  Created by zohnzliu(刘专) on 2022/03/29.
//  Copyright © 2022 tgpa. All rights reserved.
//
#import <Foundation/Foundation.h>

#import "TGPATokenDefine.h"

#ifndef TGPATokenManager_h
#define TGPATokenManager_h



/// TGPA iOS XID对外接口
@interface TGPATokenManager : NSObject

- (nonnull instancetype)init NS_UNAVAILABLE;

/// 获取SDK版本号
+ (NSInteger)getVersionCode;

/// 获取SDK版本名称
+ (nonnull NSString *)getVersionName;

/// 设置debug日志是否开启
/// @param enable  YES: 开启; NO: 关闭;
+ (void)setLogAble:(BOOL)enable;

/// 设置开启测试环境，需要在初始化接口前调用
+ (void)enableDebugMode;

/// 初始化, 异步逻辑
/// @param tgpaid  业务标识
+ (void)init:(nonnull NSString *)tgpaid DEPRECATED_ATTRIBUTE;

/// 初始化，异步逻辑，回调中返回数据
/// @param tgpaid  TGPA业务标识, 注册请联系@ericqtzhang/aydensun
/// @param callback  回调函数
+ (void)init:(nonnull NSString *)tgpaid theCallback:(nullable TGPATokenCallback)callback;

/// 同步获取DEVICETOKEN，耗时接口，请勿在主线程调用
+ (nonnull NSString *)getDeviceToken;

@end

#endif /* TGPAManager_h */
