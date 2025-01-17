using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

namespace com.tencent.pandora
{
    internal class CookieHelper
    {
        public static bool Write(string fileName, string content)
        {
            try
            {
                string path = Path.Combine(LocalDirectoryHelper.GetCookieFolderPath(), fileName);
                File.WriteAllText(path, content);
                return true;
            }
            catch(Exception e)
            {
                string error = "写入Cookie失败， " + fileName + " " + content + e.Message;
                Pandora.Instance.ReportError(error);
                Logger.LogError(error);
            }
            return false;
        }

        public static string Read(string fileName)
        {
            try
            {
                string path = Path.Combine(LocalDirectoryHelper.GetCookieFolderPath(), fileName);
                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }
            }
            catch(Exception e)
            {
                string error = "读取Cookie失败， " + fileName + e.Message;
                Pandora.Instance.ReportError(error);
                Logger.LogError(error);
            }
            return string.Empty;
        }
    }
}
