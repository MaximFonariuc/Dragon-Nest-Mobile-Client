using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.tencent.pandora
{
    internal class PanelManager
    {
        public const int SUCCESS = 0;
        public const int ERROR_ASSET_NOT_EXISTS = -1;
        public const int ERROR_NO_PARENT = -2;

        //TODO:是否允许存在同一个面板资源的多个实例还需要商榷
        public const int ERROR_ALREADY_EXISTS = -3;
        /// <summary>
        /// 面板父节点字典
        /// Key为面板的名称，Value为父节点对象
        /// </summary>
        private static Dictionary<string, GameObject> _panelParentDict;
        /// <summary>
        /// 面板字典
        /// Key为面板名称，Value为面板对象
        /// </summary>
        private static Dictionary<string, GameObject> _panelDict;

        public static void Initialize()
        {
            _panelParentDict = new Dictionary<string, GameObject>();
            _panelDict = new Dictionary<string, GameObject>();
        }

        public static void SetPanelParent(string name, GameObject parent)
        {
            if(_panelParentDict.ContainsKey(name) == false)
            {
                _panelParentDict.Add(name, parent);
            }
            else
            {
                _panelParentDict[name] = parent;
            }
        }

        public static GameObject GetPanelParent(string moduleName)
        {
            if (_panelParentDict.ContainsKey(moduleName))
            {
                return _panelParentDict[moduleName];
            }
            else
            {
                Logger.LogError("找不到游戏设置的module父节点:" + moduleName);
                return null;
            }
        }

        public static GameObject GetPanel(string name)
        {
            if(_panelDict.ContainsKey(name) == false)
            {
                Logger.LogWarning("面板不存在， " + name);
                return null;
            }
            return _panelDict[name];
        }

        //TODO:后续考虑加入加载完Prefab后，实例化Go的时候，指定新的名字
        public static void CreatePanel(string name, Action<int> callback)
        {
            string fullName = GetPanelAssetFullName(name);
            RemoteConfig.AssetInfo assetInfo = Pandora.Instance.GetRemoteConfig().GetAssetInfo(fullName);
            AssetManager.GetPanelGameObject(assetInfo, delegate(GameObject go) { OnGetGameObject(go, callback); });
        }

        public static void OnGetGameObject(GameObject go, Action<int> callback)
        {
            go.name = go.name.Replace("_copy(Clone)", "");
            string name = go.name;
            Transform parentTrans = null;
            if (_panelDict.ContainsKey(name) == true)
            {
                string error = "已经存在同名面板。 " + name;
                Logger.LogError(error);
                Pandora.Instance.ReportError(error);
                AssetManager.ReleaseProgramAsset(GetPanelAssetAssetInfo(name));
                Destroy(go);
                callback(ERROR_ALREADY_EXISTS);
                return;
            }

            if (_panelParentDict.ContainsKey(name) == true)
            {
                if (_panelParentDict[name] == null)
                {
                    string error = "面板配置的父节点已经不存在: " + name;
                    Logger.LogError(error);
                    Pandora.Instance.ReportError(error);
                    AssetManager.ReleaseProgramAsset(GetPanelAssetAssetInfo(name));
                    Destroy(go);
                    callback(ERROR_NO_PARENT);
                    return;
                }
                else
                {
                    parentTrans = _panelParentDict[name].transform;
                }
            }
            if(parentTrans == null)
            {
                parentTrans = Pandora.Instance.GetGameObject().transform;
            }
            go.transform.SetParent(parentTrans);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            go.SetActive(true);
            _panelDict.Add(name, go);
            callback(SUCCESS);
        }

        /// <summary>
        /// 显示玩家头像资源，注意图片格式可能是GIF格式，当头像格式为GIF时，显示第一帧图片
        /// </summary>
        public static void ShowPortrait(string panelName, string url, GameObject go, bool isCacheInMemory, Action<int> callback)
        {
            if (_panelDict.ContainsKey(panelName) == false)
            {
                Logger.LogError("面板不存在, " + panelName);
                callback(-1);
                return;
            }

            if (go == null)
            {
                Logger.LogError("面板中指定的go不存在, " + panelName);
                callback(-3);
                return;
            }

            UITexture uiTexture = go.GetComponent<UITexture>();
            if (uiTexture == null)
            {
                uiTexture = go.AddComponent<UITexture>();
            }
            AssetManager.GetPortrait(url, isCacheInMemory, delegate (Texture2D texture) { OnGetImage(uiTexture, texture, url, callback); });
        }

        public static void ShowImage(string panelName, string url, UITexture uiTexture, bool isCacheInMemory, Action<int> callback)
        {
            if(_panelDict.ContainsKey(panelName) == false)
            {
                Logger.LogError("面板不存在, " + panelName);
                callback(-1);
                return;
            }
            if(uiTexture == null)
            {
                Logger.LogError("面板中指定的Image组件不存在, " + panelName);
                callback(-2);
                return;
            }

            AssetManager.GetImage(url, isCacheInMemory, delegate (Texture2D texture) { OnGetImage(uiTexture, texture, url, callback); });
        }

        public static void ShowImage(string panelName, string url, GameObject go, bool isCacheInMemory, Action<int> callback)
        {
            if (_panelDict.ContainsKey(panelName) == false)
            {
                Logger.LogError("面板不存在, " + panelName);
                callback(-1);
                return;
            }

            if(go == null)
            {
                Logger.LogError("面板中指定的go不存在, " + panelName);
                callback(-3);
                return;
            }

            UITexture uiTexture = go.GetComponent<UITexture>();
            if(uiTexture == null)
            {
                uiTexture = go.AddComponent<UITexture>();
            }
            AssetManager.GetImage(url, isCacheInMemory, delegate (Texture2D texture) { OnGetImage(uiTexture, texture, url, callback); });
        }

        private static void OnGetImage(UITexture uiTexture, Texture2D texture, string url, Action<int> callback)
        {
            if(uiTexture == null)
            {
                Logger.LogError("目标UITexture已经不存在");
                AssetManager.ReleaseAsset(url);
                callback(-4);
                return;
            }
            uiTexture.SetRuntimeTexture(texture, false);
            //uiTexture.mainTexture = texture;
            callback(0);
        }

        public static void Hide(string name)
        {
            if(_panelDict.ContainsKey(name))
            {
                GameObject go = _panelDict[name];
                if(go != null)
                {
                    go.SetActive(false);
                }
            }
        }

        public static void HideAll()
        {
            foreach(KeyValuePair<string, GameObject> kvp in _panelDict)
            {
                Hide(kvp.Key);
            }
        }

        public static void Destroy(string name)
        {
            if (_panelDict.ContainsKey(name))
            {
                GameObject go = _panelDict[name];
                if(go != null)
                {
                    Destroy(go);
                }
                AssetManager.ReleaseProgramAsset(GetPanelAssetAssetInfo(name));
                _panelDict.Remove(name);
            }
        }

        private static void Destroy(GameObject go)
        {
            if(go != null)
            {
                UnityEngine.Object.Destroy(go);
            }
        }

        public static void DestroyAll()
        {
            List<string> nameList = new List<string>(_panelDict.Keys);
            for(int i = 0; i< nameList.Count; i++)
            {
                Destroy(nameList[i]);
            }
        }

        private static string GetPanelAssetFullName(string name)
        {
            return CSharpInterface.GetPlatformDescription() + "_" + name.ToLower() + ".assetbundle";
        }

        private static RemoteConfig.AssetInfo GetPanelAssetAssetInfo(string name)
        {
            string fullName = GetPanelAssetFullName(name);
            RemoteConfig.AssetInfo assetInfo = Pandora.Instance.GetRemoteConfig().GetAssetInfo(fullName);
            return assetInfo;
        }
    }
}
