//
//  PandoraWebViewController.m
//  Unity-iPhone
//
//  Created by stuartwang on 16/8/8.
//
//

#import "PandoraWebViewController.h"
#import <JavaScriptCore/JavaScriptCore.h>

@implementation PandoraWebViewController
@synthesize webview, ticket, callbackObjName, verbose;
static const NSString *WebviewBuildVer = @"1.0";
static PandoraWebViewController* m_pInst;
-(id)init{
    self = [super init];
    if (self != nil) {
        NSLog(@"pandoraWebview ver %@ init",WebviewBuildVer);
        m_pInst = self;
        x = y = width = height = -1;
        activated = false;
        webview = [[UIWebView alloc] init];
        webview.delegate = self;
        webview.scrollView.bounces = NO;
        webview.allowsInlineMediaPlayback = YES;
        ticket = @"\"\"";
        verbose = NO;
    }
    return self;
}

+(PandoraWebViewController *)getInstance{
    if (m_pInst == nil) {
        m_pInst = [[PandoraWebViewController alloc]init];
        
    }
    return m_pInst;
}

-(void)openURL:(NSString *)url{
    if (verbose) {
        NSLog(@"pandoraWebview openURL:%@", url);
    }
    
    [self.getTopView addSubview:webview];
    [webview loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:url]]];
    activated = true;
    
}

-(void)setPosition:(int)off_x y:(int)off_y width:(int)w height:(int)h color:(UIColor*)bgColor{
    self->x = off_x;
    self->y = off_y;
    
    if (w > h) {
        self->width = w;
        self->height = h;
    }else{
        if (verbose) {
            NSLog(@"pandoraWebview rect reverse! Width %d ,height %d", w, h);
        }
        self->width = h;
        self->height = w;
    }
    CGRect rect = CGRectMake(x, y, self->width, self->height);
    webview.frame = rect;
    webview.backgroundColor = bgColor;
    webview.opaque = NO;//背景不透明设置为NO
}

-(BOOL)isShow{
    return activated;
}

-(void)Close{
    if (verbose) {
        NSLog(@"pandoraWebview Close");
    }
    
    if (webview.superview) {
        if (verbose) {

            NSLog(@"pandoraWebview Start load empty page");
        }
        [webview loadHTMLString:@"" baseURL:nil];
//        [webview stopLoading];
        if (verbose) {
            NSLog(@"pandoraWebview RemoveFromSuperview");
        }
        [webview removeFromSuperview];
        
        if (verbose) {
            NSLog(@"pandoraWebview Close and remove cache");
        }
        // remore cache
        [[NSURLCache sharedURLCache] removeAllCachedResponses];
        [[NSURLCache sharedURLCache] setDiskCapacity:0];
        [[NSURLCache sharedURLCache] setMemoryCapacity:0];
        
        [[NSUserDefaults standardUserDefaults] setInteger:0 forKey:@"WebKitCacheModelPreferenceKey"];
        [[NSUserDefaults standardUserDefaults] setBool:NO forKey:@"WebKitDiskImageCacheEnabled"];
        [[NSUserDefaults standardUserDefaults] setBool:NO forKey:@"WebKitOfflineWebApplicationCacheEnabled"];
        // remove cookie
        //    NSHTTPCookieStorage *storage = [NSHTTPCookieStorage sharedHTTPCookieStorage];
        //    for (NSHTTPCookie *cookie in [storage cookies]) {
        //        [storage deleteCookie:cookie];
        //    }
        [[NSUserDefaults standardUserDefaults] synchronize];
    }
    activated = false;
}

-(void)OnDestroy{
    [self Close];
}

//游戏向潘多拉发送消息
-(void)sendMsgTojs:(const char*)msg{
    NSString *strMsg = [NSString stringWithUTF8String:msg];
    [self performSelectorOnMainThread:@selector(mainThreadSendMsg:) withObject:strMsg waitUntilDone:NO];
}

-(void)mainThreadSendMsg:(NSString *)msg{
    if (verbose) {
        NSLog(@"pandoraWebview sendMsgTojs: %@", msg);
    }
    
    NSLog(@"use stringByEvaluatingJavaScriptFromString");
    [self.webview stringByEvaluatingJavaScriptFromString:[NSString stringWithFormat:@"javascript:(function(w){if(w.pandora.onMessage){console.log(\"pandoraWebview pandora.onMessage Exist\");w.pandora.onMessage(JSON.parse('%@'));}else{console.log(\"pandoraWebview [Error] pandora.onMessage Not Exist\")}})(window);", msg]];
}

