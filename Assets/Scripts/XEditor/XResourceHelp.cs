#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using XUtliPoolLib;
using UnityEditor;

public class XResourceHelp : MonoBehaviour, IResourceHelp
{
    public bool Deprecated
    {
        get;
        set;
    }

    public void CheckResource(UnityEngine.Object o, string path)
    {
#if UNITY_EDITOR
        string p = AssetDatabase.GetAssetPath(o);
        
        string s = "Assets/Resources/";

        if (s.Length < p.Length)
        {
            string t = p.Substring(s.Length);
            if(t.IndexOf(path) != 0)
                XDebug.singleton.AddErrorLog("找海公:" + path);
        }
#endif
    }
	
}
#endif