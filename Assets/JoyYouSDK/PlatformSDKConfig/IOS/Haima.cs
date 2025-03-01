using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitHaimaSDKParamAttribute : JoyYouSDKAttribute
	{
		public InitHaimaSDKParamAttribute(
			string appId,
			string appVKey,
			string cpKey,
			string channelId,
			string noficationObjectName,
			bool isOriPortrait,
			bool isOriLandscapeLeft,
			bool isOriLandscapeRight,
			bool isOriPortraitUpsideDown,
			bool logEnable)
			: base(0, appId + ";" + appVKey + ";" + cpKey, noficationObjectName, true, true, 0, channelId,
				   isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}
		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_HAIMA; }
		}
	}
	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_HAIMA = "__Haima__";
	}
}
