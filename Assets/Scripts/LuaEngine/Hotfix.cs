//
// Hotfix.cs
// Created by huailiang.peng on 2016/03/14 11:39:07
//
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using LuaCore;
using LuaInterface;
using System.IO;
using System;
using XUtliPoolLib;
using UILib;

public class Hotfix
{
    
    static int localversion { get { return PlayerPrefs.GetInt("hotfixversion", 0); } }

    static LuaStringBuffer sharedStringBuff0 = new LuaStringBuffer();
    static LuaStringBuffer sharedStringBuff1 = new LuaStringBuffer();
    //static DelLuaRespond _sendLuaByteRPCCb = null;

    //static DelLuaError _sendLuaByteRPCErrorCb = null;

    private static void InitNet()
    {
        if (m_network == null || m_network.Deprecated)
            m_network = XInterfaceMgr.singleton.GetInterface<ILuaNetwork>(XCommon.singleton.XHash("ILUANET"));
    }
    public static void Init(Action finish)
    {
        if (finish != null) finish();
        RegistNotifyID();
        if(m_network!=null)
        {
            m_network.InitLua(1024);
        }
        //  LuaEngine.Instance.StartCoroutine(LoadHotfix(finish));
    }

    public static int LuaWait(int delay, LuaFunction cb)
    {
        return TimerManager.Instance.AddTimer(delay, 1, (x) => { cb.Call(x); });
    }

    public static int LuaLoop(int delay, int loop, LuaFunction cb)
    {
       return TimerManager.Instance.AddTimer(delay, loop, (x) => { cb.Call(x); });
    }

    public static void RemoveTimer(int seq)
    {
        TimerManager.Instance.RemoveTimer(seq);
    }

    private static ILuaNetwork m_network;
    public static bool SendLuaPtc(uint _type, LuaStringBuffer _data)
    {
        return SendLuaBytePtc(_type, _data.buffer);
    }

    public static bool SendLuaBytePtc(uint _type, byte[] _req)
    {
        InitNet();
        if (m_network == null)
        {
            Debug.LogError("network is null");
            return false;
        }
        else
        {
            return m_network.LuaSendPtc(_type, _req);
        }
    }

    public static void RegistNotifyID()
    {
        InitNet();
        object[] objs = XLua.FetchRegistID();
        if (objs != null)
        {
            List<uint> list = new List<uint>();
            for (int i = 0; i < objs.Length; i += 2)
            {
                uint type = Convert.ToUInt32(objs[i]);
                uint sign = Convert.ToUInt32(objs[i + 1]);
                if (sign == 2)
                {
                    list.Add(type);
                }
                else
                {
                    bool copyBuffer = sign == 1;
                    m_network.LuaRigsterPtc(type, copyBuffer);
                }
            }

            if (list.Count > 0) m_network.LuaRegistDispacher(list);
        }
    }

    public static void RegistPtc(uint type, bool copyBuffer)
    {
        InitNet();
        m_network.LuaRigsterPtc(type, copyBuffer);
    }
    public static void RegisterLuaRPC(uint _type, bool copyBuffer, LuaFunction _res, LuaFunction _err)
    {
        InitNet();
        if (m_network == null)
        {
            Debug.LogError("network is null");
        }
        else
        {
            m_network.LuaRigsterRPC(_type, copyBuffer,
                (bytes, length) =>
                {
                    if (bytes == null)
                    {
                        _res.Call(null, 0);
                    }
                    else
                    {
                        _res.Call(LuaProtoBuffer(bytes, length), length);
                    }

                },
                (errcode) =>
                {
                    _err.Call(errcode);
                });
        }
    }

    public static void SendLuaRPC(uint _type, LuaStringBuffer _data, LuaFunction _res, LuaFunction _err)
    {
        _SendLuaRPC(_type, _data, _res, _err, false);
    }

    public static void SendLuaRPCWithReq(uint _type, LuaStringBuffer _data, LuaFunction _res, LuaFunction _err)
    {
        _SendLuaRPC(_type, _data, _res, _err, true);
    }

    static void _SendLuaRPC(uint _type, LuaStringBuffer _data, LuaFunction _res, LuaFunction _err, bool withReq)
    {
        //PrintBytes(_data.buffer);
        _SendLuaByteRPC(_type, _data.buffer, _res, _err, withReq);
    }

