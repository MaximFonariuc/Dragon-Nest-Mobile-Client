using UnityEngine;
using System.Collections.Generic;
using XUtliPoolLib;

using com.tencent.pandora;
using com.tencent.pandora.MiniJSON;

using System.Collections;
using System;
using System.IO;

//腾讯潘多拉SDK
public class XPandoraMgr : MonoBehaviour , IXPandoraMgr
{
    // 只初始化一次
    private bool isInited = false;

    // 普通分享
    private bool isSharing = false;

    // 后端分享
    private bool isBackEndSharing = false;

    // 大图分享
    private bool isPicSharing = false;

    //正在分享的活动
    private string sharingContent = "";

    //活动是否准备好拍脸
    List<ActivityPopInfo> listActivityPopInfo = new List<ActivityPopInfo>();

    //活动tab是否显示
    List<ActivityTabInfo> listActivityTabInfo = new List<ActivityTabInfo>();

    //挑战巢穴，龙本等场景失败时弹出preLoss拍脸
    bool bPopPreLossActivity = false;

    public Font font;

    public bool Deprecated
    {
        get;
        set;
    }

    public void Init()
    {
        if (isInited)
        {
            return;
        }

        isInited = true;
        listActivityPopInfo.Clear();
        listActivityTabInfo.Clear();
        Pandora.Instance.SetFont(font);
        Pandora.Instance.SetPlaySoundDelegate(NGUITools.PlayFmod);

        //正式包连正式环境，测试包连潘多拉测试环境
        bool bFormal = XUpdater.XUpdater.singleton.XPlatform.IsPublish();
        XDebug.singleton.AddGreenLog("Pandora Init isProductEnvironment = " + bFormal.ToString());

        Pandora.Instance.Init(bFormal, "XGamePoint");
        Pandora.Instance.SetJsonGameCallback(OnJsonPandoraEvent);
    }

    public void SetUseHttps(int use)
    {
        Pandora.Instance.UseHttps = use > 0 ? true : false;
    }

    /// 初始化登陆信息，每次切换服务器或角色后需要重新登录
    public void PandoraLogin(string openid, string acctype, string area, string serverID, 
        string appId, string roleID, string accessToken, string payToken, string gameVersion, string platID)
    {
        if (XUpdater.XUpdater.singleton.XPlatform.Platfrom() == XPlatformType.Standalone)
        {
            XDebug.singleton.AddLog("PandoraLogin XPlatformType.Standalone");
            return;
        }

        Init();

        Dictionary<string, string> userDataDict = new Dictionary<string, string>();

        userDataDict["sOpenId"] = openid;                               //玩家帐号openID
        userDataDict["sAcountType"] = acctype;                      //玩家帐号类型，qq 或 wx
        userDataDict["sArea"] = area;                                     //大区 （微信，QQ，手Q等）
        userDataDict["sPartition"] = serverID;                                   //小区 (游戏内区服)
        userDataDict["sAppId"] = appId;                              //游戏App唯一标识，QQ和微信的不一样
        userDataDict["sRoleId"] = roleID;                            //游戏中的角色id 或 uid
        userDataDict["sAccessToken"] = accessToken;  //接入返回token
        userDataDict["sPayToken"] = payToken;     //支付token
        userDataDict["sGameVer"] = gameVersion;                               //游戏版本号
        userDataDict["sPlatID"] = platID;                                      //平台ID 1:android,0:ios

        ////userDataDict["sOpenId"] = "1E902F87B536BE6C1C4177DF1F7782F0";       //ceshi qq
        //userDataDict["sOpenId"] = "B16EC3F74765591249E0A5D4A98D6102";       //shana qq
        ////userDataDict["sOpenId"] = "oiz-ewUqy75OSGVoVeNdQP-YtpJA";       //shana wx
        //userDataDict["sAcountType"] = "qq";                                 //玩家帐号类型，qq 或 wx
        //userDataDict["sArea"] = "2";                                        //大区 （微信，QQ，手Q等）
        //userDataDict["sPartition"] = "20";                                   //小区 (游戏内区服)
        //userDataDict["sAppId"] = "1105309683";                              //游戏App唯一标识，QQ和微信的不一样
        //userDataDict["sRoleId"] = "23517832628";                            //游戏中的角色id 或 uid
        //userDataDict["sAccessToken"] = "3EF900853DE76855E6212CC9AFD006EA";  //接入返回token
        //userDataDict["sPayToken"] = "30A2E041AC40AC4E8885C4802F650B6C";     //支付token
        //userDataDict["sGameVer"] = "1.3.0.0";                               //游戏版本号
        //userDataDict["sPlatID"] = "1";                                      //平台ID 1:android,0:ios
        Pandora.Instance.SetUserData(userDataDict);
        Pandora.Instance.InitWebView(false);
        Pandora.Instance.SetUrlEncodeDelegate(PandoraGetEncodeUrl);

        XDebug.singleton.AddLog("pandoraSDK param:" + openid + ", " + acctype + ", " +area + ", " + serverID +
            "," + appId + ", " + roleID + "," + accessToken + "," + payToken + ", "+gameVersion + ", " + platID);
    }

