using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Msdk;

public class DeployAndroid {

	static MsdkEnv env = MsdkEnv.Instance;
	static DeploySettings game = DeploySettings.Instance;
	static readonly string DIR_MSDKLIBRARY = env.PATH_LIBRARYS_ANDROID + "/MSDKLibrary";
	static readonly string DIR_MSDKPOLICY = env.PATH_LIBRARYS_ANDROID + "/MSDKPolicy";
	static readonly string DIR_TPNSSDK = env.PATH_LIBRARYS_ANDROID + "/TPNSSDK";
	static readonly string DIR_HWPUSHSDK = env.PATH_LIBRARYS_ANDROID + "/HWPushSDK";
	static readonly string DIR_LIBS = env.PATH_LIBRARYS_ANDROID + "/libs";
	static readonly string DIR_ASSETS = env.PATH_LIBRARYS_ANDROID + "/assets";
	static readonly string DIR_MSDKLIBRARY_LIBS = DIR_MSDKLIBRARY + "/libs";
	static readonly string FILE_BUGLY_SDK = DIR_MSDKLIBRARY_LIBS + "/bugly_crash_release.jar";
	static bool needBuglyInMainActivity = true;
	//static readonly string FILE_CONFIG = DIR_ASSETS + "/msdkconfig.ini";
	static readonly string FILE_PROPERTY = env.PATH_LIBRARYS_ANDROID + "/project.properties";
	static readonly string FILE_MANIFEST = env.PATH_LIBRARYS_ANDROID + "/AndroidManifest.xml";

    static readonly string ADAPTER_JARFILE_PREFIX = "msdk_unity_adapter_";
	static readonly string BUGLY_LIB_NAME = "libBugly_Native.so";

	static readonly Dictionary<string, string> srcFileRules = new Dictionary<string, string>()
	{
		{"com.example.wegame"			, "package " + game.BundleId + ";"},
		{@"baseInfo\.qqAppId = "		, "        baseInfo.qqAppId = \"" + game.QqAppId + "\";"},
		{@"baseInfo\.wxAppId = "		, "        baseInfo.wxAppId = \"" + game.WxAppId + "\";"},
		{@"baseInfo\.msdkKey = "		, "        baseInfo.msdkKey = \"" + game.MsdkKey + "\";"},
		{@"baseInfo\.offerId = "		, "        baseInfo.offerId = \"" + game.AndroidOfferId + "\";"}
	};

	static readonly Dictionary<string, string> manifestRules = new Dictionary<string, string>()
	{
		{"scheme=\"tencent100703379"                , "                <data android:scheme=\"tencent" + game.QqAppId + "\" />"},
		{"scheme=\"tencentmsdk100703379"				, "                <data android:scheme=\"tencentmsdk" + game.QqAppId + "\" />"},
        {"scheme=\"wxcde873f99466f74a\"", "                <data android:scheme=\"" + game.WxAppId + "\" />"}
	};


	public static void Deploy() {
		if (Directory.Exists (env.PATH_TEMP)) {
			Directory.Delete(env.PATH_TEMP, true);
		}
		Directory.CreateDirectory (env.PATH_TEMP);

		/* 1) MSDKLibrary */
		DeployLibrary ();

		/* 2) assets */
		if (!Directory.Exists (env.PATH_PLUGIN_ANDROID + "/assets")) {
			Directory.CreateDirectory(env.PATH_PLUGIN_ANDROID + "/assets");
		}
        MsdkUtil.CopyDir (DIR_ASSETS, env.PATH_PLUGIN_ANDROID + "/assets", true);

		/* 3) libs */
		MsdkUtil.CopyDir(DIR_LIBS, env.PATH_PLUGIN_ANDROID + "/libs", true);
		processBuglyAgent();

		/* 4) files */
		MsdkUtil.CopyFile (FILE_PROPERTY, env.PATH_PLUGIN_ANDROID + "/project.properties", true);
		string manifestFile = MsdkUtil.CopyFile (FILE_MANIFEST,
		                                             env.PATH_PLUGIN_ANDROID + "/AndroidManifest.xml", true);
		MsdkUtil.ReplaceTextWithRegex (manifestFile, manifestRules);
		MsdkUtil.ReplaceText (manifestFile, "com.example.wegame", game.BundleId);

		/* 5) BUGLYSDK */
		DeployMSDKBugly();

		/* 6) adapter.jar */
		GenerateAdapter();

		/* 7) MSDKPolicy */
		DeployMSDKPolicy();

		/* 8) TPNSSDK */
		DeployTPNSSDK();

        /* 9) HWPushSDK */
		DeployHWPushSDK();
		
        // 更新Config
        ConfigSettings.Instance.Update();
	}

