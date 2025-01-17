using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

//modified in date 2017-04-05 , verison is 3.2
public sealed class CubeApmAgent
{
    private static string mHawkVersion = "3.2";

    private static bool _isInitialized = false;
    private static bool _hawkCtxInit = false;

    public static int GPU_OPT = 1;
    public static int CPU_OPT = 2;
    public static int FPS_OPT = 4;
    public static int PSS_OPT = 8;
    public static int DC_OPT = 16;
    public static int TRI_OPT = 32;
    public static int TEX_OPT = 64;
    public static int MONO_OPT = 128;
    public static int NET_OPT = 256;

    public static int LOW_QUALITY = 2;
    public static int MEDIUM_QUALITY = 4;
    public static int HIGH_QUALITY = 8;


    public static void SetUserId(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        _setUserId(userId);

    }

    public static void SetAppId(string appId)
    {
        if (string.IsNullOrEmpty(appId))
        {
            return;
        }

        _setAppId(appId);

    }

    public static int InitContext()
    {
        _initContext();
        return 0;
    }

    public static void MarkLoadlevel(string scene_name, int quality = 0)
    {
        _markLoadlevel(scene_name, quality);
    }
    public static void MarkLoadlevelCompleted()
    {
        _markLoadlevelCompleted();
    }
    public static void MarkLevelEnd()
    {
        _markLevelFin();
    }

    public static void DisableOpts(int opts)
    {
        _disableOpts(opts);
    }


#if (UNITY_EDITOR || UNITY_STANDALONE)

    private static int _initContext()
    {
        Debug.Log("[xclient]\tUNITY_EDITOR empty implementation , initcontext");
        return 0;
    }

    private static void _setUserId(string userId)
    {
        Debug.Log("[xclient]\tHawkSetUserId:" + userId);
    }

    private static void _setAppId(string appId)
    {
        Debug.Log("[xclient]\tHawkSetAppId:" + appId);
    }

    private static void _markLoadlevel(string scene_name, int quality)
    {
        Debug.Log("[xclient]\tmarkLoadlevel");
    }
    private static void _markLoadlevelCompleted()
    {
        Debug.Log("[xclient]\tMarkLoadlevelFin:");
    }
    private static void _markLevelFin()
    {
        Debug.Log("[xclient]\tmarkLevelEnd");
    }
    private static void _disableOpts(int opts)
    {
        Debug.Log("[xclient]\tdisableOpts");
    }


#elif UNITY_ANDROID

	private static readonly string CLASS_UNITYAGENT = "com.tencent.hawk.bridge.HawkAgent";
	private static AndroidJavaObject _hawk_bridge;
	public static AndroidJavaObject HawkBridge
	{
		get
		{
			if (_hawk_bridge == null)
			{
				_hawk_bridge = new AndroidJavaObject(CLASS_UNITYAGENT);
			}
			return _hawk_bridge;
		}
	}

	private static int _initContext()
	{
        if(_hawkCtxInit){
            return -1;
        }

		if(!_isInitialized)
		{
			_isInitialized = true;
		}else{
			return -1;
		}

		Debug.Log("[xclient] :  begin hawk init flag ");
        Debug.Log("[xclient] : " + SystemInfo.graphicsDeviceName);
        Debug.Log("[xclient] : " + SystemInfo.graphicsDeviceVendor);
        Debug.Log("[xclient] : " + SystemInfo.graphicsDeviceVersion);
        
        string gpuVendor = SystemInfo.graphicsDeviceVendor;
        string gpuRenderer = SystemInfo.graphicsDeviceName;
        string gpuVersion = SystemInfo.graphicsDeviceVersion;

        if(string.IsNullOrEmpty(gpuVendor))
        {
            gpuVendor = "NA";
        }
        
        if(string.IsNullOrEmpty(gpuRenderer))
        {
            gpuRenderer = "NA";
        }
        
        if(string.IsNullOrEmpty(gpuVersion))
        {
            gpuVersion = "NA";
        }


		try{
			int initflag = HawkBridge.CallStatic<int>("hawkInit", gpuVendor, gpuRenderer, gpuVersion);
			Debug.Log("[xclient] :  end hawk init flag : "+initflag);
		}catch(Exception e){
			Debug.Log("[xclient] :  initContext failed "+e);
		}
        _hawkCtxInit = true;
		return 0;
	}

	private static void _setUserId(string userId)
	{
		//Debug.Log("[xclient] :  begin setUserId ");
		try{
			HawkBridge.CallStatic("setUserId", userId);
		}catch{
			//Debug.LogWarning("setUserId failed");
		}
		//Debug.Log("[xclient] :  end setUserId ");
	}

	private static void _setAppId(string appId)
	{
    
		Debug.Log("[xclient] :  begin setAppId ");
		try{
			HawkBridge.CallStatic("setAppId", appId);
		}catch{
			Debug.LogWarning("setAppId failed");
		}
		Debug.Log("[xclient] :  end setAppId ");
	}

	private static void _markLoadlevel(string scene_name, int quality)
	{
		try{
			HawkBridge.CallStatic("markLevelLoad", scene_name, quality);
		}catch{
			//Debug.LogWarning("markLoadlevel failed");
		}

	}

	private static void _markLoadlevelCompleted()
	{
		try{
			HawkBridge.CallStatic("markLevelLoadCompleted");
		}catch{
			//Debug.LogWarning("markLoadlevelCompteted failed");
		}
		
	}

	private static void _markLevelFin()
	{
		try{
			//HawkBridge.CallStatic("markLevelFin");
		}catch{
			//Debug.LogWarning("markLevelEnd failed");
		}

	}

    private static void _disableOpts(int opts)
    {
        try{
			HawkBridge.CallStatic("disableOpts", opts);
		}catch{
			Debug.LogWarning("disableOpts failed");
		}
    }


#elif (UNITY_IOS || UNITY_IPHONE)
	private static int _initContext()
	{
		return 0;
	}

	private static void _setUserId(string userId)
	{
		
	}

	private static void _setAppId(string appId)
	{
		
	}

	private static void _markLoadlevel(string scene_name, int quality)
	{
		
	}

	private static void _markLoadlevelCompleted()
	{
		
	}

	private static void _markLevelFin()
	{

	}

    private static void _disableOpts(int opts)
    {
       
    }

   
#endif
}

