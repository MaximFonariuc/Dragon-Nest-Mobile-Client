using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections;

public class CheckConfigIOS : CheckConfigBase
{
    int errorBegin, warnningBegin;

    readonly string[] LIBS = {
                                 "libz.dylib",
                                 "libstdc++.dylib",
                                 "libz.1.1.3.dylib",
                                 "libsqlite3.dylib",
                                 "libxml2.dylib",
                                 "libstdc++.6.0.9.dylib",
                                 "libc++.dylib",
                                 "CoreTelephony.framework",
                                 "SystemConfiguration.framework",
                                 "UIKit.framework",
                                 "Foundation.framework",
                                 "CoreGraphics.framework",
                                 "MobileCoreServices.framework",
                                 "StoreKit.framework",
                                 "CFNetwork.framework",
                                 "CoreData.framework",
                                 "Security.framework",
                                 "CoreLocation.framework",
                                 "ImageIO.framework",
                                 "CoreText.framework",
                                 "QuartzCore.framework",
                                 "AdSupport.framework",
                                 "Photos.framework",
                                 "MSDK.framework",
                                 "MSDKLbs.framework",
                                 "MSDKPush.framework",
                                 "MSDKSensitivity.framework",
                                 "MSDKBeacon.framework",

//                                 "MSDKFoundation.framework",
//                                 "MSDKMarketing.framework",
//                                 "WGPlatformResources.bundle" 
                             };
//    readonly string LIBS_XG = "MSDKXG.framework";

    public CheckConfigIOS(string projectPath) : base(projectPath) 
    { 
        result.platform = "iOS";
    }

    public override void Check()
    {
        base.Check();

        string XUPorter = env.PATH_EDITOR + "/Scripts/XUPorter";
        if (!Directory.Exists(XUPorter)) {
            warnningMsg.Append("Unable to complete the inspection, because you have not use <XUPortter> of MSDK to config the xcode project.\n");
            return;
        }

        // 1)info.plist
        CheckPlist();
        ModuleCheckResult(errorBegin, warnningBegin, "info.plist");
        // 2)framework,libs
        CheckXcodeProjectSettings();
        ModuleCheckResult(errorBegin, warnningBegin, "Xcode Project");
        // 3)oc code
        CheckCode();
        ModuleCheckResult(errorBegin, warnningBegin, "UnityAppController.mm");

        result.resultError = errorMsg.ToString();
        result.resultWarnning = warnningMsg.ToString();
    }

    void ModuleCheckResult(int errorBegin, int warnningBegin, string moduleName)
    {
        if (errorMsg.Length - errorBegin > 0)
        {
            errorMsg.Insert(errorBegin, "<" + moduleName + "> config error. Read below for detail information.\n");
        }
        if (warnningMsg.Length - warnningBegin > 0)
        {
            warnningMsg.Insert(warnningBegin, "<" + moduleName + "> config may be not appropriate. Read below for detail information.\n");
        }
    }

