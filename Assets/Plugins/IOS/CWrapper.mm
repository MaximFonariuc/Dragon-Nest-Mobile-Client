#import "CWrapper.h"
#import "GSGamePad.h"
extern UIViewController *UnityGetGLViewController();
enum buttons {
    buttonA,
    buttonB,
    buttonX,
    buttonY,
    buttonL1,
    buttonR1,
    buttonL2,
    buttonR2,
    buttonL3,
    buttonR3,
    buttonLeft,
    buttonRight,
    buttonDown,
    buttonUp,
    buttonSelect,
    buttonStart,
    buttonHome
};

void GamesirSDKInit() {
    [GSGamePad sharedGamePad];
}

void GSGamePadShowSettingView(){
    printf("show setting\n");
    UIViewController *rootViewController = UnityGetGLViewController();
    UIView *view = [[GSGamePad sharedGamePad] gamesirCoverView];
   // view.backgroundColor = [UIColor blackColor];
    printf("gamesir button view x:%f, y:%f, width:%f, height:%f\n", view.frame.origin.x, view.frame.origin.y, view.frame.size.width, view.frame.size.height);
    [rootViewController.view addSubview:view];
}

void GSGamePadShowSettingViewAtLocation(int location){
    float width = [UIScreen mainScreen].bounds.size.width;
    float height = [UIScreen mainScreen].bounds.size.height;
    int statusBarOrientation = [UIApplication sharedApplication].statusBarOrientation;
    if ((statusBarOrientation == 3 || statusBarOrientation == 4) && width < height) {  // ios7的系统在横屏时，width也小于height
        float temp = width;
        width = height;
        height = temp;
    }
    printf("width:%f, height:%f\n, location:%d\n", width, height, location);
    int offset = 30;
    CGPoint point = CGPointMake(offset, offset);
    switch (location) {
        case 1:  {// top left
            point = CGPointMake(offset, offset);
            [[GSGamePad sharedGamePad] showSettingViewAtPoint:point];
            break;
        }
        case 2:  {// top center
            point = CGPointMake(width/2, offset);
            [[GSGamePad sharedGamePad] showSettingViewAtPoint:point];
            break;
        }
        case 3:  {// top right
            point = CGPointMake(width-offset, offset);
            [[GSGamePad sharedGamePad] showSettingViewAtPoint:point];
            break;
        }
        case 4:  {// center left
            point = CGPointMake(offset, height/2);
            [[GSGamePad sharedGamePad] showSettingViewAtPoint:point];
            break;
        }
        case 5:  {// center right
            point = CGPointMake(width-offset, height/2);
            [[GSGamePad sharedGamePad] showSettingViewAtPoint:point];
            break;
        }
        case 6:  {// bottom left
            point = CGPointMake(offset, height-offset);
            [[GSGamePad sharedGamePad] showSettingViewAtPoint:point];
            break;
        }
        case 7: { // bottom center
            point = CGPointMake(width/2, height-offset);
            [[GSGamePad sharedGamePad] showSettingViewAtPoint:point];
            break;
        }

        case 8: { // bottom right
            point = CGPointMake(width-offset, height-offset);
            [[GSGamePad sharedGamePad] showSettingViewAtPoint:point];
            break;
        }

        default: {
            point = CGPointMake(offset, offset);
            [[GSGamePad sharedGamePad] showSettingViewAtPoint:point];
            break;
        }
    }
}


void showConnectView(){
    [[GSGamePad sharedGamePad] showConnectView];
}

void hidConnectView(){
    [[GSGamePad sharedGamePad] hidConnectView];
}


void hiddenGamesirCoverView() {
    [[GSGamePad sharedGamePad] hiddenGamesirCoverView];
}
void showGamesirCoverView() {
    [[GSGamePad sharedGamePad] showGamesirCoverView];
}

bool IsConnected() {
    return [GSGamePad sharedGamePad].connected;
}

void GSGamePadConnectGamePad(){
    printf("GSGamePadConnectGamePad\n");
    [[GSGamePad sharedGamePad] scanAndConnectGamePad];
}

void GSGamePadDisConnectGamePad() {
    [[GSGamePad sharedGamePad] disConnectGamePad];
}


bool GSGamePadbuttonPressed(int button){
    switch (button) {
        case buttonA:
            return [GSGamePad sharedGamePad].buttonA.pressed;
            break;
        case buttonB:
            return [GSGamePad sharedGamePad].buttonB.pressed;
            break;
        case buttonX:
            return [GSGamePad sharedGamePad].buttonX.pressed;
            break;
        case buttonY:
            return [GSGamePad sharedGamePad].buttonY.pressed;
            break;
        case buttonL1:
            return [GSGamePad sharedGamePad].buttonL1.pressed;
            break;
        case buttonR1:
            return [GSGamePad sharedGamePad].buttonR1.pressed;
            break;
        case buttonL2:
            return [GSGamePad sharedGamePad].buttonL2.pressed;
            break;
        case buttonR2:
            return [GSGamePad sharedGamePad].buttonR2.pressed;
            break;
        case buttonL3:
            return [GSGamePad sharedGamePad].buttonL3.pressed;
            break;
        case buttonR3:
            return [GSGamePad sharedGamePad].buttonR3.pressed;
            break;
        case buttonLeft:
            return [GSGamePad sharedGamePad].dpad.left.pressed;
            break;
        case buttonRight:
            return [GSGamePad sharedGamePad].dpad.right.pressed;
            break;
        case buttonDown:
            return [GSGamePad sharedGamePad].dpad.down.pressed;
            break;
        case buttonUp:
            return [GSGamePad sharedGamePad].dpad.up.pressed;
            break;
        case buttonSelect:
            return [GSGamePad sharedGamePad].buttonSelect.pressed;
            break;
        case buttonStart:
            return [GSGamePad sharedGamePad].buttonStart.pressed;
            break;
        case buttonHome:
            return [GSGamePad sharedGamePad].buttonHome.pressed;
            break;
        default:
            break;
    }
    return NO;
}
float GSGamePadLeftThumbStickGetxAxis(){
    return [GSGamePad sharedGamePad].leftThumbStick.xAxis.value;
}
float GSGamePadLeftThumbStickGetyAxis(){
    return [GSGamePad sharedGamePad].leftThumbStick.yAxis.value;
}
float GSGamePadRightThumbStickGetxAxis(){
    return [GSGamePad sharedGamePad].rightThumbStick.xAxis.value;
}
float GSGamePadRightThumbStickGetyAxis(){
    return [GSGamePad sharedGamePad].rightThumbStick.yAxis.value;
}
float GSGamePadL2Getz(){
    return [GSGamePad sharedGamePad].buttonL2.value;
}
float GSGamePadR2Getz(){
    return [GSGamePad sharedGamePad].buttonR2.value;
}




