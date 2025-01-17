using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using System;

public class XLua
{




    /// <summary>
    /// ptc 处理
    /// </summary>
    /// <returns></returns>
    public static void NotifyRoute(uint _type, byte[] bytes, int length)
    {
        LuaScriptMgr mgr = HotfixManager.Instance.GetLuaScriptMgr();
        mgr.DoFile("LuaNotifyProcess.lua");
        LuaFunction func = mgr.GetLuaFunction("LuaNotifyProcess.Process");
        func.Call(_type, Hotfix.LuaProtoBuffer(bytes, length), length);
    }



    /// <summary>
    /// 重载 c#协议
    /// </summary>
    public static void OverideNet(uint _type,byte[] bytes,int length)
    {
        LuaScriptMgr mgr = HotfixManager.Instance.GetLuaScriptMgr();
        mgr.DoFile("LuaNotifyProcess.lua");
        LuaFunction func = mgr.GetLuaFunction("LuaNotifyProcess.ProcessOveride");
        func.Call(_type, Hotfix.LuaProtoBuffer(bytes, length), length);
    }


    /// <summary>
    /// 抓取lua初始化的协议
    /// </summary>
    /// <returns></returns>
    public static object[] FetchRegistID()
    {
        LuaScriptMgr mgr = HotfixManager.Instance.GetLuaScriptMgr();
        mgr.DoFile("LuaNotifyProcess.lua");
        LuaFunction func = mgr.GetLuaFunction("LuaNotifyProcess.FetchRegistedID");
        return func.Call();
    }


}
