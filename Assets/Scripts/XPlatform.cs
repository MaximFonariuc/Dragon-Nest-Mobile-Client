using System;
using UnityEngine;
using XUtliPoolLib;
using System.Text;
using System.Collections;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using Unity.Performance;
using Unity.AutoTune;

#if !DISABLE_JOYSDK
using Assets.SDK;
using FMODUnity;
#endif
public class XPlatform : MonoBehaviour, IPlatform
{
#if !DISABLE_JOYSDK
    private JoyYouSDK _interface;
#endif

#if Publish
    private static string _defaultLoginServer = @"qq.lzgjx.qq.com:10004";
    private static string _androidQQLoginServer = @"qq.lzgjx.qq.com:10004";
    private static string _androidWeChatLoginServer = @"wx.lzgjx.qq.com:10002";
    private static string _iOSQQLoginServer = @"qq.lzgjx.qq.com:10003";
    private static string _iOSWeChatLoginServer = @"wx.lzgjx.qq.com:10001";
    private static string _iOSGuestLoginServer = @"wx.lzgjx.qq.com:10001";

    private static string _versionServer = @"wx.lzgjx.qq.com:10000";

    private static string _hostUrl = @"https://image.lzgjx.qq.com/Official/";
    private static bool _isPublish = true;

    private static string _testDefaultLoginServer = @"test.lzgjx.qq.com:9640";
    private static string _testAndroidQQLoginServer = @"test.lzgjx.qq.com:9640";
    private static string _testAndroidWeChatLoginServer = @"test.lzgjx.qq.com:9620";
    private static string _testiOSQQLoginServer = @"test.lzgjx.qq.com:9630";
    private static string _testiOSWeChatLoginServer = @"test.lzgjx.qq.com:9610";
    private static string _testiOSGuestLoginServer = @"test.lzgjx.qq.com:9610";

    private static string _testVersionServer = @"test.lzgjx.qq.com:9700";
#elif QA_TEST
      private static string _defaultLoginServer = @"123.206.102.160:26060";
    private static string _androidQQLoginServer = @"123.206.102.160:26060";
    private static string _androidWeChatLoginServer = @"123.206.102.160:26080";
    private static string _iOSQQLoginServer = @"123.206.102.160:26050";
    private static string _iOSWeChatLoginServer = @"123.206.102.160:26070";
    private static string _iOSGuestLoginServer = @"123.206.102.160:26050";

    private static string _versionServer = @"123.206.102.160:27000";

    private static string _hostUrl = @"https://image.lzgjx.qq.com/QATest/";
    private static bool _isPublish = false;

    private static string _testDefaultLoginServer = @"127.0.0.1:25001";
    private static string _testAndroidQQLoginServer = @"127.0.0.1:25001";
    private static string _testAndroidWeChatLoginServer = @"127.0.0.1:25001";
    private static string _testiOSQQLoginServer = @"127.0.0.1:25001";
    private static string _testiOSWeChatLoginServer = @"127.0.0.1:25001";
    private static string _testiOSGuestLoginServer = @"127.0.0.1:25001";

    private static string _testVersionServer = @"127.0.0.1:24001";
#elif PANDORA_TEST
      private static string _defaultLoginServer = @"123.206.102.160:26050";
    private static string _androidQQLoginServer = @"123.206.102.160:26050";
    private static string _androidWeChatLoginServer = @"123.206.102.160:26050";
    private static string _iOSQQLoginServer = @"123.206.102.160:26050";
    private static string _iOSWeChatLoginServer = @"123.206.102.160:26050";
    private static string _iOSGuestLoginServer = @"123.206.102.160:26050";

    private static string _versionServer = @"123.206.102.160:27000";

    private static string _hostUrl = @"https://image.lzgjx.qq.com/QATest/";
    private static bool _isPublish = false;

