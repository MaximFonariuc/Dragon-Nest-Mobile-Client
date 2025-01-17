using Assets.SDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;
using XUtliPoolLib;

class XIFlyMgr : MonoBehaviour, IXIFlyMgr
{
    string current;
    private JoyYouSDK _interface;

    public static string PARAMS = "params";

    public const string LANGUAGE = "language"; // 语言
    public const string en_us = "en_us"; // 英文
    public const string zh_cn = "zh_cn"; // 中文
    public const string mandarin = "mandarin "; // 普通话

    public const string ENGINE_TYPE = "engine_type"; // 引擎类型
    public const string TYPE_LOCAL = "local"; // 本地方式
    public const string TYPE_CLOUD = "cloud"; // 服务器方式
    public const string ACCENT = "accent"; // 方言

    public const string AUDIO_FORMAT = "audio_format"; // 流保存的格式，有pcm,wav两种
    public const string ASR_AUDIO_PATH = "asr_audio_path"; // 保存的路径
    public const string SPEECH_TIME_OUT = "speech_timeout"; //超时设置
    public const string VAD_BOS = "vad_bos";
    public const string VAD_EOS = "vad_eos"; //后端超时

    string filepath = "";
    string cachePath = "";
    bool inited = false;

    [SerializeField]
    string android_appid = "";
    [SerializeField]
    string ios_appid = "";

    public bool Deprecated
    {
        get;
        set;
    }

#if UNITY_IPHONE
	
	[DllImport("__Internal")]
    private static extern bool saveToGallery( string path );
	
#endif

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        InitIFly(android_appid);
#elif UNITY_IOS && !UNITY_EDITOR
        InitIFly(ios_appid);
#endif
        SetCallback(TextRecCallback);
        _interface = new JoyYouSDK();

#if !UNITY_EDITOR
        // 先初始化为网络
        SetParameter(ENGINE_TYPE, TYPE_CLOUD);
        SetParameter(AUDIO_FORMAT, "pcm");
        //SetParameter(SPEECH_TIME_OUT, "-1");
        SetParameter(VAD_BOS, "8000");
        SetParameter(VAD_EOS, "5000");

        string audioPath = GetAudioCachePath();

        if (!string.IsNullOrEmpty(audioPath))
        {
            filepath = audioPath + "/record.pcm";
		    Debug.Log("PCM filepath: " + filepath);
            SetParameter(ASR_AUDIO_PATH, filepath);
            inited = true;
        }

