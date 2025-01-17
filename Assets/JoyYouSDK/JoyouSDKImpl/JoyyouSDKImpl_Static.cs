namespace Assets.SDK
{
	public partial class JoyYouSDK
	{
		static bool isInitialised = false;

		static JoyYouSDK()
		{
			SDKParams.Parse(typeof(JoyYouSDK));
		}
	}
}
