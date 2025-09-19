using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Reflection;
using System.Collections.Generic;
using LitJson;

class CheckConfig : EditorWindow
{
    static CheckConfigBase check;
    static int windowWidth = 800;
    static int windowHeight = 600;
    static string checkResultFile = MsdkEnv.Instance.PATH_EDITOR + "/Resources/CheckResult.txt";

    CheckResult result = null;

    CheckResult Result
    {
        get
        {
            if (result == null) {
                result = CheckResult.ReadFrom(checkResultFile);
            }
            return result;
        }
    }

    #if UNITY_EDITOR
    [PostProcessBuild(1000)]
    public static void OnPostProcessBuild( BuildTarget target, string pathToBuiltProject)
    {
        if (!ConfigSettings.Instance.CheckConfig) {
            return;
        }
        Debug.Log("CheckConfig PostProcessBuild");
	#if UNITY_5||UNITY_2017||UNITY_2017_1_OR_NEWER
        if (target == BuildTarget.iOS) {
    #else
        if (target == BuildTarget.iPhone) {
    #endif
            check = new CheckConfigIOS(pathToBuiltProject);
        } else if (target == BuildTarget.Android) {
            check = new CheckConfigAndroid(pathToBuiltProject);
        }
        check.Check();
        CheckResult.WriteTo(checkResultFile, check.result);

        CheckResult testResult = CheckResult.ReadFrom(checkResultFile);
        if (testResult == null) {
            Debug.LogError("Get check result error!");
        } else {
            ShowResult();
        }
    }

    static void ShowResult()
    {
        CheckConfig window = (CheckConfig)EditorWindow.GetWindow(typeof(CheckConfig));
        window.position = new Rect((Screen.currentResolution.width - windowWidth) / 2,
            (Screen.currentResolution.height - windowHeight) / 2, windowWidth, windowHeight);

        string title = check.result.platform + "配置检查";
        try {
            if (MsdkUtil.isUnityEarlierThan("5.1")) {
                PropertyInfo info = window.GetType().GetProperty("title");
                info.SetValue(window, title, null);
            } else {
                PropertyInfo info = window.GetType().GetProperty("titleContent");
                info.SetValue(window, new GUIContent(title), null);
            }
        } catch (Exception e) {
            Debug.LogException(e);
        }
        
        window.Show();
    }



    public Vector2 scrollPos = Vector2.zero;

    void OnGUI ()
    {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.wordWrap = true;
            style.stretchWidth = false;

            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            GUILayout.Label("若要关闭此配置检查功能，可在菜单 \"MSDk->Cofing Settings\" 不勾选 \"Check Config\".");
            EditorGUILayout.Space();
            if (Result.domain.IndexOf("release") == -1) {
                GUILayout.Label("环境：测试环境(" + Result.domain + ")");
                style.normal.textColor = Color.red;
                GUILayout.Label("   注意：您配置了MSDK测试环境，请勿将此作为正式版本发布！", style);
            } else {
                GUILayout.Label("环境：正式环境(" + Result.domain + ")");
            }

            GUILayout.Label("模块开关：（可在菜单 \"MSDk->Cofing Settings\" 中配置模块开关）");
            ModuleSwitch("信鸽推送", Result.push, style);
            ModuleSwitch("公告", Result.notice, style);
            ModuleSwitch("微信自动刷新", Result.refresh, style);
            if ("Android".Equals(Result.platform)) {
                ModuleSwitch("应用宝抢号", Result.beta, style);
                ModuleSwitch("省流量更新", Result.update, style);
                ModuleSwitch("白名单限号", Result.grayTest, style);
                ModuleSwitch("Bugly crash上报", !Result.closeBugly, style);
                ModuleSwitch("灯塔详细日志打印", Result.statLog, style);
                ModuleSwitch("v2签名", Result.v2sign, style);

                GUILayout.BeginHorizontal();
                GUILayout.Label("   MSDK日志级别：", GUILayout.Width(120));
                style.normal.textColor = Color.green;
                GUILayout.Label("" + Result.logLevel, style);
                GUILayout.EndHorizontal();
                ModuleSwitch("信鸽厂商渠道开关", result.xgOtherPushEnable, style);
            }

            EditorGUILayout.Space();

            if (!String.IsNullOrEmpty(Result.resultError)) {
                GUILayout.Label("错误：");
                style.normal.textColor = Color.red;
                GUILayout.Label(Result.resultError, style);
            }
            if (!String.IsNullOrEmpty(Result.resultWarnning)) {
                GUILayout.Label("警告：");
                style.normal.textColor = Color.yellow;
                GUILayout.Label(Result.resultWarnning, style);
            }
            if (String.IsNullOrEmpty(Result.resultError) && String.IsNullOrEmpty(Result.resultWarnning)) {
                GUILayout.Label("检查完成，恭喜您配置无误.");
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
    }

    void ModuleSwitch(string moduleName, bool value, GUIStyle style)
    {
        GUILayout.BeginHorizontal();
        if (value) {
            GUILayout.Label("   " + moduleName + "：", GUILayout.Width(120));
            style.normal.textColor = Color.green;
            GUILayout.Label("开", style);

        } else {
            GUILayout.Label("   " + moduleName + "：", GUILayout.Width(120));
            style.normal.textColor = Color.red;
            GUILayout.Label("关", style);
        }
        GUILayout.EndHorizontal();
    }
    #endif
}
