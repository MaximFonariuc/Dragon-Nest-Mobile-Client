using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.SDK;
using XUtliPoolLib;
using System;
using System.IO;

/// <summary>
/// This is a demo script to show how to use UniWebView.
/// You can follow the step 1 to 10 and get started with the basic use of UniWebView.
/// </summary>
/// 


public class UniWeb : MonoBehaviour
{
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8

#if !DISABLE_JOYSDK
    private JoyYouSDK _interface;
#endif

    private string mAppId = "1105309683";
    private string mOpenid = "0";
    private string mServerid = "0";
    private string mRoleid = "0";
    private string mToken = "0";
    private string mNickName = "";
    public static IUiUtility m_uiUtility;
    private int _gap = 90;
    ulong mOpenTime = 0;
    string mWebViewUrl = "https://apps.game.qq.com/gts/gtp3.0/customize/dn/end.php";

    //Just let it compile on platforms beside of iOS and Android
    //If you are just targeting for iOS and Android, you can ignore this

    void Awake()
    {
#if !DISABLE_JOYSDK
        _interface = new JoyYouSDK();
#endif

        m_uiUtility = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
    }

    public void InitWebInfo(int platform, string openid, string serverid, string roleid, string nickname)
    {
        if (m_uiUtility == null)
            m_uiUtility = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));

        if (platform == 0)
            mAppId = "1105309683";
        else
            mAppId = "wxfdab5af74990787a";

        mOpenid = openid;
        mServerid = serverid;
        mRoleid = roleid;
        mNickName = nickname;

#if !DISABLE_JOYSDK
        string sdkconfig = ((IHuanlePlatform)_interface).GetSDKConfig("get_login_bill", "");

#if UNITY_EDITOR
        Dictionary<string, string> testinfo = new Dictionary<string, string>();
        testinfo["token"] = "6859034BCE654365632252";
        testinfo["openid"] = "BAFE459486548AC698568956DEF";
        sdkconfig = MiniJSON.Json.Serialize(testinfo);
#endif
        object obj = MiniJSON.Json.Deserialize(sdkconfig);
        Dictionary<string, object> info =  obj as Dictionary<string, object>;

        if (info != null)
        {
            if (info.ContainsKey("token"))
                mToken = info["token"] as string ;

            if (info.ContainsKey("openid"))
                mOpenid = info["openid"] as string;
        }

        Debug.Log("The openid: " + mOpenid);
        Debug.Log("The token: " + mToken);
#endif
    }
#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR

	//1. First of all, we need a reference to hold an instance of UniWebView
	private UniWebView _webView;

	private string _errorMessage;
	private GameObject _cube;
	private Vector3 _moveVector;
    private bool _is_bgopen = true;

	void Start() 
	{

    }

    public void OpenWebView()
    {
        Debug.Log("Will do open web view");

        _gap = GetGap();

        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        mOpenTime = (ulong)(DateTime.Now - startTime).TotalMilliseconds;

        _webView = GetComponent<UniWebView>();
        if (_webView == null)
        {
            _webView = gameObject.AddComponent<UniWebView>();
            _webView.OnReceivedMessage += OnReceivedMessage;
            _webView.OnLoadComplete += OnLoadComplete;
            _webView.OnWebViewShouldClose += OnWebViewShouldClose;
            _webView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;

            _webView.InsetsForScreenOreitation += InsetsForScreenOreitation;
        }

        string url = string.Format("{0}?appid={1}&openid={2}&access_token={3}&partition={4}&roleid={5}&entertime={6}&nickname={7}", 
            mWebViewUrl, mAppId, mOpenid, mToken, mServerid, mRoleid, mOpenTime, WWW.EscapeURL(mNickName));

        Debug.Log("url: " + url);

        _webView.url = url; // WWW.EscapeURL(url);

        Debug.Log("Final url: " + _webView.url);

        Debug.Log("Width: " + Screen.width.ToString());
        Debug.Log("Height: " + Screen.height.ToString());
        //Debug.Log("Webview height: " + _webView.GetWebViewHeight().ToString());

        _webView.Load();
    }

    public void CloseWebView(UniWebView webView)
    {
        if (webView == null)
            webView = _webView;

        if (webView != null)
        {
            webView.CleanCache();
            webView.CleanCookie();
            webView.Hide();
            Destroy(webView);
            webView.OnReceivedMessage -= OnReceivedMessage;
            webView.OnLoadComplete -= OnLoadComplete;
            webView.OnWebViewShouldClose -= OnWebViewShouldClose;
            webView.OnEvalJavaScriptFinished -= OnEvalJavaScriptFinished;
            webView.InsetsForScreenOreitation -= InsetsForScreenOreitation;
        }

        _webView = null;

        _is_bgopen = true;

        if (m_uiUtility == null)
            m_uiUtility = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
        m_uiUtility.OnSetBg(true);
        m_uiUtility.OnWebViewClose();
    }
