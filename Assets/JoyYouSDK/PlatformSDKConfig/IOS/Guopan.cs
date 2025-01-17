																																							
using System;											
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitGuopanAttribute : JoyYouSDKAttribute
	{
		public InitGuopanAttribute(
			string appId
			, string appKey
			, string notifyObjName
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
			)
			: base(0, appKey, notifyObjName, true, true, 0, appId, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}
		public override string NAME
		{
			get
			{
				return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_GUOPAN;
			}
		}
	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_GUOPAN = "__Guopan__";
	}

}

