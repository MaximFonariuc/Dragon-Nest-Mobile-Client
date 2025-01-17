using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using XUtliPoolLib;

public class TssSDKManager : MonoBehaviour, ITssSdk
{

    public static TssSDKManager sington = null;

    public readonly uint gameId = 2601;
    
    public new readonly string tag = "TssSDKManager=>";

    /// <summary>
    /// 网络同步2秒同步一次
    /// </summary>
    public readonly uint sync = 2;

    private bool isLogin = false;

    void Awake()
    {
        sington = this;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void Start()
    {
#if TSS
        Debug.Log(tag + " Tss init");
        TssSdk.TssSdkInit(gameId);
#endif

        m_last = Time.time;

       
    }

    void OnDestroy()
    {
        sington = null;
    }


    float m_last = 0;
    void Update()
    {
        if (isLogin)
        {
            if (Time.time - m_last > sync)
            {
                m_last = Time.time;
                StartSendDataToSvr();
            }
        }
    }


    /// <summary>
    /// 当成功登录
    /// </summary>
    /// <param name="platf">1是internal 2是盛大 3是QQ 4是微信 max是其他</param>
    /// <param name="openId"></param>
    /// <param name="worldId">游戏分区</param>
    /// <param name="roleId">角色id</param>
    public void OnLogin(int platf, string openId, uint worldId, string roleId)
    {
#if TSS
        isLogin = true;
        Debug.Log(tag + " plat: " + platf + "openid:" + openId + " worldid: " + worldId + " roleid: " + roleId);
        if (platf == 3)
        {
            TssSdk.TssSdkSetUserInfoEx(TssSdk.EENTRYID.ENTRY_ID_QQ, openId, "1105309683", worldId, roleId);
        }
        else if (platf == 4)
        {
            TssSdk.TssSdkSetUserInfoEx(TssSdk.EENTRYID.ENTRY_ID_WX, openId, "wxfdab5af74990787a", worldId, roleId);
        }
        else
        {
            TssSdk.TssSdkSetUserInfoEx(TssSdk.EENTRYID.ENTRY_ID_OTHERS, openId, "guest100023", worldId, roleId);
        }
#endif
       
        //byte[] bytes = new byte[] { 0x12, 0x23, 0x23, 0x23, 0x23, 0x23, 0x23, 0x23, 0x23, 0x23 };
        //Hotfix.PrintBytes("start:" ,bytes);
        //ITssSdkSend itss = XInterfaceMgr.singleton.GetInterface<ITssSdkSend>(XCommon.singleton.XHash("ITssSdkSend"));
        //if (itss != null) itss.SendDataToServer(bytes, (uint)(bytes.Length));
    }

    /// <summary>
    /// 游戏从前台后台切换
    /// </summary>
    /// <param name="pause"></param>
    void OnApplicationPause(bool pause)
    {
#if TSS
        Debug.Log(tag + " puase: " + pause);
        if (pause)
        {
            TssSdk.TssSdkSetGameStatus(TssSdk.EGAMESTATUS.GAME_STATUS_BACKEND);
        }
        else
        {
            TssSdk.TssSdkSetGameStatus(TssSdk.EGAMESTATUS.GAME_STATUS_FRONTEND);
        }
#endif
    }


    /// <summary>
    /// 上报数据
    /// </summary>
    public void StartSendDataToSvr()
    {
#if TSS
        IntPtr addr = TssSdk.tss_get_report_data();
        if (addr != IntPtr.Zero)
        {
            TssSdk.AntiDataInfo info = new TssSdk.AntiDataInfo();
            if (TssSdk.Is64bit())
            {
                short anti_data_len = Marshal.ReadInt16(addr, 0);
                Int64 anti_data = Marshal.ReadInt64(addr, 2);
                info.anti_data_len = (ushort)anti_data_len;
                info.anti_data = new IntPtr(anti_data);
            }
            else if (TssSdk.Is32bit())
            {
                short anti_data_len = Marshal.ReadInt16(addr, 0);
                Int64 anti_data = Marshal.ReadInt32(addr, 2);
                info.anti_data_len = (ushort)anti_data_len;
                info.anti_data = new IntPtr(anti_data);
            }
            else
            {
                Debug.LogError(tag+" TSSSDK NO INT TYPE");
            }
            if (SendDataToSvr(info))
            {
                TssSdk.tss_del_report_data(addr);
            }
        }
        else
        {
           // Debug.Log(tag + "addr is nil!");
        }
#endif
    }


    private bool SendDataToSvr(TssSdk.AntiDataInfo info)
    {
        byte[] data = new byte[info.anti_data_len];
        Marshal.Copy(info.anti_data, data, 0, info.anti_data_len);
        return DoSendDataToSvr(data, info.anti_data_len);
    }


    private bool DoSendDataToSvr(byte[] data, uint length)
    {
#if TSS
        //send data to server
        Hotfix.PrintBytes("Send "+tag, data);
        ITssSdkSend itss = XInterfaceMgr.singleton.GetInterface<ITssSdkSend>(XCommon.singleton.XHash("ITssSdkSend"));
        if (itss != null) itss.SendDataToServer(data, length);
#endif
        return true;
    }


    /// <summary>
    /// 由服务器调用
    /// </summary>
    public void OnRcvWhichNeedToSendClientSdk(byte[] data, uint length)
    {
//        Hotfix.PrintBytes("rcv:", data);
#if TSS
        Hotfix.PrintBytes("RCV "+tag, data);
        TssSdk.TssSdkRcvAntiData(data, (ushort)length);
#endif
    }


}
