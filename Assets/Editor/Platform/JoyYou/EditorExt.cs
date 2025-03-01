using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEditor.Callbacks;
using Assets.SDK;
using UnityEditor.XCodeEditor;
using UnityEditor.JoyYouSDKEditor;
using System.Xml;

namespace UnityEditor.JoyYouSDKEditor
{
	public static class JoyYouSDK_PlatformAdapter
	{
		static void _P(object o) { Debug.Log(o); }

		[MenuItem("JoyYouSDK/IOS/DirectBuild")]
		static void DirectBuild()
		{
			AssetDatabase.Refresh();

			// Select where to save files
			// string path = UnityEditor.EditorUtility.OpenFolderPanel("ProjectExport", "", "ExportProject");

			string previousPath = EditorPrefs.GetString("BuildForiOS.PreviousPath", Application.persistentDataPath);

			string destination = EditorUtility.SaveFilePanel("Choose a destination", previousPath, EditorPrefs.GetString("BuildForiOS.Name", ""), "");
			if(string.IsNullOrEmpty(destination))
			{
				throw new Exception("无效的导出路径");
			}

			// Saving settings for the next build
			string name = destination.Substring(destination.LastIndexOf("/") + 1);
			EditorPrefs.SetString("BuildForiOS.PreviousPath", destination.Substring(0, destination.LastIndexOf("/")));
			EditorPrefs.SetString("BuildForiOS.Name", name);

			// Build the player
			// BuildPipeline.BuildPlayer(scenesPath, destination, BuildTarget.iOS, BuildOptions.Development | BuildOptions.ShowBuiltPlayer | BuildOptions.Il2CPP);

			SettingWindow.GenericBuild(SettingWindow.FindEnabledEditorScenes(), destination, BuildTarget.iOS, BuildOptions.None);
		}

		[MenuItem("JoyYouSDK/IOS/DirectBuild", true)]
		static bool DirectBuild_Validation()
		{
			Debug.Log("Current plat = " + EditorUserBuildSettings.activeBuildTarget);

			return (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS);
		}

		[PostProcessBuild]
		static void OnBuildingEnd(BuildTarget target, string path)
		{
			// Debug.LogError("PostProcessBuild" + '/' + target.ToString() + '/' + path);

			ProjectSettings4IOS ps = null;

			string platformName = Get3rdPlatformName();
			Debug.Log("current platform name is: " + platformName);

			if (platformName == JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_NONE)
			{
				ps = new ProjectSettings4IOS();
			}

			else if (platformName == JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_TENCENT)
			{
#if UNITY_IOS
				ps = new ProjectSettings4IOS_TencentMSDK();
#endif
			}

			if (ps == null)
			{
				//Debug.LogError("No platform matched, please check it!");

				return;
			}

			ps.PostProcessBuild(target, path);

			// string autoPackageScriptPath = Path.Combine(path, "build.sh");
			// CreatePackageScript(autoPackageScriptPath);
			// RunPackageScript(autoPackageScriptPath);

			Debug.Log("Build Task over !");
		}

		public static void TryGetIOSProjectCodeSigningIdentity(out string releaseCSI, out string debugCSI)
		{
			//string platformName = Get3rdPlatformName();

			// 设置打包IPA所需的签名信息
			releaseCSI = debugCSI = "";
		}

		private static string Get3rdPlatformName()
		{
			Type t = typeof(JoyYouSDK);

			string platformName = "";

			foreach (var attr in t.GetCustomAttributes(false))
			{
				JoyYouSDKPlatformFilterAttribute jySDKAttr = attr as JoyYouSDKPlatformFilterAttribute;
				if (jySDKAttr != null)
				{
					platformName = jySDKAttr.PlatformName;

					break;
				}
			}

			return platformName;
		}

		static string configFileDir = "Editor";
		static string configFileName = "PlayerSettings.xml";
		static string resourceDir = "Assets/PlayerSetting_Icons/{0}.png";

		private static string NODE_NAME_ROOT = "PlatformSettings";

