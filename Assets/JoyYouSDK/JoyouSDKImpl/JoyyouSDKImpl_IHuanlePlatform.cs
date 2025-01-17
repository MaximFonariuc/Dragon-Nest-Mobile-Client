namespace Assets.SDK
{
	public partial class JoyYouSDK : IHuanlePlatform
	{
		void IHuanlePlatform.HLRegister(string username, string password, string email)
		{
			JoyYouNativeInterface.HLRegister(username, password, email);
		}

		void IHuanlePlatform.HLLogin(string username, string password)
		{
			JoyYouNativeInterface.HLLogin(username, password);
		}

		void IHuanlePlatform.HLLogout()
		{
			JoyYouNativeInterface.HLLogout();
		}

		void IHuanlePlatform.HLResendAppstoreReceiptDataForRole(string roleId)
		{
			JoyYouNativeInterface.HLResendAppstoreReceiptDataForRole(roleId);
		}
	}
}
