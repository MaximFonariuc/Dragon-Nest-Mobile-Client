#if !DISABLE_PLUGIN

#if UNITY_ANDROID && !UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading;

public class QGameKitAndroidBridge : MonoBehaviour {
	private static string gameID;
    private static string wnsID;
	private static QGameKit.CaptureType myCaptureType;
	private int grabFrameRate = 20;
    private static QGameKit.Environment sdkEnvironmentType;
    private static QGameKitAndroidBridge singletonInstance = null;

	private static AndroidJavaObject playerActivityContext = null;
	private static IntPtr constructorMethodID = IntPtr.Zero;
	private static IntPtr setupMethodID = IntPtr.Zero;
	private static IntPtr tearDownMethodID = IntPtr.Zero;
	private static IntPtr resetMethodID = IntPtr.Zero;
	private static IntPtr startLiveBroadcastMethodID = IntPtr.Zero;
	private static IntPtr frameUpdatedMethodID = IntPtr.Zero;
	private static IntPtr stopLiveBroadcastMethodID = IntPtr.Zero;
	private static IntPtr enterLiveHallMethodID = IntPtr.Zero;
    private static IntPtr enterLiveHallInGameMethodID = IntPtr.Zero;
	private static IntPtr updateUserAccountMethodID = IntPtr.Zero;
	private static IntPtr getLiveBroadcastStatusMethodID = IntPtr.Zero;
	private static IntPtr getErrorCodeMethodID = IntPtr.Zero;
    private static IntPtr isLiveBroadcastSupportedMethodID = IntPtr.Zero;
    private static IntPtr getVersionNameMethodID = IntPtr.Zero;
    private static IntPtr showCameraMethodID = IntPtr.Zero;
    private static IntPtr hideCameraMethodID = IntPtr.Zero;
    private static IntPtr setEnvironmentTypeMethodID = IntPtr.Zero;
    private static IntPtr getLiveFrameRateMethodID = IntPtr.Zero;
    private static IntPtr doOnCreateMethodID = IntPtr.Zero;
    private static IntPtr doOnResumeMethodID = IntPtr.Zero;
    private static IntPtr doOnPauseMethodID = IntPtr.Zero;
    private static IntPtr doOnDestroyMethodID = IntPtr.Zero;
    private static IntPtr doOnBackPressedMethodID = IntPtr.Zero;
	private static IntPtr notifyUserAccountMethodID = IntPtr.Zero;
    private static IntPtr setUserAccountListenerMethodID = IntPtr.Zero;
    private static IntPtr setCommentReceiveDelegateMethodID = IntPtr.Zero;
    private static IntPtr setLogDelegateMethodID = IntPtr.Zero;
    private static IntPtr setLiveStatusChangedDelegateMethodID = IntPtr.Zero;
    private static IntPtr setShareListenerMethodID = IntPtr.Zero;
    private static IntPtr setErrorCodeDelegateMethodID = IntPtr.Zero;
    private static IntPtr setWebViewStatusChangedDelegateMethodID = IntPtr.Zero;
    private static IntPtr isSupportCameraMethodID = IntPtr.Zero;
	private static IntPtr shareLiveBroadcastMethodID = IntPtr.Zero;
    private static IntPtr isSupportLiveHallMethodID = IntPtr.Zero;
	private static IntPtr setDanmakuEnabledMethodID = IntPtr.Zero;
	private static IntPtr showDanmakuMethodID = IntPtr.Zero;
	private static IntPtr hideDanmakuMethodID = IntPtr.Zero;

    private IntPtr androidBridge = IntPtr.Zero;
	private float startTime = 0.0f;
	private float nextCaptureTime = 0.0f;
	private float frameInterval = 0.0f;

	public bool isRunning { get; private set; }

    

    private static QGameKit.UserAccountDelegate userAccountDelegate;


    

    /**
      初始化入口
      这里自动创建gameobj 并挂载组件 必须调用且仅能调用一次
    */
	public static QGameKitAndroidBridge Setup(string gameId, string wnsId, QGameKit.CaptureType captureType, QGameKit.UserAccountDelegate accountDelegate, QGameKit.Environment environmentType)
    {
        if(singletonInstance != null)
        {
            return singletonInstance;
        }

        gameID = gameId;
        wnsID = wnsId;
		myCaptureType = captureType;
        userAccountDelegate = accountDelegate;
        sdkEnvironmentType = environmentType;
        GameObject sdkObject = new GameObject("QGameKitAndroidBridge");

        DontDestroyOnLoad(sdkObject);

        singletonInstance = sdkObject.AddComponent<QGameKitAndroidBridge>();

        singletonInstance.initSDK();

        return singletonInstance;
    }

