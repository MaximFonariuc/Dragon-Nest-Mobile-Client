using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.tencent.pandora
{
    /// <summary>
    /// AssetLoader 将资源加载到内存
    /// </summary>
    public class AssetLoader : AbstractLoader
    {

        // 每60s清理一次0引用资源
        protected const int DELETE_ZERO_REFERENCE_INTERVAL = 60;
        protected float _lastDeleteTime;

        private Dictionary<string, AssetConfig> _assetConfigDict = new Dictionary<string, AssetConfig>();

        public override void Initialize()
        {
            //AssetLoader从本地加载到内存，为优化图片展示体验，增加下载并发数
            _concurrentCount = 8;
            base.Initialize();
            _lastDeleteTime = Time.realtimeSinceStartup;
            StartCoroutine(DaemonDeleteZeroReferenceAsset());
        }

        public void LoadLuaAssetList(List<string> urlList, Action<List<string>> callback)
        {
            for (int i = 0; i < urlList.Count; i++)
            {
                //Logger.Log("<color=#0000ff>将资源从本地加载到内存</color>，url: " + urlList[i]);
                AssetConfig config = new AssetConfig(AssetType.Lua, true);
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

        protected override bool HandleLoadedContent(string url, WWW www)
        {
            if(Pandora.Instance.IsDebug == true)
            {
                Logger.Log("<color=#00ff00>资源从本地加载到内存成功</color>, url: " + url);
            }
            bool result = false;
            result = AssetPool.ParseAsset(url, www, _assetConfigDict[url]);
            www.Dispose();
            return result;
        }

        protected override void HandleError(string url)
        {
            Logger.LogError("将资源加载到内存失败，且超过最大重试次数, url: " + url);
        }

        private IEnumerator DaemonDeleteZeroReferenceAsset()
        {
            while(true)
            {
                float delta = Time.realtimeSinceStartup - _lastDeleteTime;
                if(delta >= DELETE_ZERO_REFERENCE_INTERVAL)
                {
                    _lastDeleteTime = Time.realtimeSinceStartup;
                    AssetManager.DeleteZeroReferenceAsset();
                }
                yield return null;
            }
        }
    }
}
