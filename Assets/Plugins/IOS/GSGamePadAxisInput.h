//
//  GSGamePadAxisInput.h
//  Gamesir-iOSSDK
//
//  Created by xugelei on 15/12/16.
//  Copyright © 2015年 Guangzhou Xiaojikuaipao Network Technology Co., Ltd. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "GSGamePadElement.h"

@interface GSGamePadAxisInput : GSGamePadElement
typedef void (^GSGamePadAxisValueChangedHandler)(GSGamePadAxisInput *axis, float value);
@property (nonatomic, copy) GSGamePadAxisValueChangedHandler valueChangedHandler;

-(id)initWithCode:(KeyCode)code;

@property (nonatomic) KeyCode code;
/**
 *  取值范围为-1.0~1.0
 */
@property (nonatomic) float value;
@end