    private void initSDK()
    {
        // Search for our class
        IntPtr classID = AndroidJNI.FindClass("com/tencent/qgame/livesdk/bridge/Unity3D");

        // Search for it's contructor
        constructorMethodID = AndroidJNI.GetMethodID(classID, "<init>", "(Landroid/content/Context;Ljava/lang/String;Ljava/lang/String;IIII)V");
        if (constructorMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find UnityBridge constructor.");
            return;
        }

        setupMethodID = AndroidJNI.GetMethodID(classID, "setup", "(Lcom/tencent/qgame/livesdk/bridge/UserAccountListener;)Z");
        if (setupMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find setup() method.");
            return;
        }

        tearDownMethodID = AndroidJNI.GetMethodID(classID, "tearDown", "()V");
        if (tearDownMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find tearDown() method.");
            return;
        }

        resetMethodID = AndroidJNI.GetMethodID(classID, "reset", "()Z");
        if (resetMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find reset() method.");
            return;
        }

        startLiveBroadcastMethodID = AndroidJNI.GetMethodID(classID, "startLiveBroadcast", "(Ljava/lang/String;Ljava/lang/String;)Z");
        if (startLiveBroadcastMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find startLiveBroadcast() method.");
            return;
        }

        stopLiveBroadcastMethodID = AndroidJNI.GetMethodID(classID, "stopLiveBroadcast", "()Z");
        if (stopLiveBroadcastMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find stopLiveBroadcast() method.");
            return;
        }

        frameUpdatedMethodID = AndroidJNI.GetMethodID(classID, "frameUpdated", "()V");
        if (frameUpdatedMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find frameUpdated() method.");
            return;
        }

        enterLiveHallMethodID = AndroidJNI.GetMethodID(classID, "enterLiveHall", "(Landroid/content/Context;)V");
        if (enterLiveHallMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find enterLiveHall() method.");
            return;
        }

        enterLiveHallInGameMethodID = AndroidJNI.GetMethodID(classID, "enterLiveHallInGame", "(Landroid/content/Context;)V");
        if (enterLiveHallInGameMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find enterLiveHallInGameMethodID() method.");
            return;
        }

        updateUserAccountMethodID = AndroidJNI.GetMethodID(classID, "updateUserAccount", "(ILjava/lang/String;Ljava/lang/String;Ljava/lang/String;)V");
        if (updateUserAccountMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find updateUserAccount() method.");
            return;
        }

        getLiveBroadcastStatusMethodID = AndroidJNI.GetMethodID(classID, "getLiveBroadcastStatus", "()I");
        if (getLiveBroadcastStatusMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find getLiveBroadcastStatus() method.");
            return;
        }

        getErrorCodeMethodID = AndroidJNI.GetMethodID(classID, "getErrorCode", "()I");
        if (getErrorCodeMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find getErrorCode() method.");
            return;
        }

        setUserAccountListenerMethodID = AndroidJNI.GetMethodID(classID, "setUserAccountListener", "(Lcom/tencent/qgame/livesdk/bridge/UserAccountListener;)V");
        if (setUserAccountListenerMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find setUserAccountListener() method.");
            return;
        }

        setCommentReceiveDelegateMethodID = AndroidJNI.GetMethodID(classID, "setCommentReceiveListener", "(Lcom/tencent/qgame/livesdk/bridge/CommentReceiveListener;)V");
        if (setCommentReceiveDelegateMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find setCommentReceiveListener() method.");
            return;
        }

        setLogDelegateMethodID = AndroidJNI.GetMethodID(classID, "setLogListener", "(Lcom/tencent/qgame/livesdk/bridge/LogListener;)V");
        if (setLogDelegateMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find setLogListener() method.");
            return;
        }

        setLiveStatusChangedDelegateMethodID = AndroidJNI.GetMethodID(classID, "setLiveStatusChangedListener", "(Lcom/tencent/qgame/live/listener/OnLiveStatusChangedListener;)V");
        if (setLiveStatusChangedDelegateMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find setLiveStatusChangedListener() method.");
            return;
        }

        setShareListenerMethodID = AndroidJNI.GetMethodID(classID, "setShareListener", "(Lcom/tencent/qgame/livesdk/bridge/ShareListener;)V");
        if (setShareListenerMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find setShareListener() method.");
            return;
        }

        setErrorCodeDelegateMethodID = AndroidJNI.GetMethodID(classID, "setErrorCodeListener", "(Lcom/tencent/qgame/live/listener/ErrorCodeListener;)V");
        if (setErrorCodeDelegateMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find setErrorCodeDelegateMethodID() method.");
            return;
        }

        setWebViewStatusChangedDelegateMethodID = AndroidJNI.GetMethodID(classID, "setWebViewStatusChangedListener", "(Lcom/tencent/qgame/livesdk/bridge/WebViewStatusChangedListener;)V");
        if (setWebViewStatusChangedDelegateMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find setWebViewStatusChangedListener() method.");
            return;
        }

        isLiveBroadcastSupportedMethodID = AndroidJNI.GetMethodID(classID, "isLiveBroadcastSupported", "()Z");
        if (isLiveBroadcastSupportedMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find isLiveBroadcastSupportedMethodID() method.");
            return;
        }

        getVersionNameMethodID = AndroidJNI.GetMethodID(classID, "getVersionName", "()Ljava/lang/String;");
        if (getVersionNameMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find getVersionNameMethodID() method.");
            return;
        }

        showCameraMethodID = AndroidJNI.GetMethodID(classID, "showCamera", "()Z");
        if (showCameraMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find showCameraMethodID() method.");
            return;
        }

        hideCameraMethodID = AndroidJNI.GetMethodID(classID, "hideCamera", "()V");
        if (hideCameraMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find hideCameraMethodID() method.");
            return;
        }

        setEnvironmentTypeMethodID = AndroidJNI.GetMethodID(classID, "setEnvironmentType", "(I)V");
        if (setEnvironmentTypeMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find setEnvironmentTypeMethodID() method");
            return;
        }

        getLiveFrameRateMethodID = AndroidJNI.GetMethodID(classID, "getLiveFrameRate", "()I");
        if (getLiveFrameRateMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find getLiveFrameRate() method");
            return;
        }

        notifyUserAccountMethodID = AndroidJNI.GetMethodID(classID, "notifyUserAccountUpdate", "()V");
        if (notifyUserAccountMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find notifyUserAccountMethodID() method.");
            return;
        }

        doOnCreateMethodID = AndroidJNI.GetMethodID(classID, "doOnCreate", "()V");
        if (doOnCreateMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find doOnCreate() method");
            return;
        }
        doOnResumeMethodID = AndroidJNI.GetMethodID(classID, "doOnResume", "()V");
        if (doOnResumeMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find doOnResume() method");
            return;
        }
        doOnPauseMethodID = AndroidJNI.GetMethodID(classID, "doOnPause", "()V");
        if (doOnPauseMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find doOnPause() method");
            return;
        }
        doOnDestroyMethodID = AndroidJNI.GetMethodID(classID, "doOnDestroy", "()V");
        if (doOnDestroyMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find doOnDestroy() method");
            return;
        }
        doOnBackPressedMethodID = AndroidJNI.GetMethodID(classID, "doOnBackPressed", "()Z");
        if (doOnBackPressedMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find doOnBackPressed() method");
            return;
        }
        isSupportCameraMethodID = AndroidJNI.GetMethodID(classID, "isSupportCamera", "()Z");
        if (isSupportCameraMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find isSupportCamera() method");
            return;
        }
		shareLiveBroadcastMethodID = AndroidJNI.GetMethodID(classID, "shareLiveBroadcast", "()V");
		if (shareLiveBroadcastMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find shareLiveBroadcast() method");
            return;
        }

        isSupportLiveHallMethodID = AndroidJNI.GetMethodID(classID, "isSupportLiveHall", "()Z");
        if (isSupportLiveHallMethodID == IntPtr.Zero)
        {
            Debug.LogError("Can't find isSupportLiveHall() method");
            return;
        }
		
		setDanmakuEnabledMethodID = AndroidJNI.GetMethodID(classID, "setDanmakuEnabled", "(Z)V");
		if (setDanmakuEnabledMethodID == IntPtr.Zero)
		{
			Debug.LogError("Can't find setDanmakuEnabled() method");
			return;
		}
		
		showDanmakuMethodID = AndroidJNI.GetMethodID(classID, "showDanmaku", "()V");
		if (showDanmakuMethodID == IntPtr.Zero) 
		{
			Debug.LogError("Can't find showDanmaku() method");
			return;
		}
		
		hideDanmakuMethodID = AndroidJNI.GetMethodID(classID, "hideDanmaku", "()V");
		if (hideDanmakuMethodID == IntPtr.Zero)
		{
			Debug.LogError("Can't find hideDanmaku() method");
			return;
		}
		
        // Create Unity bridge object
        jvalue[] constructorParameters = AndroidJNIHelper.CreateJNIArgArray(new object[] {
            getActivityContext(), gameID, wnsID, Screen.width, Screen.height, Convert.ToInt32(myCaptureType), Convert.ToInt32(sdkEnvironmentType)
        });
        IntPtr local_capturingObject = AndroidJNI.NewObject(classID, constructorMethodID, constructorParameters);
        if (local_capturingObject == IntPtr.Zero)
        {
            Debug.LogError("--- Can't create Unity bridge object.");
            return;
        }
        // Keep a global reference to it
        androidBridge = AndroidJNI.NewGlobalRef(local_capturingObject);
        AndroidJNI.DeleteLocalRef(local_capturingObject);

        AndroidJNI.DeleteLocalRef(classID);

        Debug.Log("--- grabFrameRate = " + grabFrameRate);
        frameInterval = 1.0f / grabFrameRate;
        Debug.Log("--- 1.0f / grabFrameRate = " + frameInterval);
        startTime = Time.time;

        isRunning = false;
        if (myCaptureType == 0)
        {
            if (userAccountDelegate != null)
            {
                SetUserAccountDelegate(userAccountDelegate);
            }
            Debug.Log("CaptureType is zero, give up setup");
            return;
        }
        if (setup())
        {
            Debug.Log("Native setup success.");
        }
        else
        {
            Debug.LogError("Native setup failed!");
        }
    }

