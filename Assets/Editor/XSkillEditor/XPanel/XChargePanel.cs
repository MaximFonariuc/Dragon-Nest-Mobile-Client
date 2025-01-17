using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using System.Collections.Generic;
using XUtliPoolLib;

namespace XEditor
{
	public class XChargePanel : XPanel
	{
        protected override string PanelName
        {
            get { return "Charge"; }
        }

        protected override int Count
        {
            get { return Hoster.SkillData.Charge == null ? -1 : Hoster.SkillData.Charge.Count; }
        }

        protected override bool FoldOut
        {
            get { return Hoster.EditorData.XCharge_foldout; }
            set { Hoster.EditorData.XCharge_foldout = value; }
        }

        private GUILayoutOption[] _line = new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.Height(1) };

        protected override void OnInnerGUI()
        {
            if (Hoster.SkillData.Charge == null) return;

            for (int i = 0; i < Hoster.SkillData.Charge.Count; i++)
            {
                Hoster.SkillData.Charge[i].Index = i;
                EditorGUILayout.BeginHorizontal();
                Hoster.SkillData.Charge[i].Using_Curve = EditorGUILayout.Toggle("Using Curve", Hoster.SkillData.Charge[i].Using_Curve);

                if (GUILayout.Button(_content_remove, GUILayout.MaxWidth(30)))
                {
                    Hoster.SkillData.Charge.RemoveAt(i);
                    Hoster.ConfigData.Charge.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                if (i < Hoster.SkillData.Charge.Count)
                {
                    if (Hoster.SkillData.Charge[i].Using_Curve)
                    {
                        GameObject o = EditorGUILayout.ObjectField("Forward Prefab", Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Prefab_Forward, typeof(GameObject), true) as GameObject;
                        {
                            if (XInnerEditor.CheckPrefab(o))
                            {
                                Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Prefab_Forward = o;
                                string path = AssetDatabase.GetAssetPath(o).Remove(0, 17);
                                Hoster.SkillData.Charge[i].Curve_Forward = path.Remove(path.LastIndexOf('.'));
                                Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Forward = (o.GetComponent<XCurve>().Curve);
                            }
                            else
                            {
                                Hoster.SkillData.Charge[i].Curve_Forward = null;
                                Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Forward = null;
                            }
                        }
                        if (Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Forward != null)
                            EditorGUILayout.CurveField("Curve", Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Forward);

                        o = EditorGUILayout.ObjectField("Side Prefab", Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Prefab_Side, typeof(GameObject), true) as GameObject;
                        {
                            if (XInnerEditor.CheckPrefab(o))
                            {
                                Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Prefab_Side = o;
                                string path = AssetDatabase.GetAssetPath(o).Remove(0, 17);
                                Hoster.SkillData.Charge[i].Curve_Side = path.Remove(path.LastIndexOf('.'));
                                Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Side = (o.GetComponent<XCurve>().Curve);
                            }
                            else
                            {
                                Hoster.SkillData.Charge[i].Curve_Side = null;
                                Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Side = null;
                            }
                        }
                        if (Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Side != null)
                            EditorGUILayout.CurveField("Curve", Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Side);

                        Hoster.SkillData.Charge[i].Using_Up = EditorGUILayout.Toggle("Using Up", Hoster.SkillData.Charge[i].Using_Up);
                        if (Hoster.SkillData.Charge[i].Using_Up)
                        {
                            o = EditorGUILayout.ObjectField("Up Prefab", Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Prefab_Up, typeof(GameObject), true) as GameObject;
                            {
                                if (XInnerEditor.CheckPrefab(o))
                                {
                                    Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Prefab_Up = o;
                                    string path = AssetDatabase.GetAssetPath(o).Remove(0, 17);
                                    Hoster.SkillData.Charge[i].Curve_Up = path.Remove(path.LastIndexOf('.'));
                                    Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Up = (o.GetComponent<XCurve>().Curve);
                                }
                                else
                                {
                                    Hoster.SkillData.Charge[i].Curve_Up = null;
                                    Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Up = null;
                                }
                            }
                            if (Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Up != null)
                                EditorGUILayout.CurveField("Curve", Hoster.SkillDataExtra.ChargeEx[i].Charge_Curve_Up);
                        }
                    }
                    else
                    {
                        float begin_at = (Hoster.SkillData.Charge[i].At);
                        EditorGUILayout.BeginHorizontal();
                        begin_at = EditorGUILayout.FloatField("Begin At", begin_at);
                        GUILayout.Label("(s)");
                        GUILayout.Label("", GUILayout.MaxWidth(30));
                        EditorGUILayout.EndHorizontal();

                        Hoster.ConfigData.Charge[i].Charge_Ratio = begin_at / Hoster.SkillData.Time;
                        if (Hoster.ConfigData.Charge[i].Charge_Ratio > 1) Hoster.ConfigData.Charge[i].Charge_Ratio = 1;

                        EditorGUILayout.BeginHorizontal();
                        Hoster.ConfigData.Charge[i].Charge_Ratio = EditorGUILayout.Slider("Begin Ratio", Hoster.ConfigData.Charge[i].Charge_Ratio, 0, 1);
                        GUILayout.Label("(0~1)", EditorStyles.miniLabel);
                        EditorGUILayout.EndHorizontal();

                        Hoster.SkillData.Charge[i].At = (Hoster.ConfigData.Charge[i].Charge_Ratio * Hoster.SkillData.Time);

                        /////////////////////////////////////////////////////////////////////////////////////////////////////
                        float end_at = (Hoster.SkillData.Charge[i].End);
                        EditorGUILayout.BeginHorizontal();
                        end_at = EditorGUILayout.FloatField("End At", end_at);
                        GUILayout.Label("(s)");
                        GUILayout.Label("", GUILayout.MaxWidth(30));
                        EditorGUILayout.EndHorizontal();
                        if (end_at < begin_at)
                            end_at = begin_at;

                        Hoster.ConfigData.Charge[i].Charge_End_Ratio = end_at / Hoster.SkillData.Time;
                        if (Hoster.ConfigData.Charge[i].Charge_End_Ratio > 1)
                            Hoster.ConfigData.Charge[i].Charge_End_Ratio = 1;

                        EditorGUILayout.BeginHorizontal();
                        Hoster.ConfigData.Charge[i].Charge_End_Ratio = EditorGUILayout.Slider("End Ratio", Hoster.ConfigData.Charge[i].Charge_End_Ratio, 0, 1);
                        GUILayout.Label("(0~1)", EditorStyles.miniLabel);
                        EditorGUILayout.EndHorizontal();

                        Hoster.SkillData.Charge[i].End = (Hoster.ConfigData.Charge[i].Charge_End_Ratio * Hoster.SkillData.Time);

                        float duration = (Hoster.SkillData.Charge[i].End - Hoster.SkillData.Charge[i].At);
                        EditorGUILayout.Space();
                        Hoster.SkillData.Charge[i].Offset = EditorGUILayout.FloatField("Offset", Hoster.SkillData.Charge[i].Offset);
                        EditorGUILayout.LabelField("Velocity", (Hoster.SkillData.Charge[i].Offset / duration).ToString("F2") + "/s");
                        EditorGUILayout.LabelField("Duration", duration.ToString("F2") + "s");
                        Hoster.SkillData.Charge[i].Height = EditorGUILayout.FloatField("Height", Hoster.SkillData.Charge[i].Height);
                        if (!Hoster.SkillData.Charge[i].AimTarget)
                        {
                            EditorGUILayout.BeginHorizontal();
                            Hoster.SkillData.Charge[i].Rotation_Speed = EditorGUILayout.FloatField("Rotation", Hoster.SkillData.Charge[i].Rotation_Speed);
                            GUILayout.Label("(degree/s)");
                            GUILayout.Label("", GUILayout.MaxWidth(30));
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    EditorGUILayout.Space();
                    Hoster.SkillData.Charge[i].StandOnAtEnd = EditorGUILayout.Toggle("Stand On At End", Hoster.SkillData.Charge[i].StandOnAtEnd);
                    Hoster.SkillData.Charge[i].Control_Towards = EditorGUILayout.Toggle("Enable Control Towards", Hoster.SkillData.Charge[i].Control_Towards);
                    if (Hoster.SkillData.Charge[i].Control_Towards && Hoster.SkillData.Charge[i].Using_Curve)
                    {
                        Hoster.SkillData.Charge[i].Velocity = EditorGUILayout.FloatField("Velocity", Hoster.SkillData.Charge[i].Velocity);
                    }
                    else
                        Hoster.SkillData.Charge[i].Velocity = 0;

                    if (Hoster.SkillData.NeedTarget)
                        Hoster.SkillData.Charge[i].AimTarget = EditorGUILayout.Toggle("Aim Target", Hoster.SkillData.Charge[i].AimTarget);
                    else
                        Hoster.SkillData.Charge[i].AimTarget = false;
                }

                if (i != Hoster.SkillData.Charge.Count - 1)
                {
                    GUILayout.Box("", _line);
                    EditorGUILayout.Space();
                }
            }
        }
    }
}