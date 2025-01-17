﻿using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using XUtliPoolLib;
using System.Reflection;
using System.Text;

namespace XEditor
{
    [CustomEditor(typeof(XSkillHoster))]
    public class XSkillPanel : Editor
    {
        public static readonly float frame = (1.0f / 30.0f);

        public static readonly string[] skill_type =
        {
            "JA-Combo Skill",
            "Arts Skill",
            "Ultra Skill",
            "Combined Skill"
        };

        public static string next_file = null;
        public static string last_file = null;

        private XSkillHoster _hoster = null;
        private List<XPanel> _panels = new List<XPanel>();

        private XResultPanel _result = new XResultPanel();
        private XChargePanel _charge = new XChargePanel();
        private XJAPanel _ja = new XJAPanel();
        private XHitPanel _hit = new XHitPanel();
        private XFxPanel _fx = new XFxPanel();
        private XScriptPanel _script = new XScriptPanel();
        private XMobPanel _mob = new XMobPanel();
        private XCastChainPanel _castchain = new XCastChainPanel();
        private XManipulationPanel _manipulation = new XManipulationPanel();
        private XLogicalPanel _logical = new XLogicalPanel();
        private XCameraMotionPanel _motion = new XCameraMotionPanel();
        private XCameraPostEffectPanel _post_effect = new XCameraPostEffectPanel();
        private XAudioPanel _audio = new XAudioPanel();
        private XCameraEffectPanel _cameraeffect = new XCameraEffectPanel();
        private XWarningPanel _warning = new XWarningPanel();
        private XCombinedPanel _combined = new XCombinedPanel();
        private XComboSkillPanel _combo = new XComboSkillPanel();

        private GUIContent _content_add = new GUIContent("+", "Added Skill Timer.");

        private string _status = "nothing more...";

        private int _option = 0;
        private List<string> _xOptions = new List<string>();

        private GUIStyle _labelstyle = null;
        private GUIStyle _myLabelStyle2 = null;

        public void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
            if (_hoster == null) _hoster = target as XSkillHoster;

            if (XSkillHoster.Quit)
            {
                _hoster.FetchDataBack();
                XSkillHoster.Quit = false;
            }

            AddPanels();

            _xOptions.Clear();
            _xOptions.Add("ResultAt");
            _xOptions.Add("ChargeTo");
            _xOptions.Add("Hit");
            _xOptions.Add("Manipulation");
            _xOptions.Add("Fx");
            _xOptions.Add("Audio");
            _xOptions.Add("Camera Shake");
            _xOptions.Add("Warning");
            _xOptions.Add("Mobs");

            foreach (XPanel panel in _panels)
            {
                panel.Hoster = _hoster;
                panel.Init();
            }

            _combo.Init();
            _combined.Init();
            _script.Init();
            _castchain.Init();
            _logical.Init();
            _motion.Init();
            _post_effect.Init();

            _combo.Hoster = _hoster;
            _combined.Hoster = _hoster;
            _script.Hoster = _hoster;
            _castchain.Hoster = _hoster;
            _logical.Hoster = _hoster;
            _motion.Hoster = _hoster;
            _post_effect.Hoster = _hoster;
        }

