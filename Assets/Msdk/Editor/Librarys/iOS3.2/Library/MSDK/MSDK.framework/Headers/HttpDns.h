//
//  HttpDns.h
//  HttpDns
//
//  Created by Coolcao on 2016/11/8.
//  Copyright (c) 2016 Tencent. All rights reserved.
//
#ifndef __HttpDns_H__
#define __HttpDns_H__

#import <Foundation/Foundation.h>

@interface HttpDns : NSObject

//版本号 HttpDns_Version = @"0.0.7i";

+ (id) sharedInstance;

/**
 同步解析接口

 @param domain 待解析的域名
 @return 查询到的IP数组，超时（2s）或者未未查询到返回[0,0]数组
 */
- (NSArray *) HttpDnsGetHostByName:(NSString *) domain;

/**
 异步解析接口

 @param domain 待解析的域名
 @param handler 查询到的IP数组，超时（2s）或者未未查询到返回[0,0]数组
 */
- (void) HttpDnsGetHostByNameAsync:(NSString *) domain returnIps:(void (^)(NSArray * ipsArray))handler;

/**
 初始化设置

 @param appid 业务appid
 @param timeout 超时时间，单位：s
 @param enabled debug日志开关，YES:打开 NO:关闭
 */
- (void) HttpDnsSetAppID:(NSString *) appid TimeOut:(float) timeout OpenLog:(BOOL) enabled;

/**
 设置用户openid
 
 @param openid 用户openid
 
 @return YES:成功 NO:失败
 */
- (BOOL) HttpDnsSetOpenID:(NSString *) openid;

@end
#endif
