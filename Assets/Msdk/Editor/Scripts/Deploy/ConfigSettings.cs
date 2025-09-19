using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

[InitializeOnLoad]
public class ConfigSettings : ScriptableObject
{

    const string msdkSettingsAssetName = "ConfigSettings";
    const string msdkSettingsPath = "Msdk/Editor/Resources";
    const string msdkSettingsAssetExtension = ".asset";

    static MsdkEnv env = MsdkEnv.Instance;
    string androidConfigPath = env.PATH_PLUGIN_ANDROID + "/assets/msdkconfig.ini";
    string iosConfigPath = env.PATH_IOS_PLIST;

	public string[] msdkUrlEnvs = new string[]{"release","test"};


    public string[] logLevels = new string[] { "0:不打印", "1:打印到控制台", "2:打印到文件", "3:打印到控制台和文件" };

    private static ConfigSettings _instance;

    public static ConfigSettings Instance
    {
        get
        {
            if (_instance == null) {
                _instance = Resources.Load(msdkSettingsAssetName) as ConfigSettings;
                if (_instance == null) {
                    _instance = CreateInstance<ConfigSettings>();

                    string properPath = Path.Combine(Application.dataPath, msdkSettingsPath);
                    if (!Directory.Exists(properPath)) {
                        Directory.CreateDirectory(properPath);
                    }

                    string fullPath = Path.Combine(Path.Combine("Assets", msdkSettingsPath),
                                                   msdkSettingsAssetName + msdkSettingsAssetExtension
                                                   );
                    AssetDatabase.CreateAsset(_instance, fullPath);
                }
            }
            return _instance;
        }
    }

    #region Config Settings
    //--- Common Settings ---
    [SerializeField]
    private bool checkConfig = true;
    [SerializeField]
    private int msdkUrlIndex = 1;
    [SerializeField]
    private string msdkUrlEnv = "test";
    [SerializeField]
    private bool wxRefresh = true;
    [SerializeField]
    private bool push = true;
    [SerializeField]
    private bool needNotice = true;
    [SerializeField]
    private string noticeTime = "15";
    [SerializeField]
    private string realNameAuth = "1";
    [SerializeField]
    private bool webviewPortraitHideable = false;
    [SerializeField]
    private bool webviewLandscapeHideable = false;

    //--- Android Special Settings ---
    [SerializeField]
    private bool beta = false;							//应用宝抢号开关
    [SerializeField]
    private int logLevelIndex = 1;
    [SerializeField]
    private string localLog = "1";						//MSDKLog开关 0：不打印；1：logcat打印；2：打印到本地文件；3：logcat和本地文件同时打印；
    //本地打印的保存在SDCard/MSDK/msdk.log
    [SerializeField]
    private bool statLog = true;						//灯塔和MTA日志开关，设为true时会打印详细日志，正式上线时建议将此开关关闭即设为false
    [SerializeField]
    private bool closeBugly = false;
    [SerializeField]
    private bool saveUpdate = false;
    [SerializeField]
    private bool grayTest = false;                      // 渠道放量开关
    [SerializeField]
    private bool v2Signing = false;                     // V2签名开关，启用v2签名的包一定要打开这个开关（不开会导致渠道号读取错误），不填默认关闭
    [SerializeField]
    private bool xgOtherPushEnable = true;           // 信鸽厂商渠道配置
    [SerializeField]
    private string xgOtherPushXiaomiAppid = "2882303761518321210";
    [SerializeField]
    private string xgOtherPushXiaomiAppkey = "5811832142210";
    [SerializeField]
    private string xgOtherPushMeizuAppid = "127836";
    [SerializeField]
    private string xgOhterPushMeizuAppkey = "584eb9db1c3e4025ae96be51c0962598";
    //--- iOS Special Settings ---
    private readonly string channel = "1001";           //iOS系统渠道号, 固定

    #region Get

    public bool CheckConfig
    {
        get { return Instance.checkConfig; }
    }

    public int MsdkUrlIndex
    {
        get { return Instance.msdkUrlIndex; }
    }

