#if !DISABLE_PLUGIN
using UnityEngine;
using System.Collections;
using System;
using XUtliPoolLib;
//using com.tencent.gsdk;
using System.IO;
using System.Text;

public class ApolloManager : MonoBehaviour, IApolloManager
{
#if APOLLO
    private static CApolloVoiceSys ApolloVoiceMgr = null;
    static bool m_bCreateEngine = false;
    private string mAppID = string.Empty;
    private string mOpenID = string.Empty;
#endif

    private bool _openMusic = true;
    private bool _openSpeak = true;



    public static ApolloManager sington = null;
    public static byte[] m_strFileID = new byte[1024];


    void Awake()
    {
        sington = this;
    }

    void OnDestroy()
    {
        sington = null;
    }

    void Start()
    {
       // InitGSDK("1105309683");
    }

    /// <summary>
    /// sdk speak表示打开或者关闭其他玩家说话
    /// </summary>
    public bool openMusic
    {
        get { return _openMusic; }
        set
        {
            _openMusic = value;
#if APOLLO
            ApolloVoiceErr VoiceErr = value ? (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._OpenSpeaker()
                : (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._CloseSpeaker();
            if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
            {
                m_bCreateEngine = true;
                XDebug.singleton.AddLog(string.Format("openMusic Succ {0}", value));
            }
            else
            {
                string text = string.Format("openMusic Err is {0}", VoiceErr);
                Debug.Log(text);
            }
#endif
        }
    }


    /// <summary>
    /// sdk music代表打开或者关闭自己说话
    /// </summary>
    public bool openSpeak
    {
        get { return _openSpeak; }
        set
        {
            _openSpeak = value;
#if APOLLO
            ApolloVoiceErr VoiceErr = value ? (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._OpenMic()
              : (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._CloseMic();
            if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
            {
                m_bCreateEngine = true;
                XDebug.singleton.AddLog(string.Format("openSpeak Succ {0}", value));
            }
            else
            {
                string text = string.Format("openSpeak Err is {0}", VoiceErr);
                Debug.Log(text);
            }
#endif
        }
    }


    public void Init(int platf, string openid)
    {
#if APOLLO
        mAppID = "guest100023";
#if UNITY_EDITOR
        mAppID = "1105309683";
        openid = "63662733";
#endif
        if (platf == 3) mAppID = "1105309683";
        else if (platf == 4) mAppID = "wxfdab5af74990787a";
        mOpenID = openid;

        //SetGSDKUserName(platf, openid);


        if (ApolloVoiceMgr != null)
        {
            Debug.Log("ApolloVoiceMgr Created");
        }
        else
        {
            ApolloVoiceMgr = new CApolloVoiceSys();
            ApolloVoiceMgr.SysInitial();
        }
        if (ApolloVoiceMgr != null)
        {
            ApolloVoiceErr VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._CreateApolloVoiceEngine(mAppID, openid);
            if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
            {
                m_bCreateEngine = true;
                XDebug.singleton.AddLog("CreateApolloVoiceEngine Succ");
            }
            else
            {
                string text = string.Format("CreateApolloVoiceEngine Err is {0}", VoiceErr);
                Debug.Log(text);
            }
        }
#if Publish
         ApolloVoiceMgr.CallApolloVoiceSDK._EnableLog(false);
#endif

#endif
    }


    public void SetRealtimeMode()
    {
#if APOLLO
        if (ApolloVoiceMgr != null)
        {
            ApolloVoiceErr VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._SetMode(0);

            if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
            {
                m_bCreateEngine = true;
                XDebug.singleton.AddLog("apollo _SetMode Succ");
            }
            else
            {
                string text = string.Format("apollo _SetMode Err is {0}", VoiceErr);
                Debug.Log(text);
            }
        }
#endif
    }


    public void Capture(bool capture)
    {
#if APOLLO&&!UNITY_EDITOR && !UNITY_STANDALONE_WIN
        ApolloVoiceErr VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._CaptureMicrophone(capture);
        if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
        {
            m_bCreateEngine = true;
            Debug.Log("apollo capture Succ");
        }
        else
        {
            string text = string.Format("apollo capture Err is {0}", VoiceErr);
            Debug.Log(text);
        }
#endif
    }


    /// <summary>
    /// 禁止成员声音
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="bEnable"></param>
    public void ForbitMember(int memberId, bool bEnable)
    {
#if APOLLO
        if (ApolloVoiceMgr != null)
        {
            ApolloVoiceErr VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._ForbidMemberVoice(memberId, bEnable);
            if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
            {
                m_bCreateEngine = true;
                XDebug.singleton.AddLog("ForbitMember Succ");
            }
            else
            {
                string text = string.Format("ForbitMember Err is {0}", VoiceErr);
                Debug.Log(text);
            }
        }
#endif
    }

    /// <summary>
    /// 获取说话成员信息
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public int[] GetMembersState(ref int size)
    {
        int[] memberState = new int[32];
#if APOLLO
        size = ApolloManager.ApolloVoiceMgr.CallApolloVoiceSDK._GetMemberState(memberState);
#endif
        return memberState;
    }

    /// <summary>
    /// 加入小房间
    /// </summary>
    public void JoinRoom(string url1, string url2, string url3, Int64 roomId, Int64 roomKey, short memberId)
    {
#if APOLLO
        if (string.IsNullOrEmpty(mOpenID)) { Debug.LogError("openid is nil"); return; }
        ApolloVoiceErr VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._JoinRoom(url1, url2, url3, roomId, roomKey, memberId, mOpenID, 45000);
        if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
        {
            m_bCreateEngine = true;
            XDebug.singleton.AddLog(string.Format("JoinRoom Succ {0}  {1}  {2} {3} {4} {5}", roomId, roomKey, memberId, url1, url2, url3));
        }
        else
        {
            string text = string.Format("JoinRoom Err is {0}", VoiceErr);
            Debug.Log(text);
        }
#endif
    }

    /// <summary>
    /// 加入大房间
    /// </summary>
    /// <param name="urls">大房间url</param>
    /// <param name="role">0 主播(最多五人), 1 观众</param>
    /// <param name="busniessID">后台传递过来的业务ID</param>
    public void JoinBigRoom(string urls, int role, uint busniessID, Int64 roomid, Int64 roomkey, short memberid)
    {
#if APOLLO
        if (string.IsNullOrEmpty(mOpenID)) { Debug.LogError("openid is nil"); return; }
        ApolloVoiceErr VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._JoinBigRoom(urls, role, busniessID, roomid, roomkey, memberid, mOpenID, 45000);
        if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
        {
            m_bCreateEngine = true;
            XDebug.singleton.AddLog(string.Format("_JoinBigRoom Succ {0}  {1}  {2} {3} {4} {5}", roomid, roomkey, urls, role, busniessID, memberid));
        }
        else
        {
            string text = string.Format("_JoinBigRoom Err is {0}", VoiceErr);
            Debug.Log(text);
        }
#endif
    }


    public bool GetJoinRoomResult()
    {
        bool isInRoom = false;
#if APOLLO
        if (ApolloVoiceMgr != null)
        {
            ApolloVoiceErr VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._GetJoinRoomResult();
            if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_JOIN_SUCC)
            {
                isInRoom = true;
                XDebug.singleton.AddLog("Apollo GetJoinRoomResult Succ");
            }
            else
            {
                string text = string.Format("Apollo GetJoinRoomResult is {0}", VoiceErr);
                Debug.Log(text);
            }
        }
#endif
        return isInRoom;
    }


    public bool GetJoinRoomBigResult()
    {
        bool isInRoom = false;
#if APOLLO
        if (ApolloVoiceMgr != null)
        {
            ApolloVoiceErr VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._GetJoinRoomBigResult();
            if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_JOIN_SUCC)
            {
                isInRoom = true;
                XDebug.singleton.AddLog("Apollo _GetJoinRoomBigResult Succ");
            }
            else
            {
                string text = string.Format("Apollo _GetJoinRoomBigResult is {0}", VoiceErr);
                XDebug.singleton.AddLog(text);
            }
        }
#endif
        return isInRoom;
    }


    /// <summary>
    /// 退出小房间
    /// </summary>
    /// <param name="roomId"></param>
    /// <param name="memberId"></param>
    public void QuitRoom(Int64 roomId, short memberId)
    {
#if APOLLO
        if (string.IsNullOrEmpty(mOpenID)) { Debug.LogError("openid is nil"); return; }
        ApolloVoiceErr VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._QuitRoom(roomId, memberId, mOpenID);
        if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
        {
            m_bCreateEngine = true;
            XDebug.singleton.AddLog(string.Format("QuitRoom Succ {0} {1}", roomId, memberId));
        }
        else
        {
            string text = string.Format("QuitRoom Err is {0}", VoiceErr);
            XDebug.singleton.AddLog(text);
        }
#endif
    }

    /// <summary>
    /// 退出大房间
    /// </summary>
    public void QuitBigRoom()
    {
#if APOLLO
        if (string.IsNullOrEmpty(mOpenID)) { Debug.LogError("openid is nil"); return; }
        ApolloVoiceErr VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._QuitBigRoom();
        if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
        {
            m_bCreateEngine = true;
            XDebug.singleton.AddLog(string.Format("QuitBigRoom!"));
        }
        else
        {
            string text = string.Format("QuitBigRoom Err is {0}", VoiceErr);
            XDebug.singleton.AddLog(text);
        }
#endif
    }


    /// <summary>
    /// 获取麦克音量
    /// </summary>
    public int GetSpeakerVolume()
    {
#if APOLLO
        if (ApolloVoiceMgr != null)
        {
            return ApolloVoiceMgr.CallApolloVoiceSDK._GetMicLevel();
        }
#endif
        return 100;
    }


    /// <summary>
    /// 设置背景音乐的大小
    /// </summary>
    /// <param name="nVol"></param>
    public void SetMusicVolum(int nVol)
    {
#if APOLLO
        if (ApolloVoiceMgr != null)
        {
            ApolloVoiceErr VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._SetSpeakerVolume(nVol);
            if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
            {
                m_bCreateEngine = true;
                XDebug.singleton.AddLog(string.Format("SetMusicVolum Succ {0}", nVol));
            }
            else
            {
                string text = string.Format("SetMusicVolum Err is {0}", VoiceErr);
                XDebug.singleton.AddLog(text);
            }
        }
#endif
    }


    void OnApplicationPause(bool pause)
    {
#if APOLLO
        XDebug.singleton.AddLog("apollo puase: " + pause + " mgr: " + (ApolloVoiceMgr != null));
        if (ApolloVoiceMgr != null)
        {
            if (pause)
            {
                ApolloVoiceErr VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._Pause();
                if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
                {
                    m_bCreateEngine = true;
                    XDebug.singleton.AddLog(string.Format("OnApplicationPause pause true Succ"));
                }
                else
                {
                    string text = string.Format("OnApplicationPause Err is {0}", VoiceErr);
                    Debug.Log(text);
                }
            }
            else
            {
                ApolloVoiceErr VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._Resume();
                if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
                {
                    m_bCreateEngine = true;
                    Debug.Log(string.Format("OnApplicationPause pause false Succ"));
                }
                else
                {
                    string text = string.Format("OnApplicationPause Err is {0}", VoiceErr);
                    Debug.Log(text);
                }
            }
        }
#endif
    }



    public int InitApolloEngine(int ip1, int ip2, int ip3, int ip4, byte[] key, int len)
    {
#if APOLLO
        //int res = ApolloVoiceMgr.CallApolloVoiceSDK._SetMode(2);
        //if (res != 0)
        //    return res;

        ApolloVoiceMgr.CallApolloVoiceSDK._SetServiceInfo(ip1, ip2, ip3, ip4, 80, 60000);
        return ApolloVoiceMgr.CallApolloVoiceSDK._SetAuthkey(key, len);
#else
        return 0;
#endif
    }


    public int StartRecord(string filename)
    {
#if APOLLO
        string path = Application.persistentDataPath + "/" + filename;
        return ApolloVoiceMgr.CallApolloVoiceSDK._StartRecord(path);
#else
        return 0;
#endif
    }

    public int StopApolloRecord()
    {
#if APOLLO
        return ApolloVoiceMgr != null ? ApolloVoiceMgr.CallApolloVoiceSDK._StopRecord(true) : 0;
#else
        return 0;
#endif
    }

    public int GetApolloUploadStatus()
    {
#if APOLLO
        return ApolloVoiceMgr != null ? ApolloVoiceMgr.CallApolloVoiceSDK._GetVoiceUploadState() : 0;
#else
        return 0;
#endif
    }

    public int UploadRecordFile(string filename)
    {
#if APOLLO
#if UNITY_EDITOR
        string path = Application.persistentDataPath + "/record.sound";
        return 0;
#else
        string path = Application.persistentDataPath + "/" + filename;
        byte[] filebyte = File.ReadAllBytes(path);
        return ApolloVoiceMgr.CallApolloVoiceSDK._SendRecFile(path);
#endif

#else
        return 0;
#endif
    }
    public string GetFileID()
    {
#if APOLLO
        ApolloVoiceErr VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._GetVoiceUploadState();

        if (VoiceErr != ApolloVoiceErr.APOLLO_VOICE_SUCC)
        {
            return "";
        }

        VoiceErr = (ApolloVoiceErr)ApolloVoiceMgr.CallApolloVoiceSDK._GetFileID(m_strFileID);
        if (VoiceErr == ApolloVoiceErr.APOLLO_VOICE_SUCC)
        {
            return Encoding.Default.GetString(m_strFileID);
        }
        else
        {
            return "";
        }
#else
        return "";
#endif
    }

    public int GetMicLevel()
    {
#if APOLLO
        return ApolloVoiceMgr != null ? ApolloVoiceMgr.CallApolloVoiceSDK._GetMicLevel() : 0;
#else
        return 0;
#endif
    }

    public int StartPlayVoice(string filepath)
    {
#if APOLLO
        return ApolloVoiceMgr != null ? ApolloVoiceMgr.CallApolloVoiceSDK._PlayFile(filepath) : 0;
#else
        return 0;
#endif
    }

    public int StopPlayVoice()
    {
#if APOLLO
        return ApolloVoiceMgr != null ? ApolloVoiceMgr.CallApolloVoiceSDK._StopPlayFile() : 0;
#else
        return 0;
#endif
    }

    public int SetApolloMode(int mode)
    {
#if APOLLO
        return ApolloVoiceMgr != null ? ApolloVoiceMgr.CallApolloVoiceSDK._SetMode(mode) : 0;
#else
        return 0;
#endif
    }
}
#endif
