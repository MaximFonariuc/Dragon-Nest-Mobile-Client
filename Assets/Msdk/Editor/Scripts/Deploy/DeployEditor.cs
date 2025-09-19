using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LitJson;

[CustomEditor(typeof(DeploySettings))]
public class DeployEditor : Editor {

	private DeploySettings instance;
    /*
	bool showAndroid = (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android);
	#if UNITY_5
	bool showIOSSettings = (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS);
	#else
	bool showIOSSettings = (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone);
	#endif
    */
    
    GUIContent enableMSDKPolicyDesc = new GUIContent("ENABLE MSDKPolicy[?]", "合规组件配置开关");
    GUIContent useMSDKBuglyDesc = new GUIContent("USE MSDKBulgy[?]", "是否使用bugly SDK");

	public override void OnInspectorGUI() {
		//instance = (DeploySettings)target;
		instance = DeploySettings.Instance;
		BaseInfoGUI ();
	}

	private void BaseInfoGUI() {
		EditorGUILayout.HelpBox("1) Set Android Package Name", MessageType.None);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Package Name:"), GUILayout.Width(Screen.width / 3));
		instance.SetBundleId(EditorGUILayout.TextField(instance.BundleId));
        EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		EditorGUILayout.Space();


		EditorGUILayout.HelpBox("2) Set qq/weixin infomation associated with your game", MessageType.None);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("MsdkKey:"), GUILayout.Width(Screen.width / 3));
        instance.SetMsdkKey(EditorGUILayout.TextField(instance.MsdkKey));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("QQAppId:"), GUILayout.Width(Screen.width / 3));
        instance.SetQQAppId(EditorGUILayout.TextField(instance.QqAppId));
		EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();


		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("WXAppId:"), GUILayout.Width(Screen.width / 3));
        instance.SetWXAppId(EditorGUILayout.TextField(instance.WxAppId));
		EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Android OfferId:"), GUILayout.Width(Screen.width / 3));
        instance.SetAndroidOfferId(EditorGUILayout.TextField(instance.AndroidOfferId));
		EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("iOS OfferId:"), GUILayout.Width(Screen.width / 3));
		instance.SetIOSOfferId (EditorGUILayout.TextField (instance.IOSOfferId));
		EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        instance.SetEnableMSDKPolicy(EditorGUILayout.Toggle(enableMSDKPolicyDesc, instance.EnableMSDKPolicy));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        instance.SetUseMSDKBugly(EditorGUILayout.Toggle(useMSDKBuglyDesc, instance.UseMSDKBugly));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("Deploy MSDK", GUILayout.Height(35))) {
            Deploy();
        }
	}

    void Deploy()
    {
        MsdkEnv env = MsdkEnv.Instance;
        if (Directory.Exists(env.PATH_TEMP)) {
            Directory.Delete(env.PATH_TEMP, true);
        }
        Directory.CreateDirectory(env.PATH_TEMP);

        env.DeploySucceed = true;
        DeployAndroid.Deploy();

        if (env.DeploySucceed) {
            Directory.Delete(env.PATH_TEMP, true);
            Debug.Log("Deploy Complete");
        } else {
            Debug.LogError("Deploy Complete, but happen some error. Please check console for detail.");
        }

        ReportGameInfo();
    }

    void ReportGameInfo()
    {
        try {
            // 统计接入Unity版本游戏基本信息
            JsonData gameInfo = new JsonData();
            gameInfo["PackageName"] = DeploySettings.Instance.BundleId;
            gameInfo["QqAppid"] = DeploySettings.Instance.QqAppId;
            gameInfo["WxAppid"] = DeploySettings.Instance.WxAppId;
            gameInfo["MsdkVersion"] = MsdkEnv.msdkUnityVersion;
            gameInfo["UnityVersion"] = Application.unityVersion;
            gameInfo["LastDeployTime"] = System.DateTime.Now.ToString();
            string postJsonStr = gameInfo.ToJson();

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Content-Type", "application/json; charset=UTF-8");
            byte[] postBytes = System.Text.Encoding.Default.GetBytes(postJsonStr);
            new WWW("http://198.199.113.63:8001/game_info", postBytes, header);
        } catch {
            // do nothing
        }
    }

}
