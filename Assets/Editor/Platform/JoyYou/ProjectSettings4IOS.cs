using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor.XCodeEditor;
using System.IO;
using System.Xml;

namespace UnityEditor.JoyYouSDKEditor
{
	class ProjectSettings4IOS : ProjectSettings
	{
		public static string DefaultModsPath = null;
		public static string url_handle_filename = "handleURL.h";

		private const string configFileName = "JoyyouSDK.cfg";
		private const string NODE_NAME_ROOT = "JoyyouSDK";
		private const string Group_Path = "Directory";
		private const string Platform_IOS = "IOS";
		private static bool _HandleURL_Modify = false;
		private static bool _HandleURL_Extend = false;
		private static bool _supportOrientation = false;
		private static bool _Notification = false;
		private static bool _HandleUIView = false;

		private static void CreateSettings(ref XmlDocument doc)
		{
			if (doc == null)
			{
				doc = new XmlDocument();
			}
			// Now, doc != null
			{
				XmlNode nDecl = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
				doc.AppendChild(nDecl);
				XmlElement _root = doc.CreateElement(NODE_NAME_ROOT);
				doc.AppendChild(_root);

				string group = Group_Path;
				string platformName = Platform_IOS;

				XmlElement root = doc.DocumentElement;
				XmlNode node = root.FirstChild;
				XmlNode groupNode = null;
				string groupName = group.ToString();
				while (node != null)
				{
					if (node.Name == groupName)
					{
						groupNode = node;
						break;
					}

					node = node.NextSibling;
				}

				if (groupNode == null)
				{
					groupNode = doc.CreateElement(groupName);
					root.AppendChild(groupNode);
				}

				node = groupNode.FirstChild;
				XmlNode platformNode = null;
				while (node != null)
				{
					if (node.Name == platformName)
					{
						platformNode = node;
						break;
					}

					node = node.NextSibling;
				}

				if (platformNode == null)
				{
					platformNode = doc.CreateElement(platformName);
					groupNode.AppendChild(platformNode);
				}

				XmlElement element = platformNode as XmlElement;
				if (element != null)
				{
					element.SetAttribute("Path", UnityEditor.EditorUtility.OpenFolderPanel("选择Mods目录位置", "", "Mods"));
				}

				doc.Save(configFileName);
			}
		}

		private static string GetSettings_ModsPath(XmlDocument doc)
		{
			string sRet = "";

			if (doc != null)
			{
				string platformName = Platform_IOS;
				XmlElement root = doc.DocumentElement;
				XmlNode node = root.FirstChild;
				XmlNode groupNode = null;
				string groupName = Group_Path;
				while (node != null)
				{
					if (node.Name == groupName)
					{
						groupNode = node;
						break;
					}

					node = node.NextSibling;
				}

				if (groupNode == null)
				{
					groupNode = doc.CreateElement(groupName);
					root.AppendChild(groupNode);
				}

				node = groupNode.FirstChild;
				XmlNode platformNode = null;
				while (node != null)
				{
					if (node.Name == platformName)
					{
						platformNode = node;
						break;
					}

					node = node.NextSibling;
				}

				if (platformNode != null)
				{
					XmlElement element = platformNode as XmlElement;
					if (element != null)
					{
						sRet = element.Attributes["Path"].Value;
					}
				}
			}

			return sRet;
		}

		private static bool ExistsSettings(ref XmlDocument _doc)
		{
			/*
			string settingDir = Path.Combine(Application.dataPath, "Editor");
			if (!Directory.Exists(settingDir))
			{
				Directory.CreateDirectory(settingDir);
			}
			
			string path_xml = Path.Combine(settingDir, configFileName);
			*/

			bool isExistDoc = File.Exists(configFileName);
			string rootNodeName = NODE_NAME_ROOT;
			_doc = null;
			if (isExistDoc)
			{
				try {
					XmlDocument doc = new XmlDocument();
					doc.Load(configFileName);
					if (doc.DocumentElement.Name != rootNodeName)
					{
						isExistDoc = false;
					}
					else
					{
						_doc = doc;
					}
				} catch (System.Exception ex) {
					Debug.LogException(ex);

					isExistDoc = false;
				}
			}

			return isExistDoc;
		}

		static ProjectSettings4IOS()
		{
			if (DefaultModsPath == null)
			{
				XmlDocument doc = null;

				if (!ExistsSettings(ref doc))
				{
					CreateSettings(ref doc);
				}

				DefaultModsPath = GetSettings_ModsPath(doc);

				if (DefaultModsPath == null || DefaultModsPath == "")
				{
					Debug.LogError("No configuration file can be found, please check your settings!");

					return;
				}
			}
		}

