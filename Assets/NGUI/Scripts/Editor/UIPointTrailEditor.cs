using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIPointTrail), true)]
public class UIPointTrailEditor: UIWidgetInspector
{
    protected override void DrawCustomProperties ()
    {
        GUILayout.BeginHorizontal();

        NGUIEditorTools.DrawProperty("material", serializedObject, "mMat", GUILayout.Width(300f));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        NGUIEditorTools.DrawProperty("time", serializedObject, "mPointTime", GUILayout.Width(300f));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        NGUIEditorTools.DrawProperty("max size", serializedObject, "mMaxSize", GUILayout.Width(300f));
        GUILayout.EndHorizontal();

        GUILayout.Space(4f);
        base.DrawCustomProperties();
    }
}

