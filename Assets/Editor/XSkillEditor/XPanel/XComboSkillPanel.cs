using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using XUtliPoolLib;

namespace XEditor
{
	public class XComboSkillPanel : XPanel
	{
        //private GUILayoutOption[] _line = new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.Height(1) };
        private List<int> _del = new List<int>();

        protected override void OnInnerGUI()
        {
            if (GUILayout.Button("Browser", GUILayout.MaxWidth(70)))
            {
                string file = EditorUtility.OpenFilePanel("Select Skp file", XEditorPath.Skp, "txt");
                if (file.Length > 0)
                {
                    //int s = file.LastIndexOf('/');
                    //int e = file.LastIndexOf('.');

                    Hoster.ComboSkills.Add(XDataIO<XSkillData>.singleton.DeserializeData(file));
                }
            }

            for (int i = 0; i < Hoster.ComboSkills.Count; i++)
            {
                ShowSucceedSkills(Hoster.ComboSkills[i], i);
            }
            for (int i = _del.Count - 1; i >= 0; i--)
                Hoster.ComboSkills.RemoveAt(i);

            _del.Clear();
        }

        protected override bool FoldOut
        {
            get { return Hoster.EditorData.XComboSkills_foldout; }
            set { Hoster.EditorData.XComboSkills_foldout = value; }
        }

        protected override string PanelName
        {
            get { return "ComboSkills List"; }
        }

        protected override int Count
        {
            get { return Hoster.ComboSkills == null ? -1 : Hoster.ComboSkills.Count; }
        }

        private void ShowSucceedSkills(XSkillData skill, int i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField((i + 1) + " Skill:", skill.Name);

            if (GUILayout.Button("Delete", GUILayout.MaxWidth(70)))
            {
                _del.Add(i);
            }
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(30));
            EditorGUILayout.EndHorizontal();
        }
    }
}
