using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(UIBloodGrid), true)]
public class UIBloodGridInspector : UIWidgetInspector
{
    UIBloodGrid mBloodGrid;

    protected override void OnEnable()
    {
        base.OnEnable();
        mBloodGrid = target as UIBloodGrid;
    }
    protected override void DrawCustomProperties()
    {
        GUILayout.BeginHorizontal();
        NGUIEditorTools.SetLabelWidth(90f);
        int signalWith = EditorGUILayout.IntField("SignalWidth", mBloodGrid.SignalWidth, GUILayout.Width(130f));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        int signalHeight = EditorGUILayout.IntField("SignalHeight", mBloodGrid.SignalHeight, GUILayout.Width(130f));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        float unitSize = EditorGUILayout.FloatField("UnitSize", mBloodGrid.UnitSize, GUILayout.Width(130f));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        int maxHp = EditorGUILayout.IntField("MAXHP", mBloodGrid.MaxHP, GUILayout.Width(130f));
        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
            mBloodGrid.SignalWidth = signalWith < 0 ? 0 : signalWith;
            mBloodGrid.SignalHeight = signalHeight < 0 ? 0 : signalHeight;
            mBloodGrid.UnitSize = unitSize < 0 ? 0 : unitSize;
            mBloodGrid.SetMAXHP(maxHp);
        }
        base.DrawCustomProperties();
    }
}
