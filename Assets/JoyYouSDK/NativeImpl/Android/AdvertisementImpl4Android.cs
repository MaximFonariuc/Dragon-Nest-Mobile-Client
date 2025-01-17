using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.SDK;
namespace Assets.JoyYouSDK.NativeImpl.Android
{
	public class AdvertisementImpl4Android : IAdvertisement
	{
		private string mDefaultContentId = "";
		public AdvertisementImpl4Android(string propertyId, string defaultContentId, bool logEnable)
		{
			mDefaultContentId = defaultContentId;
#if UNITY_ANDROID
#endif
		}

		void IAdvertisement.CreateBanner(int x, int y, int width, int height, ADV_SIZE size, string content)
		{
#if UNITY_ANDROID
#endif
		}
		void IAdvertisement.BannerRefresh(int second)
		{
#if UNITY_ANDROID
#endif
		}
		void IAdvertisement.RemoveBanner()
		{
#if UNITY_ANDROID
#endif
		}
		void IAdvertisement.getAdvIDFA()
		{
#if UNITY_ANDROID
#endif
		}
	}
}
