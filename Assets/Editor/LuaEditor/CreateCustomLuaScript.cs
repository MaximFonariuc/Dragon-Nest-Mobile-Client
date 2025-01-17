//
//LuaMaker.cs
// Created by huailiang.peng on 2016/03/14 10:55:47
//

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UnityEditor.ProjectWindowCallback;
using System.Text.RegularExpressions;
using System.Text;

public class CreateCustomLuaScript : MonoBehaviour
{

    public static string CUSTOM_LUA_FILE = Application.dataPath + "/Editor/81-Script-Hotfix.lua.txt";


    [MenuItem("Assets/Create/Lua Script/Hotfix", false, 80)]
    public static void MakeHotfixLua()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
       ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
       GetSelectedPathOrFallback() + "/HotfixDlg.lua.txt",
       null,
      "Assets/Editor/LuaEditor/81-Script-Hotfix.lua.txt");
    }


    [MenuItem("Assets/Create/Lua Script/LuaDlg", false, 80)]
    public static void MakeLuaViewLua()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
       ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
       GetSelectedPathOrFallback() + "/LuaDlg.lua.txt",
       null,
      "Assets/Editor/LuaEditor/81-Script-LuaView.lua.txt");
    }

    [MenuItem("Assets/Create/Lua Script/LuaParse", false, 80)]
    public static void MakeLuaParse()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
       ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
       GetSelectedPathOrFallback() + "/LuaParseName.lua.txt",
       null,
      "Assets/Editor/LuaEditor/81-Script-LuaParse.lua.txt");
    }


    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

}


class MyDoCreateScriptAsset : EndNameEditAction
{


    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
        ProjectWindowUtil.ShowCreatedAsset(o);
    }

    internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
    {
      
        string fullPath = Path.GetFullPath(pathName);
        StreamReader streamReader = new StreamReader(resourceFile);
        string text = streamReader.ReadToEnd();
        streamReader.Close();
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
        string fileNameWithoutHead = fileNameWithoutExtension;
        if (fileNameWithoutHead.StartsWith("Hotfix")) fileNameWithoutHead = fileNameWithoutHead.Substring(6);
        Debug.Log("file name: "+fileNameWithoutHead);
        if (fileNameWithoutHead.EndsWith(".lua")) fileNameWithoutHead = fileNameWithoutHead.Substring(0, fileNameWithoutHead.Length - 4);
         text = text.Replace("#AuthorName#", System.Environment.UserName).Replace("#CreateTime#",
            System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")).Replace("#SCRIPTNAME#", fileNameWithoutExtension).Replace("#SCRIPTTITLE#",fileNameWithoutHead);
        bool encoderShouldEmitUTF8Identifier = false;
        bool throwOnInvalidBytes = false;
        UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
        bool append = false;
        StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
        streamWriter.Write(text);
        streamWriter.Close();
        AssetDatabase.ImportAsset(pathName);
        return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
    }

}