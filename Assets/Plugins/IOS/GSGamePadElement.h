//
//  GSGamePadElement.h
//  Gamesir-iOSSDK
//
//  Created by xugelei on 15/12/16.
//  Copyright © 2015年 Guangzhou Xiaojikuaipao Network Technology Co., Ltd. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef enum keycode {
    BTN_A = 1 << 0,
    BTN_B = 1 << 1,
    BTN_X = 1 << 2,
    BTN_Y = 1 << 3,
    BTN_L1 = 1 << 4,
    BTN_L2 = 1 << 5,
    BTN_R1 = 1 << 6,
    BTN_R2 = 1 << 7,
    // L2,R2的模拟量
    AXIS_RTRIGGER = 1 << 8,
    AXIS_LTRIGGER = 1 << 9,
    // dpad
    DPAD_UP = 1 << 10,
    DPAD_DOWN = 1 << 11,
    DPAD_LEFT = 1 << 12,
    DPAD_RIGHT = 1 << 13,
    // dpad的模拟量
    AXIS_HAT_X = 1 << 14,
    AXIS_HAT_Y = 1 << 15,
    
    // 左摇杆
    AXIS_X = 1 << 16,  // in sdk named L3D_X
    AXIS_Y = 1 << 17,
    BTN_THUMBL = 1 << 18,
    // 右摇杆
    AXIS_Z = 1 << 19,  // // in sdk named R3D_Z
    AXIS_RZ = 1 << 20,
    BTN_THUMBR = 1 << 21,
    
    BTN_SELECT = 1 << 22,
    BTN_START = 1 << 23,
    
    BTN_L3 = 1 << 24,
    BTN_R3 = 1 << 25,
    BTN_HOME = 1 << 26
} KeyCode;


@interface GSGamePadElement : NSObject

@end
