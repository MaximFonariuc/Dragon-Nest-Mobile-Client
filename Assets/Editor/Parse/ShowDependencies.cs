using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;

public class ShowDependencies : EditorWindow
{
    private Vector2 scrollPos;
    private  static bool m_showDependencies = true;
    //[MenuItem("Assets/Show Dependencies")]
    //public static void Init()
    //{
    //    m_showDependencies = true;
    //    GetWindow<ShowDependencies>();
    //}
    //[MenuItem("Assets/Show Hierarchy")]
    //public static void ShowDeepHierarchyDeep()
    //{
    //    m_showDependencies = false;
    //    GetWindow<ShowDependencies>();
    //}


    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.BeginVertical();
        if (m_showDependencies)
            DrawDependencies();
        else
            DrawDeepHierarchy();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private void DrawDeepHierarchy()
    {
        GUILayout.Label("DeepHierarchy:");
        UnityEngine.Object[] dependencies = EditorUtility.CollectDeepHierarchy(new[] { Selection.activeObject });
        StringBuilder sb = new StringBuilder();
        foreach (var obj in dependencies)
        {
            //GUILayout.Label(AssetDatabase.GetAssetPath(obj) + "  :  " + obj);
            string path = AssetDatabase.GetAssetPath(obj) + ":"+obj.name;
            sb.AppendLine(path);
            Debug.Log(path);
        }
        GUILayout.Label("Hirrarchy："+dependencies.Length);
        SaveDependenceisInfos(sb.ToString());
    }

    private void DrawDependencies()
    {
        GUILayout.Label("Dependencies:");
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        string[] dependencies =AssetDatabase.GetDependencies(new string[] { path }).Where(x=>x.EndsWith(".cs")==false).ToArray();
        StringBuilder fbx = new StringBuilder();
        StringBuilder texture = new StringBuilder();
        StringBuilder prefab = new StringBuilder();
        StringBuilder other = new StringBuilder();
        foreach (var obj in dependencies)
        {
            string extension = Path.GetExtension(obj);
            switch (extension) { 
                case ".fbx":
                case ".FBX":
                    fbx.AppendLine(obj);
                    break;
                case ".tga":
                case ".jpg":
                case ".png":
                case ".TGA":
                case ".JPG":
                case ".PNG":
                    texture.AppendLine(obj);
                    break;
                case ".prefab":
                    prefab.AppendLine(obj);
                    break;
                default:
                    other.AppendLine(obj);
                    break;
            }
        }

        string doc = "fbx:"+fbx.Length  +"\n"  + fbx.ToString();
        doc += "texture: " + texture.Length + "\n" + texture.ToString();
        doc += "prefab:" + prefab.Length + " \n" + prefab.ToString();
        doc += "other:" + other.Length + "\n" + other.ToString();
        GUILayout.Label("Dependencies：" + dependencies.Length);
        SaveDependenceisInfos(doc.ToString());
        
    }

    private void SaveDependenceisInfos(string doc)
    {
        StreamWriter sw = File.CreateText("d://dependencesInfo.txt");
        sw.Write(doc);
        sw.Flush();
        sw.Close();
    }
}
