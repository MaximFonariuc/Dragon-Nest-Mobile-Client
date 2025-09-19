using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class CheckConfigAndroid : CheckConfigBase
{
    readonly string[] PERMISSIONS = 
        {
            "android.permission.ACCESS_NETWORK_STAT",
            "android.permission.ACCESS_WIFI_STATE",
            "android.permission.ACCESS_FINE_LOCATION",
            "android.permission.ACCESS_COARSE_LOCATION",
            "android.permission.CHANGE_WIFI_STATE",
            "android.permission.GET_TASKS",
            "android.permission.INTERNET",
            "android.permission.MOUNT_UNMOUNT_FILESYSTEMS",
            "android.permission.READ_PHONE_STATE",
            "android.permission.RESTART_PACKAGES",
            "android.permission.SYSTEM_ALERT_WINDOW",
            "android.permission.WRITE_EXTERNAL_STORAGE",
            "android.permission.READ_LOGS",
            "android.permission.INTERNET",
            "android.permission.ACCESS_WIFI_STATE",
            "android.permission.GET_TASKS",
            "android.permission.RECEIVE_USER_PRESENT",
            "android.permission.WAKE_LOCK",
            "android.permission.VIBRATE"
        };

    public CheckConfigAndroid(string projectPath) : base(projectPath) 
    {
        result.platform = "Android";
    }

    public override void Check() 
    {
        base.Check();

        // 检查project.properties
        CheckProjectProperties();
        // 检查msdkconfig.ini
        CheckMsdkConfig();
        // 检查AndroidManifest.xml
        CheckAndroidManifest();

        result.resultError = errorMsg.ToString();
        result.resultWarnning = warnningMsg.ToString();
    }

    void CheckProjectProperties()
    {
        bool Succeed = false;
        string filePath = env.PATH_PLUGIN_ANDROID + "/project.properties";
        if (!File.Exists(filePath)) {
            errorMsg.Append(filePath + " isn't exits.\n\n");
            return;
        }
        Dictionary<string, string> configs = MsdkUtil.ReadConfigs(filePath);
        foreach (var config in configs) {
            if (config.Key.IndexOf("android.library.reference") != -1) {
                if (config.Value.Equals("./MSDKLibrary")) {
                    // 检查成功
                    Succeed = true;
                }
            }
        }
        if (!Succeed) {
            errorMsg.Append("project.properties configuration error. "
                + "It should have an item like <android.library.reference1=./MSDKLibrary>.\n\n");
        }
    }

    void GetKey(Dictionary<string, string> configs, string key, ref string value)
    {
        if (configs.ContainsKey(key)) {
            value = configs[key];
        } else {
            errorMsg.Append("Plugin/Android/assets/msdkconfig.ini don't have configuration <" + key + ">.\n\n");
        }
    }

    void GetKey(Dictionary<string, string> configs, string key, ref bool value)
    {
        try {
            if (configs.ContainsKey(key)) {
                value = Convert.ToBoolean(configs[key]);
            } else {
                errorMsg.Append("Plugin/Android/assets/msdkconfig.ini don't have configuration <" + key + ">.\n\n");
            }
        } catch (FormatException e) {
            errorMsg.Append("Plugin/Android/assets/msdkconfig.ini configuration of <" + key + "> format illegal.\n\n");
            Debug.LogException(e);
        }
    }

    void CheckMsdkConfig()
    {
        // 读取配置
        string filePath = env.PATH_PLUGIN_ANDROID + "/assets/msdkconfig.ini";
        if (!File.Exists(filePath)) {
            errorMsg.Append(filePath + " isn't exits.\n\n");
            return;
        }
        Dictionary<string, string> configs = MsdkUtil.ReadConfigs(filePath);
        // 检查环境及和各模块开开关
        GetKey(configs, "MSDK_ENV", ref result.domain);
        GetKey(configs, "needNotice", ref result.notice);
        GetKey(configs, "PUSH", ref result.push);
        GetKey(configs, "SAVE_UPDATE", ref result.update);
        GetKey(configs, "GRAY_TEST_SWITCH", ref result.grayTest);
        GetKey(configs, "localLog", ref result.logLevel);
        GetKey(configs, "STAT_LOG", ref result.statLog);
        GetKey(configs, "WXTOKEN_REFRESH", ref result.refresh);
        GetKey(configs, "CLOSE_BUGLY_REPORT", ref result.closeBugly);
        GetKey(configs, "V2SIGNING_ENABLED", ref result.v2sign);
        GetKey(configs, "XG_OTHER_PUSH_ENABLE", ref result.xgOtherPushEnable);
    }

    void CheckPermission(string filePath)
    {
        StreamReader streamReader = new StreamReader(filePath);
        string allText = streamReader.ReadToEnd();
        streamReader.Close();

        foreach (var permission in PERMISSIONS) {
            if (allText.IndexOf(permission) == -1) {
                errorMsg.Append("Lack of permission of " + "<uses-permission android:name=\"" + permission + "\"/>.\n\n");
            }
        }
    }

    string Ck(XmlNode node, string attributes, string value = "", bool recursivity = false)
    {
        string result = "";
        attributes = "android:" + attributes;
        string configration = attributes + "=\"" + value + "\"";
        if (!MsdkUtil.XmlInclue(node, attributes, value, recursivity)) {
            try {
                string package = node.Attributes["android:name"].Value;
                result += package + " lack of configuration \"" + configration + "\".\n\n";
            } catch {
                result += "lack of configuration \"" + configration + "\".\n\n";
            }
        }
        return result;
    }

    void CheckAndroidManifest()
    {
        string filePath = env.PATH_PLUGIN_ANDROID + "/AndroidManifest.xml";
        if (!File.Exists(filePath)) {
            errorMsg.Append(filePath + " isn't exits.\n\n");
            return;
        }
        CheckPermission(filePath);

        XmlDocument doc = new XmlDocument();
        doc.Load(filePath);
        XmlNode application = doc.SelectSingleNode("/manifest/application");
        XmlNodeList nodes = application.ChildNodes;
        foreach (XmlNode node in nodes) {
            if (!node.NodeType.Equals(XmlNodeType.Element)) {
                continue;
            }

            if (MsdkUtil.XmlInclue(node, "android:name", "com.tencent.tauth.AuthActivity")) {
                errorMsg.Append(Ck(node, "launchMode", "singleTask")
                    + Ck(node, "noHistory", "true")
                    + Ck(node, "name", "android.intent.category.DEFAULT", true)
                    + Ck(node, "name", "android.intent.action.VIEW", true)
                    + Ck(node, "name", "android.intent.category.BROWSABLE", true)
                    + Ck(node, "scheme", "tencent" + deploySetting.QqAppId, true));
            }
            if (MsdkUtil.XmlInclue(node, "android:name", "com.tencent.connect.common.AssistActivity")) {
                errorMsg.Append(Ck(node, "configChanges", "orientation|screenSize|keyboardHidden")
                    + Ck(node, "theme", "@android:style/Theme.Translucent.NoTitleBar"));
            }
            if (MsdkUtil.XmlInclue(node, "android:name", deploySetting.BundleId + ".wxapi.WXEntryActivity")) {
                errorMsg.Append(Ck(node, "excludeFromRecents", "true")
                    + Ck(node, "exported", "true")
                    + Ck(node, "launchMode", "singleTop")
                    + Ck(node, "taskAffinity")
                    + Ck(node, "name", "android.intent.action.VIEW", true)
                    + Ck(node, "name", "android.intent.category.DEFAULT", true)
                    + Ck(node, "scheme", deploySetting.WxAppId, true));
            }
            if (MsdkUtil.XmlInclue(node, "android:name", "com.tencent.msdk.weixin.qrcode.WXQrCodeActivity")) {
                errorMsg.Append(Ck(node, "excludeFromRecents", "true")
                    + Ck(node, "exported", "true")
                    + Ck(node, "launchMode", "singleTask")
                    + Ck(node, "taskAffinity")
                    + Ck(node, "configChanges", "orientation|screenSize|keyboardHidden")
                    + Ck(node, "theme", "@android:style/Theme.Light.NoTitleBar")
                    + Ck(node, "screenOrientation", "portrait"));
            }
            if (MsdkUtil.XmlInclue(node, "android:name", "com.tencent.msdk.webview.WebViewActivity")) {
                warnningMsg.Append(Ck(node, "process", ":msdk_inner_webview")
                    + Ck(node, "hardwareAccelerated", "true")
                    + Ck(node, "configChanges", "orientation|screenSize|keyboardHidden|navigation|fontScale|locale")
                    + Ck(node, "screenOrientation", "unspecified")
                    + Ck(node, "theme", "@android:style/Theme.NoTitleBar")
                    + Ck(node, "windowSoftInputMode", "stateHidden|adjustResize"));
            }
            if (MsdkUtil.XmlInclue(node, "android:name", "com.tencent.msdk.webview.JumpShareActivity")) {
                warnningMsg.Append(Ck(node, "theme", "@android:style/Theme.Translucent.NoTitleBar"));
            }
            if (result.update && MsdkUtil.XmlInclue(node, "android:name", "com.tencent.tmdownloader.TMAssistantDownloadService")) {
                errorMsg.Append(Ck(node, "exported", "false")
                    + Ck(node, "process", ":TMAssistantDownloadSDKService"));
            }
            if (MsdkUtil.XmlInclue(node, "android:name", "com.tencent.msdk.SchemeActivity"))
            {
                errorMsg.Append(Ck(node, "launchMode", "singleTask")
                    + Ck(node, "name", "android.intent.category.DEFAULT", true)
                    + Ck(node, "name", "android.intent.action.VIEW", true)
                    + Ck(node, "name", "android.intent.category.BROWSABLE", true)
                    + Ck(node, "scheme", "tencentmsdk" + deploySetting.QqAppId, true));
            }
            if (result.push && MsdkUtil.XmlInclue(node, "android:name", "com.tencent.android.tpush.XGPushReceiver")) {
                errorMsg.Append(Ck(node, "process", ":xg_vip_service")
                    + Ck(node, "priority", "0x7fffffff", true)
                    + Ck(node, "name", "com.tencent.android.xg.vip.action.SDK", true)
                    + Ck(node, "name", "com.tencent.android.xg.vip.action.INTERNAL_PUSH_MESSAGE", true)
                    + Ck(node, "name", "com.tencent.android.xg.vip.action.ACTION_SDK_KEEPALIVE",true)
                    + Ck(node, "name", "android.net.conn.CONNECTIVITY_CHANGE", true)
                    + Ck(node, "name", "android.intent.action.USER_PRESENT", true)
                    + Ck(node, "name", "android.bluetooth.adapter.action.STATE_CHANGED", true)
                    + Ck(node, "name", "android.intent.action.ACTION_POWER_CONNECTED", true)
                    + Ck(node, "name", "android.intent.action.ACTION_POWER_DISCONNECTED", true)
                    );
            }
            if (result.push && MsdkUtil.XmlInclue(node, "android:name", "com.tencent.android.tpush.rpc.XGRemoteService"))
            {
                errorMsg.Append(Ck(node, "name", deploySetting.BundleId + ".XGVIP_PUSH_ACTION", true));
            }
            if (result.push && MsdkUtil.XmlInclue(node, "android:name", "com.tencent.android.tpush.XGPushProvider"))
            {
                errorMsg.Append(Ck(node, "exported","false"));
            }
            if (result.push && MsdkUtil.XmlInclue(node, "android:name", "com.tencent.android.tpush.SettingsContentProvider"))
            {
                errorMsg.Append(Ck(node, "authorities", deploySetting.BundleId + ".TPUSH_PROVIDER", true));
            }
        }
    }

}
