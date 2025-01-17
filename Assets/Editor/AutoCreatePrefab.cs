using UnityEditor;
using UnityEngine;
using System.Collections;

public class AutoCreatePrefab
{

    [MenuItem("Assets/CreatePrefab")]
    public static void CreatePrefabFromFBX()
    {
        Object[] imported = Selection.GetFiltered(typeof(object), SelectionMode.Unfiltered | SelectionMode.Deep);

        if (imported.Length > 0)
        {

            for (int i = 0; i < imported.Length; i++)
            {
                string importedPath = AssetDatabase.GetAssetPath(imported[i]);

                if (importedPath != null)
                {
                    int lastsep = importedPath.LastIndexOf("/");

                    string targetPath = importedPath.Substring(0, lastsep) + "/prefabs/";
                    string name = imported[i].name;

                    GameObject go = imported[i] as GameObject;
                    
                    Object prefab = PrefabUtility.CreateEmptyPrefab(targetPath + name + ".prefab");
                    PrefabUtility.ReplacePrefab(go, prefab);
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}

