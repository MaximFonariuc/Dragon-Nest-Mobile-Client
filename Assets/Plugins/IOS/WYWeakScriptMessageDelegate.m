//
//  WYWeakScriptMessageDelegate.m
//  IMYWebView
//
//  Created by wangyangyang on 15/11/17.
//  Copyright © 2015年 IMY. All rights reserved.
//

#import "WYWeakScriptMessageDelegate.h"

@implementation WYWeakScriptMessageDelegate

- (instancetype)initWithDelegate:(id<WKScriptMessageHandler>)scriptDelegate
{
    self = [super init];
    if (self) {
        _scriptDelegate = scriptDelegate;
    }
    return self;
}

#pragma mark - WKScriptMessageHandler

- (void)userContentController:(WKUserContentController *)userContentController didReceiveScriptMessage:(WKScriptMessage *)message
{
    [self.scriptDelegate userContentController:userContentController didReceiveScriptMessage:message];
}


@end
