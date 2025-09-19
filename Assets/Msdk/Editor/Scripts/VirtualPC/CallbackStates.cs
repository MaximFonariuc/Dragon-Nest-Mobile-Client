using UnityEngine;
using System;
using System.IO;
using LitJson;


class CallbackStates
{
    private static readonly string filePath = Directory.GetCurrentDirectory() + 
        "/Assets/Msdk/Editor/Resources/CallbackSettings.txt";

    public string loginState = "";
    public string authState = "";
    public string shareState = "";
    public string relationState = "";
    public string wakeupState = "";
    public string groupState = "";
    public string cardState = "";
    public string locationState = "";
    public string nearbyState = "";

    // 更新配置项到本地
    public bool Update()
    {
        try {
            string json = JsonMapper.ToJson(this);
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
    
    // 加载本地配置项,无则返回空
    public static CallbackStates Load()
    {
        if (!File.Exists(filePath)) {
            return null;
        }
        try {
            StreamReader streamReader = new StreamReader(filePath);
            string json = streamReader.ReadToEnd();
            streamReader.Close();
            CallbackStates config = JsonMapper.ToObject<CallbackStates>(json);
            return config;
        } catch (Exception e) {
            Debug.LogWarning(e.Message);
            return null;
        }
    }

}
