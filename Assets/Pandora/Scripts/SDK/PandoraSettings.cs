using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

namespace com.tencent.pandora
{
    internal class PandoraSettings
    {
#if UNITY_EDITOR
        public static int DEFAULT_LOG_LEVEL = Logger.INFO; //给游戏输出SDK包时替换为Logger.INFO，减少Log对项目组的干扰
#else
        public static int DEFAULT_LOG_LEVEL = Logger.DEBUG;
#endif

        //可以在测试环境手动指定平台
        public static RuntimePlatform Platform
        {
            get
            {
#if UNITY_IOS || UNITY_IPHONE
                return RuntimePlatform.IPhonePlayer;
#elif UNITY_ANDROID
                return RuntimePlatform.Android;
#else
                return RuntimePlatform.WindowsPlayer;
#endif
            }
        }

        public static bool IsProductEnvironment
        {
            get;
            set;
        }

        /// <summary>
        /// 可以在Cache/Settings文件夹下读取settings.txt文件获取连接正式环境或测试环境
        /// </summary>
        public static void ReadEnvironmentSetting()
        {
            //从本地settings中读取log设置
            try
            {
                string filePath = LocalDirectoryHelper.GetSettingsFolderPath() + "/settings.txt";
                if (File.Exists(filePath) == true)
                {
                    string content = File.ReadAllText(filePath);
                    Dictionary<string, System.Object> dict = MiniJSON.Json.Deserialize(content) as Dictionary<string, System.Object>;
                    if(dict.ContainsKey("isProductEnvironment") == true)
                    {
                        IsProductEnvironment = (dict["isProductEnvironment"] as string) == "1";
                    }
                }
            }
            catch
            {
                //left blank
            }
        }

        /// <summary>
        /// 获取SDK Version
        /// </summary>
        /// <returns></returns>
        public static string GetSdkVersion()
        {
            if(Platform == RuntimePlatform.IPhonePlayer || Platform == RuntimePlatform.OSXEditor || Platform == RuntimePlatform.OSXPlayer)
            {
                return "DN-IOS-V2";
            }
            return "DN-Android-V2";
        }

        /// <summary>
        /// 获取CGI正式或测试环境地址
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static string GetCgiUrl(string appId)
        {
            string token = "http://";
            if(Pandora.Instance.UseHttps == true)
            {
                token = "https://";
            }
            if(IsProductEnvironment == true)
            {
                return token + "pandora.game.qq.com/cgi-bin/api/tplay/" + appId + "_cloud.cgi";
            }
            else
            {
                return token + "pandora.game.qq.com/cgi-bin/api/tplay/cloudtest_v3.cgi";
            }
        }

        /// <summary>
        /// 获取临时文件目录
        /// </summary>
        /// <returns></returns>
        public static string GetTemporaryCachePath()
        {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR
            return Application.dataPath + "/../CACHE";
#else
    #if UNITY_IOS
                return Application.temporaryCachePath;
    #else
                return Application.persistentDataPath;
    #endif
#endif
        }

        public static string GetAtmHost()
        {
            if(IsProductEnvironment == true)
            {
                return "jsonatm.broker.tplay.qq.com";
            }
            else
            {
                return "test.broker.tplay.qq.com";
            }
        }

        public static int GetAtmPort()
        {
            if(IsProductEnvironment == true)
            {
                return 5692;
            }
            else
            {
                return 4567;
            }
        }

        /// <summary>
        /// 各个平台上file协议开头不一样
        /// </summary>
        /// <returns></returns>
        public static string GetFileProtocolToken()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_STANDALONE_OSX //添加后，BuildSetting为Android时，也可加载assetbundle
            return "file:///";
#elif UNITY_IOS || UNITY_EDITOR_OSX || UNITY_ANDROID
            return "file://";
#endif
        }

        /// <summary>
        /// 是否从本地StreamingAssets目录下加载资源
        /// </summary>
        public static bool UseStreamingAssets
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }

        public static bool IsAndroidPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.Android;
            }
        }

        public static bool IsIOSPlatform
        {
            get
            {
//#if UNITY_5
//                return Application.platform == RuntimePlatform.IOS;
//#elif UNITY_4_7
                return Application.platform == RuntimePlatform.IPhonePlayer;
//#endif
            }
        }

        public static bool IsWindowsPlatform
        {
            get
            {
                return Application.platform == RuntimePlatform.WindowsPlayer;
            }
        }

        public static string GetPlatformDescription()
        {
            if (PandoraSettings.Platform == RuntimePlatform.IPhonePlayer)
            {
                return "ios";
            }
            return "android";
        }

        /// <summary>
        /// 是否使用Post的方式访问云端CGI
        /// </summary>
        public static bool RequestCgiByPost
        {
            get
            {
                return false;
            }
        }

    }
}