    void Awake()  {
      
    }
	void Start() {

       
    }


	void Update()
	{
		if (isRunning) {
			float elapsedTime = Time.time - startTime;
			//Debug.Log("elapsedTime=" + elapsedTime);
			if (elapsedTime >= frameInterval) {
				CaptureFrame();
				startTime = Time.time;
				//Debug.Log("CaptureFrame()");
			}
		}
	}

    private void SetEnvironmentType(QGameKit.Environment environmentType) {
        if (androidBridge == IntPtr.Zero) {
            Debug.LogError("androidBridge is null, cann't get setEnvironemtType!");
            return;   
        }
        jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(new object [] {
            (int) environmentType
        });
        AndroidJNI.CallVoidMethod(androidBridge, setEnvironmentTypeMethodID, args);
    }

    public bool IsLiveBroadcastSupported() {
        if (androidBridge == IntPtr.Zero) {
            Debug.LogError("androidBridge is null, cann't get liveBroadcast support!");
            return false;   
        }
        jvalue[] args = new jvalue[0];
		return AndroidJNI.CallBooleanMethod(androidBridge, isLiveBroadcastSupportedMethodID, args);
    }

	private bool setup() {
		if (androidBridge == IntPtr.Zero) {
            Debug.LogError("androidBridge is null, setup failed!");
			return false;
        }
        UserAccountCallback callback = new UserAccountCallback(userAccountDelegate);
		jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(new object [] {
			callback
		});
		return AndroidJNI.CallBooleanMethod(androidBridge, setupMethodID, args);
	}

