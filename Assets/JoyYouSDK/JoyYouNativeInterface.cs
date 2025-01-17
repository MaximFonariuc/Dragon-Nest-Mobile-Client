using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Assets.SDK
{
	public static class JoyYouNativeInterface
	{
#if UNITY_ANDROID
		private const string IStatisticalData_native_objname = "__IStatisticalData";
		private const string IHuanleSDK_native_objname = "__ICommonSDKPlatform";
		private const string IDeviceInfo_native_objname = "__IDeviceInfomation";
		private const string IGameRecord_native_objname = "__IGameRecord";
		private const string IAdvertisement_native_objname = "__IAdvertisement";

		private const string _ITF_EX_ShareTimeLine = "__ShareTimeline__";
		private const string _ITF_EX_ShareSdkInit = "__ShareSdkInit__";
#endif

		private static string defaultAdvContentMSG = "";

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
		private static extern void U3D_showLoginViewWithType(int type);

		[DllImport("__Internal")]
		private static extern void U3D_showCenterView();

		[DllImport("__Internal")]
		private static extern void U3D_exchangeGoods(int paramPrice, string paramBillNo, string paramBillTitle, string paramRoleId, int paramZoneId);

		[DllImport("__Internal")]
		private static extern void U3D_logout();

		[DllImport("__Internal")]
		private static extern void U3D_HLRegister(string username, string password, string email);

		[DllImport("__Internal")]
		private static extern void U3D_HLLogin(string username, string password, string param);

		[DllImport("__Internal")]
		private static extern void U3D_HLLogout();

		[DllImport("__Internal")]
		private static extern void U3D_setLogEnable_StatisticalDataItf(bool bEnable);

		[DllImport("__Internal")]
		private static extern void U3D_initAppCPA_StatisticalDataItf(string appId, string channelId);

		[DllImport("__Internal")]
		private static extern void U3D_onRegister_StatisticalDataItf(string userId);

		[DllImport("__Internal")]
		private static extern void U3D_onLogin_StatisticalDataItf(string userId);

		[DllImport("__Internal")]
		private static extern void U3D_onPay_StatisticalDataItf(string userId, string orderId, int amount, string currency);

		[DllImport("__Internal")]
		private static extern void U3D_initStatisticalGame_StatisticalDataItf(string appId, string partnerId);

		[DllImport("__Internal")]
		private static extern bool U3D_isStandaloneGame_StatisticalDataItf();

		[DllImport("__Internal")]
		private static extern void U3D_setStandaloneGame_StatisticalDataItf(bool isSG);

		[DllImport("__Internal")]
		private static extern void U3D_initAccount_StatisticalDataItf(string accountId);

		[DllImport("__Internal")]
		private static extern void U3D_setAccountType_StatisticalDataItf(int type);

		[DllImport("__Internal")]
		private static extern void U3D_setAccountName_StatisticalDataItf(string name);

		[DllImport("__Internal")]
		private static extern void U3D_setAccountLevel_StatisticalDataItf(int level);

		[DllImport("__Internal")]
		private static extern void U3D_setAccountGameServer_StatisticalDataItf(string gameServer);

		[DllImport("__Internal")]
		private static extern void U3D_setAccountGender_StatisticalDataItf(int gender);

		[DllImport("__Internal")]
		private static extern void U3D_setAccountAge_StatisticalDataItf(int age);

		[DllImport("__Internal")]
		private static extern void U3D_accountPay_StatisticalDataItf(
			string messageId
			, string status
			, string accountID
			, string orderID
			, double currencyAmount
			, string currencyType
			, double virtualCurrencyAmount
			, long chargeTime
			, string iapID
			, string paymentType
			, string gameServer
			, string gameVersion
			, int level
			, string mission
			);

		[DllImport("__Internal")]
		private static extern void U3D_onAccountPurchase_StatisticalDataItf(string item, int itemNumber, double priceInVirtualCurrency);

		[DllImport("__Internal")]
		private static extern void U3D_onAccountUse_StatisticalDataItf(string item, int itemNumber);

		[DllImport("__Internal")]
		private static extern void U3D_onAccountMissionBegin_StatisticalDataItf(string missionId);

		[DllImport("__Internal")]
		private static extern void U3D_onAccountMissionCompleted_StatisticalDataItf(string missionId);

		[DllImport("__Internal")]
		private static extern void U3D_onAccountMissionFailed_StatisticalDataItf(string missionId, string cause);

		[DllImport("__Internal")]
		private static extern void U3D_onAccountCurrencyReward_StatisticalDataItf(double virtualCurrencyAmount, string reason);

		/*
		[DllImport("__Internal")]
		private static extern void U3D_WXShareLink(string url, string title, string description, string image,int scene);

		[DllImport("__Internal")]
		private static extern void U3D_WXShareRegister(string appId, string appSecrect);

		[DllImport("__Internal")]
		private static extern void U3D_TencentOpenAPIRegister(string appId, string appKey);
		*/

		[DllImport("__Internal")]
		private static extern void U3D_ShareItfInit(int type, string jsonData);

		[DllImport("__Internal")]
		private static extern void U3D_BaiduStat (int type, string name, string label, int time);
		
		[DllImport("__Internal")]
		private static extern void U3D_AdvInit(string propertyId);

		[DllImport("__Internal")]
		private static extern void U3D_CreateBanner(int x, int y, int width, int height, int enumSize, string content);

		[DllImport("__Internal")]
		private static extern void U3D_BannerRefresh(int second);

		[DllImport("__Internal")]
		private static extern void U3D_RemoveBanner ();

		[DllImport("__Internal")]
		private static extern void U3D_GetAdvIDFA();

		[DllImport("__Internal")]
		private static extern void U3D_InitGameRecordItf(string appKey, string _params);

		[DllImport("__Internal")]
		private static extern void U3D_GameRecordItf_PauseRecording();

		[DllImport("__Internal")]
		private static extern void U3D_GameRecordItf_ResumeRecording();

		[DllImport("__Internal")]
		private static extern void U3D_GameRecordItf_StartRecording();

		[DllImport("__Internal")]
		private static extern void U3D_GameRecordItf_StopRecording();

		[DllImport("__Internal")]
		private static extern void U3D_GameRecordItf_ShowWelfareCenter();

		[DllImport("__Internal")]
		private static extern void U3D_GameRecordItf_ShowVideoStore();

		[DllImport("__Internal")]
		private static extern void U3D_GameRecordItf_ShowPlayerClub();

		[DllImport("__Internal")]
		private static extern void U3D_GameRecordItf_ShowControlBar(bool visible);

		[DllImport("__Internal")]
		private static extern void U3D_HLResendAppstoreReceiptDataForRole(string roleId);

		[DllImport("__Internal")]
		private static extern void U3D_SendGameExtData(string type, string content);

		[DllImport("__Internal")]
		private static extern bool U3D_CheckStatus(string type, string content);

		[DllImport("__Internal")]
		private static extern IntPtr U3D_GetSDKConfig(string type, string content);

		[DllImport("__Internal")]
		private static extern void U3D_ShowFloatToolkit(bool visible, double x, double y);

		[DllImport("__Internal")]
		private static extern void U3D_ShareTimeline(int type, string title, string description, string imageURL);
#else
		private static void U3D_initSDK(int appId,
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
			AndroidInvoke(IHuanleSDK_native_objname, "Init",
					appId, appKey, logEnable,
					isLongConnect, rechargeEnable, rechargeAmount, closeRechargeAlertMessage, paramSendMsgNotiClass,
					isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown);
#endif
		}

		private static void U3D_showLoginView()
		{
#if UNITY_ANDROID
			AndroidInvoke(IHuanleSDK_native_objname, "Login", null, "", "");
#endif
		}

		private static void U3D_showLoginViewWithType(int type)
		{
#if UNITY_ANDROID
			AndroidInvoke(IHuanleSDK_native_objname, "Login", type);
#endif
		}

		private static void U3D_showCenterView()
		{
#if UNITY_ANDROID
			AndroidInvoke(IHuanleSDK_native_objname, "ShowUserCentered");
#endif
		}

		private static void U3D_exchangeGoods(int paramPrice, string paramBillNo, string paramBillTitle, string paramRoleId, int paramZoneId)
		{
#if UNITY_ANDROID
			AndroidInvoke(IHuanleSDK_native_objname, "PayGoods", paramPrice, paramBillNo, paramBillTitle, paramRoleId, paramZoneId);
#endif
		}

		private static void U3D_logout()
		{
#if UNITY_ANDROID
			AndroidInvoke(IHuanleSDK_native_objname, "Logout");
#endif
		}
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
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
				U3D_initSDK(appId,
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
							isOriPortraitUpsideDown
					);
			}
			else
			{
				JoyYouInterfaceSimulator.NotificationObjeName = notificationObjectName;
			}
		}

		public static void ShowLoginView()
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
				U3D_showLoginView();
			}
			else
			{
				JoyYouInterfaceSimulator.ShowLoginView();
			}
		}

		public static void ShowLoginViewWithType(int type)
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
				U3D_showLoginViewWithType(type);
			}
			else
			{
				JoyYouInterfaceSimulator.ShowLoginViewWithType(type);
			}
		}

		public static void ShowCenterView()
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
				U3D_showCenterView();
			}
			else
			{
				JoyYouInterfaceSimulator.ShowCenterView();
			}
		}

		public static void Logout()
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
				U3D_logout();
			}
			else
			{
				JoyYouInterfaceSimulator.Logout();
			}
		}

		public static void ExchangeGoods(int price, string billNo, string billTitle, string roleId, int zoneId)
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
				U3D_exchangeGoods(price, billNo, billTitle, roleId, zoneId);
			}
			else
			{
				JoyYouInterfaceSimulator.Pay();
			}
		}

		public static void SendGameExtData(string type, string jsonData)
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				AndroidInvoke(IHuanleSDK_native_objname, "SendGameExtData", type, jsonData);
#elif UNITY_IPHONE
				U3D_SendGameExtData(type, jsonData);
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.SendGameExtData(type, jsonData);
			}
		}

		public static bool CheckStatus(string type, string jsonData)
		{
			bool flag = true;

			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				flag = AndroidInvoke<bool>(IHuanleSDK_native_objname, "CheckStatus", type, jsonData);
#elif UNITY_IPHONE
				flag = U3D_CheckStatus(type, jsonData);
#endif
			}
			else
			{
				flag = JoyYouInterfaceSimulator.CheckStatus(type, jsonData);
			}

			return flag;
		}

		public static string GetSDKConfig(string type, string jsonData)
		{
			string defaultValue = string.Empty;

			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				defaultValue = AndroidInvoke<string>(IHuanleSDK_native_objname, "GetSDKConfig", type, jsonData);
#elif UNITY_IPHONE
				defaultValue = Marshal.PtrToStringAnsi(U3D_GetSDKConfig(type, jsonData));
#endif
			}
			else
			{
				defaultValue = JoyYouInterfaceSimulator.GetSDKConfig(type, jsonData);
			}

			return defaultValue;
		}

		public static void ShareTimeline(int type, string title, string description, string imageURI)
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				string format = "{{ \"type\":{0}, \"title\":\"{1}\", \"description\":\"{2}\", \"imageURI\":\"{3}\" }}";
				string jsonData = string.Format(format, type, title, description, imageURI);
				SendGameExtData(_ITF_EX_ShareTimeLine, jsonData);