        public override void OnInspectorGUI()
        {
            if (_labelstyle == null)
            {
                _labelstyle = new GUIStyle(EditorStyles.boldLabel);
                _labelstyle.fontSize = 13;
            }

            if (_myLabelStyle2 == null)
            {
                _myLabelStyle2 = new GUIStyle(GUI.skin.label);
                _myLabelStyle2.fontStyle = FontStyle.Italic;
            }

            /*****Camera Settings*****/
            GUILayout.Label("Camera Settings :", _labelstyle);
            _hoster.defaultFov = EditorGUILayout.FloatField("Field of View", _hoster.defaultFov);

            /*****Base Settings*****/
            GUILayout.Label("Basic Settings :", _labelstyle);
            /*EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Script File", GUILayout.Width(EditorGUIUtility.labelWidth));
                EditorGUILayout.SelectableLabel(_hoster.SkillDataExtra.ScriptFile, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            EditorGUILayout.EndHorizontal();*/

            EditorGUILayout.LabelField("Data File", _hoster.SkillDataExtra.ScriptFile);
            EditorGUILayout.LabelField("Data Path", _hoster.SkillDataExtra.ScriptPath);

            _hoster.ConfigData.Speed = EditorGUILayout.FloatField("Speed", _hoster.ConfigData.Speed);
            _hoster.ConfigData.RotateSpeed = EditorGUILayout.FloatField("Rotate Speed", _hoster.ConfigData.RotateSpeed);

            EditorGUILayout.Space();
            _hoster.EditorData.XAutoSelected =
                EditorGUILayout.Toggle("Auto Selection", _hoster.EditorData.XAutoSelected);
            _hoster.EditorData.XFrameByFrame =
                EditorGUILayout.Toggle("Frame By Frame", _hoster.EditorData.XFrameByFrame);
            EditorGUILayout.Space();

            /*****Skill Settings*****/
            GUILayout.Label("Skill Settings :", _labelstyle);

            if (_hoster.SkillData.TypeToken != 3)
            {
                EditorGUI.BeginChangeCheck();
                AnimationClip clip =
                    EditorGUILayout.ObjectField("Clip", _hoster.SkillDataExtra.SkillClip, typeof(AnimationClip), true)
                        as AnimationClip;
                if (EditorGUI.EndChangeCheck() && clip != null)
                {
                    _hoster.SkillDataExtra.SkillClip = clip;
                    _hoster.ConfigData.SkillClip = AssetDatabase.GetAssetPath(clip);
                    _hoster.ConfigData.SkillClipName = clip.name;
                }

                EditorGUILayout.LabelField("Clip Name", _hoster.ConfigData.SkillClip);

                if (_hoster.SkillDataExtra.SkillClip != null)
                    _hoster.SkillDataExtra.SkillClip_Frame = (_hoster.SkillDataExtra.SkillClip.length / frame);
            }

            EditorGUILayout.LabelField("Clip Length",
                (_hoster.SkillDataExtra.SkillClip_Frame * frame).ToString("F3") + "s" + "\t" +
                _hoster.SkillDataExtra.SkillClip_Frame.ToString("F1") + "(frame)");
            EditorGUILayout.Space();

            if (_hoster.SkillData.TypeToken == 3) _hoster.SkillDataExtra.SkillClip_Frame = 0;

            _hoster.SkillData.NeedTarget = EditorGUILayout.Toggle("Need Target", _hoster.SkillData.NeedTarget);
            _hoster.SkillData.OnceOnly = EditorGUILayout.Toggle("Once Only", _hoster.SkillData.OnceOnly);
            _hoster.SkillData.IgnoreCollision =
                EditorGUILayout.Toggle("Disable Collision", _hoster.SkillData.IgnoreCollision);

            EditorGUILayout.Space();
            _hoster.SkillData.Time = EditorGUILayout.FloatField("Skill Time", _hoster.SkillData.Time);
            _hoster.SkillData.CoolDown = EditorGUILayout.FloatField("Cool Down", _hoster.SkillData.CoolDown);
            //EditorGUI.BeginChangeCheck();
            _hoster.SkillData.TypeToken = EditorGUILayout.Popup("Skill Type", _hoster.SkillData.TypeToken, skill_type);

            if (_hoster.SkillData.TypeToken != 3 && _hoster.SkillDataExtra.SkillClip == null) return;
            //if(EditorGUI.EndChangeCheck())
            {
                if (_hoster.SkillData.TypeToken == 0)
                {
                    _hoster.SkillData.SkillPosition = (int)EditorGUILayout.Popup("Position",
                        _hoster.SkillData.SkillPosition, XSkillData.JA_Command);
                    if (!_xOptions.Contains("Combos"))
                        _xOptions.Insert(2, "Combos");
                }
                else if (_hoster.SkillData.TypeToken != 3)
                {
                    if (_xOptions.Contains("Combos"))
                        _xOptions.Remove("Combos");
                }
            }
            EditorGUILayout.Space();

            _hoster.SkillData.Cast_Range_Rect =
                EditorGUILayout.Toggle("Cast Range Rect", _hoster.SkillData.Cast_Range_Rect);
            if (_hoster.SkillData.Cast_Range_Rect)
            {
                _hoster.SkillData.Cast_Scope =
                    EditorGUILayout.FloatField("Cast Range Width", _hoster.SkillData.Cast_Scope);
                _hoster.SkillData.Cast_Range_Upper =
                    EditorGUILayout.FloatField("Cast Range Depth", _hoster.SkillData.Cast_Range_Upper);
                _hoster.SkillData.Cast_Offset_X =
                    EditorGUILayout.FloatField("Cast Offset X", _hoster.SkillData.Cast_Offset_X);
                _hoster.SkillData.Cast_Offset_Z =
                    EditorGUILayout.FloatField("Cast Offset Z", _hoster.SkillData.Cast_Offset_Z);
                EditorGUILayout.LabelField("Positive means Clock-wise.", _myLabelStyle2);
                _hoster.SkillData.Cast_Scope_Shift =
                    EditorGUILayout.FloatField("Cast Rect Shift", _hoster.SkillData.Cast_Scope_Shift);
            }
            else
            {
                _hoster.SkillData.Cast_Range_Lower =
                    EditorGUILayout.FloatField("Cast Range (↓)", _hoster.SkillData.Cast_Range_Lower);
                _hoster.SkillData.Cast_Range_Upper =
                    EditorGUILayout.FloatField("Cast Range (↑)", _hoster.SkillData.Cast_Range_Upper);
                _hoster.SkillData.Cast_Scope = EditorGUILayout.FloatField("Cast Scope", _hoster.SkillData.Cast_Scope);
                _hoster.SkillData.Cast_Offset_X =
                    EditorGUILayout.FloatField("Cast Offset X", _hoster.SkillData.Cast_Offset_X);
                _hoster.SkillData.Cast_Offset_Z =
                    EditorGUILayout.FloatField("Cast Offset Z", _hoster.SkillData.Cast_Offset_Z);
                EditorGUILayout.LabelField("Positive means Clock-wise.", _myLabelStyle2);
                _hoster.SkillData.Cast_Scope_Shift =
                    EditorGUILayout.FloatField("Cast Scope Shift", _hoster.SkillData.Cast_Scope_Shift);
            }

            EditorGUILayout.Space();

            _hoster.SkillData.ForCombinedOnly =
                EditorGUILayout.Toggle("Combined Only", _hoster.SkillData.ForCombinedOnly);
            EditorGUILayout.BeginHorizontal();
            _hoster.SkillData.CameraTurnBack =
                EditorGUILayout.FloatField("Camera Tail Speed Ratio", _hoster.SkillData.CameraTurnBack);
            EditorGUILayout.LabelField("(multiplied by)", GUILayout.MaxWidth(90));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            bool MultipleAttackSupported = _hoster.SkillData.MultipleAttackSupported;
            _hoster.SkillData.MultipleAttackSupported =
                EditorGUILayout.Toggle("Multiple Direction", _hoster.SkillData.MultipleAttackSupported);
            if (!MultipleAttackSupported && _hoster.SkillData.MultipleAttackSupported)
            {
                _hoster.SkillData.MultipleAttackSupported = EditorUtility.DisplayDialog("Confirm your selection.",
                    "Multiple direction needs 8 attacking-animations support!",
                    "Confirmed", "Cancel");
            }

            if (_hoster.SkillData.MultipleAttackSupported)
            {
                _hoster.SkillData.BackTowardsDecline =
                    EditorGUILayout.Slider("Backwards Decline", _hoster.SkillData.BackTowardsDecline, 0, 1);
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("PvP Version Skill", _hoster.SkillData.PVP_Script_Name, GUILayout.MaxWidth(500));
            if (!string.IsNullOrEmpty(_hoster.SkillData.PVP_Script_Name))
            {
                if (GUILayout.Button("Delete", GUILayout.MaxWidth(70)))
                {
                    _hoster.SkillData.PVP_Script_Name = null;
                }
            }

            if (GUILayout.Button("Browser", GUILayout.MaxWidth(70)))
            {
                string file = EditorUtility.OpenFilePanel("Select Skp file", XEditorPath.Skp, "txt");
                if (file.Length > 0)
                {
                    int s = file.LastIndexOf('/');
                    int e = file.LastIndexOf('.');
                    _hoster.SkillData.PVP_Script_Name = file.Substring(s + 1, e - s - 1);
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            {
                /*****Timer*****/
                GUILayout.Label("Timer :", _labelstyle);
                EditorGUILayout.BeginHorizontal();
                if (_hoster.SkillData.TypeToken == 3)
                {
                    _option = EditorGUILayout.Popup("Create", _option,
                        new string[] { "Combined", "Fx", "Audio", "Camera Shake" });
                }
                else
                    _option = EditorGUILayout.Popup("Create", _option, _xOptions.ToArray());

                if (GUILayout.Button(_content_add, GUILayout.MaxWidth(30)))
                {
                    if (_hoster.SkillData.TypeToken == 3)
                    {
                        switch (_option)
                        {
                            case 0:
                                _combined.Add<XCombinedData>();
                                break;
                            case 1:
                                _fx.Add<XFxData>();
                                break;
                            case 2:
                                _audio.Add<XAudioData>();
                                break;
                            case 3:
                                _cameraeffect.Add<XCameraEffectData>();
                                break;
                        }
                    }
                    else
                    {
                        if (_xOptions.Contains("Combos"))
                        {
                            switch (_option)
                            {
                                case 0:
                                    _result.Add<XResultData>();
                                    break;
                                case 1:
                                    _charge.Add<XChargeData>();
                                    break;
                                case 2:
                                    _ja.Add<XJAData>();
                                    break;
                                case 3:
                                    _hit.Add<XHitData>();
                                    break;
                                case 4:
                                    _manipulation.Add<XManipulationData>();
                                    break;
                                case 5:
                                    _fx.Add<XFxData>();
                                    break;
                                case 6:
                                    _audio.Add<XAudioData>();
                                    break;
                                case 7:
                                    _cameraeffect.Add<XCameraEffectData>();
                                    break;
                                case 8:
                                    _warning.Add<XWarningData>();
                                    break;
                                case 9:
                                    _mob.Add<XMobUnitData>();
                                    break;
                            }
                        }
                        else
                        {
                            switch (_option)
                            {
                                case 0:
                                    _result.Add<XResultData>();
                                    break;
                                case 1:
                                    _charge.Add<XChargeData>();
                                    break;
                                case 2:
                                    _hit.Add<XHitData>();
                                    break;
                                case 3:
                                    _manipulation.Add<XManipulationData>();
                                    break;
                                case 4:
                                    _fx.Add<XFxData>();
                                    break;
                                case 5:
                                    _audio.Add<XAudioData>();
                                    break;
                                case 6:
                                    _cameraeffect.Add<XCameraEffectData>();
                                    break;
                                case 7:
                                    _warning.Add<XWarningData>();
                                    break;
                                case 8:
                                    _mob.Add<XMobUnitData>();
                                    break;
                            }
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            if (_hoster.SkillData.TypeToken == 3)
            {
                _combined.OnGUI();
                EditorGUILayout.Space();
                _hit.OnGUI();
                EditorGUILayout.Space();
                _fx.OnGUI();
                EditorGUILayout.Space();
                _audio.OnGUI();
                EditorGUILayout.Space();
                _cameraeffect.OnGUI();
                EditorGUILayout.Space();
            }
            else
            {
                foreach (XPanel panel in _panels)
                {
                    if (_hoster.SkillData.TypeToken != 0 && panel is XJAPanel) continue;

                    panel.OnGUI();
                    EditorGUILayout.Space();
                }
            }

            /*****Pro Settings*****/
            GUILayout.Label("Pro Settings :", _labelstyle);
            _logical.OnGUI();
            EditorGUILayout.Space();

            _motion.OnGUI();
            EditorGUILayout.Space();
            _post_effect.OnGUI();
            EditorGUILayout.Space();

            if (_hoster.SkillData.TypeToken != 3)
            {
                _script.OnGUI();
                EditorGUILayout.Space();
                _castchain.OnGUI();
                EditorGUILayout.Space();
            }

            /*****Operation*****/
            GUILayout.Label("Operation :", _labelstyle);
            _combo.OnGUI();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply", GUILayout.MaxWidth(150))) SerializeData(GetDataFileWithPath());
            EditorGUILayout.Space();
            if (GUILayout.Button("Revert", GUILayout.MaxWidth(150))) DeserializeData(GetDataFileWithPath());
            if (!string.IsNullOrEmpty(last_file))
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Open Last", GUILayout.MaxWidth(150))) next_file = last_file;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            /*****Status*****/
            GUILayout.Label("*Status* : " + _status);
            /*****Ending*****/
            EditorGUILayout.Space();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
                _status = "not saved.";
            }

            XDataBuilder.singleton.Update(_hoster);
            /*Repaint();*/

            if (!string.IsNullOrEmpty(next_file))
            {
                if (EditorUtility.DisplayDialog("Open new skill",
                        "Do you want to save current skill?",
                        "Do", "Do Not"))
                {
                    SerializeData(GetDataFileWithPath());
                }

                last_file = GetDataFileWithPath();
                XInnerEditor.OpenSkill(next_file);

                next_file = null;
            }
        }

        void OnSceneGUI()
        {
            if (_hoster.nHotID < 0 || _hoster.CurrentSkillData.Result == null ||
                _hoster.nHotID >= _hoster.CurrentSkillData.Result.Count) return;

            float offset_x = _hoster.CurrentSkillData.Result[_hoster.nHotID].LongAttackEffect
                ? (_hoster.CurrentSkillData.Result[_hoster.nHotID].LongAttackData.TriggerAtEnd
                    ? _hoster.CurrentSkillData.Result[_hoster.nHotID].Offset_X +
                      _hoster.CurrentSkillData.Result[_hoster.nHotID].LongAttackData.At_X
                    : _hoster.CurrentSkillData.Result[_hoster.nHotID].LongAttackData.At_X)
                : _hoster.CurrentSkillData.Result[_hoster.nHotID].Offset_X;
            float offset_z = _hoster.CurrentSkillData.Result[_hoster.nHotID].LongAttackEffect
                ? (_hoster.CurrentSkillData.Result[_hoster.nHotID].LongAttackData.TriggerAtEnd
                    ? _hoster.CurrentSkillData.Result[_hoster.nHotID].Offset_Z +
                      _hoster.CurrentSkillData.Result[_hoster.nHotID].LongAttackData.At_Z
                    : _hoster.CurrentSkillData.Result[_hoster.nHotID].LongAttackData.At_Z)
                : _hoster.CurrentSkillData.Result[_hoster.nHotID].Offset_Z;

            Vector3 offset = _hoster.transform.rotation * new Vector3(offset_x, 0, offset_z);
            Handles.color = new Color(1, 1, 1, 0.1f);

            if (_hoster.ShownTransform == null) _hoster.ShownTransform = _hoster.transform;

            Vector3 m = _hoster.nResultForward == Vector3.zero
                ? _hoster.ShownTransform.forward
                : _hoster.nResultForward;

            if (_hoster.CurrentSkillData.Result[_hoster.nHotID].LongAttackEffect)
            {
                m = RotateRad(-_hoster.CurrentSkillData.Result[_hoster.nHotID].Scope / 2.0f * Mathf.Deg2Rad,
                    _hoster.ShownTransform.up) * m;

                if (_hoster.CurrentSkillData.Result[_hoster.nHotID].LongAttackData.TriggerAtEnd)
                {
                    Handles.DrawSolidArc(
                        _hoster.ShownTransform == _hoster.transform
                            ? _hoster.ShownTransform.position + offset
                            : _hoster.ShownTransform.position,
                        _hoster.ShownTransform.up,
                        m,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Scope,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Range);

                    Handles.color = new Color(0f, 0f, 0f, 0.18f);
                    Handles.DrawSolidArc(
                        _hoster.ShownTransform == _hoster.transform
                            ? _hoster.ShownTransform.position + offset
                            : _hoster.ShownTransform.position,
                        _hoster.ShownTransform.up,
                        m,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Scope,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Low_Range - 0.01f);

                    Handles.color = Color.green;
                    Handles.ArrowHandleCap(0,
                        _hoster.ShownTransform == _hoster.transform
                            ? _hoster.ShownTransform.position + offset
                            : _hoster.ShownTransform.position,
                        _hoster.ShownTransform.rotation,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Range,
                        EventType.Repaint);
                }
                else
                {
                    Handles.DrawSolidArc(
                        _hoster.ShownTransform == _hoster.transform
                            ? _hoster.ShownTransform.position + offset
                            : _hoster.ShownTransform.position,
                        _hoster.ShownTransform.up,
                        _hoster.ShownTransform.forward,
                        360,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].LongAttackData.Radius);

                    Handles.color = Color.green;
                    Handles.ArrowHandleCap(0,
                        _hoster.ShownTransform == _hoster.transform
                            ? _hoster.ShownTransform.position + offset
                            : _hoster.ShownTransform.position,
                        _hoster.ShownTransform.rotation,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].LongAttackData.Radius,
                        EventType.Repaint);
                }
            }
            else
            {
                if (_hoster.CurrentSkillData.Result[_hoster.nHotID].Sector_Type)
                {
                    m = RotateRad(-_hoster.CurrentSkillData.Result[_hoster.nHotID].Scope / 2.0f * Mathf.Deg2Rad,
                        _hoster.ShownTransform.up) * m;

                    Handles.DrawSolidArc(_hoster.ShownTransform.position + offset,
                        _hoster.ShownTransform.up,
                        m,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Scope,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Range);

                    Handles.color = new Color(0f, 0f, 0f, 0.18f);
                    Handles.DrawSolidArc(_hoster.ShownTransform.position + offset,
                        _hoster.ShownTransform.up,
                        m,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Scope,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Low_Range - 0.01f);

                    Handles.color = Color.green;
                    Handles.ArrowHandleCap(0,
                        _hoster.ShownTransform.position + offset,
                        _hoster.ShownTransform.rotation,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Range,
                        EventType.Repaint);
                }
                else
                {
                    Vector3[] vecs = new Vector3[4];
                    Quaternion q = XCommon.singleton.VectorToQuaternion(XCommon.singleton.HorizontalRotateVetor3(m,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].None_Sector_Angle_Shift));

                    vecs[0] = _hoster.ShownTransform.position + offset + q * new Vector3(
                        -_hoster.CurrentSkillData.Result[_hoster.nHotID].Scope / 2.0f, 0,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Rect_HalfEffect
                            ? 0
                            : (-_hoster.CurrentSkillData.Result[_hoster.nHotID].Range / 2.0f));
                    vecs[1] = _hoster.ShownTransform.position + offset + q * new Vector3(
                        -_hoster.CurrentSkillData.Result[_hoster.nHotID].Scope / 2.0f, 0,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Range / 2.0f);
                    vecs[2] = _hoster.ShownTransform.position + offset + q * new Vector3(
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Scope / 2.0f, 0,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Range / 2.0f);
                    vecs[3] = _hoster.ShownTransform.position + offset + q * new Vector3(
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Scope / 2.0f, 0,
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Rect_HalfEffect
                            ? 0
                            : (-_hoster.CurrentSkillData.Result[_hoster.nHotID].Range / 2.0f));

                    Handles.DrawSolidRectangleWithOutline(vecs, new Color(1, 1, 1, 1), new Color(0, 0, 0, 1));

                    Handles.color = Color.green;
                    Handles.ArrowHandleCap(0,
                        _hoster.ShownTransform.position + offset,
                        _hoster.ShownTransform.rotation *
                        XCommon.singleton.FloatToQuaternion(_hoster.CurrentSkillData.Result[_hoster.nHotID]
                            .None_Sector_Angle_Shift),
                        _hoster.CurrentSkillData.Result[_hoster.nHotID].Range,
                        EventType.Repaint);
                }
            }
        }

        private Quaternion RotateRad(float rad, Vector3 axis)
        {
            float r = rad / 2.0f;

            float x = axis.x * Mathf.Sin(r);
            float y = axis.y * Mathf.Sin(r);
            float z = axis.z * Mathf.Sin(r);
            float w = Mathf.Cos(r);

            return new Quaternion(x, y, z, w);
        }

        private void SerializeData(string file)
        {
            _hoster.SkillData.Name = _hoster.SkillDataExtra.ScriptFile;

            if (_hoster.SkillData.TypeToken != 3)
            {
                string path = AssetDatabase.GetAssetPath(_hoster.SkillDataExtra.SkillClip).Remove(0, 17);
                _hoster.SkillData.ClipName = path.Remove(path.LastIndexOf('.'));
            }
            else
                _hoster.SkillData.ClipName = null;

            if (_hoster.SkillData.TypeToken != 0) _hoster.SkillData.Ja = null;
            if (_hoster.SkillData.TypeToken != 3) _hoster.SkillData.Combined = null;

            if (_hoster.SkillData.Result != null)
            {
                foreach (XResultData data in _hoster.SkillData.Result)
                {
                    if (data.Warning)
                    {
                        if (_hoster.SkillData.Warning == null || data.Warning_Idx >= _hoster.SkillData.Warning.Count)
                        {
                            EditorUtility.DisplayDialog("Confirm your configuration.",
                                "Please select right Warning Index!",
                                "Ok");

                            return;
                        }
                    }
                }
            }

            if (_hoster.SkillData.Warning != null)
            {
                for (int idx = 0; idx < _hoster.SkillData.Warning.Count; idx++)
                {
                    int i = 0;
                    for (; i < _hoster.SkillData.Result.Count; i++)
                    {
                        if (_hoster.SkillData.Result[i].Warning && _hoster.SkillData.Result[i].Warning_Idx == idx)
                            break;
                    }

                    if (i == _hoster.SkillData.Result.Count)
                    {
                        EditorUtility.DisplayDialog("Confirm your configuration.",
                            "Please MATCH your Results' and Warnings' Index!",
                            "Ok");

                        return;
                    }
                }
            }

            if (_hoster.SkillData.Combined != null)
            {
                for (int idx = 0; idx < _hoster.SkillData.Combined.Count; idx++)
                {
                    if (_hoster.SkillData.Combined[idx].Name == null ||
                        _hoster.SkillData.Combined[idx].Name.Length == 0 ||
                        (_hoster.SkillData.Combined[idx].End - _hoster.SkillData.Combined[idx].At) < frame)
                    {
                        EditorUtility.DisplayDialog("Confirm your configuration.",
                            "Please select the Combined skill.",
                            "Ok");

                        return;
                    }
                }
            }

            if (!((_hoster.SkillData.Result == null && _hoster.SkillData.Hit == null) ||
                  _hoster.SkillData.Result.Count == _hoster.SkillData.Hit.Count))
            {
                EditorUtility.DisplayDialog("Confirm your configuration.",
                    "Result count is not match with Hit count!",
                    "Ok");
                return;
            }

            if (_hoster.SkillData.Charge != null)
            {
                int idx = 0;
                for (; idx < _hoster.SkillData.Charge.Count; idx++)
                {
                    if (!_hoster.SkillData.Charge[idx].Using_Curve &&
                        _hoster.SkillData.Charge[idx].End <= 0)
                        //#36415 & support rotation
                        //_hoster.SkillData.Charge[idx].Charge_Offset == 0
                    {
                        EditorUtility.DisplayDialog("Confirm your configuration.",
                            "you are saving an invalid charge!",
                            "Ok");
                        return;
                    }
                }
            }

            if (_hoster.SkillData.Hit != null)
            {
                int idx = 0;
                for (; idx < _hoster.SkillData.Hit.Count; idx++)
                {
                    if (((_hoster.SkillData.Hit[idx].State == XBeHitState.Hit_Back ||
                          _hoster.SkillData.Hit[idx].State == XBeHitState.Hit_Roll) &&
                         (_hoster.SkillData.Hit[idx].Time_Present_Straight == 0 ||
                          _hoster.SkillData.Hit[idx].Time_Hard_Straight == 0 ||
                          _hoster.SkillData.Hit[idx].Offset == 0)) ||
                        (_hoster.SkillData.Hit[idx].Height == 0 &&
                         _hoster.SkillData.Hit[idx].State == XBeHitState.Hit_Fly) ||
                        (_hoster.SkillData.Hit[idx].FreezeDuration == 0 &&
                         _hoster.SkillData.Hit[idx].State == XBeHitState.Hit_Freezed))
                        break;
                }

                if (idx < _hoster.SkillData.Hit.Count)
                {
                    EditorUtility.DisplayDialog("Confirm your configuration.",
                        "your Hit parameters == 0!",
                        "Ok");

                    return;
                }
            }

            StripData(_hoster.SkillData);

            XDataIO<XSkillData>.singleton.SerializeData(file, _hoster.SkillData);
            XDataIO<XConfigData>.singleton.SerializeData(XEditorPath.GetCfgFromSkp(file), _hoster.ConfigData);

            XDataBuilder.Time = File.GetLastWriteTime(file);
            _status = "already saved.";
        }

        private void DeserializeData(string file)
        {
            _hoster.ConfigData = XDataIO<XConfigData>.singleton.DeserializeData(XEditorPath.GetCfgFromSkp(file));
            _hoster.SkillData = XDataIO<XSkillData>.singleton.DeserializeData(file);

            XDataBuilder.singleton.HotBuild(_hoster, _hoster.ConfigData);
            XDataBuilder.singleton.HotBuildEx(_hoster, _hoster.ConfigData);
        }

        private void AddPanels()
        {
            _panels.Clear();

            _panels.Add(_result);
            _panels.Add(_charge);
            _panels.Add(_hit);
            _panels.Add(_ja);
            _panels.Add(_manipulation);
            _panels.Add(_fx);
            _panels.Add(_audio);
            _panels.Add(_cameraeffect);
            _panels.Add(_warning);
            _panels.Add(_mob);
        }

        private string GetDataFileWithPath()
        {
            return _hoster.SkillDataExtra.ScriptPath + _hoster.SkillDataExtra.ScriptFile + ".txt";
        }

        private void StripData(XSkillData data)
        {
            if (string.IsNullOrEmpty(data.Name)) data.Name = null;
            if (string.IsNullOrEmpty(data.ClipName)) data.ClipName = null;
            if (string.IsNullOrEmpty(data.PVP_Script_Name)) data.PVP_Script_Name = null;

            if (data.Result != null && data.Result.Count > 0)
            {
                foreach (XResultData r in data.Result)
                {
                    if (r.LongAttackEffect)
                    {
                        if (string.IsNullOrEmpty(r.LongAttackData.Prefab)) r.LongAttackData.Prefab = null;
                        if (string.IsNullOrEmpty(r.LongAttackData.HitGround_Fx)) r.LongAttackData.HitGround_Fx = null;
                        if (string.IsNullOrEmpty(r.LongAttackData.End_Fx)) r.LongAttackData.End_Fx = null;
                        if (string.IsNullOrEmpty(r.LongAttackData.Audio)) r.LongAttackData.Audio = null;
                        if (string.IsNullOrEmpty(r.LongAttackData.End_Audio)) r.LongAttackData.End_Audio = null;

                        if (!r.LongAttackData.Manipulation || r.LongAttackData.Type == XResultBulletType.Ring)
                        {
                            r.LongAttackData.Manipulation = false;
                            r.LongAttackData.ManipulationForce = 0;
                            r.LongAttackData.ManipulationRadius = 0;
                        }

                        if (!data.NeedTarget || r.LongAttackData.Type == XResultBulletType.Satellite ||
                            r.LongAttackData.Type == XResultBulletType.Ring)
                        {
                            r.LongAttackData.AimTargetCenter = false;
                            r.LongAttackData.Follow = false;
                        }

                        if (r.LongAttackData.Type == XResultBulletType.Satellite ||
                            r.LongAttackData.Type == XResultBulletType.Ring)
                        {
                            r.LongAttackData.At_X = 0;
                            r.LongAttackData.At_Y = 0;
                            r.LongAttackData.At_Z = 0;
                            r.LongAttackData.IsPingPong = false;
                            r.LongAttackData.Velocity = 0;
                            r.LongAttackData.Stickytime = 0;
                            if (r.LongAttackData.Type == XResultBulletType.Ring)
                            {
                                r.LongAttackData.Radius = 0;
                                r.LongAttackData.FireAngle = 0;
                                r.LongAttackData.TriggerAtEnd = false;
                                r.LongAttackData.FlyWithTerrain = false;
                            }
                        }

                        if (r.LongAttackData.Type != XResultBulletType.Ring) r.LongAttackData.RingFull = false;
                        if (r.LongAttackData.Type != XResultBulletType.Satellite &&
                            r.LongAttackData.Type != XResultBulletType.Ring)
                        {
                            r.LongAttackData.RingRadius = 0;
                            r.LongAttackData.RingVelocity = 0;
                            r.LongAttackData.Palstance = 0;
                        }

                        if (!r.LongAttackData.TriggerAtEnd) r.LongAttackData.TriggerAtEnd_Count = 0;
                        if (r.LongAttackData.TriggerAtEnd_Count == 0) r.LongAttackData.TriggerAtEnd_Cycle = 0;
                    }
                    else
                        r.LongAttackData = null;
                }
            }
            else
                data.Result = null;

            ///////////////////////////////////////////////////
            if (data.Charge != null && data.Charge.Count > 0)
            {
                foreach (XChargeData c in data.Charge)
                {
                    if (string.IsNullOrEmpty(c.Curve_Forward)) c.Curve_Forward = null;
                    if (string.IsNullOrEmpty(c.Curve_Side)) c.Curve_Side = null;
                    if (string.IsNullOrEmpty(c.Curve_Up)) c.Curve_Up = null;
                }
            }
            else
                data.Charge = null;

            ///////////////////////////////////////////////////
            if (data.Hit != null && data.Hit.Count > 0)
            {
                foreach (XHitData h in data.Hit)
                {
                    if (string.IsNullOrEmpty(h.Fx)) h.Fx = null;

                    if (h.Additional_Using_Default)
                    {
                        h.Additional_Hit_Time_Present_Straight = 0;
                        h.Additional_Hit_Time_Hard_Straight = 0;
                        h.Additional_Hit_Offset = 0;
                        h.Additional_Hit_Height = 0;
                    }
                }
            }
            else
                data.Hit = null;

            ///////////////////////////////////////////////////
            if (data.Manipulation != null && data.Manipulation.Count > 0)
            {
            }
            else
                data.Manipulation = null;

            ///////////////////////////////////////////////////
            if (data.Ja != null && data.Ja.Count > 0)
            {
                foreach (XJAData j in data.Ja)
                {
                    XJAData n = new XJAData();
                    n.Index = j.Index;

                    if (string.IsNullOrEmpty(j.Next_Name)) j.Next_Name = null;
                    if (string.IsNullOrEmpty(j.Name)) j.Name = null;
                }
            }
            else
                data.Ja = null;

            ///////////////////////////////////////////////////
            if (data.Script != null)
            {
                int c = 0;
                if (string.IsNullOrEmpty(data.Script.Result_Name))
                {
                    data.Script.Result_Name = null;
                    c++;
                }

                if (string.IsNullOrEmpty(data.Script.Start_Name))
                {
                    data.Script.Start_Name = null;
                    c++;
                }

                if (string.IsNullOrEmpty(data.Script.Update_Name))
                {
                    data.Script.Update_Name = null;
                    c++;
                }

                if (string.IsNullOrEmpty(data.Script.Stop_Name))
                {
                    data.Script.Stop_Name = null;
                    c++;
                }

                if (c == 5) data.Script = null;
            }

            ///////////////////////////////////////////////////
            if (data.Combined != null && data.Combined.Count > 0)
            {
                foreach (XCombinedData c in data.Combined)
                {
                    if (string.IsNullOrEmpty(c.Name)) c.Name = null;
                }
            }
            else
                data.Combined = null;

            ///////////////////////////////////////////////////
            if (data.Logical != null)
            {
                if (string.IsNullOrEmpty(data.Logical.Association_Skill)) data.Logical.Association_Skill = null;
                if (string.IsNullOrEmpty(data.Logical.Exstring)) data.Logical.Exstring = null;

                if (data.Logical.QTEData == null || data.Logical.QTEData.Count == 0)
                    data.Logical.QTEData = null;

                if (data.Logical.CanCastAt_QTE == 0) data.Logical.QTE_Key = 0;
                if (data.Logical.PreservedStrength == 0)
                {
                    data.Logical.PreservedAt = 0.0f;
                    data.Logical.PreservedEndAt = 0.0f;
                }
            }

            ///////////////////////////////////////////////////
            if (data.CameraMotion != null && !string.IsNullOrEmpty(data.CameraMotion.Motion3D))
            {
                data.CameraMotion.Motion = null;
                if (string.IsNullOrEmpty(data.CameraMotion.Motion2_5D)) data.CameraMotion.Motion2_5D = null;
            }
            else
                data.CameraMotion = null;

            ///////////////////////////////////////////////////
            if (data.CameraPostEffect != null &&
                (data.CameraPostEffect.End > 0 ||
                 data.CameraPostEffect.Solid_End > 0))
            {
                if (string.IsNullOrEmpty(data.CameraPostEffect.Effect)) data.CameraPostEffect.Effect = null;
                if (string.IsNullOrEmpty(data.CameraPostEffect.Shader)) data.CameraPostEffect.Shader = null;
            }
            else
                data.CameraPostEffect = null;

            ///////////////////////////////////////////////////
            if (data.Audio != null && data.Audio.Count > 0)
            {
                foreach (XAudioData a in data.Audio)
                {
                    if (string.IsNullOrEmpty(a.Clip)) a.Clip = null;
                }
            }
            else
                data.Audio = null;

            ///////////////////////////////////////////////////
            if (data.Fx != null && data.Fx.Count > 0)
            {
                foreach (XFxData f in data.Fx)
                {
                    if (string.IsNullOrEmpty(f.Fx)) f.Fx = null;
                    if (string.IsNullOrEmpty(f.Bone)) f.Bone = null;
                }
            }
            else
                data.Fx = null;

            ///////////////////////////////////////////////////
            if (data.CameraEffect != null && data.CameraEffect.Count > 0)
            {
                foreach (XCameraEffectData e in data.CameraEffect)
                {
                }
            }
            else
                data.CameraEffect = null;

            ///////////////////////////////////////////////////
            if (data.Warning != null && data.Warning.Count > 0)
            {
                foreach (XWarningData w in data.Warning)
                {
                    if (string.IsNullOrEmpty(w.Fx)) w.Fx = null;
                    switch (w.Type)
                    {
                        case XWarningType.Warning_None:
                        {
                            w.Mobs_Inclusived = false;
                            w.RandomWarningPos = false;
                            w.MaxRandomTarget = 0;
                        }
                            break;
                        case XWarningType.Warning_Target:
                        {
                            w.Mobs_Inclusived = false;
                            w.MaxRandomTarget = 0;
                        }
                            break;
                        case XWarningType.Warning_All:
                        {
                            w.OffsetX = 0;
                            w.OffsetY = 0;
                            w.OffsetZ = 0;
                            w.MaxRandomTarget = 0;
                            if (w.RandomWarningPos) w.Mobs_Inclusived = false;
                        }
                            break;
                        case XWarningType.Warning_Multiple:
                        {
                            w.OffsetX = 0;
                            w.OffsetY = 0;
                            w.OffsetZ = 0;
                            w.Mobs_Inclusived = false;
                        }
                            break;
                    }

                    if (!w.RandomWarningPos)
                    {
                        w.PosRandomRange = 0;
                        w.PosRandomCount = 0;
                    }
                }
            }
            else
                data.Warning = null;

            ///////////////////////////////////////////////////
            if (data.Mob != null && data.Mob.Count > 0)
            {
                foreach (XMobUnitData m in data.Mob)
                {
                    if (m.TemplateID <= 0)
                    {
                        m.LifewithinSkill = false;
                        m.Shield = false;
                        m.Offset_At_X = m.Offset_At_Y = m.Offset_At_Z = 0;
                    }
                }
            }
            else
                data.Mob = null;

            ///////////////////////////////////////////////////
            if (data.Chain != null && data.Chain.TemplateID > 0)
            {
                if (string.IsNullOrEmpty(data.Chain.Name)) data.Chain.Name = null;
            }
            else
                data.Chain = null;
        }
    }
}