using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Assets.SDK;
namespace Assets.JoyYouSDK.NativeImpl.IOS
{/*
	public class ThirdPlatformItfImpl4IOS : I3RDPlatformSDK
	{
#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void U3D_initSDK(int appId,
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
									);
		[DllImport("__Internal")]
		private static extern void U3D_showLoginView();

		[DllImport("__Internal")]
		private static extern void U3D_showCenterView();

		[DllImport("__Internal")]
		private static extern void U3D_exchangeGoods(int paramPrice, string paramBillNo, string paramBillTitle,
											 string paramRoleId, int paramZoneId);
		[DllImport("__Internal")]
		private static extern void U3D_logout();
#endif
		public ThirdPlatformItfImpl4IOS(int appId,
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
#if UNITY_IPHONE
			U3D_initSDK(appId
				, appKey
				, logEnable
				, isLongConnect
				, rechargeEnable
				, rechargeAmount
				, closeRechargeAlertMessage
				, paramSendMsgNotiClass
				, isOriPortrait
				, isOriLandscapeLeft
				, isOriLandscapeRight
				, isOriPortraitUpsideDown);
#endif
		}
		void I3RDPlatformSDK.ShowLoginView()
		{
#if UNITY_IPHONE
			U3D_showLoginView();
#endif
		}
		void I3RDPlatformSDK.ShowCenterView()
		{
#if UNITY_IPHONE
			U3D_showCenterView();
#endif
		}
		void I3RDPlatformSDK.Pay(int paramPrice, string paramBillNo, string paramBillTitle,
										 string paramRoleId, int paramZoneId)
		{
#if UNITY_IPHONE
			U3D_exchangeGoods(paramPrice, paramBillNo, paramBillTitle, paramRoleId, paramZoneId);
#endif
		}
		void I3RDPlatformSDK.Logout()
		{
#if UNITY_IPHONE
			U3D_logout();
#endif
		}
	}*/
}
