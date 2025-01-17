//
// EditorHelper.cs
// Created by huailiang.peng on 2016/03/18 11:20:38
//

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class HelperEditor : MonoBehaviour 
{

	public static string basepath
	{
		get
		{
			string path=Application.dataPath;
			path =path.Remove(path.IndexOf("/Assets"));
			return path;
		}
	}


    [MenuItem("LuaTools/Helper/OpenHotfixCacheDiectory  #&h")]
	public static void OpenHotfix()
	{
		Open(Application.temporaryCachePath+"/Hotfix");
	}

    [MenuItem("LuaTools/Helper/OpenAssetsBundleDirectory")]
	public static void OpenAssetbundle()
	{
		Open(basepath+"/AssetsBundle");
	}

    [MenuItem("LuaTools/Helper/CleanCache")]
	public static void CleanCache()
	{
		Caching.ClearCache();
	}

    [MenuItem("LuaTools/Helper/PlayerPrefsDeleteAll")]
	public static void PlayerPrefsDeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}

	public static void Open(string path)
	{
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
#if UNITY_EDITOR_OSX
		string shell =basepath+"/Shell/open.sh";
//		Debug.Log("shell: "+shell+" exist: "+System.IO.File.Exists(shell));
		string arg= path;
		string ex=shell+" "+arg;
		System.Diagnostics.Process.Start("/bin/bash", ex);
#elif UNITY_EDITOR_WIN
        path = path.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer.exe", path);
#endif
	}
}
