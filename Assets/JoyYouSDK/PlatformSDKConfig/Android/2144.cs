using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroid2144Attribute : JoyYouSDKAttribute
	{
		public InitAndroid2144Attribute(
			string appKey
			, string appSecret
			, string notifyObjName
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
		)
			: base(
				  0
				  , appKey
				  , notifyObjName
				  , true
				  , true
				  , 0
				  , appSecret
				  , isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown
				  , logEnable)
		{
		}
		public override string NAME
		{
			get
			{
				return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_2144_ANDROID;
			}
		}
	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_2144_ANDROID = "__Android_2144__";
	}

}
