using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XUtliPoolLib;

using System.Collections.Generic;

namespace XEditor
{
    public class XCombinedPanel : XPanel
	{
        private GUILayoutOption[] _line = new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.Height(1) };

        protected override void OnInnerUpdate()
        {
            if (Hoster.SkillData.Combined == null) return;
            Hoster.SkillDataExtra.SkillClip_Frame = 0;
            float Start_At = 0;

            for (int i = 0; i < Hoster.SkillData.Combined.Count; i++)
            {
                float insert_point = (Hoster.SkillData.Combined[i].At / XSkillPanel.frame);
                float combined_point = (Hoster.SkillData.Combined[i].End / XSkillPanel.frame);

                Start_At += (i == 0 ? 0 : (Hoster.SkillDataExtra.SkillClip_Frame));
                Hoster.SkillDataExtra.SkillClip_Frame += (combined_point - insert_point);
            }
        }

        protected override void OnInnerGUI()
        {
            if (Hoster.SkillData.Combined == null) return;
            Hoster.SkillDataExtra.SkillClip_Frame = 0;
            float Clip_Frame = 0;
            float Start_At = 0;

            for (int i = 0; i < Hoster.SkillData.Combined.Count; i++)
            {
                Hoster.SkillData.Combined[i].Index = i;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Skill " + (i + 1), Hoster.SkillData.Combined[i].Name);

                if (GUILayout.Button("Browser", GUILayout.MaxWidth(70)))
                {
                    string file = EditorUtility.OpenFilePanel("Select Skp file", XEditorPath.Skp, "txt");
                    if (file.Length > 0)
                    {
                        int s = file.LastIndexOf('/');
                        int e = file.LastIndexOf('.');
                        Hoster.SkillData.Combined[i].Name = file.Substring(s + 1, e - s - 1);

                        XSkillData skill = XDataIO<XSkillData>.singleton.DeserializeData(file.Substring(file.IndexOf("Assets/")));

                        if(skill.TypeToken == 0)
                        {
                            EditorUtility.DisplayDialog("Confirm your configuration.",
                            "Can not select combo-skill!",
                            "Ok");

                            Hoster.SkillData.Combined[i].Name = null;
                        }
                        else
                        {
                            Hoster.SkillDataExtra.CombinedEx[i].Skill = skill;
                            Hoster.ConfigData.Combined[i].Skill_PathWithName = file.Substring(file.IndexOf("SkillPackage/"));

                            Hoster.SkillDataExtra.CombinedEx[i].Clip = Resources.Load(Hoster.SkillDataExtra.CombinedEx[i].Skill.ClipName, typeof(AnimationClip)) as AnimationClip;
                        }
                    }
                }
                if (Hoster.SkillData.Combined[i].Name == null)
                {
                    Hoster.SkillDataExtra.CombinedEx[i].Skill = null;
                    Hoster.ConfigData.Combined[i].Skill_PathWithName = null;
                }
                else if (GUILayout.Button("Open", GUILayout.MaxWidth(50)))
                {
                    XSkillPanel.next_file = (XDataBuilder.prefixPath + "/" + Hoster.ConfigData.Combined[i].Skill_PathWithName);
                }
                if (GUILayout.Button(_content_remove, GUILayout.MaxWidth(30)))
                {
                    Hoster.SkillData.Combined.RemoveAt(i);
                    Hoster.ConfigData.Combined.RemoveAt(i);
                    Hoster.SkillDataExtra.CombinedEx.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    continue;
                }
                EditorGUILayout.EndHorizontal();

                Clip_Frame = ((Hoster.SkillDataExtra.CombinedEx[i].Skill == null ? 0 : Hoster.SkillDataExtra.CombinedEx[i].Skill.Time) / (1.0f / 30.0f));

                EditorGUILayout.Space();
                float insert_point = (Hoster.SkillData.Combined[i].At / XSkillPanel.frame);
                EditorGUILayout.BeginHorizontal();
                insert_point = EditorGUILayout.FloatField("Cut From", insert_point);
                GUILayout.Label("(frame)");
                GUILayout.Label("", GUILayout.MaxWidth(30));
                EditorGUILayout.EndHorizontal();

                Hoster.ConfigData.Combined[i].From_Ratio = (Clip_Frame > 0) ? insert_point / Clip_Frame : 0;
                if (Hoster.ConfigData.Combined[i].From_Ratio > 1) Hoster.ConfigData.Combined[i].From_Ratio = 1;
                EditorGUILayout.BeginHorizontal();
                Hoster.ConfigData.Combined[i].From_Ratio = EditorGUILayout.Slider("Ratio", Hoster.ConfigData.Combined[i].From_Ratio, 0, 1);
                GUILayout.Label("(0~1)", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();
                Hoster.SkillData.Combined[i].At = (Hoster.ConfigData.Combined[i].From_Ratio * Clip_Frame) * XSkillPanel.frame;

                float combined_point = (Hoster.SkillData.Combined[i].End / XSkillPanel.frame);
                EditorGUILayout.BeginHorizontal();
                combined_point = EditorGUILayout.FloatField("Cut To", combined_point);
                GUILayout.Label("(frame)");
                GUILayout.Label("", GUILayout.MaxWidth(30));
                EditorGUILayout.EndHorizontal();

                Hoster.ConfigData.Combined[i].To_Ratio = (Clip_Frame > 0) ? combined_point / Clip_Frame : 0;
                if (Hoster.ConfigData.Combined[i].To_Ratio > 1) Hoster.ConfigData.Combined[i].To_Ratio = 1;
                EditorGUILayout.BeginHorizontal();
                Hoster.ConfigData.Combined[i].To_Ratio = EditorGUILayout.Slider("Ratio", Hoster.ConfigData.Combined[i].To_Ratio, 0, 1);
                GUILayout.Label("(0~1)", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                Hoster.SkillData.Combined[i].End = (Hoster.ConfigData.Combined[i].To_Ratio * Clip_Frame) * XSkillPanel.frame;

                if (Hoster.SkillData.Combined[i].End < Hoster.SkillData.Combined[i].At) Hoster.SkillData.Combined[i].End = Hoster.SkillData.Combined[i].At;

                Start_At = Hoster.SkillDataExtra.SkillClip_Frame;
                Hoster.SkillDataExtra.SkillClip_Frame += (combined_point - insert_point);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Self Start At", Start_At + " (frame)");
                GUILayout.Label("", GUILayout.MaxWidth(30));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                Hoster.SkillData.Combined[i].Override_Presentation = EditorGUILayout.Toggle("Effect Presents in Combined", Hoster.SkillData.Combined[i].Override_Presentation);
                if (i != Hoster.SkillData.Combined.Count - 1)
                {
                    GUILayout.Box("", _line);
                    EditorGUILayout.Space();
                }
            }
        }

        protected override bool FoldOut
        {
            get { return Hoster.EditorData.XCombined_foldout; }
            set { Hoster.EditorData.XCombined_foldout = value; }
        }

        protected override string PanelName
        {
            get { return "Combined"; }
        }

        protected override int Count
        {
            get { return Hoster.SkillData.Combined != null ? Hoster.SkillData.Combined.Count : -1; }
        }
    }
}
