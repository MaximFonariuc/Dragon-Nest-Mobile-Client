//
//  WYWeakScriptMessageDelegate.h
//  IMYWebView
//
//  Created by wangyangyang on 15/11/17.
//  Copyright © 2015年 IMY. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <WebKit/WKScriptMessageHandler.h>

@interface WYWeakScriptMessageDelegate : NSObject <WKScriptMessageHandler>

@property (nonatomic, weak) id<WKScriptMessageHandler> scriptDelegate;

- (instancetype)initWithDelegate:(id<WKScriptMessageHandler>)scriptDelegate;

@end
