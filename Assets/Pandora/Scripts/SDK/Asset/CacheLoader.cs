using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.tencent.pandora
{
    /// <summary>
    /// CacheLoader负责将资源从远端服务器加载到本地Pandora/program目录下
    /// 只有从远端下载到本地阶段才需要重连机制
    /// </summary>
    public class CacheLoader : AbstractLoader
    {
        /// <summary>
        /// 资源出错重试加载次数
        /// </summary>
        public const int PROGRAME_ASSET_RETRY_COUNT = 3;
        public const int ASSET_RETRY_COUNT = 1;

        private HashSet<string> _programAssetUrlSet = new HashSet<string>();
        //Key为AssetInfo.url, Value为AssetInfo
        private Dictionary<string, RemoteConfig.AssetInfo> _programAssetInfoDict = new Dictionary<string, RemoteConfig.AssetInfo>();
        //Key为组（Group）名，Value为分组AssetInfoList
        private Dictionary<string, List<RemoteConfig.AssetInfo>> _programAssetInfoListDict = new Dictionary<string, List<RemoteConfig.AssetInfo>>();

        private Coroutine _progressCoroutine;
        private Queue<string> _failedGroupQueue = new Queue<string>();
        private Queue<string> _succeededGroupQueue = new Queue<string>();
        private Dictionary<string, string> _progressMessage = new Dictionary<string, string>();
        public Action<Dictionary<string, string>> programAssetProgressCallback;

        public override void Initialize()
        {
            //CacheLoader从远端加载资源，为避免挤占游戏资源，故设置并发数为1
            _concurrentCount = 1;
            base.Initialize();
        }

        public void LoadProgramAssetList(string group, List<RemoteConfig.AssetInfo> assetInfoList, Action<List<string>> callback)
        {
            if(_programAssetInfoListDict.ContainsKey(group) == true)
            {
                string error = "有尚未处理完的加载";
                Logger.LogError(error);
                return;
            }
            _programAssetInfoListDict.Add(group, assetInfoList);
            List<string> urlList = new List<string>();
            for (int i = 0; i < assetInfoList.Count; i++)
            {
                string url = assetInfoList[i].url;
                _programAssetUrlSet.Add(url);
                if(_programAssetInfoDict.ContainsKey(url) == true)
                {
                    _programAssetInfoDict.Remove(url);
                }
                _programAssetInfoDict.Add(url, assetInfoList[i]);
                urlList.Add(url);
            }
            base.LoadAssetList(urlList, callback, PROGRAME_ASSET_RETRY_COUNT);
            InvokeLoadStart(group);
            StartInspectLoading();
        }

        private void StartInspectLoading()
        {
            if(_progressCoroutine == null)
            {
                _progressCoroutine = StartCoroutine(InspectProgramAssetProgress());
            }
        }
        
        private IEnumerator InspectProgramAssetProgress()
        {
            while(true)
            {
                var iter = _programAssetInfoListDict.GetEnumerator();
                while (iter.MoveNext())
                {
                    string group = iter.Current.Key;
                    List<RemoteConfig.AssetInfo> assetInfoList = iter.Current.Value;
                    int groupTotalBytes = 0;
                    int groupLoadedBytes = 0;
                    for(int i = 0; i < assetInfoList.Count; i++)
                    {
                        RemoteConfig.AssetInfo assetInfo = assetInfoList[i];
                        groupTotalBytes += assetInfo.size;
                        if(_failedSet.Contains(assetInfo.url) == true)
                        {
                            _failedGroupQueue.Enqueue(group);
                            break;
                        }
                        if(_loadedSet.Contains(assetInfo.url) == true)
                        {
                            groupLoadedBytes += assetInfo.size;
                            continue;
                        }
                        WWW www = null;
                        _loadingDict.TryGetValue(assetInfo.url, out www);
                        if(www != null)
                        {
                            groupLoadedBytes += (int)(www.progress * assetInfo.size);
                        }
                    }
                    if(groupLoadedBytes >= groupTotalBytes)
                    {
                        _succeededGroupQueue.Enqueue(group);
                    }
                    else
                    {
                        float progress = (float)groupLoadedBytes / (float)groupTotalBytes;
                        InvokeLoadProgress(group, (int)(progress * 100));
                    }
                }
                while(_failedGroupQueue.Count > 0)
                {
                    string failedGroup = _failedGroupQueue.Dequeue();
                    _programAssetInfoListDict.Remove(failedGroup);
                    InvokeLoadError(failedGroup);
                }
                while(_succeededGroupQueue.Count > 0)
                {
                    string succeededGroup = _succeededGroupQueue.Dequeue();
                    _programAssetInfoListDict.Remove(succeededGroup);
                    InvokeLoadComplete(succeededGroup);
                }
                yield return null;
            }
        }

        private void InvokeLoadStart(string group)
        {
            if(programAssetProgressCallback != null)
            {
                Dictionary<string, string> msg = new Dictionary<string, string>();
                msg.Add("type", "assetLoadStart");
                msg.Add("name", group);
                programAssetProgressCallback(msg);
            }
        }

        private void InvokeLoadError(string group)
        {
            if(programAssetProgressCallback != null)
            {
                Dictionary<string, string> msg = new Dictionary<string, string>();
                msg.Add("type", "assetLoadError");
                msg.Add("name", group);
                programAssetProgressCallback(msg);
            }
        }

        private void InvokeLoadComplete(string group)
        {
            if (programAssetProgressCallback != null)
            {
                Dictionary<string, string> msg = new Dictionary<string, string>();
                msg.Add("type", "assetLoadComplete");
                msg.Add("name", group);
                programAssetProgressCallback(msg);
            }
        }

        private void InvokeLoadProgress(string group, int progress)
        {
            if (programAssetProgressCallback != null)
            {
                if(_progressMessage.ContainsKey("type") == false)
                {
                    _progressMessage.Add("type", "assetLoadProgress");
                }
                if (_progressMessage.ContainsKey("name") == false)
                {
                    _progressMessage.Add("name", group);
                }
                if (_progressMessage.ContainsKey("progress") == false)
                {
                    _progressMessage.Add("progress", "0");
                }
                _progressMessage["name"] = group;
                _progressMessage["progress"] = progress.ToString();
                programAssetProgressCallback(_progressMessage);
            }
        }

        public void LoadAsset(string url, Action<string> callback)
        {
            //Logger.Log("<color=#0000ff>将资源从远端下载到本地</color>，url: " + url);
            base.LoadAsset(url, callback, ASSET_RETRY_COUNT);
        }

        protected override bool HandleLoadedContent(string url, WWW www)
        {
            if(Pandora.Instance.IsDebug == true)
            {
                Logger.Log("<color=#0000ff>资源从远端下载到本地成功</color>，url: " + url);
            }
            string path = string.Empty;
            if (_programAssetUrlSet.Contains(url))
            {
                path = CacheManager.GetCachedProgramAssetPath(url);
                string md5 = CachedFileMd5Helper.GetFileMd5(www.bytes);
                RemoteConfig.AssetInfo assetInfo = _programAssetInfoDict[url];
                //当cdn出错或运营商劫持时等情况下会出现下载的资源md5码和配置中的值不一致的情况
                if (PandoraSettings.UseStreamingAssets == false && assetInfo != null)
                {
                    if(assetInfo.md5 != md5)
                    {
                        www.Dispose();
                        www = null;
                        string error = "下载到资源计算的Md5值和配置中的Md5值不相等, url: " + url;
                        Pandora.Instance.ReportError(error, ErrorCodeConfig.MD5_VALIDATE_FAILED);
                        Logger.LogError(error);
                        return false;
                    }
                }
                WriteAllBytes(path, www.bytes, ErrorCodeConfig.FILE_WRITE_FAILED);
                www.Dispose();
                www = null;
                return true;
            }

            path = CacheManager.GetCachedAssetPath(url);
            WriteAllBytes(path, www.bytes, ErrorCodeConfig.FILE_WRITE_FAILED);
            www.Dispose();
            www = null;
            return true;
        }

        private void WriteAllBytes(string path, byte[] bytes, int errorCode)
        {
            int maxRetryCount = 3;
            for (int i = 0; i < maxRetryCount; i++)
            {
                try
                {
                    File.WriteAllBytes(path, bytes);
                    break;
                }
                catch (Exception e)
                {
                    if (i == (maxRetryCount - 1))
                    {
                        string error = "创建本地文件失败： " + path + " " + e.Message;
                        Pandora.Instance.ReportError(error, errorCode);
                        Logger.LogError(error);
                    }
                }
            }
        }

        protected override void HandleError(string url)
        {
            string error = "将资源加载到本地失败，且超过最大重试次数, url: " + url;
            Logger.LogError(error);
            Pandora.Instance.ReportError(error, ErrorCodeConfig.ASSET_LOAD_FAILED);
        }

        public override void Clear()
        {
            _programAssetUrlSet.Clear();
            _programAssetInfoDict.Clear();
            _programAssetInfoListDict.Clear();
            if(_progressCoroutine != null)
            {
                StopCoroutine(_progressCoroutine);
                _progressCoroutine = null;
            }
            base.Clear();
        }
    }

}