    private static string _testDefaultLoginServer = @"127.0.0.1:25001";
    private static string _testAndroidQQLoginServer = @"127.0.0.1:25001";
    private static string _testAndroidWeChatLoginServer = @"127.0.0.1:25001";
    private static string _testiOSQQLoginServer = @"127.0.0.1:25001";
    private static string _testiOSWeChatLoginServer = @"127.0.0.1:25001";
    private static string _testiOSGuestLoginServer = @"127.0.0.1:25001";

    private static string _testVersionServer = @"127.0.0.1:24001";
#elif RECHARGE_TEST
    private static string _defaultLoginServer = @"127.0.0.1:25001";
    private static string _androidQQLoginServer = @"127.0.0.1:25001";
    private static string _androidWeChatLoginServer = @"127.0.0.1:25001";
    private static string _iOSQQLoginServer = @"127.0.0.1:25001";
    private static string _iOSWeChatLoginServer = @"127.0.0.1:25001";
    private static string _iOSGuestLoginServer = @"127.0.0.1:25001";

    private static string _versionServer = @"127.0.0.1:24001";

    private static string _hostUrl = @"https://image.lzgjx.qq.com/Test/";
    private static bool _isPublish = false;

    private static string _testDefaultLoginServer = @"127.0.0.1:25001";
    private static string _testAndroidQQLoginServer = @"127.0.0.1:25001";
    private static string _testAndroidWeChatLoginServer = @"127.0.0.1:25001";
    private static string _testiOSQQLoginServer = @"127.0.0.1:25001";
    private static string _testiOSWeChatLoginServer = @"127.0.0.1:25001";
    private static string _testiOSGuestLoginServer = @"127.0.0.1:25001";

    private static string _testVersionServer = @"127.0.0.1:24001";
#else
    private static string _defaultLoginServer = @"127.0.0.1:25001";
    private static string _androidQQLoginServer = @"127.0.0.1:25001";
    private static string _androidWeChatLoginServer = @"127.0.0.1:25001";
    private static string _iOSQQLoginServer = @"127.0.0.1:25001";
    private static string _iOSWeChatLoginServer = @"127.0.0.1:25001";
    private static string _iOSGuestLoginServer = @"127.0.0.1:25001";

    private static string _versionServer = @"127.0.0.1:24001";

    private static string _hostUrl = @"https://image.lzgjx.qq.com/Test/";
    private static bool _isPublish = false;

    private static string _testDefaultLoginServer = @"127.0.0.1:25001";
    private static string _testAndroidQQLoginServer = @"127.0.0.1:25001";
    private static string _testAndroidWeChatLoginServer = @"127.0.0.1:25001";
    private static string _testiOSQQLoginServer = @"127.0.0.1:25001";
    private static string _testiOSWeChatLoginServer = @"127.0.0.1:25001";
    private static string _testiOSGuestLoginServer = @"127.0.0.1:25001";

    private static string _testVersionServer = @"127.0.0.1:24001";
#endif

    private static bool _isTestMode = false;
    private int m_autoTuneGroup = -1;
    private bool m_apmInit = false;
    private INativePlugin m_NativePlugin = null;
    public static string UrlConfig
    {
        get
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}",
                _defaultLoginServer, _androidQQLoginServer, _androidWeChatLoginServer,
                _iOSQQLoginServer, _iOSWeChatLoginServer, _iOSGuestLoginServer,
                _versionServer, _hostUrl, _isPublish,
                _testDefaultLoginServer, _testAndroidQQLoginServer, _testAndroidWeChatLoginServer,
                _testiOSQQLoginServer, _testiOSWeChatLoginServer, _testiOSGuestLoginServer,
                _testVersionServer);
        }
    }

    public bool Deprecated
    {
        get;
        set;
    }

    void Awake()
    {
#if !DISABLE_JOYSDK
        _interface = new JoyYouSDK();
#endif

        string _localConfig = GetLocalConfigPath();
        string _cacheConfig = string.Format("{0}/config.cfg", Application.persistentDataPath);
        if (File.Exists(_cacheConfig))
            LoadConfig(_cacheConfig);
        else
            LoadConfig(_localConfig);

        _isTestMode = File.Exists(Path.Combine(Application.persistentDataPath, "TEST_VERSION"));

        Debug.Log("ChannelID : " + GetChannelID());

#if USE_WETEST
        this.gameObject.AddComponent<WeTest.U3DAutomation.U3DAutomationBehaviour>();
#if BUGLY
        BuglyAgent.RegisterLogCallback(WeTest.U3DAutomation.CrashMonitor._OnLogCallbackHandler);
#endif
#endif

        AutoTune.Init("127",  // build id
                      true, // use persistent path
                      null, // defaults in case of network error the first time
#if DEBUG
                      AutoTune.Endpoint.Sandbox); // what endpoint to use
#else
                      AutoTune.Endpoint.Production); // what endpoint to use