    public string MsdkUrlEnv
    {
        get { return Instance.msdkUrlEnv; }
    }

    public bool WxRefresh
    {
        get { return Instance.wxRefresh; }
    }

    public bool Push
    {
        get { return Instance.push; }
    }

    public bool NeedNotice
    {
        get { return Instance.needNotice; }
    }

    public string NoticeTime
    {
        get { return Instance.noticeTime; }
    }

    public string RealNameAuth
    {
        get { return Instance.realNameAuth; }
    }

    public bool WebviewPortraitHideable
    {
        get { return Instance.webviewPortraitHideable; }
    }

    public bool WebviewLandscapeHideable
    {
        get { return Instance.webviewLandscapeHideable; }
    }

    public bool Beta
    {
        get { return Instance.beta; }
    }

    public int LogLevelIndex
    {
        get { return Instance.logLevelIndex; }
    }

    public string LocalLog
    {
        get { return Instance.localLog; }
    }

    public bool StatLog
    {
        get { return Instance.statLog; }
    }

    public bool CloseBugly
    {
        get { return Instance.closeBugly; }
    }

    public bool SaveUpdate
    {
        get { return Instance.saveUpdate; }
    }

    public bool GrayTest
    {
        get { return Instance.grayTest; }
    }

    public bool V2Signing
    {
        get { return Instance.v2Signing; }
    }

    public string Channel
    {
        get { return Instance.channel; }
    }

    public bool XgOtherPushEnable
    {
        get { return Instance.xgOtherPushEnable; }
    }

    public string XgOtherPushXiaomiAppid
    {
        get { return xgOtherPushXiaomiAppid; }
    }

    public string XgOtherPushXiaomiAppkey
    {
        get { return xgOtherPushXiaomiAppkey; }
    }
    public string XgOtherPushMeizuAppid
    {
        get { return xgOtherPushMeizuAppid; }
    }

    public string XgOtherPushMeizuAppkey
    {
        get { return xgOhterPushMeizuAppkey; }
    }

    #endregion Get

    #region Set

    public void SetCheckConfig(bool value)
    {
        if (checkConfig != value) {
            checkConfig = value;
            DirtyEditor();
        }
    }

    public void SetMsdkUrl(int index)
    {
        if (msdkUrlIndex != index) {
            msdkUrlIndex = index;
            msdkUrlEnv = msdkUrlEnvs[index];
            DirtyEditor();

            Replace("^MSDK_ENV=", "MSDK_ENV=" + msdkUrlEnv);
            ReplaceBelow(@"        <key>MSDK_ENV</key>", "        <string>" + msdkUrlEnv + "</string>");
        }
    }

    public void SetWxRefresh(bool value)
    {
        if (wxRefresh != value) {
            wxRefresh = value;
            DirtyEditor();

            Replace("^WXTOKEN_REFRESH.*=", "WXTOKEN_REFRESH=" + b2s(wxRefresh));
            ReplaceBelow(@"        <key>AutoRefreshToken</key>", "        <" + b2s(wxRefresh) + "/>");
        }
    }

    public void SetPush(bool value)
    {
        if (push != value) {
            push = value;
            DirtyEditor();

            Replace("^PUSH.*=", "PUSH=" + b2s(push));
            ReplaceBelow(@"        <key>MSDK_PUSH_SWITCH</key>", "        <" + b2s(push) + "/>");
        }
    }

    public void SetNeedNotice(bool value)
    {
        if (needNotice != value) {
            needNotice = value;
            DirtyEditor();

            Replace("^needNotice.*=", "needNotice=" + b2s(needNotice));
            ReplaceBelow(@"        <key>NeedNotice</key>", "        <" + b2s(needNotice) + "/>");
        }
    }

    public void SetNoticeTime(string value)
    {
        if (noticeTime != value) {
            noticeTime = value;
            DirtyEditor();

            int num;
            if (System.Int32.TryParse(noticeTime, out num)) {
                Replace("^noticeTime.*=", "noticeTime=" + noticeTime);
                int secondNum = 60 * num;
                ReplaceBelow(@"        <key>NoticeTime</key>", "        <integer>" + secondNum.ToString() + "</integer>");
            } else {
                Debug.LogError("noticeTime is not integer!");
            }
        }
    }