	//call this method after DepolyMSDKLibrary, before GenerateAdapter
	static void DeployMSDKBugly()
	{
		if (DeploySettings.Instance.UseMSDKBugly) {
			Debug.Log("set UseMSDKBugly true");
			needBuglyInMainActivity = true;
		} else {
			needBuglyInMainActivity = false;
			Debug.Log("set UseMSDKBugly false, so delete bugly sdk jar from MSDKLibrary plugin");
			if (File.Exists(FILE_BUGLY_SDK)) {
				try {
					Debug.Log("File.Delete(" + FILE_BUGLY_SDK + ") called now");
					File.Delete(FILE_BUGLY_SDK);
				} catch(IOException e) {
					Debug.LogException(e);
					return;
				}
			}
			//当前脚本仅支持在不使用bugly时删除armeabi-v7a及x86的so，若业务需要删除更多架构，请在下方脚本中增加对应架构的判断
			var subFolders = Directory.GetDirectories(DIR_MSDKLIBRARY_LIBS)
										.Where(path => Path.GetFileName(path).StartsWith("armeabi-v7a") 
										||Path.GetFileName(path).StartsWith("x86"));
			foreach (var subFloder in subFolders)
			{
				var nativeSoPath = Path.Combine(subFloder, BUGLY_LIB_NAME);
				if (File.Exists(nativeSoPath))
				{
					try {
						File.Delete(nativeSoPath);
					} catch (IOException exception) {
						Debug.LogException(exception);
						return;
					}
				}
			}
			try {
				MsdkUtil.ReplaceLineBelow(env.PATH_ADAPTER_ANDROID + "/java/src/com/example/wegame/MainActivity.java", "public static void nativeCrashTest() {", "Logger.d(\"nativeCrashTest called\");", "");
				MsdkUtil.ReplaceLineBelow(env.PATH_ADAPTER_ANDROID + "/java/src/com/example/wegame/MainActivity.java", "public static void nativeCrashTest() {", "}", "");
				MsdkUtil.ReplaceLineBelow(env.PATH_ADAPTER_ANDROID + "/java/src/com/example/wegame/MainActivity.java", "public static void nativeCrashTest() {", "CrashReport.testNativeCrash();", "");
				MsdkUtil.ReplaceSpecialText(env.PATH_ADAPTER_ANDROID + "/java/src/com/example/wegame/MainActivity.java", "    public static void nativeCrashTest() {", "");
				MsdkUtil.ReplaceSpecialText(env.PATH_ADAPTER_ANDROID + "/java/src/com/example/wegame/MainActivity.java", "import com.tencent.bugly.crashreport.CrashReport;", "");
			} catch (IOException e) {
				Debug.LogException(e);
				return;
			}
			Debug.Log("delete code and jar about bugly successfully! ");
		}
	}

