using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.tencent.pandora
{
    public class RemoteConfig
    {
        public int ruleId;
        /// <summary>
        /// 总量开关
        /// </summary>
        public bool totalSwitch;
        /// <summary>
        /// 是否要将Log保存在本地
        /// </summary>
        public bool isDebug;
        /// <summary>
        /// 是否上报日志到服务器
        /// </summary>
        public bool isNetLog;
        public bool isShowLogo;
        /// <summary>
        /// 上报日志的等级
        /// </summary>
        public int netLogLevel;
        public string brokerHost;
        public int brokerPort;
        public string brokerAlternateIp1;
        public string brokerAlternateIp2;
        /// <summary>
        /// 功能开关字典
        /// </summary>
        public Dictionary<string, bool> functionSwitchDict;
        /// <summary>
        /// 资源配置
        /// </summary>
        public Dictionary<string, List<AssetInfo>> assetInfoListDict;
        public Dictionary<string, AssetInfo> assetInfoDict;

        /// <summary>
        /// 返回数据内容为空
        /// </summary>
        public bool IsEmpty { get; private set; }

        public bool IsError { get; private set; }

        public RemoteConfig(string response)
        {
            Parse(response);
        }

        public bool GetFunctionSwitch(string functionName)
        {
            if (functionSwitchDict.ContainsKey(functionName))
            {
                return functionSwitchDict[functionName];
            }
            return false;
        }

        public string GetAssetPath(string assetName)
        {
            if(assetInfoDict.ContainsKey(assetName) == true)
            {
                return assetInfoDict[assetName].url;
            }
            return string.Empty;
        }

        public AssetInfo GetAssetInfo(string name)
        {
            if (assetInfoDict.ContainsKey(name) == true)
            {
                return assetInfoDict[name];
            }
            return null;
        }

        private void Parse(string response)
        {
            try
            {
                if(response == "{}")
                {
                    IsError = true;
                    return;
                }
                Dictionary<string, System.Object> configDict = MiniJSON.Json.Deserialize(response) as Dictionary<string, System.Object>;
                if(configDict == null)
                {
                    IsError = true;
                    return;
                }
                // 4、是否Debug模式，是则日志落地，不是则忽略，默认不是Debug模式
                Pandora.Instance.IsDebug = (ParseIntField(configDict, "isDebug", 0) == 1);

                //1.规则Id
                this.ruleId = ParseIntField(configDict, "id", 0);
                if(this.ruleId == 0)
                {
                    Logger.Log("<color=#ff0000>没有匹配到任何规则</color>");
                    IsEmpty = true;
                    return;
                }
                 
                //2.总量开关
                this.totalSwitch = (ParseIntField(configDict, "totalSwitch") == 1);
                if(this.totalSwitch == false)
                {
                    Logger.Log("<color=#ff0000>总量开关关闭，规则Id： " + this.ruleId.ToString() + "</color>");
                    IsEmpty = true;
                    return;
                }

                //3.功能开关
                this.functionSwitchDict = ParseFuntionSwitch(configDict);

                // 4.1、是否需要日志上报，默认需要，除非遇到上报异常情况，才取消上报
                this.isNetLog = (ParseIntField(configDict, "isNetLog", 1) == 1);
                this.netLogLevel = ParseIntField(configDict, "log_level", 0);
                this.isShowLogo = (ParseStringField(configDict, "is_show_logo") == "true");

                // 5、broker host, port, cap_ip1, cap_ip2
                this.brokerHost = ParseStringField(configDict, "ip");
                this.brokerPort = ParseIntField(configDict, "port", 0);
                if(string.IsNullOrEmpty(this.brokerHost) || this.brokerPort == 0)
                {
                    string error = "Broker域名或端口配置错误，规则Id： " + this.ruleId.ToString();
                    Logger.LogError(error);
                    Pandora.Instance.ReportError(error);
                    this.IsError = true;
                    return;
                }

                // 5、cap_ip1, cap_ip2
                this.brokerAlternateIp1 = ParseStringField(configDict, "cap_ip1");
                this.brokerAlternateIp2 = ParseStringField(configDict, "cap_ip2");
                if(string.IsNullOrEmpty(this.brokerAlternateIp1) || string.IsNullOrEmpty(this.brokerAlternateIp2))
                {
                    Logger.LogWarning("Broker的备选IP1或IP2没有配置");
                }

                // 6、解析资源列表sourcelist，返回key为资源name，Value为FileInfo的dict
                bool hasError;
                this.assetInfoDict = ParseAssetInfoDict(configDict, out hasError);
                if(hasError == true)
                {
                    this.IsError = true;
                    return;
                }

                // 7、解析文件分组字典
                this.assetInfoListDict = ParseAssetInfoListDict(configDict, assetInfoDict, out hasError);
                if(hasError == true)
                {
                    this.IsError = true;
                    return;
                }
            }
            catch (Exception e)
            {
                string error = "解析RemoteConfig发生错误： " + e.Message;
                Logger.LogError(error);
                Pandora.Instance.ReportError(error);
                IsError = true;
            }
        }

        private int ParseIntField(Dictionary<string, System.Object> content, string fieldName, int defaultValue = -1)
        {
            int result = defaultValue;
            if (content.ContainsKey(fieldName) == true)
            {
                result = Convert.ToInt32(content[fieldName]);
            }
            return result;
        }

        public ushort ParseUShortField(Dictionary<string, System.Object> content, string fieldName, ushort defaultValue = 0)
        {
            ushort result = defaultValue;
            if (content.ContainsKey(fieldName) == true)
            {
                result = Convert.ToUInt16(content[fieldName]);
            }
            return result;
        }

        private string ParseStringField(Dictionary<string, System.Object> content, string fieldName)
        {
            string result = string.Empty;
            if (content.ContainsKey(fieldName) == true)
            {
                result = content[fieldName] as string;
            }
            return result;
        }

        private Dictionary<string, bool> ParseFuntionSwitch(Dictionary<string, System.Object> content)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            string switchContent = ParseStringField(content, "function_switch");
            if (string.IsNullOrEmpty(switchContent) == false)
            {
                string[] switchItems = switchContent.Split(',');
                for(int i = 0; i < switchItems.Length; i++)
                {
                    string item = switchItems[i];
                    string[] segments = item.Split(':');
                    if (segments.Length == 2)
                    {
                        string key = segments[0];
                        int value = System.Convert.ToInt32(segments[1]);
                        result[key] = (value == 1);
                    }
                }
            }
            return result;
        }

        private Dictionary<string, AssetInfo> ParseAssetInfoDict(Dictionary<string, System.Object> content, out bool hasError)
        {
            hasError = false;
            Dictionary<string, AssetInfo> result = new Dictionary<string, AssetInfo>();
            if (content.ContainsKey("sourcelist") == false)
            {
                Logger.LogError("文件列表配置错误，未发现sourcelist字段");
                hasError = true;
                return result;
            }
            Dictionary<string, System.Object> sourceList = content["sourcelist"] as Dictionary<string, System.Object>;
            if(sourceList == null || sourceList.ContainsKey("count") == false || sourceList.ContainsKey("list") == false)
            {
                Logger.LogError("文件列表配置错误，sourcelist字段中count或list不存在");
                hasError = true;
                return result;
            }

            int fileCount = Convert.ToInt32(sourceList["count"]);
            List<System.Object> fileMetaList = sourceList["list"] as List<System.Object>;
            if(fileCount != fileMetaList.Count)
            {
                Logger.LogError("文件列表配置错误，sourcelist字段中count值或list内容长度值不一致");
                hasError = true;
                return result;
            }
            for (int i = 0; i < fileMetaList.Count; i++)
            {
                System.Object obj = fileMetaList[i];
                Dictionary<string, System.Object> meta = obj as Dictionary<string, System.Object>;
                if (meta.ContainsKey("url") == false || meta.ContainsKey("luacmd5") == false || meta.ContainsKey("size") == false)
                {
                    string error = "文件列表配置错误，sourcelist.list某一个文件的url，luacmd5，size字段不存在";
                    Logger.LogError(error);
                    Pandora.Instance.ReportError(error);
                    hasError = true;
                    return result;
                }

                AssetInfo fileInfo = new AssetInfo();
                fileInfo.url = meta["url"] as string;
                fileInfo.size = (int)(long)meta["size"];
                fileInfo.md5 = meta["luacmd5"] as string;
                fileInfo.name = Path.GetFileName(fileInfo.url);
                if (string.IsNullOrEmpty(fileInfo.name) || string.IsNullOrEmpty(fileInfo.md5) || fileInfo.size <= 0 || string.IsNullOrEmpty(fileInfo.url))
                {
                    string error = "文件列表配置错误，sourcelist.list某一个文件的url，luacmd5，size字段内容不正确";
                    Logger.LogError(error);
                    Pandora.Instance.ReportError(error);
                    hasError = true;
                    return result;
                }

                if (result.ContainsKey(fileInfo.name) == true)
                {
                    Logger.LogError("文件列表配置错误，sourcelist.list存在同名的资源配置信息");
                    hasError = true;
                    return result;
                }

                result.Add(fileInfo.name, fileInfo);
            }
            return result;
        }

        private Dictionary<string, List<AssetInfo>> ParseAssetInfoListDict(Dictionary<string, System.Object> content, Dictionary<string, AssetInfo> fileInfoDict, out bool hasError)
        {
            hasError = false;
            Dictionary<string, List<AssetInfo>> result = new Dictionary<string, List<AssetInfo>>();
            string dependency = ParseStringField(content, "dependency");
            if(string.IsNullOrEmpty(dependency))
            {
                string error = "依赖文件列表字段dependecy内容不存在或内容为空";
                Logger.LogError(error);
                Pandora.Instance.ReportError(error);
                hasError = true;
                return result;
            }
            string[] groups = dependency.Split('|');
            for(int i = 0; i < groups.Length; i++)
            {
                string group = groups[i];
                string[] segments = group.Split(':');
                if (segments.Length != 2)
                {
                    string error = "依赖文件列表字段dependecy内容中Group的内容格式配置错误";
                    Logger.LogError(error);
                    Pandora.Instance.ReportError(error);
                    hasError = true;
                    return result;
                }
                string groupName = segments[0];
                string groupValue = segments[1];
                string[] fileNames = groupValue.Split(',');
                List<AssetInfo> fileInfoList = new List<AssetInfo>();
                for(int j = 0; j < fileNames.Length; j++)
                {
                    string name = fileNames[j];
                    if(name.StartsWith("@")) //依赖其他group
                    {
                        string otherGroupName = name.Substring(1);
                        if(result.ContainsKey(otherGroupName) == false)
                        {
                            string error = "依赖文件列表字段dependecy内容中关于其他Group的依赖配置错误，被依赖的Group需要配置在右边";
                            Logger.LogError(error);
                            Pandora.Instance.ReportError(error);
                            hasError = true;
                            return result;
                        }
                        fileInfoList.AddRange(result[otherGroupName]);
                    }
                    else
                    {
                        if (fileInfoDict.ContainsKey(name) == false)
                        {
                            string error = "依赖文件列表字段dependecy内容中的文件名在sourcelist中不存在";
                            Logger.LogError(error);
                            Pandora.Instance.ReportError(error);
                            hasError = true;
                            return result;
                        }
                        fileInfoList.Add(fileInfoDict[name]);
                    }
                }
                if(result.ContainsKey(groupName) == true)
                {
                    string error = "依赖文件列表字段dependecy内容中存在重复的GroupName";
                    Logger.LogError(error);
                    Pandora.Instance.ReportError(error);
                    hasError = true;
                    return result;
                }
                result.Add(groupName, fileInfoList);
            }
            return result;
        }

        /// <summary>
        /// 文件配置信息
        /// </summary>
        public class AssetInfo
        {
            public string name;
            public string url;
            public string md5;
            public int size;

            public override string ToString()
            {
                return name + " " + url;
            }
        }
    }
}



