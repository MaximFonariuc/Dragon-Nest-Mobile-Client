using System.Collections.Generic;
using UnityEngine;

namespace com.tencent.pandora
{
    /// <summary>
    /// 将Log显示在屏幕GUI上
    /// </summary>
    internal class LogConsole : MonoBehaviour, ILogRecorder
    {
        private const int LOG_MAX_COUNT = 100;
        private const int OFFSET = 10;

        private static readonly Dictionary<int, Color> LOG_TYPE_COLOR_DICT = new Dictionary<int, Color>
        {
            {Logger.DEBUG, Color.white },
            {Logger.INFO, Color.white },
            {Logger.WARNING, Color.yellow },
            {Logger.ERROR, Color.red }
        };

        private bool _visible;
        private Vector2 _logListScrollPosition;
        private Vector2 _selectedScrollPosition;
        private Rect _windowRect;
        private bool _showDebug = true;
        private bool _showInfo = true;
        private bool _showWarning = true;
        private bool _showError = true;
        
        private List<Log> _logList;
        private Log _selectedLog;

        protected void Awake()
        {
            _windowRect = new Rect(OFFSET, OFFSET, Screen.width - OFFSET * 2, Screen.height  - OFFSET * 2);
            _logList = new List<Log>(LOG_MAX_COUNT);
        }

        protected void OnGUI()
        {
            if (_visible == false)
            {
                if(GUILayout.Button("显示Pandora LogGUI"))
                {
                    _visible = true;
                }
                return;
            }
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            _windowRect = GUILayout.Window(int.MaxValue, _windowRect, DrawWindowContent, "Console(点击键盘左上角`键关闭或显示，发生错误时强制显示)");
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
        }

        private void DrawWindowContent(int windowId)
        {
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            DrawLogList();
            DrawSelectedLog();
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            DrawLogSetting();
        }

        private void DrawLogList()
        {
            _logListScrollPosition = GUILayout.BeginScrollView(_logListScrollPosition, GUILayout.Height(Screen.height * 0.6f));
            for(int i = 0; i < _logList.Count; i++)
            {
                Log log = _logList[i];
                if(_showDebug == false && log.level == Logger.DEBUG)
                {
                    continue;
                }
                if(_showInfo == false && log.level == Logger.INFO)
                {
                    continue;
                }
                if(_showWarning == false && log.level == Logger.WARNING)
                {
                    continue;
                }
                if(_showError == false && log.level == Logger.ERROR)
                {
                    continue;
                }
                GUI.contentColor = LOG_TYPE_COLOR_DICT[log.level];
                int length = log.message.Length;
                string label = log.message.Substring(0, UnityEngine.Mathf.Min(200, length));
                if (GUILayout.Button(label))
                {
                    _selectedLog = log;
                }
            }
            GUILayout.EndScrollView();
            GUI.contentColor = Color.white;
        }

        private void DrawSelectedLog()
        {
            _selectedScrollPosition = GUILayout.BeginScrollView(_selectedScrollPosition);
            GUI.contentColor = LOG_TYPE_COLOR_DICT[_selectedLog.level];
            GUILayout.Label(_selectedLog.message + "\n" + _selectedLog.stackTrace);
            GUI.contentColor = Color.white;
            GUILayout.EndScrollView();
        }

        private void DrawLogSetting()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Log选项：");
            _showDebug = GUILayout.Toggle(_showInfo, "显示Debug");
            _showInfo = GUILayout.Toggle(_showInfo, "显示Info");
            _showWarning = GUILayout.Toggle(_showWarning, "显示警告");
            _showError = GUILayout.Toggle(_showError, "显示错误");
            if(GUILayout.Button("清空缓存"))
            {
                LocalDirectoryHelper.Clean();
            }
            if(GUILayout.Button("清空所有Log"))
            {
                _logList.Clear();
                _selectedLog = default(Log);
            }
#if UNITY_5
            if(GUILayout.Button("复制Log到剪贴板"))
            {

                GUIUtility.systemCopyBuffer = _selectedLog.level.ToString() + "\n" + _selectedLog.message + "\n" + _selectedLog.stackTrace;
            }
#endif
            if (GUILayout.Button("关闭LogGUI"))
            {
                _visible = false;
            }
            GUILayout.EndHorizontal();
        }

        public void Add(Log log)
        {
            if(_logList.Count >= LOG_MAX_COUNT)
            {
                _logList.RemoveAt(0);
            }
            _logList.Add(log);
        }

        public void Dispose()
        {
            _logList.Clear();
        }
    }
}

