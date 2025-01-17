using System;
using System.Collections.Generic;
using System.Text;
using XEditor;
using UnityEditor;
using UnityEngine;
using XUtliPoolLib;

namespace XEditor
{
	public class XCameraMotionPanel : XPanel
	{
        GUIStyle _myLabelStyle2 = null;

        public static readonly string[] CameraMotionSpaceStr = 
        {
            "World",
            "Self"
        };

        protected override void OnInnerGUI()
        {
            if (_myLabelStyle2 == null)
            {
                _myLabelStyle2 = new GUIStyle(GUI.skin.label);
                _myLabelStyle2.fontStyle = FontStyle.Italic;
            }

            if (Hoster.SkillData.CameraMotion == null) return;

            Hoster.SkillDataExtra.MotionEx.Motion3D = EditorGUILayout.ObjectField("Motion Clip for 3D", Hoster.SkillDataExtra.MotionEx.Motion3D, typeof(AnimationClip), true) as AnimationClip;
            if (null != Hoster.SkillDataExtra.MotionEx.Motion3D)
            {
                string path = AssetDatabase.GetAssetPath(Hoster.SkillDataExtra.MotionEx.Motion3D).Remove(0, 17);
                Hoster.SkillData.CameraMotion.Motion3D = path.Remove(path.LastIndexOf('.'));
                EditorGUILayout.LabelField("Camera Motion Name", Hoster.SkillData.CameraMotion.Motion3D);
                Hoster.SkillData.CameraMotion.Motion3DType = (CameraMotionType)EditorGUILayout.EnumPopup("Type", Hoster.SkillData.CameraMotion.Motion3DType);
            }
            else
                Hoster.SkillData.CameraMotion.Motion3D = null;
            EditorGUILayout.Space();
            
            Hoster.SkillDataExtra.MotionEx.Motion2_5D = EditorGUILayout.ObjectField("Motion Clip for 2.5D", Hoster.SkillDataExtra.MotionEx.Motion2_5D, typeof(AnimationClip), true) as AnimationClip;
            if (null != Hoster.SkillDataExtra.MotionEx.Motion2_5D)
            {
                string path = AssetDatabase.GetAssetPath(Hoster.SkillDataExtra.MotionEx.Motion2_5D).Remove(0, 17);
                Hoster.SkillData.CameraMotion.Motion2_5D = path.Remove(path.LastIndexOf('.'));
                EditorGUILayout.LabelField("Camera Motion Name", Hoster.SkillData.CameraMotion.Motion2_5D);
                Hoster.SkillData.CameraMotion.Motion2_5DType = (CameraMotionType)EditorGUILayout.EnumPopup("Type", Hoster.SkillData.CameraMotion.Motion2_5DType);
            }
            else
            {
                EditorGUILayout.LabelField("By default, 2.5D motion using 3D motion if be null", _myLabelStyle2);
                Hoster.SkillData.CameraMotion.Motion2_5D = null;
            }

            EditorGUILayout.Space();
            Hoster.SkillData.CameraMotion.LookAt_Target = EditorGUILayout.Toggle("Look at Target", Hoster.SkillData.CameraMotion.LookAt_Target);
            EditorGUILayout.LabelField("Anchor based mode ignores this setting", _myLabelStyle2);
            EditorGUILayout.Space();

            float motion_at = (Hoster.SkillData.CameraMotion.At / XSkillPanel.frame);
            EditorGUILayout.BeginHorizontal();
            motion_at = EditorGUILayout.FloatField("Play At ", motion_at);
            GUILayout.Label("(frame)");
            GUILayout.Label("", GUILayout.MaxWidth(30));
            EditorGUILayout.EndHorizontal();

            Hoster.SkillDataExtra.MotionEx.Ratio = motion_at / Hoster.SkillDataExtra.SkillClip_Frame;
            if (Hoster.SkillDataExtra.MotionEx.Ratio > 1) Hoster.SkillDataExtra.MotionEx.Ratio = 1;

            EditorGUILayout.BeginHorizontal();
            Hoster.SkillDataExtra.MotionEx.Ratio = EditorGUILayout.Slider("Ratio", Hoster.SkillDataExtra.MotionEx.Ratio, 0, 1);
            GUILayout.Label("(0~1)", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
            Hoster.SkillData.CameraMotion.At = (Hoster.SkillDataExtra.MotionEx.Ratio * Hoster.SkillDataExtra.SkillClip_Frame) * XSkillPanel.frame;
        }

        protected override bool FoldOut
        {
            get { return Hoster.EditorData.XCameraMotion_foldout; }
            set { Hoster.EditorData.XCameraMotion_foldout = value; }
        }

        protected override string PanelName
        {
            get { return "Camera Motion"; }
        }

        protected override int Count
        {
            get { return -1; }
        }
    }
}
