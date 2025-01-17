using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
#endif

namespace com.tencent.pandora
{
    internal class LuaStateManager
    {
        private static LuaSvr _luaSvr;
        private static bool _isInitialized;

        private static HashSet<string> _executingLuaGroupSet = new HashSet<string>();

        /// <summary>
        /// 启动lua虚拟机
        /// </summary>
        public static void Initialize()
        {
            _luaSvr = new LuaSvr();
            _luaSvr.init(null, OnComplete, LuaSvrFlag.LSF_BASIC | LuaSvrFlag.LSF_EXTLIB);
        }

        private static void OnComplete()
        {
#if UNITY_EDITOR
            //LuaState.loaderDelegate = LocalLuaBytesPool.GetLuaBytes;
            LuaState.loaderDelegate = AssetManager.GetLuaBytes;
#else
            LuaState.loaderDelegate = AssetManager.GetLuaBytes;
#endif
            LuaState.errorDelegate = ReportLuaError;
            _isInitialized = true;
            Logger.LogInfo("Lua虚拟机初始化成功");
        }

        private static void ReportLuaError(string error)
        {
            Pandora.Instance.ReportError(error, ErrorCodeConfig.LUA_SCRIPT_EXCEPTION);
            Pandora.Instance.Report(error, ErrorCodeConfig.LUA_SCRIPT_EXCEPTION_DETAIL, ErrorCodeConfig.TNM2_TYPE_LITERALS);
        }

        public static void Reset()
        {
            _isInitialized = false;
            _executingLuaGroupSet.Clear();
            //lua虚拟机尚未初始化的情况下logout时需要判断一下
            if(_luaSvr != null)
            {
                _luaSvr.reset(null, OnComplete, LuaSvrFlag.LSF_BASIC | LuaSvrFlag.LSF_EXTLIB);
            }
        }

        public static bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        /// <summary>
        /// 执行Lua文件入口
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static System.Object DoFile(string fileName)
        {
            byte[] bytes = AssetManager.GetLuaBytes(fileName);
            return DoBuffer(bytes, fileName);
        }

        public static System.Object DoBuffer(byte[] bytes, string fileName)
        {
            if(bytes == null || bytes.Length == 0)
            {
                Logger.LogError("没有找到Lua文件： " + fileName);
                return null;
            }
            System.Object result;
            if (_luaSvr.luaState[0].doBuffer(bytes, fileName, out result) == true)
            {
                return result;
            }
            return null;
        }

        public static bool IsGroupLuaExecuting(string group)
        {
            return _executingLuaGroupSet.Contains(group);
        }

        /// <summary>
        /// 执行一个文件组中的Lua文件，在Editor环境下从Actions/Resources下加载
        /// </summary>
        /// <param name="fileInfoList"></param>
        public static void DoLuaFileInFileInfoList(string group, List<RemoteConfig.AssetInfo> fileInfoList)
        {
            if(_executingLuaGroupSet.Contains(group) == true)
            {
                string error = "资源组 " + group + " 中的Lua文件正在运行中~";
                Logger.LogError(error);
                Pandora.Instance.ReportError(error);
                return;
            }
            _executingLuaGroupSet.Add(group);
            for(int i  = 0; i < fileInfoList.Count; i++)
            {
                string name = fileInfoList[i].name;
                if(name.ToLower().Contains("_lua.assetbundle") == true)
                {
                    string luaName = GetLuaName(name);
                    string msg = "资源组加载完成，开始执行入口Lua文件： " + luaName;
                    Logger.Log(msg);
#if UNITY_EDITOR
                    //LocalLuaBytesPool.LoadLuaBytesInDirectory(luaName);
#endif
                    try
                    {
                        DoFile(luaName);
                    }
                    catch(Exception e)
                    {
                        string error = "Lua DoFile 失败， FileName: " + luaName;
                        Pandora.Instance.ReportError(error, ErrorCodeConfig.LUA_DO_FILE_EXCEPTION);
                        Pandora.Instance.Report(error, ErrorCodeConfig.LUA_SCRIPT_EXCEPTION_DETAIL, ErrorCodeConfig.TNM2_TYPE_LITERALS);
                    }
                }
            }
            //只上报第一个模块执行，用于标记成功执行到lua的用户数量
            if(_executingLuaGroupSet.Count == 1)
            {
                string msg = "执行Lua了";
                Pandora.Instance.ReportError(msg, ErrorCodeConfig.EXECUTE_ENTRY_LUA);
            }
        }

        private static string GetLuaName(string name)
        {
            return name.Split('_')[1];
        }

        public static System.Object CallLuaFunction(string functionName, params System.Object[] args)
        {
            LuaFunction func = (LuaFunction)_luaSvr.luaState[0][functionName];
            if(func != null)
            {
                return func.call(args);
            }
            return null;
        }


        public static void ReloadLua()
        {
#if UNITY_EDITOR
            _isInitialized = false;
            _luaSvr.reset(null, OnReload, LuaSvrFlag.LSF_BASIC | LuaSvrFlag.LSF_EXTLIB);
#endif
        }

#if UNITY_EDITOR
        private static void OnReload()
        {
            _isInitialized = true;
            LuaState.loaderDelegate = AssetManager.GetLuaBytes;
            Logger.Log("Lua虚拟机重启成功");
            AssetManager.Clear();
            Pandora.Instance.LoadProgramAsset();
        }
#endif

    }
}
