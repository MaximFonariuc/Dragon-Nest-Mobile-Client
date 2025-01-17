using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor.XCodeEditor;
using System.IO;

namespace UnityEditor.JoyYouSDKEditor
{
	class ProjectSettings
	{
		public virtual void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject) { }
		public void PostProcessBuild(BuildTarget target, string pathToBuiltProject)
		{
			OnPostProcessBuild(target, pathToBuiltProject);
		}
	}
}
