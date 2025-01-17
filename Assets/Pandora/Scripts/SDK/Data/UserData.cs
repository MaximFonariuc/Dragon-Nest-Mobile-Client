using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace com.tencent.pandora
{
    public class UserData
    {
        public string sRoleId = ""; // 以roleid作为账号切换的唯一判定
        public string sOpenId = "";
        public string sServiceType = "";
        public string sAcountType = "";
        public string sArea = "";
        public string sPartition = "";
        public string sAppId = "";
        public string sAccessToken = "";
        public string sPayToken = "";
        public string sGameVer = "";
        public string sPlatID = "";
        public string sQQInstalled = "";
        public string sWXInstalled = "";
        public string sGameName = "";
        public string sSdkVersion = "";

        public string sParam0 = ""; //预留1
        public string sParam1 = ""; //预留2

        // 是否为空
        public bool IsRoleEmpty()
        {
            return string.IsNullOrEmpty(sRoleId);
        }

        // 账号切换时，需要清空
        public void Clear()
        {
            sRoleId = "";
            sOpenId = "";
            sServiceType = "";
            sAcountType = "";
            sArea = "";
            sPartition = "";
            sAppId = "";
            sAccessToken = "";
            sPayToken = "";
            sGameVer = "";
            sPlatID = "";
            sQQInstalled = "";
            sWXInstalled = "";
            sGameName = "";
            sSdkVersion = "";
            sParam0 = "";
            sParam1 = "";
        }

        // 账号切换
        public void Assign(Dictionary<string, string> data)
        {
            if (data.ContainsKey("sRoleId"))
            {
                sRoleId = data["sRoleId"];
            }
            if (data.ContainsKey("sOpenId"))
            {
                sOpenId = data["sOpenId"];
            }
            if (data.ContainsKey("sServiceType"))
            {
                sServiceType = data["sServiceType"];
            }
            if (data.ContainsKey("sAcountType"))
            {
                sAcountType = data["sAcountType"];
            }
            if (data.ContainsKey("sArea"))
            {
                sArea = data["sArea"];
            }
            if (data.ContainsKey("sPartition"))
            {
                sPartition = data["sPartition"];
            }
            if (data.ContainsKey("sAppId"))
            {
                sAppId = data["sAppId"];
            }
            if (data.ContainsKey("sAccessToken"))
            {
                sAccessToken = data["sAccessToken"];
            }
            if (data.ContainsKey("sPayToken"))
            {
                sPayToken = data["sPayToken"];
            }
            if (data.ContainsKey("sGameVer"))
            {
                sGameVer = data["sGameVer"];
            }
            if (data.ContainsKey("sPlatID"))
            {
                sPlatID = data["sPlatID"];
            }
            if (data.ContainsKey("sQQInstalled"))
            {
                sQQInstalled = data["sQQInstalled"];
            }
            if (data.ContainsKey("sWXInstalled"))
            {
                sWXInstalled = data["sWXInstalled"];
            }
            if (data.ContainsKey("sGameName"))
            {
                sGameName = data["sGameName"];
            }
            if (data.ContainsKey("sSdkVersion"))
            {
                sSdkVersion = data["sSdkVersion"];
            }
            if (data.ContainsKey("sParam0"))
            {
                sParam0 = data["sParam0"];
            }
            if (data.ContainsKey("sParam1"))
            {
                sParam1 = data["sParam1"];
            }
        }

        // 相同角色的情况下刷新sAssessToken和sPayToken
        public void Refresh(Dictionary<string, string> data)
        {
            string newRoleId = data["sRoleId"];
            if (string.IsNullOrEmpty(newRoleId) || string.IsNullOrEmpty(sRoleId) || newRoleId != sRoleId)
            {
                return;
            }

            if (data.ContainsKey("sAccessToken"))
            {
                sAccessToken = data["sAccessToken"];
            }
            if (data.ContainsKey("sPayToken"))
            {
                sPayToken = data["sPayToken"];
            }
            Logger.Log("Refresh UserData: " + this.ToString());
        }

        public string GetUrlParams()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("sOpenId=");
            sb.Append(sOpenId);
            sb.Append("&sAcountType=");
            sb.Append(sAcountType);
            sb.Append("&sAppId=");
            sb.Append(sAppId);
            sb.Append("&sPlatID=");
            sb.Append(sPlatID);
            sb.Append("&sAppId=");
            sb.Append(sAppId);
            sb.Append("&sAccessToken=");
            sb.Append(sAccessToken);
            sb.Append("&sPayToken=");
            sb.Append(sPayToken);
            sb.Append("&sSdkVersion=");
            sb.Append(sSdkVersion);
            sb.Append("&sArea=");
            sb.Append(sArea);
            sb.Append("&sPartition=");
            sb.Append(sPartition);
            sb.Append("&sGameVer=");
            sb.Append(sGameVer);
            sb.Append("&sParam0=");
            sb.Append(sParam0);
            sb.Append("&sParam1=");
            sb.Append(sParam1);
            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<color=#0000ff>UserData: ");
            sb.Append("sOpenId=");
            sb.Append(sOpenId);
            sb.Append("&sAcountType=");
            sb.Append(sAcountType);
            sb.Append("&sAppId=");
            sb.Append(sAppId);
            sb.Append("&sPlatID=");
            sb.Append(sPlatID);
            sb.Append("&sAppId=");
            sb.Append(sAppId);
            sb.Append("&sAccessToken=");
            sb.Append(sAccessToken);
            sb.Append("&sPayToken=");
            sb.Append(sPayToken);
            sb.Append("&sSdkVersion=");
            sb.Append(sSdkVersion);
            sb.Append("&sArea=");
            sb.Append(sArea);
            sb.Append("&sPartition=");
            sb.Append(sPartition);
            sb.Append("&sGameName=");
            sb.Append(sGameName);
            sb.Append("&sGameVer=");
            sb.Append(sGameVer);
            sb.Append("&sQQInstalled=");
            sb.Append(sQQInstalled);
            sb.Append("&sWXInstalled=");
            sb.Append(sWXInstalled);
            sb.Append("&sParam0=");
            sb.Append(sParam0);
            sb.Append("&sParam1=");
            sb.Append(sParam1);
            sb.Append("</color>");
            return sb.ToString();
        }
    }
}

