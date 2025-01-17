using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;


/// <summary>
/// author huailiang.peng
/// 热修资源打包工具
/// </summary>

public class AssetbundleEditor : EditorWindow
{

    private int platformInt = 1;
    private string[] platformString = new string[] { "Android", "IOS", "WP", "WIN", "OSX" };
    private int[] platArray = new int[] { 0, 1, 2, 3, 4, 5 };

    public static string selectPath = Application.dataPath;
    public static int hotfixVersion = 0;
    public static int currHotfixVersion = 0;
    public static AssetbundleEditor  window;



    [MenuItem("LuaTools/HotfixMaker #&p")]
    private static void Init()
    {
        selectPath = selectPath.Remove(selectPath.Length-6)+"AssetsBundle/";
        window = (AssetbundleEditor)GetWindow(typeof(AssetbundleEditor), true, "热修资源打包工具");
        window.Show();
        currHotfixVersion = PlayerPrefs.GetInt("hotfixversion",0);
        hotfixVersion=currHotfixVersion+1;
    }


    private void OnGUI()
    {
        GUILayout.Label("设置一些打包参数");
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        platformInt = EditorGUILayout.IntPopup("选择平台: ", platformInt, platformString, platArray);
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("选择路径"))
        {
            selectPath = GetSaveDirPath();
            Debug.Log("select path: "+selectPath);
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.TextField(selectPath);
        GUILayout.EndHorizontal();

        GUILayout.Space(25);

        GUILayout.Label("选择要打包的内容");
        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.Label("选择单个打包");
        if (GUILayout.Button("Package"))
        {
            BuildSingleAsset(GetBuildTarget());
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUI.backgroundColor = Color.red;
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("选择多个打包");
        if (GUILayout.Button("Package"))
        {
            BuildMultAsset(GetBuildTarget());
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Hotfix   打包");
        if (GUILayout.Button("Package"))
        {
            PackHotFixed(GetBuildTarget());
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        GUI.backgroundColor = Color.white;
        GUILayout.BeginHorizontal();
        GUILayout.Label("curr hotfix version: "+currHotfixVersion);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        GUILayout.Label("Hotfix Version");
        hotfixVersion =  EditorGUILayout.IntField(hotfixVersion);
        GUI.backgroundColor = Color.white;
        if (GUILayout.Button("生成热修配置"))
        {
            Debug.Log("hotfix: "+hotfixVersion);
            HotfixToolEditor.MakeHotfixConfig(hotfixVersion,selectPath);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
       
    }
   

    public BuildTarget GetBuildTarget()
    {
        if (platformInt == 0)
        {
            return BuildTarget.Android;
        }
        else if (platformInt == 1)
        {
            return BuildTarget.iOS;
        }
        //else if (platformInt == 2)
        //{
        //    return BuildTarget.WP8Player;
        //}
        else if (platformInt == 3)
        {
            return BuildTarget.StandaloneWindows;
        }
        else if (platformInt == 4)
        {
            return BuildTarget.StandaloneOSX;
        }

        return BuildTarget.Android;
    }
   
    public static string GetSaveDirPath()
    {
        return EditorUtility.SaveFolderPanel("选择目录", selectPath, "select");
    }

    // output single file
    public static void BuildSingleAsset(BuildTarget target)
    {
        string path = AssetbundleEditor.selectPath;
        if (path.Length != 0)
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object),SelectionMode.DeepAssets);
            if(selection==null){ EditorUtility.DisplayDialog("提示", "NO SELECTION", "提示"); return; }
            else if (selection.Length>1) { EditorUtility.DisplayDialog("提示", "Please select single file!", "提示"); return; }
            Debug.Log("se: " + selection[0].name);
            //BuildPipeline.BuildAssetBundle(null, selection, path + "/" + selection[0].name+".assetbundle",
            //     BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies, target);
        }
    }

    //output multy files 
    public static void BuildMultAsset(BuildTarget target)
    {
        string path = AssetbundleEditor.selectPath;
        if (path.Length != 0)
        {
            UnityEngine.Object[] selections = Selection.GetFiltered(typeof(UnityEngine.Object),SelectionMode.DeepAssets);
            foreach(var item in selections)
            {
                Debug.Log("se: " + item.name);
                //BuildPipeline.BuildAssetBundle(item, null, path + "/" + item.name+".assetbundle",
                //     BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies, target);
            }
        }
    }

    
  
    public static void PackHotFixed(BuildTarget target)
    {
        string[] files = HotfixToolEditor.GetHotfixFilesList();
		foreach(var path in files)
		{
			//UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path,typeof(UnityEngine.Object));
			//int index = path.LastIndexOf('/');
#if UNITY_EDITOR_WIN
            //index = path.LastIndexOf('\\');
#endif

			//int last=path.LastIndexOf('.');
			//string name=path.Substring(index+1,last-index-1);
			//Build(obj, AssetbundleEditor.selectPath, name, target);
            //Debug.Log("sele: "+selectPath+" name: "+name);
			//BuildPipeline.BuildAssetBundle(obj, null, AssetbundleEditor.selectPath + "/" + name + ".assetbundle",
   //              BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies, target);
			
		}
		EditorUtility.DisplayDialog("提示", "打包完成", "提示");
        HotfixToolEditor.TransPostfixLua();
        Caching.ClearCache();
		HelperEditor.OpenAssetbundle();
    }

    
    public static void Build(UnityEngine.Object obj, string path, string name, BuildTarget target)
    {
        if (!Directory.Exists(path)) { EditorUtility.DisplayDialog("提示", "不存在路径", "确定"); }
        else
        {
			

            //BuildPipeline.BuildAssetBundle(obj, null, path + "/" + name + ".assetbundle",
            //     BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies, target);
            

            EditorUtility.DisplayDialog("提示", "打包完成", "提示");
        }
    }


}