	public void TearDown() {
		if (androidBridge == IntPtr.Zero) {
		    Debug.LogError("androidBridge is null, tear down failed!");
            return ;
		}
		jvalue[] args = new jvalue[0];
		AndroidJNI.CallVoidMethod(androidBridge, tearDownMethodID, args);
	}

	public bool Reset() {
		if (androidBridge == IntPtr.Zero) {
		    Debug.LogError("androidBridge is null, reset failed!");
            return false;
		}
		jvalue[] args = new jvalue[0];
		return AndroidJNI.CallBooleanMethod(androidBridge, resetMethodID, args);
	}

	public bool StartLiveBroadcast(string title, string description)
	{
		if (androidBridge == IntPtr.Zero) {
			Debug.LogError("Start Live Broadcast failed because bridge object is null!");
			return false;
		}

		nextCaptureTime = 0.0f;

		jvalue[] args = new jvalue[2];
		args[0].l = AndroidJNI.NewStringUTF(title);
		args[1].l = AndroidJNI.NewStringUTF(description);
		if (AndroidJNI.CallBooleanMethod(androidBridge, startLiveBroadcastMethodID, args)) {
			Debug.Log("Start Live Broadcast success");
            grabFrameRate = GetLiveFrameRate();
            frameInterval = 1.0f / grabFrameRate;
            Debug.Log("--- grabFrameRate = " + grabFrameRate);
            Debug.Log("--- 1.0f / grabFrameRate = " + frameInterval);
            return true;			
		}
		Debug.LogError("Native Start Live Broadcast failed!");
		return false;
	}
	
	public bool StopLiveBroadcast()
	{		
		if (androidBridge == IntPtr.Zero) {
		    Debug.LogError("androidBridge is null, stop liveBroadcast failed!");
            return false;
		}
		jvalue[] args = new jvalue[0];
		return AndroidJNI.CallBooleanMethod(androidBridge, stopLiveBroadcastMethodID, args);
	}

	public void EnterLiveHall() {
		if (androidBridge == IntPtr.Zero) {
		    Debug.LogError("androidBridge is null, enter LiveHall failed!");
            return ;
		}
		jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(new object [] {
			getActivityContext()
		});
		AndroidJNI.CallVoidMethod(androidBridge, enterLiveHallMethodID, args);
	}