#endif

        AutoTune.Fetch(GotAtuoTuneSettings);
    }
    void GotAtuoTuneSettings(Dictionary<string, object> settings, int group)
    {
        object value;
        if(settings.TryGetValue("quality_level", out value))
        {
            m_autoTuneGroup = (int)value;
        }
    }


    private string GetLocalConfigPath()
    {
        string filePath = null;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                    filePath = string.Format("{0}!assets/config.cfg", Application.dataPath);
                break;
            case RuntimePlatform.IPhonePlayer:
                    filePath = string.Format("{0}/Raw/config.cfg", Application.dataPath);
                break;
            default:
                    filePath = string.Format("{0}/StreamingAssets/config.cfg", Application.dataPath);
                break;
        }
        return filePath;
    }

    private void LoadConfig(string path)
    {
        AssetBundle bundle = null;
#if !UNITY_EDITOR
        bundle = AssetBundle.LoadFromFile(path);
#endif
        if (bundle == null) return;
        TextAsset text = bundle.LoadAsset(bundle.GetAllAssetNames()[0]) as TextAsset;
        string[] config = text.text.Split(new Char[] { '|' });
        bundle.Unload(true);

        _defaultLoginServer = config[0];
        _androidQQLoginServer = config[1];
        _androidWeChatLoginServer = config[2];
        _iOSQQLoginServer = config[3];
        _iOSWeChatLoginServer = config[4];
        _iOSGuestLoginServer = config[5];
        _versionServer = config[6];
        _hostUrl = config[7];
        _isPublish = bool.Parse(config[8]);
        _testDefaultLoginServer = config[9];
        _testAndroidQQLoginServer = config[10];
        _testAndroidWeChatLoginServer = config[11];
        _testiOSQQLoginServer = config[12];
        _testiOSWeChatLoginServer = config[13];
        _testiOSGuestLoginServer = config[14];
        _testVersionServer = config[15];
    }

    public void OnPlatformLogin()
    {
#if !DISABLE_JOYSDK
        ((IHuanlePlatform)_interface).ShowLoginView();
#endif
    }

    public void OnQQLogin()
    {
        //Debug.Log("interface: " + _interface.ToString());
        //Debug.Log("I3RDPlatformSDK interface: " + ((I3RDPlatformSDK)_interface).ToString());
#if !DISABLE_JOYSDK
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                ((I3RDPlatformSDK)_interface).ShowLoginView();
                break;
            case RuntimePlatform.IPhonePlayer:
                ((IHuanlePlatform)_interface).ShowLoginViewWithType(1);
                break;
        }
#endif
    }

    public void OnWeChatLogin()
    {
#if !DISABLE_JOYSDK
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                ((IHuanlePlatform)_interface).HLLogin("", "");
                break;
            case RuntimePlatform.IPhonePlayer:
                ((IHuanlePlatform)_interface).ShowLoginViewWithType(2);
                break;
        }