#elif UNITY_IPHONE
			U3D_ShareTimeline(type, title, description, imageURI);
#endif
			}
		}

        public static void ShowFloatToolkit(bool visible, double x, double y)
        {
            if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
            {
#if UNITY_ANDROID
                AndroidInvoke(IHuanleSDK_native_objname, "ShowFloatToolkit", visible, x, y);
#elif UNITY_IPHONE
				U3D_ShowFloatToolkit(visible, x, y);
#endif
			}
        }

		public static void QuitGame(string paramString)
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				AndroidInvoke(IHuanleSDK_native_objname, "Release");
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.QuitGame(paramString);
			}
		}

		public static void HLRegister(string username, string password, string email)
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				AndroidInvoke(IHuanleSDK_native_objname, "Register", username, password, email);
#elif UNITY_IPHONE
				U3D_HLRegister(username, password, email);
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.onRegister();
			}
		}

		public static void HLLogin(string username, string password)
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				AndroidInvoke(IHuanleSDK_native_objname, "Login", username, password, "");
#elif UNITY_IPHONE
				U3D_HLLogin(username, password, "");
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.ShowLoginView();
			}
		}

		public static void HLLogout()
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				AndroidInvoke(IHuanleSDK_native_objname, "Logout");
#elif UNITY_IPHONE
				U3D_HLLogout();
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.Logout();
			}
		}

		public static void setLogEnable_StatisticalDataItf(bool bEnable)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "setLogEnable", bEnable);