    public void SetRealNameAuth(string value)
    {
        if (realNameAuth != value) {
            realNameAuth = value;
            DirtyEditor();

            int num;
            if (System.Int32.TryParse(realNameAuth, out num)) {
                Replace("^MSDK_REAL_NAME_AUTH_SWITCH.*=", "MSDK_REAL_NAME_AUTH_SWITCH=" + realNameAuth);
                ReplaceBelow(@"        <key>MSDK_REAL_NAME_AUTH_SWITCH</key>",
                    "        <integer>" + realNameAuth + "</integer>");
            } else {
                Debug.LogError("realNamtAuth is not integer!");
            }
        }
    }

    public void SetWebviewPortraitHideable(bool value)
    {
        if (webviewPortraitHideable != value) {
            webviewPortraitHideable = value;
            DirtyEditor();

            ReplaceManifest("toolbar_portrait_hideable", 
                "            <meta-data android:name=\"toolbar_portrait_hideable\" android:value=\"" +
                b2s(webviewPortraitHideable)  + "\"/>");
            ReplaceManifest("titlebar_hideable", 
                "            <meta-data android:name=\"titlebar_hideable\" android:value=\"" +
                b2s(webviewPortraitHideable) + "\"/>");
            ReplaceBelow(@"        <key>MSDK_Webview_Portrait_NavBar_Hideable</key>",
                "        <" + b2s(webviewPortraitHideable) + "/>");
        }
    }

    public void SetWebviewLandscapeHideable(bool value)
    {
        if (webviewLandscapeHideable != value) {
            webviewLandscapeHideable = value;
            DirtyEditor();

            ReplaceManifest("toolbar_landscape_hideable", 
                "            <meta-data android:name=\"toolbar_landscape_hideable\" android:value=\"" +
                b2s(webviewLandscapeHideable) + "\"/>");
            ReplaceBelow(@"        <key>MSDK_Webview_Landscape_NavBar_Hideable</key>",
                "        <" + b2s(webviewLandscapeHideable) + "/>");
        }
    }

    public void SetBeta(bool value)
    {
        if (beta != value) {
            beta = value;
            DirtyEditor();

            Replace("^BETA.*=", "BETA=" + b2s(beta));
        }
    }

    public void SetLocalLog(int index)
    {
        if (logLevelIndex != index) {
            logLevelIndex = index;
            localLog = logLevels[index].Substring(0, 1);
            DirtyEditor();

            Replace("^localLog.*=", "localLog=" + localLog);
        }
    }

    public void SetStatLog(bool value)
    {
        if (statLog != value) {
            statLog = value;
            DirtyEditor();

            Replace("^STAT_LOG.*=", "STAT_LOG=" + b2s(statLog));
        }
    }

    public void SetCloseBugly(bool value)
    {
        if (closeBugly != value) {
            closeBugly = value;
            DirtyEditor();

            Replace("^CLOSE_BUGLY_REPORT.*=", "CLOSE_BUGLY_REPORT=" + b2s(closeBugly));
        }
    }

    public void SetSaveUpdate(bool value)
    {
        if (saveUpdate != value) {
            saveUpdate = value;
            DirtyEditor();

            Replace("^SAVE_UPDATE.*=", "SAVE_UPDATE=" + b2s(saveUpdate));
        }
    }

    public void SetGrayTest(bool value)
    {
        if (grayTest != value) {
            grayTest = value;
            DirtyEditor();

            Replace("^GRAY_TEST_SWITCH.*=", "GRAY_TEST_SWITCH=" + b2s(grayTest));
        }
    }

    public void SetV2Signing(bool value)
    {
        if (v2Signing != value)
        {
            v2Signing = value;
            DirtyEditor();

            Replace("^V2SIGNING_ENABLED.*=", "V2SIGNING_ENABLED=" + b2s(v2Signing));
        }

    }

