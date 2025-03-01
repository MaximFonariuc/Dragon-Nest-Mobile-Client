using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidGioneeAttribute : JoyYouSDKAttribute
	{

		public InitAndroidGioneeAttribute(

			string apiKey
			, string url4createOrder
			, string notifyObjName

			, bool isOriPortrait

			, bool isOriLandscapeLeft

			, bool isOriLandscapeRight

			, bool isOriPortraitUpsideDown

			, bool logEnable

			)

			: base(0, apiKey, notifyObjName, true, true, 0, url4createOrder, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{



		}

		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_GIONEE_ANDROID; }
		}

	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_GIONEE_ANDROID = "__Android_Gionee__";
	}

}