#endif
    }

    public void OnGuestLogin()
    {
#if !DISABLE_JOYSDK
        ((IHuanlePlatform)_interface).ShowLoginViewWithType(0);
#endif
    }

    public void LogOut()
    {
#if !DISABLE_JOYSDK
        ((IHuanlePlatform)_interface).Logout();
#endif
        PDatabase.singleton.playerInfo = null;
        PDatabase.singleton.friendsInfo = null;

    }
    public void LoginCallBack(string token)
    {
        IEntrance entrance = XInterfaceMgr.singleton.GetInterface<IEntrance>(0);

        if (entrance == null)
            XDebug.singleton.AddLog("LoginCallBack is invoked at wrong time, token: ", token);
        else
            entrance.Authorization(token);
    }

    public void LogoutCallBack(string msg)
    {
        IEntrance entrance = XInterfaceMgr.singleton.GetInterface<IEntrance>(0);

        if (entrance == null)
            XDebug.singleton.AddLog("LogoutCallBack is invoked at wrong time");
        else
            entrance.AuthorizationSignOut(msg);
    }

    public void SendGameExData(string type, string json)
    {
        //Debug.Log("SendGameExData=>" + type + " json:" + json);
#if !DISABLE_JOYSDK
        if (_interface == null)
            XDebug.singleton.AddLog("SendGameExData error!!!");
        else
            ((IHuanlePlatform)_interface).SendGameExtData(type, json);
#endif
    }

    public void UserViewClosedCallBack(string msg)
    {
        //Debug.Log("UserViewClosedCallBack=> " + msg);
        PDatabase.singleton.HandleExData(msg);
    }

    public void SetPushStatus(bool status)
    {
#if !DISABLE_JOYSDK
        if(status)
        {
            ((IHuanlePlatform)_interface).SendGameExtData("push_setting", "{'status':1}");
        }
        else
        {
            ((IHuanlePlatform)_interface).SendGameExtData("push_setting", "{'status':0}");
        }
#endif
    }

    public void SendUserInfo(uint serverID,ulong roleID)
    {
#if !DISABLE_JOYSDK
        string format = "{{ \"zone_id\":{0}, \"role_id\":\"{1}\" }}";
        string jsonData = string.Format(format, serverID, roleID);

        ((IHuanlePlatform)_interface).SendGameExtData("send_user_info", jsonData);
#endif
    }

    public string GetHostWithHttpDns(string url)
    {
        return com.tencent.httpdns.HttpDns.GetHostByName(url);
    }

    public bool CheckStatus(string type, string json)
    {
#if !DISABLE_JOYSDK
        return ((IHuanlePlatform)_interface).CheckStatus(type, json);
#else
        return false;
#endif
    }

    public string GetSDKConfig(string type,string json)
    {
#if !DISABLE_JOYSDK
        return ((IHuanlePlatform)_interface).GetSDKConfig(type, json);
#else
        return "0";
#endif
    }

    public bool CheckWeChatInstalled()
    {
        return CheckStatus("Weixin_Installed", "");
    }

    public string GetChannelID()
    {
        string filePath = Application.persistentDataPath + @"/ChannelID";
        string chanelID = "";
        if (File.Exists(filePath))
        {
            chanelID = File.ReadAllText(filePath);
        }
        else
        {
            chanelID = GetSDKConfig("get_channel_id", "");
            try
            {
                File.WriteAllText(filePath, chanelID);
                SetNoBackupFlag(filePath);
            }
            catch (System.Exception e)
            {
                Debug.Log("Save chanelID file error " + e.Message);
            }
        }

        return chanelID;
    }

    public string GetBatteryLevel()
    {
        return GetSDKConfig("get_battery_level", "");
    }

    public void ResgiterSDONotification(uint serverid, string rolename)
    {
#if UNITY_IOS
        StringBuilder token = new StringBuilder("{\"AreaId\":\"the area id\", \"RoleName\":\"the role name\"}");
        token.Replace("the area id", serverid.ToString());
        token.Replace("the role name", rolename);
#if !DISABLE_JOYSDK
        //Debug.Log("json: " + token.ToString());
        ((IHuanlePlatform)_interface).SendGameExtData("APS_UserInfo", token.ToString());
#endif
#endif
    }

    public string GetPFToken()
    {
#if Awake
        return "sdo2";
#else
        return "sdo";
#endif
    }

    public string GetVersionServer()
    {
        if (_isTestMode)
            return _testVersionServer;

        return _versionServer;
    }

    public string GetTestVersionServer()
    {
        return _testVersionServer;
    }

    public string GetHostUrl()
    {
        return _hostUrl;
    }

    public string GetLoginServer(string loginType)
    {
        if (_isTestMode)
        {
            switch (loginType)
            {
                case "Guest":
                    return _testiOSGuestLoginServer;
                case "QQ":
                    switch (Application.platform)
                    {
                        case RuntimePlatform.Android:
                            return _testAndroidQQLoginServer;
                        case RuntimePlatform.IPhonePlayer:
                            return _testiOSQQLoginServer;
                    }
                    break;
                case "WeChat":
                    switch (Application.platform)
                    {
                        case RuntimePlatform.Android:
                            return _testAndroidWeChatLoginServer;
                        case RuntimePlatform.IPhonePlayer:
                            return _testiOSWeChatLoginServer;
                    }
                    break;
            }
            return _testDefaultLoginServer;
        }

        switch (loginType)
        {
            case "Guest":
                return _iOSGuestLoginServer;
            case "QQ":
                switch(Application.platform)
                {
                    case RuntimePlatform.Android:
                        return _androidQQLoginServer;
                    case RuntimePlatform.IPhonePlayer:
                        return _iOSQQLoginServer;
                }
                break;
            case "WeChat":
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        return _androidWeChatLoginServer;
                    case RuntimePlatform.IPhonePlayer:
                        return _iOSWeChatLoginServer;
                }
                break;
        }
        return _defaultLoginServer;
    }

    public bool IsPublish()
    {
        return _isPublish;
    }

    public bool IsTestMode()
    {
        return _isTestMode;
    }

    public XPlatformType Platfrom()
    {
#if UNITY_IOS
        return XPlatformType.IOS;
#elif UNITY_ANDROID
        return XPlatformType.Android;
#else
        return XPlatformType.Standalone;
#endif
    }
    public bool IsEdior()
    {
#if UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }
    public void SetNoBackupFlag(string fullpath)
    {
#if UNITY_IOS
        UnityEngine.iOS.Device.SetNoBackupFlag(fullpath);
#endif
    }
    
    public int GetQualityLevel()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        return 4;
#else
    if (m_autoTuneGroup != -1)        
        return m_autoTuneGroup;

#if UNITY_IOS
        var iOSGen = UnityEngine.iOS.Device.generation;
        if (iOSGen == UnityEngine.iOS.DeviceGeneration.iPhoneUnknown ||
            iOSGen == UnityEngine.iOS.DeviceGeneration.iPhone7 ||
            iOSGen == UnityEngine.iOS.DeviceGeneration.iPhone7Plus ||
        iOSGen == UnityEngine.iOS.DeviceGeneration.iPhone6S ||
        iOSGen == UnityEngine.iOS.DeviceGeneration.iPhone6SPlus ||
            SystemInfo.systemMemorySize >= 2048)
        {
            return 3;
        }
        else if (SystemInfo.systemMemorySize > 1536)
        {
            return 2;
        }
        return 0;
#elif UNITY_ANDROID
        if (SystemInfo.maxTextureSize > 8192 && SystemInfo.systemMemorySize > 3192)
        {
            return 3;
        }
        if (SystemInfo.systemMemorySize > 4096 && SystemInfo.graphicsMemorySize >= 1024)
        {
            return 2;
        }  
        if (SystemInfo.systemMemorySize <= 2048 || SystemInfo.graphicsMemorySize <= 256)//1.5g
            return 0;
        if (SystemInfo.systemMemorySize <= 4096 || SystemInfo.graphicsMemorySize <= 512)//2.5g
            return 1;
        return 1;
#else
        return 2;
#endif

#endif

    }
    public void InitApm()
    {
#if USE_CUBE
        CubeApmAgent.SetAppId("APM_DGNT");
        CubeApmAgent.DisableOpts(CubeApmAgent.DC_OPT | CubeApmAgent.TRI_OPT);
        CubeApmAgent.InitContext();
        m_apmInit = true;
#endif
    }
    public void MarkLoadlevel(string scene_name)
    {
#if USE_CUBE
        if(m_apmInit)
            CubeApmAgent.MarkLoadlevel(scene_name);
#endif
        AutoTune.GetPerfRecorder().BeginExperiment(scene_name);
    }
    public void MarkLoadlevelCompleted()
    {
#if USE_CUBE
        if (m_apmInit)
            CubeApmAgent.MarkLoadlevelCompleted();
#endif
        AutoTune.GetPerfRecorder().EndExperiment();
    }
    public void MarkLevelEnd()
    {
#if USE_CUBE
        if (m_apmInit)
            CubeApmAgent.MarkLevelEnd();
#endif
    }

    public void SetApmUserID(string userID)
    {
#if USE_CUBE
        if (m_apmInit)
            CubeApmAgent.SetUserId(userID);
#endif
    }
    public void SetScreenLightness(int percentage)
    {
#if !DISABLE_JOYSDK
        ((IHuanlePlatform)_interface).SendGameExtData("set_screen_lightness", "{'percent':" + percentage.ToString() + "}");
#endif
    }

    public void ResetScreenLightness()
    {
#if !DISABLE_JOYSDK
        ((IHuanlePlatform)_interface).SendGameExtData("reset_screen_lightness", "");
#endif
    }

    public System.Object CreateClass(EClassType type)
    {
        switch (type)
        {
            case EClassType.ERuntimeFMOD:
                return XRuntimeFmod.GetFMOD();
        }
        return null;
    }
    public void ReturnClass(EClassType type, System.Object obj)
    {
        switch (type)
        {
            case EClassType.ERuntimeFMOD:
                XRuntimeFmod.ReturnFMOD(obj as XRuntimeFmod);
                break;
        }
    }
    public void ClearClass()
    {
        XRuntimeFmod.Clear();
    }
