using UnityEngine;
using System.Collections;

namespace Msdk {

	public sealed class WGPlatform {

		private static string msdkUnityVersion = "3.3.30u";
		private static IMsdk _instance;
        private static bool pcDebug = false;

		private WGPlatform() {}

		public static IMsdk Instance
		{
			get
			{
				if(_instance == null){
                    #if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
                    if (pcDebug) {
                        _instance = new WGPlatformPC();
                    }
                    #elif UNITY_ANDROID
					_instance = new WGPlatformAndroid();
                    #elif UNITY_IPHONE
					_instance = new WGPlatformIOS();
                    #endif
				}
				return _instance;
			}
		}

		public static string Version
		{
			get { return msdkUnityVersion; }
		}

        public static void SetPCDebug(bool open)
        {
            pcDebug = open;
        }

        public static bool GetPCDebug()
        {
            return pcDebug;
        }
	}
}