    public void EnterLiveHallInGame()
    {
        if (androidBridge == IntPtr.Zero)
        {
            Debug.LogError("androidBridge is null, enter LiveHallInGame failed!");
            return;
        }
        jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(new object[] {
            getActivityContext()
        });
        AndroidJNI.CallVoidMethod(androidBridge, enterLiveHallInGameMethodID, args);
    }

    public void UpdateUserAccount(QGameKit.UserAccount account) {
		if (androidBridge == IntPtr.Zero || null == account) {
			Debug.LogError("Update user account failed because bridge object or account is null!");
			return ;
		}

		jvalue[] args = new jvalue[4];
		int platform = 0;
		if (QGameKit.LoginPlatform.QQ == account.platform) {
			platform = 1;
		} else if (QGameKit.LoginPlatform.WeChat == account.platform) {
			platform = 2;
		}
		args[0].i = platform;
		args[1].l = AndroidJNI.NewStringUTF(account.appId);
		args[2].l = AndroidJNI.NewStringUTF(account.id);
		args[3].l = AndroidJNI.NewStringUTF(account.token);
		AndroidJNI.CallVoidMethod(androidBridge, updateUserAccountMethodID, args);
	}
	
	public void UpdateUserAccount() {
		if (androidBridge == IntPtr.Zero) {
		    Debug.LogError("androidBridge is null, UpdateUserAccount down failed!");
            return ;
		}
		jvalue[] args = new jvalue[0];
		AndroidJNI.CallVoidMethod(androidBridge, notifyUserAccountMethodID, args);
	}

	public void CaptureFrame()
	{
		if (androidBridge == IntPtr.Zero) {
		    Debug.LogError("androidBridge is null, captureFrame failed!");
            return;
        }
		StartCoroutine(CaptureScreen());
	}

	private IEnumerator CaptureScreen() {
		yield return new WaitForEndOfFrame();

		//float start = Time.time;
        jvalue[] args = new jvalue[0];
        AndroidJNI.CallVoidMethod(androidBridge, frameUpdatedMethodID, args);
        //float delta = Time.time - start;
        //Debug.Log("CaptureFrame cost=" + delta);
	}	

	private static AndroidJavaObject getActivityContext ()
	{
		if (playerActivityContext == null) {
			AndroidJavaClass actClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			if (actClass == null) {
				Debug.LogError ("Get UnityPlayer Class failed");
				return null;
			}
			playerActivityContext = actClass.GetStatic<AndroidJavaObject> ("currentActivity");
			if (playerActivityContext == null) {
				Debug.LogError ("get context failed");
				return null;
			}
		}
		return playerActivityContext;
	}

    public class UserAccountCallback : AndroidJavaProxy
    {
        private QGameKit.UserAccountDelegate accountDelegate;
        public UserAccountCallback(QGameKit.UserAccountDelegate mDelegate) : base("com.tencent.qgame.livesdk.bridge.UserAccountListener"){ 
            accountDelegate = mDelegate;
        }
        AndroidJavaObject getUserAccount() {
            //Debug.Log("getUserAccount Thread " + Thread.CurrentThread.Name);
            QGameKit.UserAccount account = accountDelegate();
            AndroidJavaObject javaAccount = new AndroidJavaObject("com.tencent.qgame.livesdk.webview.Account"); 
            javaAccount.Set("openId", account.id);
            javaAccount.Set("accessToken", account.token);
            javaAccount.Set("loginType", (int)account.platform);
            javaAccount.Set("appId", account.appId);
            return javaAccount;
        }
    }
    public void SetUserAccountDelegate(QGameKit.UserAccountDelegate accountDelegate) {
        
        if (androidBridge == IntPtr.Zero)
        {
            Debug.LogError("androidBridge is null, setUserAccountDelegate failed!");
            return;
        }
        UserAccountCallback callback = new UserAccountCallback(accountDelegate);
		jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(new object [] {
			callback
		});
		AndroidJNI.CallVoidMethod(androidBridge, setUserAccountListenerMethodID, args);
    }

    public QGameKit.LiveStatus GetLiveBroadcastStatus() {
		if (androidBridge == IntPtr.Zero) {
            Debug.LogError("androidBridge is null, cann't getLiveBroadcastStatus!");
			return QGameKit.LiveStatus.Unknown;
		}
		jvalue[] args = new jvalue[0];
		int state = AndroidJNI.CallIntMethod(androidBridge, getLiveBroadcastStatusMethodID, args);
		return getStateById(state);    	
    } 