#pragma mark - delegate
- (void)webViewDidFinishLoad:(UIWebView *)webView{
    if (verbose) {
        NSLog(@"pandoraWebview webViewDidFinishLoad: %@",webView.request.URL.absoluteString);
        NSLog(@"pandoraWebview UserAgent = %@", [webView stringByEvaluatingJavaScriptFromString:@"navigator.userAgent"]);
    }
        
    JSContext *jscontext = [webview valueForKeyPath:@"documentView.webView.mainFrame.javaScriptContext"];
    //向js中注入信息
    jscontext.exceptionHandler = ^(JSContext *context, JSValue *exceptionValue) {
        context.exception = exceptionValue;
        if (verbose) {
            NSLog(@"pandoraWebview 异常信息JSwebViewDidFinishLoad：%@", exceptionValue);
        }
    };
    [jscontext evaluateScript:[NSString stringWithFormat:@"javascript:(function(w) {w.pandora = {'info': %@ ,'onMessage':null};})(window);", self.ticket]];//潘多拉初始化的w.pandora.info
    //注入回调（pandora.sendMessage是js拉向游戏发消息）
    jscontext[@"pandora"][@"sendMessage"] = ^() {
        //取参数，生成json
        NSArray *args = [JSContext currentArguments];
        NSError* error;
        
        NSString *retJsString = nil;
        if([args[0] isObject]){
            if(![NSJSONSerialization isValidJSONObject:[args[0] toDictionary]]){
                if (verbose) {
                    NSLog(@"pandoraWebview JS Ret can't Serialize to Json:%@", [args[0] toDictionary]);
                }
                return;
            }
            NSData* jsonData = [NSJSONSerialization dataWithJSONObject:[args[0] toDictionary] options:0 error:&error];
            retJsString = [[NSString alloc ]initWithData:jsonData encoding:NSUTF8StringEncoding];
        }else if([args[0] isString]){
            retJsString = [NSString stringWithFormat:@"%@",args[0]];
        }else if([args[0] isBoolean]){
            retJsString = [NSString stringWithFormat:@"%@",args[0]];
        }else if([args[0] isNumber]){
            retJsString = [NSString stringWithFormat:@"%@",args[0]];
        }else if([args[0] isNull]){
            NSLog(@"pandoraWebview Js send message: Log content is null");
            return;
        }
        
        if (verbose) {
            NSLog(@"pandoraWebview Js send message:%@", retJsString);
        }
        
        //回传游戏
        dispatch_async(dispatch_get_main_queue(), ^{
            UnitySendMessage([callbackObjName UTF8String], "OnPageMessage", [retJsString UTF8String]);
        });
    };
    
    jscontext[@"console"][@"log"] = ^() {
        NSArray *args = [JSContext currentArguments];
        NSError* error;
        NSString *retJsString = nil;
        for (int i = 0; i < args.count; i++) {
            if([args[i] isObject]){
                if(![NSJSONSerialization isValidJSONObject:[args[i] toDictionary]]){
                    if (verbose) {
                        NSLog(@"%@", [args[i] toDictionary]);
                    }
                    return ;
                };
                NSData* jsonData = [NSJSONSerialization dataWithJSONObject:[args[i] toDictionary] options:0 error:&error];
                retJsString = [[NSString alloc ]initWithData:jsonData encoding:NSUTF8StringEncoding];
            }else if([args[i] isString]){
                retJsString = [NSString stringWithFormat:@"%@",args[i]];
            }else if([args[i] isBoolean]){
                retJsString = [NSString stringWithFormat:@"%@",args[i]];
            }else if([args[i] isNumber]){
                retJsString = [NSString stringWithFormat:@"%@",args[i]];
            }else if([args[i] isNull]){
                return;
            }
            
            if (verbose) {
                NSLog(@"JsLog:%@", retJsString);
            }
        }
    };
    
    //通知js, 在JS中判断PandoraInitialized方法是否存在，存在则调用一下
    [jscontext evaluateScript:@"javascript:(function(w){if(typeof(PandoraInitialized)==='function'){PandoraInitialized();}})(window);"];//执行一次使json转为js成员

    //通知游戏
    if (![@"about:blank"  isEqual: webView.request.URL.absoluteString]) {
        UnitySendMessage([callbackObjName UTF8String], "OnPageLoaded", "");
    }
    
    if (verbose) {
        NSLog(@"pandoraWebview webViewDidFinishLoad already finish!");
    }
    
}

#pragma mark - utility mehtod
- (UIView *)getTopView {
    UIView *view;
    UIViewController *controller = [UIApplication sharedApplication].keyWindow.rootViewController;
    if (controller == nil) {
        view = [[UIApplication sharedApplication]delegate].window;
    } else {
        view = controller.view;
    }
    return view;
}

-(void)setVerbose:(BOOL)v{
    verbose = v;
}
@end
