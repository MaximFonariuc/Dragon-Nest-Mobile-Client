using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Msdk
{
    class MsdkUtil
    {
        public static readonly string UnityTag = "MSDK Unity";

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR

#elif UNITY_ANDROID
        public static AndroidJavaClass androidLog = new AndroidJavaClass("com.tencent.msdk.tools.Logger");
#elif UNITY_IPHONE
        [DllImport("__Internal")]
        public static extern void iosLog(string msg);
#endif

        public static void Log(string msg)
        {
            try {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
                Debug.Log(msg);
#elif UNITY_ANDROID
                Debug.Log(UnityTag + ":" + msg);
                // if (androidLog != null) {
                //     androidLog.CallStatic("d", UnityTag, msg);
                // } else {
                //     Debug.LogError("Android Logger is null!");
                // }
#elif UNITY_IPHONE
                iosLog(UnityTag + ":" + msg);
#endif
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }


    }
}
