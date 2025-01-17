//
// ModifyOriginLuaScript.cs
// Created by huailiang.peng on 2016/03/17 10:16:39
//
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;
using System;

public class HotfixToolEditor : MonoBehaviour
{

    private static string orig_lua_path = Application.dataPath + "/Hotfix/";

    private static string res_lua_path = Application.dataPath + "/Resources/lua/Hotfix/";

    [MenuItem("LuaTools/Hotfix/DeployLocalization")]
    public static void DeployLocal()
    {
        TransNoBom();
        EncrypLua();
        CopyFieTemporaryCacheWithoutTip();
        DecryptLua();
    }

    [MenuItem("LuaTools/Hotfix/TransNobomUTF8")]
    public static void TransNoBom()
    {
        string[] files = Directory.GetFiles(orig_lua_path);
        foreach (string f in files)
        {
            string file = f.Replace('\\', '/');

            string content = File.ReadAllText(file);
            using (var sw = new StreamWriter(file, false, new UTF8Encoding(false)))
            {
                sw.Write(content);
            }
            Debug.Log("Encode file::>>" + file + " OK!");
        }
    }

    [MenuItem("LuaTools/Hotfix/Hotfix->.lua")]
    public static void HotfixToLua()
    {
        string[] files = Directory.GetFiles(res_lua_path);
        foreach (var item in files)
        {
            FileInfo fi = new FileInfo(item);
            if (fi.Extension.Equals(".txt"))
            {
                string mofile = fi.FullName.Replace(".lua.txt", ".lua");
                if (!File.Exists(mofile)) fi.MoveTo(mofile);
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("LuaTools/Hotfix/Hotfix->.txt")]
    public static void HotfixToTxt()
    {
        string[] files = Directory.GetFiles(res_lua_path);
        foreach (var item in files)
        {
            FileInfo fi = new FileInfo(item);
            if (fi.Extension.Equals(".lua"))
            {
                string mofile = fi.FullName.Replace(".lua", ".lua.txt");
                if (!File.Exists(mofile)) fi.MoveTo(mofile);
            }
        }
        AssetDatabase.Refresh();
    }



    [MenuItem("LuaTools/Hotfix/DecryptLua")]
    public static void DecryptLua()
    {
        string CipherText = "习伆买伇习伆买伇乀伅习伅丰伀买企习伄习伇习伆习伄买伃丰伃习伈乐伄买伇买伂习伇丰伈买伇习伅习伂习伆丰伇习伄买伃习伇乐伄习伇习伆买伇习伅丰伅习伇习伆";

        byte[] cipherBytes = UnicodeEncoding.BigEndianUnicode.GetBytes(CipherText);
        int cipherBytesCount = cipherBytes.Length;

        byte[] proclaimBytes = new byte[cipherBytesCount / 2];

        for (int i = 0; i < cipherBytesCount; i += 4)
        {
            byte cipherByte1 = cipherBytes[i + 1];
            byte cipherByte2 = cipherBytes[i + 3];

            int lowerByte = (cipherByte1 & 0x0f) << 4;
            int upperByte = cipherByte1 & 0xf0;

            int lowerByte2 = cipherByte2 & 0x0f;
            int upperByte2 = (cipherByte2 & 0xf0) >> 4;

            proclaimBytes[i / 2] = Convert.ToByte(lowerByte | upperByte2);
            proclaimBytes[i / 2 + 1] = Convert.ToByte(upperByte | lowerByte2);
        }
        print(UnicodeEncoding.BigEndianUnicode.GetString(proclaimBytes, 0, proclaimBytes.Length));
    }

    [MenuItem("LuaTools/Hotfix/EncrypLua")]
    public static void EncrypLua()
    {
        string proclaimText = "fwfwEe0qdgfds3hTwrg8webf7dsgTgfwe5gf";

        byte[] proclaimBytes = UnicodeEncoding.BigEndianUnicode.GetBytes(proclaimText);
        int proclaimBytesCount = proclaimBytes.Length;
        byte[] cipherBytes = new byte[proclaimBytesCount * 2];

        for (int i = 0; i < proclaimBytesCount; i += 2)
        {
            byte proclaimByte = proclaimBytes[i];
            int upperByte = proclaimByte & 0xf0;
            int lowerByte = proclaimByte & 0x0f;
            upperByte = upperByte >> 4;
            lowerByte = lowerByte << 4;

            byte proclaimByte2 = proclaimBytes[i + 1];
            int upperByte2 = proclaimByte2 & 0xf0;
            int lowerByte2 = proclaimByte2 & 0x0f;
            upperByte2 |= upperByte;
            lowerByte2 |= lowerByte;

            cipherBytes[i * 2] = 0x4e;
            cipherBytes[i * 2 + 1] = Convert.ToByte(upperByte2);
            cipherBytes[(i + 1) * 2] = 0x4f;
            cipherBytes[(i + 1) * 2 + 1] = Convert.ToByte(lowerByte2);
        }
        Debug.Log(UnicodeEncoding.BigEndianUnicode.GetString(cipherBytes, 0, cipherBytes.Length));
    }  


    [MenuItem("LuaTools/Hotfix/TransPostfixLua")]
    public static void TransPostfixLua()
    {
        TransNoBom();
        string[] files = Directory.GetFiles(orig_lua_path);
        foreach (var item in files)
        {
            FileInfo fi = new FileInfo(item);
            if (fi.Extension.Equals(".txt"))
            {
                string mofile = fi.FullName.Replace(".txt", ".lua");
                if (!File.Exists(mofile)) fi.MoveTo(mofile);
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("Trans Postfix Lua Finish!");
    }


    [MenuItem("LuaTools/Hotfix/TransPostfixTxt")]
    public static void TransPostfixTxt()
    {
        TransNoBom();
        string[] files = Directory.GetFiles(orig_lua_path);
        foreach (var item in files)
        {
            FileInfo fi = new FileInfo(item);
            if (fi.Extension.Equals(".lua"))
            {
                string mofile = fi.FullName.Replace(".lua", ".txt");
                if (!File.Exists(mofile)) fi.MoveTo(mofile);
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("Trans Postfix txt Finish!");
    }

    [MenuItem("LuaTools/Hotfix/CopyHotfixToTemporaryCache")]
    public static void CopyFieTemporaryCache()
    {
        CopyFieTemporaryCacheWithoutTip();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("提示", "拷贝完成", "提示");
        HelperEditor.OpenHotfix();
    }

    public static void CopyFieTemporaryCacheWithoutTip()
    {
        TransPostfixLua();
        string path = Application.temporaryCachePath + "/Hotfix/";
        if (Directory.Exists(path))
        {
            Debug.Log("delete path: " + path);
            Directory.Delete(path, true);
        }
        Directory.CreateDirectory(path);
        string[] files = Directory.GetFiles(orig_lua_path);
        foreach (var item in files)
        {
            FileInfo finfo = new FileInfo(item);
            if (!finfo.Extension.Equals(".meta"))
            {
                finfo.CopyTo(path + finfo.Name);
            }
        }
        Debug.Log("copy file finish  path: " + path);
    }



    [MenuItem("LuaTools/Liblua/ULua -> Resources")]
    public static void CopyLuaResources()
    {
        string sour_path = Application.dataPath + "/uLua/lua/";
        string dest_path = Application.dataPath + "/Resources/lua/";
        if (Directory.Exists(dest_path))
        {
            Directory.Delete(dest_path, true);
        }
        Directory.Move(sour_path, dest_path);
        TransFilesPostfix(dest_path, false);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("提示", "拷贝完成", "提示");
    }

    [MenuItem("LuaTools/Liblua/Resources -> ULua")]
    public static void CopyResourcesToULua()
    {
        string sour_path = Application.dataPath + "/Resources/lua/";
        string dest_path = Application.dataPath + "/uLua/lua/";
        if (Directory.Exists(dest_path))
        {
            Directory.Delete(dest_path, true);
        }
        Directory.Move(sour_path, dest_path);
        TransFilesPostfix(dest_path, true);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("提示", "拷贝完成", "提示");
    }

    [MenuItem("LuaTools/Hotfix/ResetHotfixVersion")]
    public static void ResetHotfixVersion()
    {
        PlayerPrefs.SetInt("hotfixversion", 0);
    }


    public static void MakeHotfixConfig(int version, string path)
    {

        string text = "version:" + version + "\n";
        string[] files = Directory.GetFiles(orig_lua_path);
        foreach (var item in files)
        {
            FileInfo fi = new FileInfo(item);
            if (fi.Extension.Equals(".txt") || fi.Extension.Equals(".lua"))
            {
                text += fi.Name.Substring(0, fi.Name.IndexOf(".")) + ",";
            }
        }
        text = text.Remove(text.Length - 1);
        StreamWriter temp_sw = new StreamWriter(path + "/HotfixConfig",
                                                 false, System.Text.Encoding.UTF8);
        temp_sw.Write(text);
        temp_sw.Close();
        AssetDatabase.Refresh();

        HelperEditor.Open(path);
    }

    public static void TransFilesPostfix(string dir_path, bool tolua)
    {
        file_list.Clear();
        GetFiles(dir_path);
        for (int i = 0; i < file_list.Count; i++)
        {
            FileInfo fi = file_list[i];
            if (!tolua)
            {
                string mofile = fi.FullName.Replace(".lua", ".lua.txt");
                fi.MoveTo(mofile);
            }
            else
            {
                string mofile = fi.FullName.Replace(".lua.txt", ".lua");
                fi.MoveTo(mofile);
            }
        }
    }

    private static List<FileInfo> file_list = new List<FileInfo>();

    public static void GetFiles(string dir_path)
    {
        DirectoryInfo dir = new DirectoryInfo(dir_path);
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i] as FileInfo;
            if (file != null)   //是文件   
            {
                if (file.Extension.Equals(".lua") || file.Extension.Equals(".txt"))
                    file_list.Add(file);
            }
            else   //对于子目录，进行递归调用 　　
                GetFiles(files[i].FullName);
        }
    }

    public static string[] GetHotfixFilesList()
    {
        TransPostfixTxt();
        string[] files = Directory.GetFiles(orig_lua_path);
        List<string> list = new List<string>();
        foreach (var item in files)
        {
            FileInfo finfo = new FileInfo(item);
            if (finfo.Extension.Equals(".txt"))
            {
                int index = finfo.FullName.IndexOf("Asset");
                if (index < finfo.FullName.Length)
                {
                    string fpath = finfo.FullName.Substring(index);
                    list.Add(fpath);
                    Debug.Log("fpath: " + fpath);
                }
            }
        }
        return list.ToArray();
    }

}
