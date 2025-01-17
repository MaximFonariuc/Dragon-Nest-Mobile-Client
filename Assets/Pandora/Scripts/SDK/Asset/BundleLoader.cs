using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_5
namespace com.tencent.pandora
{
    /// <summary>
    /// BundleLoader 使用Assetbunle相关接口将本地的Assetbundle资源加载到内存
    /// </summary>
    public class BundleLoader : AbstractLoader
    {
        private Dictionary<string, AssetConfig> _assetConfigDict = new Dictionary<string, AssetConfig>();

        public void LoadLuaAssetList(List<string> urlList, Action<List<string>> callback)
        {
            for (int i = 0; i < urlList.Count; i++)
            {
                //Logger.Log("<color=#0000ff>将资源从本地加载到内存</color>，url: " + urlList[i]);
                AssetConfig config = new AssetConfig(AssetType.LuaPrefab, true);
                _assetConfigDict[urlList[i]] = config;
            }
            base.LoadAssetList(urlList, callback, 0);
        }

        public void LoadAsset(string url, AssetConfig config, Action<string> callback)
        {
            //Logger.Log("<color=#0000ff>将资源从本地加载到内存</color>, url: " + url);
            _assetConfigDict[url] = config;
            base.LoadAsset(url, callback, 0);
        }

        protected override IEnumerator DaemonLoad()
        {
            while(true)
            {
                if(_toLoadQueue.Count > 0 && PauseDownloading == false)
                {
                    string url = _toLoadQueue.Dequeue();
                    string path = ConvertUrlToPath(url);
                    AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
                    _loadingSet.Add(url);
                    yield return request;
                    _loadingSet.Remove(url);
                    if(request.assetBundle == null)
                    {
                        _errorQueue.Enqueue(url);
                    }
                    else
                    {
                        if(HandleLoadedContent(url, request.assetBundle) == true)
                        {
                            _errorMaxRetryCountDict.Remove(url);
                            _errorRetryCountDict.Remove(url);
                            _loadedSet.Add(url);
                        }
                        else
                        {
                            _errorQueue.Enqueue(url);
                        }
                    }
                }
                yield return null;
            }
        }

        private string ConvertUrlToPath(string url)
        {
            return url.Replace(PandoraSettings.GetFileProtocolToken(), "");
        }

        protected bool HandleLoadedContent(string url, AssetBundle bundle)
        {
            bool result = false;
            if (Pandora.Instance.IsDebug == true)
            {
                Logger.Log("<color=#008888>资源从本地加载到内存成功</color>, url: " + url);
            }
            result = AssetPool.ParseAsset(url, bundle, _assetConfigDict[url]);
            return result;
        }

        protected override void HandleError(string url)
        {
            Logger.LogError("将资源加载到内存失败，且超过最大重试次数, url: " + url);
        }

    }
}
#endif