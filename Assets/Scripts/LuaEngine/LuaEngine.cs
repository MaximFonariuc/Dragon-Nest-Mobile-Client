//
// LuaEngine.cs
// Created by huailiang.peng on 2016/04/14 11:39:07
//

using UnityEngine;
using System.Collections;
using LuaCore;
using System.IO;
using LuaInterface;
using XUtliPoolLib;


public class LuaEngine : MonoBehaviour, ILuaEngine
{

    public static LuaEngine Instance = null;

    void Awake()
    {
        Instance = this;
    }


    void OnDestroy()
    {
        HotfixManager.Instance.Dispose();
        Instance = null;
    }



    bool gui = false;
    //string cname = "LuaEngine";
    string origin_text = "des";
    string des_text = "code";
    int y = 200;

#if UNITY_EDITOR
    void OnGUI()
    {

        if (gui)
        {
            GUI.color = Color.red;
            y = 200;
            GUI.Label(new Rect(20, y, 700, 30), "HOTFIX TOOL");
            GUI.color = Color.white;

            GUI.color = Color.green;
            y += 40;
            GUI.Label(new Rect(20, y, 100, 30), "Click Btn Path:");
            y += 30;
            GUI.TextArea(new Rect(20, y, 700, 20), UICamera.clickpath);

           y += 30;
           GUI.Label(new Rect(20, y, 280, 30), "Test DES Encryption");
           y += 30;
           origin_text = GUI.TextField(new Rect(20, y, 180, 20), origin_text);
           if (GUI.Button(new Rect(220, y, 80, 24), "Encrypt"))
           {
               des_text = Encryption.Encrypt(origin_text);
               Debug.Log(des_text);
           }
           if (GUI.Button(new Rect(320, y, 80, 24), "Decrypt"))
           {
               Debug.Log(Encryption.Decrypt(des_text));
           }
        }
    }
#endif
    private bool init = false;

    public void InitLua()
    {
        init = true;
        TimerManager.Instance.Init();     
        HotfixManager.Instance.LoadHotfix(OnInitFinish);
    }


    void Update()
    {
        if (init)
        {
            TimerManager.Instance.Update();
        }
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.F5))
        {
            //gui = !gui;
        }
#endif
    }

    void OnApplicationPause(bool pause)
    {
        HotfixManager.Instance.OnPause(pause);
    }

    private void OnInitFinish()
    {
        Debug.Log("Hotfix init finish!");
    }


    public IHotfixManager hotfixMgr
    {
        get
        {
            return HotfixManager.Instance as IHotfixManager;
        }
    }

    public ILuaUIManager luaUIManager
    {
        get
        {
            return LuaUIManager.Instance as ILuaUIManager;
        }
    }

    public ILuaGameInfo luaGameInfo
    {
        get
        {
            return LuaGameInfo.single as ILuaGameInfo;
        }
    }
}
