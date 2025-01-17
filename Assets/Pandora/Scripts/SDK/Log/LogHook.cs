using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace com.tencent.pandora
{
    /// <summary>
    /// 3种Log
    /// 1.Unity Console
    /// 2.屏幕GUI显示
    /// 3.本地Log文件
    /// 在Unity5以下版本上使用该接口时，需要留意项目组是否占用Application.RegisterLogCallback
    /// 若占用的话则可以通过将Pandora.Instance.GetLogHook().HandleLog添加至项目组的Log回调中
    /// </summary>
    public class LogHook : MonoBehaviour
    {
        private const string PANDORA_NAMESPACE = "com.tencent.pandora";
        private List<ILogRecorder> _recorderList = new List<ILogRecorder>();
        private bool _localLogSetting = false;
        private bool _isWriterAdded;
        private bool _isConsoleAdded;

        protected void Awake()
        {
            ReadSettings();
        }

        private void ReadSettings()
        {
            //从本地settings中读取log设置
            try
            {
                string filePath = LocalDirectoryHelper.GetSettingsFolderPath() + "/settings.txt";
                if (File.Exists(filePath) == true)
                {
                    string content = File.ReadAllText(filePath);
                    Dictionary<string, System.Object> dict = MiniJSON.Json.Deserialize(content) as Dictionary<string, System.Object>;
                    if(dict.ContainsKey("log") == true)
                    {
                        _localLogSetting = (dict["log"] as string) == "1";
                    }
                }
                
            }
            catch
            {
                //left blank
            }
        }

        private void Update()
        {
            if ( Logger.Enable == true
                && (Application.platform == RuntimePlatform.WindowsEditor
                || PandoraSettings.IsProductEnvironment == false
                || (PandoraSettings.IsProductEnvironment == true && Pandora.Instance.IsDebug == true)
                || _localLogSetting == true))
            {
                if(Logger.HandleLog == null)
                {
                    Logger.HandleLog = HandleLog;
                }
                if(_isWriterAdded == false)
                {
                    _isWriterAdded = true;
                    _recorderList.Add(new LogWriter());
                }
                if(_isConsoleAdded == false)
                {
                    if(Pandora.Instance.IsDebug == true && Pandora.Instance.GetRemoteConfig() != null && Pandora.Instance.GetRemoteConfig().GetFunctionSwitch("console") == true)
                    {
                        _isConsoleAdded = true;
                        _recorderList.Add(this.gameObject.AddComponent<LogConsole>());
                    }
                }
            }
            else
            {
                Logger.HandleLog = null;
            }
        }

        public void HandleLog(string message, string stackTrace, int level)
        {
            if (_recorderList.Count == 0)
            {
                return;
            }

            Log log = new Log
            {
                message = message,
                stackTrace = stackTrace,
                level = level
            };

            for (int i = 0; i < _recorderList.Count; i++)
            {
                _recorderList[i].Add(log);
            }
        }

    }
}

