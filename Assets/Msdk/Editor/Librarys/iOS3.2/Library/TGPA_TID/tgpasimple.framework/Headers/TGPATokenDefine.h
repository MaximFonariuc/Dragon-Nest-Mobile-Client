//
//  TGPATokenDefine.h
//  tgpatoken
//
//  Created by zohnzliu(刘专) on 2022/3/29.
//  Copyright © 2022 zohnzliu(刘专). All rights reserved.
//

#ifndef TGPATokenDefine_h
#define TGPATokenDefine_h

#define TGPATOKEN_CALLBACK_KEY_DEVICETOKEN @"DeviceToken"

#define TGPATOKEN_REQUEST_SERIAL_QUEUE "mtgpa_init"

// key: DeviceToken, value为DeviceToken值，标准长度为32位，获取失败返回整形错误码, 详细错误码请参考文档
typedef void (^TGPATokenCallback)(NSString *_Nonnull key, NSString *_Nonnull value);



#endif /* TGPATokenDefine_h */
