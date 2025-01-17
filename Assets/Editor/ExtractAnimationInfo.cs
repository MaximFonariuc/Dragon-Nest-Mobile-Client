using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using XUtliPoolLib;
using XEditor;

public class ExtractAnimationInfo : MonoBehaviour {

    [MenuItem("Assets/ExtractAnimationInfo")]
    static void ExtractAnimationInfoFromClip()
    {
        UnityEngine.Object[] imported = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        string prefix = "Assets/Resources/";
        int prefixlength = prefix.Length;

        Dictionary<string, float> v = new Dictionary<string, float>();

        foreach (UnityEngine.Object o in imported)
        {
            AnimationClip ac = o as AnimationClip;

            if(ac == null) continue;

            float f = ac.length;

            string s = AssetDatabase.GetAssetPath(o);

            //Debug.Log(s.Substring(prefixlength));
            //Debug.Log(f);

            v.Add(s, f);
        }

        if (v.Count > 0)
        {
            //StreamWriter sw = File.CreateText("E:\\DragonNest\\res\\XProject\\Assets\\animlength.txt");

            StreamWriter sw = File.CreateText("Assets\\Resources\\Table\\animlength.txt");
            sw.WriteLine("name\tlength\thashvalue");
            sw.WriteLine("name\tlength\thashvalue");

            foreach (KeyValuePair<string, float> r in v)
            {
                int l = r.Key.LastIndexOf("/");
                int l2 = r.Key.LastIndexOf(".");
                string s = r.Key.Substring(l+1, l2 - l - 1);
                sw.WriteLine(s + "\t" + r.Value + "\t" + XCommon.singleton.XHash(s));
            }
            
            sw.Flush();
            sw.Close();

            AssetDatabase.Refresh();
        }
    }
}
