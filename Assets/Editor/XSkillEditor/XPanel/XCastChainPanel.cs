using System;
using XUtliPoolLib;
using UnityEngine;
using UnityEditor;

namespace XEditor
{
    public class XCastChainPanel : XPanel
	{
        protected override void OnInnerGUI()
        {
            GUIStyle _myLabelStyle = null;

            if (_myLabelStyle == null)
            {
                _myLabelStyle = new GUIStyle(GUI.skin.label);
                _myLabelStyle.fontStyle = FontStyle.Italic;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Chain Skill", Hoster.SkillData.Chain.Name);
            if (!string.IsNullOrEmpty(Hoster.SkillData.Chain.Name))
            {
                if (GUILayout.Button("Delete", GUILayout.MaxWidth(70)))
                {
                    Hoster.SkillData.Chain.Name = null;
                }
            }
            if (GUILayout.Button("Browser", GUILayout.MaxWidth(70)))
            {
                string file = EditorUtility.OpenFilePanel("Select Skp file", XEditorPath.Skp, "txt");
                if (file.Length > 0)
                {
                    int s = file.LastIndexOf('/');
                    int e = file.LastIndexOf('.');
                    Hoster.SkillData.Chain.Name = file.Substring(s + 1, e - s - 1);

                    //XSkillData skill = XDataIO<XSkillData>.singleton.DeserializeData(file);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("the template id for the unit mobbed by host of this skill.", _myLabelStyle);
            Hoster.SkillData.Chain.TemplateID = EditorGUILayout.IntField("Unit ID", Hoster.SkillData.Chain.TemplateID);
            
            float cast_at = (Hoster.SkillData.Chain.At / XSkillPanel.frame);
            EditorGUILayout.BeginHorizontal();
            cast_at = EditorGUILayout.FloatField("Cast At ", cast_at);
            GUILayout.Label("(frame)");
            GUILayout.Label("", GUILayout.MaxWidth(30));
            EditorGUILayout.EndHorizontal();

            Hoster.SkillDataExtra.Chain.Ratio = cast_at / Hoster.SkillDataExtra.SkillClip_Frame;
            if (Hoster.SkillDataExtra.Chain.Ratio > 1) Hoster.SkillDataExtra.Chain.Ratio = 1;

            EditorGUILayout.BeginHorizontal();
            Hoster.SkillDataExtra.Chain.Ratio = EditorGUILayout.Slider("Ratio", Hoster.SkillDataExtra.Chain.Ratio, 0, 1);
            GUILayout.Label("(0~1)", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();

            Hoster.SkillData.Chain.At = (Hoster.SkillDataExtra.Chain.Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillPanel.frame;
        }

        protected override string PanelName
        {
            get { return "Cast Chain"; }
        }

        protected override bool FoldOut
        {
            get { return Hoster.EditorData.XCastChain_foldout; }
            set { Hoster.EditorData.XCastChain_foldout = value; }
        }

        protected override int Count
        {
            get { return -1; }
        }
    }
}