#elif UNITY_IPHONE
			U3D_setLogEnable_StatisticalDataItf(bEnable);
#endif
		}

		//cpa part
		/* userid 建议30字符以内 */
		public static void initAppCPA(string appId, string channelId)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "initAppCPA", appId, channelId);
#elif UNITY_IPHONE
			U3D_initAppCPA_StatisticalDataItf(appId, channelId);
#endif
		}

		public static void onRegister(string userId)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "onRegister", userId);
#elif UNITY_IPHONE
			U3D_onRegister_StatisticalDataItf(userId);
#endif
		}

		public static void onLogin(string userId)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "onLogin", userId);
#elif UNITY_IPHONE
			U3D_onLogin_StatisticalDataItf(userId);
#endif
		}

		/* 目前支持的货币种类有: 人民币 CNY, 港币 HKD, 台币 TWD, 美元 USD, 欧元 EUR, 英镑 GBP, 日元 JPY */
		public static void onPay(string userId, string orderId, int amount,/*费用*/ string currency/*币种*/)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "onPay", userId, orderId, amount, currency);
#elif UNITY_IPHONE
			U3D_onPay_StatisticalDataItf(userId, orderId, amount, currency);
#endif
		}

		// game part
		public static void initStatisticalGame(string appId, string partnerId)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "initStatisticalGame", appId, partnerId);
