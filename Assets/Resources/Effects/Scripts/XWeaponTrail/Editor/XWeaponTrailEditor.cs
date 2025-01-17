using UnityEngine;
using System.Collections;
using UnityEditor;
using Xft;

[CustomEditor(typeof(XWeaponTrail))]
[CanEditMultipleObjects]
public class XWeaponTrailEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Version: " + XWeaponTrail.Version);
        EditorGUILayout.LabelField("Author: Shallway");
        EditorGUILayout.LabelField("Email: shallwaycn@gmail.com");
        EditorGUILayout.LabelField("Web: http://shallway.net");
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Forum", GUILayout.Width(120), GUILayout.Height(32)))
        {
            Application.OpenURL("http://shallway.net/xffect/forum/categories/x-weapontrail");
        }

        if (GUILayout.Button("Get more effects!", GUILayout.Width(120), GUILayout.Height(32)))
        {
            Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/3810");
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        DrawDefaultInspector();
    }
}