    public int GetErrorCode() {
		if (androidBridge == IntPtr.Zero) {
            Debug.LogError("androidBridge is null, getErrorCode failed!");
			return 0;
		}
		jvalue[] args = new jvalue[0];
		int error = AndroidJNI.CallIntMethod(androidBridge, getErrorCodeMethodID, args);
		return error;
    }

    public class CommentReceiveCallback : AndroidJavaProxy
    {
        private QGameKit.CommentReceiveDelegate commentDelegate;
        public CommentReceiveCallback(QGameKit.CommentReceiveDelegate mDelegate ) : base("com.tencent.qgame.livesdk.bridge.CommentReceiveListener"){
            commentDelegate = mDelegate;
        }
        void onCommentReceive(AndroidJavaObject comments) {
            List<QGameKit.LiveComment> commentList = new List<QGameKit.LiveComment>();
            int size = comments.Call<int>("size");
            for (int i = 0; i < size; i++) {
                AndroidJavaObject javaObject = comments.Call<AndroidJavaObject>("get", i);
                QGameKit.LiveComment comment = new QGameKit.LiveComment();
                comment.type = (QGameKit.CommentType)javaObject.Get<int>("msgType");
                comment.nick = javaObject.Get<string>("nick");
                comment.content = javaObject.Get<string>("msgContent");
                //unity 5.0以下不能直接传long
                string msgTimeStr = javaObject.Call<string>("msgTime2String");
                comment.timestamp = long.Parse(msgTimeStr);
                commentList.Add(comment);
            }
            commentDelegate(commentList);
        }
    }

    public void SetCommentReceiveDelegete(QGameKit.CommentReceiveDelegate commentDelegate) {
        if (androidBridge == IntPtr.Zero) {
            Debug.LogError("androidBridge is null, setCommentReceiveDelegate failed!");
		    return ;
		}
        CommentReceiveCallback callback = new CommentReceiveCallback(commentDelegate);
        jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(new object [] {
			callback
		});
		AndroidJNI.CallVoidMethod(androidBridge, setCommentReceiveDelegateMethodID, args);
    }

    public class LogCallback : AndroidJavaProxy
    {
        private QGameKit.LogDelegate logDelegate;
        public LogCallback(QGameKit.LogDelegate mDelegate) : base("com.tencent.qgame.livesdk.bridge.LogListener") { 
            logDelegate = mDelegate;
        }
        void log(string msg) {
            logDelegate(msg);
        }
    }
    
    public void SetLogDelegate(QGameKit.LogDelegate logDelegate) {
        if (androidBridge == IntPtr.Zero) {
            Debug.LogError("androidBridge is null, setLogDelegate failed!");
		    return ;
		}
        LogCallback callback = new LogCallback(logDelegate);
        jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(new object [] {
			callback
		});
		AndroidJNI.CallVoidMethod(androidBridge, setLogDelegateMethodID, args);
    }
    
    public class LiveStatusChangedCallback : AndroidJavaProxy
    {
        private QGameKit.LiveStatusChangedDelegate liveStatusChangedDelegate;
        private QGameKitAndroidBridge mBridge;
        public LiveStatusChangedCallback(QGameKitAndroidBridge bridge, QGameKit.LiveStatusChangedDelegate mDelegate) : base("com/tencent/qgame/live/listener/OnLiveStatusChangedListener") {
            liveStatusChangedDelegate = mDelegate;
            mBridge = bridge;
        }

        void onLiveStatusChanged(int stateId) {
        	Debug.Log ("Live status change to: " + stateId);
        	QGameKit.LiveStatus status = mBridge.getStateById(stateId);
        	switch (status) {
        		case QGameKit.LiveStatus.LiveStarting:
				mBridge.isRunning = true;
				Debug.Log("Start CaptureFrame");
        		break;

        		case QGameKit.LiveStatus.LivePaused:
        		case QGameKit.LiveStatus.LiveStopping:
        		mBridge.isRunning = false;
        		break;        		

        		case QGameKit.LiveStatus.Error:
        		mBridge.isRunning = false;
        		break;
        	}
            liveStatusChangedDelegate(status);
        }
    }
    
    public void SetLiveStatusChangedDelegate(QGameKit.LiveStatusChangedDelegate liveDelegate) {
       
        if (androidBridge == IntPtr.Zero) {
            Debug.LogError("androidBridge is null, setLiveStatusChangedDelegate failed!");
		    return ;
		}
        LiveStatusChangedCallback callback = new LiveStatusChangedCallback(this, liveDelegate);
        jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(new object [] {
			callback
		});
		AndroidJNI.CallVoidMethod(androidBridge, setLiveStatusChangedDelegateMethodID, args);
    }

