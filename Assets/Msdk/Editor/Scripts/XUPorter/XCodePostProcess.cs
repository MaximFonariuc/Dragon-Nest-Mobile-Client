using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
using UnityEditor.iOS.Xcode;
#endif
using System.IO;
using System.Collections.Generic;

public static class XCodePostProcess
{
    static MsdkEnv env = MsdkEnv.Instance;

#if UNITY_EDITOR
    [PostProcessBuild(999)]
    public static void OnPostProcessBuild( BuildTarget target, string pathToBuiltProject)
    {

#if UNITY_5||UNITY_2017||UNITY_2017_1_OR_NEWER
        if (target == BuildTarget.iOS) {
#else
        if (target == BuildTarget.iPhone) {
#endif
            Debug.Log ("Run XCodePostProcess to Config Xcode project.");
        } else {
            return;
        }

        string path = Path.GetFullPath (pathToBuiltProject);

        // Create a new project object from build target
        XCProject project = new XCProject( path);

        // TODO GAME : Deploy MSDK to xcode project
        DeployIOS.Deploy(pathToBuiltProject);

        // Find and run through all projmods files to patch the project.
        // Please pay attention that ALL projmods files in your project folder will be excuted!
        string[] files = null;
        files = Directory.GetFiles (Application.dataPath, "*.projmods", SearchOption.AllDirectories);
        foreach( string file in files ) {
            project.ApplyMod( file );
        }
			
        // Finally save the xcode project
        project.Save();

        string projPath = UnityEditor.iOS.Xcode.PBXProject.GetPBXProjectPath(path);
        UnityEditor.iOS.Xcode.PBXProject proj = new UnityEditor.iOS.Xcode.PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));

        // 获取当前项目名字  
#if UNITY_2019_3_OR_NEWER // 适配 2019.3 及以上版本
        string target_1 = proj.GetUnityMainTargetGuid(); // app
        string target_2 = proj.GetUnityFrameworkTargetGuid(); // UnityFramework 是Unity 2019.3 新增的，2019.3 开始包含两个target
#else
        string target_1 = proj.TargetGuidByName(UnityEditor.iOS.Xcode.PBXProject.GetUnityTargetName());
#endif

		// TODO GAME : optional,设置签名的证书等属性
        // 对所有的编译配置设置选项  
        proj.SetBuildProperty(target_1, "ENABLE_BITCODE", "NO");
#if UNITY_2019_3_OR_NEWER // 适配 2019.3 及以上版本
        proj.SetBuildProperty(target_2, "ENABLE_BITCODE", "NO"); // Unity 2019.3及以上版本才需要
        proj.AddBuildProperty(target_2, "FRAMEWORK_SEARCH_PATHS", "$(SRCROOT)/MSDK");
        proj.AddBuildProperty(target_2, "FRAMEWORK_SEARCH_PATHS", "$(SRCROOT)/MSDK/XG");
        proj.AddBuildProperty(target_2, "LIBRARY_SEARCH_PATHS", "$(SRCROOT)/MSDK/XG");
        proj.AddBuildProperty(target_2, "FRAMEWORK_SEARCH_PATHS", "$(SRCROOT)/MSDK/TGPA_TID");
        proj.AddBuildProperty(target_2, "FRAMEWORK_SEARCH_PATHS", "$(SRCROOT)/MSDK/Beacon");
        if (DeploySettings.Instance.UseMSDKBugly) {
                proj.AddBuildProperty(target_2, "FRAMEWORK_SEARCH_PATH", "$(SRCROOT)/MSDK/Bugly");
        }

#endif
        proj.SetBuildProperty(target_1, "CODE_SIGN_IDENTITY", "iPhone Developer: Xiaochen Song (4LF6UN27XS)");      // 签名证书
        proj.SetBuildProperty(target_1, "PROVISIONING_PROFILE", "e475ba70-1681-4cca-8003-55f354d49f6b");
        proj.SetBuildProperty(target_1, "PROVISIONING_PROFILE_SPECIFIER", "MSDKV3-DEV");  // 签名描述文件
        proj.SetBuildProperty(target_1, "DEVELOPMENT_TEAM", "JBS4AWYMFX");
        proj.SetBuildProperty(target_1, "CODE_SIGN_STYLE", "Manual");
        proj.SetBuildProperty(target_1, "OTHER_CODE_SIGN_FLAGS", "--generate-entitlement-der");

	#if UNITY_2017||UNITY_2017_1_OR_NEWER //(AddCapability 要求 Unity 版本在 2017及以上)
		// Need create entitlements 
		// entitlements 文件名业务自定
         string relativeEntitlementFilePath = "Unity-iPhone/newmsdk2.entitlements";
         Debug.Log("entitlementsPath : " + relativeEntitlementFilePath);
         string absoluteEntitlementFilePath = pathToBuiltProject + "/" + relativeEntitlementFilePath;

         PlistDocument tempEntitlements = new PlistDocument();
         string key_associatedDomains = "com.apple.developer.associated-domains";
         var arr = (tempEntitlements.root[key_associatedDomains] = new PlistElementArray()) as PlistElementArray;

