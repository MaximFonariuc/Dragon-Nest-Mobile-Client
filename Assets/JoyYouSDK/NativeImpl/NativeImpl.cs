using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.SDK;

namespace Assets.JoyYouSDK.NativeImpl
{
	public interface ITPNative
	{
		void _U3D_initSDK(
			int appId,
			string appKey,
			bool logEnable,
			bool isLongConnect,
			bool rechargeEnable,
			int rechargeAmount,
			string closeRechargeAlertMsg,
			string notificationObjectName,
			bool isOriPortrait,
			bool isOriLandscapeLeft,
			bool isOriLandscapeRight,
			bool isOriPortraitUpsideDown);
		void _U3D_showLoginView();
		void _U3D_showCenterView();
		void _U3D_logout();
		void _U3D_exchangeGoods(int paramPrice, string paramBillNo, string paramBillTitle,
											 string paramRoleId, int paramZoneId);
	}

	public static class ThirdPlatformAdapter
	{
		private static ITPNative tpItf =
#if UNITY_IPHONE
			null; //new Assets.JoyYouSDK.NativeImpl.IOS.ThirdPlatformItfImpl4IOS();
#elif UNITY_ANDROID
 null;//new Assets.JoyYouSDK.NativeImpl.Android.ThirdPlatformItfImpl4Android();
#else
				null;
#endif
		public static void InitSDK(
			int appId,
			string appKey,
			bool logEnable,
			bool isLongConnect,
			bool rechargeEnable,
			int rechargeAmount,
			string closeRechargeAlertMsg,
			string notificationObjectName,
			bool isOriPortrait,
			bool isOriLandscapeLeft,
			bool isOriLandscapeRight,
			bool isOriPortraitUpsideDown)
		{
			if (tpItf != null && Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
				tpItf._U3D_initSDK(appId,
							appKey,
							logEnable,
							isLongConnect,
							rechargeEnable,
							rechargeAmount,
							closeRechargeAlertMsg,
							notificationObjectName,
							isOriPortrait,
							isOriLandscapeLeft,
							isOriLandscapeRight,
							isOriPortraitUpsideDown);
			}
			else
			{
				JoyYouInterfaceSimulator.NotificationObjeName = notificationObjectName;
			}
		}

		public static void ShowLoginView()
		{
			if (tpItf != null && Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
					tpItf._U3D_showLoginView();
			}
			else
			{
				JoyYouInterfaceSimulator.ShowLoginView();
			}
		}

		public static void ShowCenterView()
		{
			if (tpItf != null && Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
				tpItf._U3D_showCenterView();
			}
			else
			{
				JoyYouInterfaceSimulator.ShowCenterView();
			}
		}

		public static void Logout()
		{
			if (tpItf != null && Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
				tpItf._U3D_logout();
			}
			else
			{
				JoyYouInterfaceSimulator.Logout();
			}
		}

		public static void ExchangeGoods(int price, string billNo, string billTitle, string roleId, int zoneId)
		{
			if (tpItf != null && Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
				tpItf._U3D_exchangeGoods(price, billNo, billTitle, roleId, zoneId);
			}
			else
			{
				JoyYouInterfaceSimulator.Pay();
			}
		}
	}

	public interface ITPExNative
	{
		void ShowFloatToolkit(bool visible, double x, double y);
		void SendGameExtData(string type, string jsonData);
		void QuitGame(string paramString);
		void RequestRealUserRegister(string uid, bool IsQuery);
	}
}
