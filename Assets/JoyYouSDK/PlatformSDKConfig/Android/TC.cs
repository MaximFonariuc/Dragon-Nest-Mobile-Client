																																							
using System;											
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidTCAttribute : JoyYouSDKAttribute
	{
		public InitAndroidTCAttribute(
			string notifyObjName
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
			)
			: base(0, null, notifyObjName, true, true, 0, null, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}
		public override string NAME
		{
			get
			{
				return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_TC_ANDROID;
			}
		}
	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_TC_ANDROID = "__Android_TC__";
	}

}

