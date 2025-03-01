using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitGiantAttribute : JoyYouSDKAttribute
	{
		public InitGiantAttribute(
			int appId
			, string appName
			, string notifyObjName
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
			)
			: base(appId, appName, notifyObjName, true, true, 0, null, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}

		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_GIANT; }
		}
	}
	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_GIANT = "__GIANT__";
	}
}
