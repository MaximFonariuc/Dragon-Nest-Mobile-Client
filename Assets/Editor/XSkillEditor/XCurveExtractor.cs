using System;
using System.Text;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Collections.Generic;
using XUtliPoolLib;

namespace XEditor
{
    class XCurveExtractor : EditorWindow
	{
        [MenuItem(@"XEditor/Generate Curve")]
        static void CurveOpener()
        {
            XCurveGenerator xw = EditorWindow.GetWindow<XCurveGenerator>(@"XCurve Generator", typeof(XCurveExtractor));
            EditorWindow.GetWindow<XCurveExtractor>(@"y-Rotation Extractor", typeof(XCurveGenerator));

            xw.Focus();
        }

        private GUIStyle _labelstyle = null;
        private AnimationClip _clip = null;
        private AnimationClipCurveData[] _data = null;

        void OnGUI()
        {
            if (_labelstyle == null)
            {
                _labelstyle = new GUIStyle(EditorStyles.boldLabel);
                _labelstyle.fontSize = 13;
            }

            GUILayout.Label(@"Extract a Curve from:", _labelstyle);

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            _clip = EditorGUILayout.ObjectField("Animation Clip", _clip, typeof(AnimationClip), true) as AnimationClip;
            if (EditorGUI.EndChangeCheck())
            {
                if (_clip != null)
                {
                    _data = AnimationUtility.GetAllCurves(_clip, true);
                    //_checks = new bool[_data.Length];
                }
                else
                    _data = null;
            }
            EditorGUILayout.EndHorizontal();

            /*EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (_data != null && _data.Length > 0)
            {
                for (int i = 0; i < _data.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        if (i != 0) EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }
                    _checks[i] = EditorGUILayout.ToggleLeft(_data[i].propertyName, _checks[i]);
                }
                EditorGUILayout.EndHorizontal();
            }*/

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(120));
            if (GUILayout.Button("Generate"))
            {
                if (_data != null)
                {
                    AnimationCurve curve = new AnimationCurve();
                    List<Keyframe> frames = new List<Keyframe>();

                    for (int i = 0; i < _data[3].curve.length; i++)
                    {
                        Vector3 y = new Quaternion(
                            _data[3].curve.Evaluate(_data[3].curve.keys[i].time),
                            _data[4].curve.Evaluate(_data[4].curve.keys[i].time),
                            _data[5].curve.Evaluate(_data[5].curve.keys[i].time),
                            _data[6].curve.Evaluate(_data[6].curve.keys[i].time)).eulerAngles;

                        Keyframe frame = new Keyframe(_data[3].curve.keys[i].time, y.y, 0, 0);
                        frames.Add(frame);
                    }

                    curve.keys = frames.ToArray();

                    for (int i = 0; i < curve.keys.Length; ++i)
                    {
                        curve.SmoothTangents(i, 0); //zero weight means average
                    }

                    CreateCurvePrefab(_clip.name, curve);
                    AssetDatabase.Refresh();
                }
            }
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(120));
            EditorGUILayout.EndHorizontal();
        }

        UnityEngine.Object CreateCurvePrefab(string name, AnimationCurve curve)
        {
            string path = XEditorPath.GetPath("Curve/Auto_Camera");
            string fullname = null;

            UnityEngine.Object prefab = null;

            fullname = path + name + ".prefab";

            prefab = PrefabUtility.CreateEmptyPrefab(fullname);

            EditorGUIUtility.PingObject(prefab);

            GameObject go = new GameObject(name);
            XCurve xcurve = go.AddComponent<XCurve>();
            xcurve.Curve = curve;
            PrefabUtility.ReplacePrefab(go, prefab);
            XServerCurveGenerator.GenerateCurve(go, fullname);
            DestroyImmediate(go);

            return prefab;
        }
	}
}
