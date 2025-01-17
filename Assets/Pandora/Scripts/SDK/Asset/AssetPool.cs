using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace com.tencent.pandora
{
    /// <summary>
    /// 注意：
    /// 1.因为将Lua打包为AssetBundle时，lua文件的路径信息会丢失,所以模块下的Lua文件不能有子目录，并且所有Lua文件不能重名
    /// 2.Pandora资源Prefab暂不使用资源公用机制，所以资源加载后可以立马Unload(false)
    /// </summary>
    public class AssetPool
    {
        /// <summary>
        /// lua资源不释放
        /// </summary>
        public const int LUA_INIT_REFERENCE_COUNT = 999;
        //资源引用计数为0后再缓存一段时间后释放引用
        public const int DELETE_ASSET_INTERVAL = 60;

        //强引用资源字典
        private static Dictionary<string, Asset> _strongAssetDict = new Dictionary<string, Asset>();
        //弱引用资源字典
        //短时间内持有资源的一个引用，释放引用后可以被Resource.UnloadUnusedAssets回收
        //可以避免短时间内加载重复资源时创建重复的内存
        private static Dictionary<string, Asset> _weakAssetDict = new Dictionary<string, Asset>();
        private static List<string> _deletedKeyList = new List<string>();
        private static Dictionary<string, byte[]> _luagAssetDict = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        public static bool ParseAsset(string url, AssetBundle bundle, AssetConfig config)
        {
            switch (config.type)
            {
                case AssetType.Lua:
                    return ParseLuaAsset(url, bundle);
                case AssetType.Prefab:
                    return ParsePrefabAsset(url, bundle, config.isCacheInMemory);
                case AssetType.Assetbundle:
                    return ParseAssetBundle(url, bundle);
                case AssetType.LuaPrefab:
                    return ParseLuaPrefabAsset(url, bundle, config.isCacheInMemory);
            }
            return false;
        }

        public static bool ParseAsset(string url, WWW www, AssetConfig config)
        {
            switch(config.type)
            {
                case AssetType.Lua:
                    return ParseLuaAsset(url, www.assetBundle);
                case AssetType.Prefab:
                    return ParsePrefabAsset(url, www.assetBundle, config.isCacheInMemory);
                case AssetType.Image:
                    return ParseImageAsset(url, www, config.isCacheInMemory);
                case AssetType.Assetbundle:
                    return ParseAssetBundle(url, www.assetBundle);
                case AssetType.Portrait:
                    return ParsePortraitAsset(url, www, config.isCacheInMemory);
            }
            return false;
        }

        public static bool ParseLuaAsset(string url, AssetBundle assetBundle)
        {
            bool result = false;
            try
            {
                UnityEngine.Object[] luas = assetBundle.LoadAllAssets(typeof(TextAsset));

                for (int i = 0; i < luas.Length; i++)
                {
                    UnityEngine.Object o = luas[i];
                    AddAsset(o.name, o, LUA_INIT_REFERENCE_COUNT, true);
                }
                assetBundle.Unload(false);
                result = true;
            }
            catch (Exception e)
            {
                string error = "资源解析失败 " + url + "\n" + e.Message;
                Pandora.Instance.ReportError(error, ErrorCodeConfig.ASSET_PARSE_FAILED);
                Logger.LogError(error);
                LocalDirectoryHelper.DeleteAssetByUrl(url);
            }
            return result;
        }

        //支持以大小写不敏感的方式搜索Lua资源
        public static UnityEngine.Object FindLuaAsset(string key)
        {
            var enumerator = _strongAssetDict.Keys.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(enumerator.Current.ToLower() == key)
                {
                    return _strongAssetDict[enumerator.Current].Object;
                }
            }
            return null;
        }


        // kiddpeng add 这里referenceCount  isCacheInMemory暂时没用
        private static void AddLuaAsset(string key, byte[] content, int referenceCount, bool isCacheInMemory)
        {
            if (_luagAssetDict.ContainsKey(key) == true)
            {
                Logger.LogError("LuaAssetDict 重复资源： " + key);
                return;
            }
            _luagAssetDict.Add(key, content);
        }

        private static void AddAsset(string key, UnityEngine.Object obj, int referenceCount, bool isCacheInMemory)
        {
            if(_strongAssetDict.ContainsKey(key) == true)
            {
                Logger.LogError("StrongAssetDict 重复资源： " + key);
                return;
            }
            if(_weakAssetDict.ContainsKey(key) == true)
            {
                Logger.LogError("WeakAssetDict 重复资源： " + key);
                return;
            }
            
            if(isCacheInMemory == true)
            {
                _strongAssetDict.Add(key, new Asset(key, obj, referenceCount));
            }
            else
            {
                _weakAssetDict.Add(key, new Asset(key, obj, 0));
            }
        }

        public static bool HasAsset(string key)
        {
            return IsInStrongDict(key) || IsInWeakDict(key);
        }

        private static bool IsInStrongDict(string key)
        {
            if (_strongAssetDict.ContainsKey(key) == false)
            {
                //Logger.Log("资源尚不存在StrongDict缓存中，Key： " + key);
                return false;
            }
            return true;
        }

        private static bool IsInWeakDict(string key)
        {
            if(_weakAssetDict.ContainsKey(key) == false)
            {
                //Logger.Log("资源尚不存在WeakDict缓存中，Key： " + key);
                return false;
            }
            return true;
        }

        public static UnityEngine.Object GetAsset(string key)
        {
            if(IsInStrongDict(key) == true)
            {
                Asset asset = _strongAssetDict[key];
                asset.ReferenceCount += 1;
                return asset.Object;
            }
            else if(IsInWeakDict(key) == true)
            {
                Asset asset = _weakAssetDict[key];
                return asset.Object;
            }
            return null;
        }

        public static byte[] GetLuaAsset(string key)
        {
            if (_luagAssetDict.ContainsKey(key) == true)
            {
                return _luagAssetDict[key];
            }
            else
            {
                return null;
            }
        }

        public static void ReleaseAsset(string key)
        {
            if(IsInStrongDict(key) == false)
            {
                return;
            }
            Asset asset = _strongAssetDict[key];
            asset.ReferenceCount -= 1;
        }
        
        public static bool ParseLuaPrefabAsset(string url, AssetBundle assetBundle, bool isCacheInMemory)
        {
            bool result = false;
            try
            {
                UnityEngine.Object[] prefabs = assetBundle.LoadAllAssets(typeof(GameObject));

                if (prefabs.Length > 1)
                {
                    throw new Exception("资源打包不符合规范，一个AssetBundle中只能有一个Prefab， " + url);
                }
                for (int i = 0; i < prefabs.Length; i++)
                {
                    GameObject prefab = prefabs[i] as GameObject;

                    if (prefab != null)
                    {
                        LuaAssetPartner partner = prefab.GetComponent<LuaAssetPartner>();

                        for (int k = 0; k < partner.listLuaFileName.Count; k++)
                        {
                            string fileName = partner.listLuaFileName[k];
                            string content = partner.listLuaContent[k];

                            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(content);
                            AddLuaAsset(fileName, byteArray, LUA_INIT_REFERENCE_COUNT, true);
                        }
                    }
                }
                assetBundle.Unload(true);
                result = true;
            }
            catch (Exception e)
            {
                string error = "资源解析失败： " + url + "\n" + e.Message;
                Pandora.Instance.ReportError(error, ErrorCodeConfig.ASSET_PARSE_FAILED);
                Logger.LogError(error);
                LocalDirectoryHelper.DeleteAssetByUrl(url);
            }
            return result;
        }

        public static bool ParsePrefabAsset(string url, AssetBundle assetBundle, bool isCacheInMemory)
        {
            bool result = false;
            try
            {
                UnityEngine.Object[] prefabs = assetBundle.LoadAllAssets(typeof(GameObject));
                
                if (prefabs.Length > 1)
                {
                    throw new Exception("资源打包不符合规范，一个AssetBundle中只能有一个Prefab， " + url);
                }
                for (int i = 0; i < prefabs.Length; i++)
                {
                    UnityEngine.Object o = prefabs[i];
                    AddAsset(url, o, 0, isCacheInMemory);
                }
                assetBundle.Unload(false);
                result = true;
            }
            catch (Exception e)
            {
                string error = "资源解析失败： " + url + "\n" + e.Message;
                Pandora.Instance.ReportError(error, ErrorCodeConfig.ASSET_PARSE_FAILED);
                Logger.LogError(error);
                LocalDirectoryHelper.DeleteAssetByUrl(url);
            }
            return result;
        }

        public static bool ParseImageAsset(string url, WWW www, bool isCacheInMemory)
        {
            bool result = false;
            try
            {
                Texture2D texture = www.textureNonReadable;
                texture.name = url;
                AddAsset(url, texture, 0, isCacheInMemory);
                result = true;
            }
            catch (Exception e)
            {
                string error = "资源解析失败： " + url + "\n" + e.Message;
                Pandora.Instance.ReportError(error, ErrorCodeConfig.ASSET_PARSE_FAILED);
                Logger.LogError(error);
                LocalDirectoryHelper.DeleteAssetByUrl(url);
            }
            return result;
        }

        public static bool ParseAssetBundle(string url, AssetBundle assetBundle)
        {
            AddAsset(url, assetBundle, 0, false);
            return true;
        }




        public static bool ParsePortraitAsset(string url, WWW www, bool isCacheInMemory)
        {
            bool result = false;
            try
            {
                Texture2D texture = GIFDecoder.Decode(www.bytes) ?? www.textureNonReadable;
                texture.name = url;
                AddAsset(url, texture, 0, isCacheInMemory);
                result = true;
            }
            catch (Exception e)
            {
                string error = "资源解析失败： " + url + "\n" + e.Message;
                Pandora.Instance.ReportError(error, ErrorCodeConfig.ASSET_PARSE_FAILED);
                Logger.LogError(error);
                LocalDirectoryHelper.DeleteAssetByUrl(url);
            }
            return result;
        }

        public static void Clear()
        {
            _strongAssetDict.Clear();
            _weakAssetDict.Clear();
            _luagAssetDict.Clear();
        }

        public static List<string> DeleteZeroReferenceAsset()
        {
            _deletedKeyList.Clear();
            DeleteZeroReferenceAsset(_strongAssetDict);
            DeleteZeroReferenceAsset(_weakAssetDict);
            return _deletedKeyList;
        }

        private static void DeleteZeroReferenceAsset(Dictionary<string, Asset> assetDict)
        {
            var enumerator = assetDict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string key = enumerator.Current.Key;
                Asset asset = enumerator.Current.Value;
                if (Time.realtimeSinceStartup > asset.ReleaseTime)
                {
                    _deletedKeyList.Add(key);
                }
            }

            for (int i = 0; i < _deletedKeyList.Count; i++)
            {
                assetDict.Remove(_deletedKeyList[i]);
            }
        }

        /// <summary>
        /// 供UnityEditor下检查资源引用次数工具调用
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, int> GetAssetReferenceCountDict()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            var sEnumerator = _strongAssetDict.Keys.GetEnumerator();
            while (sEnumerator.MoveNext())
            {
                string key = sEnumerator.Current;
                result.Add(key, _strongAssetDict[key].ReferenceCount);
            }
            var wEnumerator = _weakAssetDict.Keys.GetEnumerator();
            while (wEnumerator.MoveNext())
            {
                string key = wEnumerator.Current;
                result.Add(key, _weakAssetDict[key].ReferenceCount);
            }
            return result;
        }

        internal class Asset
        {
            private int _referenceCount;

            public Asset(string key, UnityEngine.Object obj, int referenceCount)
            {
                this.Key = key;
                this.Object = obj;
                this.ReferenceCount = referenceCount;
            }
            /// <summary>
            /// 资源在相关字典中的Key，面板Prefab的Key为name，其他资源是url
            /// </summary>
            public string Key { get; set; }

            public UnityEngine.Object Object { get; set; }

            public float ReleaseTime { get; set; }

            public int ReferenceCount
            {
                get
                {
                    return _referenceCount;
                }
                set
                {
                    _referenceCount = value;
                    if(_referenceCount == 0)
                    {
                        this.ReleaseTime = Time.realtimeSinceStartup + AssetPool.DELETE_ASSET_INTERVAL;
                    }
                    else
                    {
                        this.ReleaseTime = float.MaxValue;
                    }
                }
            }

        }

    }
}
