using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.tencent.pandora
{
    public class CacheManager
    {
        private static Dictionary<string, System.Object> _cachedProgramAssetInfoDict = new Dictionary<string, System.Object>();
        private static string _programAssetMetaPath;
        private static CacheLoader _cacheLoader;
        private static bool _isInitialized = false;
        private static bool _isMetaDirty = false;

        public static void Initialize()
        {
            if(_isInitialized == false)
            {
                _isInitialized = true;
                LoadLocalMetaFile();
                GameObject pandoraGo = Pandora.Instance.GetGameObject();
                _cacheLoader = pandoraGo.AddComponent<CacheLoader>();
                _cacheLoader.Initialize();
            }
        }

        public static void SetProgramAssetProgressCallback(Action<Dictionary<string, string>> callback)
        {
            _cacheLoader.programAssetProgressCallback = callback;
        }
        /// <summary>
        /// 检查本地文件方案2：
        /// 1.读取本地Cache目录下的meta文件，对比其中的文件Md5
        /// 2.检查本地Cache目录下是否存在目标文件
        /// 
        /// 在本地测试，设置从StreamingAssets目录下读取资源时不会读取meta文件内容
        /// </summary>
        private static void LoadLocalMetaFile()
        {
            try
            {
                _programAssetMetaPath = LocalDirectoryHelper.GetProgramAssetMetaPath();
                if (File.Exists(_programAssetMetaPath) == true)
                {
                    string meta = File.ReadAllText(_programAssetMetaPath);
                    if (string.IsNullOrEmpty(meta) == false)
                    {
                        _cachedProgramAssetInfoDict = MiniJSON.Json.Deserialize(meta) as Dictionary<string, System.Object>;
                    }
                    if (_cachedProgramAssetInfoDict == null)
                    {
                        _cachedProgramAssetInfoDict = new Dictionary<string, System.Object>();
                    }
                }
            }
            catch(Exception e)
            {
                string error = "CachedFile Meta文件读取失败 " + e.Message;
                Logger.LogError(error);
                Pandora.Instance.ReportError(error, ErrorCodeConfig.META_READ_FAILED);
            }
        }

        public static void LoadProgramAssetList(string group, List<RemoteConfig.AssetInfo> assetInfoList, Action<string, List<RemoteConfig.AssetInfo>> callback)
        {
            List<RemoteConfig.AssetInfo> downloadAssetInfoList = new List<RemoteConfig.AssetInfo>();
            for (int i = 0; i < assetInfoList.Count; i++)
            {
                RemoteConfig.AssetInfo assetInfo = assetInfoList[i];
                if(PandoraSettings.UseStreamingAssets == true || IsProgramAssetExists(assetInfo.name, assetInfo.md5) == false)
                {
                    CacheManager.DeleteUnmatchCacheFile(assetInfo.name);
                    assetInfo.url = GetAssetRealUrl(assetInfo.url);
                    downloadAssetInfoList.Add(assetInfo);
                }
            }
            if (downloadAssetInfoList.Count == 0)
            {
                callback(group, assetInfoList);
            }
            else
            {
                _cacheLoader.LoadProgramAssetList(group, downloadAssetInfoList, delegate (List<string> pAssetList) { OnLoadProgramAssetList(assetInfoList); callback(group, assetInfoList); });
            }
        }

        /// <summary>
        /// 为了避免每次文件修改都提交cdn，可以设置测试阶段从本地StreamingAssets目录下下载资源
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetAssetRealUrl(string url)
        {
            if (PandoraSettings.UseStreamingAssets == false)
            {
                return url;
            }
            string name = Path.GetFileName(url);
            if(LocalDirectoryHelper.IsStreamingAssetsExists(name) == false)
            {
                return url;
            }
            return LocalDirectoryHelper.GetStreamingAssetsUrl() + "/" + name;
        }

        private static void OnLoadProgramAssetList(List<RemoteConfig.AssetInfo> assetInfoList)
        {
            for (int i = 0; i < assetInfoList.Count; i++)
            {
                RemoteConfig.AssetInfo assetInfo = assetInfoList[i];
                RefreshProgramAssetMeta(assetInfo.name, assetInfo.size, assetInfo.md5);
            }
        }

        /// <summary>
        /// 1.检查程序缓存文件是否存在
        /// 2.检查记录中缓存文件的Md5值和RemoteConfig中相应值是否相同
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="assetMd5"></param>
        /// <returns></returns>
        public static bool IsProgramAssetExists(string assetName, string assetMd5)
        {
            string assetPath = GetCachedProgramAssetPath(assetName);
            if (File.Exists(assetPath) == false)
            {
                return false;
            }

            if (_cachedProgramAssetInfoDict.ContainsKey(assetName) == false)
            {
                return false;
            }

            Dictionary<string, System.Object> cachedAssetInfo = _cachedProgramAssetInfoDict[assetName] as Dictionary<string, System.Object>;
            if((cachedAssetInfo["md5"] as string) == assetMd5)
            {
                return true;
            }
            return false;
        }

        public static void DeleteUnmatchCacheFile(string assetName)
        {
            string assetPath = GetCachedProgramAssetPath(assetName);
            if (File.Exists(assetPath) == false)
            {
                return;
            }

            try
            {
                File.Delete(assetPath);
            }
            catch
            {
                Logger.LogError("删除Meta文件中Md5不匹配的文件： " + assetName);
            }
        }

        public static bool IsProgramAssetExists(List<RemoteConfig.AssetInfo> assetInfoList)
        {
            for(int i = 0; i < assetInfoList.Count; i++)
            {
                RemoteConfig.AssetInfo assetInfo = assetInfoList[i];
                if(IsProgramAssetExists(assetInfo.name, assetInfo.md5) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static string GetCachedProgramAssetPath(string url)
        {
            string name = Path.GetFileName(url);
            return Path.Combine(LocalDirectoryHelper.GetProgramAssetFolderPath(), name);
        }

        /// <summary>
        /// 返回file://协议的url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetCachedProgramAssetUrl(string url)
        {
            return PandoraSettings.GetFileProtocolToken() + GetCachedProgramAssetPath(url);
        }

        public static void RefreshProgramAssetMeta(string name, int size, string md5)
        {
            Dictionary<string, System.Object> assetInfo = new Dictionary<string, System.Object>();
            assetInfo["name"] = name;
            assetInfo["size"] = size;
            assetInfo["md5"] = md5;
            if (_cachedProgramAssetInfoDict.ContainsKey(name) == true)
            {
                Dictionary<string, System.Object> existAssetInfo = _cachedProgramAssetInfoDict[name] as Dictionary<string, System.Object>;
                if(existAssetInfo["md5"].ToString() != md5 || existAssetInfo["size"].ToString() != size.ToString())
                {
                    _isMetaDirty = true;
                    _cachedProgramAssetInfoDict[name] = assetInfo;
                }
            }
            else
            {
                _isMetaDirty = true;
                _cachedProgramAssetInfoDict.Add(name, assetInfo);
            }
        }

        public static void WriteProgramAssetMeta()
        {
            int maxRetryCount = 3;
            for(int i = 0; i < maxRetryCount; i++)
            {
                try
                {
                    if (_isMetaDirty == true)
                    {
                        _isMetaDirty = false;
                        string meta = MiniJSON.Json.Serialize(_cachedProgramAssetInfoDict);
                        File.WriteAllText(_programAssetMetaPath, meta);
                    }
                    break;
                }
                catch (Exception e)
                {
                    if(i == (maxRetryCount - 1))
                    {
                        string error = "CachedFile Meta文件更新失败 " + e.Message;
                        Logger.LogError(error);
                        Pandora.Instance.ReportError(error, ErrorCodeConfig.META_WRITE_FAILED);
                    }
                }
            }
        }

        public static void LoadAsset(string url, Action<string> callback)
        {
            if(IsAssetExists(url) == true)
            {
                if(callback != null)
                {
                    callback(url);
                }
            }
            else
            {
                _cacheLoader.LoadAsset(url, callback);
            }
        }

        /// <summary>
        /// 检查缓存资源是否存在
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsAssetExists(string url)
        {
            string assetPath = GetCachedAssetPath(url);
            return File.Exists(assetPath);
        }

        public static void DeleteAsset(string url)
        {
            string assetPath = GetCachedAssetPath(url);
            if(File.Exists(assetPath) == true)
            {
                try
                {
                    File.Delete(assetPath);
                }
                catch
                {
                    Logger.LogError("删除文件出错： " + assetPath);
                }
            }
        }

        public static string GetCachedAssetPath(string url)
        {
            string name = Path.GetFileName(url);
            return Path.Combine(LocalDirectoryHelper.GetAssetFolderPath(), url.GetHashCode().ToString() + "-" + name);
        }

        public static string GetCachedAssetUrl(string url)
        {
            return PandoraSettings.GetFileProtocolToken() + GetCachedAssetPath(url);
        }

        public static bool PauseDownloading
        {
            get
            {
                return _cacheLoader.PauseDownloading;
            }
            set
            {
                _cacheLoader.PauseDownloading = value;
            }
        }

        public static void Clear()
        {
            _cacheLoader.Clear();
        }

    }
}
