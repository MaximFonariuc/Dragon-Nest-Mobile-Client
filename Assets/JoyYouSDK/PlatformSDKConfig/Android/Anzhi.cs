using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidAnzhiAttribute : JoyYouSDKAttribute
	{
		public InitAndroidAnzhiAttribute(
			string appKey
			, string appSecret
			, string notifyObjName
			, string url4cb
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
			)
			: base(0, appKey + "__JOYYOU__" + appSecret, notifyObjName, true, true, 0, url4cb, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}
		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_ANZHI_ANDROID; }
		}
	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_ANZHI_ANDROID = "__Android_AnZhi__";
	}

}