#elif UNITY_IPHONE
			U3D_initStatisticalGame_StatisticalDataItf(appId, partnerId);
#endif
		}

		// void onGameResume(Activity page);
		// void onGamePause(Activity page);
		// 注册后调用
		// 判断是否单机游戏
		public static bool isStandaloneGame()
		{
#if UNITY_ANDROID
			return AndroidInvoke<bool>(IStatisticalData_native_objname, "isStandaloneGame");
#elif UNITY_IPHONE
			return U3D_isStandaloneGame_StatisticalDataItf();		
#else
            return false;
#endif
        }

        public static void setStandaloneGame(bool isSG)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "setStandaloneGame", isSG);
#elif UNITY_IPHONE
			U3D_setStandaloneGame_StatisticalDataItf(isSG);
#endif
		}

		// 初始化角色信息（accountId为服务器为角色分配的uuid）
		public static void initAccount(string accountId)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "initAccount", accountId);
#elif UNITY_IPHONE
			U3D_initAccount_StatisticalDataItf(accountId);
#endif
		}

		// 设置角色账户类型
		public static void setAccountType(GameAccountType type)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "setAccountTypeByString", type.ToString());
#elif UNITY_IPHONE
			U3D_setAccountType_StatisticalDataItf((int)type);
#endif
		}

		// 设置账户&角色的显性名 (注册名)
		public static void setAccountName(string name)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "setAccountName", name);