    public static void SendLuaByteRPC(uint _type, byte[] _req, LuaFunction _res, LuaFunction _err)
    {
        _SendLuaByteRPC(_type, _req, _res, _err, false);
    }

    public static void SendLuaByteRPCWithReq(uint _type, byte[] _req, LuaFunction _res, LuaFunction _err)
    {
        _SendLuaByteRPC(_type, _req, _res, _err, true);
    }

   
    static void _SendLuaByteRPC(uint _type, byte[] _req, LuaFunction _res, LuaFunction _err, bool withReq)
    {
        InitNet();
        if (m_network == null)
        {
            Debug.LogError("network is null");
        }
        else
        {
            //if (_sendLuaByteRPCCb == null)
            //    _sendLuaByteRPCCb = new DelLuaRespond(_SendLuaByteRPCRespond);
            //if (_sendLuaByteRPCErrorCb == null)
            //    _sendLuaByteRPCErrorCb = new DelLuaError(_SendLuaByteRPCError);
            m_network.LuaSendRPC(_type, _req,
                (bytes, length) =>
                {
                    if (withReq)
                        _res.Call(LuaProtoBuffer(_req, length), LuaProtoBuffer1(bytes, length), length);
                    else
                    {
                        _res.Call(LuaProtoBuffer(bytes, length), length);
                    }
                },
                (errcode) =>
                {
                    _err.Call(errcode);
                });
        }
    }

    private static IModalDlg m_modalDlg;
    public static void LuaMessageBoxConfirm(string str, LuaFunction okDel, LuaFunction cancelDel)
    {
        if (m_modalDlg == null || m_modalDlg.Deprecated)
            m_modalDlg = XInterfaceMgr.singleton.GetInterface<IModalDlg>(XCommon.singleton.XHash("IModalDlg"));
        if (m_modalDlg == null)
        {
            Debug.LogError("modal dlg is null!");
        }
        else
        {
            m_modalDlg.LuaShow(str,
                (btn) =>
                {
                    if (okDel != null) okDel.Call();
                    return true;
                },
                (btn) =>
                {
                    if (cancelDel != null) cancelDel.Call();
                    return true;
                });
        }
    }

