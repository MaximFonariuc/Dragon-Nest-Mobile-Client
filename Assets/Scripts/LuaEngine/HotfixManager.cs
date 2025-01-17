//
// HotfixManager.cs
// Created by huailiang.peng on 2016-3-11 17:0:4
//
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using XUtliPoolLib;


public class HotfixManager : IHotfixManager
{
    LuaScriptMgr hotfixLua = null;
    LuaFunction func_refresh = null;
    LuaFunction func_click_b = null;
    LuaFunction func_click_a = null;

    LuaFunction func_enterscene = null;
    LuaFunction func_leavescene = null;
    LuaFunction func_enterfinally = null;
    LuaFunction func_attach = null;
    LuaFunction func_detach = null;
    LuaFunction func_reconnect = null;
    LuaFunction func_pause = null;
    LuaFunction func_fade = null;
	LuaFunction func_pandora = null;
    LuaFunction func_pay = null;


    public const string CLICK_LUA_FILE = "HotfixClick.lua";
    public const string DOC_LUA_FILE = "HotfixDocument.lua";
    public const string MSG_LUE_FILE = "HotfixMsglist.lua";

    public string[] befRpath, aftPath, breakPath;

    public bool useHotfix = true;

    private bool init = false;

    private List<string> doluafiles = new List<string>();

    public Dictionary<string, string> hotmsglist = new Dictionary<string, string>();

    private static HotfixManager _single;
    public static HotfixManager Instance
    {
        get
        {
            if (_single == null) _single = new HotfixManager();
            return _single;
        }
    }



    /// <summary>
    /// init load click and refresh type of hot fix files
    /// </summary>
    public void LoadHotfix(System.Action finish)
    {
        init = true;
        hotfixLua = new LuaScriptMgr();
        hotfixLua.Start();
        Hotfix.Init(() =>
        {
            TryFixMsglist();
            InitClick();
            InitDocument();
            HotfixPatch.Init(hotfixLua);
            OnAttachToHost();
            if (finish != null) finish();
        });
    }


    public void Dispose()
    {
        doluafiles.Clear();
        DisposeFunc(func_click_a);
        DisposeFunc(func_click_b);
        DisposeFunc(func_attach);
        DisposeFunc(func_detach);
        DisposeFunc(func_leavescene);
        DisposeFunc(func_enterfinally);
        DisposeFunc(func_enterscene);
        DisposeFunc(func_reconnect);
        DisposeFunc(func_pause);
        DisposeFunc(func_fade);
        DisposeFunc(func_pandora);
        DisposeFunc(func_pay);
        if (init)
        {
            hotfixLua.Destroy();
            hotfixLua = null;
            init = false;
        }
    }


    private void DisposeFunc(LuaFunction func)
    {
        if (func != null)
        {
            func.Dispose();
            func = null;
        }
    }


