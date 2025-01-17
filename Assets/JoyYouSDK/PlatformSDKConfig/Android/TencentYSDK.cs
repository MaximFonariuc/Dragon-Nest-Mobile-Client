																																							
using System;											
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidTencentYSDKAttribute : JoyYouSDKAttribute
	{
		public InitAndroidTencentYSDKAttribute(
			string qqAppId
			, string qqAppKey
			, string wxAppId
			, string wxAppKey
			, string msdkKey
			, string notifyObjName
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
			)
			: base(0
			, CreateParams(qqAppId, qqAppKey, wxAppId, wxAppKey, msdkKey)
			, notifyObjName
			, true, true, 0, null
			, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}

		private static string CreateParams(
			string qqAppId
			, string qqAppKey
			, string wxAppId
			, string wxAppKey
			, string msdkKey
			)
		{
			ParamsCollector pcl = new ParamsCollector();
			pcl.AddItemPair("qqAppId", qqAppId);
			pcl.AddItemPair("qqAppKey", qqAppKey);
			pcl.AddItemPair("wxAppId", wxAppId);
			pcl.AddItemPair("wxAppKey", wxAppKey);
			pcl.AddItemPair("msdkKey", msdkKey);
			return pcl.GetJsonData();
		}

		public override string NAME
		{
			get
			{
				return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_TENCENT_YSDK_ANDROID;
			}
		}
	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_TENCENT_YSDK_ANDROID = "__Android_TencentYSDK__";
	}

}

