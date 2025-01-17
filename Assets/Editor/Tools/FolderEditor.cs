using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;

[CustomEditor(typeof(UnityEngine.Object))]
public class FolderEditor : Editor
{
    enum EFolderType
    {
        ETxtTable,
        EOther
    }
    private EFolderType folderType = EFolderType.EOther;
    public void OnEnable()
    {
        string n = target.ToString();
        if (n == "Table (UnityEngine.DefaultAsset)")
        {
            string path = AssetDatabase.GetAssetPath(target);
            if (path=="Assets/Table")
            {
                folderType = EFolderType.ETxtTable;
            }
        }

    }

    public override void OnInspectorGUI()
    {
        switch (folderType)
        {
            case EFolderType.ETxtTable:
                {
                    GUI.enabled = true;
                    if (GUILayout.Button("AllTable2Bytes", GUILayout.Width(200)))
                    {
                        XEditor.AssetModify.AllTable2Bytes();
                    }
                }
                break;
            case EFolderType.EOther:
                GUI.enabled = false;
                base.OnInspectorGUI();
                break;
        }
    }

}