    public void SetXgOterPushEnable(bool value)
    {
        if (xgOtherPushEnable != value)
        {
            xgOtherPushEnable = value;
            DirtyEditor();

            Replace("^XG_OTHER_PUSH_ENABLE.*=", "XG_OTHER_PUSH_ENABLE=" + b2s(xgOtherPushEnable));
        }
    }

    public void SetXgOtherPushXiaomiAppid(string value)
    {
        if (xgOtherPushXiaomiAppid != value)
        {
            xgOtherPushXiaomiAppid = value;
            DirtyEditor();

            Replace("^XG_OTHER_PUSH_XIAOMI_APPID.*=", "XG_OTHER_PUSH_XIAOMI_APPID=" + xgOtherPushXiaomiAppid);
        }
    }

    public void SetXgOtherPushXiaomiAppikey(string value)
    {
        if (xgOtherPushXiaomiAppkey != value)
        {
            xgOtherPushXiaomiAppkey = value;
            DirtyEditor();

            Replace("^XG_OTHER_PUSH_XIAOMI_APPKEY.*=", "XG_OTHER_PUSH_XIAOMI_APPKEY=" + xgOtherPushXiaomiAppkey);
        }
    }

    public void SetXgOtherPushMeizuAppid(string value)
    {
        if (xgOtherPushMeizuAppid != value)
        {
            xgOtherPushMeizuAppid = value;
            DirtyEditor();


            Replace("^XG_OTHER_PUSH_MEIZU_APPID.*=", "XG_OTHER_PUSH_MEIZU_APPID=" + xgOtherPushMeizuAppid);
        }
    }

    public void SetXgOtherPushMeizuAppkey(string value)
    {
        if (xgOhterPushMeizuAppkey != value)
        {
            xgOhterPushMeizuAppkey = value;
            DirtyEditor();

            Replace("^XG_OTHER_PUSH_MEIZU_APPKEY.*=", "XG_OTHER_PUSH_MEIZU_APPKEY=" + xgOhterPushMeizuAppkey);
        }
    }

    #endregion Set

    public void Replace(string regex, string newString)
    {
        if (!File.Exists(androidConfigPath)) {
            Debug.LogError("Assets/Plugin/Android/assets/msdkconfig.ini is not exist! Please deploy MSDK first.");
        } else {
            MsdkUtil.ReplaceTextWithRegex(androidConfigPath, regex, newString);
        }
    }

    private void ReplaceManifest(string regex, string newString)
    {
        string manifest = env.PATH_PLUGIN_ANDROID + "/AndroidManifest.xml";
        string manifestCopy = env.PATH_PLUGIN_ANDROID + "/Copy_AndroidManifest.xml";
        if (File.Exists(manifest)) {
            MsdkUtil.ReplaceTextWithRegex(manifest, regex, newString);
        } else {
            Debug.LogError("Assets/Plugin/Android/AndroidManifest.xml is not exits! Please deploy MSDK first.");
        }
        if (File.Exists(manifestCopy)) {
            MsdkUtil.ReplaceTextWithRegex(manifestCopy, regex, newString);
        }
    }

    public void ReplaceBelow(string below, string addString)
    {
        if (!File.Exists(iosConfigPath)) {
            Debug.LogError(iosConfigPath + " is not exist!");
        } else {
            MsdkUtil.ReplaceBelow(iosConfigPath, below, addString);
        }
    }