    /// 登出，每次切换服务器后调用
    public void PandoraLogout()
    {
        if (XUpdater.XUpdater.singleton.XPlatform.Platfrom() == XPlatformType.Standalone)
        {
            XDebug.singleton.AddLog("PandoraLogin XPlatformType.Standalone");
            return;
        }

        listActivityPopInfo.Clear();
        listActivityTabInfo.Clear();
        Pandora.Instance.Logout();
    }

#region 接口
    public void PandoraInit(bool isProductEnvironment, string rootName = "")
    {
        Pandora.Instance.Init(isProductEnvironment, rootName);
    }

    public void PandoraDo(Dictionary<string, string> commandDict)
    {
        Pandora.Instance.Do(commandDict);
    }

    public void PandoraDoJson(string json)
    {
        Pandora.Instance.DoJson(json);
    }

    public void SetPandoraPanelParent(string panelName, GameObject panelParent)
    {
        if (XUpdater.XUpdater.singleton.XPlatform.Platfrom() == XPlatformType.Standalone)
        {
            XDebug.singleton.AddLog("PandoraLogin XPlatformType.Standalone");
            return;
        }

        XDebug.singleton.AddLog("Pandora SetPandoraPanelParent panelName = " + panelName);
        Pandora.Instance.SetPanelParent(panelName, panelParent);
    }

    public void SetFont(Font font)
    {
        Pandora.Instance.SetFont(font);
    }

    public void LoadProgramAsset()
    {
        Pandora.Instance.LoadProgramAsset();
    }

    public void SetUserData(Dictionary<string, string> data)
    {
        Pandora.Instance.SetUserData(data);
    }

    public void SetGameCallback(Action<Dictionary<string, string>> callback)
    {
        Pandora.Instance.SetGameCallback(callback);
    }

    public void SetJsonGameCallback(Action<string> callback)
    {
        Pandora.Instance.SetJsonGameCallback(callback);
    }
#endregion

    private string PandoraGetEncodeUrl(string oriUrl)
    {
        Dictionary<string, object> jsonData = new Dictionary<string, object>();
        jsonData["url"] = oriUrl;
        string paramStr = MiniJSON.Json.Serialize(jsonData);

        XDebug.singleton.AddLog("[XPandoraMgr PandoraGetEncodeUrl] param = " + paramStr);
        string newUrl = XUpdater.XUpdater.singleton.XPlatform.GetSDKConfig("get_encoded_url", paramStr);

        XDebug.singleton.AddLog("[XPandoraMgr PandoraGetEncodeUrl] newUrl = " + newUrl);
        return newUrl;
    }

    /// <summary>
    /// 通知潘多拉打开活动模块弹窗
    /// </summary>
    public void PopPLPanel()
    {
        if (XUpdater.XUpdater.singleton.XPlatform.Platfrom() == XPlatformType.Standalone)
        {
            XDebug.singleton.AddLog("PandoraLogin XPlatformType.Standalone");
            return;
        }

        XDebug.singleton.AddGreenLog("Pandora ShowPLPanel");

        //根据优先级由小到大排序
        listActivityPopInfo.Sort(CompareActivityPopInfo);

        for (int i = 0; i < listActivityPopInfo.Count; ++i)
        {
            listActivityPopInfo[i].isClose = false;
        }

        //排队弹拍脸框
        PopSinglePLPanel();
    }

    private void PopSinglePLPanel()
    {
        IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
        if (entrance != null)
            entrance.ResetAllPopPLParent();

        for (int i = 0; i < listActivityPopInfo.Count; ++i)
        {
            if (listActivityPopInfo[i].isReady && listActivityPopInfo[i].isClose == false)
            {
                //特殊关卡失败后，弹preLoss拍脸
                if (listActivityPopInfo[i].activityName == "preLoss")
                {
                    if (bPopPreLossActivity)
                    {
                        bPopPreLossActivity = false;
                    }
                    else
                    {
                        continue;
                    }
                }

                Dictionary<string, string> cmdDict = new Dictionary<string, string>();
                cmdDict["type"] = "pop";
                cmdDict["content"] = listActivityPopInfo[i].activityName;
                string json = MiniJSON.Json.Serialize(cmdDict);
                Pandora.Instance.DoJson(json);

                XDebug.singleton.AddLog("PandoraMgr PopSinglePLPanel json = " + json);
                break;
            }
        }
    }

    public void PopPreLossActivity(bool pop)
    {
        bPopPreLossActivity = pop;
    }

    static int CompareActivityPopInfo(ActivityPopInfo info1, ActivityPopInfo info2)
    {
        return info1.priority - info2.priority;
    }

