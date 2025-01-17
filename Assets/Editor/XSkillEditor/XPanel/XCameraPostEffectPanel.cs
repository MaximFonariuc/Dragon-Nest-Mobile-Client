using System;
using System.Collections.Generic;
using System.Text;
using XEditor;
using UnityEditor;
using UnityEngine;
using XUtliPoolLib;

namespace XEditor
{
    public class XCameraPostEffectPanel : XPanel
	{
        protected override void OnInnerGUI()
        {
            Hoster.SkillDataExtra.PostEffectEx.Effect = EditorGUILayout.ObjectField("Post Effect", Hoster.SkillDataExtra.PostEffectEx.Effect, typeof(UnityEngine.Object), true) as UnityEngine.Object;
            if (Hoster.SkillDataExtra.PostEffectEx.Effect != null)
            {
                Hoster.SkillData.CameraPostEffect.Effect = Hoster.SkillDataExtra.PostEffectEx.Effect.name;

                Hoster.ConfigData.PostEffect.EffectLocation = AssetDatabase.GetAssetPath(Hoster.SkillDataExtra.PostEffectEx.Effect);
                EditorGUILayout.LabelField("Post Effect Location", Hoster.ConfigData.PostEffect.EffectLocation);

                //Hoster.SkillDataExtra.PostEffectEx.Shader = EditorGUILayout.ObjectField("Shader", Hoster.SkillDataExtra.PostEffectEx.Shader, typeof(UnityEngine.Shader), true) as UnityEngine.Shader;
                //if (Hoster.SkillDataExtra.PostEffectEx.Shader != null)
                //{
                //    string path = AssetDatabase.GetAssetPath(Hoster.SkillDataExtra.PostEffectEx.Shader).Remove(0, 17);
                //    Hoster.SkillData.CameraPostEffect.Shader = path.Remove(path.LastIndexOf('.'));
                //    EditorGUILayout.LabelField("Shader Location", Hoster.SkillData.CameraPostEffect.Shader);
                //}
                //else
                //    Hoster.SkillData.CameraPostEffect.Shader = null;
            }
            else
            {
                Hoster.SkillData.CameraPostEffect.Effect = null;
                Hoster.ConfigData.PostEffect.EffectLocation = null;
            }

            EditorGUILayout.Space();

            float effect_at = (Hoster.SkillData.CameraPostEffect.At / XSkillPanel.frame);
            EditorGUILayout.BeginHorizontal();
            effect_at = EditorGUILayout.FloatField("Play At ", effect_at);
            GUILayout.Label("(frame)");
            GUILayout.Label("", GUILayout.MaxWidth(30));
            EditorGUILayout.EndHorizontal();

            Hoster.SkillDataExtra.PostEffectEx.At_Ratio = effect_at / Hoster.SkillDataExtra.SkillClip_Frame;
            if (Hoster.SkillDataExtra.PostEffectEx.At_Ratio > 1) Hoster.SkillDataExtra.PostEffectEx.At_Ratio = 1;

            EditorGUILayout.BeginHorizontal();
            Hoster.SkillDataExtra.PostEffectEx.At_Ratio = EditorGUILayout.Slider("Ratio", Hoster.SkillDataExtra.PostEffectEx.At_Ratio, 0, 1);
            GUILayout.Label("(0~1)", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
            Hoster.SkillData.CameraPostEffect.At = (Hoster.SkillDataExtra.PostEffectEx.At_Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillPanel.frame;

            float effect_end = (Hoster.SkillData.CameraPostEffect.End / XSkillPanel.frame);
            EditorGUILayout.BeginHorizontal();
            effect_end = EditorGUILayout.FloatField("End At ", effect_end);
            GUILayout.Label("(frame)");
            GUILayout.Label("", GUILayout.MaxWidth(30));
            EditorGUILayout.EndHorizontal();

            Hoster.SkillDataExtra.PostEffectEx.End_Ratio = effect_end / Hoster.SkillDataExtra.SkillClip_Frame;
            if (Hoster.SkillDataExtra.PostEffectEx.End_Ratio > 1) Hoster.SkillDataExtra.PostEffectEx.End_Ratio = 1;
            if (Hoster.SkillDataExtra.PostEffectEx.End_Ratio < Hoster.SkillDataExtra.PostEffectEx.At_Ratio) Hoster.SkillDataExtra.PostEffectEx.End_Ratio = Hoster.SkillDataExtra.PostEffectEx.At_Ratio;

            EditorGUILayout.BeginHorizontal();
            Hoster.SkillDataExtra.PostEffectEx.End_Ratio = EditorGUILayout.Slider("Ratio", Hoster.SkillDataExtra.PostEffectEx.End_Ratio, 0, 1);
            GUILayout.Label("(0~1)", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
            Hoster.SkillData.CameraPostEffect.End = (Hoster.SkillDataExtra.PostEffectEx.End_Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillPanel.frame;

            EditorGUILayout.Space();

            Hoster.SkillData.CameraPostEffect.SolidBlack = EditorGUILayout.Toggle("Solid Black", Hoster.SkillData.CameraPostEffect.SolidBlack);
            if (Hoster.SkillData.CameraPostEffect.SolidBlack)
            {
                float solid_at = (Hoster.SkillData.CameraPostEffect.Solid_At / XSkillPanel.frame);
                EditorGUILayout.BeginHorizontal();
                solid_at = EditorGUILayout.FloatField("Play At ", solid_at);
                GUILayout.Label("(frame)");
                GUILayout.Label("", GUILayout.MaxWidth(30));
                EditorGUILayout.EndHorizontal();

                Hoster.SkillDataExtra.PostEffectEx.Solid_At_Ratio = solid_at / Hoster.SkillDataExtra.SkillClip_Frame;
                if (Hoster.SkillDataExtra.PostEffectEx.Solid_At_Ratio > 1) Hoster.SkillDataExtra.PostEffectEx.Solid_At_Ratio = 1;

                EditorGUILayout.BeginHorizontal();
                Hoster.SkillDataExtra.PostEffectEx.Solid_At_Ratio = EditorGUILayout.Slider("Ratio", Hoster.SkillDataExtra.PostEffectEx.Solid_At_Ratio, 0, 1);
                GUILayout.Label("(0~1)", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();
                Hoster.SkillData.CameraPostEffect.Solid_At = (Hoster.SkillDataExtra.PostEffectEx.Solid_At_Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillPanel.frame;

                float solid_end = (Hoster.SkillData.CameraPostEffect.Solid_End / XSkillPanel.frame);
                EditorGUILayout.BeginHorizontal();
                solid_end = EditorGUILayout.FloatField("End At ", solid_end);
                GUILayout.Label("(frame)");
                GUILayout.Label("", GUILayout.MaxWidth(30));
                EditorGUILayout.EndHorizontal();

                Hoster.SkillDataExtra.PostEffectEx.Solid_End_Ratio = solid_end / Hoster.SkillDataExtra.SkillClip_Frame;
                if (Hoster.SkillDataExtra.PostEffectEx.Solid_End_Ratio > 1) Hoster.SkillDataExtra.PostEffectEx.Solid_End_Ratio = 1;
                if (Hoster.SkillDataExtra.PostEffectEx.Solid_End_Ratio < Hoster.SkillDataExtra.PostEffectEx.Solid_At_Ratio) Hoster.SkillDataExtra.PostEffectEx.Solid_End_Ratio = Hoster.SkillDataExtra.PostEffectEx.Solid_At_Ratio;

                EditorGUILayout.BeginHorizontal();
                Hoster.SkillDataExtra.PostEffectEx.Solid_End_Ratio = EditorGUILayout.Slider("Ratio", Hoster.SkillDataExtra.PostEffectEx.Solid_End_Ratio, 0, 1);
                GUILayout.Label("(0~1)", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();
                Hoster.SkillData.CameraPostEffect.Solid_End = (Hoster.SkillDataExtra.PostEffectEx.Solid_End_Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillPanel.frame;
            }
        }

        protected override bool FoldOut
        {
            get { return Hoster.EditorData.XCameraPostEffect_foldout; }
            set { Hoster.EditorData.XCameraPostEffect_foldout = value; }
        }

        protected override string PanelName
        {
            get { return "Camera Post Effect"; }
        }

        protected override int Count
        {
            get { return -1; }
        }
    }
}
