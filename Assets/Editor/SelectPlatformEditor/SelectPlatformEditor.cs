using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using XEditor;
using ABSystem;

class SelectPlatformEditor : EditorWindow
{
    public enum PlatformType
    {
        NotSelect,
        Win32,
        iOS,
        Android,
    }

    public enum ScriptingDefineType
    {
        发布版本,
        TencentQA版本,
        测试版本,
        Debug版本,
    }

    static string[] SCENES = null;//FindEnabledEditorScenes();

    static private bool _initPlatform = true;
    static private PlatformType _platformType = PlatformType.NotSelect;
    static private PlatformType _lastPlatform = PlatformType.NotSelect;

    static private string _targetDir = "";

    static private BuildTarget _buildTarget;

    //=======================dev=======================
    static private bool _WeTestOpen = false;



    static private bool _DevOpen = false;

    static private bool _DisableSDK = false;

    //=======================pre build=======================
    static private bool _BuildBundle = true;

    //=======================build=======================
    static private bool _buglyOpen = false;

    static private bool _TssOpen = false;

    static private bool _ApolloOpen = false;

    static private bool _BroadcastOpen = false;

    static private bool _HttpDnsOpen = false;

    static private string _oldScriptingDefine = "";

    static private bool _GameSirOpen = false;

    static private bool _CubeOpen = true;


    static private string _scriptingDefine = "Release";
    static private ScriptingDefineType _scriptingDefineType = ScriptingDefineType.Debug版本;

    static private BuildOptions _buildOption = BuildOptions.None;

    static public bool isBuild = false;

    static public string Kind
    {
        get
        {
            if (_scriptingDefineType == ScriptingDefineType.Debug版本 ||
                _scriptingDefineType == ScriptingDefineType.TencentQA版本) return "_debug";
            else if (_scriptingDefineType == ScriptingDefineType.测试版本) return "_test";
            else if (_scriptingDefineType == ScriptingDefineType.发布版本) return "_publish";
            else return "_qa";
        }
    }

