using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Assets.SDK;

namespace Assets.JoyYouSDK.NativeImpl.IOS
{
	class ThirdPlatformExItfImpl4IOS : I3RDPlatformSDKEX
	{
		void I3RDPlatformSDKEX.SendGameExtData(string type, string jsonData)
		{
#if UNITY_IPHONE
#endif
		}

		bool I3RDPlatformSDKEX.CheckStatus(string type, string jsonData)
		{
			bool flag = true;

#if UNITY_IPHONE
#endif

			return flag;
		}

		string I3RDPlatformSDKEX.GetSDKConfig(string type, string jsonData)
		{
			string defaultValue = string.Empty;

#if UNITY_IPHONE
#endif

			return defaultValue;
		}

		void I3RDPlatformSDKEX.QuitGame(string paramString)
		{
#if UNITY_IPHONE
#endif
		}

		void I3RDPlatformSDKEX.ShowFloatToolkit(bool visible, double x, double y)
		{
#if UNITY_IPHONE
#endif
		}

		void I3RDPlatformSDKEX.RequestRealUserRegister(string uid, bool IsQuery)
		{
#if UNITY_IPHONE
#endif
		}
	}
}
