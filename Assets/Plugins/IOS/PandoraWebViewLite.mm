//
//  IMWebViewLite.m
//  Unity-iPhone
//
//  Created by stuartwang on 16/8/8.
//
//
#import "PandoraWebViewController.h"
#import <Photos/Photos.h>
#import <AssetsLibrary/ALAssetsLibrary.h>

extern "C" {
#pragma mark - Tools
    UIColor* HexToUIColor(char* hexColor){
        if (hexColor == NULL) {
            return UIColor.whiteColor;
        }
        if (hexColor[0] != '#' ) {
            NSLog(@"pandoraWebview colorTrans:%s have't # at begin", hexColor);
            return UIColor.whiteColor;
        }
        if (strlen(hexColor) != 9 ) {
            NSLog(@"pandoraWebview colorTrans:%s color str length not 9", hexColor);
            return UIColor.whiteColor;
        }
        
        long long hexColorLong = strtoll(&hexColor[1], NULL, 16);
        float red =   ((float)((hexColorLong & 0xFF000000) >> 24))/255.0;
        float green = ((float)((hexColorLong & 0xFF0000) >> 16))/255.0;
        float blue =  ((float)((hexColorLong & 0xFF00) >> 8))/255.0;
        float alpha = ((float)( hexColorLong & 0xFF))/255.0;
        if ([PandoraWebViewController getInstance].verbose) {
            NSLog(@"pandoraWebview colorTrans:%s to %f,%f,%f,%f", hexColor, red, green, blue, alpha);
        }
        return [UIColor colorWithRed:red green:green blue:blue alpha:alpha];
    }
    
#pragma mark - UnityBridgeMethod
//#define BELOW_IOS_8 (NSFoundationVersionNumber <= NSFoundationVersionNumber_iOS_7_1)
#define BELOW_IOS_8   ( [[[UIDevice currentDevice] systemVersion] floatValue] < 8.0 )
    void PANDORA_WEBVIEW_OpenUrl(const char* url, int w, int h, int x, int y, char* bgColor, bool delayshow) {
        CGRect rect = [[UIScreen mainScreen] bounds];
        CGSize size = rect.size;
        CGFloat scale = 1;
        if ([[UIScreen mainScreen] respondsToSelector:@selector(nativeScale)]){
            scale = [UIScreen mainScreen].nativeScale;
        }else{
            scale = [UIScreen mainScreen].scale;
        }
        
        //如果传入的X，Y < 0, 那么当做对应坐标居中
//        if(x < 0){
//            x = (size.width * scale - w) / 2;
//        }
//        if(y < 0) {
//            y = (size.height * scale - h) / 2;
//        }
        
        //宽或者高小于0，则当做全屏处理
        if(w < 0 || h < 0)
        {
            w = size.width * scale;
            h = size.height * scale;
        }
        
        if ([PandoraWebViewController getInstance].verbose) {
            NSLog(@"pandoraWebview size : %d, %d, %d, %d", x, y, w, h);
        }
//        bgColor = "#0000ffFF";
        [[PandoraWebViewController getInstance] setPosition:x/scale y: y/scale width:w/scale height:h/scale color:HexToUIColor(bgColor)];
        [[PandoraWebViewController getInstance] openURL:[NSString stringWithUTF8String:url]];
    }
    
    bool PANDORA_WEBVIEW_IsShow(){
        return [[PandoraWebViewController getInstance] isShow];
    }
    
    void PANDORA_WEBVIEW_Close() {
        [[PandoraWebViewController getInstance] Close];
    }
    
    void PANDORA_WEBVIEW_ObjName(const char* name){
        [PandoraWebViewController getInstance].callbackObjName = [NSString stringWithUTF8String:name];
    }
    
    void PANDORA_WEBVIEW_SetTicket(const char *ticket){
        [[PandoraWebViewController getInstance] setTicket:[NSString stringWithUTF8String:ticket]];
    }
    
    void PANDORA_WEBVIEW_WriteMessage(const char *info){
        [[PandoraWebViewController getInstance] sendMsgTojs:info];
    }
    
    void PANDORA_WEBVIEW_OnDestroy(const char *info){
        [[PandoraWebViewController getInstance] OnDestroy];
    }
    
    int PANDORA_WEBVIEW_GetPhysicalScreenWidth(){
        CGSize size = [[UIScreen mainScreen] bounds].size;
        CGFloat retWidth = 0;
        
        if (BELOW_IOS_8) {
            if (UnityCurrentOrientation() == landscapeLeft || UnityCurrentOrientation() == landscapeRight) {
                retWidth = size.height;
            } else {
                retWidth = size.width;
            }
        } else {
            retWidth = size.width;
        }
        
        CGFloat scale = 1;
        if ([[UIScreen mainScreen] respondsToSelector:@selector(nativeScale)]){
            scale = [UIScreen mainScreen].nativeScale;
        }else{
            scale = [UIScreen mainScreen].scale;
        }
        
        return (int)(retWidth * scale);
    }
    
    int PANDORA_WEBVIEW_GetPhysicalScreenHeight(){
        CGSize size = [[UIScreen mainScreen] bounds].size;
        CGFloat retHeight = 0;
        
        if (BELOW_IOS_8) {
            if (UnityCurrentOrientation() == landscapeLeft || UnityCurrentOrientation() == landscapeRight) {
                retHeight = size.width;
            } else {
                retHeight = size.height;
            }
        } else {
            retHeight = size.height;
        }
        
        CGFloat scale = 1;
        if ([[UIScreen mainScreen] respondsToSelector:@selector(nativeScale)]){
            scale = [UIScreen mainScreen].nativeScale;
        }else{
            scale = [UIScreen mainScreen].scale;
        }
        
        return (int)(retHeight * scale);
    }
    
    
    void PANDORA_WEBVIEW_IsShowWebviewScrollIndicator(bool Horizontal, bool Vertical){
        return;
    }
    
    void PANDORA_WEBVIEW_SetVerbose(bool verbose){
        [[PandoraWebViewController getInstance] setVerbose:verbose];
    }
        
    bool iosSavePicToAlbum(const char* path){
        __block NSString *NSPath = [[NSString stringWithUTF8String:path] mutableCopy];
        if (BELOW_IOS_8) {
            //iOS7
            ALAuthorizationStatus author =[ALAssetsLibrary authorizationStatus];
            if(author == ALAuthorizationStatusNotDetermined){
                
                ALAssetsLibrary *assetsLibrary = [[ALAssetsLibrary alloc] init];
                [assetsLibrary enumerateGroupsWithTypes:ALAssetsGroupAll usingBlock:^(ALAssetsGroup *group, BOOL *stop) {
                    if (*stop) {
                        NSLog(@"iosSavePicToAlbum 授权成功");
                        UIImage *image = [[UIImage alloc] initWithContentsOfFile:NSPath];
                        UIImageWriteToSavedPhotosAlbum(image ,nil,nil,nil);
                        UnitySendMessage("Pandora GameObject", "iosSavePhotoToAlbumResponse", "1");
                    }
                    *stop = TRUE;
                } failureBlock:^(NSError *error) {
                    NSLog(@"不允许");
                    UnitySendMessage("Pandora GameObject", "iosSavePhotoToAlbumResponse", "0");
                }];
            }else if(author == ALAuthorizationStatusRestricted || author ==ALAuthorizationStatusDenied){
                //无权限
                NSLog(@"无权限");
                UnitySendMessage("Pandora GameObject", "iosSavePhotoToAlbumResponse", "0");
            }else{
                NSLog(@"iosSavePicToAlbum 已授权");
                UIImage *image = [[UIImage alloc] initWithContentsOfFile:NSPath];
                UIImageWriteToSavedPhotosAlbum(image ,nil,nil,nil);
                UnitySendMessage("Pandora GameObject", "iosSavePhotoToAlbumResponse", "1");
            }
            
        }else{
            //iOS8以上
            PHAuthorizationStatus photoAuthStatus = [PHPhotoLibrary authorizationStatus];
            if (photoAuthStatus == PHAuthorizationStatusNotDetermined) {// 未询问是否授权 可以用下面的请求授权方法询问用户
                NSLog(@"iosSavePicToAlbum 请求授权");
                [PHPhotoLibrary requestAuthorization:^(PHAuthorizationStatus status) {
                    if (status == PHAuthorizationStatusAuthorized) {
                        // 用户同意授权
                        NSLog(@"iosSavePicToAlbum 授权成功");
                        UIImage *image = [[UIImage alloc] initWithContentsOfFile:NSPath];
                        UIImageWriteToSavedPhotosAlbum(image ,nil,nil,nil);
                        UnitySendMessage("Pandora GameObject", "iosSavePhotoToAlbumResponse", "1");
                        
                    }else {
                        // 用户拒绝授权
                        NSLog(@"iosSavePicToAlbum 授权失败");
                        UnitySendMessage("Pandora GameObject", "iosSavePhotoToAlbumResponse", "0");
                    }
                }];
            }else if(photoAuthStatus == PHAuthorizationStatusRestricted || photoAuthStatus == PHAuthorizationStatusDenied) {// 未授权
                NSLog(@"iosSavePicToAlbum 未授权");
                UnitySendMessage("Pandora GameObject", "iosSavePhotoToAlbumResponse", "0");
            }else{// 已授权
                NSLog(@"iosSavePicToAlbum 已授权");
                UIImage *image = [[UIImage alloc] initWithContentsOfFile:NSPath];
                UIImageWriteToSavedPhotosAlbum(image ,nil,nil,nil);
                UnitySendMessage("Pandora GameObject", "iosSavePhotoToAlbumResponse", "1");
            }
        }
        return true;
    }

}