#elif UNITY_IPHONE
			U3D_setAccountName_StatisticalDataItf(name);
#endif
		}

		// 角色等级改变时调用
		public static void setAccountLevel(int level)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "setAccountLevel", level);
#elif UNITY_IPHONE
			U3D_setAccountLevel_StatisticalDataItf(level);
#endif
		}

		// 设置角色区服
		public static void setAccountGameServer(string gameServer)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "setAccountGameServer", gameServer);
#elif UNITY_IPHONE
			U3D_setAccountGameServer_StatisticalDataItf(gameServer);
#endif
		}

		// 设置性别
		public static void setAccountGender(GameGender gender)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "setAccountGenderByString", gender.ToString());
#elif UNITY_IPHONE
			U3D_setAccountGender_StatisticalDataItf((int)gender);
#endif
		}

		// 设置年龄
		public static void setAccountAge(int age)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "setAccountAge", age);
#elif UNITY_IPHONE
			U3D_setAccountAge_StatisticalDataItf(age);
#endif
		}

		public static void accountPay(
			string messageId
			, string status
			, string accountID
			, string orderID
			, double currencyAmount
			, string currencyType
			, double virtualCurrencyAmount
			, long chargeTime
			, string iapID
			, string paymentType
			, string gameServer
			, string gameVersion
			, int level
			, string mission
			)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "accountPay", messageId, status, accountID, orderID, currencyAmount,
				currencyType, virtualCurrencyAmount, chargeTime, iapID, paymentType, gameServer, gameVersion, level, mission);
#elif UNITY_IPHONE
			U3D_accountPay_StatisticalDataItf(messageId, status, accountID, orderID, currencyAmount, currencyType, virtualCurrencyAmount,
				chargeTime, iapID, paymentType, gameServer, gameVersion, level, mission);
#endif
		}

		// 跟踪游戏消费点
		// 记录付费点
		public static void onAccountPurchase(
			string item						// 某个消费点的编号，最多32字符
			, int itemNumber				// 消费数量
			, double priceInVirtualCurrency	// 虚拟币单价
		)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "onAccountPurchase", item, itemNumber, priceInVirtualCurrency);
#elif UNITY_IPHONE
			U3D_onAccountPurchase_StatisticalDataItf(item, itemNumber, priceInVirtualCurrency);
#endif
		}

		// 消耗物品或服务等
		public static void onAccountUse(
			string item						// 参数定义同上
			, int itemNumber
		)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "onAccountUse", item, itemNumber);
#elif UNITY_IPHONE
			U3D_onAccountUse_StatisticalDataItf(item, itemNumber);
#endif
		}

		// 任务统计
		public static void onAccountMissionBegin(string missionId)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "onAccountMissionBegin", missionId);
#elif UNITY_IPHONE
			U3D_onAccountMissionBegin_StatisticalDataItf(missionId);
#endif
		}

		public static void onAccountMissionCompleted(string missionId)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "onAccountMissionCompleted", missionId);
#elif UNITY_IPHONE
			U3D_onAccountMissionCompleted_StatisticalDataItf(missionId);
#endif
		}

		public static void onAccountMissionFailed(string missionId, string cause)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "onAccountMissionFailed", missionId, cause);
#elif UNITY_IPHONE
			U3D_onAccountMissionFailed_StatisticalDataItf(missionId, cause);
#endif
		}

		// 跟踪获赠的虚拟币
		// 赠予虚拟币
		public static void onAccountCurrencyReward(double virtualCurrencyAmount, string reason)
		{
#if UNITY_ANDROID
			AndroidInvoke(IStatisticalData_native_objname, "onAccountCurrencyReward", virtualCurrencyAmount, reason);
#elif UNITY_IPHONE
			U3D_onAccountCurrencyReward_StatisticalDataItf(virtualCurrencyAmount, reason);
#endif
		}

        private static void AndroidInvoke ( string _itf_obj_name , string method , params object[] args )
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
							string initItfMethodName = getItfInitMethodName(_itf_obj_name);
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

