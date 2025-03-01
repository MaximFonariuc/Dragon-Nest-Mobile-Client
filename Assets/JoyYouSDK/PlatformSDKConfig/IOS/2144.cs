using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class Init2144Attribute : JoyYouSDKAttribute
	{
		public Init2144Attribute(
			int gameID
			, string appId
			, string notifyObjName
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
		) : base(
			gameID
			, appId
			, notifyObjName
			, true
			, true
			, 0
			, null
			, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight ,isOriPortraitUpsideDown
			, logEnable	)
		{
			
		}

		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_2144; }
		}
	}
	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_2144 = "__2144__";
	}
}
