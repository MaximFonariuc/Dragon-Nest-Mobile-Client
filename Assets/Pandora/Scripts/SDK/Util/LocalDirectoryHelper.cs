using System;
using System.IO;
using UnityEngine;

namespace com.tencent.pandora
{
    internal class LocalDirectoryHelper
    {
        //过期时间
        private const int EXPIRE_DAYS = 7;
        private static string _root;

        public static void Initialize()
        {
            CreateIfNotExists();
            DeleteExpiredAsset();
            DeleteExpiredCookie();
            DeleteExpiredLog();
        }

        public static void CreateIfNotExists()
        {
            try
            {
                Directory.CreateDirectory(GetProgramAssetFolderPath());
                Directory.CreateDirectory(GetAssetFolderPath());
                Directory.CreateDirectory(GetCookieFolderPath());
                Directory.CreateDirectory(GetLogFolderPath());
                Directory.CreateDirectory(GetSettingsFolderPath());
            }
            catch(Exception e)
            {
                string error = "创建文件夹失败:" + e.Message;
                Logger.LogError(error);
                Pandora.Instance.ReportError(error);
            }
        }

        private static string GetPandoraRoot()
        {
            if(string.IsNullOrEmpty(_root) == true)
            {
                _root = Path.Combine(PandoraSettings.GetTemporaryCachePath(), "Pandora");
            }
            return _root;
        }

        public static string GetLogFolderPath()
        {
            return Path.Combine(GetPandoraRoot(), "logs");
        }

        public static string GetSettingsFolderPath()
        {
            return Path.Combine(GetPandoraRoot(), "settings");
        }

        /// <summary>
        /// 该目录下保存程序运行需要的相关资源
        /// </summary>
        /// <returns></returns>
        public static string GetProgramAssetFolderPath()
        {
            return Path.Combine(GetPandoraRoot(), "program");
        }

        public static string GetCookieFolderPath()
        {
            return Path.Combine(GetPandoraRoot(), "cookies");
        }

        public static string GetProgramAssetMetaPath()
        {
            return Path.Combine(LocalDirectoryHelper.GetProgramAssetFolderPath(), "meta.txt");
        }

        /// <summary>
        /// 该目录下保存程序运行中动态加载的资源
        /// </summary>
        /// <returns></returns>
        public static string GetAssetFolderPath()
        {
            return Path.Combine(GetPandoraRoot(), "assets");
        }

        public static string GetStreamingAssetsUrl()
        {
            return PandoraSettings.GetFileProtocolToken() + Application.streamingAssetsPath + "/Pandora";
        }

        public static bool IsStreamingAssetsExists(string name)
        {
            return File.Exists(Application.streamingAssetsPath + "/Pandora/" + name);
        }

        public static void DeleteExpiredCookie()
        {
            DeleteExpiredAsset(GetCookieFolderPath());
        }

        public static void DeleteExpiredAsset()
        {
            DeleteExpiredAsset(GetAssetFolderPath());
        }

        public static void DeleteExpiredLog()
        {
            DeleteExpiredAsset(GetLogFolderPath());
        }

        private static void DeleteExpiredAsset(string directory)
        {
            try
            {
                string[] paths = Directory.GetFiles(directory);
                for (int i = 0; i < paths.Length; i++)
                {
                    string path = paths[i];
                    DateTime lastWrite = File.GetLastWriteTimeUtc(path);
                    double daySpan = DateTime.UtcNow.Subtract(lastWrite).TotalDays;
                    if(daySpan >= EXPIRE_DAYS)
                    {
                        File.Delete(path);
                        Logger.Log("删除过期文件： " + path);
                    }
                }
            }
            catch(Exception e)
            {
                string error = "删除文件发生异常，directory: " + directory + "\n" + e.Message;
                Logger.LogError(error);
                Pandora.Instance.ReportError(error);
            }
        }

        public static void Clean()
        {
            DeleteDirectoryAssets(GetCookieFolderPath());
            DeleteDirectoryAssets(GetLogFolderPath());
        }

        public static void DeleteCookies()
        {
            DeleteDirectoryAssets(GetCookieFolderPath());
        }

        private static void DeleteDirectoryAssets(string directory)
        {
            try
            {
                string[] paths = Directory.GetFiles(directory);
                for (int i = 0; i < paths.Length; i++)
                {
                    File.Delete(paths[i]);
                }
            }
            catch(Exception e)
            {
                string error = "删除文件发生异常，directory: " + directory + "\n" + e.Message;
                Logger.LogError(error);
                Pandora.Instance.ReportError(error);
            }
        }

        public static void DeleteAssetByUrl(string url)
        {
            try
            {
                string path = url.Replace(PandoraSettings.GetFileProtocolToken(), "");
                if (File.Exists(path) == true)
                {
                    File.Delete(path);
                }
            }
            catch(Exception e)
            {
                string error = "删除文件发生异常，Url: " + url + "\n" + e.Message;
                Logger.LogError(error);
                Pandora.Instance.ReportError(error);
            }
        }

    }
}
