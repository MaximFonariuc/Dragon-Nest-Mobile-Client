using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XUtliPoolLib;

using System.Collections.Generic;

namespace XEditor
{
	public class XAudioPanel : XPanel
	{
        private GUILayoutOption[] _line = new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.Height(1) };

        protected override int Count
        {
            get { return Hoster.SkillData.Audio == null ? -1 : Hoster.SkillData.Audio.Count; }
        }

        protected override void OnInnerGUI()
        {
            if (Hoster.SkillData.Audio == null) return;

            for (int i = 0; i < Hoster.SkillData.Audio.Count; i++)
            {
                Hoster.SkillData.Audio[i].Index = i;
                EditorGUILayout.BeginHorizontal();

                Hoster.SkillData.Audio[i].Clip = EditorGUILayout.TextField("Clip Name", Hoster.SkillData.Audio[i].Clip);

                if (GUILayout.Button(_content_remove, GUILayout.MaxWidth(30)))
                {
                    Hoster.SkillData.Audio.RemoveAt(i);
                    Hoster.SkillDataExtra.Audio.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();

                if (i < Hoster.SkillData.Audio.Count)
                {
                    float audio_at = (Hoster.SkillData.Audio[i].At / XSkillPanel.frame);
                    EditorGUILayout.BeginHorizontal();
                    audio_at = EditorGUILayout.FloatField("Play At ", audio_at);
                    GUILayout.Label("(frame)");
                    GUILayout.Label("", GUILayout.MaxWidth(30));
                    EditorGUILayout.EndHorizontal();

                    Hoster.SkillDataExtra.Audio[i].Ratio = audio_at / Hoster.SkillDataExtra.SkillClip_Frame;
                    if (Hoster.SkillDataExtra.Audio[i].Ratio > 1) Hoster.SkillDataExtra.Audio[i].Ratio = 1;

                    EditorGUILayout.BeginHorizontal();
                    Hoster.SkillDataExtra.Audio[i].Ratio = EditorGUILayout.Slider("Ratio", Hoster.SkillDataExtra.Audio[i].Ratio, 0, 1);
                    GUILayout.Label("(0~1)", EditorStyles.miniLabel);
                    EditorGUILayout.EndHorizontal();

                    Hoster.SkillData.Audio[i].At = (Hoster.SkillDataExtra.Audio[i].Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillPanel.frame;
                    Hoster.SkillData.Audio[i].Channel = Hoster.SkillData.TypeToken == 3 ? AudioChannel.SkillCombine : AudioChannel.Skill;
                    EditorGUILayout.EnumPopup("Channel", Hoster.SkillData.Audio[i].Channel);
                }

                if (i != Hoster.SkillData.Audio.Count - 1)
                {
                    GUILayout.Box("", _line);
                    EditorGUILayout.Space();
                }
            }
        }

        protected override bool FoldOut
        {
            get { return Hoster.EditorData.XAudio_foldout; }
            set { Hoster.EditorData.XAudio_foldout = value; }
        }

        protected override string PanelName
        {
            get { return "Audio"; }
        }
    }
}
