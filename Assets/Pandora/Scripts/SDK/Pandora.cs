using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace com.tencent.pandora
{
    public class Pandora
    {
        #region singleton instance
        private static Pandora _instance;
        public static Pandora Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Pandora();
                }
                return _instance;
            }
        }
        #endregion


        private bool _isInitialized = false;
        //游戏UI面板的参考分辨率
        private Vector2 _referenceResolution;
        private GameObject _gameObject;
        private string _rootName;
        private UserData _userData;
        private RemoteConfig _remoteConfig;

        private BrokerSocket _brokerSocket;
        private AtmSocket _atmSocket;
        private LogHook _logHook;

        private Action<string> _jsonGameCallback;
        private Action<Dictionary<string, string>> _gameCallback;
        private Action<string> _playSoundDelegate;

        public Action<string, string, Action<Sprite>> GetIconSprite;

        //适配龙之谷NGUI文本的处理方式
        private Dictionary<string, Font> _fontDict = new Dictionary<string, Font>();

        private int _totalGroupCount;
        private int _loadedGroupCount;

        public void Init(bool isProductEnvironment, string rootName = "")
        {
            if(_isInitialized == false)
            {
                _isInitialized = true;
                _rootName = rootName;
                PandoraSettings.IsProductEnvironment = isProductEnvironment;
                PandoraSettings.ReadEnvironmentSetting();
                _userData = new UserData();
                LocalDirectoryHelper.Initialize();
                CreatePandoraGameObject();
                AddLogHook();
                CacheManager.Initialize();
                AssetManager.Initialize();
                PanelManager.Initialize();
                TextPartner.GetFont = GetFont;
                Logger.LogLevel = PandoraSettings.DEFAULT_LOG_LEVEL;
                Logger.LogInfo("<color=#0000ff>Pandora Init " + GetSystemInfo() + "</color>");
                _atmSocket = _gameObject.AddComponent<AtmSocket>();
                _atmSocket.Connect(PandoraSettings.GetAtmHost(), PandoraSettings.GetAtmPort(), null);
            }
        }

        private string GetSystemInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DeviceModel=");sb.Append(SystemInfo.deviceModel);
            sb.Append("&DeviceName="); sb.Append(SystemInfo.deviceName);
            sb.Append("&DeviceType="); sb.Append(SystemInfo.deviceType);
            sb.Append("&OperatingSystem="); sb.Append(SystemInfo.operatingSystem);
            sb.Append("&ProcessorType="); sb.Append(SystemInfo.processorType);
            sb.Append("&SystemMemorySize="); sb.Append(SystemInfo.systemMemorySize);
            sb.Append("&GraphicsDeviceName="); sb.Append(SystemInfo.graphicsDeviceName);
            return sb.ToString();
        }

        private void CreatePandoraGameObject()
        {
            if(string.IsNullOrEmpty(_rootName) == false)
            {
                _gameObject = GameObject.Find(_rootName);
            }
            if(_gameObject == null)
            {
                _gameObject = new GameObject("ui_pandora");
                UIRoot root =  _gameObject.AddComponent<UIRoot>();
                GameObject cameraGo = new GameObject("Camera");
                cameraGo.transform.SetParent(_gameObject.transform);
                Camera camera = cameraGo.AddComponent<Camera>();
                camera.cullingMask = LayerMask.GetMask("UI");
                camera.clearFlags = CameraClearFlags.Depth;
                camera.nearClipPlane = -100;
                camera.farClipPlane = 100;
                camera.orthographic = true;
                camera.orthographicSize = 1.0f;

                UICamera uiCamera = cameraGo.AddComponent<UICamera>();
                uiCamera.eventType = UICamera.EventType.UI;
                uiCamera.eventReceiverMask = LayerMask.GetMask("UI");
                GameObject.DontDestroyOnLoad(_gameObject);
            }
        }

        /// <summary>
        /// 在Pandora面板嵌入游戏面板的模式下，开启Pandora面板前，需要调用该接口设置Pandora面板挂接的父节点
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="panelParent"></param>
        public void SetPanelParent(string panelName, GameObject panelParent)
        {
            PanelManager.SetPanelParent(panelName, panelParent);
        }

        public bool EnableLogger
        {
            set
            {
#if UNITY_EDITOR
                Logger.Enable = value;
#else
                Logger.Enable = true;
#endif
            }
        }

        public GameObject GetGameObject()
        {
            return _gameObject;
        }

        private void AddLogHook()
        {
            _logHook = _gameObject.AddComponent<LogHook>();
        }

        public LogHook GetLogHook()
        {
            return _logHook;
        }

        public void SetPlaySoundDelegate(Action<string> playSoundDelegate)
        {
            _playSoundDelegate = playSoundDelegate;
        }

        public void PlaySound(string id)
        {
            if(_playSoundDelegate != null)
            {
                _playSoundDelegate(id);
            }
        }

        /// <summary>
        /// 设置游戏处理Pandora发往游戏的消息回调
        /// </summary>
        /// <param name="callback"></param>
        public void SetGameCallback(Action<Dictionary<string, string>> callback)
        {
            _gameCallback = callback;
            CacheManager.SetProgramAssetProgressCallback(ProgramAssetProgressCallback);
        }

        public void SetJsonGameCallback(Action<string> callback)
        {
            _jsonGameCallback = callback;
            CacheManager.SetProgramAssetProgressCallback(ProgramAssetProgressCallback);
        }

        private void ProgramAssetProgressCallback(Dictionary<string, string> dict)
        {
            CallGame(MiniJSON.Json.Serialize(dict));
        }

        public void CallGame(string jsonStr)
        {
            try
            {
                if(_jsonGameCallback != null)
                {
                    _jsonGameCallback(jsonStr);
                    return;
                }

                if(_gameCallback != null)
                {
                    Dictionary<string, System.Object> rawDict = MiniJSON.Json.Deserialize(jsonStr) as Dictionary<string, System.Object>;
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, System.Object> kvp in rawDict)
                    {
                        dict[kvp.Key] = (kvp.Value == null) ? string.Empty : kvp.Value as string;
                    }
                    _gameCallback(dict);
                }
            }
            catch(Exception e)
            {
                string error = "Lua发送消息,游戏回调发生异常, " + e.Message;
                Logger.LogError(error);
                Pandora.Instance.ReportError(error, ErrorCodeConfig.PANDORA_2_GAME_EXCEPTION);
                Pandora.Instance.Report("Pandora2GameException: " + jsonStr + "|" + e.Message, ErrorCodeConfig.PANDORA_2_GAME_EXCEPTION_DETAIL, ErrorCodeConfig.TNM2_TYPE_LITERALS);
            }
        }

        public void SetUserData(Dictionary<string, string> data)
        {
            if (_userData.IsRoleEmpty())
            {
                data["sSdkVersion"] = PandoraSettings.GetSdkVersion();
                _userData.Assign(data);
                Logger.LogInfo(_userData.ToString());
                LoadRemoteConfig(_userData);
            }
            else
            {
                _userData.Refresh(data);
            }
        }

        public UserData GetUserData()
        {
            return _userData;
        }

        private void LoadRemoteConfig(UserData userData)
        {
            AssetManager.LoadRemoteConfig(userData, OnGetRemoteConfig);
        }

        private void OnGetRemoteConfig(RemoteConfig remoteConfig)
        {
            if(remoteConfig.IsError == true || remoteConfig.IsEmpty == true)
            {
                CallGame("{\"type\":\"pandoraError\",\"content\":\"cgiFailed\"}");
                return;
            }
            
            if (remoteConfig.IsEmpty == false && remoteConfig.IsError == false)
            {
                Logger.LogInfo(_userData.ToString());
                Logger.LogInfo("<color=#00ff00>匹配到管理端规则：</color>  " + remoteConfig.ruleId.ToString());
                _remoteConfig = remoteConfig;
                LuaStateManager.Initialize();
                _brokerSocket = _gameObject.AddComponent<BrokerSocket>();
                _brokerSocket.AlternateIp1 = _remoteConfig.brokerAlternateIp1;
                _brokerSocket.AlternateIp2 = _remoteConfig.brokerAlternateIp2;
                _brokerSocket.Connect(_remoteConfig.brokerHost, _remoteConfig.brokerPort, OnBrokerConnected);
            }
        }

        private void OnBrokerConnected()
        {
            LoadProgramAsset();
        }

        internal void LoadProgramAsset()
        {
            _totalGroupCount = _remoteConfig.assetInfoListDict.Keys.Count;
            _loadedGroupCount = 0;
            foreach (KeyValuePair<string, List<RemoteConfig.AssetInfo>> kvp in _remoteConfig.assetInfoListDict)
            {
                AssetManager.LoadProgramAssetList(kvp.Key, kvp.Value, OnLoaded);
            }
        }

        public void LoadProgramAsset(string group)
        {
            Logger.Log("用户发起重连请求");
            if(_userData == null || _userData.IsRoleEmpty())
            {
                return;
            }

            if(_remoteConfig == null)
            {
                LoadRemoteConfig(_userData);
                ReportError("用户成功发起重连请求!", ErrorCodeConfig.START_RELOAD);
                return;
            }
            _totalGroupCount = 1;
            _loadedGroupCount = 0;
            if(_remoteConfig != null && _remoteConfig.assetInfoListDict.ContainsKey(group) == true && LuaStateManager.IsGroupLuaExecuting(group) == false)
            {
                AssetManager.LoadProgramAssetList(group, _remoteConfig.assetInfoListDict[group], OnLoaded);
                ReportError("用户成功发起重连请求!", ErrorCodeConfig.START_RELOAD);
            }
        }

        private void OnLoaded(string group, List<RemoteConfig.AssetInfo> fileInfoList)
        {
            _loadedGroupCount += 1;
            if(_loadedGroupCount >= _totalGroupCount)
            {
                CacheManager.WriteProgramAssetMeta();
            }
            CallGame("{\"type\":\"assetLoadComplete\",\"name\":\"" + group + "\"}");
            LuaStateManager.DoLuaFileInFileInfoList(group, fileInfoList);
        }

        public bool IsProgramAssetReady(string group)
        {
            if(_remoteConfig == null && _remoteConfig.assetInfoListDict.ContainsKey(group) == false)
            {
                return false;
            }

            return CacheManager.IsProgramAssetExists(_remoteConfig.assetInfoListDict[group]);
        }

        public RemoteConfig GetRemoteConfig()
        {
            return _remoteConfig;
        }

        internal BrokerSocket BrokerSocket
        {
            get
            {
                return _brokerSocket;
            }
        }

        /// <summary>
        /// 设置Pandora模块使用的字体
        /// </summary>
        /// <param name="font"></param>
        public void SetFont(Font font)
        {
            if(_fontDict.ContainsKey(font.name) == false)
            {
                _fontDict.Add(font.name, font);
            }
        }

        internal Font GetFont(string name)
        {
            if(_fontDict.ContainsKey(name))
            {
                return _fontDict[name];
            }
            Font font = Resources.Load<Font>("StaticUI/" + name);
            if(font != null)
            {
                _fontDict.Add(name, font);
            }
            return font;
        }

        /// <summary>
        /// 游戏往Pandora发送消息
        /// </summary>
        /// <param name="commandDict"></param>
        public void Do(Dictionary<string, string> commandDict)
        {
            CSharpInterface.GameCallLua(MiniJSON.Json.Serialize(commandDict));
        }

        public void DoJson(string jsonStr)
        {
            CSharpInterface.GameCallLua(jsonStr);
        }

        public bool PauseDownloading
        {
            get
            {
                return CacheManager.PauseDownloading;
            }
            set
            {
                CacheManager.PauseDownloading = value;
            }
        }

        public bool PauseSocketSending
        {
            get
            {
                return _brokerSocket.PauseSending;
            }
            set
            {
                _brokerSocket.PauseSending = value;
                _atmSocket.PauseSending = value;
            }
        }

        public void ReportError(string error, int id = 0)
        {
            Report(error, id, 0);
        }

        public void Report(string content, int id, int type)
        {
            if(_atmSocket != null)
            {
                _atmSocket.Report(content, id, type);
            }
        }

        // 是否要将Log保存在本地
        public bool IsDebug
        {
            get;
            set;
        }

		// 是否使用Https进行网络访问
        public bool UseHttps
        {
            get;
            set;
        }


        public static Dictionary<string, int> GetAssetReferenceCountDict()
        {
            return AssetPool.GetAssetReferenceCountDict();
        }

        public void Logout()
        {
            try
            {
                if (_isInitialized == true)
                {
                    if (_atmSocket != null)
                    {
                        _atmSocket.Close();
                    }
                    if (_brokerSocket != null)
                    {
                        _brokerSocket.Close();
                    }
                    _userData.Clear();
                    PanelManager.DestroyAll();
                    LuaStateManager.Reset();
                    AssetManager.Clear();
                }
            }
            catch(Exception e)
            {
                Logger.LogError("Pandora Logout发生异常: " + e.Message);
            }
        }
        #region Pandora WebView
        private PandoraWebView _webView;
        private bool _canUseWebView;
        private Func<string, string> _urlEncodeDelegate;

        /// <summary>
        /// 初始化WebView，openInOtherProcess：在安卓上是否开启新的进程来打开页面
        /// </summary>
        /// <param name="openInOtherProcess"></param>
        public void InitWebView(bool openInOtherProcess = false)
        {
#if UNITY_EDITOR || UNITY_IOS
            if (_webView == null)
            {
                _webView = _gameObject.AddComponent<PandoraWebView>();
                _canUseWebView = _webView.CanUseWebView;
                if(_canUseWebView == true)
                {
                    _webView.SetPageLoadedCallback(OnPageLoaded);
                    _webView.SetPageMessageCallback(OnPageMessage);
                    _webView.SetPageBackCallback(OnPageBack);
                    _webView.Init(openInOtherProcess);
                }
                else
                {
                    Logger.LogInfo("Pandora WebView 当前不可用~");
                }
            }
#else
            return;
#endif
        }

        // 游戏设置对URL加密的delegate(对URL加密使用MSDK的加密方法)
        public void SetUrlEncodeDelegate(Func<string, string> urlEncodeDelegate)
        {
            _urlEncodeDelegate = urlEncodeDelegate;
        }

        private string EncodeUrl(string url)
        {
            if(url.Contains("?"))
            {
                url += "&";
            }
            else
            {
                url += "?";
            }
            url += _userData.GetUrlParams();
            try
            {
                if(_urlEncodeDelegate != null)
                {
                    url = _urlEncodeDelegate(url);
                }
            }
            catch(Exception e)
            {
                Logger.LogError("Url Encoding Error: " + e.Message + ":" + e.StackTrace);
            }
            return url;
        }

        /// <summary>
        /// Pandora发送消息给WebView
        /// </summary>
        /// <param name="message"></param>
        public void WritePageMessage(string message)
        {
            if (_webView != null && _canUseWebView == true)
            {
                _webView.WriteMessage(message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="openInOtherProcess"></param>
        /// <param name="color"></param>
        /// <param name="delayShow"></param>
        public void OpenPage(string url, bool openInOtherProcess, string color = "#ffffff", bool delayShow = false)
        {
            if(_webView != null && _canUseWebView == true)
            {
                url = EncodeUrl(url);
                _webView.OpenUrl(url, openInOtherProcess, color, delayShow);
            }
        }

        public void ClosePage()
        {
            if(_webView != null && _canUseWebView == true)
            {
                _webView.Close();
            }
        }

        public void ShowPage()
        {
            if (_webView != null && _canUseWebView == true)
            {
                _webView.Show();
            }
        }

        public void HidePage()
        {
            if (_webView != null && _canUseWebView == true)
            {
                _webView.Hide();
            }
        }

        private void OnPageLoaded()
        {
            CSharpInterface.OnPageLoaded();
        }

        private void OnPageMessage(string message)
        {
            CSharpInterface.OnPageMessage(message);
        }

        private void OnPageBack(string isDown)
        {
            CSharpInterface.OnPageBack(isDown);
        }
#endregion
    }
}