    /// <summary>
    /// 关闭所有潘多拉面板
    /// </summary>
    public void CloseAllPandoraPanel()
    {
        Dictionary<string, string> cmdDict = new Dictionary<string, string>();
        cmdDict["type"] = "close";
        cmdDict["content"] = "all";
        string json = MiniJSON.Json.Serialize(cmdDict);
        Pandora.Instance.DoJson(json);
    }

    /// <summary>
    /// 关闭潘多拉精彩活动中ui
    /// </summary>
    public void ClosePandoraTabPanel(string module)
    {
        for(int i = 0; i < listActivityTabInfo.Count; ++i)
        {
            if(listActivityTabInfo[i].moduleName == module)
            {
                Dictionary<string, string> cmdDict = new Dictionary<string, string>();
                cmdDict["type"] = "close";
                cmdDict["content"] = listActivityTabInfo[i].activityName;

                string json = MiniJSON.Json.Serialize(cmdDict);
                XDebug.singleton.AddLog("Pandora ClosePandoraTabPanel json = " + json);

                Pandora.Instance.DoJson(json);
            }
        }
    }

    /// <summary>
    /// 通知潘多拉分享结果
    /// </summary>
    /// <param name="result"></param>
    public void NoticePandoraShareResult(string result)
    {
        Dictionary<string, string> cmdDict = new Dictionary<string, string>();
        cmdDict["ret"] = result == "Success" ? "0" : "1";
        cmdDict["msg"] = result == "Success" ? "" : result;
        cmdDict["content"] = sharingContent;

        if (isSharing)
        {
            cmdDict["type"] = "shareRet";
            isSharing = false;
        }
        else if(isBackEndSharing)
        {
            cmdDict["type"] = "backEndShareRet";
            isBackEndSharing = false;
        }
        else if(isPicSharing)
        {
            cmdDict["type"] = "picShareRet";
            isPicSharing = false;
        }

        string json = MiniJSON.Json.Serialize(cmdDict);
        Pandora.Instance.DoJson(json);
    }

    /// <summary>
    /// 通知潘多拉直购结果
    /// </summary>
    /// <param name="result"></param>
    public void NoticePandoraBuyGoodsResult(string result)
    {
        Dictionary<string, string> cmdDict = new Dictionary<string, string>();
        cmdDict["type"] = "midasPayCallback";
        cmdDict["content"] = "rmb";
        cmdDict["result"] = result;
        string json = MiniJSON.Json.Serialize(cmdDict);
        Pandora.Instance.DoJson(json);
    }