		public virtual void ResetProjectByMode(BuildTarget target, string pathToBuiltProject, string modeFilename)
		{
			ResetProjectByMode(target, pathToBuiltProject, ProjectSettings4IOS.DefaultModsPath, modeFilename);
		}

		public virtual void ResetProjectByMode(BuildTarget target, string pathToBuiltProject, string path2Mods, string modeFilename)
		{
			if (target != BuildTarget.iOS)
			{
				Debug.LogWarning("Build target is not iPhone, XCodePostProcess will not be run!");

				return;
			}
			
			// Create a new project object from build target
			XCProject project = new XCProject(pathToBuiltProject);
			
			// Find and run through all .projmods files to patch the project
			// Note that all .projmods files in your project folder will be excuted
			string[] files = Directory.GetFiles(path2Mods, modeFilename, SearchOption.AllDirectories);
			foreach (string file in files)
			{
				UnityEngine.Debug.Log("The .projmods file is " + file);

				project.ApplyMod(file);
			}
			
			// Finally save the xcode project
			WritePlistFile(pathToBuiltProject);
			AddExtCode(pathToBuiltProject);
			AddExtBuildSettings(ref project);

			project.Save();
		}

		public virtual void WritePlistFile(string pathToBuiltProject) {}
		public virtual void PlistAddKey(XCPlist plist) {}
		public virtual void PlistAddURLScheme(XCPlist plist) {}
		public virtual void AddExtCode(string pathToBuiltProject) {}

		public static void AddExtCode_HandleURL_Modify(string pathToBuiltProject)
		{
			if (_HandleURL_Modify)
			{
				return;
			}

			_HandleURL_Modify = true;

			XClass UnityAppController = new XClass(pathToBuiltProject + "/Classes/UnityAppController.mm");

			// 在指定代码后面添加一行
			// 引入头文件
			UnityAppController.WriteBelow("#include \"PluginBase/AppDelegateListener.h\"", "#import \"" + url_handle_filename + "\"");

			// 替换指定某行代码
			// UnityAppController.Replace("return YES;", "return [ShareSDK handleOpenURL:url sourceApplication:sourceApplication annotation:annotation wxDelegate:nil];");

			// openURL接口
			UnityAppController.WriteBelow("AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);",
				"\t[SDKProcess application:application openURL:url sourceApplication:sourceApplication annotation:annotation];");

			// didFinishLaunchingWithOptions接口
			UnityAppController.WriteBelow("::printf(\"-> applicationDidFinishLaunching()\\n\");",
				"\t[SDKProcess application:application didFinishLaunchingWithOptions:launchOptions];");

			// applicationDidEnterBackground接口
			UnityAppController.WriteBelow("::printf(\"-> applicationDidEnterBackground()\\n\");",
				"\t[SDKProcess applicationDidEnterBackground:application];");

			// applicationWillEnterForeground接口
			UnityAppController.WriteBelow("::printf(\"-> applicationWillEnterForeground()\\n\");",
				"\t[SDKProcess applicationWillEnterForeground:application];");

			// applicationDidBecomeActive接口
			UnityAppController.WriteBelow("_didResignActive = false;",
				"\t[SDKProcess applicationDidBecomeActive:application];");

			/*
			// dealloc接口
			UnityAppController.WriteBelow("[self releaseViewHierarchy];",
				"\t[SDKProcess dealloc];");
			*/
		}