    public class ShareDelegateCallback : AndroidJavaProxy
    {
        private QGameKit.ShareDelegate shareDelegate;
        public ShareDelegateCallback(QGameKit.ShareDelegate mDelegate) : base("com.tencent.qgame.livesdk.bridge.ShareListener") { 
            shareDelegate = mDelegate;
        }
        void share(AndroidJavaObject javaObject) {
            QGameKit.ShareContent share = new QGameKit.ShareContent();
            share.fopenId = javaObject.Get<string>("fopenId");
            share.title = javaObject.Get<string>("title");
            share.description = javaObject.Get<string>("description");
            share.targetUrl = javaObject.Get<string>("targetUrl");
            share.imageUrl = javaObject.Get<string>("imageUrl");
            shareDelegate(share);
        }
    }
    
    public void SetShareDelegate(QGameKit.ShareDelegate shareDelegate) {
        
        if (androidBridge == IntPtr.Zero) {
            Debug.LogError("androidBridge is null, setShareDelegate failed!");
		    return ;
		}
        ShareDelegateCallback callback = new ShareDelegateCallback(shareDelegate);
        jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(new object [] {
			callback
		});
		AndroidJNI.CallVoidMethod(androidBridge, setShareListenerMethodID, args);
    }

    public class ErrorCodeCallback : AndroidJavaProxy
    {
        private QGameKit.ErrorCodeListenerDelegate errorCodeDelegate;
        public ErrorCodeCallback(QGameKit.ErrorCodeListenerDelegate mDelegate) : base("com/tencent/qgame/live/listener/ErrorCodeListener") {
            errorCodeDelegate = mDelegate;
        }
        void onResult(int errorCode, String errorMessage) {
            errorCodeDelegate(errorCode, errorMessage);
        }
    }

    public void SetErrorCodeDelegate(QGameKit.ErrorCodeListenerDelegate errorDelegate) {
        
        if (androidBridge == IntPtr.Zero) {
            Debug.LogError("androidBridge is null, setErrorCodeDelegate failed!");
		    return ;
		}
        ErrorCodeCallback callback = new ErrorCodeCallback(errorDelegate);
        jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(new object [] {
			callback
		});
		AndroidJNI.CallVoidMethod(androidBridge, setErrorCodeDelegateMethodID, args);
    }

    public class WebViewStatusChangedCallback : AndroidJavaProxy
    {
        private QGameKit.WebViewStatusChangedDelegate webviewStatusChangedDelegate;
        public WebViewStatusChangedCallback(QGameKit.WebViewStatusChangedDelegate mDelegate) : base("com.tencent.qgame.livesdk.bridge.WebViewStatusChangedListener")
        {
            webviewStatusChangedDelegate = mDelegate;
        }

        void onWebViewStatusChanged(int status)
        {
            webviewStatusChangedDelegate((QGameKit.WebViewStatus) status);
        }
    }

    public void SetWebViewStatusChangedDelegate(QGameKit.WebViewStatusChangedDelegate webviewDelegate)
    {
        if (androidBridge == IntPtr.Zero)
        {
            Debug.LogError("androidBridge is null, setLiveStatusChangedDelegate failed!");
            return;
        }
        WebViewStatusChangedCallback callback = new WebViewStatusChangedCallback(webviewDelegate);
        jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(new object[] {
            callback
        });
        AndroidJNI.CallVoidMethod(androidBridge, setWebViewStatusChangedDelegateMethodID, args);
    }

    public string GetVersionName() {
        if (androidBridge == IntPtr.Zero) {
            Debug.LogError("androidBridge is null, getVersionName failed!");
			return null;
        }
        jvalue[] args = new jvalue[0];
		return AndroidJNI.CallStringMethod(androidBridge, getVersionNameMethodID, args);
    }

	public void ShareLiveBroadcast() {
		if (androidBridge == IntPtr.Zero)
        {
            Debug.LogError("androidBridge is null, ShareLiveBroadcast failed!");
            return;
        }
        jvalue[] args = new jvalue[0];
        AndroidJNI.CallVoidMethod(androidBridge, shareLiveBroadcastMethodID, args);
	}
	
    private int GetLiveFrameRate()
    {
        if (androidBridge == IntPtr.Zero)
        {
            Debug.LogError("androidBridge is null, getLiveFrameRate failed!");
            return 20;
        }
        jvalue[] args = new jvalue[0];
        return AndroidJNI.CallIntMethod(androidBridge, getLiveFrameRateMethodID, args);
    }

    public bool ShowCamera() {
        if (androidBridge == IntPtr.Zero) {
		    Debug.LogError("androidBridge is null, showCamera faile!");
            return false;
        }
        jvalue[] args = new jvalue[0];
        return AndroidJNI.CallBooleanMethod(androidBridge, showCameraMethodID, args);
    }

