using UnityEngine;
using System.Collections;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TextAsset))]
public class BytesTableEditor : Editor
{
    enum ETableType
    {
        ETxtTable,
        EBytesTable,
        ELua,
        EOther
    }
    private ETableType tableType = ETableType.EOther;
    public void OnEnable()
    {
        string path = AssetDatabase.GetAssetPath(target);
        if(path.StartsWith("Assets/Resources/Table/") && path.EndsWith(".bytes"))
        {
            tableType = ETableType.EBytesTable;
        }
        else if(path.StartsWith("Assets/Table/") && path.EndsWith(".txt"))
        {
            tableType = ETableType.ETxtTable;
        }
        else if(path.StartsWith("Assets/Resources/lua") && path.EndsWith(".txt"))
        {
            tableType = ETableType.ELua;
        }
    }

    public override void OnInspectorGUI()
    {        
        switch(tableType)
        {
            case ETableType.ETxtTable:
                {
                    GUI.enabled = true;
                    if (GUILayout.Button("Table2Bytes", GUILayout.Width(80)))
                    {
                        if (targets == null || targets.Length == 1)
                        {
                            XEditor.AssetModify.Table2Bytes(target);
                        }
                        else
                        {
                            XEditor.AssetModify.Table2Bytes(targets);
                        }
                    }
                    //if (GUILayout.Button("Table2Bytes", GUILayout.Width(80)))
                    //{
                    //    if (targets == null || targets.Length == 1)
                    //    {
                    //        XEditor.AssetModify.Table2Bytes(target);
                    //    }
                    //}
                }
                break;
            case ETableType.EBytesTable:
                {
                    GUI.enabled = true;
                    if (GUILayout.Button("Open", GUILayout.Width(80)))
                    {
                        XEditor.BytesTableViewEditor window = (XEditor.BytesTableViewEditor)EditorWindow.GetWindow(typeof(XEditor.BytesTableViewEditor), true, "BytesTableViewEditor");
                        window.Init(target);
                        window.Show();
                    }
                }
                break;
            case ETableType.ELua:
                {
                    GUI.enabled = true;
                    GUILayout.Label((target as TextAsset).text);
                }
                break;
            case ETableType.EOther:
                GUI.enabled = false;
                base.OnInspectorGUI();
                break;
        }
    }
}
