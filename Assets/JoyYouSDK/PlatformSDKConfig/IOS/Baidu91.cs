using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitBaidu91SDKParamAttribute : JoyYouSDKAttribute
	{
		public InitBaidu91SDKParamAttribute(
			int appId,
			string appKey,
			string noficationObjectName,
			bool isOriPortrait,
			bool isOriLandscapeLeft,
			bool isOriLandscapeRight,
			bool isOriPortraitUpsideDown,
			bool logEnable)
			: base(appId, appKey, noficationObjectName, true, true, 0, "",
				   isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}

		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_BAIDU91; }
		}
	}
	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_BAIDU91 = "__Baidu91__";
	}
}
