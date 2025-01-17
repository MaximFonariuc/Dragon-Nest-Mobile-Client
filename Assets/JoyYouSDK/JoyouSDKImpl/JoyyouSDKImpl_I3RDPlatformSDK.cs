namespace Assets.SDK
{
	public partial class JoyYouSDK : I3RDPlatformSDK
	{
		void I3RDPlatformSDK.ShowLoginView()
		{
			if (isInitialised)
			{
				JoyYouNativeInterface.ShowLoginView();
			}
		}

		void I3RDPlatformSDK.ShowLoginViewWithType(int type)
		{
			if (isInitialised)
			{
				JoyYouNativeInterface.ShowLoginViewWithType(type);
			}
		}

		void I3RDPlatformSDK.Logout()
		{
			if (isInitialised)
			{
				JoyYouNativeInterface.Logout();
			}
		}

		void I3RDPlatformSDK.Pay(int price, string billNo, string billTitle, string roleId, int zoneId)
		{
			if (isInitialised)
			{
				JoyYouNativeInterface.ExchangeGoods(price, billNo, billTitle, roleId, zoneId);
			}
		}

		void I3RDPlatformSDK.ShowCenterView()
		{
			if (isInitialised)
			{
				JoyYouNativeInterface.ShowCenterView();
			}
		}
	}
}
