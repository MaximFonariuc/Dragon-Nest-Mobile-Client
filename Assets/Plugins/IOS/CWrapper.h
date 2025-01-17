

extern "C" void GamesirSDKInit();
extern "C" void GSGamePadShowSettingView();
extern "C" void GSGamePadConnectGamePad();
extern "C" void GSGamePadDisConnectGamePad();
extern "C" bool GSGamePadbuttonPressed(int button);
extern "C" float GSGamePadLeftThumbStickGetxAxis();
extern "C" float GSGamePadLeftThumbStickGetyAxis();
extern "C" float GSGamePadRightThumbStickGetxAxis();
extern "C" float GSGamePadRightThumbStickGetyAxis();
extern "C" float GSGamePadL2Getz();
extern "C" float GSGamePadR2Getz();

extern "C" void GSGamePadShowSettingViewAtLocation(int location);
extern "C" void showConnectView();
extern "C" void hidConnectView();
extern "C" void hiddenGamesirCoverView();
extern "C" void showGamesirCoverView();

extern "C" bool IsConnected();