        //lame.Lame_InitEncoder(1, 16000, 16000, 0, 0, 0, 3.0f);
#else
        inited = true;
#endif
    }

    void InitIFly(string appid)
    {
        Microphone.IsRecording(null);
        //Debug.Log("Voice manager Init, appid: " + appid);
        //Voice.Init(appid);
    }

    public bool IsIFlyListening()
    {
        return isListening;
    }

    public bool IsInited()
    {
        return inited;
    }

    public bool IsRecordFileExist()
    {
        return File.Exists(filepath);
    }

    public bool isListening
    {
        get
        {
            return false;// Voice.isListening();
        }
    }

    public MonoBehaviour GetMonoBehavior()
    {
        return this;
    }

    public void SetParameter(string var1, string var2)
    {
        //SalfCall(() =>
        //{
        //    Voice.SetParameter(var1, var2);
        //});
    }

    void SalfCall(System.Action action)
    {
        try
        {
            action();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public int StartRecord()
    {
        Debug.Log("SpeechRecognizer:Start");
        int code = 0;
        //SalfCall(() =>
        //{
        //    code = Voice.Start();
        //});

        Debug.Log(string.Format("SpeechRecognizer:Start({0})", code));
        return code;
    }

    public void StopRecord()
    {
        Debug.Log("SpeechRecognizer:Stop");
        //SalfCall(() =>
        //{
        //    Voice.Stop();
        //});
    }

    public void Cancel()
    {
        Debug.Log("SpeechRecognizer:Cancel");
        //SalfCall(() =>
        //{
        //    Voice.Cancel();
        //});
    }

    void InitEnd(string code)
    {
        Debug.Log(string.Format("SpeechRecognizer:InitEnd:" + code));
    }

    void onBeginOfSpeech(string text)
    {
        Debug.Log("onBeginOfSpeech");
    }

    void onError(string text)
    {
        Debug.Log(string.Format("onError:{0}", text));
    }

    void onEndOfSpeech(string text)
    {
        Debug.Log("onEndOfSpeech");
    }

    void onVolumeChanged(string volume)
    {
        //Debug.Log("onVolumeChanged:" + volume);
        if (voicecallback != null)
            voicecallback(volume);
    }

    System.Action<string> callback = null;
    System.Action<string> voicecallback = null;

    public void SetCallback(System.Action<string> action)
    {
        callback = action;
    }

    public void SetVoiceCallback(System.Action<string> action)
    {
        voicecallback = action;
    }

    void onResult(string text)
    {
        if (callback != null)
        {
            callback(text);
        }
    }

    string GetAudioCachePath()
    {
        if (string.IsNullOrEmpty(cachePath))
        {
#if UNITY_IOS || UNITY_EDITOR
            cachePath = Application.temporaryCachePath;
#else
			cachePath = Application.persistentDataPath;
#endif

            if (string.IsNullOrEmpty(cachePath))
                return "";

            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);
        }

        return cachePath;
    }

    string GetString(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        return path.Replace('\\', '/');
    }

    public string StartTransMp3(string destFileName)
    {
        //string dstFilePath = filepath.Substring(0, filepath.LastIndexOf('.')) + destFileName + ".mp3";
        //Debug.Log ("srcFIlePath: " + filepath + ", dst file path:" + dstFilePath);
        //lame.Lame_EncodeFile(filepath, dstFilePath);
        //return dstFilePath;
        return "";
    }


    public AudioClip GetAudioClip(string filepath)
    {
#if UNITY_ANDROID || UNITY_IOS
        return null;
#else
        //return XBehaviorTree.GetAudioClip(filepath);
        return null;
#endif

        //StartCoroutine(LoadAudioClip(filepath, callback1, callback2));
    }

    void TextRecCallback(string res)
    {

    }

    public bool ScreenShotQQShare(string filepath, string type)
    {
        if (!File.Exists(filepath))
            return false;

        // Construct content json string
        Dictionary<string, string> jsonData = new Dictionary<string, string>();

        //JsonData jsonData = new JsonData();
        //jsonData["scene"] = "Session";
        //jsonData["scene"] = "QZone";
        jsonData["scene"] = type;
        jsonData["filePath"] = filepath;
        // jsonData["imageUrl"] = "http://f.hiphotos.baidu.com/baike/pic/item/5fdf8db1cb134954643deefc564e9258d1094a02.jpg";
        string paramStr = MiniJSON.Json.Serialize(jsonData);

        Debug.Log("SharePhotoWithQQ paramStr = " + paramStr);

        ((IHuanlePlatform)_interface).SendGameExtData("share_send_to_with_photo_qq", paramStr);
        return true;
    }
    public bool ScreenShotWeChatShare(string filepath, string type)
    {
        if (!File.Exists(filepath))
            return false;

        // Construct content json string
        Dictionary<string, string> jsonData = new Dictionary<string, string>();
        jsonData["scene"] = type;
        jsonData["filePath"] = filepath;
        jsonData["mediaTagName"] = "MSG_INVITE";
        jsonData["messageExt"] = "SharePhotoWithWeixin";
        jsonData["messageAction"] = "WECHAT_SNS_JUMP_APP";
        string paramStr = MiniJSON.Json.Serialize(jsonData);

        Debug.Log("SharePhotoWithWeixin paramStr = " + paramStr);

        ((IHuanlePlatform)_interface).SendGameExtData("share_send_to_with_photo_wx", paramStr);
        return true;
    }

    public bool ScreenShotSave(string filepath)
    {
#if UNITY_IPHONE
        return saveToGallery(filepath);
#endif
        return true;
    }

    public bool RefreshAndroidPhotoView(string androidpath)
    {
#if UNITY_ANDROID
        AndroidJavaClass obj = new AndroidJavaClass("com.ryanwebb.androidscreenshot.MainActivity");
        obj.CallStatic<bool>("scanMedia", androidpath);
#endif
        return true;
    }

    public bool OnOpenWebView()
    {
        Debug.Log("OnOpenWebView");
#if UNITY_IOS || UNITY_ANDROID
        UniWeb web = GetComponent<UniWeb>();

        if (web != null)
        {
            web.OpenWebView();
        }
#endif
        return true;
    }

    public void OnCloseWebView()
    {
#if UNITY_IOS || UNITY_ANDROID
        UniWeb web = GetComponent<UniWeb>();

        if (web != null)
        {
            web.CloseWebView(null);
        }
#endif
    }

    public void OnInitWebViewInfo(int platform, string openid, string serverid, string roleid, string nickname)
    {
#if UNITY_IOS || UNITY_ANDROID
        UniWeb web = GetComponent<UniWeb>();
        if (web != null)
            web.InitWebInfo(platform, openid, serverid, roleid, nickname);
#endif
    }

    public bool ShareWechatLink(string desc, string logopath, string url, bool issession)
    {
        // Construct content json string
        Dictionary<string, object> jsonData = new Dictionary<string, object>();
        if (issession)
            jsonData["scene"] = "Session";
        else
            jsonData["scene"] = "Timeline";
        jsonData["title"] = desc;
        jsonData["desc"] = desc;
        jsonData["url"] = url;
        jsonData["mediaTagName"] = "MSG_INVITE";
        jsonData["filePath"] = logopath;
        jsonData["messageExt"] = "ShareUrlWithWeixin";
        string paramStr = MiniJSON.Json.Serialize(jsonData);

        Debug.Log("ShareUrlWithWeixin paramStr = " + paramStr);

        ((IHuanlePlatform)_interface).SendGameExtData("share_send_to_with_url_wx", paramStr);
        return true;
    }

    public bool ShareWechatLinkWithMediaTag(string desc, string logopath, string url, bool issession, string media)
    {
        Dictionary<string, object> jsonData = new Dictionary<string, object>();
        if (issession)
            jsonData["scene"] = "Session";
        else
            jsonData["scene"] = "Timeline";
        jsonData["title"] = desc;
        jsonData["desc"] = desc;
        jsonData["url"] = url;
        jsonData["mediaTagName"] = media;
        jsonData["filePath"] = logopath;
        jsonData["messageExt"] = "ShareUrlWithWeixin";
        string paramStr = MiniJSON.Json.Serialize(jsonData);

        Debug.Log("ShareUrlWithWeixin paramStr = " + paramStr);

        ((IHuanlePlatform)_interface).SendGameExtData("share_send_to_with_url_wx", paramStr);
        return true;
    }


    public bool ShareQZoneLink(string title, string summary, string url, string logopath, bool issession)
    {
        // Construct content json string
        Dictionary<string, object> jsonData = new Dictionary<string, object>();
        if (issession)
            jsonData["scene"] = "Session";
        else
            jsonData["scene"] = "QZone";
        jsonData["title"] = title;
        jsonData["summary"] = summary;
        jsonData["targetUrl"] = url;
        jsonData["imageUrl"] = logopath;
        // jsonData["imageUrl"] = "http://f.hiphotos.baidu.com/baike/pic/item/5fdf8db1cb134954643deefc564e9258d1094a02.jpg";
        string paramStr = MiniJSON.Json.Serialize(jsonData);

        Debug.Log("ShareWithQQClient paramStr = " + paramStr);

        ((IHuanlePlatform)_interface).SendGameExtData("share_send_to_struct_qq", paramStr);
        return true;
    }


    public void OnEvalJsScript(string script)
    {
#if UNITY_IOS || UNITY_ANDROID
        UniWeb web = GetComponent<UniWeb>();

        if (web != null)
        {
            web.EvalJsScript(script);
        }
#endif
    }

    public void OnScreenLock(bool islock)
    {
#if UNITY_IOS || UNITY_ANDROID
        UniWeb web = GetComponent<UniWeb>();

        if (web != null)
        {
            Debug.Log("Will eval screen lock");
            if (islock)
                web.EvalJsScript("DNScreenLock()");
            else
                web.EvalJsScript("DNScreenUnlock()");
        }
#endif
    }

    public void RefershWebViewShow(bool show)
    {
#if UNITY_IOS || UNITY_ANDROID
  
        UniWeb web = GetComponent<UniWeb>();

        if (web != null)
        {
            Debug.Log("Will eval screen lock");
            web.OnShowWebView(show);
        }
#endif
    }
}
