using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
//using UnityEditor.XCodeEditor;
#endif
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class AndroidPostProcess {

	#if UNITY_EDITOR
	[PostProcessBuild(999)]
	public static void OnPostProcessBuild( BuildTarget target, string pathToBuiltProject )
	{
		
		if (target != BuildTarget.Android) {
			Debug.LogWarning("Target is not Android. AndroidPostProcess will not run");
			return;
		}

		string unityVersion = Application.unityVersion;
		Log ("" + unityVersion);
		if (unityVersion.StartsWith ("5.")) {
			// do nothing
		} else { // 4.x
			DoSomethingU4X(pathToBuiltProject);
		}
	}
	#endif

	public static void Log(string message)
	{
		UnityEngine.Debug.Log("Android PostProcess: "+message);
	}

	private static void DoSomethingU4X(string pathToBuiltProject){

		string rootBuglyPluginPath = Path.Combine(Application.dataPath, "Plugins/BuglyPlugins");

		if (Directory.Exists(rootBuglyPluginPath)) {
			string androidLibsPath = Path.Combine(rootBuglyPluginPath, "Android/libs");

			if (Directory.Exists(androidLibsPath)) {

				string targetProjectPath = Path.Combine(pathToBuiltProject, PlayerSettings.productName);
				if (Directory.Exists(targetProjectPath)) {
					CopyDirectory(androidLibsPath, Path.Combine(targetProjectPath, "libs"));
				}
			}
		}
	}

	private static void CopyDirectory(string sourcePath, string destinationPath)
	{
		DirectoryInfo info = new DirectoryInfo(sourcePath);
		Directory.CreateDirectory(destinationPath);
		foreach (FileSystemInfo  fsi in info.GetFileSystemInfos())
		{
			if (fsi.Name.EndsWith(".meta")) {
				continue;
			}
			string destName = Path.Combine(destinationPath, fsi.Name);
			if (fsi is System.IO.FileInfo) {
				File.Copy(fsi.FullName, destName);
			}      
			else                     
			{
				Directory.CreateDirectory(destName);
				CopyDirectory(fsi.FullName, destName);
			}
		}
	}
}