		public static void AddExtCode_HandleURL_Extend(string pathToBuiltProject)
		{
			if (_HandleURL_Extend)
			{
				return;
			}

			_HandleURL_Extend = true;

			XClass UnityAppController = new XClass(pathToBuiltProject + "/Classes/UnityAppController.mm");

			// 防止重复引入
			if (!_HandleURL_Modify)
			{
				UnityAppController.WriteBelow("#include \"PluginBase/AppDelegateListener.h\"", "#import \"" + url_handle_filename + "\"");
			}

			UnityAppController.WriteBelow("SensorsCleanup();\n}",
@"
- (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url
{
	// [SDKProcess application:application handleOpenURL:url];
	[SDKProcess application:application openURL:url sourceApplication:nil annotation:(id)0];

	return YES;
}");
		}

		public static void AddExtCode_supportOrientation(string path2BuildProject)
		{
			if (_supportOrientation)
			{
				return;
			}

			_supportOrientation = true;

			XClass UnityAppController = new XClass(path2BuildProject + "/Classes/UnityAppController.mm");

			string pos_line = "// Anyway this is intersected with values provided from UIViewController, so we are good";
			string content = "\t[SDKProcess application:application supportedInterfaceOrientationsForWindow:window];";
			UnityAppController.WriteBelow(pos_line, content);
		}

		// 推送相关
		public static void AddExtCode_Notification(string path2BuildProject)
		{
			if (_Notification)
			{
				return;
			}

			_Notification = true;

			XClass UnityAppController = new XClass(path2BuildProject + "/Classes/UnityAppController.mm");

			// didRegisterForRemoteNotificationsWithDeviceToken接口
			string pos_line = "UnitySendDeviceToken(deviceToken);";
			string content = "\n\t[SDKProcess application:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];";
			UnityAppController.WriteBelow(pos_line, content);

			// didFailToRegisterForRemoteNotificationsWithError接口
			pos_line = "UnitySendRemoteNotificationError(error);";
			content = "\n\t[SDKProcess application:application didFailToRegisterForRemoteNotificationsWithError:error];";
			UnityAppController.WriteBelow(pos_line, content);

			// didReceiveLocalNotification接口
			pos_line = "UnitySendLocalNotification(notification);";
			content = "\n\t[SDKProcess application:application didReceiveLocalNotification:notification];";
			UnityAppController.WriteBelow(pos_line, content);

			// didReceiveRemoteNotification接口
			pos_line = "UnitySendRemoteNotification(userInfo);";
			content = "\n\t[SDKProcess application:application didReceiveRemoteNotification:userInfo];";
			UnityAppController.WriteBelow(pos_line, content);
		}

		// 3D Touch相关
		public static void AddExtCode_UIView(string path2BuildProject)
		{
			if (_HandleUIView)
			{
				return;
			}
			
			_HandleUIView = true;
			
			XClass UnityAppController = new XClass(path2BuildProject + "/Classes/UI/UnityView.mm");
			
			// 在指定代码后面添加一行
			// 引入头文件
			UnityAppController.WriteBelow("#include \"Unity/UnityMetalSupport.h\"", "#import \"" + url_handle_filename + "\"");
			
			// touchesMoved接口
			string pos_line = "UnitySendTouchesMoved(touches, event);";
			string content = "\n\t[SDKProcess touchesMoved:touches withEvent:event];";
			UnityAppController.WriteBelow(pos_line, content);
		}

		public virtual void AddExtBuildSettings(ref XCProject project)
		{
			string releaseCSI = null;
			string debugCSI = null;
			JoyYouSDK_PlatformAdapter.TryGetIOSProjectCodeSigningIdentity(out releaseCSI, out debugCSI);

			AddIOSCodeSigningIdentity(ref project, releaseCSI, debugCSI);

			SetBitcode(ref project, false);
			SetDebugInfoFormat(ref project);
		}

		public static void AddIOSCodeSigningIdentity(ref XCProject project, string codeSigningIdentityRelease, string codeSigningIdentityDebug)
		{
			if (codeSigningIdentityRelease != null && codeSigningIdentityRelease != "")
			{
				project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", codeSigningIdentityRelease, "Release");
			}

			if (codeSigningIdentityDebug != null && codeSigningIdentityDebug != "")
			{
				project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", codeSigningIdentityDebug, "Debug");
			}
		}

		public static void AddPreprocessMacro(ref XCProject project, string[] macros)
		{
			if (macros != null && macros.Length > 0)
			{
				foreach (string item in macros)
				{
					project.overwriteBuildSetting("GCC_PREPROCESSOR_DEFINITIONS[arch=*]", item, "Release");
					project.overwriteBuildSetting("GCC_PREPROCESSOR_DEFINITIONS[arch=*]", item, "Debug");
				}
			}
		}

		public static void SetBitcode(ref XCProject project, bool enable)
		{
			string s_enable = enable ? "YES" : "NO";
			project.overwriteBuildSetting("ENABLE_BITCODE", s_enable, "Release");
			project.overwriteBuildSetting("ENABLE_BITCODE", s_enable, "Debug");
		}

		public static void SetDebugInfoFormat(ref XCProject project)
		{
			project.overwriteBuildSetting("DEBUG_INFORMATION_FORMAT", "DWARF", "Release");
			project.overwriteBuildSetting("DEBUG_INFORMATION_FORMAT", "DWARF", "Debug");
		}

		/*
		public static void SetCodeSignEntitlements(ref XCProject project, string path)
		{
			if (path != null && path != "")
			{
				project.overwriteBuildSetting("CODE_SIGN_ENTITLEMENTS", path, "Release");
				project.overwriteBuildSetting("CODE_SIGN_ENTITLEMENTS", path, "Debug");
			}
		}
		*/

		public static void SetXcodeCppException(ref XCProject project, bool enable)
		{
			string target = enable ? "YES" : "NO";
			project.overwriteBuildSetting("GCC_ENABLE_CPP_EXCEPTIONS", target, "Release");
			project.overwriteBuildSetting("GCC_ENABLE_CPP_EXCEPTIONS", target, "Debug");
		}

		public static void SetXcodeCppRTTI(ref XCProject project, bool enable)
		{
			string target = enable ? "YES" : "NO";
			project.overwriteBuildSetting("GCC_ENABLE_CPP_RTTI", target, "Release");
			project.overwriteBuildSetting("GCC_ENABLE_CPP_RTTI", target, "Debug");
		}

		public static void SetXcodeObjCException(ref XCProject project, bool enable)
		{
			string target = enable ? "YES" : "NO";
			project.overwriteBuildSetting("GCC_ENABLE_OBJC_EXCEPTIONS", target, "Release");
			project.overwriteBuildSetting("GCC_ENABLE_OBJC_EXCEPTIONS", target, "Debug");
		}

		// 允许HTTP协议
		private static bool _AddATSSection = false;
		public static void AddATSSection(ref XCPlist plist)
		{
			if (_AddATSSection)
			{
				return;
			}

			_AddATSSection = true;

			string ats = @"
	<key>NSAppTransportSecurity</key>
	<dict>
		<key>NSAllowsArbitraryLoads</key>
		<true/>
	</dict>
";
			plist.AddKey(ats);
		}

		private static bool _addEncryption = false;
		public static void AddEncryption(ref XCPlist plist)
		{
			if (_addEncryption)
			{
				return;
			}

			_addEncryption = true;

			string encryption = "<key>ITSAppUsesNonExemptEncryption</key><false/>";
			plist.AddKey(encryption);
		}

		public static void AddAppQueriesScheme(ref XCPlist plist, string item)
		{
			string ats = @"
	<key>LSApplicationQueriesSchemes</key>
	<array>
		<string>" + item + @"</string>
	</array>
";
			plist.AddKey(ats);
		}

		public static void AddAppQueriesSchemes(ref XCPlist plist, string[] items)
		{
			if (items.Length == 0)
			{
				return;
			}

			string prefix = @"
	<key>LSApplicationQueriesSchemes</key>
	<array>";
			string postfix = "</array>";

			StringBuilder tmp = new StringBuilder();
			foreach (string str in items)
			{
				tmp.Append("<string>");
				tmp.Append(str);
				tmp.Append("</string>");
			}

			plist.AddKey(prefix + tmp.ToString() + postfix);
		}

		public class IOSURLTypeItem
		{
			public string URLName { get; set; }
			public string URLSchemes { get; set; }

			public IOSURLTypeItem():this("", "") { }

			public IOSURLTypeItem(string name, string scheme)
			{
				this.URLName = name;
				this.URLSchemes = scheme;
			}

			public string getInnerDescription()
			{
				string des =
@"<dict>
	<key>CFBundleURLName</key>
	<string>" + URLName + @"</string>
	<key>CFBundleURLSchemes</key>
	<array>
		<string>" + URLSchemes + @"</string>
	</array>
</dict>";
				return des;
			}
		}

		public class URLTypesSection
		{
			public URLTypesSection() { }

			private List<IOSURLTypeItem> items = new List<IOSURLTypeItem>();

			public URLTypesSection AddItem(IOSURLTypeItem item)
			{
				this.items.Add(item);

				return this;
			}

			public URLTypesSection AddItem(string name, string value)
			{
				return this.AddItem(new IOSURLTypeItem(name, value));
			}

			public string GetPlistDescription()
			{
				StringBuilder sb = new StringBuilder();
				foreach (IOSURLTypeItem item in items)
				{
					sb.Append(item.getInnerDescription());
					sb.Append("\n");
				}

				string content = sb.ToString();
				if (!string.IsNullOrEmpty(content))
				{
					content =
@"<key>CFBundleURLTypes</key>
<array>" + content + "</array>\n";
				}

				return content;
			}
		}

		public static void AddURLTypes(ref XCPlist plist, URLTypesSection section)
		{
			if (section != null)
			{
				plist.AddKey(section.GetPlistDescription());
			}
		}

		public static void AddStringKVPair(ref XCPlist plist, string key, string value)
		{
			string item = @"<key>"+ key + @"</key>
	<string>" + value + "</string>";

			if (plist != null)
			{
				plist.AddKey(item);
			}
		}

		public static void AddBooleanKVPair(ref XCPlist plist, string key, bool value)
		{
			string __true = @"<key>" + key + @"</key><true/>";
			string __false = @"<key>" + key + @"</key><false/>";

			if (plist != null)
			{
				plist.AddKey(value ? __true : __false);
			}
		}

		public static void AddIntegerNumberKVPair(ref XCPlist plist, string key, int value)
		{
			string item = @"<key>" + key + @"</key>
	<integer>" + value + "</integer>";

			if (plist != null)
			{
				plist.AddKey(item);
			}
		}

		public static void AddRealNumberKVPair(ref XCPlist plist, string key, double value)
		{
			string item = @"<key>" + key + @"</key>
	<real>" + value + "</real>";

			if (plist != null)
			{
				plist.AddKey(item);
			}
		}
	}
}
