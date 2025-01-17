using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 通用Http，Https访问基础设施
/// </summary>
namespace com.tencent.pandora
{
    public class WwwLoader: MonoBehaviour
    {
        private const int MAX_RETRY_COUNT = 5;

        private int _retryCount = 0;

        //访问云端CGI
        public void LoadRemoteConfig(UserData userData, Action<RemoteConfig> callback)
        {
            StartCoroutine(GetRemoteConfig(userData, callback));
        }

        private IEnumerator GetRemoteConfig(UserData userData, Action<RemoteConfig> callback)
        {
            string url = string.Empty;
            WWW www = null;
            if(PandoraSettings.RequestCgiByPost == true)
            {
                url = PandoraSettings.GetCgiUrl(userData.sAppId);
                Dictionary<string, string> headers = GenerateCgiHeaders(userData);
                byte[] postData = GenerateCgiPostData(userData);
                www = new WWW(url, postData, headers);
            }
            else
            {
                url = PandoraSettings.GetCgiUrl(userData.sAppId) + "?" + GenerateCgiGetParams(userData);
                www = new WWW(url);
            }
            Logger.LogInfo("连接接CGI： " + url + " 开始时间: " + Time.realtimeSinceStartup);
            yield return www;
            
            if (string.IsNullOrEmpty(www.error))
            {
                Logger.LogInfo("连接CGI成功： " + url + " 结束时间: " + Time.realtimeSinceStartup);
                RemoteConfig remoteConfig = ParseCgiResponse(www.bytes);
                callback(remoteConfig);
                www.Dispose();
                www = null;
            }
            else
            {
                yield return new WaitForSeconds(10.0f);
                _retryCount++;
                if (_retryCount < MAX_RETRY_COUNT)
                {
                    Logger.LogError("请求Cgi数据失败， Error: " + www.error + " 重连次数： " + _retryCount.ToString());
                    www.Dispose();
                    www = null;
                    LoadRemoteConfig(userData, callback);
                }
                else
                {
                    string error = "请求Cgi数据失败，且超过最大重试次数， Error：" + www.error;
                    Logger.LogError(error);
                    Pandora.Instance.ReportError(error, ErrorCodeConfig.CGI_TIMEOUT);
                    Pandora.Instance.Report("Cgi timeout, " + error, ErrorCodeConfig.CGI_TIMEOUT_DETAIL, ErrorCodeConfig.TNM2_TYPE_LITERALS);
                    www.Dispose();
                    www = null;
                    callback(new RemoteConfig("{}"));
                }
            }
        }

        private Dictionary<string, string> GenerateCgiHeaders(UserData userData)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("User-Agent", "Pandora(" + userData.sSdkVersion + ")");
            return result;
        }

        private string GenerateCgiGetParams(UserData userData)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("openid=");
            builder.Append(userData.sOpenId);
            builder.Append("&partition=");
            builder.Append(userData.sPartition);
            builder.Append("&gameappversion=");
            builder.Append(userData.sGameVer);
            builder.Append("&areaid=");
            builder.Append(userData.sArea);
            builder.Append("&appid=");
            builder.Append(userData.sAppId);
            builder.Append("&acctype=");
            builder.Append(userData.sAcountType);
            builder.Append("&platid=");
            builder.Append(userData.sPlatID);
            builder.Append("&sdkversion=");
            builder.Append(userData.sSdkVersion);
            builder.Append("&_pdr_time=");
            builder.Append(TimeHelper.NowSeconds());
            return builder.ToString();
        }

        private string GenerateCgiPostParams(UserData userData)
        {
            Dictionary<string, string> postParams = new Dictionary<string, string>();
            postParams["openid"] = userData.sOpenId;
            postParams["partition"] = userData.sPartition;
            postParams["gameappversion"] = userData.sGameVer;
            postParams["areaid"] = userData.sArea;
            postParams["acctype"] = userData.sAcountType;
            postParams["platid"] = userData.sPlatID;
            postParams["appid"] = userData.sAppId;
            postParams["sdkversion"] = userData.sSdkVersion;
            string serialPostParams = MiniJSON.Json.Serialize(postParams);
            return MsdkTea.Encode(serialPostParams);
        }

        private byte[] GenerateCgiPostData(UserData userData)
        {
            string postContent = "{\"data\":\"" + GenerateCgiPostParams(userData) + "\",\"encrypt\" : \"true\"}";
            return Encoding.UTF8.GetBytes(postContent);
        }

        private RemoteConfig ParseCgiResponse(byte[] data)
        {
            string response = string.Empty;
            try
            {
                if (PandoraSettings.RequestCgiByPost == true)
                {
                    string rawContent = Encoding.UTF8.GetString(data);
                    Dictionary<string, System.Object> dict = MiniJSON.Json.Deserialize(rawContent) as Dictionary<string, System.Object>;
                    byte[] base4EncodedBytes = Convert.FromBase64String(dict["data"] as string);
                    response = MsdkTea.Decode(base4EncodedBytes);
                }
                else
                {
                    response = Encoding.UTF8.GetString(data);
                }
                Logger.LogInfo("获得RemoteConfig： " + response);
            }
            catch (Exception e)
            {
                Logger.LogError("解析RemoteConfig发生异常： " + e.Message);
            }
            if (string.IsNullOrEmpty(response))
            {
                return new RemoteConfig("{}");
            }
            return new RemoteConfig(response);
        }

        //通用访问Url
        public void LoadWww(string url, string requestJson, bool isPost, Action<string> callback)
        {
            StartCoroutine(RequestWww(url, requestJson, isPost, callback));
        }

        private IEnumerator RequestWww(string url, string requestJson, bool isPost, Action<string> callback)
        {
            WWW www = null;
            if(isPost == true)
            {
                byte[] postData = GeneratePostData(requestJson);
                www = new WWW(url, postData);
                Logger.Log("Post网址： " + url + " PostData: " + requestJson);
            }
            else
            {
                url = url + GenerateGetParams(requestJson);
                www = new WWW(url);
                Logger.Log("Get网址： " + url);
            }
            yield return www;
            if(string.IsNullOrEmpty(www.error))
            {
                string result = Encoding.UTF8.GetString(www.bytes);
                callback(result);
            }
            else
            {
                string error = "访问网址出错，url: " + url + "  " + www.error;
                Logger.LogError(error);
                Pandora.Instance.ReportError(error);
                callback(string.Empty);
            }
            www.Dispose();
            www = null;
        }

        private string GenerateGetParams(string requestJson)
        {
            if (string.IsNullOrEmpty(requestJson))
            {
                return string.Empty;
            }
            Dictionary<string, System.Object> dict = MiniJSON.Json.Deserialize(requestJson) as Dictionary<string, System.Object>;
            if(dict == null)
            {
                return string.Empty;
            }
            bool first = true;
            StringBuilder builder = new StringBuilder();

            var enumerator = dict.Keys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string key = enumerator.Current;
                if (dict[key] != null)
                {
                    if (first == true)
                    {
                        first = false;
                        builder.Append("?");
                    }
                    else
                    {
                        builder.Append("&");
                    }
                    builder.Append(key + "=");
                    builder.Append(dict[key].ToString());
                }
            }
            return builder.ToString();
        }

        private byte[] GeneratePostData(string requestJson)
        {
            if(string.IsNullOrEmpty(requestJson))
            {
                return new byte[0];
            }
            return Encoding.UTF8.GetBytes(requestJson);
        }

        public void Clear()
        {
            _retryCount = 0;
        }

    }
}
