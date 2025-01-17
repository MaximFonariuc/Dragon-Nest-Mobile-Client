using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class XExtractMultiLan : EditorWindow
{
    //private static HashSet<string> KeyStrings = new HashSet<string>();

    private static Dictionary<string, HashSet<string> > KeyStrings = new Dictionary<string, HashSet<string>>();

    private static string LocalizeFile = "./Assets/Resources/Table/Local/Localization.txt";

    public static readonly char[] sep = { '\t' };
    public static readonly char[] eof = { '\r', '\n' };

    //[MenuItem(@"XEditor/Tools/AppendLocalize")]
    //[MenuItem("Assets/XExtractMultiLan")]
    //private static void Execute()
    //{
    //    DoWork();
    //}

    private static void DoWork()
    {
        KeyStrings.Clear();

        foreach (Object o in Selection.GetFiltered(typeof (Object), SelectionMode.DeepAssets))
        {
            if (AssetDatabase.GetAssetPath(o).Contains(".prefab"))
            {
                GameObject go = PrefabUtility.InstantiatePrefab(o) as GameObject;

                AppendLocalize(AssetDatabase.GetAssetPath(o), go, o);
            }

            if (AssetDatabase.GetAssetPath(o).Contains(".txt"))
            {
                string path = AssetDatabase.GetAssetPath(o);
                if(path.IndexOf("Localization") >= 0) continue;

                if (path.IndexOf("StringTable") >= 0) continue;

                if (path.IndexOf("GlobalConfig") >= 0) continue;

                ExtractChineseFromTable(path);
            }
        }
        
        SaveLocalizeFile();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void AppendLocalize(string name, GameObject go, Object o)
    {
        HashSet<string> StringSet = new HashSet<string>();

        UILabel[] labels = go.GetComponentsInChildren<UILabel>(true);
        foreach (UILabel uiLabel in labels)
        {
            string text = uiLabel.text;
            if (ContainsChinese(text))
            {
                StringSet.Add(text);
            }
        }

        UIInput[] inputs = go.GetComponentsInChildren<UIInput>(true);
        foreach (UIInput uiInput in inputs)
        {
            string text = uiInput.defaultText;
            if (ContainsChinese(text))
            {
                StringSet.Add(text);
            }
        }

        if(StringSet.Count > 0)
            KeyStrings.Add(name, StringSet);

        DestroyImmediate(go);
    }

    private static void ExtractChineseFromTable(string path)
    {
        int start = ("Assets/Resources/").Length;
        int l = path.Length - start - (".txt").Length;
        string strPath = path.Substring(start, l);
        TextAsset data = Resources.Load(strPath, typeof (TextAsset)) as TextAsset;
        Stream s = new System.IO.MemoryStream(data.bytes);
        StreamReader reader = new StreamReader(s);

        string line;
        string[] cols;

        reader.ReadLine();
        reader.ReadLine();

        HashSet<string> StringSet = new HashSet<string>();

        while (true)
        {
            line = reader.ReadLine();
            if (line == null)
                break;

            line = line.TrimEnd(eof);
            cols = line.Split(sep);

            for (int i = 0; i < cols.Length; i++)
            {
                if(ContainsChinese(cols[i]))
                    StringSet.Add(cols[i]);
            }
        }

        if (StringSet.Count > 0)
            KeyStrings.Add(path, StringSet);

    }

    private static bool ContainsChinese(string str)
    {
        int l = str.Length;

        for (int i = 0; i < l; i++)
        {
            char c = str[i];

            int v = Convert.ToInt32(c);

            if (v < 0 || v > 255) return true;
        }

        return false;
    }

    private static void SaveLocalizeFile()
    {
        StreamWriter sw = File.CreateText(LocalizeFile);
        sw.WriteLine("KEY,Language");

        foreach (KeyValuePair<string, HashSet<string>> keyValuePair in KeyStrings)
        {
            sw.WriteLine("--" + keyValuePair.Key);

            foreach (string s in keyValuePair.Value)
            {
                sw.WriteLine(s + ",");
            }
        }

        sw.Flush();
        sw.Close();
    }
}