#region Pay
    /// <summary>
    /// 获取支付相关票据
    /// </summary>
    /// <returns></returns>
    public string GetPayBill()
    {
#if !DISABLE_JOYSDK
        string payBill = ((IHuanlePlatform)_interface).GetSDKConfig("get_payment_bill", "");
        return payBill;
#else
        return "0";
#endif
    }

    /// <summary>
    /// 支付
    /// </summary>
    /// <param name="price"></param>
    /// <param name="paramID"></param>
    /// <param name="role"></param>
    /// <param name="serverID"></param>
    public void Pay(int price, string orderId, string paramID, ulong role, uint serverID)
    {
#if !DISABLE_JOYSDK
        ((IHuanlePlatform)_interface).Pay(price, orderId, paramID, role.ToString(), (int)serverID);
#endif
    }

    /// <summary>
    /// MD5加密
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public string GetMD5(string plainText)
    {
        byte[] fromData = System.Text.Encoding.Unicode.GetBytes(plainText);

        System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();
        byte[] targetData = md5.ComputeHash(fromData);

        string byte2String = null;

        for (int i = 0; i < targetData.Length; i++)
        {
            byte2String += targetData[i].ToString("X");
        }

        return byte2String;
    }

    public string UserMd5(string str)
    {
        string cl = str;
        string pwd = "";
        MD5 md5 = MD5.Create();//实例化一个md5对像
        byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
        for (int i = 0; i < s.Length; i++)
        {
            pwd = pwd + s[i].ToString("X2");
        }
        return pwd;
    }

    /// <summary>
    /// IOS 所有的充值类型的回调都是这里
    /// </summary>
    /// <param name="msg"></param>
    public void PayCallBack(string msg)
    {
        XDebug.singleton.AddLog("[Pay]CallBack: ", msg);
        IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
        if (entrance != null)
        {
            entrance.OnPayCallback(msg);
        }
    }
