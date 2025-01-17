#if !DISABLE_PLUGIN


using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XUtliPoolLib;

public class BroadcastManager : MonoBehaviour, IBroardcast
{


    private static string m_token = "", m_openId = "", m_appid = "";
    private static QGameKit.LoginPlatform m_platf = QGameKit.LoginPlatform.QQ;
    private static GameObject m_gameobject;
    private static QGameKit.ShareContent m_shareContent;
#if BROADCAST
    private bool isConfigOpen = true;
    private static bool enableDanku = false;
#endif

    /// <summary>
    /// 初始化
    /// </summary>
    public void ToStart()
    {
#if BROADCAST
        QGameKit.CaptureType captureType = QGameKit.CaptureType.AudioApolloVoice | QGameKit.CaptureType.VideoCapture;
#if Release || Publish
        QGameKit.Setup("1105309683", "203090", captureType, UserAccountDelegate, QGameKit.Environment.Release);
#else
        QGameKit.Setup("1105309683", "203090", captureType, UserAccountDelegate, QGameKit.Environment.Release);
#endif
        if (!enableDanku)
        {
            QGameKit.SetDanmakuEnabled(true);
            enableDanku = true;
        }
        QGameKit.SetLogDelegate(LogDelegate);
        QGameKit.SetCommentReceiveDelegate(CommentReceiveDelegate);
        QGameKit.SetLiveStatusDelegate(LiveStatusChangedDelegate);
        QGameKit.SetShareDelegate(ShareDelegate);
        QGameKit.SetErrorCodeDelegate(ErrorCodeListenerDelegate);
#endif
    }


    void OnDestroy()
    {
        TearDown();
    }


    /// <summary>
    /// qgamekit很多回调都是在多线程里实现的
    /// 不能直接在回调中调用unity的API
    /// </summary>
    private void Update()
    {
        if (m_gameobject != null && m_shareContent != null)
        {
            DoSahre();
            m_shareContent = null;
        }
    }


    public static QGameKit.UserAccount UserAccountDelegate()
    {
        QGameKit.UserAccount account = new QGameKit.UserAccount();
        account.platform = m_platf; // QGameKit.LoginPlatform.WeChat
        account.appId = m_appid; // ⼿手Q或微信开放平台申请登录能⼒力时分配的 AppID
        account.id = m_openId; // 请赋值为⼿手Q或微信登录后获得的 OpenID
        account.token = m_token; // 请赋值为⼿手Q或微信登录后获得的AccessToken
                                 // Debug.Log("UserAccountDelegate platf: " + m_platf + " openid: " + m_openId + " token: " + m_token);
        return account; // 将⽤用户信息返回给 SDK
    }


    public bool IsBroadState()
    {
#if BROADCAST
        return true && isConfigOpen;
#else
        return false;
#endif
    }

    public bool ShowCamera(bool show)
    {
        Debug.Log("showcamera:" + show);
#if BROADCAST
        if (QGameKit.IsSupportCamera() && isConfigOpen)
        {
            if (show)
            {
                return QGameKit.ShowCamera();
            }
            else
            {
                QGameKit.HideCamera();
                return true;
            }
        }
        else
        {
            Hotfix.LuaShowSystemTip("系统版本太低，不支持开启摄像头");
            return false;
        }
#else
        return false;
#endif
    }

    public void SetAccount(int platf, string openid, string token)
    {
#if BROADCAST
        m_gameobject = gameObject;
        int open = PlayerPrefs.GetInt("BroadcastOpen");
        isConfigOpen = open == 1;
        if (isConfigOpen)
        {
            m_openId = openid;
            m_token = token;
            if (platf == 3)
            {
                m_platf = QGameKit.LoginPlatform.QQ;
                m_appid = "1105309683";
            }
            else if (platf == 4)
            {
                m_platf = QGameKit.LoginPlatform.WeChat;
                m_appid = "wxfdab5af74990787a";
            }
            else
            {
                m_platf = QGameKit.LoginPlatform.Guest;
                m_appid = "1105309683";
            }
            ToStart();
        }
#endif
    }


    /// <summary>
    /// 反初始化 
    /// </summary>
    public void TearDown()
    {
#if BROADCAST
        if (isConfigOpen && !string.IsNullOrEmpty(m_openId))
        {
            Debug.Log("tear down!");
            QGameKit.TearDown();
        }
#endif
    }

    /// <summary>
    /// 开始直播
    /// </summary>
    public void StartLiveBroadcast(string title, string desc)
    {
#if BROADCAST
        if (isConfigOpen)
        {
            Debug.Log("Start live broadcast");
            QGameKit.LiveStatus code = (QGameKit.LiveStatus)QGameKit.GetErrorCode();
            if (code == QGameKit.LiveStatus.Error)
            {
                QGameKit.Reset();
            }
            QGameKit.StartLiveBroadcast(title, desc);
        }
#endif
    }


