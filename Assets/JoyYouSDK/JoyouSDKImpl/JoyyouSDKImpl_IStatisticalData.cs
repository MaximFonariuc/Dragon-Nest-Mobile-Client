namespace Assets.SDK
{
	public partial class JoyYouSDK : IStatisticalData
	{
		void IStatisticalData.setLogEnable(bool bEnable)
		{
			JoyYouNativeInterface.setLogEnable_StatisticalDataItf(bEnable);
		}

		void IStatisticalData.initAppCPA(string appId, string channelId)
		{
			JoyYouNativeInterface.initAppCPA(appId, channelId);
		}

		void IStatisticalData.onRegister(string userId)
		{
			JoyYouNativeInterface.onRegister(userId);
		}

		void IStatisticalData.onLogin(string userId)
		{
			JoyYouNativeInterface.onLogin(userId);
		}

		void IStatisticalData.onPay(string userId, string orderId, int amount, string currency)
		{
			JoyYouNativeInterface.onPay(userId, orderId, amount, currency);
		}

		void IStatisticalData.initStatisticalGame(string appId, string partnerId)
		{
			JoyYouNativeInterface.initStatisticalGame(appId, partnerId);
		}

		// void onGameResume(Activity page);
		// void onGamePause(Activity page);
		bool IStatisticalData.isStandaloneGame()
		{ 
			return JoyYouNativeInterface.isStandaloneGame();
		}

		void IStatisticalData.setStandaloneGame(bool isSG)
		{
			JoyYouNativeInterface.setStandaloneGame(isSG);
		}

		void IStatisticalData.initAccount(string accountId)
		{
			JoyYouNativeInterface.initAccount(accountId);
		}

		void IStatisticalData.setAccountType(GameAccountType type)
		{
			JoyYouNativeInterface.setAccountType(type);
		}

		void IStatisticalData.setAccountName(string name)
		{
			JoyYouNativeInterface.setAccountName(name);
		}

		void IStatisticalData.setAccountLevel(int level)
		{
			JoyYouNativeInterface.setAccountLevel(level);
		}

		void IStatisticalData.setAccountGameServer(string gameServer)
		{
			JoyYouNativeInterface.setAccountGameServer(gameServer);
		}

		void IStatisticalData.setAccountGender(GameGender gender)
		{
			JoyYouNativeInterface.setAccountGender(gender);
		}

		void IStatisticalData.setAccountAge(int age)
		{
			JoyYouNativeInterface.setAccountAge(age);
		}

		void IStatisticalData.accountPay(
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
			JoyYouNativeInterface.accountPay(messageId, status, accountID, orderID, currencyAmount,
				currencyType, virtualCurrencyAmount, chargeTime, iapID, paymentType, gameServer, gameVersion, level, mission);
		}

		void IStatisticalData.onAccountPurchase(
			string item
			, int itemNumber
			, double priceInVirtualCurrency
		)
		{
			JoyYouNativeInterface.onAccountPurchase(item, itemNumber, priceInVirtualCurrency);
		}

		void IStatisticalData.onAccountUse(
			string item
			, int itemNumber
		)
		{
			JoyYouNativeInterface.onAccountUse(item, itemNumber);
		}

		void IStatisticalData.onAccountMissionBegin(string missionId)
		{
			JoyYouNativeInterface.onAccountMissionBegin(missionId);
		}

		void IStatisticalData.onAccountMissionCompleted(string missionId)
		{
			JoyYouNativeInterface.onAccountMissionCompleted(missionId);
		}

		void IStatisticalData.onAccountMissionFailed(string missionId, string cause)
		{
			JoyYouNativeInterface.onAccountMissionFailed(missionId, cause);
		}

		void IStatisticalData.onAccountCurrencyReward(double virtualCurrencyAmount, string reason)
		{
			JoyYouNativeInterface.onAccountCurrencyReward(virtualCurrencyAmount, reason);
		}
	}
}
