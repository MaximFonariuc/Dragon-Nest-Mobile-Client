using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ConfigSettings))]
public class ConfigEditor : Editor
{

	private ConfigSettings instance;
    /*
	bool showAndroid = (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android);
#if UNITY_5
	bool showIOSSettings = (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS);
#else
	bool showIOSSettings = (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone);
#endif
    */
    GUIContent msdkUrlDesc = new GUIContent("MSDK_ENV[?]", "MSDK环境选择，联调时可选择测试环境，正式发布时务必选择正式环境");
	GUIContent wxRefreshDesc = new GUIContent("WXTOKEN_REFRESH[?]", "MSDK微信自动刷新Token开关");
	GUIContent pushDesc = new GUIContent("PUSH[?]", "信鸽推送功能开关");
	GUIContent noticeDesc = new GUIContent("needNotice[?]", "MSDK公告功能开关");
	GUIContent noticeTimeDesc = new GUIContent("noticeTime[?]", "MSDK公告系统公告数据更新时间(连网请求公告数据), 单位为分钟(最小值为5分钟)");
    GUIContent realNameAuthDesc = new GUIContent("realNameAuth[?]", "实名认证配置, 0:使用MSDK实名认证UI，认证后回到登录页;\n" +
        "1:使用MSDK实名认证UI，认证后登录进入游戏;\n2:使用游戏自定义认证UI，认证后回调到游戏;");
	GUIContent betaDesc = new GUIContent("BETA[?]", "应用宝抢号功能开关");
	GUIContent statLogDesc = new GUIContent("STAT_LOG[?]", "灯塔和MTA日志开关，设为true时会打印详细日志，正式上线时建议设为false");
	GUIContent closeBuglyDesc = new GUIContent("CLOSE_BUGLY_REPORT[?]", "关闭bugly上报开关，设为true即关闭了crash上报功能");
	GUIContent localLogDesc = new GUIContent("localLog[?]", "MSDKLog开关 0：不打印；1：logcat打印；2：打印到本地文件；" +
	                                         "3：logcat和本地文件同时打印；本地打印的保存在SDCard/MSDK/msdk.log");
    GUIContent saveUpdateDesc = new GUIContent("SAVE_UPDATE[?]", "应用宝省流量下载更新功能开关");
    GUIContent grayTestDesc = new GUIContent("GRAY_TEST_SWITCH[?]", "白名单限号开关，预约抢号需向协同规划组申请");
    GUIContent checkConfigDesc = new GUIContent("Check Config[?]", "使用Unity打包Android/iOS时检查MSDK相关配置是否正确");

    GUIContent webviewConfigDesc = new GUIContent("Webview Config", "MSDK内置浏览器配置");
    GUIContent webveiwPHDesc = new GUIContent("    Portrait Hideable[?]", "浏览器竖屏时，导航栏是否可滑动隐藏");
    GUIContent webveiwLHDesc = new GUIContent("    Landscape Hideable[?]", "浏览器横屏时，导航栏是否可滑动隐藏");

    GUIContent v2SignDesc = new GUIContent("V2SIGNING_ENABLED[?]", "V2签名开关，启用v2签名的包一定要打开这个开关（不开会导致渠道号读取错误），不填默认关闭");

    GUIContent xgOtherPushEnableDesc = new GUIContent("XG_OTHER_PUSH_ENABLE[?]", "信鸽厂商渠道开关");
    GUIContent xgOtherPushXiaomiAppidDesc = new GUIContent("    XG_OTHER_PUSH_XIAOMI_APPID[?]", "小米渠道 appid");
    GUIContent xgOtherPushXiaomiAppkeyDesc = new GUIContent("    XG_OTHER_PUSH_XIAOMI_APPKEY[?]", "小米渠道 appkey");
    GUIContent xgOtherPushMeizuAppidDesc = new GUIContent("    XG_OTHER_PUSH_MEIZU_APPID[?]", "魅族渠道 appid");
    GUIContent xgOtherPushMeizuAppkeyDesc = new GUIContent("    XG_OTHER_PUSH_MEIZU_APPKEY[?]", "魅族渠道 appkey");