    /// <summary>
    /// 停止直播
    /// </summary>
    public void StopBroadcast()
    {
#if BROADCAST
        if (isConfigOpen)
        {
            Debug.Log("stop live broadcast!");
            QGameKit.StopLiveBroadcast();
        }
#endif
    }

    /// <summary>
    /// 获取当前状态
    /// </summary>
    public int GetState()
    {
#if BROADCAST
        if (isConfigOpen)
        {
            QGameKit.LiveStatus code = (QGameKit.LiveStatus)QGameKit.GetErrorCode();
            Debug.Log("GetState " + code);
            return (int)code;
        }
        else return 0;
#else
        return 0;
#endif
    }


    /// <summary>
    /// 进入直播大厅
    /// </summary>
    public void EnterHall()
    {
#if BROADCAST
        if (isConfigOpen)
        {
            Debug.Log("Hall");
            QGameKit.EnterLiveHall();
        }
#endif
    }

    // 游戏监听并处理状态变化
    public static void LiveStatusChangedDelegate(QGameKit.LiveStatus newState)
    {
        // Debug.Log("live status: " + newState);
        if (m_uiUtility == null || m_uiUtility.Deprecated)
            m_uiUtility = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
        if (newState == QGameKit.LiveStatus.LiveStarting || newState == QGameKit.LiveStatus.LiveStarted)
        {
            m_uiUtility.StartBroadcast(true);
            ApolloManager.sington.Capture(true);
        }
        else if (newState == QGameKit.LiveStatus.LiveStopping || newState == QGameKit.LiveStatus.LiveStopped)
        {
            m_uiUtility.StartBroadcast(false);
            ApolloManager.sington.Capture(false);
        }
    }

    public static void ErrorCodeListenerDelegate(int errorCode, string errorMessage)
    {
        // Debug.Log("ErrorCodeListenerDelegate...errorCode=" + errorCode + ", errorMessage=" + errorMessage);
    }

    public static void LogDelegate(string log)
    {
        Debug.Log(log);
    }

    static IUiUtility m_uiUtility;
    public static void CommentReceiveDelegate(List<QGameKit.LiveComment> comments)
    {
        //if (m_uiUtility == null || m_uiUtility.Deprecated)
        //    m_uiUtility = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
        //for (int i = 0; i < comments.Count; i++)
        //{
        //    // Debug.Log("comment index " + i);
        //    QGameKit.LiveComment comment = comments[i];
        //    if (m_uiUtility != null) m_uiUtility.PushBarrage(UTF8String(comment.nick), UTF8String(comment.content));

        //}
    }


    public static string UTF8String(string input)
    {
        UTF8Encoding utf8 = new UTF8Encoding();
        return utf8.GetString(utf8.GetBytes(input));
    }

    public static void ShareDelegate(QGameKit.ShareContent shareContent)
    {
        //Debug.Log("ShareContent title " + shareContent.title);
        m_shareContent = shareContent;
    }


    private void DoSahre()
    {
        XPlatform platf = m_gameobject.GetComponent<XPlatform>();
        Dictionary<string, string> jsondata = new Dictionary<string, string>();
        jsondata["scene"] = "Session";
        jsondata["targetUrl"] = m_shareContent.targetUrl;
        jsondata["imageUrl"] = m_shareContent.imageUrl;
        jsondata["title"] = m_shareContent.title;
        jsondata["description"] = m_shareContent.description;
        jsondata["summary"] = "";
        string json = MiniJSON.Json.Serialize(jsondata);
        platf.SendGameExData("share_send_to_struct_qq", json);
    }


#if UNITY_EDITOR
    void OnGUI()
    {
#if BROADCAST_TEST
        if (GUI.Button(new Rect(20, 20, 140, 60), "Start"))
        {
            StartLiveBroadcast("龙之谷dragonnest", "I just want to test for broadcast...");
        }
        if (GUI.Button(new Rect(20, 100, 140, 60), "Stop"))
        {
            StopBroadcast();
        }
        if (GUI.Button(new Rect(20, 180, 140, 60), "Reset"))
        {
            Debug.Log("reset live broadcast!");
            QGameKit.Reset();
        }
        if (GUI.Button(new Rect(20, 260, 140, 60), "Hall"))
        {
            EnterHall();
        }
        if (GUI.Button(new Rect(20, 340, 140, 60), "HallInGame"))
        {
            Debug.Log("HallInGame");
            QGameKit.EnterLiveHallInGame();
        }
        if (GUI.Button(new Rect(20, 420, 140, 60), "State"))
        {
            GetState();
        }
        if (GUI.Button(new Rect(20, 500, 140, 60), "TearDown"))
        {
            TearDown();
        }
#endif
    }
#endif

}

#endif