#endregion


    public void SendExtDara(string key, string param)
    {
#if !DISABLE_JOYSDK
        XDebug.singleton.AddLog("[SendExtDara] paramStr = ",param," key =",key);
        ((IHuanlePlatform)_interface).SendGameExtData(key, param);
#endif
    }

#region WeChatGroup
    /// <summary>
    /// 创建微信群
    /// </summary>
    /// <param name="param"></param>
    public void CreateWXGroup(string param)
    {
#if !DISABLE_JOYSDK
        XDebug.singleton.AddLog("[CreateWXGroup] paramStr = ",param);
        ((IHuanlePlatform)_interface).SendGameExtData("create_wx_group", param);
#endif
    }

    /// <summary>
    /// 加入微信群
    /// </summary>
    /// <param name="param"></param>
    public void JoinWXGroup(string param)
    {
#if !DISABLE_JOYSDK
        XDebug.singleton.AddLog("[JoinWXGroup] paramStr = ", param);
        ((IHuanlePlatform)_interface).SendGameExtData("join_wx_group", param);
#endif
    }

    /// <summary>
    /// 微信群分享
    /// </summary>
    /// <param name="param"></param>
    public void ShareWithWXGroup(string param)
    {
#if !DISABLE_JOYSDK
        XDebug.singleton.AddLog("[ShareWithWXGroup] paramStr = ",param);
        ((IHuanlePlatform)_interface).SendGameExtData("share_send_to_struct_wx_union", param);
#endif
    }

    /// <summary>
    /// 查询微信群
    /// </summary>
    /// <param name="param"></param>
    public void QueryWXGroup(string param)
    {
#if !DISABLE_JOYSDK
        XDebug.singleton.AddLog("[QueryWXGroup] paramStr = ",param);
        ((IHuanlePlatform)_interface).SendGameExtData("query_wx_group", param);
#endif
    }

