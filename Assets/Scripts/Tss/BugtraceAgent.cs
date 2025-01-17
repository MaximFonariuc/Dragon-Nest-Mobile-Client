using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

public sealed class BugtraceAgent  {
	private const string SDK_PACKAGE = "com.tencent.tp.bugtrace";

#if UNITY_4_6
	private static Application.LogCallback s_oldLogCallback;
	public static Application.LogCallback GetCurrentLogCallback(){
		Type t = typeof(Application);
		
		// Instance Field
		BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		
		// Static Field
		flag = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
		FieldInfo fieldInfo = t.GetField ("s_LogCallback", flag);
		
		if (fieldInfo != null && fieldInfo.IsPrivate && fieldInfo.IsStatic) {
			object callback = fieldInfo.GetValue (null);
			if (callback != null){
				return (Application.LogCallback)callback;
				
			}
		}
		return null;
	}
#endif

	private static bool _isInitialized = false;
	public static void EnableExceptionHandler(){
		if (_isInitialized){
			return;
		}
		RegisterExceptionHandler();
		_isInitialized = true;
	}

	public static void DisableExceptionHandler(){
		if (!_isInitialized){
			return;
		}
		UnregisterExceptionHandler();
		_isInitialized = false;
	}

	private static void RegisterExceptionHandler(){
#if UNITY_4_6 
		AppDomain.CurrentDomain.UnhandledException += UncaughtExceptionHandler;
		s_oldLogCallback = GetCurrentLogCallback ();
		Application.RegisterLogCallback (LogCallbackHandler);
#endif

#if (UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
		Application.logMessageReceived += LogCallbackHandler;
#endif

	}

	private static void UnregisterExceptionHandler(){
#if UNITY_4_6
		AppDomain.CurrentDomain.UnhandledException -= UncaughtExceptionHandler;
		Application.RegisterLogCallback (s_oldLogCallback);
#endif

#if (UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
		Application.logMessageReceived -= LogCallbackHandler;
#endif
	}

	private static void LogCallbackHandler(string condition, string stack, LogType type){
		if (type == LogType.Exception) {
			//FileLog.Instance.Log ("<LogCallbackHandler> reason:" + condition);
			//FileLog.Instance.Log ("<LogCallbackHandler> stack:" + stack);
			HandleException (condition, stack);
		}
#if UNITY_4_6
		if (s_oldLogCallback != null) {
			s_oldLogCallback(condition, stack, type);
		}
#endif

	}

	private static void UncaughtExceptionHandler(object sender, System.UnhandledExceptionEventArgs args){
		Exception e = (Exception)args.ExceptionObject;
		if (e != null){
			HandleException (e.Message, e.StackTrace);
		}

	}

	//private static void ReportException(string reason, string stack){
		//FileLog.Instance.Log ("<ReportException> reason:" + reason);
		//FileLog.Instance.Log ("<ReportException> stack:" + stack);
		//HandleException (reason, stack);
	//}

#if (UNITY_EDITOR || UNITY_STANDALONE)
	private static void HandleException(string reason, string stack){

	}





#elif UNITY_ANDROID
	private static readonly string CLASS_UNITYAGENT = "com.tencent.tp.bugtrace.BugtraceAgent";
	private static AndroidJavaObject _unityAgent;
	public static AndroidJavaObject UnityAgent{
		get{
			if (_unityAgent == null){
				using (AndroidJavaClass clazz = new AndroidJavaClass(CLASS_UNITYAGENT)) {
					_unityAgent = clazz.CallStatic<AndroidJavaObject> ("getInstance");
				}
			}
			return _unityAgent;
		}
	}
	private static void HandleException(string reason, string stack){
        string cmd = "crash-reportcsharpexception|";
        cmd += "cause:" + reason;
        cmd += "stack:" + stack;
		//FileLog.Instance.Log("Android HandleException");
		try{
			AndroidJavaClass agent  = new AndroidJavaClass("com.tencent.tp.TssJavaMethod");
            if (agent != null){
                agent.CallStatic("sendCmd", cmd);
            }
		}catch{
			//FileLog.Instance.Log("Android HandleException catch");
		}
	}

#elif (UNITY_IOS || UNITY_IPHONE)
	[DllImport("__Internal")]
	private static extern void ReportCSharpException (string reason, string stack);
	private static void HandleException(string reason, string stack){
		ReportCSharpException(reason, stack);
	}

#endif
} // end of class BugtraceAgent
