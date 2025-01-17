using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.tencent.pandora
{
    /// <summary>
    /// 资源加载分两种情况：
    /// 1.将资源从远端下载到本地Pandora/Cache目录下
    /// 2.将资源从Pandora/Cache目录加载到内存
    /// </summary>
    public class AssetManager
    {
        private const string LUA_ASSET_TOKEN = "_lua";
        private static bool _isInitialized = false;

        private static WwwLoader _wwwLoader;
        private static AssetLoader _assetLoader;
#if UNITY_5
        private static BundleLoader _bundleLoader;
#endif

        public static void Initialize()
        {
            if(_isInitialized == false)
            {
                _isInitialized = true;
                GameObject pandoraGo = Pandora.Instance.GetGameObject();
                _wwwLoader = pandoraGo.AddComponent<WwwLoader>();
                _assetLoader = pandoraGo.AddComponent<AssetLoader>();
                _assetLoader.Initialize();
#if UNITY_5
                _bundleLoader = pandoraGo.AddComponent<BundleLoader>();
                _bundleLoader.Initialize();
#endif
            }
        }

        public static void LoadRemoteConfig(UserData userData, Action<RemoteConfig> callback)
        {
            _wwwLoader.LoadRemoteConfig(userData, callback);
        }

        public static void LoadWww(string url, string requestJson, bool isPost, Action<string> callback)
        {
            _wwwLoader.LoadWww(url, requestJson, isPost, callback);
        }

#region 初次加载Lua资源列表
        /// <summary>
        /// 加载资源分组，分两步
        /// 1.将分组文件全部加载到Cache目录下
        /// 2.将分组中Lua文件加载到内存
        /// </summary>
        /// <param name="assetInfoList"></param>
        /// <param name="callback"></param>
        public static void LoadProgramAssetList(string group, List<RemoteConfig.AssetInfo> assetInfoList, Action<string, List<RemoteConfig.AssetInfo>> callback)
        {
            CacheManager.LoadProgramAssetList(group, assetInfoList, delegate (string pGroup, List<RemoteConfig.AssetInfo> pAssetInfoList) { LoadLuaAssetList(pGroup, pAssetInfoList, callback); });
        }

        /// <summary>
        /// 第二步将Lua资源加载到内存
        /// </summary>
        /// <param name="assetInfoList"></param>
        /// <param name="callback"></param>
        private static void LoadLuaAssetList(string group, List<RemoteConfig.AssetInfo> assetInfoList, Action<string, List<RemoteConfig.AssetInfo>> callback)
        {
            List<string> luaAssetUrlList = new List<string>();
            //是否所有的Lua资源都在第一步中成功加载到了本地
            bool isAllLuaAssetInCache = true;
            for (int i = 0; i < assetInfoList.Count; i++)
            {
                RemoteConfig.AssetInfo assetInfo = assetInfoList[i];
                if(assetInfo.url.Contains(LUA_ASSET_TOKEN) == true)
                {
                    if(CacheManager.IsProgramAssetExists(assetInfo.name, assetInfo.md5) == true)
                    {
                        string luaCacheUrl = CacheManager.GetCachedProgramAssetUrl(assetInfo.url);
                        luaAssetUrlList.Add(luaCacheUrl);
                    }
                    else
                    {
                        isAllLuaAssetInCache = false;
                        CacheManager.DeleteUnmatchCacheFile(assetInfo.name);
                        //资源加载到本地后写入失败的情况
                        //此时AssetLoader直接从远端加载
                        luaAssetUrlList.Add(assetInfo.url);
                    }
                    
                }
            }
            if(isAllLuaAssetInCache == true)
            {
#if UNITY_5
                if(CanUseBundleLoader == true)
                {
                    _bundleLoader.LoadLuaAssetList(luaAssetUrlList, delegate (List<string> pLuaAssetUrlList) { callback(group, assetInfoList); });
                    return;
                }
#endif
            }

            _assetLoader.LoadLuaAssetList(luaAssetUrlList, delegate (List<string> pLuaAssetUrlList) { callback(group, assetInfoList); });
        }
#endregion

#region 加载和获取面板Prefab
        public static void GetPanelGameObject(RemoteConfig.AssetInfo assetInfo, Action<GameObject> callback)
        {
            string cacheUrl = CacheManager.GetCachedProgramAssetUrl(assetInfo.url);
            if(AssetPool.HasAsset(cacheUrl) == true)
            {
                OnLoadPrefab(cacheUrl, callback);
            }
            else
            {
                if(CacheManager.IsProgramAssetExists(assetInfo.name, assetInfo.md5) == true)
                {
                    LoadProgramPrefab(assetInfo, true, callback);
                }
                else
                {
                    CacheManager.DeleteUnmatchCacheFile(assetInfo.name);
                    //1.资源下载到本地写入文件失败
                    //2.资源尚未使用之前被删除
                    //这两个情况会导致资源在本地不存在，这个时候直接从远端再加载一次
                    CacheManager.LoadAsset(assetInfo.url, delegate (string pUrl) { LoadProgramPrefab(assetInfo, true, callback); });
                }
            }
        }

        private static void LoadProgramPrefab(RemoteConfig.AssetInfo assetInfo, bool isCacheInMemory, Action<GameObject> callback)
        {
            string url = assetInfo.url;
            AssetConfig config = new AssetConfig(AssetType.Prefab, isCacheInMemory);
            if (CacheManager.IsProgramAssetExists(assetInfo.name, assetInfo.md5) == true)
            {
                url = CacheManager.GetCachedProgramAssetUrl(assetInfo.url);
#if UNITY_5
                if(CanUseBundleLoader == true)
                {
                    _bundleLoader.LoadAsset(url, config, delegate (string pUrl) { OnLoadPrefab(pUrl, callback); });
                    return;
                }
#endif
            }
            _assetLoader.LoadAsset(url, config, delegate (string pUrl) { OnLoadPrefab(pUrl, callback); });
        }
#endregion


#region 加载和获取普通Prefab资源
        public static void GetGameObject(string url, Action<GameObject> callback)
        {
            GetGameObject(url, false, callback);
        }

        public static void GetGameObject(string url, bool isCacheInMemory, Action<GameObject> callback)
        {
            string cacheUrl = CacheManager.GetCachedAssetUrl(url);
            if (AssetPool.HasAsset(cacheUrl) == true)
            {
                OnLoadPrefab(cacheUrl, callback);
            }
            else
            {
                if(CacheManager.IsAssetExists(url) == true)
                {
                    LoadPrefab(url, isCacheInMemory, callback);
                }
                else
                {
                    CacheManager.LoadAsset(url, delegate (string pUrl) { LoadPrefab(pUrl, isCacheInMemory, callback); });
                }
            }
        }

        private static void LoadPrefab(string url, bool isCacheInMemory, Action<GameObject> callback)
        {
            AssetConfig config = new AssetConfig(AssetType.Prefab, isCacheInMemory);
            if (CacheManager.IsAssetExists(url) == true)
            {
                url = CacheManager.GetCachedAssetPath(url);
#if UNITY_5
                if(CanUseBundleLoader == true)
                {
                    _bundleLoader.LoadAsset(url, config, delegate (string pUrl) { OnLoadPrefab(pUrl, callback); });
                    return;
                }
#endif
            }
            _assetLoader.LoadAsset(url, config, delegate (string pUrl) { OnLoadPrefab(pUrl, callback); });
        }

        private static void OnLoadPrefab(string cacheUrl, Action<GameObject> callback)
        {
            GameObject prefab = AssetPool.GetAsset(cacheUrl) as GameObject;
            GameObject go = null;
            if (prefab != null)
            {
                go = GameObject.Instantiate(prefab) as GameObject;
#if UNITY_EDITOR
                ReplaceShader(go);
#endif
            }
            if (callback != null)
            {
                callback(go);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 在pc下测试移动平台的Prefab时，需要将assetbundle包中的shader替换为pc下的版本
        /// </summary>
        /// <param name="bundle"></param>
        private static void ReplaceShader(GameObject go)
        {
            UISprite[] sprites = go.GetComponentsInChildren<UISprite>(true);
            for (int i = 0; i < sprites.Length; i++)
            {
                Material material = sprites[i].material;
                if(material != null)
                {
                    string shaderName = material.shader.name;
                    Shader shader = Shader.Find(shaderName);
                    if (shader != null)
                    {
                        material.shader = shader;
                    }
                }
                else
                {
                    Logger.Log("UISpirte上未找到Material " + sprites[i].name);
                }
            }
        }
#endif

#endregion

#region 通用AssetBundle资源加载
        public static void GetAssetBundle(string url, Action<AssetBundle> callback)
        {
            string cacheUrl = CacheManager.GetCachedAssetUrl(url);
            if(AssetPool.HasAsset(cacheUrl) == true)
            {
                OnLoadAssetBundle(cacheUrl, callback);
            }
            else
            {
                if(CacheManager.IsAssetExists(url) == true)
                {
                    LoadAssetBundle(url, callback);
                }
                else
                {
                    CacheManager.LoadAsset(url, delegate (string pUrl) { LoadAssetBundle(pUrl, callback); });
                }
            }
        }

        private static void LoadAssetBundle(string url, Action<AssetBundle> callback)
        {
            AssetConfig config = new AssetConfig(AssetType.Assetbundle, false);
            if (CacheManager.IsAssetExists(url) == true)
            {
                url = CacheManager.GetCachedAssetUrl(url);
#if UNITY_5
                if(CanUseBundleLoader == true)
                {
                    _bundleLoader.LoadAsset(url, config, delegate (string pUrl) { OnLoadAssetBundle(pUrl, callback); });
                    return;
                }
#endif
            }
            _assetLoader.LoadAsset(url, config, delegate (string pUrl) { OnLoadAssetBundle(pUrl, callback); });
        }

        private static void OnLoadAssetBundle(string cacheUrl, Action<AssetBundle> callback)
        {
            AssetBundle assetBundle = AssetPool.GetAsset(cacheUrl) as AssetBundle;
            if(callback != null)
            {
                callback(assetBundle);
            }
        }
#endregion

#region 加载和获取图片资源
        public static void GetPortrait(string url, bool isCacheInMemory, Action<Texture2D> callback)
        {
            string cacheUrl = CacheManager.GetCachedAssetUrl(url);
            if (AssetPool.HasAsset(cacheUrl) == true)
            {
                OnLoadImage(cacheUrl, callback);
            }
            else
            {
                if (CacheManager.IsAssetExists(url) == true)
                {
                    LoadPortrait(cacheUrl, isCacheInMemory, callback);
                }
                else
                {
                    CacheManager.LoadAsset(url, delegate (string pUrl) { LoadPortrait(pUrl, isCacheInMemory, callback); });
                }
            }
        }

        private static void LoadPortrait(string url, bool isCacheInMemory, Action<Texture2D> callback)
        {
            AssetConfig config = new AssetConfig(AssetType.Portrait, isCacheInMemory);
            if (CacheManager.IsAssetExists(url) == true)
            {
                url = CacheManager.GetCachedAssetUrl(url);
            }
            _assetLoader.LoadAsset(url, config, delegate (string pUrl) { OnLoadImage(url, callback); });
        }

        public static void GetImage(string url, Action<Texture2D> callback)
        {
            GetImage(url, false, callback);
        }

        /// <summary>
        /// isCacheInMemory:图片加载后是否需要在内存中缓存，若为true，则需要手动释放内存
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isCacheInMemory"></param>
        /// <param name="callback"></param>
        public static void GetImage(string url, bool isCacheInMemory, Action<Texture2D> callback)
        {
            string cacheUrl = CacheManager.GetCachedAssetUrl(url);
            if (AssetPool.HasAsset(cacheUrl) == true)
            {
                OnLoadImage(cacheUrl, callback);
            }
            else
            {
                if(CacheManager.IsAssetExists(url) == true)
                {
                    LoadImage(url, isCacheInMemory, callback);
                }
                else
                {
                    CacheManager.LoadAsset(url, delegate (string pUrl) { LoadImage(pUrl, isCacheInMemory, callback); });
                }
            }
        }

        private static void LoadImage(string url, bool isCacheInMemory, Action<Texture2D> callback)
        {
            AssetConfig config = new AssetConfig(AssetType.Image, isCacheInMemory);
            if (CacheManager.IsAssetExists(url) == true)
            {
                url = CacheManager.GetCachedAssetUrl(url);
            }
            _assetLoader.LoadAsset(url, config, delegate (string pUrl) { OnLoadImage(url, callback); });
        }

        private static void OnLoadImage(string cacheUrl, Action<Texture2D> callback)
        {
            Texture2D texture = AssetPool.GetAsset(cacheUrl) as Texture2D;
            if (callback != null)
            {
                callback(texture);
            }
        }
#endregion

        public static byte[] GetLuaBytes(string name)
        {
            return GetLuaBytesFromPrefabAsset(name);                
        }

        public static byte[] GetLuaBytesFromPrefabAsset(string name)
        {
            string luaKey = name + ".lua.bytes";
            return AssetPool.GetLuaAsset(luaKey);
        }

        public static void ReleaseProgramAsset(RemoteConfig.AssetInfo assetInfo)
        {
            if(assetInfo == null)
            {
                return;
            }

            if(CacheManager.IsProgramAssetExists(assetInfo.name, assetInfo.md5) == true)
            {
                string cacheUrl = CacheManager.GetCachedProgramAssetUrl(assetInfo.url);
                AssetPool.ReleaseAsset(cacheUrl);
            }
            else
            {
                AssetPool.ReleaseAsset(assetInfo.url);
            }
        }

        public static void ReleaseAsset(string url)
        {
            if(CacheManager.IsAssetExists(url) == true)
            {
                string cacheUrl = CacheManager.GetCachedAssetUrl(url);
                AssetPool.ReleaseAsset(cacheUrl);
            }
            else
            {
                AssetPool.ReleaseAsset(url);
            }
        }

        /// <summary>
        /// 获取资源对象的通用接口
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static UnityEngine.Object GetAsset(string url)
        {
            string cacheUrl = CacheManager.GetCachedAssetUrl(url);
            return AssetPool.GetAsset(cacheUrl);
        }

        public static void Clear()
        {
            _assetLoader.Clear();
#if UNITY_5
            _bundleLoader.Clear();
#endif
            CacheManager.Clear();
            AssetPool.Clear();
        }

        /// <summary>
        /// 通过在管理端增加FunctionSwitch的方式，控制前端资源加载步骤中，资源从本地加载到内存的具体方式
        /// 默认为true
        /// </summary>
        private static bool CanUseBundleLoader
        {
            get
            {
                return !Pandora.Instance.GetRemoteConfig().GetFunctionSwitch("wwwLoader");
            }
        }

        public static void DeleteZeroReferenceAsset()
        {
            List<string> deleteList = AssetPool.DeleteZeroReferenceAsset();
            for (int i = 0; i < deleteList.Count; i++)
            {
                _assetLoader.RemoveFormLoadedSet(deleteList[i]);
#if UNITY_5
                _bundleLoader.RemoveFormLoadedSet(deleteList[i]);
#endif
            }
        }

    }

    /// <summary>
    /// 资源类型
    /// </summary>
    public enum AssetType
    {
        Lua,
        /// <summary>
        /// 普通Prefab资源
        /// </summary>
        Prefab,
        /// <summary>
        /// 普通图片资源
        /// </summary>
        Image,
        /// <summary>
        /// 普通Assetbunle资源
        /// </summary>
        Assetbundle,
        /// <summary>
        /// 玩家头像资源，可能为GIF格式
        /// </summary>
        Portrait,
        /// <summary>
        /// 伪装成Prefab的Lua资源
        /// </summary>
        LuaPrefab
    }

    public struct AssetConfig
    {
        public AssetType type;
        public bool isCacheInMemory;

        public AssetConfig(AssetType type, bool isCacheInMemory)
        {
            this.type = type;
            this.isCacheInMemory = isCacheInMemory;
        }
    }
}
