//
//  GSGamePadButton.h
//  Gamesir-iOSSDK
//
//  Created by xugelei on 15/12/16.
//  Copyright © 2015年 Guangzhou Xiaojikuaipao Network Technology Co., Ltd. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "GSGamePadElement.h"


@class GSGamePadButton;
typedef void (^GSGamePadButtonValueChangedHandler)(GSGamePadButton *button, float value, BOOL pressed);
@interface GSGamePadButton : GSGamePadElement

-(id)initWithCode:(KeyCode)code;

/**
 *  除L2  R2按键（因L2 R2有模拟量），其它按键均为按下或弹起时调用一次。
 */
@property (nonatomic, copy) GSGamePadButtonValueChangedHandler pressedChangedHandler;

@property (nonatomic) KeyCode code;

/**
 *  按键是否按下
 */
@property (nonatomic) BOOL pressed;

/**
 *  按键压力等级，0.0~1.0
 */
@property (nonatomic) float value;

@end
