using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace com.tencent.pandora
{
    /// <summary>
    /// 页面成功加载回调
    /// </summary>
	public delegate void PageLoadedCallback();
    /// <summary>
    /// 页面消息回调
    /// </summary>
    /// <param name="message"></param>
    public delegate void PageMessageCallback(string message);
    /// <summary>
    /// 页面回退回调
    /// </summary>
    /// <param name="isDown"></param>
    public delegate void PageBackCallback(string isDown);

    public class PandoraWebView : MonoBehaviour
    {
#if UNITY_EDITOR
#else
#if UNITY_ANDROID
		AndroidJavaClass webViewBridge = new AndroidJavaClass("com.tencent.pandora.webview.unity.WebViewUnityBridge");
		const string FuncInit = "initialize";
		const string FuncShowUrl = "showUrl";
		const string FuncClose = "close";
		const string FuncSetGameObjectName = "setGameObjectName";
		const string FuncIsShow = "isShow";
		const string FuncGoBack = "goBack";
		const string FuncCanGoBack = "canGoBack";
		const string FuncWriteMessage = "writeMessage";
		const string FuncSetInfoString = "setInfo";
		const string FuncUninitialize = "uninitiated";
		const string FuncGetScreenWidth = "getScreenWidth";
		const string FuncGetScreenHeight = "getScreenHeight";
		const string FuncCanUseWebView = "canUseWebView";
		const string FuncShow = "show";
		const string FuncHide = "hide";
		const string FuncVerbose = "setVerbose";
		const string FunSetUseActivity = "setUseActivity";
#elif UNITY_IOS
		[DllImport ("__Internal")]
		private static extern void PANDORA_WEBVIEW_OpenUrl(string url, int width, int height, int left, int top, string color, bool delayShow);
		
		[DllImport ("__Internal")]
		private static extern void PANDORA_WEBVIEW_Close();
		
		[DllImport ("__Internal")]
		private static extern bool PANDORA_WEBVIEW_IsShow();
		
		[DllImport ("__Internal")]
		private static extern void PANDORA_WEBVIEW_ObjName(string objName);
		
		[DllImport ("__Internal")]
		private static extern void PANDORA_WEBVIEW_SetTicket(string ticket);

		[DllImport ("__Internal")]
		private static extern void PANDORA_WEBVIEW_WriteMessage(string message);

		[DllImport ("__Internal")]
		private static extern void PANDORA_WEBVIEW_OnDestroy();

		[DllImport ("__Internal")]
		private static extern int PANDORA_WEBVIEW_GetPhysicalScreenWidth();

		[DllImport ("__Internal")]
		private static extern int PANDORA_WEBVIEW_GetPhysicalScreenHeight();

		[DllImport ("__Internal")]
		private static extern void PANDORA_WEBVIEW_IsShowWebviewScrollIndicator(bool Horizontal, bool Vertical);

		[DllImport ("__Internal")]
		private static extern void PANDORA_WEBVIEW_SetVerbose(bool verbose);
#endif
#endif

        PageLoadedCallback _pageLoadedCallback = null;
        PageMessageCallback _pageMessageCallback = null;
        PageBackCallback _pageBackCallback = null;

        bool _hasOpenUrl = false;

        //安卓webview是否在新开的进程中开启页面，默认是false
        private bool _openInOtherProcess = false;

        public void SetPageLoadedCallback(PageLoadedCallback callback)
        {
            _pageLoadedCallback = callback;
        }

        public void SetPageMessageCallback(PageMessageCallback callback)
        {
            _pageMessageCallback = callback;
        }

        public void SetPageBackCallback(PageBackCallback callback)
        {
            _pageBackCallback = callback;
        }

        public void Init(bool openInOtherProcess = false)
        {
            _openInOtherProcess = openInOtherProcess;

            bool verbose = false;
            if(PandoraSettings.IsProductEnvironment == false)
            {
                verbose = true; 
            }
            string info = "\"PandoraWebView\"";
            try
            {
#if UNITY_EDITOR
#else
#if UNITY_ANDROID
                SetUseActivity(_openInOtherProcess);//new method
                if (webViewBridge != null)
                {
                    Logger.LogInfo("Android webview MutilProcess status: " + _openInOtherProcess);
                    webViewBridge.CallStatic(FuncVerbose, verbose);
                    webViewBridge.CallStatic(FuncInit, _openInOtherProcess);//use Multi process 
                    webViewBridge.CallStatic(FuncSetGameObjectName, name);
                    webViewBridge.CallStatic(FuncSetInfoString, info);
                }
#elif UNITY_IOS
			    PANDORA_WEBVIEW_ObjName(name);
                PANDORA_WEBVIEW_SetVerbose(verbose);
                PANDORA_WEBVIEW_SetTicket(info);
#endif
#endif
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + ":" + e.StackTrace);
            }
        }

        /// <summary>
        /// Andorid环境下开启新Activity（新Process）打开WebView
        /// </summary>
        /// <param name="useActivity"></param>
        public void SetUseActivity(bool useActivity)
        {
#if UNITY_EDITOR || UNITY_IOS
#else
#if UNITY_ANDROID
            if (webViewBridge != null)
            {
                webViewBridge.CallStatic(FunSetUseActivity, useActivity);
            }
#endif
#endif
        }

        public bool CanUseWebView
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
#if UNITY_ANDROID
                if (webViewBridge != null)
                {
                    return webViewBridge.CallStatic<bool>(FuncCanUseWebView);
                }
                return false;
#else
                return true;
#endif
#endif
            }
        }

        public int ScreenWidth
        {
            get
            {
#if UNITY_EDITOR
#else
#if UNITY_ANDROID
                if (webViewBridge != null) return webViewBridge.CallStatic<int>(FuncGetScreenWidth);
#elif UNITY_IOS
                return PANDORA_WEBVIEW_GetPhysicalScreenWidth();
#endif
#endif
                return Screen.width;
            }
        }

        public int ScreenHeight
        {
            get
            {
#if UNITY_EDITOR
#else
#if UNITY_ANDROID
                if (webViewBridge != null) return webViewBridge.CallStatic<int>(FuncGetScreenHeight);
#elif UNITY_IOS
                return PANDORA_WEBVIEW_GetPhysicalScreenHeight();
#endif
#endif
                return Screen.height;
            }
        }

        public void OpenUrl(string url, bool openInOtherProcess, string color = "#ffffffff", bool delayShow = false)
        {
            OpenUrl(url, Screen.width, Screen.height, openInOtherProcess, 0, 0, color, delayShow);
        }

        public void OpenUrl(string url, int width, int height, bool openInOtherProgress, int centerOffsetX = 0, int centerOffsetY = 0, string color = "#ffffffff", bool delayShow = false)
        {
            //做一个单进程和多进程的标志位
            if (_openInOtherProcess != openInOtherProgress)
            {
                Destroy();
                Init(openInOtherProgress);
                _openInOtherProcess = openInOtherProgress;
            }
            Logger.LogInfo("Open url: " + url);

            int offsetX = Screen.width / 2 - (width / 2 - centerOffsetX);
            int offsetY = Screen.height / 2 - (height / 2 - centerOffsetY);
            offsetY = Screen.height - offsetY - height;

            int nativeWidth = this.ScreenWidth;
            int nativeHeight = this.ScreenHeight;

            if (nativeWidth <= 0 || nativeHeight <= 0)
            {
                Logger.LogWarning("Get native width or height error, nativeWidth: " + nativeWidth + " nativeHeight: " + nativeHeight);
            }
            else
            {
                float widthRate = (float)nativeWidth / Screen.width;
                float heightRate = (float)nativeHeight / Screen.height;
                Logger.Log("nativeWidth: " + nativeWidth + " nativeHeight: " + nativeHeight + " widthRate: " + widthRate + " heightRate: " + heightRate);
                width = (int)(width * widthRate);
                height = (int)(height * heightRate);
                offsetX = (int)(offsetX * widthRate);
                offsetY = (int)(offsetY * heightRate);
            }

            try
            {
#if UNITY_EDITOR
                Application.OpenURL(url);
#else
#if UNITY_ANDROID
                if (webViewBridge != null)
                {
                    webViewBridge.CallStatic(FuncShowUrl, width, height, offsetX, offsetY, url, color, delayShow);
                }
#elif UNITY_IOS
                PANDORA_WEBVIEW_OpenUrl(url, width, height, offsetX, offsetY, color, delayShow);
#endif
#endif
                _hasOpenUrl = true;
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + ":" + e.StackTrace);
            }
        }

        public void Close()
        {
            Logger.Log("Close Webview");
            try
            {

#if UNITY_EDITOR
#else
#if UNITY_ANDROID
                if (webViewBridge != null)
                {
                    webViewBridge.CallStatic(FuncClose);
                }
#elif UNITY_IOS
                PANDORA_WEBVIEW_Close();
#endif
#endif
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + ":" + e.StackTrace);
            }

            _hasOpenUrl = false;
        }

        public bool IsShow
        {
            get
            {
                try
                {
#if UNITY_EDITOR
#else
#if UNITY_ANDROID
                if (webViewBridge != null)
                {
                    return webViewBridge.CallStatic<bool>(FuncIsShow);
                }
#elif UNITY_IOS
                return PANDORA_WEBVIEW_IsShow();
#endif
#endif
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message + ":" + e.StackTrace);
                }
                return false;
            }
        }

        public void WriteMessage(string message)
        {
            if (_hasOpenUrl == false)
            {
                return;
            }

            try
            {

#if UNITY_EDITOR
#else
#if UNITY_ANDROID
                if (webViewBridge != null)
                {
                    webViewBridge.CallStatic(FuncWriteMessage, message);
                }
#elif UNITY_IOS
                PANDORA_WEBVIEW_WriteMessage(message);
#endif
#endif
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + ":" + e.StackTrace);
            }
        }

        public void Show()
        {
            Logger.Log("Page Show");
#if UNITY_EDITOR
#else
#if UNITY_ANDROID
            if (webViewBridge != null)
            {
                webViewBridge.CallStatic(FuncShow);
            }
#elif UNITY_IOS
#else
#endif
#endif
        }

        public void Hide()
        {
            Logger.Log("Path Hide");
#if UNITY_EDITOR
#else
#if UNITY_ANDROID
            if (webViewBridge != null)
            {
                webViewBridge.CallStatic(FuncHide);
            }
#elif UNITY_IOS
#else
#endif
#endif
        }

        public void Destroy()
        {
            try
            {
#if UNITY_EDITOR
#else
#if UNITY_ANDROID
                if (webViewBridge != null)
                {
                    webViewBridge.CallStatic(FuncUninitialize);
                }
#elif UNITY_IOS
                PANDORA_WEBVIEW_OnDestroy();
#endif
#endif
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + ":" + e.StackTrace);
            }
        }

        //OnPageLoaded/Plugins/iOS/PandoraWebViewController.m调用
        public void OnPageLoaded()
        {
            Logger.Log("PageLoaded");
            if (_pageLoadedCallback != null)
            {
                _pageLoadedCallback();
            }
        }

        //OnPageMessage由Assets/Plugins/iOS/PandoraWebViewController.m调用
        public void OnPageMessage(string message)
        {
            Logger.Log("Page message: " + message);
            if (_pageMessageCallback != null)
            {
                _pageMessageCallback(message);
            }
        }

        //Andorid专属
        public void OnBackPress(string isDown)
        {
            Logger.Log("Page Back Pressed");
            if (_pageBackCallback != null)
            {
                _pageBackCallback(isDown);
            }
        }

    }
}