#if UNITY_ANDROID
		private static string getItfInitMethodName(string _itf_obj_name)
		{
			string initItfMethodName =
								_itf_obj_name == IHuanleSDK_native_objname ? "initInstance4BridgedCommonSDKPlatform" :
								_itf_obj_name == IStatisticalData_native_objname ? "initInstance4Statistical" :
								_itf_obj_name == IDeviceInfo_native_objname ? "initInstance4DeviceInfomation" :
								_itf_obj_name == IGameRecord_native_objname ? "initInstance4GameRecord" :
								_itf_obj_name == IAdvertisement_native_objname ? "initInstance4Advertisement" :
								"ERROR_ITF_NAME"
								;
			return initItfMethodName;
		}
#endif

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
							string initItfMethodName = getItfInitMethodName(_itf_obj_name);
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

		/*
		public static void WXShareLink(string url, string title, string description, string image, int scene)
		{
#if UNITY_ANDROID
#elif UNITY_IPHONE
			U3D_WXShareLink(url, title, description, image, scene);
#endif
		}

		public static void WXShareRegister(string appId, string appSecrect)
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
#elif UNITY_IPHONE
				U3D_WXShareRegister(appId, appSecrect);
#endif
			}
		}

		public static void TencentOpenAPIRegister(string appId, string appKey)
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
#elif UNITY_IPHONE
				U3D_TencentOpenAPIRegister(appId, appKey);
#endif
			}
		}
		*/

		public static void ShareSdkInit(int type, string jsonData)
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				string format = "{{ \"type\":{0}, \"data\":{1} }}";
				string newJsonData = string.Format(format, type, jsonData);
				SendGameExtData(_ITF_EX_ShareSdkInit, newJsonData);
#elif UNITY_IPHONE
				U3D_ShareItfInit(type, jsonData);
#endif
			}
		}

		public static void BaiduStat(int type, string name, string label, int time)
		{
#if UNITY_ANDROID
#elif UNITY_IPHONE
			U3D_BaiduStat(type, name, label, time);
#endif
		}

		public static void initAdv(string propertyId, string defaultContentId, bool logEnable)
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_IPHONE
				JoyYouNativeInterface.defaultAdvContentMSG = defaultContentId;
				U3D_AdvInit (propertyId);
#elif UNITY_ANDROID
				AndroidInvoke(IAdvertisement_native_objname, "Init", propertyId);
				if (logEnable)
				{
					AndroidInvoke(IAdvertisement_native_objname, "SetLogLevelByString", "L_VERBOSE");
				}
				else
				{
					AndroidInvoke(IAdvertisement_native_objname, "SetLogLevelByString", "L_NONE");
				}
