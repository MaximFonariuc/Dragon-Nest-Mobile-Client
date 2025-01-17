using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityEditor.JoyYouSDKEditor
{
	class SettingWindow : EditorWindow
	{
		protected BuildTargetGroup curTargetGroup;
		protected BuildTarget curBuildTarget;

		public virtual void Awake()
		{
			// autoRepaintOnSceneChange = true;
			this.titleContent = new GUIContent("BuildSettings");
		}

		public void BuildProject(string directory)
		{
			try
			{
				GenericBuild(FindEnabledEditorScenes(), directory, curBuildTarget, BuildOptions.None);

				PostBuildEvent();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public virtual void PostBuildEvent() {}

		public virtual void OnGUI() {}

		public static string[] FindEnabledEditorScenes()
		{
			List<string> EditorScenes = new List<string>();

			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
			{
				if (!scene.enabled) continue;

				EditorScenes.Add(scene.path);
			}

			return EditorScenes.ToArray();
		}

		public static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
		{
			EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);

			string res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options).ToString();

			if (res.Length > 0)
			{
				throw new Exception("BuildPlayer failure: " + res);
			}
		}
	}
}