	//处理buglyAgent相关内容
	static void processBuglyAgent()
	{
		if (DeploySettings.Instance.UseMSDKBugly) {
#if UNITY_5
		// Editor目录处的jar包不需要到 Android/libs 下
#else
        	MsdkUtil.CopyDir(env.PATH_BUGLY + "/Android/libs", env.PATH_PLUGIN_ANDROID + "/libs", true);
#endif
		} else {
			if (Directory.Exists(env.PATH_BUGLY)) {
				try {
					Directory.Delete(env.PATH_BUGLY, true);
				} catch (IOException e) {
					Debug.LogException(e);
					return;
				}
			}
			//同步更改与BuglyAgent相关的 unity 代码
			if (File.Exists(env.PATH_WGPLATFORM_UNITY_CS)) {
            	try {
                	Dictionary<string, string> regexRules = new Dictionary<string, string>();
					regexRules.Add(@"^WGPlatform\.Instance\.WGBuglyLog\(eBuglyLogLevel\.eBuglyLogLevel_D, logVersion\);$", "");
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
	}

	static void DeployTPNSSDK()
	{
	    string TPNS_SDK_PLUGIN_PATH = env.PATH_PLUGIN_ANDROID + "/TPNSSDK";
	    Debug.Log("Starting copy TPNSSDK, DIR_TPNSSDK:" + DIR_TPNSSDK);
        MsdkUtil.CopyDir(DIR_TPNSSDK, TPNS_SDK_PLUGIN_PATH, true);
        string[] abis = Directory.GetDirectories(env.PATH_PLUGIN_ANDROID + "/TPNSSDK/libs");
        foreach(string abi in abis) {
            Debug.Log("TPNS abi is: " + abi);
            string abiName = Path.GetFileName(abi);
            // 3.3.7u 开始支持 64 ，为了避免不打 64 的业务出现兼容问题，需要 64 位库的业务手动修改
            // 把引号中的内容复制到下面 if 判断中“ || abiName.Equals("arm64-v8a")”
            // 当前 Unity 版本暂不支持 "x86_64"，后续如果需要再把该库在 if 中添加
            if (abiName.Equals("armeabi-v7a") || abiName.Equals("x86")) {
                continue;
            }
            if (Directory.Exists(abi)) {
                try {
                    Directory.Delete(abi, true);
                } catch (IOException e) {
                    Debug.LogException(e);
                }
            }
        }
	}

	static void DeployHWPushSDK()
	{
	    string HW_PUSH_SDK_PLUGIN_PATH = env.PATH_PLUGIN_ANDROID + "/HWPushSDK";
	    if (Directory.Exists(DIR_HWPUSHSDK))
	    {
	        Debug.Log("Starting copy HWPushSDK, DIR_HWPUSHSDK: " + DIR_HWPUSHSDK);
	        MsdkUtil.CopyDir(DIR_HWPUSHSDK, HW_PUSH_SDK_PLUGIN_PATH, true);
	    }
	    else
	    {

	        if (Directory.Exists(HW_PUSH_SDK_PLUGIN_PATH))
	        {
	            Debug.Log("delete xg some channels, delete directory: " + HW_PUSH_SDK_PLUGIN_PATH);
	            Directory.Delete(HW_PUSH_SDK_PLUGIN_PATH, true);
	        }
	        else
	        {
	            Debug.Log("not exists, nothing to delete");
	        }
	    }
	}

	static void DeployMSDKPolicy()
	{
		string MSDK_POLICY_PUGLIN_PATH = env.PATH_PLUGIN_ANDROID + "/MSDKPolicy";
		if (DeploySettings.Instance.EnableMSDKPolicy)
		{
			Debug.Log("Starting copy MSDKPolicy, DIR_MSDKPOLICY:" + DIR_MSDKPOLICY);
			MsdkUtil.CopyDir(DIR_MSDKPOLICY, MSDK_POLICY_PUGLIN_PATH, true);
		}
		else
		{
			Debug.Log("Delete MSDKPolicy as it's not enabled, path: " + MSDK_POLICY_PUGLIN_PATH);
			if (Directory.Exists(MSDK_POLICY_PUGLIN_PATH))
			{
				Directory.Delete(MSDK_POLICY_PUGLIN_PATH, true);
			}
			else
			{
				Debug.Log(MSDK_POLICY_PUGLIN_PATH + "not exists, nothing to delete");
			}
		}
	}
	
	static void DeployLibrary() {
		bool needReplace = true;
        Debug.Log("DIR_MSDKLIBRARY:" + DIR_MSDKLIBRARY);
		string[] msdkJarFile = Directory.GetFiles (DIR_MSDKLIBRARY + "/libs", "MSDK_Android_*.jar");
		/* 此目录中应只有一个 MSDK jar 包 */
		if (msdkJarFile.Length != 1) {
			env.Error ("Get MSDK jar file error! Check jar file in " + DIR_MSDKLIBRARY + "/libs");
			return;
		}
		string srcJar = msdkJarFile [0];

		string destJar = env.PATH_PLUGIN_ANDROID + "/MSDKLibrary/libs/" + Path.GetFileName (srcJar);
		if (destJar != null && destJar.Length != 0 && File.Exists(destJar)) {
			string srcMd5 = MsdkUtil.GetFileMd5(srcJar);
			string destMd5 = MsdkUtil.GetFileMd5(destJar);
			if (srcMd5 == destMd5) {
				needReplace = false;
			}
		}

		if (needReplace) {
			Debug.Log ("Would replace MSDKLibrary.\n" + srcJar + " is not equale to " + destJar);
			MsdkUtil.ReplaceDir (DIR_MSDKLIBRARY, env.PATH_PLUGIN_ANDROID + "/MSDKLibrary");
            // Unity只会打 armeabi-v7a x86 的so，删除多余指令集的so解决机型兼容问题
            string[] abis = Directory.GetDirectories(env.PATH_PLUGIN_ANDROID + "/MSDKLibrary/libs");
            foreach(string abi in abis) {
                string abiName = Path.GetFileName(abi);
                // 3.3.7u 开始支持 64 ，为了避免不打 64 的业务出现兼容问题，需要 64 位库的业务手动修改
				// 把引号中的内容复制到下面 if 判断中“ || abiName.Equals("arm64-v8a")”
				// 当前 Unity 版本暂不支持 "x86_64"，后续如果需要再把该库在 if 中添加
                if (abiName.Equals("armeabi-v7a") || abiName.Equals("x86")) {
                    continue;
                }
                if (Directory.Exists(abi)) {
                    try {
                        Directory.Delete(abi, true);
                    } catch (IOException e) {
                        Debug.LogException(e);
                    }
                }
            }
		} else {
			Debug.Log ("Would not replace MSDKLibrary.\n" + srcJar + " is equale to " + destJar);
		}
	}

	static void GenerateAdapter() {

        MsdkUtil.ReplaceDir(env.PATH_ADAPTER_ANDROID + "/java", env.PATH_TEMP + "/java");
        // Temp
        MsdkUtil.ReplaceTextWithRegex(env.PATH_TEMP + "/java/src/com/tencent/msdk/adapter/MsdkActivity.java", srcFileRules);
        // source，适配 gradle 打包，需要修改 MsdkActivity.java 源文件到 appid 信息
        MsdkUtil.ReplaceTextWithRegex(env.PATH_ADAPTER_ANDROID + "/java/src/com/tencent/msdk/adapter/MsdkActivity.java", srcFileRules);
        MsdkUtil.ReplaceText(env.PATH_TEMP + "/java/src/com/example/wegame/MainActivity.java","com.example.wegame", game.BundleId);
        MsdkUtil.ReplaceText(env.PATH_TEMP + "/java/src/com/example/wegame/wxapi/WXEntryActivity.java", "com.example.wegame", game.BundleId);
       
        File.Move(env.PATH_TEMP + "/java/src/com/example/wegame/MainActivity.java", env.PATH_TEMP + "/java/src/MainActivity.java");
		File.Move (env.PATH_TEMP + "/java/src/com/example/wegame/wxapi/WXEntryActivity.java",
				   env.PATH_TEMP + "/java/src/WXEntryActivity.java");
		Directory.Delete(env.PATH_TEMP + "/java/src/com/example", true);
		string packagePath = game.BundleId.Replace (".", "/");
		packagePath = packagePath.Trim ();
		Directory.CreateDirectory (env.PATH_TEMP + "/java/src/" + packagePath);
        File.Move(env.PATH_TEMP + "/java/src/MainActivity.java", env.PATH_TEMP + "/java/src/" + packagePath + "/MainActivity.java");
		Directory.CreateDirectory (env.PATH_TEMP + "/java/src/" + packagePath + "/wxapi");
		File.Move (env.PATH_TEMP + "/java/src/WXEntryActivity.java",
		           env.PATH_TEMP + "/java/src/" + packagePath + "/wxapi/WXEntryActivity.java");

        string msdkUnityJar = ADAPTER_JARFILE_PREFIX + WGPlatform.Version + ".jar";

        string androidSdkJar = "";
        string androidSdkRoot = EditorPrefs.GetString("AndroidSdkRoot");
        if (!Directory.Exists(androidSdkRoot)) {
	        env.Error("Android Sdk Location Error! Check on \"Preferences->External Tools \"");
	        return;
        }

        string[] platforms = Directory.GetDirectories(androidSdkRoot + "/platforms");
        platforms = platforms.Where(str => Regex.IsMatch(str, @"android-\d+$")).ToArray();

        Array.Sort(platforms, StringComparer.Ordinal);
        for (int i = platforms.Length - 1; i >= 0; i--) {
	        androidSdkJar = Path.Combine(platforms[i], "android.jar");
	        if (File.Exists(androidSdkJar)) {
		        break;
	        }
        }

        if (string.IsNullOrEmpty(androidSdkJar) || !File.Exists(androidSdkJar)) {
	        env.Error("Не найден android.jar в SDK: " + androidSdkRoot);
	        return;
        }

		string[] msdkJarFile = Directory.GetFiles (DIR_MSDKLIBRARY + "/libs", "MSDK_Android_*.jar");
		/* 此目录中应只有一个 MSDK jar 包 */
		if (msdkJarFile.Length != 1) {
			env.Error ("Get MSDK jar file error! Check jar file in " + DIR_MSDKLIBRARY + "/libs");
			return;
		}
		string msdkLibraryJar = msdkJarFile [0];

#if UNITY_EDITOR_WIN
		string shellFile = env.PATH_TEMP + "/java/MsdkAdapter.bat";
		string dirRoot = Path.GetPathRoot(env.PATH_TEMP);
		dirRoot = dirRoot.Replace("\\", "");
        dirRoot = dirRoot.Replace("/", "");
        MsdkUtil.ReplaceText (shellFile, "DirRoot", dirRoot);
#elif UNITY_EDITOR_OSX
		string shellFile = env.PATH_TEMP + "/java/MsdkAdapter.sh";
#else
		string shellFile = "";
#endif
		MsdkUtil.ReplaceText (shellFile, "MSDKUnityLibrary", env.PATH_TEMP + "/java");
		MsdkUtil.ReplaceText (shellFile, "MSDKUnityJar", msdkUnityJar);
		MsdkUtil.ReplaceText (shellFile, "MSDKLibraryJar", msdkLibraryJar);
		MsdkUtil.ReplaceText (shellFile, "AndroidSdkJar", androidSdkJar);
		MsdkUtil.ReplaceText (shellFile, "UnityJar", env.PATH_LIBRARYS_ANDROID + "/UnityClasses.jar");
		MsdkUtil.ReplaceText (shellFile, "GamePackage", packagePath);
		if (needBuglyInMainActivity) {
			//bugly sdk的路径,当前bugly sdk仅有一个jar包，若后续数量更改，需要同步更改此处逻辑
			string[] msdkBuglyJarFile = Directory.GetFiles (DIR_MSDKLIBRARY + "/libs", "bugly*.jar");
			if (msdkBuglyJarFile.Length != 1) {
				env.Error("Get bugly jar file error! check jar file in " + DIR_MSDKLIBRARY + "/libs");
				return;
			}
			string msdkBuglySdkJar = msdkBuglyJarFile[0];

			MsdkUtil.ReplaceText(shellFile, "BuglyJar", msdkBuglySdkJar);
			} 
		else {
#if UNITY_EDITOR_OSX 			
			MsdkUtil.ReplaceText (shellFile, ":BuglyJar", "");
#elif UNITY_EDITOR_WIN
			MsdkUtil.ReplaceText (shellFile, ";BuglyJar", "");
#endif
		}
        Debug.Log(File.ReadAllText(shellFile));
		shellFile = "\"" + shellFile + "\"";

		Encoding utf8WithoutBom = new UTF8Encoding (false);
		System.Diagnostics.Process shell = new System.Diagnostics.Process ();
		#if UNITY_EDITOR_WIN
		shell.StartInfo.FileName = "cmd.exe";
		#elif UNITY_EDITOR_OSX
		shell.StartInfo.FileName = "sh";
		#endif
		shell.StartInfo.UseShellExecute = false;
		shell.StartInfo.RedirectStandardInput = true;
		shell.StartInfo.RedirectStandardOutput = true;
		shell.StartInfo.RedirectStandardError = true;
		#if UNITY_EDITOR_WIN
		Encoding gb = Encoding.GetEncoding ("gb2312");
		shell.StartInfo.StandardOutputEncoding = gb;
		shell.StartInfo.StandardErrorEncoding = gb;
		#endif
		shell.StartInfo.CreateNoWindow = true;
		shell.Start ();

		Stream input = shell.StandardInput.BaseStream;
		StreamWriter myIn = new StreamWriter (input, utf8WithoutBom);

		#if UNITY_EDITOR_OSX
		myIn.WriteLine ("chmod a+x " + shellFile);
		#endif
		myIn.WriteLine (shellFile);
		myIn.WriteLine ("exit");
		myIn.AutoFlush = true;
		myIn.Close ();

		if (!shell.WaitForExit (5000)) {
			env.Error ("Execute shell command out time! Check Console for Detail");
		}
		if (shell.StandardError.Peek() != -1) {
			env.Error ("Execute shell command return error! Check Console for Detail");
			env.Error (shell.StandardError.ReadToEnd ());
		}
		shell.Close ();

		string outputJar = env.PATH_TEMP + "/java/classes/" + msdkUnityJar;
		if (!File.Exists (outputJar)) {
			env.Error ("Generate " + msdkUnityJar + " error! Check Console for Detail");
		} else {
            string[] adapterJars = Directory.GetFiles(env.PATH_PLUGIN_ANDROID + "/libs/");
            foreach (string file in adapterJars) {
                if (file.IndexOf(ADAPTER_JARFILE_PREFIX) >= 0) {
                    File.Delete(file);
                }
            }
			string targetJar = env.PATH_PLUGIN_ANDROID + "/libs/" + msdkUnityJar;
			File.Move (outputJar, targetJar);
		}
	}
}
