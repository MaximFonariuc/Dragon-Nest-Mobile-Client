namespace Assets.SDK
{
	public partial class JoyYouSDK : IAdvertisement
	{
		void IAdvertisement.CreateBanner(int x, int y, int width, int height, ADV_SIZE size, string content)
		{
			JoyYouNativeInterface.AdvCreateBanner(x, y, width, height, size, content);
		}

		void IAdvertisement.BannerRefresh(int second)
		{
			JoyYouNativeInterface.AdvBannerRefresh(second);
		}

		void IAdvertisement.RemoveBanner()
		{
			JoyYouNativeInterface.AdvRemoveBanner();
		}

		void IAdvertisement.getAdvIDFA()
		{
			JoyYouNativeInterface.getAdvIDFA();
		}
	}
}