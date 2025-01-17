using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XUtliPoolLib;

public class TurnTexture2ListWindow : EditorWindow
{

    [MenuItem("Tools/UI/TurnPrefabTexture2List")]
    public static void Execute()
    {
        TurnTexture2ListWindow window = (TurnTexture2ListWindow)EditorWindow.GetWindow(typeof(TurnTexture2ListWindow));
        window.minSize = new Vector2(450, 500);
    }

    Dictionary<string, string> dict = new Dictionary<string, string>();
    
    void OnGUI()
    {
        if (GUILayout.Button("Run"))
        {
            if (Selection.objects.Length == 0)
            {
                Debug.LogError("请只少选择一个贴图");
            }
            else
            {
                dict.Clear();
                for(int i = 0; i < Selection.objects.Length; i++)
                {
                    if(!Selection.objects[i].name.EndsWith("Split"))
                    {
                        Debug.LogError("请仅选择拆分图.");
                        return;
                    }
                    string oldName = Selection.objects[i].name.Substring(0, Selection.objects[i].name.LastIndexOf("_"));
                    dict.Add(oldName, Selection.objects[i].name);
                }
                FindPrefab();
            }
        }
    }
    
    public void FindPrefab()
    {
        string[] allGuids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets" });
        //Debug.Log("allGuids : " + allGuids.Length);
        //DirectoryInfo direction = new DirectoryInfo("Assets/Resources/Effects/");
        //FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
        int total = 0;
        foreach (string guid in allGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            //string[] names = AssetDatabase.GetDependencies(new string[] { assetPath });  //依赖的东东
            GameObject o = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            GameObject go = PrefabUtility.InstantiatePrefab(o) as GameObject;

            UITexture[] list = go.GetComponentsInChildren<UITexture>(true);
            bool change = false;
            for(int i = 0; i < list.Length; i++)
            {
                if (!string.IsNullOrEmpty(list[i].texPath))
                {
                    string safe = Safe(list[i].texPath);
                    if (!string.IsNullOrEmpty(safe))
                    {
                        string texName = dict[safe];
                        list[i].texPath = list[i].texPath.Replace(safe, texName);
                        UITexture tex = list[i].transform.GetComponent<UITexture>();
                        if (tex == null)
                        {
                            Debug.LogError(go.name + "have xuitexture but haven't ui.");
                            continue;
                        }
                        tex.mtexType = UITexture.GetTextureListType(list[i].texPath);
                        change = true;
                    }
                }
            }

            //XUITexture[] list2 = go.GetComponentsInChildren<XUITexture>(true);
            //for(int i = 0; i < list2.Length; i++)
            //{
            //    if (!string.IsNullOrEmpty(list2[i].TexturePath))
            //    {
            //        string safe = Safe(list2[i].TexturePath);
            //        if (!string.IsNullOrEmpty(safe))
            //        {
            //            string texName = dict[safe];
            //            list2[i].TexturePath = list2[i].TexturePath.Replace(safe, texName);
            //            UITexture tex = list[i].transform.GetComponent<UITexture>();
            //            if (tex == null)
            //            {
            //                Debug.LogError(go.name + "have xuitexture but haven't ui.");
            //                continue;
            //            }
            //            tex.mType = UITexture.TextureType.List;
            //            change = true;
            //        }
            //    }
            //}
            if (change)
            {
                Debug.Log(go.name);
                Object prefab = (o == null) ? PrefabUtility.CreateEmptyPrefab(AssetDatabase.GetAssetPath(o)) : (o as GameObject);
                PrefabUtility.ReplacePrefab(go, prefab);
            }
            DestroyImmediate(go);
            total++;
            ShowProgress((float)total / (float)allGuids.Length, allGuids.Length, total);
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        EditorUtility.ClearProgressBar();
        Debug.Log("finish.");
    }
    public static void ShowProgress(float val, int total, int cur)
    {
        EditorUtility.DisplayProgressBar("Searching", string.Format("Finding ({0}/{1}), please wait...", cur, total), val);
    }

    public string Safe(string str)
    {
        foreach (var part in dict)
        {
            if(str.Contains(part.Key))
            {
                if(!str.Contains(part.Value))
                {
                    return part.Key;
                }
            }
        }
        return "";
    }
}
