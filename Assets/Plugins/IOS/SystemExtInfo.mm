
#import <Foundation/Foundation.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <CoreTelephony/CTCarrier.h>
#import <sys/utsname.h>


@interface SystemExtInfo : NSObject
@end

@implementation SystemExtInfo

#if defined (__cplusplus)
extern "C" 
{
#endif
    

    //获取像素密度
    int GetDensity()
    {
    	//NSLog(@"GetDensity---------");
        struct utsname systemInfo;
        uname(&systemInfo);
        NSString* platform=[NSString stringWithCString:systemInfo.machine encoding:NSASCIIStringEncoding];
        NSLog(@"devices is %@",platform);
        if(UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone)
        {
            if ([platform isEqualToString:@"iPhone1,1"]) return 163;
            if ([platform isEqualToString:@"iPhone1,2"]) return 163;
            if ([platform isEqualToString:@"iPhone2,1"]) return 163;//@"iPhone 3GS";
            if ([platform isEqualToString:@"iPhone7,1"]) return 401;//@"iPhone 6 Plus";
            if ([platform isEqualToString:@"iPhone8,2"]) return 401;//@"iPhone 6s Plus";
            if ([platform isEqualToString:@"iPhone9,2"]) return 401;//@"iPhone 7 Plus";
            if ([platform isEqualToString:@"iPod1,1"])   return 163;//@"iPod Touch 1G";
            if ([platform isEqualToString:@"iPod2,1"])   return 163;//@"iPod Touch 2G";
            if ([platform isEqualToString:@"iPod3,1"])   return 163;//@"iPod Touch 3G";
            return 326;//iphone4 iphone4s iphone5 iphone5s iphone6 iphone7 iphone7s

        }
        else if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
        {
            if ([platform isEqualToString:@"iPad2,1"])   return 132;//@"iPad 2";
            if ([platform isEqualToString:@"iPad2,2"])   return 132;//@"iPad 2";
            if ([platform isEqualToString:@"iPad2,3"])   return 132;//@"iPad 2";
            if ([platform isEqualToString:@"iPad2,4"])   return 132;//@"iPad 2";
            if ([platform isEqualToString:@"iPad2,5"])   return 163;//@"iPad Mini 1G";
            if ([platform isEqualToString:@"iPad2,6"])   return 163;//@"iPad Mini 1G";
            if ([platform isEqualToString:@"iPad2,7"])   return 163;//@"iPad Mini 1G";
            if ([platform isEqualToString:@"iPad4,4"])   return 326;//@"iPad Mini 2G";
            if ([platform isEqualToString:@"iPad4,5"])   return 326;//@"iPad Mini 2G";
            if ([platform isEqualToString:@"iPad4,6"])   return 326;//@"iPad Mini 2G";
            if ([platform isEqualToString:@"iPad4,7"])   return 326;//@"iPadmini3";
            if ([platform isEqualToString:@"iPad4,8"])   return 326;//@"iPadmini3";
            if ([platform isEqualToString:@"iPad4,9"])   return 326;//@"iPadmini3";
            if ([platform isEqualToString:@"iPad5,1"])   return 326;//@"iPadmini4";
            if ([platform isEqualToString:@"iPad5,2"])   return 326;//@"iPadmini4";

            return 264; //ipad3,ipad4,ipad air,ipad air2
        }
        
        return 163*2;
    }

    
    char* MakeStrCopy (const char* string)
    {
        if (string == NULL)return NULL;
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        return res;
        
    }

    
    const char* TransferCode(NSString* pre)
    {
        char* str=MakeStrCopy("");
        if([pre isEqualToString:@"中国移动"])
        {
            str=MakeStrCopy("MM");
        }
        else if ([pre isEqualToString:@"中国联通"])
        {
            str=MakeStrCopy("UN");
        }
        else if ([pre isEqualToString:@"中国电信"])
        {
            str=MakeStrCopy("DX");
        }
        return str;
    }
    
    //获取运营商
    const char* CheckSIM()
    {
        CTTelephonyNetworkInfo *telephonyInfo = [[CTTelephonyNetworkInfo alloc] init];
        CTCarrier *nsstr = [telephonyInfo subscriberCellularProvider];
        NSLog(@"CheckSIM  %@",nsstr.carrierName);
       // return MakeStrCopy([nsstr.carrierName UTF8String]);
        return TransferCode(nsstr.carrierName);
    }

#if defined (__cplusplus)  
}
#endif



@end