		 //需要替换成业务自己的 Universal Link
         arr.values.Add(new PlistElementString("applinks:wiki.ssl.msdk.qq.com"));

         string key_applesignin = "com.apple.developer.applesignin";
         var appleArr = (tempEntitlements.root[key_applesignin] = new PlistElementArray()) as PlistElementArray;
         appleArr.values.Add(new PlistElementString("Default"));

         proj.AddCapability(target_1, PBXCapabilityType.AssociatedDomains, relativeEntitlementFilePath);

         string projPath_1 = UnityEditor.iOS.Xcode.PBXProject.GetPBXProjectPath(pathToBuiltProject);
         File.WriteAllText(projPath_1, proj.WriteToString());
         tempEntitlements.WriteToFile(absoluteEntitlementFilePath);
	#endif
#if UNITY_2019_3_OR_NEWER // 适配 2019.3 及以上版本 指定target添加依赖库
        // 添加依赖库,AddFrameworkToProject 只能 System/Library/Frameworks 目录下的
        Debug.Log("Adding frameworks...");
        string[] frameworks = {
            "libz.dylib",
            "libz.1.1.3.dylib",
            "libsqlite3.dylib",
            "libxml2.dylib",
            "libc++.dylib",

            "CoreTelephony.framework",
            "SystemConfiguration.framework",
            "UIKit.framework",
            "Foundation.framework",
            "CoreGraphics.framework",
            "MobileCoreServices.framework",
            "StoreKit.framework",
            "CFNetwork.framework",
            "CoreData.framework",
            "Security.framework",
            "CoreLocation.framework",
            "ImageIO.framework",
            "CoreText.framework",
            "QuartzCore.framework",
            "AdSupport.framework",
            "AuthenticationServices.framework",
            "WebKit.framework",
            "Photos.framework"
        };

        foreach (var framework in frameworks)
        {
            proj.AddFrameworkToProject(target_2, framework, false);
        }
        proj.AddFrameworkToProject(target_2, "UserNotifications.framework", true);
        proj.AddFrameworkToProject(target_2, "Network.framework", true);

        // MSDK 相关
        Debug.Log("Adding MSDK files...");
        proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/MSDK.framework", "$(SRCROOT)/MSDKConfig/MSDK.framework", PBXSourceTree.Source));
        proj.AddFileToBuild(target_1, proj.AddFile(pathToBuiltProject + "/MSDK/WGPlatformResources.bundle", "$(SRCROOT)/MSDKConfig/WGPlatformResources.bundle", PBXSourceTree.Source));
        proj.AddFileToBuild(target_1, proj.AddFile(pathToBuiltProject + "/MSDK/MSDKResources.bundle", "$(SRCROOT)/MSDKConfig/MSDKResources.bundle", PBXSourceTree.Source));
        proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/MSDKAdapter.framework", "$(SRCROOT)/MSDKConfig/MSDKAdapter.framework", PBXSourceTree.Source));
        proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/MSDKLbs.framework", "$(SRCROOT)/MSDKConfig/MSDKLbs.framework", PBXSourceTree.Source));
        proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/MSDKPush.framework", "$(SRCROOT)/MSDKConfig/MSDKPush.framework", PBXSourceTree.Source));
        proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/XG/XGMTACloud.framework", "$(SRCROOT)/MSDKConfig/XGMTACloud.framework", PBXSourceTree.Source));
        proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/XG/libXG-SDK-Cloud.a", "$(SRCROOT)/MSDKConfig/libXG-SDK-Cloud.a", PBXSourceTree.Source));
        proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/MSDKSensitivity.framework", "$(SRCROOT)/MSDKConfig/MSDKSensitivity.framework", PBXSourceTree.Source));
        proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/TGPA_TID/tgpasimple.framework", "$(SRCROOT)/MSDKConfig/tgpasimple.framework", PBXSourceTree.Source));
        proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/MSDKBeacon.framework", "$(SRCROOT)/MSDKConfig/MSDKBeacon.framework", PBXSourceTree.Source));
        proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/Beacon/BeaconAPI_Audit.framework", "$(SRCROOT)/MSDKConfig/BeaconAPI_Audit.framework", PBXSourceTree.Source));
        proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/Beacon/BeaconAPI_Base.framework", "$(SRCROOT)/MSDKConfig/BeaconAPI_Base.framework", PBXSourceTree.Source));
        proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/Beacon/BeaconId.framework", "$(SRCROOT)/MSDKConfig/BeaconId.framework", PBXSourceTree.Source));
        proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/Beacon/QimeiSDK.framework", "$(SRCROOT)/MSDKConfig/QimeiSDK.framework", PBXSourceTree.Source));
        if (DeploySettings.Instance.UseMSDKBugly) {
                proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/MSDKBugly.framework", "$(SRCROOT)/MSDKConfig/MSDKBugly.framework", PBXSourceTree.Source));
                proj.AddFileToBuild(target_2, proj.AddFile(pathToBuiltProject + "/MSDK/Bugly/Bugly.framework", "$(SRCROOT)/MSDKConfig/Bugly.framework", PBXSourceTree.Source));
        }
#endif
        // 保存工程  
        proj.WriteToFile(projPath);

    }
#endif

}
