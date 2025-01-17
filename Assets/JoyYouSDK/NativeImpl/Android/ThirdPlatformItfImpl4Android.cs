using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.SDK;

namespace Assets.JoyYouSDK.NativeImpl.Android
{
	public class ThirdPlatformItfImpl4Android : I3RDPlatformSDK
	{
		public ThirdPlatformItfImpl4Android(int appId,
						string appKey,
						bool logEnable,
						bool isLongConnect,
						bool rechargeEnable,
						int rechargeAmount,
						string closeRechargeAlertMessage,
						string paramSendMsgNotiClass,
						bool isOriPortrait,
						bool isOriLandscapeLeft,
						bool isOriLandscapeRight,
						bool isOriPortraitUpsideDown
		)
		{
#if UNITY_ANDROID
			AndroidBridge.CommonPlatformCall("Init",
					appId, appKey, logEnable,
					isLongConnect, rechargeEnable, rechargeAmount, closeRechargeAlertMessage, paramSendMsgNotiClass,
					isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown);
#endif
		}
		void I3RDPlatformSDK.ShowLoginView()
		{
#if UNITY_ANDROID
			AndroidBridge.CommonPlatformCall("Login", null, "", "");
#endif
		}

		void I3RDPlatformSDK.ShowLoginViewWithType(int type)
		{
#if UNITY_ANDROID
			AndroidBridge.CommonPlatformCall("Login", type);
#endif
		}

		void I3RDPlatformSDK.ShowCenterView()
		{
#if UNITY_ANDROID
			AndroidBridge.CommonPlatformCall("ShowUserCentered");
#endif
		}
		void I3RDPlatformSDK.Pay(int paramPrice, string paramBillNo, string paramBillTitle,
										 string paramRoleId, int paramZoneId)
		{
#if UNITY_ANDROID
			AndroidBridge.CommonPlatformCall("PayGoods", paramPrice, paramBillNo, paramBillTitle, paramRoleId, paramZoneId);
#endif
		}
		void I3RDPlatformSDK.Logout()
		{
#if UNITY_ANDROID
			AndroidBridge.CommonPlatformCall("Logout");
#endif
		}
	}
}