    /// <summary>
    /// 是否显示某个活动的tab
    /// </summary>
    /// <param name="activityName"></param>
    /// <returns></returns>
    public bool IsActivityTabShow(string tabName)
    {
        for(int i = 0; i < listActivityTabInfo.Count; ++i)
        {
            if (listActivityTabInfo[i].tabName == tabName && listActivityTabInfo[i].tabShow == true)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 是否显示某个活动的tab
    /// </summary>
    /// <param name="sysID"></param>
    /// <returns></returns>
    public bool IsActivityTabShow(int sysID)
    {
        for (int i = 0; i < listActivityTabInfo.Count; ++i)
        {
            if (listActivityTabInfo[i].sysID == sysID && listActivityTabInfo[i].tabShow == true)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 是否显示某个活动的tab
    /// </summary>
    /// <param name="activityName"></param>
    /// <returns></returns>
    public bool IsActivityTabShowByContent(string tabContent)
    {
        for (int i = 0; i < listActivityTabInfo.Count; ++i)
        {
            if (listActivityTabInfo[i].activityName == tabContent && listActivityTabInfo[i].tabShow == true)
                return true;
        }

        return false;
    }

    public List<ActivityTabInfo> GetAllTabInfo()
    {
        return listActivityTabInfo;
    }

    public List<ActivityPopInfo> GetAllPopInfo()
    {
        return listActivityPopInfo;
    }

    /// <summary>
    /// 消息接收
    /// </summary>
    /// <param name="json"></param>
    public void OnJsonPandoraEvent(string json)
    {
        XDebug.singleton.AddGreenLog("[PandoraMgr]OnJsonPandoraEvent json =" + json);

        _OnJsonPandoraEvent(json);
        HotfixManager.Instance.OnPandoraCallback(json);
    }

    /// <summary>
    /// 消息接收
    /// </summary>
    /// <param name="json"></param>
    private void _OnJsonPandoraEvent(string json)
    {
        Dictionary<string, object> dict = Json.Deserialize(json) as Dictionary<string, object>;
        if (dict == null)
        {
            XDebug.singleton.AddLog("[PandoraMgr]_OnJsonPandoraEvent dict == null");
            return;
        }

        object typeObj;
        if (dict.TryGetValue("type", out typeObj))
        {
            string type = typeObj.ToString();

            switch (type)
            {
                case "pandoraPanelRet": //潘多拉panel创建成功
                    {
                        OnPandoraPanelCreate(dict);
                    }
                    break;
                case "popReady":    //潘多拉通知游戏可以弹出活动弹窗了
                    {
                        OnActivityPopReady(dict);
                    }
                    break;
                case "popStop":     //潘多拉通知游戏停止活动的弹窗
                    {
                        OnActivityPopStop(dict);
                    }
                    break;
                case "popClose":    //潘多拉通知游戏活动面板被玩家关闭了
                    {
                        OnActivityPopClose(dict);
                    }
                    break;
                case "showTab":     //潘多拉通知游戏展示精彩活动里面的Tab
                    {
                        OnActivityShowTab(dict);
                    }
                    break;
                case "hideTab":     //潘多拉通知游戏隐藏精彩活动里面的Tab
                    {
                        OnActivityHideTab(dict);
                    }
                    break;
                case "showRedpoint":    //展示对应活动的红点
                    {
                        OnActivityShowRedPoint(dict);
                    }
                    break;
                case "hideRedpoint":        //隐藏对应活动的红点
                    {
                        OnActivityHideRedPoint(dict);
                    }
                    break;
                case "shareWechatLink":     //微信分享
                    {
                        OnActivityWeChatShare(dict);
                    }
                    break;
                case "shareQQLink":     //手Q分享
                    {
                        OnActivityQQShare(dict);
                    }
                    break;
                case "backEndShareWechat":  //微信后端分享
                    {
                        OnActitityBackEndShareWeChat(dict);
                    }
                    break;
                case "backEndShareQQ":  //QQ后端分享
                    {
                        OnActitityBackEndShareQQ(dict);
                    }
                    break;
                case "picShare":    //大图分享
                    {
                        OnActivityPicShare(dict);
                    }
                    break;
                case "showItemTips":        //"显示道具详情"
                    {
                        OnActivityShowItemTip(dict);
                    }
                    break;
                case "goSystem":    //跳转游戏内系统
                    {
                        OnActivityGoSystem(dict);
                    }
                    break;
                case "openUrl":     //打开链接
                    {
                        OnActivityOpenUrl(dict);
                    }
                    break; 
                case "midasPay":    //道具直购
                    {
                        OnBuyGoods(dict);
                    }
                    break;
                case "goComment":   //跳转appstore好评
                    {
                        OnGoComment(dict);
                    }
                    break;
            }
        }
    }

    private void OnPandoraPanelCreate(Dictionary<string, object> dict)
    {
        //检查panel创建结果
        object panelNameObj;
        if (!dict.TryGetValue("panelName", out panelNameObj))
        {
            return;
        }

        object moduleObj;
        if (! dict.TryGetValue("module", out moduleObj))
        {
            return;
        }

        if (moduleObj.ToString() == "pop")
        {
            object createRetObj;
            if (dict.TryGetValue("ret", out createRetObj))
            {
                int error = -1;
                if (int.TryParse(createRetObj.ToString(), out error))
                {
                    if (error == 0)
                    {
                        XDebug.singleton.AddGreenLog("PandoraPanelCreate error = " + error);

                        IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
                        if (entrance != null)
                            entrance.ShowPandoraPopView(true);
                    }
                }
            }
        }
    }

    private void OnActivityPopReady(Dictionary<string, object> dict)
    {
        object contentObj;
        if (dict.TryGetValue("content", out contentObj))
        {
            XDebug.singleton.AddLog("Pandora OnActivityPopReady content = " + contentObj.ToString());

            object priorityObj = null;
            int priority = 999;
            if (dict.TryGetValue("pri", out priorityObj) )
            {
                int.TryParse(priorityObj.ToString(), out priority);
            }

            object typeObj = null;
            int normal = 1;
            if (dict.TryGetValue("normal", out typeObj))
            {
                int.TryParse(typeObj.ToString(), out normal);
            }

            for (int i = 0; i < listActivityPopInfo.Count; ++i)
            {
                if (listActivityPopInfo[i].activityName == contentObj.ToString())
                {
                    listActivityPopInfo[i].isReady = true;
                    listActivityPopInfo[i].priority = priority;
                    listActivityPopInfo[i].isNomalType = normal;
                    return;
                }
            }

            ActivityPopInfo info = new ActivityPopInfo();
            info.activityName = contentObj.ToString();
            info.isReady = true;
            info.isClose = false;
            info.priority = priority;
            info.isNomalType = normal;
            listActivityPopInfo.Add(info);
        }
    }

    private void OnActivityPopStop(Dictionary<string, object> dict)
    {
        object contentObj;
        if (dict.TryGetValue("content", out contentObj))
        {
            XDebug.singleton.AddLog("Pandora OnActivityPopStop content = " + contentObj.ToString());

            for (int i = 0; i < listActivityPopInfo.Count; ++i)
            {
                if (listActivityPopInfo[i].activityName == contentObj.ToString())
                {
                    listActivityPopInfo[i].isReady = false;
                    return;
                }
            }

            ActivityPopInfo info = new ActivityPopInfo();
            info.activityName = contentObj.ToString();
            info.isReady = false;
            info.isClose = false;
            info.priority = 999;
            listActivityPopInfo.Add(info);
        }
    }

    private void OnActivityPopClose(Dictionary<string, object> dict)
    {
        object contentObj;
        if (dict.TryGetValue("content", out contentObj))
        {
            string content = contentObj.ToString();
            XDebug.singleton.AddLog("Pandora OnActivityPopClose content = " + content);

            for (int i = 0; i < listActivityPopInfo.Count; ++i)
            {
                if (listActivityPopInfo[i].activityName == content)
                {
                    IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
                    if (entrance != null)
                        entrance.ShowPandoraPopView(false);

                    listActivityPopInfo[i].isClose = true;
                    break;
                }
            }

            //排队弹拍脸框
            PopSinglePLPanel();
        }
    }

    private void AddActivityTabInfo(string content, string tabName, bool tabShow, bool redPointShow, string moduleName, int sysID, int sortIndex = -1)
    {
        ActivityTabInfo info = new ActivityTabInfo();
        info.activityName = content;
        info.tabName = tabName;
        info.tabShow = tabShow;
        info.redPointShow = redPointShow;
        info.moduleName = moduleName;
        info.sysID = sysID;
        info.sort = sortIndex;
        listActivityTabInfo.Add(info);

        IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
        if (entrance != null)
        {
            entrance.AttachPandoraSDKRedPoint(sysID, moduleName);
        }
    }

    private void OnActivityShowTab(Dictionary<string, object> dict)
    {
        object contentObj;
        if (! dict.TryGetValue("content", out contentObj))
            return;
        string content = contentObj.ToString();

        object tabNameObj;
        dict.TryGetValue("name", out tabNameObj);
        string tabName = tabNameObj == null ? "TabName" : tabNameObj.ToString();

        object moduleNameObj;
        dict.TryGetValue("module", out moduleNameObj);
        string moduleName = moduleNameObj == null ? "action" : moduleNameObj.ToString();

        object sysIDObj;
        int sysID = 0;
        if (dict.TryGetValue("gameId", out sysIDObj))
            int.TryParse(sysIDObj.ToString(), out sysID);

        object sortObj;
        int sortIndex = 999;
        if (dict.TryGetValue("pri", out sortObj))
            int.TryParse(sortObj.ToString(), out sortIndex);

        XDebug.singleton.AddLog("Pandora OnActivityShowTab content = " + content);

        for (int i = 0; i < listActivityTabInfo.Count; ++i)
        {
            if(listActivityTabInfo[i].activityName == content)
            {
                listActivityTabInfo[i].tabName = tabName;
                listActivityTabInfo[i].tabShow = true;
                listActivityTabInfo[i].sysID = sysID;
                listActivityTabInfo[i].sort = sortIndex;
                return;
            }
        }

        AddActivityTabInfo(content, tabName, true, false, moduleName, sysID, sortIndex);
    }

    private void OnActivityHideTab(Dictionary<string, object> dict)
    {
        object contentObj;
        if (!dict.TryGetValue("content", out contentObj))
            return;

        object tabNameObj;
        dict.TryGetValue("name", out tabNameObj);
        string tabName = tabNameObj == null ? "TabName" : tabNameObj.ToString();
        string content = contentObj.ToString();

        object moduleNameObj;
        dict.TryGetValue("module", out moduleNameObj);
        string moduleName = moduleNameObj == null ? "action" : moduleNameObj.ToString();

        object sysIDObj;
        int sysID = 0;
        if (dict.TryGetValue("gameId", out sysIDObj))
            int.TryParse(sysIDObj.ToString(), out sysID);

        XDebug.singleton.AddLog("Pandora OnActivityHideTab content = " + content);

        for (int i = 0; i < listActivityTabInfo.Count; ++i)
        {
            if (listActivityTabInfo[i].activityName == content)
            {
                listActivityTabInfo[i].tabName = tabName;
                listActivityTabInfo[i].tabShow = false;
                listActivityTabInfo[i].sysID = sysID;
                return;
            }
        }

        AddActivityTabInfo(content, tabName, false, false, moduleName, sysID);
    }

    private void OnActivityShowRedPoint(Dictionary<string, object> dict)
    {
        object contentObj;
        if (!dict.TryGetValue("content", out contentObj))
            return;
        string content = contentObj.ToString();

        object sysIDObj;
        int sysID = 0;
        if (dict.TryGetValue("gameId", out sysIDObj))
            int.TryParse(sysIDObj.ToString(), out sysID);

        object moduleNameObj;
        dict.TryGetValue("module", out moduleNameObj);
        string moduleName = moduleNameObj == null ? "action" : moduleNameObj.ToString();

        bool bFind = false;
        for (int i = 0; i < listActivityTabInfo.Count; ++i)
        {
            if (listActivityTabInfo[i].activityName == content)
            {
                listActivityTabInfo[i].redPointShow = true;
                listActivityTabInfo[i].sysID = sysID;
                bFind = true;
                break;
            }
        }

        if (!bFind)
        {
            object tabNameObj;
            dict.TryGetValue("name", out tabNameObj);
            string tabName = tabNameObj == null ? "TabName" : tabNameObj.ToString();

            AddActivityTabInfo(content, tabName, false, true, moduleName, sysID);
        }

        XDebug.singleton.AddLog("Pandora OnActivityShowRedPoint content = " + content);

        IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
        if (entrance != null)
        {
            entrance.UpdatePandoraSDKRedPoint(sysID, true, moduleName);
        }
    }

     private void OnActivityHideRedPoint(Dictionary<string, object> dict)
    {
        object contentObj;
        if (!dict.TryGetValue("content", out contentObj))
            return;
        string content = contentObj.ToString();

        object sysIDObj;
        int sysID = 0;
        if (dict.TryGetValue("gameId", out sysIDObj))
            int.TryParse(sysIDObj.ToString(), out sysID);

        object moduleNameObj;
        dict.TryGetValue("module", out moduleNameObj);
        string moduleName = moduleNameObj == null ? "action" : moduleNameObj.ToString();

        bool bFind = false;

        for (int i = 0; i < listActivityTabInfo.Count; ++i)
        {
            if (listActivityTabInfo[i].activityName == content)
            {
                listActivityTabInfo[i].redPointShow = false;
                listActivityTabInfo[i].sysID = sysID;
                bFind = true;
                break;
            }
        }

        if (!bFind)
        {
            object tabNameObj;
            dict.TryGetValue("name", out tabNameObj);
            string tabName = tabNameObj == null ? "TabName" : tabNameObj.ToString();

            AddActivityTabInfo(content, tabName, false, false, moduleName, sysID);
        }

        XDebug.singleton.AddLog("Pandora OnActivityHideRedPoint content = " + content);

        IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
        if (entrance != null)
        {
            entrance.UpdatePandoraSDKRedPoint(sysID, false, moduleName);
        }
    }

    private void OnActivityWeChatShare(Dictionary<string, object> dict)
    {
        IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
        if (entrance != null)
        {
            if (!entrance.CheckWXInstalled())
            {
                XDebug.singleton.AddLog("Pandora wx not installed");
                return;
            }
        }
        
        object sceneObj;
        if (! dict.TryGetValue("scene", out sceneObj))
            return;

        object titleObj, descObj, urlObj, mediaTagNameObj, filePathObj, messageExtObj, contentObj;
        dict.TryGetValue("title", out titleObj);
        dict.TryGetValue("desc", out descObj);
        dict.TryGetValue("url", out urlObj);
        dict.TryGetValue("mediaTagName", out mediaTagNameObj);
        dict.TryGetValue("filePath", out filePathObj);
        dict.TryGetValue("messageExt", out messageExtObj);
        dict.TryGetValue("content", out contentObj);

        if (filePathObj != null)
        {
            StartCoroutine(DownloadPic(filePathObj.ToString(), (filepath) =>
            {
                if (!string.IsNullOrEmpty(filepath))
                {
                    Dictionary<string, string> jsondata = new Dictionary<string, string>();
                    jsondata["scene"] = sceneObj == null ? "" : sceneObj.ToString();
                    jsondata["title"] = titleObj == null ? "" : titleObj.ToString();
                    jsondata["desc"] = descObj == null ? "" : descObj.ToString();
                    jsondata["url"] = urlObj == null ? "" : urlObj.ToString();
                    jsondata["mediaTagName"] = mediaTagNameObj == null ? "" : mediaTagNameObj.ToString();
                    jsondata["filePath"] = filepath;
                    jsondata["messageExt"] = messageExtObj == null ? "" : messageExtObj.ToString();
                    string json = MiniJSON.Json.Serialize(jsondata);
                    XDebug.singleton.AddLog("Pandora pandoraShareWX json = " + json);

                    XPlatform platf = gameObject.GetComponent<XPlatform>();
                    if (platf != null)
                    {
                        XDebug.singleton.AddLog("Pandora share_send_to_with_url_wx json = " + json);
                        isSharing = true;
                        sharingContent = contentObj == null ? "" : contentObj.ToString();
                        platf.SendGameExData("share_send_to_with_url_wx", json);
                    }
                }
            }));
        }
    }

    private void OnActivityQQShare(Dictionary<string, object> dict)
    {
        IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
        if (entrance != null)
        {
            if (!entrance.CheckQQInstalled())
            {
                XDebug.singleton.AddLog("Pandora qqshare not installed");
                return;
            }
        }

        object sceneObj;
        if (!dict.TryGetValue("scene", out sceneObj))
            return;

        object titleObj, summaryObj, targetUrlObj, imageUrlObjj, contentObj;
        dict.TryGetValue("title", out titleObj);
        dict.TryGetValue("summary", out summaryObj);
        dict.TryGetValue("targetUrl", out targetUrlObj);
        dict.TryGetValue("imageUrl", out imageUrlObjj);
        dict.TryGetValue("content", out contentObj);

        if (imageUrlObjj != null)
        {
            StartCoroutine(DownloadPic(imageUrlObjj.ToString(), (filepath) =>
            {
                if (!string.IsNullOrEmpty(filepath))
                {
                    Dictionary<string, string> jsondata = new Dictionary<string, string>();
                    jsondata["scene"] = sceneObj == null ? "" : sceneObj.ToString();
                    jsondata["title"] = titleObj == null ? "" : titleObj.ToString();
                    jsondata["summary"] = summaryObj == null ? "" : summaryObj.ToString();
                    jsondata["targetUrl"] = targetUrlObj == null ? "" : targetUrlObj.ToString();
                    jsondata["imageUrl"] = filepath;
                    string json = MiniJSON.Json.Serialize(jsondata);
                    XDebug.singleton.AddLog("Pandora pandoraShareQQ json = " + json);

                    XPlatform platf = gameObject.GetComponent<XPlatform>();
                    if (platf != null)
                    {
                        isSharing = true;
                        sharingContent = contentObj == null ? "" : contentObj.ToString();
                        platf.SendGameExData("share_send_to_struct_qq", json);
                    }
                }
            }));
        }
    }

    private void OnActitityBackEndShareWeChat(Dictionary<string, object> dict)
    {
        object openIDObj, titleObj, descObj, mediaTagNameObj, contentObj;
        dict.TryGetValue("openid", out openIDObj);
        dict.TryGetValue("title", out titleObj);
        dict.TryGetValue("desc", out descObj);
        dict.TryGetValue("mediaTagName", out mediaTagNameObj);
        dict.TryGetValue("content", out contentObj);

        if (openIDObj == null || titleObj == null || descObj == null || mediaTagNameObj == null)
        {
            XDebug.singleton.AddLog("Pandora OnActitityBackEndShareWeChat param == null");
            return;
        }

        IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
        if (entrance != null)
        {
            isBackEndSharing = true;
            sharingContent = contentObj == null ? "" : contentObj.ToString();
            entrance.ShareToWXFriendBackEnd(openIDObj.ToString(), titleObj.ToString(), descObj.ToString(), mediaTagNameObj.ToString());
        }
    }

    private void OnActitityBackEndShareQQ(Dictionary<string, object> dict)
    {
        object openIDObj, titleObj, descObj, imageUrlObj, targetUrlObj, previewTextObj, mediaTagNameObj, contentObj;
        dict.TryGetValue("openid", out openIDObj);
        dict.TryGetValue("title", out titleObj);
        dict.TryGetValue("desc", out descObj);
        dict.TryGetValue("imageUrl", out imageUrlObj);
        dict.TryGetValue("targetUrl", out targetUrlObj);
        dict.TryGetValue("previewText", out previewTextObj);
        dict.TryGetValue("gameTag", out mediaTagNameObj);
        dict.TryGetValue("content", out contentObj);

        if (openIDObj == null || titleObj == null || descObj == null || imageUrlObj == null || targetUrlObj == null || previewTextObj == null
            || mediaTagNameObj == null)
        {
            XDebug.singleton.AddLog("Pandora OnActitityBackEndShareQQ param == null");
            return;
        }

        IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
        if (entrance != null)
        {
            isBackEndSharing = true;
            sharingContent = contentObj == null ? "" : contentObj.ToString();
            entrance.ShareToQQFreindBackEnd(openIDObj.ToString(), titleObj.ToString(), descObj.ToString(), mediaTagNameObj.ToString(), targetUrlObj.ToString(), imageUrlObj.ToString(), previewTextObj.ToString());
        }
    }

    private void OnActivityPicShare(Dictionary<string, object> dict)
    {
        object accountObj, sceneObj, gameObjPathObj, contentObj;
        dict.TryGetValue("account", out accountObj);
        dict.TryGetValue("scene", out sceneObj);
        dict.TryGetValue("gameObjPath", out gameObjPathObj);
        dict.TryGetValue("content", out contentObj);

        if (accountObj == null || sceneObj == null || gameObjPathObj == null)
        {
            XDebug.singleton.AddLog("Pandora OnActivityPicShare param == null");
            return;
        }

        IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
        if (entrance != null)
        {
            if (accountObj.ToString() == "qq" && !entrance.CheckQQInstalled())
            {
                XDebug.singleton.AddLog("Pandora qqshare not installed");
                return;
            }
            else if(accountObj.ToString() == "wx" && !entrance.CheckWXInstalled())
            {
                XDebug.singleton.AddLog("Pandora wxshare not installed");
                return;
            }

            isPicSharing = true;
            sharingContent = contentObj == null ? "" : contentObj.ToString();
            entrance.PandoraPicShare(accountObj.ToString(), sceneObj.ToString(), gameObjPathObj.ToString());
        }
    }

    private void OnActivityShowItemTip(Dictionary<string, object> dict)
    {
        object itemIdObj;
        if (dict.TryGetValue("itemId", out itemIdObj))
        {
            int itemId = 0;
            if (int.TryParse(itemIdObj.ToString(), out itemId))
            {
                XDebug.singleton.AddLog("Pandora itemId = " + itemId);

                //显示道具详情
                IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
                if (entrance != null)
                    entrance.ShowTooltipDialog(itemId);
            }
        }
    }

    private void OnActivityGoSystem(Dictionary<string, object> dict)
    {
        object contentObj;
        if (dict.TryGetValue("content", out contentObj))
        {
            int sysID = 0;
            if (int.TryParse(contentObj.ToString(), out sysID))
            {
                IGameSysMgr entrance = XInterfaceMgr.singleton.GetInterface<IGameSysMgr>(XCommon.singleton.XHash("IGameSysMgr"));
                if (entrance != null)
                    entrance.OpenSystem(sysID);
            }
        }
    }

    private void OnActivityOpenUrl(Dictionary<string, object> dict)
    {
        object contentObj;
        if (dict.TryGetValue("content", out contentObj))
        {
            bool landscape = true;

            object landscapeObj;
            if (dict.TryGetValue("landscape", out landscapeObj))
            {
                int way = 0;
                if (int.TryParse(landscapeObj.ToString(), out way))
                {
                    if (way == 0)
                        landscape = false;
                }
            }

            IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
            if (entrance != null)
                entrance.OpenUrl(contentObj.ToString(), landscape);
        }
    }

    private void OnBuyGoods(Dictionary<string, object> dict)
    {
        object contentObj;
        if (!dict.TryGetValue("content", out contentObj))
            return;

        if (contentObj.ToString() != "rmb")
            return;

        //通用参数
        object offerIdObj;
        if (! dict.TryGetValue("offerId", out offerIdObj))
            return;

        if (XUpdater.XUpdater.singleton.XPlatform.Platfrom() == XPlatformType.Android)
        {
            //安卓参数
            object payUrlObj;
            if (!dict.TryGetValue("payUrl", out payUrlObj))
                return;

            Dictionary<string, object> jsonData = new Dictionary<string, object>();
            jsonData["offerId"] = offerIdObj.ToString();
            jsonData["zoneId"] = "1";
            jsonData["tokenUrl"] = payUrlObj.ToString();

            string paramStr = MiniJSON.Json.Serialize(jsonData);

            XDebug.singleton.AddLog("[XPandoraMgr OnBuyGoods] param = " + paramStr);
            IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
            if (entrance != null)
                entrance.SDKPandoraBuyGoods(paramStr);
        }
        else if (XUpdater.XUpdater.singleton.XPlatform.Platfrom() == XPlatformType.IOS)
        {
            //ios参数
            object productIdUrlObj;
            if (!dict.TryGetValue("productId", out productIdUrlObj))
                return;

            object postPfObj;
            if (!dict.TryGetValue("postPf", out postPfObj))
                return;

            object buyNumObj;
            if (!dict.TryGetValue("buyNum", out buyNumObj))
                return;

            Dictionary<string, object> jsonData = new Dictionary<string, object>();
            jsonData["offerId"] = offerIdObj.ToString();
            jsonData["zoneId"] = "1";
            jsonData["productId"] = productIdUrlObj.ToString();
            jsonData["buyNum"] = buyNumObj.ToString(); //道具id*道具单价角*数量
            jsonData["pfExt"] = postPfObj.ToString(); 

            string paramStr = MiniJSON.Json.Serialize(jsonData);

            XDebug.singleton.AddLog("[XPandoraMgr OnBuyGoods] param = " + paramStr);
            IUiUtility entrance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
            if (entrance != null)
                entrance.SDKPandoraBuyGoods(paramStr);
        }
    }

    private void OnGoComment(Dictionary<string, object> dict)
    {
        object urlObj;
        if (!dict.TryGetValue("content", out urlObj))
            return;

        Application.OpenURL(urlObj.ToString());
    }

    IEnumerator DownloadPic(string path, Action<string> callback)
    {
        int hash = Mathf.Abs(path.GetHashCode());
        string dir = Application.temporaryCachePath + "/ImageCache/";
        try
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }
        catch
        {
        }

        string pa = dir + hash;
        if (! File.Exists(pa))
        {
            string url = path;
            print(url + " pa: " + pa);
            WWW w = new WWW(url);
            while (!w.isDone)
                yield return w;

            if (!string.IsNullOrEmpty(w.error))
            {
                Debug.LogError("error:" + w.error);
                pa = "";
            }
            else
            {
                try
                {
                    Texture2D image = w.texture;
                    byte[] pngData = image.EncodeToJPG();
                    File.WriteAllBytes(pa, pngData);
                }
                catch (Exception e)
                {
                    pa = "";
                    Debug.LogError("wraite error" + e.StackTrace);
                }
            }

            callback(pa);

            w.Dispose();
            w = null;
        }
        else
        {
            callback(pa);
        }
    }

    public Bounds GetBoundsIncludesChildren(Transform trans)
    {
        return NGUIMath.CalculateAbsoluteWidgetBounds(trans);
    }
}
