using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;


public class MsdkEnv
{
    public static readonly string msdkUnityVersion = "3.3.30u";
    public readonly string WORKSPACE;
    public readonly string PATH_PLUGIN_ANDROID;
    public readonly string PATH_PUGLIN_IOS;
    public readonly string PATH_EDITOR;
    public readonly string PATH_LIBRARYS;
    public readonly string PATH_LIBRARYS_ANDROID;
    public readonly string PATH_LIBRARYS_IOS;
    public readonly string PATH_TEMP;
    public readonly string PATH_ADAPTER_ANDROID;
    public readonly string PATH_ADAPTER_IOS;
    public readonly string PATH_BUGLY;
    
    public readonly string PATH_IOS_PLIST;

    public readonly string PATH_WGPLATFORM_UNITY_CS;

    static MsdkEnv instance;

    public static MsdkEnv Instance
    {
        get
        {
            if (instance == null) {
                instance = new MsdkEnv();
            }
            return instance;
        }
    }

    bool deploySucceed = true;

    public bool DeploySucceed
    {
        get { return deploySucceed; }
        set { deploySucceed = value; }
    }

    public MsdkEnv()
    {
        WORKSPACE = Directory.GetCurrentDirectory();
        PATH_PLUGIN_ANDROID = Path.Combine(WORKSPACE, @"Assets/Plugins/Android");
        PATH_PUGLIN_IOS = Path.Combine(WORKSPACE, @"Assets/Plugins/iOS");
        PATH_EDITOR = Path.Combine(WORKSPACE, @"Assets/Msdk/Editor");
        PATH_LIBRARYS = Path.Combine(WORKSPACE, @"Assets/Msdk/Editor/Librarys");
        PATH_TEMP = Path.Combine(WORKSPACE, @"Assets/Msdk/Editor/Temp");
        PATH_ADAPTER_ANDROID = Path.Combine(WORKSPACE, @"Assets/Msdk/Adapter/Android");
        PATH_ADAPTER_IOS = Path.Combine(WORKSPACE, @"Assets/Msdk/Adapter/iOS");
        PATH_BUGLY = Path.Combine(WORKSPACE, @"Assets/Msdk/BuglyPlugins");
        PATH_IOS_PLIST = Path.Combine(WORKSPACE, @"Assets/Msdk/Editor/Resources/MSDKInfo.plist");
        PATH_WGPLATFORM_UNITY_CS = Path.Combine(WORKSPACE, @"Assets/Msdk/Adapter/Commons/WGPlatformUnity.cs");

        PrepareDir(PATH_PLUGIN_ANDROID);
        PrepareDir(PATH_PUGLIN_IOS);

        DirectoryInfo resourceDirInfo = new DirectoryInfo(PATH_LIBRARYS);
        DirectoryInfo[] androidDirs = resourceDirInfo.GetDirectories("Android*");
        // 此目录中应只有一个 Android 开头的目录
        if (androidDirs.Length != 1) {
            Debug.LogError("Get Android Resource Directory Error!");
            return;
        }
        PATH_LIBRARYS_ANDROID = androidDirs[0].FullName;

        DirectoryInfo[] iOSDirs = resourceDirInfo.GetDirectories("iOS*");
        // 此目录中应只有一个 iOS 开头的目录
        if (iOSDirs.Length != 1) {
            Debug.LogError("Get iOS Resource Directory Error!");
            return;
        }
        PATH_LIBRARYS_IOS = iOSDirs[0].FullName;
    }

    // 准备环境
    private void PrepareDir(string dirPath)
    {
        if (!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
    }

    // 打印错误日志，标明布署失败
    public void Error(string msg)
    {
        deploySucceed = false;
        Debug.LogError(msg);
    }
}
