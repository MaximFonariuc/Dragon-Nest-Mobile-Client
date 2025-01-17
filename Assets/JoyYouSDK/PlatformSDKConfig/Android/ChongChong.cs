																																							
using System;											
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidChongChongAttribute : JoyYouSDKAttribute
	{
		public InitAndroidChongChongAttribute(
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
				return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_CHONGCHONG_ANDROID;
			}
		}
	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_CHONGCHONG_ANDROID = "__Android_ChongChong__";
	}

}

