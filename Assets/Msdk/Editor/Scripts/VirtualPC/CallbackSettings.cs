using UnityEngine;
using System;
using System.IO;
using LitJson;

class CallbackSettings : ScriptableObject
{
    public string[] loginStates = {
                                      "eFlag_Succ",
                                      "eFlag_QQ_LoginFail",
                                      "eFlag_QQ_NotInstall",
                                      "eFlag_WX_LoginFail",
                                      "eFlag_WX_NotInstall",
                                      "eFlag_Local_Invalid"
                                  };

    public string[] authStates = { "eFlag_Succ", "eFlag_Error" };

    public string[] shareStates = {
                                      "eFlag_Succ",
                                      "eFlag_QQ_UserCancel",
                                      "eFlag_WX_UserCancel",
                                      "eFlag_WX_NotInstall"
                                  };
    public string[] relationStates = {"eFlag_Succ", "eFlag_Error"};
    public string[] wakeupStates = {
                                       "无回调",
                                       "eFlag_Succ",
                                       "eFlag_UrlLogin",
                                       "eFlag_NeedSelectAccount",
                                       "eFlag_NeedLogin"
                                   };
    public string[] groupStates = { "eFlag_Succ", "eFlag_Error" };
    public string[] cardStates = { "eFlag_Succ", "eFlag_Error" };
    public string[] lbsStates = { "eFlag_Succ", "eFlag_LbsNeedOpenLocationService", "eFlag_LbsLocateFail"};
    // 各配置项索引
    private int loginIndex, authIndex, shareIndex, relationIndex, wakeupIndex, groupIndex, cardIndex, locationIndex, nearbyIndex;
    // 回调配置项
    public CallbackStates callback;

    private static CallbackSettings instance;

    public static CallbackSettings Instance
    {
        get
        {
            if (instance == null) {
                instance = CreateInstance("CallbackSettings") as CallbackSettings;
            }
            return instance;
        }
    }

    private CallbackSettings()
    {
        callback = CallbackStates.Load();
        
        if (callback == null || String.IsNullOrEmpty(callback.loginState)) {
            // 若为空，则对配置初始化
            callback = new CallbackStates();
            callback.loginState = loginStates[loginIndex];
            callback.authState = authStates[authIndex];
            callback.shareState = shareStates[shareIndex];
            callback.relationState = relationStates[relationIndex];
            callback.wakeupState = wakeupStates[wakeupIndex];
            callback.groupState = groupStates[groupIndex];
            callback.cardState = cardStates[cardIndex];
            callback.locationState = lbsStates[locationIndex];
            callback.nearbyState = lbsStates[nearbyIndex];
            callback.Update();
        } else {
            // 更新索引值
            try {
                loginIndex = Array.IndexOf(loginStates, callback.loginState, 0);
                authIndex = Array.IndexOf(authStates, callback.authState, 0);
                shareIndex = Array.IndexOf(shareStates, callback.shareState, 0);
                relationIndex = Array.IndexOf(relationStates, callback.relationState, 0);
                wakeupIndex = Array.IndexOf(wakeupStates, callback.wakeupState, 0);
                groupIndex = Array.IndexOf(groupStates, callback.groupState, 0);
                cardIndex = Array.IndexOf(cardStates, callback.cardState, 0);
                locationIndex = Array.IndexOf(lbsStates, callback.locationState, 0);
                nearbyIndex = Array.IndexOf(lbsStates, callback.nearbyState, 0);
            } catch (Exception e) {
                Debug.LogWarning(e.Message);
            }
        }
        
    }

    public int LoginIndex
    {
        get
        {
            return loginIndex;
        }
        set
        {
            if (loginIndex != value && value < loginStates.Length) {
                loginIndex = value;
                callback.loginState = loginStates[loginIndex];
                callback.Update();
            }
        }
    }

    public int AuthIndex
    {
        get
        {
            return authIndex;
        }
        set
        {
            if (authIndex != value && value < authStates.Length) {
                authIndex = value;
                callback.authState = authStates[authIndex];
                callback.Update();
            }
        }
    }

    public int ShareIndex
    {
        get
        {
            return shareIndex;
        }
        set
        {
            if (shareIndex != value && value < shareStates.Length) {
                shareIndex = value;
                callback.shareState = shareStates[shareIndex];
                callback.Update();
            }
        }
    }

    public int RelationIndex
    {
        get
        {
            return relationIndex;
        }
        set
        {
            if (relationIndex != value && value < relationStates.Length) {
                relationIndex = value;
                callback.relationState = relationStates[relationIndex];
                callback.Update();
            }
        }
    }

    public int WakeupIndex
    {
        get
        {
            return wakeupIndex;
        }
        set
        {
            if (wakeupIndex != value && value < wakeupStates.Length) {
                wakeupIndex = value;
                callback.wakeupState = wakeupStates[wakeupIndex];
                callback.Update();
            }
        }
    }

    public int GroupIndex
    {
        get
        {
            return groupIndex;
        }
        set
        {
            if (groupIndex != value && value < groupStates.Length) {
                groupIndex = value;
                callback.groupState = groupStates[groupIndex];
                callback.Update();
            }
        }
    }

    public int CardIndex
    {
        get
        {
            return cardIndex;
        }
        set
        {
            if (cardIndex != value && value < cardStates.Length) {
                cardIndex = value;
                callback.cardState = cardStates[cardIndex];
                callback.Update();
            }
        }
    }

    public int LocationIndex
    {
        get
        {
            return locationIndex;
        }
        set
        {
            if (locationIndex != value && value < lbsStates.Length) {
                locationIndex = value;
                callback.locationState = lbsStates[locationIndex];
                callback.Update();
            }
        }
    }

    public int NearbyIndex
    {
        get
        {
            return nearbyIndex;
        }
        set
        {
            if (nearbyIndex != value && value < lbsStates.Length) {
                nearbyIndex = value;
                callback.nearbyState = lbsStates[nearbyIndex];
                callback.Update();
            }
        }
    }
}
