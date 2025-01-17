using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.JoyYouSDK.NativeImpl.Android
{
	public static class AndroidBridge
	{
		private static string IStatisticalData_native_objname = "__IStatisticalData";
		private static string IHuanleSDK_native_objname = "__ICommonSDKPlatform";
		private static string IDeviceInfo_native_objname = "__IDeviceInfomation";
		private static string IGameRecord_native_objname = "__";

		private static void AndroidInvoke(string _itf_obj_name, string method, params object[] args)
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
					{
						using (AndroidJavaClass klass = new AndroidJavaClass("com.joyyou.itf.JoyyouInterfaceFactory"))
						{
							string initItfMethodName =
								_itf_obj_name == IHuanleSDK_native_objname ? "initInstance4BridgedCommonSDKPlatform" :
								_itf_obj_name == IStatisticalData_native_objname ? "initInstance4Statistical" :
								_itf_obj_name == IDeviceInfo_native_objname ? "initInstance4DeviceInfomation" :
								_itf_obj_name == IGameRecord_native_objname ? "initInstance4GameRecord" :
								"ERROR_ITF_NAME"
								;
							klass.CallStatic(initItfMethodName, jo);
							using (AndroidJavaObject itf4Obj = klass.GetStatic<AndroidJavaObject>(_itf_obj_name))
							{
								itf4Obj.Call(method, args);
							}
						}
					}
				}
#endif
			}
		}

		private static T AndroidInvoke<T>(string _itf_obj_name, string method, params object[] args)
		{
			T value = default(T);
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
					{
						using (AndroidJavaClass klass = new AndroidJavaClass("com.joyyou.itf.JoyyouInterfaceFactory"))
						{
							string initItfMethodName =
								_itf_obj_name == IHuanleSDK_native_objname ? "initInstance4BridgedCommonSDKPlatform" :
								_itf_obj_name == IStatisticalData_native_objname ? "initInstance4Statistical" :
								_itf_obj_name == IDeviceInfo_native_objname ? "initInstance4DeviceInfomation" :
								_itf_obj_name == IGameRecord_native_objname ? "initInstance4GameRecord" :
								"ERROR_ITF_NAME"
								;
							klass.CallStatic(initItfMethodName, jo);
							using (AndroidJavaObject itf4Obj = klass.GetStatic<AndroidJavaObject>(_itf_obj_name))
							{
								return itf4Obj.Call<T>(method, args);
							}
						}
					}
				}
#endif
			}
			return value;
		}

		public static void CommonPlatformCall(string method, params object [] args)
		{
			AndroidInvoke(IHuanleSDK_native_objname, method, args);
		}

		public static T CommonPlatformCall<T>(string method, params object[] args)
		{
			return AndroidInvoke<T>(IHuanleSDK_native_objname, method, args);
		}


		public static void StatisticalDataCall(string method, params object[] args)
		{
			AndroidInvoke(IStatisticalData_native_objname, method, args);
		}

		public static T StatisticalDataCall<T>(string method, params object[] args)
		{
			return AndroidInvoke<T>(IStatisticalData_native_objname, method, args);
		}

		public static void DeviceInfomationCall(string method, params object[] args)
		{
			AndroidInvoke(IDeviceInfo_native_objname, method, args);
		}

		public static T DeviceInfomationCall<T>(string method, params object[] args)
		{
			return AndroidInvoke<T>(IDeviceInfo_native_objname, method, args);
		}

		public static void GameRecordCall(string method, params object[] args)
		{
			AndroidInvoke(IGameRecord_native_objname, method, args);
		}

		public static T GameRecordCall<T>(string method, params object[] args)
		{
			return AndroidInvoke<T>(IGameRecord_native_objname, method, args);
		}
	}
}