#if UNITY_EDITOR
    void Update() {

        if (Input.GetKeyUp(KeyCode.F10))
        {
            string str = "uniwebview://webview?funcname=dnsetshareinfo&args=%7B%22title%22%3A%22%E5%88%86%E4%BA%AB%E6%B5%8B%E8%AF%95%E6%A0%87%E9%A2%98%22%2C%22desc%22%3A%22%E5%88%86%E4%BA%AB%E6%B5%8B%E8%AF%95%E6%8F%8F%E8%BF%B0%E6%8F%8F%E8%BF%B0%E6%8F%8F%E8%BF%B0%E3%80%82%E3%80%82%E3%80%82%E3%80%82%22%2C%22url%22%3A%22%2F%2Fiu.qq.com%2Fdn%2Fdist%2Fhtml%2Ftest.htm%22%2C%22imgurl%22%3A%22%2F%2Fgame.gtimg.cn%2Fimages%2Fiu%2Fdn%2Fb1.png%22%2C%22type%22%3A%22weixin%22%7D";
            if (_webView == null) _webView = gameObject.AddComponent<UniWebView>();
            UniWebViewMessage message = new UniWebViewMessage(str);
            OnReceivedMessage(_webView, message);
        }
	}
#endif
    //5. When the webView complete loading the url sucessfully, you can show it.
    //   You can also set the autoShowWhenLoadComplete of UniWebView to show it automatically when it loads finished.
    void OnLoadComplete(UniWebView webView, bool success, string errorMessage) {
		if (success) {
			webView.Show();
		} else {
			Debug.Log("Something wrong in webview loading: " + errorMessage);
			_errorMessage = errorMessage;
		}
	}

	//6. The webview can talk to Unity by a url with scheme of "uniwebview". See the webpage for more
	//   Every time a url with this scheme clicked, OnReceivedMessage of webview event get raised.
	void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
    {
        Debug.Log("OnReceivedMessage");
		Debug.Log(message.rawMessage);
		//7. You can get the information out from the url path and query in the UniWebViewMessage
		//For example, a url of "uniwebview://move?direction=up&distance=1" in the web page will 
		//be parsed to a UniWebViewMessage object with:
		//				message.scheme => "uniwebview"
		//              message.path => "move"
		//              message.args["direction"] => "up"
		//              message.args["distance"] => "1"
		// "uniwebview" scheme is sending message to Unity by default.
		// If you want to use your customized url schemes and make them sending message to UniWebView,
		// use webView.AddUrlScheme("your_scheme") and webView.RemoveUrlScheme("your_scheme")
		if (string.Equals(message.path, "webview"))
        {
			Vector3 direction = Vector3.zero;


            if (message.args.ContainsKey("funcname"))
            {
                if (string.Equals(message.args["funcname"], "dnqueryuserinfo"))
                {
                    if (message.args.ContainsKey("callback"))
                    {

                        Debug.Log("Will callback");
                        string callbackname = message.args["callback"];

                        Dictionary<string, object> jsonData = new Dictionary<string, object>();
                        jsonData["appid"] = mAppId;
                        jsonData["openid"] = mOpenid;
                        jsonData["access_token"] = mToken;
                        jsonData["partition"] = mServerid;
                        jsonData["roleid"] = mRoleid;
                        jsonData["entertime"] = mOpenTime;
                        jsonData["nickname"] = mNickName;
                        string paramStr = MiniJSON.Json.Serialize(jsonData);

                        //paramStr = "100539858";

                        string jsscript = string.Format("{0}({1})", callbackname, paramStr);

                        Debug.Log(jsscript);

                        webView.EvaluatingJavaScript(jsscript);
                    }
                }
                else if (string.Equals(message.args["funcname"], "dnclosewebview"))
                {
                    CloseWebView(webView);

                }
                else if (string.Equals(message.args["funcname"], "dniswifi"))
                {
                    if (message.args.ContainsKey("callback"))
                    {

                        Debug.Log("Will dniswifi callback");
                        string callbackname = message.args["callback"];
                        bool iswifi = (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
                        int res = iswifi ? 1 : 0;

                        string jsscript = string.Format("{0}({1})", callbackname, res);

                        Debug.Log(jsscript);

                        webView.EvaluatingJavaScript(jsscript);
                    }
                }
                else if (string.Equals(message.args["funcname"], "dnisbgopen"))
                {
                    if (message.args.ContainsKey("callback"))
                    {

                        Debug.Log("Will dnisbgopen callback");
                        string callbackname = message.args["callback"];
                        int res = _is_bgopen ? 1 : 0;

                        string jsscript = string.Format("{0}({1})", callbackname, res);

                        Debug.Log(jsscript);

                        webView.EvaluatingJavaScript(jsscript);
                    }
                }
                else if (string.Equals(message.args["funcname"], "dnopenbg"))
                {
                    _is_bgopen = true;
                    m_uiUtility.OnSetBg(true);
                }
                else if (string.Equals(message.args["funcname"], "dnclosebg"))
                {
                    _is_bgopen = false;
                    m_uiUtility.OnSetBg(false);
                }
                else if (string.Equals(message.args["funcname"], "dnchangemenu"))
                {
                    if (message.args.ContainsKey("menutype"))
                    {
                        // 0: main menu  1; detail
                        int menutype = int.Parse(message.args["menutype"]);
                        m_uiUtility.OnSetWebViewMenu(menutype);
                    }
                }
                else if (string.Equals(message.args["funcname"], "dnbackgame"))
                {
                    if (message.args.ContainsKey("backtype"))
                    {
                        int backtype = int.Parse(message.args["backtype"]);
                        m_uiUtility.OnSetWebViewMenu(backtype);
                    }

                    if (message.args.ContainsKey("callback"))
                    {
                        Debug.Log("Will dnbackgame callback");
                        string callbackname = message.args["callback"];
                        string jsscript = string.Format("{0}()", callbackname);
                        Debug.Log(jsscript);

                        webView.EvaluatingJavaScript(jsscript);
                    }
                }
                else if (string.Equals(message.args["funcname"], "dnrefreshredpoint"))
                {
                    if (message.args.ContainsKey("args"))
                    {
                        string redpointinfo = WWW.UnEscapeURL(message.args["args"]);
                        Debug.Log("dnrefreshredpoint" + redpointinfo);
                        m_uiUtility.OnWebViewRefershRefPoint(redpointinfo);
                    }
                }
                else if (string.Equals(message.args["funcname"], "dnsetheaderinfo"))
                {
                    if (message.args.ContainsKey("args"))
                    {
                        string headerinfo = WWW.UnEscapeURL(message.args["args"]);
                        Debug.Log("dnsetheaderinfo" + headerinfo);
                        m_uiUtility.OnWebViewSetheaderInfo(headerinfo);
                    }
                }
                else if (string.Equals(message.args["funcname"], "dnshownav"))
                {
                    if (message.args.ContainsKey("type"))
                    {
                        int show = int.Parse(message.args["type"]);

                        if (show == 1)
                            _gap = GetGap();
                        else
                            _gap = 0;
                        _webView.ForceResize();
                        
                    }
                }
                else if (string.Equals(message.args["funcname"], "dncloseloading"))
                {
                    if (message.args.ContainsKey("show"))
                    {
                        int show = int.Parse(message.args["show"]);
                        Debug.Log("dncloseloading: " + message.args["show"]);

                        m_uiUtility.OnWebViewCloseLoading(show);

                        if (show == 1)
                            _webView.Hide();
                        else
                            _webView.Show();
                    }
                }
                else if (string.Equals(message.args["funcname"], "dnshowreconnect"))
                {
                    if (message.args.ContainsKey("show"))
                    {
                        int show = int.Parse(message.args["show"]);
                        Debug.Log("dnshowreconnect: " + message.args["show"]);

                        m_uiUtility.OnWebViewShowReconnect(show);

                        if (show == 1)
                            _webView.Hide();
                        else
                            _webView.Show();
                    }
                }
                else if (string.Equals(message.args["funcname"], "dnsetlivetab"))
                {
                    m_uiUtility.OnWebViewLiveTab();
                }
                else if (string.Equals(message.args["funcname"], "dnbackhistory"))
                {
                    _webView.GoBack();
                }
                else if (string.Equals(message.args["funcname"], "dnsetshareinfo"))
                {
                    string info = WWW.UnEscapeURL(message.args["args"]);
                    //Debug.Log("dnsetshareinfo" + info);
                    Dictionary<string, object> dic = MiniJSON.Json.Deserialize(info) as Dictionary<string, object>;
                    XPlatform platf = gameObject.GetComponent<XPlatform>();
                    object title, imgUrl, desc, url, type;
                    dic.TryGetValue("title", out title);
                    dic.TryGetValue("imgurl", out imgUrl);
                    dic.TryGetValue("desc", out desc);
                    dic.TryGetValue("url", out url);
                    dic.TryGetValue("type", out type);
                    //Debug.Log("title: " + title + " url: " + url + " desc: " + desc + " img: " + imgUrl + " type: " + type);
                    if (type.Equals("qq") || type.Equals("qzone"))
                    {
                        Dictionary<string, string> jsondata = new Dictionary<string, string>();
                        jsondata["scene"] = type.Equals("qq") ? "Session" : "QZone";
                        if (url != null) jsondata["targetUrl"] = "https:"+url.ToString();
                        if (imgUrl != null) jsondata["imageUrl"] = imgUrl.ToString();
                        if (title != null) jsondata["title"] = title.ToString();
                        if (desc != null) jsondata["description"] = desc.ToString();
                        jsondata["summary"] = "";
                        string json = MiniJSON.Json.Serialize(jsondata);
                        platf.SendGameExData("share_send_to_struct_qq", json);
                    }
                    else if (type.Equals("weixin") || type.Equals("timeline"))
                    {
                        if (!gameObject.activeSelf) return;
                        StartCoroutine(DownloadPic(imgUrl.ToString(), (filepath) =>
                            {
                                //Debug.Log("cb: "+filepath);
                                if (!string.IsNullOrEmpty(filepath))
                                {
                                    Dictionary<string, string> jsondata = new Dictionary<string, string>();
                                    jsondata["scene"] = type.Equals("weixin") ? "Session" : "Timeline";
                                    if (title != null) jsondata["title"] = title.ToString();
                                    if (desc != null) jsondata["desc"] = desc.ToString();
                                    if (url != null) jsondata["url"] = url.ToString();
                                    jsondata["mediaTagName"] = "MSG_INVITE";
                                    jsondata["filePath"] = filepath;
                                    jsondata["messageExt"] = "ShareUrlWithWeixin";
                                    string json = MiniJSON.Json.Serialize(jsondata);
                                    platf.SendGameExData("share_send_to_with_url_wx", json);
                                }
                            }));
                    }
                    else
                    {
                        Debug.LogError("err type: " + type);
                    }
                }
            }
		}
  //      else if (string.Equals(message.path, "close")) {
		//	//8. When you done your work with the webview, 
		//	//you can hide it, destory it and do some clean work.
		//	webView.Hide();
		//	Destroy(webView);
		//	webView.OnReceivedMessage -= OnReceivedMessage;
		//	webView.OnLoadComplete -= OnLoadComplete;
		//	webView.OnWebViewShouldClose -= OnWebViewShouldClose;
		//	webView.OnEvalJavaScriptFinished -= OnEvalJavaScriptFinished;
		//	webView.InsetsForScreenOreitation -= InsetsForScreenOreitation;
		//	_webView = null;
		//}
	}


    IEnumerator DownloadPic(string path, Action<string> cb)
    {
        int hash = Mathf.Abs( path.GetHashCode());
        string dir = Application.temporaryCachePath + "/ImageCache/";
        try
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }
        catch { }
        string pa = dir + hash;
        if (File.Exists(pa))
        {
            cb(pa);
        }
        else
        {
            string url =  path;
            //print(url+" pa: "+pa);
            WWW w = new WWW(url);
            while (!w.isDone)
                yield return w;
            if (!string.IsNullOrEmpty(w.error))
            {
                Debug.LogError("error:"+w.error);
                cb(string.Empty);
            }
            else
            {
                try
                {
                    Texture2D image = w.texture;
                    byte[] pngData = image.EncodeToJPG();
                    File.WriteAllBytes(pa, pngData);
                    cb(pa);
                }
                catch(Exception e) { cb(string.Empty); Debug.LogError("wraite error"+e.StackTrace); }
            }
            w.Dispose();
            w = null;
        }
    }

	//9. By using EvaluatingJavaScript method, you can talk to webview from Unity.
	//It can evel a javascript or run a js method in the web page.
	//(In the demo, it will be called when the cube hits the sphere)
	public void ShowAlertInWebview(float time, bool first) {
		_moveVector = Vector3.zero;
		if (first) {
			//Eval the js and wait for the OnEvalJavaScriptFinished event to be raised.
			//The sample(float time) is written in the js in webpage, in which we pop 
			//up an alert and return a demo string.
			//When the js excute finished, OnEvalJavaScriptFinished will be raised.
			_webView.EvaluatingJavaScript("sample(" + time +")");
		}
	}

	//In this demo, we set the text to the return value from js.
	void OnEvalJavaScriptFinished(UniWebView webView, string result) {
		Debug.Log("js result: " + result);
	}

	//10. If the user close the webview by tap back button (Android) or toolbar Done button (iOS), 
	//    we should set your reference to null to release it. 
	//    Then we can return true here to tell the webview to dismiss.
	bool OnWebViewShouldClose(UniWebView webView) {
		if (webView == _webView) {
			_webView = null;
			return true;
		}
		return false;
	}

    public void EvalJsScript(string jsscript)
    {
        Debug.Log(jsscript);

        if (jsscript.Contains("DNBackClick"))
        {
            if (_webView != null)
            {
                try
                {
                    _webView.EvaluatingJavaScript(jsscript);
                }
                catch
                {
                    CloseWebView(_webView);
                }
            }
        }
        else
        {
            if (_webView != null)
                _webView.EvaluatingJavaScript(jsscript);
        }

    }

    public void OnShowWebView(bool show)
    {
        if (_webView != null)
        {
            if (show)
                _webView.Show();
            else
                _webView.Hide();
        }
    }

    private int GetGap()
    {
#if UNITY_IOS
        if (Screen.width == 1920)
	        return 35 * Screen.width / 1920 + (Screen.height - (Screen.width * 1080 / 1920))/4;
		else
			return (int)(35*1.3 * Screen.width / 1920) + (Screen.height - (Screen.width * 1080 / 1920))/4;
#else
        return 90 * Screen.width / 1920 + (Screen.height - (Screen.width * 1080 / 1920))/2;
#endif
    }

	// This method will be called when the screen orientation changed. Here we returned UniWebViewEdgeInsets(5,5,bottomInset,5)
	// for both situation. Although they seem to be the same, screenHeight was changed, leading a difference between the result.
	// eg. on iPhone 5, bottomInset is 284 (568 * 0.5) in portrait mode while it is 160 (320 * 0.5) in landscape.
	UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation) {
        //int bottomInset = (int)(UniWebViewHelper.screenHeight * 0.5f);
        //int bottomInset = (int)(UniWebViewHelper.screenHeight);
        //int rightInset = (int)(UniWebViewHelper.screenWidth);

        Debug.Log("Gap: " + _gap.ToString());

        return new UniWebViewEdgeInsets(_gap, 0, 0, 0);
	}
#endif //End of #if UNITY_IOS || UNITY_ANDROID


#endif
}
