using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XUtliPoolLib;
using System.Collections.Generic;

namespace XEditor
{
	public class XCameraEffectPanel : XPanel
	{
        private GUILayoutOption[] _line = new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.Height(1) };

        protected override string PanelName
        {
            get { return "Camera Shake"; }
        }

        protected override int Count
        {
            get { return Hoster.SkillData.CameraEffect != null ? Hoster.SkillData.CameraEffect.Count : -1; }
        }

        protected override bool FoldOut
        {
            get { return Hoster.EditorData.XCameraEffect_foldout; }
            set { Hoster.EditorData.XCameraEffect_foldout = value; }
        }

        protected override void OnInnerGUI()
        {
            if (Hoster.SkillData.CameraEffect == null) return;

            for (int i = 0; i < Hoster.SkillData.CameraEffect.Count; i++)
            {
                Hoster.SkillData.CameraEffect[i].Combined = (Hoster.SkillData.TypeToken == 3);

                Hoster.SkillData.CameraEffect[i].Index = i;
                EditorGUILayout.BeginHorizontal();
                Hoster.SkillData.CameraEffect[i].Time = EditorGUILayout.FloatField("Time", Hoster.SkillData.CameraEffect[i].Time);
                if (GUILayout.Button(_content_remove, GUILayout.MaxWidth(30)))
                {
                    Hoster.SkillData.CameraEffect.RemoveAt(i);
                    Hoster.ConfigData.CameraEffect.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
                if (i < Hoster.SkillData.CameraEffect.Count)
                {
                    Hoster.SkillData.CameraEffect[i].Frequency = EditorGUILayout.FloatField("Frequency", Hoster.SkillData.CameraEffect[i].Frequency);
                    EditorGUILayout.Space();
                    Hoster.SkillData.CameraEffect[i].AmplitudeX = EditorGUILayout.FloatField("Amplitude X", Hoster.SkillData.CameraEffect[i].AmplitudeX);
                    Hoster.SkillData.CameraEffect[i].AmplitudeY = EditorGUILayout.FloatField("Amplitude Y", Hoster.SkillData.CameraEffect[i].AmplitudeY);
                    Hoster.SkillData.CameraEffect[i].AmplitudeZ = EditorGUILayout.FloatField("Amplitude Z", Hoster.SkillData.CameraEffect[i].AmplitudeZ);
                    Hoster.SkillData.CameraEffect[i].FovAmp = EditorGUILayout.FloatField("Fov Amplitude", Hoster.SkillData.CameraEffect[i].FovAmp);
                    EditorGUILayout.Space();
                    Hoster.SkillData.CameraEffect[i].Coordinate = (CameraMotionSpace)EditorGUILayout.EnumPopup("Coordinate", Hoster.SkillData.CameraEffect[i].Coordinate);
                    EditorGUILayout.Space();
                    Hoster.SkillData.CameraEffect[i].ShakeX = EditorGUILayout.Toggle("Shake X", Hoster.SkillData.CameraEffect[i].ShakeX);
                    Hoster.SkillData.CameraEffect[i].ShakeY = EditorGUILayout.Toggle("Shake Y", Hoster.SkillData.CameraEffect[i].ShakeY);
                    Hoster.SkillData.CameraEffect[i].ShakeZ = EditorGUILayout.Toggle("Shake Z", Hoster.SkillData.CameraEffect[i].ShakeZ);
                    EditorGUILayout.Space();
                    float shake_at = (Hoster.SkillData.CameraEffect[i].At / XSkillPanel.frame);
                    EditorGUILayout.BeginHorizontal();
                    shake_at = EditorGUILayout.FloatField("Shake At ", shake_at);
                    GUILayout.Label("(frame)");
                    GUILayout.Label("", GUILayout.MaxWidth(30));
                    EditorGUILayout.EndHorizontal();

                    Hoster.ConfigData.CameraEffect[i].Ratio = shake_at / Hoster.SkillDataExtra.SkillClip_Frame;
                    if (Hoster.ConfigData.CameraEffect[i].Ratio > 1) Hoster.ConfigData.CameraEffect[i].Ratio = 1;

                    EditorGUILayout.BeginHorizontal();
                    Hoster.ConfigData.CameraEffect[i].Ratio = EditorGUILayout.Slider("Ratio", Hoster.ConfigData.CameraEffect[i].Ratio, 0, 1);
                    GUILayout.Label("(0~1)", EditorStyles.miniLabel);
                    EditorGUILayout.EndHorizontal();

                    Hoster.SkillData.CameraEffect[i].At = (Hoster.ConfigData.CameraEffect[i].Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillPanel.frame;
                    EditorGUILayout.Space();
                    Hoster.SkillData.CameraEffect[i].Random = EditorGUILayout.Toggle("Random Effect", Hoster.SkillData.CameraEffect[i].Random);
                    if (i != Hoster.SkillData.CameraEffect.Count - 1)
                    {
                        GUILayout.Box("", _line);
                        EditorGUILayout.Space();
                    }
                }
            }
        }
    }
}