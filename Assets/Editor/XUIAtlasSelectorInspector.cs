using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(XUIAtlasSelector))]
public class XUIAtlasSelectorInspector : Editor
{
    private bool ctrlPressed = false;
    private XUIAtlasSelector _selector;
    private GUIStyle _labelstyle = null;
    private GUILayoutOption[] _line = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) };
    public override void OnInspectorGUI()
    {
 
        _selector = target as XUIAtlasSelector;
        if (_labelstyle == null)
        {
            _labelstyle = new GUIStyle(EditorStyles.boldLabel);
            _labelstyle.fontSize = 13;
        }
        ctrlPressed = Event.current.control || Event.current.command;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.IntField("UIWidget Size:",_selector.widgetSize);
        EditorGUILayout.EndHorizontal();
        foreach(KeyValuePair<string, XUIAtlasSelector.AtlasInfo> value in _selector.atlasInfos)
       
        {
            XUIAtlasSelector.AtlasInfo info = value.Value ;
            GUILayout.Box("", _line);
            EditorGUILayout.BeginHorizontal();        
            GUILayout.Label(info.atlasName+"("+ info.list.Count+" )",_labelstyle);
            info.startDepth = EditorGUILayout.IntField(" startDepth:", info.startDepth);
            if (GUILayout.Button("execute"))
            {
                for(int j = 0,size = info.list.Count;j < size; j++)
                {
                    UIWidget widget = info.list[j];
                    widget.depth = info.startDepth + j;
                }
            }
            EditorGUILayout.EndHorizontal();
            for (int j = 0,size = info.list.Count;j < size; j++)
            {
                UIWidget widget = info.list[j];
               
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(widget.name))
                {
                    SelectObject(widget, ctrlPressed);
                }
                widget.depth = EditorGUILayout.IntField("depth:", widget.depth);
                EditorGUILayout.EndHorizontal();
            }
        }
       
    }

    void SelectObject(Object selectedObject, bool append)
    {
        if (append)
        {
            List<Object> currentSelection = new List<Object>(Selection.objects);
            if (currentSelection.Contains(selectedObject)) currentSelection.Remove(selectedObject);
            else currentSelection.Add(selectedObject);

            Selection.objects = currentSelection.ToArray();
        }
        else Selection.activeObject = selectedObject;
    }

}