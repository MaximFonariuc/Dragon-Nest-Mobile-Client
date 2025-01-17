using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FindSomethingReferences
{
    [MenuItem("Assets/Find Something References", false, 10)]
    static private void Find()
    {
        string[] allGuids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets" });
        //Debug.Log("allGuids : " + allGuids.Length);
        //DirectoryInfo direction = new DirectoryInfo("Assets/Resources/Effects/");
        //FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
        int total = 0;
        foreach (string guid in allGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string[] names = AssetDatabase.GetDependencies(new string[] { assetPath });  //依赖的东东
            GameObject go = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            UITexture[] pslist = go.GetComponentsInChildren<UITexture>(true);
            if(pslist.Length != 0)
            {
                Debug.LogError("This Prefab Use Texture:" + go.name);
            }
            total++;
            ShowProgress((float)total / (float)allGuids.Length, allGuids.Length, total);
        }
        EditorUtility.ClearProgressBar();
    }
    public static void ShowProgress(float val, int total, int cur)
    {
        EditorUtility.DisplayProgressBar("Searching", string.Format("Finding ({0}/{1}), please wait...", cur, total), val);
    }
}