    public void HideCamera() {
        if (androidBridge == IntPtr.Zero) {
            Debug.LogError("androidBridge is null, hideCamera failed!");
			return ;
        }
        jvalue[] args = new jvalue[0];
		AndroidJNI.CallVoidMethod(androidBridge, hideCameraMethodID, args);
    }

    public void DoOnCreate()
    {
        if (androidBridge == IntPtr.Zero)
        {
            Debug.LogError("androidBridge is null, onCreate failed!");
            return;
        }
        jvalue[] args = new jvalue[0];
        AndroidJNI.CallVoidMethod(androidBridge, doOnCreateMethodID, args);
    }

    public void DoOnResume()
    {
        if (androidBridge == IntPtr.Zero)
        {
            Debug.LogError("androidBridge is null, onResume failed!");
            return;
        }
        jvalue[] args = new jvalue[0];
        AndroidJNI.CallVoidMethod(androidBridge, doOnResumeMethodID, args);
    }
    public void DoOnPause()
    {
        if (androidBridge == IntPtr.Zero)
        {
            Debug.LogError("androidBridge is null, onPause failed!");
            return;
        }
        jvalue[] args = new jvalue[0];
        AndroidJNI.CallVoidMethod(androidBridge, doOnPauseMethodID, args);
    }
    public void DoOnDestroy()
    {
        if (androidBridge == IntPtr.Zero)
        {
            Debug.LogError("androidBridge is null, onDestory failed!");
            return;
        }
        jvalue[] args = new jvalue[0];
        AndroidJNI.CallVoidMethod(androidBridge, doOnDestroyMethodID, args);
    }
    public bool DoOnBackPressed()
    {
        if (androidBridge == IntPtr.Zero)
        {
            Debug.LogError("androidBridge is null, onBackPressed failed!");
            return false;
        }
        jvalue[] args = new jvalue[0];
        return AndroidJNI.CallBooleanMethod(androidBridge, doOnBackPressedMethodID, args);
    }

    public bool IsSupportCamera()
    {
        if (androidBridge == IntPtr.Zero)
        {
            Debug.LogError("androidBridge is null, IsSupportCamera failed!");
            return false;
        }
        jvalue[] args = new jvalue[0];
        return AndroidJNI.CallBooleanMethod(androidBridge, isSupportCameraMethodID, args);
    }

    public bool IsSupportLiveHall()
    {
        if (androidBridge == IntPtr.Zero)
        {
            Debug.LogError("androidBridge is null, IsSupportLiveHall failed!");
            return false;
        }
        jvalue[] args = new jvalue[0];
        return AndroidJNI.CallBooleanMethod(androidBridge, isSupportLiveHallMethodID, args);
    }
	
	public void SetDanmakuEnabled(bool enable) {
		if (androidBridge == IntPtr.Zero)
		{
			Debug.LogError("androidBridge is null, setDanmakuEnabled failed!");
            return;
		}
		jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(new object[] {
            enable
        });
		AndroidJNI.CallVoidMethod(androidBridge, setDanmakuEnabledMethodID, args);
	}
	
	public void ShowDanmaku() {
		if (androidBridge == IntPtr.Zero)
        {
            Debug.LogError("androidBridge is null, ShowDanmaku failed!");
            return;
        }
        jvalue[] args = new jvalue[0];
		AndroidJNI.CallVoidMethod(androidBridge, showDanmakuMethodID, args);
	}
	
	public void HideDanmaku() {
		if (androidBridge == IntPtr.Zero)
        {
            Debug.LogError("androidBridge is null, HideDanmaku failed!");
            return;
        }
        jvalue[] args = new jvalue[0];
		AndroidJNI.CallVoidMethod(androidBridge, hideDanmakuMethodID, args);
	}

    private void JavaMessage(string message)
    {
        Debug.Log("message " + message);
        
    }

    private QGameKit.LiveStatus getStateById(int id) {
    	switch (id) {
    		case 1:	// Uninit
    		return (QGameKit.LiveStatus.Uninitialized);
    		case 2:	// Prepared
    		return (QGameKit.LiveStatus.Prepared);
    		case 3: // Starting
    		return (QGameKit.LiveStatus.LiveStarting);
    		case 4:	// Started
    		return (QGameKit.LiveStatus.LiveStarted);
    		case 5:	// Paused
    		return (QGameKit.LiveStatus.LivePaused);
			case 6: // Resume
			return (QGameKit.LiveStatus.LiveResume);
    		case 7:	// Stopping
    		return (QGameKit.LiveStatus.LiveStopping);      		
    		case 8:	// Stopped
    		return (QGameKit.LiveStatus.LiveStopped);
    		case 9:	// Error
    		return (QGameKit.LiveStatus.Error);
    	}
    	return QGameKit.LiveStatus.Unknown;    	
    }
}

#endif

#endif