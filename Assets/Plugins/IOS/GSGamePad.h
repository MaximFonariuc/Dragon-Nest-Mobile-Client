//
//  GSGamePad.h
//  Gamesir-iOSSDK
//
//  Created by xugelei on 15/12/16.
//  Copyright © 2015年 Guangzhou Xiaojikuaipao Network Technology Co., Ltd. All rights reserved.
//
#import <UIKit/UIKit.h>
#import "GSGamePadButton.h"
#import "GSGamePadAxisInput.h"
#import "GSGamePadDirectionPad.h"
@interface GSGamePad : NSObject

/**
 用于组合键block中
 */
typedef enum compositeButtons {
    compositebuttonA  = 0x001,
    compositebuttonB  = 0x002,
    compositebuttonX  = 0x004,
    compositebuttonY  = 0x008,
    compositebuttonL1 = 0x010,
    compositebuttonR1 = 0x020,
    compositebuttonL2 = 0x040,
    compositebuttonR2 = 0x080,
    compositebuttonL3 = 0x100,
    compositebuttonR3 = 0x200,
} compositeButtons;

/**
 用于摇杆左右震动
 */
typedef enum vibrateSides {
    LEFTVIBRATE,
    RIGHTVIBRATE,
    ALLVIBRATE,
} vibrateSides;

/**
 *  手柄连接状态block
 *
 *  @param connected 是否已连接
 */
typedef void (^GSGamePadValueChangedHandler)(BOOL connected);

/**
 *  组合键block
 *
 *  @param compositeState 组合键枚举 见枚举 compositeButtons
 */
typedef void (^GSGamePadCompositeButtonChangedHandler)(NSInteger compositeState);

// 按键按下的事件
typedef void (^GSGamePadButtonChangedHandler)(GSGamePadButton* btn);

// 摇杆操作的事件
typedef void (^GSGamePadAxisEvenHandler)(GSGamePadAxisInput* axis);

+ (GSGamePad *)sharedGamePad;

- (void)showSettingView;
- (void)showSettingViewAtPoint:(CGPoint)point;

/**
 * 隐藏settingview
 */
- (void)showConnectView;
- (void)hidConnectView;

/**
 * 隐藏悬浮图标
 */
- (void)hiddenGamesirCoverView;
- (void)showGamesirCoverView;

/**
 *  扫描并连接手柄，扫描8秒后若没有发现手柄会自动停止扫描，若连接上了手柄，也会停止扫描。
 */
- (void)scanAndConnectGamePad;

/**
 *  停止扫描
 */
- (void)stopScanGamePad;

/**
 *  断开已连接的手柄
 */
- (void)disConnectGamePad;

/**
 *  让手柄震动
 *
 *  @param vibrateSide 左边震动还是右边震动 vibrateSides 枚举：
 *  LEFTVIBRATE,  左边震动
 *  RIGHTVIBRATE, 右边震动
 *  ALLVIBRATE,   两边都震动
 */
- (void)vibrateWithSides:(vibrateSides)vibrateSide;
/**
 *  让手柄震动
 *
 *  @param leftvib, rightvib, 分别表示左右两边的震动强度（0-255）, time表示震动时间(1-10s)：
 *
 */
- (void)vibrate:(int)leftvib right:(int)rightvib time:(int)time;

/**
 *  手柄是否已连接
 */
@property (nonatomic)  BOOL connected;

/**
 *  定义此block可监控手柄连接还是断开
 */
@property (nonatomic, copy) GSGamePadValueChangedHandler valueChangedHandler;

/**
 *  提供一个悬浮小球，通过点击此小球显示连接断开界面，此小球可移动。
 *
 *  @return 返回的悬浮小球View
 */
- (UIView *)gamesirCoverView;

// 返回当前蓝牙开启状态
- (int )BluetoothState;

/**
 *  定义此block可监控多个按键同时按下时的状态，例如：同时按下A 和 B按键时：
 [GSGamePad sharedGamePad].compositeButtonChangeHandler = ^ (NSInteger compositeState) {
    if (compositeState == (buttonA | buttonB)) {
    NSLog(@"a and b pressed");
    }
 };
 */
@property (nonatomic, copy) GSGamePadCompositeButtonChangedHandler compositeButtonChangeHandler;
@property (nonatomic, copy) GSGamePadButtonChangedHandler buttonEventHandler;
@property (nonatomic, copy) GSGamePadAxisEvenHandler axisEventHandler;
/**

  Y
 / \
X   B
 \ /
  A
 
 */
@property (nonatomic, strong) GSGamePadButton* buttonA;
@property (nonatomic, strong) GSGamePadButton* buttonB;
@property (nonatomic, strong) GSGamePadButton* buttonX;
@property (nonatomic, strong) GSGamePadButton* buttonY;

@property (nonatomic, strong) GSGamePadButton* buttonL1;
@property (nonatomic, strong) GSGamePadButton* buttonR1;

@property (nonatomic, strong) GSGamePadButton* buttonL2;
@property (nonatomic, strong) GSGamePadButton* buttonR2;

/**
 *  左摇杆键按下,仅G2u及以上型号才有
 */
@property (nonatomic, strong) GSGamePadButton* buttonL3;

/**
 *  右摇杆键按下,仅G2u及以上型号才有
 */
@property (nonatomic, strong) GSGamePadButton* buttonR3;

/**
 *  选择键
 */
@property (nonatomic, strong) GSGamePadButton* buttonSelect;
/**
 *  开始键
 */
@property (nonatomic, strong) GSGamePadButton* buttonStart;

/**
 *  电源键，短按触发
 */
@property (nonatomic, strong) GSGamePadButton* buttonHome;

/**
 *  十字键
 */
@property (nonatomic, strong) GSGamePadDirectionPad *dpad;

/**
 *  左摇杆
 */
@property (nonatomic, strong) GSGamePadDirectionPad* leftThumbStick;

/**
 *  右摇杆
 */
@property (nonatomic, strong) GSGamePadDirectionPad* rightThumbStick;

@end