/*Config 示例：
  {
   "accesstoken" : "",
   "acctype" : "qq",
   "act_switch" : "0",
   "appid" : "1105647895",
   "areaid" : "2",
   "buyType" : "0",
   "cap_ip1" : "182.254.42.116",
   "cap_ip2" : "182.254.88.158",
   "dependency" : "core:android_frame_lua.assetbundle,android_pop_lua.assetbundle,android_pop.assetbundle|notice:android_frame_lua.assetbundle,android_notice_lua.assetbundle,android_notice.assetbundle",
   "errmsg" : "totalSwitch Control",
   "fakeLoginInfo" : "",
   "flag" : "0",
   "function_switch" : "patface:1,notice:1",
   "gameappversion" : "1.3.0.0",
   "id" : "759",
   "ip" : "she.broker.tplay.qq.com",
   "isDebug" : "0",
   "isNetLog" : "0",
   "log_level" : "3",
   "lua_cdnurl" : "",
   "lua_newversion" : "",
   "luac_s_md5" : "",
   "luaversion" : "",
   "lucky_switch" : "0",
   "md5" : "",
   "mds_op" : "0",
   "openid" : "ACB1E569C331456C1E93D061CF3383C2",
   "partitionid" : "1",
   "platid" : "1",
   "port" : "15692",
   "private_whitelist" : "",
   "punchface_switch" : "0",
   "rate" : "0",
   "remark" : "",
   "representmap" : "",
   "ret" : "0",
   "sdkversion" : "Snake-Android-V1",
   "serverip" : "10.193.9.117",
   "sort" : "B",
   "sourcelist" : {
      "count" : 5,
      "list" : [
         {
            "extend" : "",
            "luacmd5" : "2CCB1A07D874A04743EB2EE7445681B7",
            "sid" : 1300,
            "size" : 21841,
            "url" : "http://down.qq.com/yxgw/she/20161112185503/android_frame_lua.assetbundle",
            "zipmd5" : ""
         },
         {
            "extend" : "",
            "luacmd5" : "82350EFBEF41B363CADAE7EEFED0DAB3",
            "sid" : 1301,
            "size" : 64172,
            "url" : "http://down.qq.com/yxgw/she/20161115233437/android_notice.assetbundle",
            "zipmd5" : ""
         },
         {
            "extend" : "",
            "luacmd5" : "553653164D64E7AF7D7F9260D2ACF942",
            "sid" : 1302,
            "size" : 8796,
            "url" : "http://down.qq.com/yxgw/she/20161115233437/android_notice_lua.assetbundle",
            "zipmd5" : ""
         },
         {
            "extend" : "",
            "luacmd5" : "F081B5B7D4326CC5CD3871FDDBB821E6",
            "sid" : 1303,
            "size" : 10502,
            "url" : "http://down.qq.com/yxgw/she/20161115233437/android_pop.assetbundle",
            "zipmd5" : ""
         },
         {
            "extend" : "",
            "luacmd5" : "9CA68B1AF591634E309DA69D7DA82D3B",
            "sid" : 1304,
            "size" : 6523,
            "url" : "http://down.qq.com/yxgw/she/20161115233438/android_pop_lua.assetbundle",
            "zipmd5" : ""
         }
      ]
   },
   "totalSwitch" : "1",
   "zip_md5" : ""
}
 * */
