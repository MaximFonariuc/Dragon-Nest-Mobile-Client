using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XUtliPoolLib;

using System.Collections.Generic;
using System.IO;

namespace XEditor
{
	public class XScriptPanel : XPanel
	{
        protected override void OnInnerGUI()
        {
            if (Hoster.SkillData.Script == null) return;

            UpdateScript(ref Hoster.SkillData.Script.Start_Name, "Start");
            UpdateScript(ref Hoster.SkillData.Script.Update_Name, "Update");
            UpdateScript(ref Hoster.SkillData.Script.Result_Name, "Result");
            UpdateScript(ref Hoster.SkillData.Script.Stop_Name, "Stop");
        }

        protected override bool FoldOut
        {
            get { return Hoster.EditorData.XScript_foldout; }
            set { Hoster.EditorData.XScript_foldout = value; }
        }

        protected override string PanelName
        {
            get { return "Script"; }
        }

        private void UpdateScript(ref string name, string type)
        {
            EditorGUILayout.BeginHorizontal();
            name = EditorGUILayout.TextField(type, name);

            if (!ScriptFileExist(name))
            {
                if (GUILayout.Button("new", GUILayout.MaxWidth(50)))
                {
                    ScriptFileGen(name);
                }
            }
            else 
            {
                if (GUILayout.Button("delete", GUILayout.MaxWidth(50)))
                {
                    if (EditorUtility.DisplayDialog("Are you sure?",
                        "Do you want to DELETE it?",
                        "Yes",
                        "No"))
                    {
                        if (ScriptFileDel(name))
                            name = null;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private bool ScriptFileExist(string name)
        {
            return File.Exists(XSkillScriptGen.singleton.ScriptPath + Hoster.SkillDataExtra.ScriptFile + "_" + name + ".cs");
        }

        private bool ScriptFileGen(string name)
        {
            if (name == null || name.Length == 0)
                return false;

            return XSkillScriptGen.singleton.ScriptGen(Hoster.SkillDataExtra.ScriptFile, name);
        }

        private bool ScriptFileDel(string name)
        {
            if (name == null || name.Length == 0)
                return false;

            return XSkillScriptGen.singleton.ScriptDel(Hoster.SkillDataExtra.ScriptFile, name);
        }

        protected override int Count
        {
            get { return -1; }
        }
    }
}