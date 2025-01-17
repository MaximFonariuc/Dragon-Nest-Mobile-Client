#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

namespace com.tencent.pandora
{
    /// <summary>
    /// 在UnityEditor环境下，从Actions/Resources目录下加载Lua文件
    /// </summary>
    internal class LocalLuaBytesPool
    {
        public static Dictionary<string, byte[]> _luaBytesDict = new Dictionary<string, byte[]>();

        public static void LoadLuaBytesInDirectory(string directoryName)
        {
            string[] paths = Directory.GetFiles("Assets/Actions/Resources/" + directoryName + "/Lua");
            for(int i = 0;i < paths.Length; i++)
            {
                string path = paths[i];
                if(path.Contains(".meta"))
                {
                    continue;
                }
                string name = Path.GetFileName(path);
                name = name.Replace(".bytes", "");
                TextAsset asset = Resources.Load<TextAsset>(directoryName + "/Lua/" + name);
                if(_luaBytesDict.ContainsKey(name) == false)
                {
                    _luaBytesDict.Add(name, asset.bytes);
                }
            }
        }

        public static byte[] GetLuaBytes(string name)
        {
            string luaKey = name + ".lua";
            if(_luaBytesDict.ContainsKey(luaKey) == true)
            {
                return _luaBytesDict[luaKey];
            }
            foreach(string s in _luaBytesDict.Keys)
            {
                if(s.ToLower() == luaKey)
                {
                    return _luaBytesDict[s];
                }
            }
            return null;
        }
    }
}
#endif