    public bool DoLuaFile(string name)
    {
        if (doluafiles.Contains(name) || !init) return true;
        else
        {
            string path = name;
            if (hotfixLua != null)
            {
                if (LuaStatic.Load(path) == null) return false;
                hotfixLua.lua.DoFile(path);
                doluafiles.Add(name);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Used for localization
    /// </summary>
    public void TryFixMsglist()
    {
        if (!useHotfix || !init) return;
        string path = MSG_LUE_FILE;
        byte[] objs = LuaStatic.Load(path);
        if (objs != null)
        {
            ByteReader reader = new ByteReader(objs);
            hotmsglist = reader.ReadDictionary();
        }
    }

    private void InitClick()
    {
        if (!useHotfix || !init) return;
        bool dofile = DoLuaFile(CLICK_LUA_FILE);
        if (dofile)
        {
            if (func_click_b == null)
            {
                func_click_b = hotfixLua.lua.GetFunction("HotfixClick.BeforeClick");
            }
            if (func_click_a == null)
            {
                func_click_a = hotfixLua.lua.GetFunction("HotfixClick.AfterClick");
            }
            LuaFunction func_regist = hotfixLua.lua.GetFunction("HotfixClick.FetchRegists");
            if (func_regist != null)
            {
                object[] o = func_regist.Call();
                int k = -1, v = -1;
                for (int i = 0; i < o.Length; i++)
                {
                    if (o[i].Equals("|"))
                    {
                        if (k == -1) k = i;
                        else { v = i; break; }
                    }
                }
                befRpath = new string[k];
                for (int i = 0; i < k; i++)
                {
                    befRpath[i] = o[i] as string;
                }
                int max = Mathf.Max(v - k - 1, 0);
                aftPath = new string[max];
                for (int i = 0; i < max; i++)
                {
                    aftPath[i] = o[i + k + 1] as string;
                }
                max = Mathf.Max(0, o.Length - v - 1);
                breakPath = new string[max];
                for (int i = 0; i < max; i++)
                {
                    breakPath[i] = o[i + v + 1] as string;
                }
            }
            func_regist.Dispose();
        }
    }

    /// <summary>
    /// Used for All Click Event
    /// if lua return false, go on execute c#, else interupt
    /// </summary>
    public bool TryFixClick(HotfixMode _mode, string _path)
    {
        if (!useHotfix || !init) return false;
        if (_mode == HotfixMode.BEFORE)
        {
            for (int i = 0; i < breakPath.Length; i++)
            {
                if (breakPath[i].Equals(_path))
                {
                    func_click_b.Call(_path, true);
                    return true;
                }
            }

            for (int i = 0; i < befRpath.Length; i++)
            {
                if (befRpath[i].Equals(_path))
                {
                    func_click_b.Call(_path, false);
                    return false;
                }
            }
        }
        else
        {
            for (int i = 0; i < aftPath.Length; i++)
            {
                if (aftPath[i].Equals(_path))
                {
                    func_click_a.Call(_path);
                    return false;
                }
            }
        }
        return false;
    }



    /// <summary>
    /// Used for GamePage Refresh Hide Unload
    /// if lua return false, go on execute c#, else interupt
    /// </summary>
    public bool TryFixRefresh(HotfixMode _mode, string _pageName, GameObject go)
    {
        if (_pageName == "LoadingDlg" || _pageName == "LoginDlg" || _pageName == "LoginTip") return false;
        if (useHotfix && init)
        {
            string filename = "Hotfix" + _pageName + ".lua";
            bool dolua = DoLuaFile(filename);
            if (dolua)
            {
                func_refresh = null;
                if (_mode == HotfixMode.BEFORE) func_refresh = hotfixLua.lua.GetFunction(_pageName + ".BeforeRefresh");// : _pageName + ".AfterRefresh");
                else if (_mode == HotfixMode.AFTER) func_refresh = hotfixLua.lua.GetFunction(_pageName + ".AfterRefresh");
                else if (_mode == HotfixMode.HIDE) func_refresh = hotfixLua.lua.GetFunction(_pageName + ".Hide");
                else if (_mode == HotfixMode.UNLOAD) func_refresh = hotfixLua.lua.GetFunction(_pageName + ".Unload");
                if (func_refresh != null)
                {
                    object[] r = func_refresh.Call(go);
                    func_refresh.Release();
                    return r != null && r.Length > 0 ? (bool)r[0] : false;
                }
                else
                {
                    XDebug.singleton.AddLog("func is null!" + _pageName + " mode: " + _mode);
                }
            }
        }
        return false;
    }

    public bool TryFixHandler(HotfixMode _mode, string _handlerName, GameObject go)
    {
        if (useHotfix && init)
        {
            string filename = "Hotfix" + _handlerName + ".lua";
            bool dolua = DoLuaFile(filename);
            if (dolua)
            {
                func_refresh = null;
                if (_mode == HotfixMode.BEFORE) func_refresh = hotfixLua.lua.GetFunction(_handlerName + ".BeforeHandlerShow");// : _pageName + ".AfterRefresh");
                else if (_mode == HotfixMode.AFTER) func_refresh = hotfixLua.lua.GetFunction(_handlerName + ".AfterHandlerShow");
                else if (_mode == HotfixMode.HIDE) func_refresh = hotfixLua.lua.GetFunction(_handlerName + ".Hide");
                else if (_mode == HotfixMode.UNLOAD) func_refresh = hotfixLua.lua.GetFunction(_handlerName + ".Unload");
                // func_refresh = hotfixLua.lua.GetFunction(_mode == HotfixMode.BEFORE ? _handlerName + ".BeforeHandlerShow" : _handlerName + ".AfterHandlerShow");
                if (func_refresh != null)
                {
                    object[] r = func_refresh.Call(go);
                    func_refresh.Release();
                    return r != null && r.Length > 0 ? (bool)r[0] : false;
                }
                else
                {
                    XDebug.singleton.AddGreenLog("func is null! " + _handlerName);
                }
            }
        }
        return false;
    }

    public void CallLuaFunc(string className, string funcName)
    {
        LuaFunction func = null;
        func = hotfixLua.lua.GetFunction(XCommon.singleton.StringCombine(className, ".", funcName));
        if(func != null)
        {
            func.Call();
            func.Release();
        }
    }

    public void RegistedPtc(uint _type, byte[] body, int length)
    {
        XLua.NotifyRoute(_type, body, length);
    }


    public void ProcessOveride(uint _type, byte[] body, int length)
    {
        XLua.OverideNet(_type, body, length);
    }

    public LuaScriptMgr GetLuaScriptMgr()
    {
        return hotfixLua;
    }

    private void InitDocument()
    {
        if (!useHotfix || !init) return;
        bool dofile = DoLuaFile(DOC_LUA_FILE);
        if (dofile)
        {
            if(func_leavescene == null)
            {
                func_leavescene = hotfixLua.lua.GetFunction("HotfixDocument.LeaveScene");
            }
            if (func_enterscene == null)
            {
                func_enterscene = hotfixLua.lua.GetFunction("HotfixDocument.EnterScene");
            }
            if (func_enterfinally == null)
            {
                func_enterfinally = hotfixLua.lua.GetFunction("HotfixDocument.EnterSceneFinally");
            }
            if (func_attach == null)
            {
                func_attach = hotfixLua.lua.GetFunction("HotfixDocument.Attach");
            }
            if (func_detach == null)
            {
                func_detach = hotfixLua.lua.GetFunction("HotfixDocument.Detach");
            }
            if (func_reconnect == null)
            {
                func_reconnect = hotfixLua.lua.GetFunction("HotfixDocument.Reconnect");
            }
            if (func_pause == null)
            {
                func_pause = hotfixLua.lua.GetFunction("HotfixDocument.Pause");
            }
            if (func_fade == null)
            {
                func_fade = hotfixLua.lua.GetFunction("HotfixDocument.FadeShow");
            }
            if(func_pandora == null)
            {
                func_pandora = hotfixLua.lua.GetFunction("HotfixDocument.PandoraCallback");
            }
            if(func_pay == null)
            {
                func_pay = hotfixLua.lua.GetFunction("HotfixDocument.PayCallback");
            }
        }
    }

    public void OnLeaveScene()
    {
        if(func_leavescene != null)
        {
            func_leavescene.Call();
        }
    }

    public void OnEnterScene()
    {
        if (func_enterscene != null)
        {
            func_enterscene.Call();
        }
    }

    public void OnEnterSceneFinally()
    {
        if (func_enterfinally != null)
        {
            func_enterfinally.Call();
        }
    }


    public void OnAttachToHost()
    {
        if (func_attach != null)
        {
            func_attach.Call();
        }
    }

    public void OnPandoraCallback(string json)
    {
        if(func_pandora != null)
        {
            Debug.Log("json=>"+json);
            func_pandora.Call(json);
        }
    }

    public void OnPayCallback(string result, string paramID)
    {
        if(func_pay != null)
        {
            func_pay.Call(result, paramID);
        }
    }

    public void OnReconnect()
    {
        if (func_reconnect != null)
        {
            func_reconnect.Call();
        }
    }

    public void OnDetachFromHost()
    {
        if (func_detach != null)
        {
            func_detach.Call();
        }
    }

    public void FadeShow(bool show)
    {
        if (func_fade != null)
        {
            func_fade.Call(show);
        }
    }

    public void OnPause(bool pause)
    {
        if (func_pause != null)
        {
            func_pause.Call(pause);
        }
    }

}
