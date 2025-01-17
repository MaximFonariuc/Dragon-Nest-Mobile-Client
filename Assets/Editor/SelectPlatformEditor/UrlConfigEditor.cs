using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class UrlConfigEditor : EditorWindow
{
    private string _defaultLoginServer = @"127.0.0.1:25001";
    private string _androidQQLoginServer = @"127.0.0.1:25001";
    private string _androidWeChatLoginServer = @"127.0.0.1:25001";
    private string _iOSQQLoginServer = @"127.0.0.1:25001";
    private string _iOSWeChatLoginServer = @"127.0.0.1:25001";
    private string _iOSGuestLoginServer = @"127.0.0.1:25001";

    private string _versionServer = @"127.0.0.1:24001";

    private string _hostUrl = @"https://image.lzgjx.qq.com/Test/";
    private bool _isPublish = false;

    private static string _testDefaultLoginServer = @"127.0.0.1:25001";
    private static string _testAndroidQQLoginServer = @"127.0.0.1:25001";
    private static string _testAndroidWeChatLoginServer = @"127.0.0.1:25001";
    private static string _testiOSQQLoginServer = @"127.0.0.1:25001";
    private static string _testiOSWeChatLoginServer = @"127.0.0.1:25001";
    private static string _testiOSGuestLoginServer = @"127.0.0.1:25001";

    private static string _testVersionServer = @"127.0.0.1:24001";

    [MenuItem(@"XBuild/UrlConfig")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(UrlConfigEditor));
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        _defaultLoginServer = EditorGUILayout.TextField(_defaultLoginServer);
        EditorGUILayout.LabelField("DefaultLoginServer");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _androidQQLoginServer = EditorGUILayout.TextField(_androidQQLoginServer);
        EditorGUILayout.LabelField("AndroidQQLoginServer");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _androidWeChatLoginServer = EditorGUILayout.TextField(_androidWeChatLoginServer);
        EditorGUILayout.LabelField("AndroidWeChatLoginServer");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _iOSQQLoginServer = EditorGUILayout.TextField(_iOSQQLoginServer);
        EditorGUILayout.LabelField("iOSQQLoginServer");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _iOSWeChatLoginServer = EditorGUILayout.TextField(_iOSWeChatLoginServer);
        EditorGUILayout.LabelField("iOSWeChatLoginServer");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _iOSGuestLoginServer = EditorGUILayout.TextField(_iOSGuestLoginServer);
        EditorGUILayout.LabelField("iOSGuestLoginServer");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _versionServer = EditorGUILayout.TextField(_versionServer);
        EditorGUILayout.LabelField("VersionServer");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _hostUrl = EditorGUILayout.TextField(_hostUrl);
        EditorGUILayout.LabelField("HostUrl");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _isPublish = EditorGUILayout.ToggleLeft("IsPublish", _isPublish);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _testDefaultLoginServer = EditorGUILayout.TextField(_testDefaultLoginServer);
        EditorGUILayout.LabelField("TestDefaultLoginServer");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _testAndroidQQLoginServer = EditorGUILayout.TextField(_testAndroidQQLoginServer);
        EditorGUILayout.LabelField("TestAndroidQQLoginServer");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _testAndroidWeChatLoginServer = EditorGUILayout.TextField(_testAndroidWeChatLoginServer);
        EditorGUILayout.LabelField("TestAndroidWeChatLoginServer");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _testiOSQQLoginServer = EditorGUILayout.TextField(_testiOSQQLoginServer);
        EditorGUILayout.LabelField("TestiOSQQLoginServer");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _testiOSWeChatLoginServer = EditorGUILayout.TextField(_testiOSWeChatLoginServer);
        EditorGUILayout.LabelField("TestiOSWeChatLoginServer");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _testiOSGuestLoginServer = EditorGUILayout.TextField(_testiOSGuestLoginServer);
        EditorGUILayout.LabelField("TestiOSGuestLoginServer");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _testVersionServer = EditorGUILayout.TextField(_testVersionServer);
        EditorGUILayout.LabelField("TestVersionServer");
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Build", GUILayout.MaxWidth(80)))
        {
            Build();
        }
    }

    void Build()
    {
        StreamWriter sw;
        sw = new StreamWriter("Assets/config.txt");
        sw.Write(string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}",
                _defaultLoginServer, _androidQQLoginServer, _androidWeChatLoginServer,
                _iOSQQLoginServer, _iOSWeChatLoginServer, _iOSGuestLoginServer,
                _versionServer, _hostUrl, _isPublish,
                _testDefaultLoginServer, _testAndroidQQLoginServer, _testAndroidWeChatLoginServer,
                _testiOSQQLoginServer, _testiOSWeChatLoginServer, _testiOSGuestLoginServer,
                _testVersionServer));
        sw.Flush();
        sw.Close();
        AssetDatabase.ImportAsset("Assets/config.txt");

        List<AssetBundleBuild> list = new List<AssetBundleBuild>();
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = "config.cfg";
        build.assetNames = new string[] { "Assets/config.txt" };
        list.Add(build);

        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", list.ToArray(), BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

        AssetDatabase.ImportAsset("Assets/StreamingAssets/config.cfg");
    }

    public static void FastBuild()
    {
        StreamWriter sw;
        sw = new StreamWriter("Assets/config.txt");
        sw.Write(XPlatform.UrlConfig);
        sw.Flush();
        sw.Close();
        AssetDatabase.ImportAsset("Assets/config.txt");

        List<AssetBundleBuild> list = new List<AssetBundleBuild>();
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = "config.cfg";
        build.assetNames = new string[] { "Assets/config.txt" };
        list.Add(build);

        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", list.ToArray(), BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

        AssetDatabase.ImportAsset("Assets/StreamingAssets/config.cfg");
    }
}