using UnityEditor;
using UnityEngine;
using System.Collections;
using XEditor;

public class LightMapHelper : EditorWindow
{
    [MenuItem(@"XEditor/Tools/LightMapHelper")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(LightMapHelper));
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Bake", GUILayout.MaxWidth(150)))
        {
            Bake();
        }
    }

    public void Bake()
    {
        LightmapEditorSettings.maxAtlasHeight = 512;
        LightmapEditorSettings.maxAtlasSize = 512;
        Lightmapping.Clear();
        Lightmapping.Bake();
    }
}
