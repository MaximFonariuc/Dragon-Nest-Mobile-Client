using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Assets.SDK;
namespace Assets.JoyYouSDK.NativeImpl.IOS
{/*
	public class AdvertisementImpl4IOS : IAdvertisement
	{
		private string mDefaultContentId = "";
#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void U3D_AdvInit(string propertyId);

		[DllImport("__Internal")]
		private static extern void U3D_CreateBanner(int x, int y, int width, int height, int enumSize, string content);

		[DllImport("__Internal")]
		private static extern void U3D_BannerRefresh(int second);

		[DllImport("__Internal")]
		private static extern void U3D_RemoveBanner();

		[DllImport("__Internal")]
		private static extern void U3D_GetAdvIDFA();
#endif
		public AdvertisementImpl4IOS(string propertyId, string defaultContentId, bool logEnable)
		{
			mDefaultContentId = defaultContentId;
#if UNITY_IPHONE
			U3D_AdvInit (propertyId);
#endif
		}

		void IAdvertisement.CreateBanner(int x, int y, int width, int height, ADV_SIZE size, string contentId)
		{
#if UNITY_IPHONE
			if (contentId == null || contentId.Length == 0)
			{
				contentId = mDefaultContentId;
			}
			U3D_CreateBanner(x, y, width, height, (int)size, contentId);
#endif
		}
		void IAdvertisement.BannerRefresh(int second)
		{
#if UNITY_IPHONE
			U3D_BannerRefresh(second);
#endif
		}
		void IAdvertisement.RemoveBanner()
		{
#if UNITY_IPHONE
			U3D_RemoveBanner();
#endif
		}
		void IAdvertisement.getAdvIDFA()
		{
#if UNITY_IPHONE
			U3D_GetAdvIDFA();
#endif
		}
	}*/
}
