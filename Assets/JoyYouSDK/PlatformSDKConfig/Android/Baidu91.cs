using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidBaidu91Attribute : JoyYouSDKAttribute
	{
		public InitAndroidBaidu91Attribute(
			int appId
			, string appKey
			, string notifyObjName
			, string url4cb
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
			)
			: base(appId, appKey, notifyObjName, true, true, 0, url4cb, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}
		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_BAIDU91_ANDROID; }
		}
	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_BAIDU91_ANDROID = "__Android_Baidu91__";
	}

}