    public static IUiUtility m_uiUtility;
    public static void LuaShowSystemTip(string text)
    {
        if (m_uiUtility == null || m_uiUtility.Deprecated)
            m_uiUtility = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));

        if (m_uiUtility == null)
            Debug.LogError("uiUtility is null!");
        else
            m_uiUtility.ShowSystemTip(text);
    }
    public static void LuaShowSystemTipErrCode(int errCode)
    {
        if (m_uiUtility == null || m_uiUtility.Deprecated)
            m_uiUtility = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));

        if (m_uiUtility == null)
            Debug.LogError("uiUtility is null!");
        else
            m_uiUtility.ShowSystemTip(errCode);
    }
    public static void LuaShowItemAccess(int itemID)
    {
        if (m_uiUtility == null || m_uiUtility.Deprecated)
            m_uiUtility = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));

        if (m_uiUtility == null)
            Debug.LogError("uiUtility is null!");
        else
            m_uiUtility.ShowItemAccess(itemID);
    }
    public static void LuaShowItemTooltipDialog(int itemID, GameObject icon)
    {
        if (m_uiUtility == null || m_uiUtility.Deprecated)
            m_uiUtility = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));

        if (m_uiUtility == null)
            Debug.LogError("uiUtility is null!");
        else
            m_uiUtility.ShowTooltipDialog(itemID, icon);
    }

    public static void LuaShowDetailTooltipDialog(int itemID, GameObject icon)
    {
        if (m_uiUtility == null || m_uiUtility.Deprecated)
            m_uiUtility = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));

        if (m_uiUtility == null)
            Debug.LogError("uiUtility is null!");
        else
            m_uiUtility.ShowDetailTooltip(itemID, icon);
    }

    public static void LuaShowItemTooltipDialogByUID(string strUID, GameObject icon)
    {
        if (m_uiUtility == null || m_uiUtility.Deprecated)
            m_uiUtility = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));

        if (m_uiUtility == null)
            Debug.LogError("uiUtility is null!");
        else
            m_uiUtility.ShowTooltipDialogByUID(strUID, icon);
    }

    private static ILuaExtion m_luaExtion = null;

    public static ILuaExtion luaExtion
    {
        get
        {
            if (null == m_luaExtion || m_luaExtion.Deprecated)
                m_luaExtion = XInterfaceMgr.singleton.GetInterface<ILuaExtion>(XCommon.singleton.XHash("ILuaExtion"));
            return m_luaExtion;
        }
    }

    public static void SetPlayer(string key, string value)
    {
        luaExtion.SetPlayerProprerty(key, value);
    }

    public static object GetPlayer(string key)
    {
        return luaExtion.GetPlayeProprerty(key);
    }

    public static object CallPlayerMethod(bool isPublic, string method, params object[] args)
    {
        return luaExtion.CallPlayerMethod(isPublic, method, args);
    }

    public static object GetDocument(string doc)
    {
        return luaExtion.GetDocument(doc);
    }

    public static void SetDocumentMember(string doc, string key, object value, bool isPublic, bool isField)
    {
        luaExtion.SetDocumentMember(doc, key, value, isPublic, isField);
    }

    public static object GetDocumentMember(string doc, string key, bool isPublic, bool isField)
    {
        return luaExtion.GetDocumentMember(doc, key, isPublic, isField);
    }

    public static string GetGetDocumentLongMember(string doc, string key, bool isPublic, bool isField)
    {
        return GetDocumentMember(doc, key, isPublic, isField).ToString();
    }
    public static object GetDocumentStaticMember(string doc, string key, bool isPublic, bool isField)
    {
        return luaExtion.GetDocumentStaticMember(doc, key, isPublic, isField);
    }

    public static object CallDocumentMethod(string doc, bool isPublic, string method, params object[] args)
    {
        return luaExtion.CallDocumentMethod(doc, isPublic, method, args);
    }

    public static string CallDocumentLongMethod(string doc, bool isPublic, string method, params object[] args)
    {
        return luaExtion.CallDocumentMethod(doc, isPublic, method, args).ToString();
    }

    public static object CallDocumentStaticMethod(string doc, bool isPublic, string method, params object[] args)
    {
        return luaExtion.CallDocumentStaticMethod(doc, isPublic, method, args);
    }

    public static object GetSingle(string className)
    {
        return luaExtion.GetSingle(className);
    }

    public static object GetSingleMember(string className, string key, bool isPublic, bool isField, bool isStatic)
    {
        return luaExtion.GetSingleMember(className, key, isPublic, isField, isStatic);
    }

    public static string GetSingleLongMember(string className, string key, bool isPublic, bool isField, bool isStatic)
    {
        return GetSingleMember(className, key, isPublic, isField, isStatic).ToString();
    }

    public static void SetSingleMember(string className, string key, object value, bool isPublic, bool isField, bool isStatic)
    {
        luaExtion.SetSingleMember(className, key, value, isPublic, isField, isStatic);
    }

    public static object CallSingleMethod(string className, bool isPublic, bool isStatic, string methodName, params object[] args)
    {
        return luaExtion.CallSingleMethod(className, isPublic, isStatic, methodName, args);
    }

    public static string CallSingleLongMethod(string className, bool isPublic, bool isStatic, string methodName, params object[] args)
    {
        return luaExtion.CallSingleMethod(className, isPublic, isStatic, methodName, args).ToString();
    }

    public static object GetEnumType(string classname, string value)
    {
        return luaExtion.GetEnumType(classname, value);
    }

    public static string GetStringTable(string key, params object[] args)
    {
        return luaExtion.GetStringTable(key, args);
    }

    public static string GetGlobalString(string key)
    {
        return luaExtion.GetGlobalString(key);
    }

    public static string GetObjectString(object o,string name)
    {
        return PublicExtensions.GetPublicField(o, name).ToString();
    }

    public static string GetObjectString(object o, string name, bool isPublic, bool isField)
    {
        if (isPublic)
        {
            if (isField)
                return o.GetPublicField(name).ToString();
            else
                return PublicExtensions.GetPublicProperty(o, name).ToString();
        }
        else
        {
            if (isField)
                return o.GetPrivateField(name).ToString();
            else
                return PrivateExtensions.GetPrivateProperty(o, name).ToString();
        }
    }

    public static XLuaLong GetLuaLong(string str)
    {
        return luaExtion.Get(str);
    }

    public static void RefreshPlayerName()
    {
        luaExtion.RefreshPlayerName();
    }

    private static IGameSysMgr m_GameSysMgr = null;
    public static IGameSysMgr GameSysMgr
    {
        get
        {
            if (null == m_GameSysMgr || m_GameSysMgr.Deprecated)
                m_GameSysMgr = XInterfaceMgr.singleton.GetInterface<IGameSysMgr>(XCommon.singleton.XHash("IGameSysMgr"));
            return m_GameSysMgr;
        }
    }

    public static bool OpenSys(int sys)
    {
        GameSysMgr.OpenSystem(sys);
        return GameSysMgr.IsSystemOpen(sys);
    }

    public static int onlineReTime
    {
        get { return (int)GameSysMgr.OnlineRewardRemainTime; }
    }

    public static void AttachSysRedPointRelative(int sys, int childSys, bool bImmCalculate)
    {
        GameSysMgr.AttachSysRedPointRelative(sys, childSys, bImmCalculate);
    }

    public static void AttachSysRedPointRelativeUI(int sys, GameObject go)
    {
        GameSysMgr.AttachSysRedPointRelativeUI(sys, go);
    }

    public static void DetachSysRedPointRelative(int sys)
    {
        GameSysMgr.DetachSysRedPointRelative(sys);
    }

    public static void DetachSysRedPointRelativeUI(int sys)
    {
        GameSysMgr.DetachSysRedPointRelativeUI(sys);
    }

    public static void ForceUpdateSysRedPointImmediately(int sys, bool redpoint)
    {
        GameSysMgr.ForceUpdateSysRedPointImmediately(sys, redpoint);
    }

    public static bool GetSysRedPointState(int sys)
    {
        return GameSysMgr.GetSysRedPointState(sys);
    }


    public static void LuaDoFile(string name)
    {
        LuaScriptMgr mgr = HotfixManager.Instance.GetLuaScriptMgr();
        if (mgr != null)
        {
            mgr.DoFile(name);
        }
    }

    public static LuaFunction LuaGetFunction(string func)
    {
        LuaScriptMgr mgr = HotfixManager.Instance.GetLuaScriptMgr();
        if (mgr != null) return mgr.GetLuaFunction(func);
        else
        {
            Debug.LogError("LuaScriptMgr is null");
            return null;
        }
    }

    public static string LuaTableBuffer(string location)
    {
        Stream stream = XResourceLoaderMgr.singleton.ReadText(location, ".bytes");
        StreamReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static System.IO.BinaryReader LuaTableBin(string location)
    {
        Stream stream = XResourceLoaderMgr.singleton.ReadText(location, ".bytes");
        System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);
        //int length = reader.ReadInt32();
        return reader;
    }
    public static void ReturnableStream(System.IO.BinaryReader reader)
    {
        dataHandler.UnInit(true);
        XResourceLoaderMgr.singleton.ClearStream(reader.BaseStream);
    }

    static int fileSize = 0;
    public static void ReadFileSize(System.IO.BinaryReader reader)
    {
        fileSize = reader.ReadInt32();
    }

    public static void CheckFileSize(System.IO.BinaryReader reader, string tableName)
    {
        int pos = (int)reader.BaseStream.Position;
        if (pos != fileSize)
        {
            XDebug.singleton.AddErrorLog2("read table error:{0} size:{1} read size:{2}", tableName, fileSize, pos);
        }
    }

    static int rowSize = 0;
    static int beforePos = 0;
    public static void ReadRowSize(System.IO.BinaryReader reader)
    {
        rowSize = reader.ReadInt32();
        beforePos = (int)reader.BaseStream.Position;
    }

    public static void CheckRowSize(System.IO.BinaryReader reader, string tableName, int lineno)
    {
        int afterPos = (int)reader.BaseStream.Position;
        int delta = afterPos - beforePos;
        if (rowSize > delta)
        {
            reader.BaseStream.Seek(rowSize - delta, SeekOrigin.Current);
        }
        else if (rowSize < delta)
        {
            XDebug.singleton.AddErrorLog2("read table error:{0} line:{1}", tableName, lineno);
        }
    }
    static DataHandler dataHandler = new DataHandler();
    public static void ReadDataHandle(System.IO.BinaryReader reader)
    {
        dataHandler.Init(reader);
    }



    public static ushort startOffset;
    public static byte count;
    public static byte allSameMask;
    public static void ReadSeqHead(System.IO.BinaryReader reader)
    {
        startOffset = reader.ReadUInt16();
    }
    public static int ReadSeqListHead(System.IO.BinaryReader reader)
    {
        count = reader.ReadByte();
        allSameMask = 0;
        startOffset = 0;
        if (count > 0)
        {
            allSameMask = reader.ReadByte();
            startOffset = reader.ReadUInt16();
        }
        return count;
    }

    public static int ReadInt(int key)
    {
        return dataHandler.intBuffer[startOffset+key];
    }
    public static int ReadInt(int index, int key)
    {
        int[] buffer = dataHandler.intBuffer;
        return allSameMask == 1 ? (buffer[startOffset + key]) : buffer[dataHandler.indexBuffer[startOffset + index] + key];
    }
    public static uint ReadUInt(int key)
    {
        return dataHandler.uintBuffer[startOffset + key];
    }
    public static uint ReadUInt(int index, int key)
    {
        uint[] buffer = dataHandler.uintBuffer;
        return allSameMask == 1 ? (buffer[startOffset + key]) : buffer[dataHandler.indexBuffer[startOffset + index] + key];
    }
    public static string ReadLong(System.IO.BinaryReader reader)
    {
        return reader.ReadInt64().ToString();
    }
    public static string ReadLong(int key)
    {
        return dataHandler.longBuffer[startOffset + key].ToString();
    }

    public static string ReadLong(int index, int key)
    {
        long[] buffer = dataHandler.longBuffer;
        return allSameMask == 1 ? (buffer[startOffset + key]).ToString() : buffer[dataHandler.indexBuffer[startOffset + index] + key].ToString();
    }

    public static float ReadFloat(int key)
    {
        return dataHandler.floatBuffer[startOffset + key];
    }
    public static float ReadFloat(int index, int key)
    {
        float[] buffer = dataHandler.floatBuffer;
        return allSameMask == 1 ? (buffer[startOffset + key]) : buffer[dataHandler.indexBuffer[startOffset + index] + key];
    }
    public static double ReadDouble(int key)
    {
        return dataHandler.doubleBuffer[startOffset + key];
    }
    public static double ReadDouble(int index, int key)
    {
        double[] buffer = dataHandler.doubleBuffer;
        return allSameMask == 1 ? (buffer[startOffset + key]) : buffer[dataHandler.indexBuffer[startOffset + index] + key];
    }
    public static string ReadString(int key)
    {
        return dataHandler.stringBuffer[startOffset + key];
    }
    public static string ReadString(int index, int key)
    {
        string[] buffer = dataHandler.stringBuffer;
        return allSameMask == 1 ? (buffer[startOffset + key]) : buffer[dataHandler.indexBuffer[startOffset + index] + key];
    }
    public static string ReadString(System.IO.BinaryReader reader)
    {
        return dataHandler.ReadString(reader);
    }
    //public static LuaStringBuffer LuaProtoBuffer(byte[] bytes)
    //{
    //    sharedStringBuff0.Set(bytes);
    //    return sharedStringBuff0;;
    //    //return new LuaStringBuffer(bytes);
    //}

    public static LuaStringBuffer LuaProtoBuffer(byte[] bytes,int length)
    {
        sharedStringBuff0.Copy(bytes, length);
        return sharedStringBuff0; ;
        //return new LuaStringBuffer(bytes);
    }
    //public static LuaStringBuffer LuaProtoBuffer1(byte[] bytes)
    //{
    //    sharedStringBuff1.Set(bytes);
    //    return sharedStringBuff1;// new LuaStringBuffer(bytes);
    //}
    public static LuaStringBuffer LuaProtoBuffer1(byte[] bytes, int length)
    {
        sharedStringBuff1.Copy(bytes, length);
        return sharedStringBuff1;// new LuaStringBuffer(bytes);
    }

    public static void SetClickCallback(GameObject go, LuaFunction cb)
    {
        UIEventListener.Get(go).onClick = (g) => { cb.Call(go); };
    }

    public static void SetPressCallback(GameObject go,LuaFunction cb)
    {
        UIEventListener.Get(go).onPress = (g, press) => { cb.Call(g, press); };
    }

    public static void SetDragCallback(GameObject go, LuaFunction cb)
    {
        UIEventListener.Get(go).onDrag = (g, delta) => { cb.Call(g,delta.x,delta.y); };
    }

    public static void SetSubmmitCallback(GameObject go,LuaFunction cb)
    {
        UIEventListener.Get(go).onSubmit = (g) => { cb.Call(g); };
    }
    

    public static void InitWrapContent(GameObject goWrapContent, LuaFunction cb)
    {
        IXUIWrapContent _wrapContent = goWrapContent.GetComponent("XUIWrapContent") as IXUIWrapContent;
        _wrapContent.RegisterItemUpdateEventHandler((t, index) => { cb.Call(t, index); });
    }

    public static void SetWrapContentCount(GameObject goWrapContent, int wrapCount, bool bResetPosition)
    {
        IXUIWrapContent _wrapContent = goWrapContent.GetComponent("XUIWrapContent") as IXUIWrapContent;
        _wrapContent.SetContentCount(wrapCount);
        
        if (bResetPosition)
        {
            IXUIScrollView _scrollView = goWrapContent.transform.parent.GetComponent("XUIScrollView") as IXUIScrollView;
            _scrollView.ResetPosition();
        }
    }

    public static XUIPool SetupPool(GameObject parent, GameObject tpl, uint Count)
    {
        XUIPool pool = new XUIPool(null);
        pool.SetupPool(parent, tpl, Count, false);
        return pool;
    }

    private static IXNormalItemDrawer m_itemDrawer;
    public static void DrawItemView(GameObject goItemView, int itemID, int count, bool showCount)
    {
        if (m_itemDrawer == null || m_itemDrawer.Deprecated)
            m_itemDrawer = XInterfaceMgr.singleton.GetInterface<IXNormalItemDrawer>(XCommon.singleton.XHash("IXNormalItemDrawer"));

        if (m_itemDrawer == null)
            Debug.LogError("IXNormalItemDrawer is null");
        else
            m_itemDrawer.DrawItem(goItemView, itemID, count, showCount);
    }

    public static void SetTexture(UITexture text, string localtion, bool makepiexl)
    {
        text.SetTexture(localtion);
        //text.mainTexture = XResourceLoaderMgr.singleton.GetSharedResource<Texture>(localtion, ".png");
        if (makepiexl) text.MakePixelPerfect();
    }

    public static void DestoryTexture(UITexture uitex, string location)
    {
        if (uitex != null)
        {
            uitex.SetTexture("");
            //Texture tex = uitex.mainTexture;
            //if (tex != null)
            //{
            //    uitex.mainTexture = null;
            //    //XResourceLoaderMgr.singleton.DestroyShareResource(location, tex);
            //}
        }
    }

    private static IX3DAvatarMgr m_avatarMgr;
    public static void EnableMainDummy(bool enable, UIDummy snapShot)
    {
        if (m_avatarMgr == null || m_avatarMgr.Deprecated)
            m_avatarMgr = XInterfaceMgr.singleton.GetInterface<IX3DAvatarMgr>(XCommon.singleton.XHash("IX3DAvatarMgr"));
        m_avatarMgr.EnableMainDummy(enable, snapShot);
    }

    public static void SetMainDummy(bool ui)
    {
         if (m_avatarMgr == null || m_avatarMgr.Deprecated)
            m_avatarMgr = XInterfaceMgr.singleton.GetInterface<IX3DAvatarMgr>(XCommon.singleton.XHash("IX3DAvatarMgr"));
         m_avatarMgr.SetMainDummy(ui);
    }

    public static void ResetMainAnimation()
    {
        if (m_avatarMgr == null || m_avatarMgr.Deprecated)
            m_avatarMgr = XInterfaceMgr.singleton.GetInterface<IX3DAvatarMgr>(XCommon.singleton.XHash("IX3DAvatarMgr"));
        m_avatarMgr.ResetMainAnimation();
    }


    public static string CreateCommonDummy(int dummyPool, uint presentID, IUIDummy snapShot, float scale)
    {
        if (m_avatarMgr == null || m_avatarMgr.Deprecated)
            m_avatarMgr = XInterfaceMgr.singleton.GetInterface<IX3DAvatarMgr>(XCommon.singleton.XHash("IX3DAvatarMgr"));
        return m_avatarMgr.CreateCommonDummy(dummyPool,presentID, snapShot, null, scale);
    }


    public void SetDummyAnim(int dummyPool, string idStr, string anim)
    {
        if (m_avatarMgr == null || m_avatarMgr.Deprecated)
            m_avatarMgr = XInterfaceMgr.singleton.GetInterface<IX3DAvatarMgr>(XCommon.singleton.XHash("IX3DAvatarMgr"));
        m_avatarMgr.SetDummyAnim(dummyPool, idStr, anim);
    }

    public void SetMainDummyAnim(string anim)
    {
        if (m_avatarMgr == null || m_avatarMgr.Deprecated)
            m_avatarMgr = XInterfaceMgr.singleton.GetInterface<IX3DAvatarMgr>(XCommon.singleton.XHash("IX3DAvatarMgr"));
        m_avatarMgr.SetMainDummyAnim(anim);
    }

    public static void DestroyDummy(int dummyPool, string idStr)
    {
        if (m_avatarMgr == null || m_avatarMgr.Deprecated)
            m_avatarMgr = XInterfaceMgr.singleton.GetInterface<IX3DAvatarMgr>(XCommon.singleton.XHash("IX3DAvatarMgr"));
        m_avatarMgr.DestroyDummy(dummyPool, idStr);
    }

    public static int ParseIntSeqList(object obj, int index, int key)
    {
        ISeqListRef<int> var = (ISeqListRef<int>)obj;
        return var[index, key];        
    }

    public static uint ParseUIntSeqList(object obj, int index, int key)
    {
        ISeqListRef<uint> var = (ISeqListRef<uint>)obj;
        return var[index, key];        
    }

    public static float ParseFloatSeqList(object obj, int index, int key)
    {
        ISeqListRef<float> var = (ISeqListRef<float>)obj;
        return var[index, key];        
    }
    public static double ParseDoubleSeqList(object obj, int index, int key)
    {
        ISeqListRef<double> var = (ISeqListRef<double>)obj;
        return var[index, key];        
    }
    public static string ParseStringSeqList(object obj, int index, int key)
    {
        ISeqListRef<string> var = (ISeqListRef<string>)obj;
        return var[index, key];        
    }

    public static ulong TransInt64(string a)
    {
        ulong _a = 0;
        ulong.TryParse(a, out _a);
        return _a;
    }


    public static string TansString(ulong o)
    {
        return o.ToString();
    }

    public static string OpInit64(string a, string b, int op)
    {
        ulong _a = 0;
        ulong _b = 0;
        ulong.TryParse(a, out _a);
        ulong.TryParse(b, out _b);
        ulong result = 0;
        switch (op)
        {
            case 0: result = _a + _b; break;
            case 1: result = _a - _b; break;
            case 2: result = _a * _b; break;
            case 3: result = _a / _b; break;
            case 4: result = _a % _b; break;
        }
        return result.ToString();
    }

    public static void PrintBytes(byte[] bytes)
    {
        PrintBytes("LUA", bytes);
    }
    public static void PrintBytes(string tag, byte[] bytes,int length)
    {
#if LuaDebug
        StringBuilder sb = new StringBuilder(tag.ToUpper());
        sb.Append(" length:");
        sb.Append(bytes.Length.ToString());
        sb.Append(" =>");
        for (int i = 0; i < bytes.Length&&i<length; i++)
        {
            sb.Append(bytes[i]);
            sb.Append(" ");
        }
        Debug.Log(sb.ToString());
#endif
    }
    public static void PrintBytes(string tag, byte[] bytes)
    {
#if LuaDebug
        StringBuilder sb = new StringBuilder(tag.ToUpper());
        sb.Append(" length:");
        sb.Append(bytes.Length.ToString());
        sb.Append(" =>");
        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i]);
            sb.Append(" ");
        }
        Debug.Log(sb.ToString());
#endif
    }



}
