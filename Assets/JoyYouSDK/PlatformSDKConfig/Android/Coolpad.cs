using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidCoolpadAttribute : JoyYouSDKAttribute
	{
		public InitAndroidCoolpadAttribute(
			string appId  // application key
			, string orderServerRequset
			, string notifyObjName
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
			)
			: base(0, CreateParams(appId, orderServerRequset), notifyObjName, true, true, 0, null, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}

		private static string CreateParams(
			string appId
			, string orderServerRequset)
		{
			ParamsCollector pcl = new ParamsCollector();
			pcl.AddItemPair("appId",appId);
			pcl.AddItemPair("orderServerRequset", orderServerRequset);
			return pcl.GetJsonData();
		}

		public override string NAME
		{
			get
			{
				return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_COOLPAD_ANDROID;
			}
		}
	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_COOLPAD_ANDROID = "__Android_Coolpad__";
	}

}
