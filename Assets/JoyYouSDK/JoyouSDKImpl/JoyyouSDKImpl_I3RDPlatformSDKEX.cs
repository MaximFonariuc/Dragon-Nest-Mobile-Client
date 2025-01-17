namespace Assets.SDK
{
	public partial class JoyYouSDK : I3RDPlatformSDKEX
	{
		void I3RDPlatformSDKEX.SendGameExtData(string type, string jsonData)
		{
			if (isInitialised)
			{
				JoyYouNativeInterface.SendGameExtData(type, jsonData);
			}
		}

		bool I3RDPlatformSDKEX.CheckStatus(string type, string jsonData)
		{
			bool flag = true;

			if (isInitialised)
			{
				flag = JoyYouNativeInterface.CheckStatus(type, jsonData);
			}

			return flag;
		}

		string I3RDPlatformSDKEX.GetSDKConfig(string type, string jsonData)
		{
			string defaultValue = string.Empty;

			if (isInitialised)
			{
				defaultValue = JoyYouNativeInterface.GetSDKConfig(type, jsonData);
			}

			return defaultValue;
		}

		void I3RDPlatformSDKEX.QuitGame(string paramString)
		{
			if (isInitialised)
			{
				JoyYouNativeInterface.QuitGame(paramString);
			}
		}

		void I3RDPlatformSDKEX.ShowFloatToolkit(bool visible, double x, double y)
		{
			if (isInitialised)
			{
				JoyYouNativeInterface.ShowFloatToolkit(visible, x, y);
			}
		}
		
		void I3RDPlatformSDKEX.RequestRealUserRegister(string uid, bool IsQuery)
		{
			if (isInitialised)
			{
                JoyYouNativeInterface.RequestRealUserRegister(uid, IsQuery);
			}
		}
	}
}