#endregion

    public int GetDensity()
    {
        return XExtNativeInfo.U3DGetDensity();
    }


    public string GetSim()
    {
        return XExtNativeInfo.U3DGetSim();
    }

    public Component AddComponent(UnityEngine.GameObject go, EComponentType type)
    {
        switch(type)
        {
#if UNITY_EDITOR
            case EComponentType.EXBehaviorTree:
                return go.AddComponent<XBehaviorTree>();
#endif
            case EComponentType.EUIDummy:
                return go.AddComponent<UIDummy>();
            case EComponentType.EXFmod:
                return go.AddComponent<XFmod>();
            default:
                return null;
        }
    }

    public void ReloadFMOD()
    {
        string _cacheConfig = string.Format("{0}/update/AssetBundles/fmodconfig.ab", Application.persistentDataPath);
        if (File.Exists(_cacheConfig))
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(_cacheConfig);
            TextAsset text = bundle.LoadAsset(bundle.GetAllAssetNames()[0]) as TextAsset;
            string[] config = text.text.Split(new Char[] { '|' });
            bundle.Unload(true);

            for (int i = 0; i < config.Length; ++i)
            {
                string path = string.Format("{0}/update/AssetBundles/{1}.bank", Application.persistentDataPath, config[i]);
                if (File.Exists(path))
                {
                    try
                    {
                        RuntimeManager.UnloadBank(config[i]);
                        RuntimeManager.LoadBank(config[i], File.ReadAllBytes(path));
                    }
                    catch(Exception ex)
                    {
                        XDebug.singleton.AddErrorLog(ex.Message);
                    }
                }
            }
        }
    }

    public INativePlugin GetNativePlugin()
    {
        if(m_NativePlugin==null)
        {
            m_NativePlugin = new NativePluginHelper();
        }
        return m_NativePlugin;
    }

    


}
