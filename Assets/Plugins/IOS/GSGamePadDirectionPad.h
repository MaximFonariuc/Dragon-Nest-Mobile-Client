//
//  GSGamePadDirectionPad.h
//  Gamesir-iOSSDK
//
//  Created by xugelei on 15/12/16.
//  Copyright © 2015年 Guangzhou Xiaojikuaipao Network Technology Co., Ltd. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "GSGamePadButton.h"
#import "GSGamePadAxisInput.h"
@interface GSGamePadDirectionPad : GSGamePadElement
typedef void (^GSGamePadDirectionPadValueChangedHandler)(GSGamePadDirectionPad *dpad, float xValue, float yValue);
@property (nonatomic, copy) GSGamePadDirectionPadValueChangedHandler valueChangedHandler;
@property (nonatomic,strong) GSGamePadButton* up;
@property (nonatomic,strong) GSGamePadButton* down;
@property (nonatomic,strong) GSGamePadButton* left;
@property (nonatomic,strong) GSGamePadButton* right;

/**
 *  x轴坐标，取值范围为-1.0~1.0
 */
@property (nonatomic,strong) GSGamePadAxisInput *xAxis;

/**
 *  y轴坐标，取值范围为-1.0~1.0
 */
@property (nonatomic,strong) GSGamePadAxisInput *yAxis;
@end