    [MenuItem(@"XBuild/SelectPlatform")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(SelectPlatformEditor));
        //RunGitPull();
    }

    private void InitPlatform()
    {
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.iOS:
                _platformType = PlatformType.iOS;
                _lastPlatform = PlatformType.iOS;
                break;
            case BuildTarget.Android:
                _platformType = PlatformType.Android;
                _platformType = PlatformType.Android;
                break;
            case BuildTarget.StandaloneWindows:
                _platformType = PlatformType.Win32;
                _platformType = PlatformType.Win32;
                break;
            default:
                _platformType = PlatformType.NotSelect;
                _platformType = PlatformType.NotSelect;
                break;
        }
        _initPlatform = false;
    }

    void OnGUI()
    {
        if (_initPlatform)
        {
            InitPlatform();
        }

        _lastPlatform = _platformType;
        _platformType = (PlatformType)EditorGUILayout.EnumPopup("Platform Type", _platformType);

        BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
        switch (_platformType)
        {
            case PlatformType.iOS:
                targetGroup = BuildTargetGroup.iOS;
                break;
            case PlatformType.Android:
                targetGroup = BuildTargetGroup.Android;
                break;
            case PlatformType.Win32:
                targetGroup = BuildTargetGroup.Standalone;
                break;
            default:
                break;
        }

        if (_lastPlatform != _platformType)
        {
            if (EditorUtility.DisplayDialog("Switch Platform Or Not",
                        "Do you want to Switch Platform?",
                        "Yes", "No"))
            {
                SwitchPlatForm(_platformType);
            }
            else
            {
                _platformType = _lastPlatform;
            }
        }

        EditorGUILayout.BeginHorizontal();
        _scriptingDefineType = (ScriptingDefineType)EditorGUILayout.EnumPopup("版本选择", _scriptingDefineType);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Product Name: ");
        GUILayout.Label(PlayerSettings.productName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Bundle Identifier: ");
        GUILayout.Label(PlayerSettings.applicationIdentifier);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Bundle Version: ");
        GUILayout.Label(PlayerSettings.bundleVersion);
        EditorGUILayout.EndHorizontal();

        //pre build
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("=========================PreBuild=========================");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        _BuildBundle = EditorGUILayout.ToggleLeft("Build Bundle", _BuildBundle);
        EditorGUILayout.EndHorizontal();
        if (_platformType != PlatformType.NotSelect)
        {
            if (GUILayout.Button("PreBuild", GUILayout.MaxWidth(80)))
            {
                if (EditorUtility.DisplayDialog("PreBuild Or Not",
                        "Do you want to PreBuild (UI Texture,Scene Texture,Animation Bundle)?",
                        "Yes", "No"))
                {
                    PreBuild();
                }
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("=========================DevOption=========================");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _DevOpen = EditorGUILayout.ToggleLeft("Dev Open", _DevOpen);
        _WeTestOpen = EditorGUILayout.ToggleLeft("WeTest Open", _WeTestOpen);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _DisableSDK = EditorGUILayout.ToggleLeft("Disable AllSDK", _DisableSDK);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("DevBuild", GUILayout.MaxWidth(100)))
        {
            DevPreBuild();
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=========================ReleaseOption=========================");
        EditorGUILayout.BeginHorizontal();
        _buglyOpen = EditorGUILayout.ToggleLeft("Bugly Open", _buglyOpen);
        _TssOpen = EditorGUILayout.ToggleLeft("Tss Open", _TssOpen);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        _HttpDnsOpen = EditorGUILayout.ToggleLeft("HttpDns Open", _HttpDnsOpen);
        _ApolloOpen = EditorGUILayout.ToggleLeft("Apollo Open", _ApolloOpen);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        _BroadcastOpen = EditorGUILayout.ToggleLeft("Broadcast Open", _BroadcastOpen);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        _GameSirOpen = EditorGUILayout.ToggleLeft("GameSir Open", _GameSirOpen);
        _CubeOpen = EditorGUILayout.ToggleLeft("Cube Open", _CubeOpen);
        EditorGUILayout.EndHorizontal();
        //EditorGUILayout.BeginHorizontal();
        //_SupportAddLoadScene = EditorGUILayout.ToggleLeft("Additive Load Scene", _SupportAddLoadScene);
        //EditorGUILayout.EndHorizontal();

        _buildOption = BuildOptions.None;



        if (_DevOpen)
            _buildOption = BuildOptions.Development | BuildOptions.ConnectWithProfiler;

        if (GUILayout.Button("SetScriptDefine", GUILayout.MaxWidth(120)))
        {
            SetScriptDefine();
            if (_oldScriptingDefine == "") _oldScriptingDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            SwitchPlatForm(_platformType);
        }

        if (_platformType != PlatformType.NotSelect)
        {
            if (GUILayout.Button("Build", GUILayout.MaxWidth(80)))
            {
                if (EditorUtility.DisplayDialog("Build Or Not",
                        "Do you want to Build?",
                        "Yes", "No"))
                {
                    SetScriptDefine();
                    if (_oldScriptingDefine == "") _oldScriptingDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
                    SwitchPlatForm(_platformType);
                    Build();
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, _oldScriptingDefine);
                    _oldScriptingDefine = "";
                }
            }
        }

        //ShowABlayout();
    }

    static private void SetScriptDefine()
    {
        switch (_scriptingDefineType)
        {
            case ScriptingDefineType.TencentQA版本:
                _scriptingDefine = "QA_TEST";
                break;
            case ScriptingDefineType.发布版本:
                _scriptingDefine = "Publish";
                break;
            case ScriptingDefineType.测试版本:
                _scriptingDefine = "Test";
                break;
            default:
                _scriptingDefine = "";
                break;
        }

        if (_buglyOpen)
        {
            _scriptingDefine = _scriptingDefine + ";BUGLY";
        }

        if (_TssOpen)
        {
            _scriptingDefine = _scriptingDefine + ";TSS";
        }
        if (_ApolloOpen)
        {
            _scriptingDefine = _scriptingDefine + ";APOLLO";
        }

        if (_HttpDnsOpen)
        {
            _scriptingDefine = _scriptingDefine + ";HTTP_DNS";
        }
        if (_BroadcastOpen)
        {
            _scriptingDefine = _scriptingDefine + ";BROADCAST";
        }
        if (_WeTestOpen)
        {
            _scriptingDefine = _scriptingDefine + ";USE_WETEST";
        }
        if (_CubeOpen)
        {
            _scriptingDefine = _scriptingDefine + ";USE_CUBE";
        }

        if (_GameSirOpen)
        {
            _scriptingDefine = _scriptingDefine + ";GAMESIR";
        }

        if (_DisableSDK)
        {
            _scriptingDefine = _scriptingDefine + ";DISABLE_JOYSDK;DISABLE_FMODE";
        }
    }

    static private void SwitchPlatForm(PlatformType platformType)
    {
        switch (platformType)
        {
            case PlatformType.Win32:
                PlayerSetting_Win32();
                break;
            case PlatformType.iOS:
                PlayerSetting_iOS();
                break;
            case PlatformType.Android:
                PlayerSetting_Android();
                break;
            default:
                break;
        }
    }

    static private string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && !EditorScenes.Contains(scene.path))
                EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    static private void PreBuild()
    {
        if (_BuildBundle)
            AssetBundleBuildPanel.BuildAssetBundles();
    }

    static private void CopyAsset(string src, string des)
    {
        if (!File.Exists(des) && File.Exists(src))
        {
            AssetDatabase.CopyAsset(src, des);
        }
    }
    static private void DeleteAsset(string des)
    {
        if (File.Exists(des))
        {
            AssetDatabase.DeleteAsset(des);
        }
    }

    static private void CreateDevDir()
    {
        if (!Directory.Exists("Assets/Plugins/Android/libs/armeabi/"))
        {
            Directory.CreateDirectory("Assets/Plugins/Android/libs/armeabi/");
        }
        if (!Directory.Exists("Assets/Plugins/Android/libs/armeabi-v7a/"))
        {
            Directory.CreateDirectory("Assets/Plugins/Android/libs/armeabi-v7a/");
        }
    }
    static private void DevPreBuild()
    {
        if (_platformType == PlatformType.Android && _scriptingDefineType != ScriptingDefineType.发布版本)
        {
            if (_DisableSDK)
            {
                MoveFolder("Assets/Plugins/Android", "Assets/Android");
            }
            if (_WeTestOpen)
            {
                CreateDevDir();
                CopyAsset("Assets/ExtraPlugins/Android/wetest/dll/U3DAutomation.dll", "Assets/Lib/U3DAutomation.dll");
                CopyAsset("Assets/ExtraPlugins/Android/wetest/libs/u3dautomation.jar", "Assets/Plugins/Android/libs/u3dautomation.jar");
                CopyAsset("Assets/ExtraPlugins/Android/wetest/libs/libcrashmonitor.so", "Assets/Plugins/Android/libs/armeabi/libcrashmonitor.so");
                CopyAsset("Assets/ExtraPlugins/Android/wetest/libs/libcrashmonitor.so", "Assets/Plugins/Android/libs/armeabi-v7a/libcrashmonitor.so");
                AssetDatabase.SaveAssets();
            }
            else
            {
                DeleteAsset("Assets/Lib/U3DAutomation.dll");
                DeleteAsset("Assets/Plugins/Android/libs/u3dautomation.jar");
                DeleteAsset("Assets/Plugins/Android/libs/armeabi/libcrashmonitor.so");
                DeleteAsset("Assets/Plugins/Android/libs/armeabi-v7a/libcrashmonitor.so");
            }
            //if (_CubeOpen)
            //{
            //    CreateDevDir();
            //    CopyAsset("Assets/ExtraPlugins/Android/cubeapm/libs/cubeclient.jar", "Assets/Plugins/Android/libs/cubeclient.jar");
            //    CopyAsset("Assets/ExtraPlugins/Android/cubeapm/libs/armeabi-v7a/libcubehawk.so", "Assets/Plugins/Android/libs/armeabi/libcubehawk.so");
            //    CopyAsset("Assets/ExtraPlugins/Android/cubeapm/libs/armeabi-v7a/libcubehawk.so", "Assets/Plugins/Android/libs/armeabi-v7a/libcubehawk.so");
            //    AssetDatabase.SaveAssets();                
            //}
            //else
            //{
            //    DeleteAsset("Assets/Plugins/Android/libs/cubeclient.jar");
            //    DeleteAsset("Assets/Plugins/Android/libs/armeabi/libcubehawk.so");
            //    DeleteAsset("Assets/Plugins/Android/libs/armeabi-v7a/libcubehawk.so");
            //}
        }
        else
        {
            CleanDevBuild();
        }
        AssetDatabase.Refresh();
    }
    static private void CleanDevBuild()
    {
        DeleteAsset("Assets/Lib/U3DAutomation.dll");
        DeleteAsset("Assets/Plugins/Android/libs/u3dautomation.jar");
        DeleteAsset("Assets/Plugins/Android/libs/armeabi/libcrashmonitor.so");
        DeleteAsset("Assets/Plugins/Android/libs/armeabi-v7a/libcrashmonitor.so");

        //DeleteAsset("Assets/Plugins/Android/libs/cubeclient.jar");
        //DeleteAsset("Assets/Plugins/Android/libs/armeabi/libcubehawk.so");
        //DeleteAsset("Assets/Plugins/Android/libs/armeabi-v7a/libcubehawk.so");

        if (_DisableSDK)
        {
            if (Directory.Exists("Assets/Plugins/Android"))
                Directory.Delete("Assets/Plugins/Android", true);
            MoveFolder("Assets/Android", "Assets/Plugins/Android");
        }
    }

    static private void Build()
    {
        SCENES = FindEnabledEditorScenes();
        SwitchPlatForm(_platformType);
        EditorUserBuildSettings.SwitchActiveBuildTarget(_buildTarget);
        //每次build删除之前的残留
        if (Directory.Exists(_targetDir))
        {
            if (File.Exists(PlayerSettings.productName))
            {
                File.Delete(PlayerSettings.productName);
            }
        }
        else
        {
            Directory.CreateDirectory(_targetDir);
        }

        BeforeBuild(EditorUserBuildSettings.activeBuildTarget);

        UrlConfigEditor.FastBuild();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        string lastName = "";
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) lastName = ".apk";
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows) lastName = ".exe";

        TextAsset data = AssetDatabase.LoadAssetAtPath("Assets/Resources/XMainClient.bytes", typeof(TextAsset)) as TextAsset;
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android && (data == null || data.bytes.Length == 0))
        {
            EditorUtility.DisplayDialog("Error", "XMainClient.bytes Not Find!!!", "OK");
            return;
        }
        isBuild = true;
        string res = BuildPipeline.BuildPlayer(SCENES, _targetDir + "/DragonNest" + Kind + lastName, _buildTarget, _buildOption).ToString();
        isBuild = false;
        CleanDevBuild();

        AfterBuild(EditorUserBuildSettings.activeBuildTarget);

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
        {
            System.Diagnostics.Process.Start(_targetDir);
        }

        EditorUtility.DisplayDialog("Package Build Finish", "Package Build Finish!(" + res + ")", "OK");

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "");
    }

    #region jekins use
    static void SetMacrow()
    {
        _scriptingDefine = "BUGLY;USE_WETEST;APOLLO;BROADCAST";
        TextAsset cfg = AssetDatabase.LoadAssetAtPath("Assets/Build.txt", typeof(TextAsset)) as TextAsset;
        if (cfg != null) _scriptingDefine = cfg.text;

        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.iOS:
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, _scriptingDefine);
                break;
            case BuildTarget.Android:
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _scriptingDefine);
                break;
            default:
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, _scriptingDefine);
                break;
        }

    }
    [MenuItem("XBuild/IOS/Debug版本")]
    static void BuildiOSDebug()
    {
        _scriptingDefineType = ScriptingDefineType.Debug版本;
        _scriptingDefine = "BUGLY;USE_WETEST;APOLLO;BROADCAST";
        TextAsset cfg = AssetDatabase.LoadAssetAtPath("Assets/Build.txt", typeof(TextAsset)) as TextAsset;
        if (cfg != null) _scriptingDefine = cfg.text;
        UnityEngine.Debug.Log("*********** _scriptingDefine " + _scriptingDefine + "  cfg: " + (cfg == null) + " *************");
        if (_scriptingDefine.Contains("QA_TEST")) _scriptingDefineType = ScriptingDefineType.TencentQA版本;
        _buildOption = BuildOptions.None;
        PlayerSetting_iOS();
        Build();
    }


    [MenuItem("XBuild/Android/Debug版本")]
    static void BuildAndroidDebug()//Debug版本
    {
        _scriptingDefineType = ScriptingDefineType.Debug版本;
        _buildOption = BuildOptions.None;
        _scriptingDefine = "APOLLO;GAMESIR;BROADCAST";
        TextAsset cfg = AssetDatabase.LoadAssetAtPath("Assets/Build.txt", typeof(TextAsset)) as TextAsset;
        if (cfg != null) _scriptingDefine = cfg.text;
        UnityEngine.Debug.Log("*********** _scriptingDefine "+ _scriptingDefine+"  cfg: "+(cfg==null)+" *************");
        if (_scriptingDefine.Contains("QA_TEST")) _scriptingDefineType = ScriptingDefineType.TencentQA版本;
        PlayerSetting_Android();
        Build();
    }
    
    #endregion

    #region Shell Build
    static void ShellBuildWin32()
    {
        _scriptingDefineType = ScriptingDefineType.测试版本;
        _scriptingDefine = "";
        TextAsset cfg = AssetDatabase.LoadAssetAtPath("Assets/Build.txt", typeof(TextAsset)) as TextAsset;
        if (cfg != null) _scriptingDefine = cfg.text;
        _buildOption = BuildOptions.None;
        PlayerSetting_Win32();
        Build();
    }

    static void ShellBuildAndroid()
    {
        _scriptingDefineType = ScriptingDefineType.测试版本;
        _scriptingDefine = "";
        TextAsset cfg = AssetDatabase.LoadAssetAtPath("Assets/Build.txt", typeof(TextAsset)) as TextAsset;
        if (cfg != null) _scriptingDefine = cfg.text;
        _buildOption = BuildOptions.None;
        PlayerSetting_Android();
        Build();
    }

    static void ShellBuildiOS()
    {
        _scriptingDefineType = ScriptingDefineType.测试版本;
        _scriptingDefine = "";
        TextAsset cfg = AssetDatabase.LoadAssetAtPath("Assets/Build.txt", typeof(TextAsset)) as TextAsset;
        if (cfg != null) _scriptingDefine = cfg.text;
        _buildOption = BuildOptions.None;
        PlayerSetting_iOS();
        Build();
    }
    #endregion

    static private void MoveMainClient(string from, string to)
    {
        byte[] o = File.ReadAllBytes(from);
        File.WriteAllBytes(to, o);
        File.Delete(from);
        //AssetDatabase.MoveAsset(from, to);
        AssetDatabase.ImportAsset(to);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static public bool MoveEditor(string from, string to)
    {
        string editorDir = "Assets/Scripts";
        if (!Directory.Exists(editorDir + "/" + to))
        {
            AssetDatabase.RenameAsset(editorDir + "/" + from, to);
        }
        else
        {
            if (Directory.Exists(editorDir + "/" + from))
            {
                AssetDatabase.DeleteAsset(editorDir + "/" + to);
                AssetDatabase.RenameAsset(editorDir + "/" + from, to);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return true;
    }

    static private bool MoveFolder(string from, string to)
    {
        if (!Directory.Exists(to))
        {
            AssetDatabase.MoveAsset(from, to);
        }
        else
        {
            if (Directory.Exists(from))
            {
                AssetDatabase.DeleteAsset(to);
                AssetDatabase.MoveAsset(from, to);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return true;
    }


    static private void CopyFolder(string from, string to)
    {
        string dir = Path.GetDirectoryName(Application.dataPath);
        FileInfo[] files = (new DirectoryInfo(Path.Combine(dir , from))).GetFiles("*.dll");
        string _to = Path.Combine(dir, to);
        if (!Directory.Exists(_to)) Directory.CreateDirectory(_to);
        foreach (var ie in files)
        {
            string dest = Path.Combine(_to, ie.Name);
            if (File.Exists(dest)) File.Delete(dest);
            File.Copy(ie.FullName, dest);
            UnityEngine.Debug.Log("is copy : " + ie.FullName+" dest: "+dest);
        }
    }

    static private void CleanGit()
    {
        Process p = new Process();

        if (System.Environment.OSVersion.Platform == PlatformID.Unix)
        {
            System.Diagnostics.Process.Start("/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal",
                "/Users/liming/Desktop/DN/dragon-nest/XProject/Assets/Editor/AutoBuild/XClean.sh");
        }
        else
        {
            p.StartInfo.FileName = "sh";
            p.StartInfo.Arguments = Application.dataPath + "/Editor/AutoBuild/XClean.sh";

            p.StartInfo.WorkingDirectory = Application.dataPath;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;

            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            if (String.IsNullOrEmpty(output) == false)
                UnityEngine.Debug.Log(output);
            p.WaitForExit();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    

    static private bool BeforeBuild(BuildTarget target)
    {
        AssetBundleBuildConfig config = AssetBundleBuildPanel.LoadAssetAtPath<AssetBundleBuildConfig>(AssetBundleBuildPanel.savePath);

        if (!Directory.Exists("Assets/BuildTemp"))
        {
            AssetDatabase.CreateFolder("Assets", "BuildTemp");
        }

        for (int i = 0; i < config.filters.Count; ++i)
        {
            if (!config.filters[i].valid) continue;

            if (!MoveFolder(config.filters[i].path, config.filters[i].path.Replace("/", ".").Replace(".Resources.", "/BuildTemp/"))) return false;
        }
        CopyFolder("Assets/Lib", "Library/Lib");
        switch (target)
        {
            case BuildTarget.Android:
                InjectEditor.DoInjectMainClient();
                MoveMainClient("Assets/Lib/XMainClient.dll", "Assets/Resources/XMainClient.bytes");
                AssetDatabase.DeleteAsset("Assets/Resources/dep.bytes");
                AssetDatabase.CopyAsset("Assets/StreamingAssets/update/Android/AssetBundles/dep.all", "Assets/Resources/dep.bytes");
                if (!MoveFolder("Assets/StreamingAssets/update/iOS", "Assets/BuildTemp/iOS")) return false;
                if (!MoveFolder("Assets/StreamingAssets/update/AssetBundles", "Assets/BuildTemp/AssetBundles")) return false;
                break;
            case BuildTarget.iOS:
                AssetDatabase.DeleteAsset("Assets/Resources/dep.bytes");
                AssetDatabase.CopyAsset("Assets/StreamingAssets/update/iOS/AssetBundles/dep.all", "Assets/Resources/dep.bytes");
                if (!MoveFolder("Assets/StreamingAssets/update/Android", "Assets/BuildTemp/Android")) return false;
                if (!MoveFolder("Assets/StreamingAssets/update/AssetBundles", "Assets/BuildTemp/AssetBundles")) return false;
                if (!MoveFolder("Assets/Plugins/x86/ulua.dll", "")) return false;
                break;
            default:
                AssetDatabase.DeleteAsset("Assets/Resources/dep.bytes");
                AssetDatabase.CopyAsset("Assets/StreamingAssets/update/AssetBundles/dep.all", "Assets/Resources/dep.bytes");
                if (!MoveFolder("Assets/StreamingAssets/update/iOS", "Assets/BuildTemp/iOS")) return false;
                if (!MoveFolder("Assets/StreamingAssets/update/Android", "Assets/BuildTemp/Android")) return false;
                break;
        }

        if (!MoveFolder("Assets/Resources/Curve", "Assets/BuildTemp/Curve")) return false;

        if (_scriptingDefineType == ScriptingDefineType.TencentQA版本 || _scriptingDefineType == ScriptingDefineType.Debug版本)
        {
            AssetDatabase.MoveAsset("Assets/Plugins/Android/assets/msdkconfig.ini", "Assets/Plugins/Android/assets/msdkconfig.ini.bak");
            AssetDatabase.MoveAsset("Assets/Plugins/Android/assets/msdkconfigTest.ini", "Assets/Plugins/Android/assets/msdkconfig.ini");
        }     

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return true;
    }

    static private bool AfterBuild(BuildTarget target)
    {
        AssetBundleBuildConfig config = AssetBundleBuildPanel.LoadAssetAtPath<AssetBundleBuildConfig>(AssetBundleBuildPanel.savePath);

        for (int i = 0; i < config.filters.Count; ++i)
        {
            if (!config.filters[i].valid) continue;

            if (!MoveFolder(config.filters[i].path.Replace("/", ".").Replace(".Resources.", "/BuildTemp/"), config.filters[i].path)) return false;
        }

        switch (target)
        {
            case BuildTarget.Android:
                MoveMainClient("Assets/Resources/XMainClient.bytes", "Assets/Lib/XMainClient.dll");
                MoveFolder("Assets/BuildTemp/iOS", "Assets/StreamingAssets/update/iOS");
                MoveFolder("Assets/BuildTemp/AssetBundles", "Assets/StreamingAssets/update/AssetBundles");
                break;
            case BuildTarget.iOS:
                PostVedioBuild.OnPostProcessBuild();
                MoveFolder("Assets/BuildTemp/Android", "Assets/StreamingAssets/update/Android");
                MoveFolder("Assets/BuildTemp/AssetBundles", "Assets/StreamingAssets/update/AssetBundles");
                break;
            default:
                MoveFolder("Assets/BuildTemp/iOS", "Assets/StreamingAssets/update/iOS");
                MoveFolder("Assets/BuildTemp/Android", "Assets/StreamingAssets/update/Android");
                break;
        }
        if (!MoveFolder("Assets/BuildTemp/Curve", "Assets/Resources/Curve")) return false;
        if (_scriptingDefineType == ScriptingDefineType.TencentQA版本 || _scriptingDefineType == ScriptingDefineType.Debug版本)
        {
            AssetDatabase.MoveAsset("Assets/Plugins/Android/assets/msdkconfig.ini", "Assets/Plugins/Android/assets/msdkconfigTest.ini");
            AssetDatabase.MoveAsset("Assets/Plugins/Android/assets/msdkconfig.ini.bak", "Assets/Plugins/Android/assets/msdkconfig.ini");
        }
        CopyFolder("Library/Lib", "Assets/Lib");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return true;
    }

    static private void PlayerSetting_Win32()
    {
        _buildTarget = BuildTarget.StandaloneWindows;
        EditorUserBuildSettings.SwitchActiveBuildTarget(_buildTarget);

        _targetDir = Application.dataPath.Replace("/Assets", "") + "/Win32";

        PlayerSettings.companyName = "huanle";
        PlayerSettings.productName = "龙之谷";

        PlayerSettings.defaultIsFullScreen = false;
        PlayerSettings.defaultScreenWidth = 1136;
        PlayerSettings.defaultScreenHeight = 640;
        PlayerSettings.runInBackground = true;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.useAnimatedAutorotation = true;

        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

        PlayerSettings.applicationIdentifier = "com.huanle.dragon";
        string _version = "0.0.0";
        PlayerSettings.bundleVersion = _version;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, _scriptingDefine);

        PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;
        PlayerSettings.strippingLevel = StrippingLevel.StripByteCode;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static private void PlayerSetting_iOS()
    {
        _buildTarget = BuildTarget.iOS;
        EditorUserBuildSettings.SwitchActiveBuildTarget(_buildTarget);

        _targetDir = Application.dataPath.Replace("/Assets", "") + "/IOS";// "/Users/liming/Desktop" + "/IOS";

        PlayerSettings.companyName = "huanle";
        PlayerSettings.productName = "龙之谷";

        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.useAnimatedAutorotation = true;

        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

#if J_Tencent
        PlayerSettings.bundleIdentifier = "com.tencent.dragonnest";
#else
        PlayerSettings.applicationIdentifier = "com.huanle.dragon";
#endif
        XBundleTools.InitVersionBytes();
        string _version = "0.0.0";
        if (File.Exists(XBundleTools.ResRoot))
        {
            _version = ASCIIEncoding.ASCII.GetString(File.ReadAllBytes(XBundleTools.ResRoot));
        }
        PlayerSettings.bundleVersion = _version + (_scriptingDefineType == ScriptingDefineType.TencentQA版本 ? "QA" : "");
        PlayerSettings.iOS.buildNumber = _version;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        PlayerSettings.accelerometerFrequency = 0;
        PlayerSettings.iOS.locationUsageDescription = "";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, _scriptingDefine);

        PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0_Subset;
        PlayerSettings.aotOptions = "nrgctx-trampolines=4096,nimt-trampolines=4096,ntrampolines=4096";
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.iOS.targetOSVersionString = "7.1";
        PlayerSettings.strippingLevel = StrippingLevel.StripByteCode;
        PlayerSettings.iOS.scriptCallOptimization = ScriptCallOptimizationLevel.FastButNoExceptions;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static private void PlayerSetting_Android()
    {
        _buildTarget = BuildTarget.Android;
        EditorUserBuildSettings.SwitchActiveBuildTarget(_buildTarget);

        _targetDir = Application.dataPath.Replace("/Assets", "") + "/Android";

        PlayerSettings.companyName = "huanle";
        PlayerSettings.productName = "龙之谷";

        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.useAnimatedAutorotation = true;

        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

        PlayerSettings.applicationIdentifier = "com.tencent.tmgp.dragonnest";
        XBundleTools.InitVersionBytes();
        string _version = "0.0.0";
        if (File.Exists(XBundleTools.ResRoot))
        {
            _version = ASCIIEncoding.ASCII.GetString(File.ReadAllBytes(XBundleTools.ResRoot));
        }

        PlayerSettings.bundleVersion = _version + (_scriptingDefineType == ScriptingDefineType.TencentQA版本 ? "QA" : "");
        int bundleVersionCode = int.Parse(System.DateTime.Now.ToString("yyMMdd"));
        PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;
        PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
        PlayerSettings.Android.forceSDCardPermission = true;
        PlayerSettings.Android.forceInternetPermission = true;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _scriptingDefine);
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);

        PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0_Subset;
        PlayerSettings.strippingLevel = StrippingLevel.Disabled;

        PlayerSettings.Android.keystoreName = Application.dataPath + "/Editor/AutoBuild/dragonnest.keystore";
        PlayerSettings.Android.keystorePass = "com.tencent.tmgp.dragonnest";
        PlayerSettings.Android.keyaliasName = "dragonnestawake";
        PlayerSettings.Android.keyaliasPass = "com.tencent.tmgp.dragonnest";
        PlayerSettings.Android.splashScreenScale = AndroidSplashScreenScale.ScaleToFill;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void RunGitPull()
    {
        Process p = new Process();

        if (System.Environment.OSVersion.Platform == PlatformID.Unix)
        {
            System.Diagnostics.Process.Start("/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal",
                "/Users/liming/Desktop/DN/dragon-nest/XProject/Assets/Editor/AutoBuild/gitpull.sh");
        }
        else
        {
            p.StartInfo.FileName = "sh";
            p.StartInfo.Arguments = Application.dataPath + "/Editor/AutoBuild/gitpull.sh";

            p.StartInfo.WorkingDirectory = Application.dataPath;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;

            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            if (String.IsNullOrEmpty(output) == false)
                UnityEngine.Debug.Log(output);
            p.WaitForExit();
        }
    }
    //static EditorBuildSettingsScene[] defaultScene;
}
