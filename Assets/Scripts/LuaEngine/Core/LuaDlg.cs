using UnityEngine;
using System.Collections;
using LuaInterface;
using System.Text;

public class LuaDlg : MonoBehaviour
{

    private LuaScriptMgr mgr;

    private string m_name
    {
        get { return name.Substring(0, 1).ToUpper() + name.Substring(1); }
    }

    private const string AWAKE = "Awake";
    private const string START = "Start";
    private const string ENABLE = "OnEnable";
    private const string DISABLE = "OnDisable";
    private const string HIDE = "OnHide";
    private const string SHOW = "OnShow";
    private const string DESTROY = "OnDestroy";

    void Awake()
    {
        mgr = HotfixManager.Instance.GetLuaScriptMgr();
        mgr.DoFile("Lua" + m_name + ".lua");
        LuaFunction func = mgr.GetLuaFunction(StrAppend(AWAKE));
        if (func != null) func.Call(gameObject);
    }


    void Start()
    {
        if (mgr != null)
        {
            LuaFunction func = mgr.GetLuaFunction(StrAppend(START));
            if (func != null) func.Call();
        }
    }


    void OnEnable()
    {
        if (mgr != null)
        {
            LuaFunction func = mgr.GetLuaFunction(StrAppend(ENABLE));
            if (func != null) func.Call();
        }
    }



    void OnDisable()
    {
        if (mgr != null)
        {
            LuaFunction func = mgr.GetLuaFunction(StrAppend(DISABLE));
            if (func != null) func.Call();
        }
    }

    public void OnHide()
    {
        if (mgr != null)
        {
            LuaFunction func = mgr.GetLuaFunction(StrAppend(HIDE));
            if (func != null) func.Call();
        }
    }


    public void OnDestroy()
    {
        if (mgr != null)
        {
            try
            {
                LuaFunction func = mgr.GetLuaFunction(StrAppend(DESTROY));
                if (func != null) func.Call();
            }
            catch { };
        }
    }


    public void OnShow()
    {
        if (mgr != null)
        {
            LuaFunction func = mgr.GetLuaFunction(StrAppend(SHOW));
            if (func != null) func.Call();
        }
    }


    private string StrAppend(string func)
    {
        StringBuilder sb = new StringBuilder("Lua");
        sb.Append(m_name);
        sb.Append(".");
        sb.Append(func);
        return sb.ToString();
    }

}
