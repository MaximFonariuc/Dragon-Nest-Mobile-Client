namespace Assets.SDK
{
	public partial class JoyYouSDK : IShareDirector
	{
		/*
		void ITencentWX.WXShareLink(string url, string title, string description, string image, int scene)
		{
			JoyYouNativeInterface.WXShareLink(url, title, description, image, scene);
		}

		void ITencentWX.WXShareRegister(string appId)
		{
			JoyYouNativeInterface.WXShareRegister(appId);
		}
		*/

		void IShareDirector.ShareTimeline(SHARE_TYPE type, string title, string description, string imageURI)
		{
			JoyYouNativeInterface.ShareTimeline((int)type, title, description, imageURI);
		}
	}
}
