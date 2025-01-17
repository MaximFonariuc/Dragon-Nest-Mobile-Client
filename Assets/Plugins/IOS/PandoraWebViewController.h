//
//  PandoraWebViewController.h
//  Unity-iPhone
//
//  Created by stuartwang on 16/8/8.
//
//

#import <UIKit/UIKit.h>
#define PANDORA_WEBVIEW_VER 1.0

@interface PandoraWebViewController : UIViewController<UIWebViewDelegate> {
    int x;
    int y;
    int width;
    int height;
    bool activated;
}
@property (nonatomic, retain) UIWebView *webview;
@property (nonatomic, retain) NSString* ticket;
@property (nonatomic, retain) NSString* callbackObjName;
@property BOOL verbose;
+(PandoraWebViewController *)getInstance;

-(void)openURL:(NSString *)url;

-(void)setPosition:(int)off_x y:(int)off_y width:(int)w height:(int)h color:(UIColor*)bgColor;

-(void)setVerbose:(BOOL)v;

-(BOOL)isShow;

-(void)Close;

-(void)OnDestroy;

-(void)sendMsgTojs:(const char*)msg;
@end
