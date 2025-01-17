using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(PositionGroup))]

public class PositionGroupEditor : Editor
{
    PositionGroup pg;
    List<string> strC = new List<string>();
    List<Vector3> ve3C = new List<Vector3>();
    public override void OnInspectorGUI()
    {
        pg = target as PositionGroup;

        GUILayout.Space(6f);

        strC.Clear();
        ve3C.Clear();
        for (int i = 0; i < pg.pos.Count; i++)
        {
            GUILayout.BeginHorizontal();
            NGUIEditorTools.SetLabelWidth(60f);
            string str = EditorGUILayout.TextField("Group " + i.ToString(), pg.desc[i], GUILayout.Width(150f));
            strC.Add(str);
            NGUIEditorTools.SetLabelWidth(30f);
            GUILayout.Space(50f);
            Vector3 v3 = EditorGUILayout.Vector3Field("Pos", pg.pos[i], GUILayout.Width(200f));
            ve3C.Add(v3);
            if (GUILayout.Button("sign", GUILayout.MaxWidth(50)))
            {
                ve3C[i] = pg.gameObject.transform.localPosition;
                GUI.changed = true;
            }
            if (GUILayout.Button("set", GUILayout.MaxWidth(50)))
            {
                pg.gameObject.transform.localPosition = pg.pos[i];
                GUI.changed = false;
            }
            if (GUILayout.Button("—", GUILayout.MaxWidth(30)))
            {
                pg.ChangeList(i);
                GUI.changed = false;
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add", GUILayout.MaxWidth(200)))
        {
            pg.ChangeList(-1);
        }

        if (GUI.changed)
        {
            for(int i = 0; i < strC.Count; i++)
            {
                pg.desc[i] = strC[i];
                pg.pos[i] = ve3C[i];
            }
        }
    }
}