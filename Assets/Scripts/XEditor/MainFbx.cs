#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class MainFbx : MonoBehaviour
{
    public GameObject fbx;

    public static void _MakeMainFbx(GameObject fbx)
    {
        string fbxPath = AssetDatabase.GetAssetPath(fbx).ToLower();
        if(fbxPath.EndsWith("_bandpose.fbx"))
        {
            int index = fbxPath.LastIndexOf("/");
            if (index >= 0)
            {
                string dir = fbxPath.Substring(0, index);
                index = dir.LastIndexOf("/");
                if (index >= 0)
                {
                    string dirname = dir.Substring(index + 1);
                    GameObject mainFbx = new GameObject(dirname);
                    MainFbx mf = mainFbx.AddComponent<MainFbx>();
                    mf.fbx = fbx;
                    string prefabPath = dir + "/" + dirname + ".prefab";
                    XEditor.AssetModify.CreateOrReplacePrefab(mainFbx, prefabPath);
                    GameObject.DestroyImmediate(mainFbx);
                }
            }
        }
        
    }

    [MenuItem(@"Assets/Tool/Fbx/MakeMainFbx")]
    public static void MakeMainFbx()
    {
        GameObject[] fbxs = Selection.gameObjects;
        if (fbxs.Length == 1)
        {
            _MakeMainFbx(fbxs[0]);
        }
    }

    [MenuItem(@"Assets/Tool/Fbx/RefreshMainFbx")]
    private static void RefreshMainFbx()
    {
        DirectoryInfo di = new DirectoryInfo("Assets/Creatures");
        DirectoryInfo[] subDirs = di.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
        foreach (DirectoryInfo subDir in subDirs)
        {
            FileInfo[] files = subDir.GetFiles("*_bandpose.fbx", SearchOption.TopDirectoryOnly);
            if (files.Length == 1)
            {
                string path = files[0].FullName.Replace("\\", "/");
                int index = path.IndexOf("Assets/Creatures");
                path = path.Substring(index);
                GameObject fbx = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                _MakeMainFbx(fbx);
            }
        }
        AssetDatabase.Refresh();
    }
}
#endif