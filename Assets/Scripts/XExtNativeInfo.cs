using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class XExtNativeInfo
{

    [DllImport("__Internal")]
    private static extern int GetDensity();

    [DllImport("__Internal")]
    private static extern string CheckSIM();
    
    
    public static int U3DGetDensity()
    {
#if UNITY_EDITOR
        return 200;
#elif UNITY_ANDROID
        int density = 200;
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.act.hot1.tencent.SystemInfoActivity");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("uniqueInstance");
            density = jo.Call<int>("GetDensity");
        }
        catch (Exception e) { Debug.Log("err: "+e.StackTrace); }
        Debug.Log("android density is: " + density);
        return density;
#elif UNITY_IOS
        int density = GetDensity();
        Debug.Log("ios density is: " + density);
        return density;
#else
        return 200;
#endif
    }



    public static string U3DGetSim()
    {
#if UNITY_EDITOR
        return "";
#elif UNITY_ANDROID
        string str = "";
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.act.hot1.tencent.SystemInfoActivity");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("uniqueInstance");
            str = jo.Call<string>("CheckSIM");
        }
        catch (Exception e) { Debug.Log("err: " + e.StackTrace); }
        Debug.Log("androidCheckSIM: " + str);
        return str;
#elif UNITY_IOS
        string str = CheckSIM();
        Debug.Log("ios CheckSIM: " + str);
        return str;
#else
        return "";
#endif

    }

}