    // 1)info.plist
    void CheckPlist()
    {
        // 记录错误信息初始位置
        errorBegin = errorMsg.Length;
        warnningBegin = warnningMsg.Length;

        string filePath = result.projectPath + "/info.plist";
        XmlDocument doc = new XmlDocument();
        doc.XmlResolver = null;
        try {
            doc.Load(filePath);
        } catch (Exception e) {
            Debug.LogException(e);
            errorMsg.Append("    Load " + filePath + " error!\n");
            return;
        }

        XmlNode application = doc.SelectSingleNode("/plist/dict");

        // 检查环境，开关配置
        result.domain = MsdkUtil.XmlNextValue(application, "key", "MSDK_ENV", "string");
        if (String.IsNullOrEmpty(result.domain)) {
            errorMsg.Append("    <MSDK_URL> is null.\n\n");
        }
        errorMsg.Append(getSwitch(application, "MSDK_PUSH_SWITCH", ref result.push)
            + getSwitch(application, "AutoRefreshToken", ref result.refresh)
            + getSwitch(application, "NeedNotice", ref result.notice));

        // 检查appid等配置
        errorMsg.Append(CheckXmlMatch(application, "CHANNEL_DENGTA", "1001")
            + CheckXmlMatch(application, "MSDK_OfferId", deploySetting.IOSOfferId)
            + CheckXmlMatch(application, "QQAppID", deploySetting.QqAppId)
            + CheckXmlMatch(application, "WXAppID", deploySetting.WxAppId)
            + CheckXmlMatch(application, "MSDKKey", deploySetting.MsdkKey));
        string noticeTime = MsdkUtil.XmlNextValue(application, "key", "NoticeTime", "integer");
        try {
            Convert.ToInt32(noticeTime);
        } catch {
            errorMsg.Append("    <NoticeTime> configuration error, it must be integer.\n\n");
        }

        // NSAppTransportSecurity
        XmlNode securityConfig = application.SelectSingleNode("key[. = 'NSAppTransportSecurity']");
        if (securityConfig != null && securityConfig.NextSibling != null
                && "dict".Equals(securityConfig.NextSibling.Name)) {
            securityConfig = securityConfig.NextSibling;
        } else {
            errorMsg.Append("    <NSAppTransportSecurity> must be config.\n\n");
        }
        if (!MsdkUtil.XmlMath(securityConfig, "key", "NSAllowsArbitraryLoads", "true")) {
            errorMsg.Append("    <NSAllowsArbitraryLoads> must be true.\n\n");
        }

        // LSApplicationQueriesSchemes
        string[] schemes = {
                               "mqq",
                               "mqqapi",
                               "wtloginmqq2",
                               "mqqopensdkapiV3",
                               "mqqopensdkapiV2",
                               "mqqwpa",
                               "mqqOpensdkSSoLogin",
                               "mqqgamebindinggroup",
                               "mqqopensdkfriend",
                               "mqzone",
                               "weixin",
                               "wechat",
                               "weixinULAPI",
                               "weixinURLParamsAPI",
                               "mqqopensdknopasteboard",
                               "mqqopensdknopasteboardios16"
                           };
        XmlNode queriesSchemes = application.SelectSingleNode("key[. = 'LSApplicationQueriesSchemes']");
        if (queriesSchemes == null || queriesSchemes.NextSibling == null
                || !"array".Equals(queriesSchemes.NextSibling.Name)) {
            errorMsg.Append("    <LSApplicationQueriesSchemes> should be config.");
        } else {
            queriesSchemes = queriesSchemes.NextSibling;
            foreach (string scheme in schemes) {
                XmlNode schemeContent = queriesSchemes.SelectSingleNode("string" + "[. = '" + scheme + "']");
                if (schemeContent == null) {
                    errorMsg.Append("    <LSApplicationQueriesSchemes> lack of <" + scheme + ">.\n\n");
                }
            }
        }

        // CFBundleURLTypes
        XmlNode urlTypes = application.SelectSingleNode("key[. = 'CFBundleURLTypes']");
        if (urlTypes == null || urlTypes.NextSibling == null 
                || !"array".Equals(urlTypes.NextSibling.Name)) {
            errorMsg.Append("    <CFBundleURLTypes> must be config.\n\n");
        } else {
            urlTypes = urlTypes.NextSibling;
        }
        errorMsg.Append(CheckUrlType(urlTypes, "weixin", deploySetting.WxAppId));
        errorMsg.Append(CheckUrlType(urlTypes, "tencentopenapi", "tencent" + deploySetting.QqAppId));
        errorMsg.Append(CheckUrlType(urlTypes, "QQ", deploySetting.QqScheme));
        errorMsg.Append(CheckUrlType(urlTypes, "QQLaunch", "tencentlaunch" + deploySetting.QqAppId));
		errorMsg.Append(CheckUrlType(urlTypes, "tencentmsdk", "tencentmsdk" + deploySetting.QqAppId));
    }

    string getSwitch(XmlNode parent, string key, ref bool moduleSwitch)
    {
        try {
            moduleSwitch = Convert.ToBoolean(MsdkUtil.XmlNextName(parent, "key", key));
            return "";
        } catch {
            return "    Get switch of <" + key + "> error. Please check your <info.plist> file.\n\n";
        }
    }

