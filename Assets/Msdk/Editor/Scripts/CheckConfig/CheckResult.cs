using UnityEngine;
using System;
using System.IO;
using LitJson;

public class CheckResult
{
    public string platform = "";
    // Common
    public string domain = "";
    public bool push = false;
    public bool refresh = true;
    public bool notice = false;

    // Android
    public bool beta = false;
    public bool update = false;
    public bool grayTest = false;
    public string logLevel = "1";
    public bool statLog = true;
    public bool closeBugly = false;
    public bool v2sign = false;
    public bool xgOtherPushEnable = true;

    public string projectPath = "";
    public string resultError = "";
    public string resultWarnning = "";
    

    public static bool WriteTo(string filePath, CheckResult result) 
    {
        try {
            string json = JsonMapper.ToJson(result);
            StreamWriter streamWriter = new StreamWriter(filePath, false);
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
            return true;
        } catch (Exception e) {
            Debug.LogException(e);
            return false;
        }
    }

    public static CheckResult ReadFrom(string filePath) 
    {
        try {
            StreamReader streamReader = new StreamReader(filePath);
            string json = streamReader.ReadToEnd();
            streamReader.Close();
            CheckResult result = JsonMapper.ToObject<CheckResult>(json);
            return result;
        } catch (Exception e) {
            Debug.LogException(e);
            return null;
        }
    }

}