	public override void OnInspectorGUI() {
		instance = (ConfigSettings)target;

		CommonInfoGUI ();
	}

	private void CommonInfoGUI() {
		EditorGUILayout.HelpBox("1) Common config (android and iOS)", MessageType.None);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(msdkUrlDesc);
		instance.SetMsdkUrl (EditorGUILayout.Popup(instance.MsdkUrlIndex, instance.msdkUrlEnvs));
        EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space ();

		instance.SetPush (EditorGUILayout.Toggle (pushDesc, instance.Push));
        EditorGUILayout.Space();
		instance.SetWxRefresh (EditorGUILayout.Toggle(wxRefreshDesc, instance.WxRefresh));
        EditorGUILayout.Space();
		instance.SetNeedNotice (EditorGUILayout.Toggle (noticeDesc, instance.NeedNotice));
        EditorGUILayout.Space();

		int num;
		if (!System.Int32.TryParse(instance.NoticeTime, out num)) {
			EditorGUILayout.HelpBox("Invalid noticeTime, need a number", MessageType.Error);
		}
        EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField (noticeTimeDesc);
		instance.SetNoticeTime(EditorGUILayout.TextField(instance.NoticeTime));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.Space ();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(realNameAuthDesc);
        instance.SetRealNameAuth(EditorGUILayout.TextField(instance.RealNameAuth));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField(webviewConfigDesc);
        instance.SetWebviewPortraitHideable(EditorGUILayout.Toggle(webveiwPHDesc, instance.WebviewPortraitHideable));
        instance.SetWebviewLandscapeHideable(EditorGUILayout.Toggle(webveiwLHDesc, instance.WebviewLandscapeHideable));
        EditorGUILayout.Space();

		EditorGUILayout.HelpBox("2) Android special config", MessageType.None);
		instance.SetBeta (EditorGUILayout.Toggle (betaDesc, instance.Beta));
        EditorGUILayout.Space();
		instance.SetStatLog (EditorGUILayout.Toggle (statLogDesc, instance.StatLog));
        EditorGUILayout.Space();
        instance.SetCloseBugly (EditorGUILayout.Toggle (closeBuglyDesc, instance.CloseBugly));
        EditorGUILayout.Space();
        instance.SetSaveUpdate (EditorGUILayout.Toggle (saveUpdateDesc, instance.SaveUpdate));
        EditorGUILayout.Space();
        instance.SetGrayTest(EditorGUILayout.Toggle(grayTestDesc, instance.GrayTest));
        EditorGUILayout.Space();

        instance.SetV2Signing(EditorGUILayout.Toggle(v2SignDesc, instance.V2Signing));
        EditorGUILayout.Space();

		int num2;
		if (!System.Int32.TryParse(instance.LocalLog, out num2)) {
			EditorGUILayout.HelpBox("Invalid localLog, need a number", MessageType.Error);
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField (localLogDesc);
        instance.SetLocalLog(EditorGUILayout.Popup(instance.LogLevelIndex, instance.logLevels));
		EditorGUILayout.EndHorizontal ();
        EditorGUILayout.Space();

        instance.SetXgOterPushEnable(EditorGUILayout.Toggle(xgOtherPushEnableDesc, instance.XgOtherPushEnable));
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(xgOtherPushXiaomiAppidDesc);
        instance.SetXgOtherPushXiaomiAppid(EditorGUILayout.TextField(instance.XgOtherPushXiaomiAppid));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(xgOtherPushXiaomiAppkeyDesc);
        instance.SetXgOtherPushXiaomiAppikey(EditorGUILayout.TextField(instance.XgOtherPushXiaomiAppkey));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(xgOtherPushMeizuAppidDesc);
        instance.SetXgOtherPushMeizuAppid(EditorGUILayout.TextField(instance.XgOtherPushMeizuAppid));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(xgOtherPushMeizuAppkeyDesc);
        instance.SetXgOtherPushMeizuAppkey(EditorGUILayout.TextField(instance.XgOtherPushMeizuAppkey));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("3) Check config when package", MessageType.None);
        instance.SetCheckConfig(EditorGUILayout.Toggle(checkConfigDesc, instance.CheckConfig));
        EditorGUILayout.Space();
	}

}