#endif
			}
		}

		public static void AdvCreateBanner(int x, int y, int width, int height, ADV_SIZE size, string contentId)
		{
			if (contentId == "" || contentId == null)
			{
				contentId = defaultAdvContentMSG;
			}
#if UNITY_IPHONE
			U3D_CreateBanner(x, y, width, height, (int)size, contentId);
#elif UNITY_ANDROID
			AndroidInvoke(IAdvertisement_native_objname, "CreateBanner", x, y, width, height, size.ToString(), contentId);
#endif
		}

		public static void AdvBannerRefresh(int sec)
		{
#if UNITY_IPHONE
			U3D_BannerRefresh(sec);
#elif UNITY_ANDROID
			AndroidInvoke(IAdvertisement_native_objname, "BannerRefresh", sec);
#endif
		}

		public static void AdvRemoveBanner()
		{
#if UNITY_IPHONE
			U3D_RemoveBanner();
#elif UNITY_ANDROID
			AndroidInvoke(IAdvertisement_native_objname, "RemoveBanner");
#endif
		}

		public static void getAdvIDFA()
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_IPHONE
				U3D_GetAdvIDFA();
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.getAdvIDFA();
			}
		}

		public static string GetMACAddress()
		{
#if UNITY_ANDROID
			string s = AndroidInvoke<string>(IDeviceInfo_native_objname, "getMACAddress");
			return s;
#else
			return "";
#endif
		}
		
		public static void RequestRealUserRegister(string uid, bool IsQuery)
		{
#if UNITY_ANDROID
			AndroidInvoke(IHuanleSDK_native_objname, "RequestRealUserRegister", uid, IsQuery);
#else
			return ;
#endif
		}

		public static void InitGameRecordItf(string appKey, string _params)
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				AndroidInvoke(IGameRecord_native_objname, "Init", appKey, _params);
#elif UNITY_IPHONE
				U3D_InitGameRecordItf(appKey, _params);
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.InitGameRecordItf(appKey, _params);
			}
		}
		
		public static void GameRecordItf_ShowCtrlBar(bool visible)
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				AndroidInvoke(IGameRecord_native_objname, "ShowControlBar", visible);
#elif UNITY_IPHONE
				U3D_GameRecordItf_ShowControlBar(visible);
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.GameRecordItf_ShowCtrlBar(visible);
			}
		}

		public static void GameRecordItf_PauseRecording()
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				AndroidInvoke(IGameRecord_native_objname, "PauseRecording");
#elif UNITY_IPHONE
				U3D_GameRecordItf_PauseRecording();
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.GameRecordItf_PauseRecording();
			}
		}
		
		public static void GameRecordItf_ResumeRecording()
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				AndroidInvoke(IGameRecord_native_objname, "ResumeRecording");
#elif UNITY_IPHONE
				U3D_GameRecordItf_ResumeRecording();
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.GameRecordItf_ResumeRecording();
			}
		}

		public static void GameRecordItf_StartRecording()
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				AndroidInvoke(IGameRecord_native_objname, "StartRecording");
#elif UNITY_IPHONE
				U3D_GameRecordItf_StartRecording();
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.GameRecordItf_StartRecording();
			}
		}

		public static void GameRecordItf_StopRecording()
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				AndroidInvoke(IGameRecord_native_objname, "StopRecording");
#elif UNITY_IPHONE
				U3D_GameRecordItf_StopRecording();
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.GameRecordItf_StopRecording();
			}
		}

		public static void GameRecordItf_ShowWelfareCenter()
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				AndroidInvoke(IGameRecord_native_objname, "ShowWelfareCenter");
#elif UNITY_IPHONE
				U3D_GameRecordItf_ShowWelfareCenter();
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.GameRecordItf_ShowCoinWebView();
			}
		}
		
		public static void GameRecordItf_ShowVideoStore()
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				AndroidInvoke(IGameRecord_native_objname, "ShowVideoStore");
#elif UNITY_IPHONE
				U3D_GameRecordItf_ShowVideoStore();
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.GameRecordItf_ShowRecordLibraryView();
			}
		}

		public static void GameRecordItf_ShowPlayerClub()
		{
			if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
#if UNITY_ANDROID
				AndroidInvoke(IGameRecord_native_objname, "ShowPlayerClub");
#elif UNITY_IPHONE
				U3D_GameRecordItf_ShowPlayerClub();
#endif
			}
			else
			{
				JoyYouInterfaceSimulator.GameRecordItf_ShowPlayerClub();
			}
		}

		public static void HLResendAppstoreReceiptDataForRole(string roleId)
		{
#if UNITY_ANDROID

			SendGameExtData(roleId, "");

#elif UNITY_IPHONE

			U3D_HLResendAppstoreReceiptDataForRole(roleId);

#endif
		}
	}
}