		public static void SavePlayerSettings(BuildTargetGroup group, string platformName)
		{
			string settingDir = Path.Combine(Application.dataPath, configFileDir);
			if (!Directory.Exists(settingDir))
			{
				Directory.CreateDirectory(settingDir);
			}

			XmlDocument doc = new XmlDocument();
			string path_xml = Path.Combine(settingDir, configFileName);

			bool isExistDoc = File.Exists(path_xml);
			string rootNodeName = NODE_NAME_ROOT;

			if (isExistDoc)
			{
				try {
					doc.Load(path_xml);

					if (doc.DocumentElement.Name != rootNodeName)
					{
						doc.RemoveAll();

						isExistDoc = false;
					}
				}
				catch (System.Exception ex)
				{
					Debug.LogException(ex);

					isExistDoc = false;
				}
			}

			if (!isExistDoc)
			{
				XmlNode nDecl = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
				doc.AppendChild(nDecl);
				XmlElement _root = doc.CreateElement(rootNodeName);
				doc.AppendChild(_root);
			}

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

			while(node != null)
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

			var list = doc.SelectNodes("/" + NODE_NAME_ROOT + "/" + group.ToString() + "/" + platformName + "/" + "Image");

			int[] sizeArray = PlayerSettings.GetIconSizesForTargetGroup(group);
			Texture2D[] texArray = PlayerSettings.GetIconsForTargetGroup(group);

			for (int i = 0; i < sizeArray.Length; ++i)
			{
				bool find_exist = false;

				foreach (XmlElement j in list)
				{
					if (j.Attributes["Size"] != null && j.Attributes["Size"].Value == sizeArray[i].ToString())
					{
						j.SetAttribute("TextureName", (texArray[i] != null) ? texArray[i].name : "null");

						find_exist = true;

						break;
					}
				}

				if (!find_exist)
				{
					XmlElement ele = doc.CreateElement("Image");
					ele.SetAttribute("Size", sizeArray[i].ToString());
					ele.SetAttribute("TextureName", (texArray[i] != null) ? texArray[i].name : "null");
					platformNode.AppendChild(ele);
				}
			}

			doc.Save(path_xml);
		}

		public static void RestorePlayerSettings(BuildTargetGroup group, string platformName)
		{
			Texture2D[] all_tex = null;
			GetPlayerSettings(group, platformName, ref all_tex);

			if (all_tex != null)
			{
				PlayerSettings.SetIconsForTargetGroup(group, all_tex);
			}
		}

		private static string[] getTexturesNames(Texture2D[] all_tex)
		{
			List<string> l_names = new List<string>();

			if (all_tex != null)
			{
				for (int i = 0; i < all_tex.Length; ++i)
				{
					if (all_tex[i] != null)
					{
						l_names.Add(all_tex[i].name);
					}
				}
			}

			return l_names.ToArray();
		}

		public static bool IsPlayerSettingsChanaged(BuildTargetGroup group,  Texture2D[] all_tex)
		{
			string[] all_tex_name = getTexturesNames(all_tex);
			string[] current_tex_name = getTexturesNames(PlayerSettings.GetIconsForTargetGroup(group));

			// Debug.Log(all_tex_name.Length + "/" + current_tex_name.Length);

			if (all_tex_name == null || current_tex_name == null)
			{
				if(all_tex_name == null && current_tex_name == null)
				{
					return false;
				}

				return true;
			}

			if (all_tex_name.Length != current_tex_name.Length)
			{
				return true;
			}
			else
			{
				bool exist_in_cfg = false;

				foreach(string s in current_tex_name)
				{
					exist_in_cfg = false;

					foreach(string k in all_tex_name)
					{
						if(k == s)
						{
							exist_in_cfg = true;

							break;
						}
					}

					if (!exist_in_cfg)
					{
						return true;
					}
				}
			}

			return false;
		}

		public static void GetPlayerSettings(BuildTargetGroup group, string platformName, ref Texture2D[] all_tex)
		{
			all_tex = null;

			string settingDir = Path.Combine(Application.dataPath, configFileDir);
			if (!Directory.Exists(settingDir))
			{
				Directory.CreateDirectory(settingDir);
			}

			XmlDocument doc = new XmlDocument();

			string path_xml = Path.Combine(settingDir, configFileName);
			bool isExistDoc = File.Exists(path_xml);
			string rootNodeName = NODE_NAME_ROOT;

			if (isExistDoc)
			{
				try {
					doc.Load(path_xml);
					if (doc.DocumentElement.Name != rootNodeName)
					{
						isExistDoc = false;
					}
				}
				catch (System.Exception ex)
				{
					Debug.LogException(ex);

					isExistDoc = false;
				}
			}

			if (!isExistDoc)
			{
				Debug.LogWarning("Invalid restore settings, ignored.");

				return;
			}

			// XmlElement root = doc.DocumentElement;

			string path = "/" + NODE_NAME_ROOT + "/" + group.ToString() + "/" + platformName + "/" + "Image";
			XmlNodeList list = doc.SelectNodes(path);
			if (list.Count == 0)
			{
				Debug.LogWarning("No configurations will be restored, ignored.");

				return;
			}

			List<Texture2D> textures = new List<Texture2D>();

			foreach (XmlNode node in list)
			{
				Texture2D tex = null;

				string texName = node.Attributes["TextureName"].Value;
				if (texName != "null")
				{
					tex = AssetDatabase.LoadMainAssetAtPath(string.Format(resourceDir, texName)) as Texture2D;
				}

				textures.Add(tex);
			}

			all_tex = textures.ToArray();
		}
	}
}
