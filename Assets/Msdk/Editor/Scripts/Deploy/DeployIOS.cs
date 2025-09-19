using UnityEngine;
using UnityEditor.XCodeEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class DeployIOS {
    static MsdkEnv env = MsdkEnv.Instance;
    static string iosConfigPath = env.PATH_IOS_PLIST;

	static readonly string weixin =
	@"                <string>weixin</string>
                <key>CFBundleURLSchemes</key>
                <array>";
	static readonly string tencentopenapi =
	@"                <string>tencentopenapi</string>
                <key>CFBundleURLSchemes</key>
                <array>";
	static readonly string QQ =
	@"                <string>QQ</string>
                <key>CFBundleURLSchemes</key>
                <array>";
	static readonly string QQLaunch =
	@"                <string>QQLaunch</string>
                <key>CFBundleURLSchemes</key>
                <array>";
    static readonly string tencentMsdk =
@"                <string>tencentmsdk</string>
                <key>CFBundleURLSchemes</key>
                <array>";
	static readonly string offerId =
	@"        <key>MSDK_OfferId</key>";
	static readonly string qqAppId =
	@"        <key>QQAppID</key>";
	static readonly string qqAppKey =
	@"        <key>QQAppKey</key>";
	static readonly string wxAppId =
	@"        <key>WXAppID</key>";
	static readonly string msdkKey =
	@"        <key>MSDKKey</key>";


	public static void Deploy() {
		UpdateBaseInfo();
	}

    public static void Deploy(string projectPath)
    {
        string path = Path.GetFullPath(projectPath);
        //动态修改 MSDKXcodeConfig.projmods 文件的配置
		CopyFrameworks(path);
        CopyOtherFiles(path);

        EditorMod(path);

        // 修改 plist 文件
        UpdateBaseInfo();
        EditorPlist(path);

        // 修改 xcode 代码(UnityAppController.mm)
        EditorCode(path);

        // 修改关于bugly的配置
        proessBuglyLib(path);

        // 更新Config
        ConfigSettings.Instance.Update();
    }

    private static void CopyFrameworks(string pathToBuiltProject)
    {
        string destDir = pathToBuiltProject + "/MSDK";
        if (!Directory.Exists(destDir)) {
            Directory.CreateDirectory(destDir);
        }

        MsdkUtil.ReplaceDir(env.PATH_LIBRARYS_IOS + "/Library/MSDK" + "/MSDK.framework",
                            destDir + "/MSDK.framework");
        MsdkUtil.ReplaceDir(env.PATH_LIBRARYS_IOS + "/Library/MSDK"  + "/MSDKResources" +  ".bundle",
                            destDir + "/MSDKResources.bundle");
		MsdkUtil.ReplaceDir(env.PATH_LIBRARYS_IOS + "/Library/MSDK"  + "/WGPlatformResources" +".bundle",
							destDir + "/WGPlatformResources.bundle");
        MsdkUtil.ReplaceDir(env.PATH_LIBRARYS_IOS + "/Library/MSDKLbs"  + "/MSDKLbs"  + ".framework",
                            destDir + "/MSDKLbs.framework");
        MsdkUtil.ReplaceDir(env.PATH_LIBRARYS_IOS + "/Library/MSDKPush" + "/MSDKPush" + ".framework",
                         destDir + "/MSDKPush.framework");             
        MsdkUtil.ReplaceDir(env.PATH_LIBRARYS_IOS + "/MSDKAdapter"  + "/MSDKAdapter"  + ".framework",
                            destDir + "/MSDKAdapter.framework");
        MsdkUtil.CopyDir(env.PATH_LIBRARYS_IOS + "/Library/MSDKPush/XG", destDir + "/XG", true);
        MsdkUtil.ReplaceDir(env.PATH_LIBRARYS_IOS + "/Library/MSDKSensitivity" + "/MSDKSensitivity" + ".framework",
                         destDir + "/MSDKSensitivity.framework");
        MsdkUtil.CopyDir(env.PATH_LIBRARYS_IOS + "/Library/TGPA_TID", destDir + "/TGPA_TID", true);
        MsdkUtil.ReplaceDir(env.PATH_LIBRARYS_IOS + "/Library/MSDKBeacon" + "/MSDKBeacon" + ".framework",
                         destDir + "/MSDKBeacon.framework");
        MsdkUtil.CopyDir(env.PATH_LIBRARYS_IOS + "/Library/MSDKBeacon/Beacon", destDir + "/Beacon", true);
    }

    private static void proessBuglyLib(string pathToBuiltProject)
    {
        string destDir = pathToBuiltProject + "/MSDK";
        if (!Directory.Exists(destDir)) {
            Directory.CreateDirectory(destDir);
        }
        if (DeploySettings.Instance.UseMSDKBugly) {
            //copy BuglyAgent dir
            MsdkUtil.CopyDir(env.PATH_BUGLY + "/iOS", destDir + "/Bugly", true);
            //copy MSDKBugly framework
            MsdkUtil.ReplaceDir(env.PATH_LIBRARYS_IOS + "/Library/MSDKBugly" + "/MSDKBugly" + ".framework",
                         destDir + "/MSDKBugly.framework");
            MsdkUtil.CopyDir(env.PATH_LIBRARYS_IOS + "/Library/MSDKBugly/Bugly", destDir + "/Bugly", true);
            //process dependency Bugly.framework
            Dictionary<string, string> modFileRules = new Dictionary<string, string>()
            {
            {".*/MSDK/Bugly/BuglyBridge.h"             , "        \"" + pathToBuiltProject + "/MSDK/Bugly/BuglyBridge.h\","},
            {".*/MSDK/Bugly/libBuglyBridge.a"          , "        \"" + pathToBuiltProject + "/MSDK/Bugly/libBuglyBridge.a\","},
            {".*/MSDK/Bugly/BuglyAgent.h"          , "        \"" + pathToBuiltProject + "/MSDK/Bugly/BuglyAgent.h\","},
            {".*/MSDK/MSDKBugly.framework"              , "        \"" + pathToBuiltProject + "/MSDK/MSDKBugly.framework\","},
            {".*/MSDK/Bugly/Bugly.framework"             , "        \"" + pathToBuiltProject + "/MSDK/Bugly/Bugly.framework\","}
            };

            MsdkUtil.ReplaceTextWithRegex(env.PATH_EDITOR + "/Resources/MSDKXcodeConfig.projmods", modFileRules);
            Debug.Log("update dependency in projmods about bugly successfully");
        } else {
            if (Directory.Exists(env.PATH_BUGLY)) {
                Dictionary<string, string> modFileRules = new Dictionary<string, string>()
                {
                    {".*/MSDK/Bugly/BuglyBridge.h"             , ""},
                    {".*/MSDK/Bugly/libBuglyBridge.a"          , ""},
                    {".*/MSDK/MSDKBugly.framework"              , ""},
                    {".*/MSDK/Bugly/Bugly.framework"              , ""}
                };
                MsdkUtil.ReplaceTextWithRegex(env.PATH_EDITOR + "/Resources/MSDKXcodeConfig.projmods", modFileRules);
                try {
                    Directory.Delete(env.PATH_BUGLY,true);
                } catch (IOException e) {
                    Debug.LogException(e);
                    return;
                }
                if (File.Exists(env.PATH_WGPLATFORM_UNITY_CS)) {
                    try {
                        Dictionary<string, string> regexRules = new Dictionary<string, string>();
                        regexRules.Add(@"BuglyAgent\.ConfigCrashReporter\(1, 4\);", "");
                        regexRules.Add(@"BuglyAgent\.InitWithAppId\(""[^""]*""\);", "");
                        regexRules.Add(@"BuglyAgent\.ConfigDebugMode \(false\);", "");
                        regexRules.Add(@"BuglyAgent\.ConfigAutoReportLogLevel\(LogSeverity\.LogException\);", "");
                        regexRules.Add(@"BuglyAgent\.EnableExceptionHandler \(\);", "");
                        MsdkUtil.ReplaceTextWithRegex(env.PATH_WGPLATFORM_UNITY_CS, regexRules);
                    } catch (IOException exception) {
                        Debug.LogException(exception);
                        return;
                    }
                }
            }
            Debug.Log("delete addtional bugly dependency since DeploySettings.Instance.UseMSDKBugly is false");
        }
    }

    private static void CopyOtherFiles(string pathToBuiltProject)
    {
        if (!MsdkUtil.isUnityEarlierThan("5.0")) {
            return;
        }

        string destDir = pathToBuiltProject + "/MSDK";
        if (!Directory.Exists(destDir)) {
            Directory.CreateDirectory(destDir);
        }

        //MsdkUtil.CopyDir(env.PATH_ADAPTER_IOS + "/oc", destDir + "/oc", true);
    }

    private static void EditorMod(string pathToBuiltProject)
    {
        Dictionary<string, string> modFileRules = new Dictionary<string, string>()
        {
            {".*/MSDK/MSDK.framework"                  , "        \"" + pathToBuiltProject + "/MSDK/MSDK.framework\","},
            {".*/MSDK/WGPlatformResources.bundle"      , "        \"" + pathToBuiltProject + "/MSDK/WGPlatformResources.bundle\","},
            {".*/MSDK/MSDKResources.bundle"            , "        \"" + pathToBuiltProject + "/MSDK/MSDKResources.bundle\","},
			{".*/MSDK/MSDKAdapter.framework"           , "        \"" + pathToBuiltProject + "/MSDK/MSDKAdapter.framework\","},
            {".*/MSDK/MSDKLbs.framework"               , "        \"" + pathToBuiltProject + "/MSDK/MSDKLbs.framework\","},
            {".*/MSDK/MSDKPush.framework"              , "        \"" + pathToBuiltProject + "/MSDK/MSDKPush.framework\","},
            {".*/MSDK/XG/XGMTACloud.framework"         , "        \"" + pathToBuiltProject + "/MSDK/XG/XGMTACloud.framework\","},
            {".*/MSDK/XG/libXG-SDK-Cloud.a"            , "        \"" + pathToBuiltProject + "/MSDK/XG/libXG-SDK-Cloud.a\","},
            {".*/MSDK/XG/XGForFreeVersion.h"           , "        \"" + pathToBuiltProject + "/MSDK/XG/XGForFreeVersion.h\","},
            {".*/MSDK/XG/XGPush.h"                     , "        \"" + pathToBuiltProject + "/MSDK/XG/XGPush.h\","},
            {".*/MSDK/XG/XGPushPrivate.h"              , "        \"" + pathToBuiltProject + "/MSDK/XG/XGPushPrivate.h\","},
            {".*/MSDK/MSDKSensitivity.framework"       , "        \"" + pathToBuiltProject + "/MSDK/MSDKSensitivity.framework\","},
            {".*/MSDK/TGPA_TID/tgpasimple.framework"   , "        \"" + pathToBuiltProject + "/MSDK/TGPA_TID/tgpasimple.framework\","},
            {".*/MSDK/MSDKBeacon.framework"            , "        \"" + pathToBuiltProject + "/MSDK/MSDKBeacon.framework\","},
            {".*/MSDK/Beacon/BeaconAPI_Audit.framework", "        \"" + pathToBuiltProject + "/MSDK/Beacon/BeaconAPI_Audit.framework\","},
            {".*/MSDK/Beacon/BeaconAPI_Base.framework" , "        \"" + pathToBuiltProject + "/MSDK/Beacon/BeaconAPI_Base.framework\","},
            {".*/MSDK/Beacon/BeaconId.framework"       , "        \"" + pathToBuiltProject + "/MSDK/Beacon/BeaconId.framework\","},
            {".*/MSDK/Beacon/QimeiSDK.framework"       , "        \"" + pathToBuiltProject + "/MSDK/Beacon/QimeiSDK.framework\","}
        };

        MsdkUtil.ReplaceTextWithRegex(env.PATH_EDITOR + "/Resources/MSDKXcodeConfig.projmods", modFileRules);
    }

    private static void EditorPlist(string filePath)
    {
        DeployIOS.UpdateBaseInfo();
        XCPlist list = new XCPlist(filePath);
        string plistAdd = File.ReadAllText(iosConfigPath);
        list.AddKey(plistAdd);
        list.Save();
    }

    private static void EditorCode(string projectPath)
    {
        string ocFile = projectPath + "/Classes/UnityAppController.mm";

        StreamReader streamReader = new StreamReader(ocFile);
        string text_all = streamReader.ReadToEnd();
        streamReader.Close();
        if (string.IsNullOrEmpty(text_all)) {
            return;
        }
        if (text_all.Contains(MsdkNativeCode.IOS_HEADER)) {
            Debug.LogWarning("You are appending to XCode project, would not modified <Classes/UnityAppController.mm>");
            return;
        }

        // 在指定代码后面增加一行代码
        MsdkUtil.WriteBelow(ocFile, MsdkNativeCode.IOS_SRC_HEADER, MsdkNativeCode.IOS_HEADER);
        MsdkUtil.WriteBelow(ocFile, MsdkNativeCode.IOS_SRC_FINISH, MsdkNativeCode.IOS_XG_REGISTER);
        // 在指定代码中替换一行
        MsdkUtil.ReplaceLineBelow(ocFile, MsdkNativeCode.IOS_SRC_OPENURL_OPTION, "return ", MsdkNativeCode.IOS_HANDLE_URL);
        MsdkUtil.WriteBelow(ocFile, MsdkNativeCode.IOS_SRC_REGISTER, MsdkNativeCode.IOS_XG_SUCC);
        MsdkUtil.WriteBelow(ocFile, MsdkNativeCode.IOS_SRC_REGISTER_FAIL, MsdkNativeCode.IOS_XG_FAIL);
        MsdkUtil.WriteBelow(ocFile, MsdkNativeCode.IOS_SRC_RECEIVE, MsdkNativeCode.IOS_XG_RECEIVE);
        MsdkUtil.WriteBelow(ocFile, MsdkNativeCode.IOS_SRC_ACTIVE, MsdkNativeCode.IOS_XG_CLEAR + MsdkNativeCode.IOS_BECAME_ACTIVE);
#if UNITY_2019_3_OR_NEWER
        // 适配 2019.3
        string ocFile_main = projectPath + "/MainApp/main.mm";
        MsdkUtil.ReplaceLineBelow(ocFile_main, MsdkNativeCode.IOS_SRC_MAINAPP, MsdkNativeCode.IOS_SRC_MAINAPP, MsdkNativeCode.IOS_MAINAPP);
        MsdkUtil.ReplaceLineBelow(ocFile, MsdkNativeCode.IOS_SRC_OPENUNIVERSALLINK_HIGH, "return ", MsdkNativeCode.IOS_HANDLE_UNIVERSALLINK_HIGH);
#else
        // 高版本Unity导出XCode后没有低版本的openurl 入口函数，
        MsdkUtil.ReplaceLineBelow(ocFile, MsdkNativeCode.IOS_SRC_OPENURL, "return ", MsdkNativeCode.IOS_HANDLE_URL);
        // UL 入口函数，适用于导出 mm 文件后，没有自动生成 UL 入口函数的情况
        MsdkUtil.WriteBelow(ocFile, MsdkNativeCode.IOS_SRC_OPENUNIVERSALLINK, MsdkNativeCode.IOS_HANDLE_UNIVERSALLINK);
#endif
    }

	public static void UpdateBaseInfo() {
		MsdkUtil.ReplaceBelow (iosConfigPath, weixin,
		                       "                    <string>" + DeploySettings.Instance.WxAppId + "</string>");
		MsdkUtil.ReplaceBelow (iosConfigPath, tencentopenapi,
		                       "                    <string>tencent" + DeploySettings.Instance.QqAppId + "</string>");
		MsdkUtil.ReplaceBelow (iosConfigPath, QQ,
			                    "                    <string>" + DeploySettings.Instance.QqScheme + "</string>");
		MsdkUtil.ReplaceBelow (iosConfigPath, QQLaunch,
		                       "                    <string>tencentlaunch" + DeploySettings.Instance.QqAppId + "</string>");
        MsdkUtil.ReplaceBelow(iosConfigPath, tencentMsdk,
                               "                    <string>tencentmsdk" + DeploySettings.Instance.QqAppId + "</string>");
		MsdkUtil.ReplaceBelow (iosConfigPath, offerId,
		                       "        <string>" + DeploySettings.Instance.IOSOfferId + "</string>");
		MsdkUtil.ReplaceBelow (iosConfigPath, qqAppId,
		                       "        <string>" + DeploySettings.Instance.QqAppId + "</string>");
		MsdkUtil.ReplaceBelow (iosConfigPath, wxAppId,
		                       "        <string>" + DeploySettings.Instance.WxAppId + "</string>");
		MsdkUtil.ReplaceBelow (iosConfigPath, msdkKey,
		                       "        <string>" + DeploySettings.Instance.MsdkKey + "</string>");
	}

}
