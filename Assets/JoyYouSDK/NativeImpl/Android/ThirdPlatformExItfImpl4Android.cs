using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.SDK;

namespace Assets.JoyYouSDK.NativeImpl.Android
{
	public class ThirdPlatformExItfImpl4Android : I3RDPlatformSDKEX
	{
		void I3RDPlatformSDKEX.SendGameExtData(string type, string jsonData)
		{
#if UNITY_ANDROID
			AndroidBridge.CommonPlatformCall("SendGameExtData", type, jsonData);
#endif
		}

		bool I3RDPlatformSDKEX.CheckStatus(string type, string jsonData)
		{
			bool flag = true;

#if UNITY_ANDROID
			flag = AndroidBridge.CommonPlatformCall<bool>("CheckStatus", type, jsonData);
#endif

			return flag;
		}

		string I3RDPlatformSDKEX.GetSDKConfig(string type, string jsonData)
		{
			string defaultValue = string.Empty;

#if UNITY_ANDROID
			defaultValue = AndroidBridge.CommonPlatformCall<string>("GetSDKConfig", type, jsonData);
#endif

			return defaultValue;
		}

		void I3RDPlatformSDKEX.QuitGame(string paramString)
		{
#if UNITY_ANDROID
			AndroidBridge.CommonPlatformCall("Release");
#endif
		}

		void I3RDPlatformSDKEX.ShowFloatToolkit(bool visible, double x, double y)
		{
#if UNITY_ANDROID
			AndroidBridge.CommonPlatformCall("ShowFloatToolkit", visible, x, y);
#endif
		}

		void I3RDPlatformSDKEX.RequestRealUserRegister(string uid, bool IsQuery)
		{
#if UNITY_ANDROID
			AndroidBridge.CommonPlatformCall("RequestRealUserRegister", uid, IsQuery);
#endif
		}
	}
}