    string CheckXmlMatch(XmlNode parent, string key,  string value, string logName = "")
    {
        if (MsdkUtil.XmlMath(parent, "key", key, "string", value)) {
            return "";
        } else {
            if (String.IsNullOrEmpty(logName)) {
                return "    <" + key + "> should be <" + value + ">.\n\n";
            } else {
                return "    " + logName + " config error. <" + key + "> should be <" + value + ">.\n\n";
            }
        }
    }

    string CheckUrlType(XmlNode parent, string identifier, string schemeValue)
    {
        if (parent == null) {
            return "    <Url Types> is not configured.\n\n";
        }
        XmlNode urlType = parent.SelectSingleNode("./dict[string = '" + identifier + "']");
        if (urlType == null) {
            return "    Url Type of <weixin> is not configured.\n\n";
        }
        if (!MsdkUtil.XmlMath(urlType, "key", "CFBundleTypeRole", "string", "Editor")) {
            return "    Url Type of <" + identifier + "> config error.<Role> should be <Editor>.\n\n";
        }
        XmlNode schemeNode = urlType.SelectSingleNode("array/string");
        if (schemeNode == null || !schemeNode.InnerText.Equals(schemeValue)) {
            return "    Url Type of <" + identifier + "> config error. It's <URL Schemes> should be <" + schemeValue + ">.\n\n";
        } else {
            return "";
        }
    }

    // 2)framework,libs
    void CheckXcodeProjectSettings()
    {
        // 记录错误信息初始位置
        errorBegin = errorMsg.Length;
        warnningBegin = warnningMsg.Length;

        string filePath = result.projectPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
        string fileContent = "";
        try {
            StreamReader streamReader = new StreamReader(filePath);
            fileContent = streamReader.ReadToEnd();
            streamReader.Close();
        } catch (Exception e) {
            Debug.LogException(e);
        } 
        if (String.IsNullOrEmpty(fileContent)) {
            errorMsg.Append("    Can't find " + filePath + " to check xcode project settings.\n\n");
            return;
        }

    }

    void HasLibs(string fileContent, string[] librarys)
    {
        foreach (string library in librarys) {
            HasLib(fileContent, library);
        }
    }

    void HasLib(string fileContent, string library)
    {
        if (fileContent.IndexOf(library) == -1) {
            errorMsg.Append("    Build Phases->Link Binary With Libraries lack of <" + library + ">.\n\n");
        }
    }

    // 3)oc code
    void CheckCode()
    {
        string filePath = result.projectPath + "/Classes/UnityAppController.mm";
        string fileContent = "";
        try {
            StreamReader streamReader = new StreamReader(filePath);
            fileContent = streamReader.ReadToEnd();
            streamReader.Close();
        } catch (Exception e) {
            Debug.LogException(e);
        }
        if (String.IsNullOrEmpty(fileContent)) {
            errorMsg.Append("    Read " + filePath + " error.\n\n");
            return;
        }
        HasCode(fileContent, MsdkNativeCode.IOS_HEADER);
        HasCode(fileContent, MsdkNativeCode.IOS_HANDLE_URL);
#if UNITY_2019_3_OR_NEWER // 适配 2019.3及以上版本
        HasCode(fileContent, MsdkNativeCode.IOS_HANDLE_UNIVERSALLINK_HIGH);
#else
        HasCode(fileContent, MsdkNativeCode.IOS_HANDLE_UNIVERSALLINK);
#endif
        HasCode(fileContent, MsdkNativeCode.IOS_BECAME_ACTIVE);
        if (result.push) {
            HasCode(fileContent, MsdkNativeCode.IOS_XG_REGISTER);
            HasCode(fileContent, MsdkNativeCode.IOS_XG_SUCC);
            HasCode(fileContent, MsdkNativeCode.IOS_XG_FAIL);
            HasCode(fileContent, MsdkNativeCode.IOS_XG_RECEIVE);
            HasCode(fileContent, MsdkNativeCode.IOS_XG_CLEAR);
        }
    }

    void HasCode(string fileContent, string code)
    {
        if (fileContent.IndexOf(code) == -1) {
            errorMsg.Append("    ObjectiveC code lack of : \n<\n" + code + "\n>.\n\n");
        }
    }
}
