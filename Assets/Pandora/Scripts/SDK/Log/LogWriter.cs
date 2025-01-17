using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace com.tencent.pandora
{
    /// <summary>
    /// 将Log写入本地
    /// 可以在管理端单配规则，来调试线上发生的问题
    /// </summary>
    internal class LogWriter : ILogRecorder
    {
        private string _currentLogPath;

        public void Add(Log log)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(log.level.ToString());
            sb.Append("  ");
            sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            sb.Append("\n");
            sb.Append(log.message);
            sb.Append("\n");
            sb.Append(log.stackTrace);
            sb.Append("\n");
            Write(sb.ToString());
        }

        private void Write(string content)
        {
            try
            {
                string path = GetLogFilePath();
                if (path != _currentLogPath)
                {
                    _currentLogPath = path;
                }
                File.AppendAllText(_currentLogPath, content);
            }
            catch
            {
                //left blank
            }
        }

        private string GetLogFilePath()
        {
            return LocalDirectoryHelper.GetLogFolderPath() + "/log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
        }

        public void Dispose()
        {
        }
    }
}