    public void Update()
    {
        Replace("^MSDK_ENV=", "MSDK_ENV=" + msdkUrlEnv);
        Replace("^WXTOKEN_REFRESH.*=", "WXTOKEN_REFRESH=" + b2s(wxRefresh));
        Replace("^PUSH.*=", "PUSH=" + b2s(push));
        Replace("^needNotice.*=", "needNotice=" + b2s(needNotice));
        Replace("^noticeTime.*=", "noticeTime=" + noticeTime);
        Replace("^BETA.*=", "BETA=" + b2s(beta));
        Replace("^localLog.*=", "localLog=" + localLog);
        Replace("^STAT_LOG.*=", "STAT_LOG=" + b2s(statLog));
        Replace("^CLOSE_BUGLY_REPORT.*=", "CLOSE_BUGLY_REPORT=" + b2s(closeBugly));
        Replace("^SAVE_UPDATE.*=", "SAVE_UPDATE=" + b2s(saveUpdate));
        Replace("^GRAY_TEST_SWITCH.*=", "GRAY_TEST_SWITCH=" + b2s(grayTest));
        Replace("^V2SIGNING_ENABLED.*=", "V2SIGNING_ENABLED=" + b2s(v2Signing));
        Replace("^XG_OTHER_PUSH_ENABLE.*=", "XG_OTHER_PUSH_ENABLE=" + b2s(xgOtherPushEnable));
        Replace("^XG_OTHER_PUSH_XIAOMI_APPID.*=", "XG_OTHER_PUSH_XIAOMI_APPID=" + xgOtherPushXiaomiAppid);
        Replace("^XG_OTHER_PUSH_XIAOMI_APPKEY.*=", "XG_OTHER_PUSH_XIAOMI_APPKEY=" + xgOtherPushXiaomiAppid);
        Replace("^XG_OTHER_PUSH_MEIZU_APPID.*=", "XG_OTHER_PUSH_MEIZU_APPID=" + xgOtherPushMeizuAppid);
        Replace("^XG_OTHER_PUSH_MEIZU_APPKEY.*=", "XG_OTHER_PUSH_MEIZU_APPKEY=" + xgOhterPushMeizuAppkey);
        ReplaceBelow(@"        <key>MSDK_ENV</key>", "        <string>" + msdkUrlEnv + "</string>");
        ReplaceBelow(@"        <key>AutoRefreshToken</key>", "        <" + b2s(wxRefresh) + "/>");
        ReplaceBelow(@"        <key>MSDK_PUSH_SWITCH</key>", "        <" + b2s(push) + "/>");
        ReplaceBelow(@"        <key>NeedNotice</key>", "        <" + b2s(needNotice) + "/>");
        int num;
        if (System.Int32.TryParse(noticeTime, out num)) {
            int secondNum = 60 * num;
            ReplaceBelow(@"        <key>NoticeTime</key>", "        <integer>" + secondNum.ToString() + "</integer>");
        }
        int num2;
        if (System.Int32.TryParse(realNameAuth, out num2)) {
            Replace("^MSDK_REAL_NAME_AUTH_SWITCH.*=", "MSDK_REAL_NAME_AUTH_SWITCH=" + realNameAuth);
            ReplaceBelow(@"        <key>MSDK_REAL_NAME_AUTH_SWITCH</key>",
                "        <integer>" + realNameAuth + "</integer>");
        }

        ReplaceManifest("toolbar_portrait_hideable",
            "            <meta-data android:name=\"toolbar_portrait_hideable\" android:value=\"" +
            b2s(webviewPortraitHideable) + "\"/>");
        ReplaceManifest("titlebar_hideable", 
            "            <meta-data android:name=\"titlebar_hideable\" android:value=\"" +
            b2s(webviewPortraitHideable) + "\"/>");
        ReplaceBelow(@"        <key>MSDK_Webview_Portrait_NavBar_Hideable</key>",
            "        <" + b2s(webviewPortraitHideable) + "/>");
        ReplaceManifest("toolbar_landscape_hideable", 
            "            <meta-data android:name=\"toolbar_landscape_hideable\" android:value=\"" +
            b2s(webviewLandscapeHideable) + "\"/>");
        ReplaceBelow(@"        <key>MSDK_Webview_Landscape_NavBar_Hideable</key>",
            "        <" + b2s(webviewLandscapeHideable) + "/>");
    }

    private string b2s(bool value)
    {
        return value ? "true" : "false";
    }

    private void DirtyEditor()
    {
        EditorUtility.SetDirty(Instance);
    }
    #endregion
}
