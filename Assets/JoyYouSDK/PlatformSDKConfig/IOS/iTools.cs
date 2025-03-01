using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitIToolsSDKParamAttribute : JoyYouSDKAttribute
	{
		public InitIToolsSDKParamAttribute(
			int appId,
			string appKey,
			string noficationObjectName,
			bool isOriPortrait,
			bool isOriLandscapeLeft,
			bool isOriLandscapeRight,
			bool isOriPortraitUpsideDown)
			: base(appId, appKey, noficationObjectName, true, true, 100, "",
			isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, false)
		{

		}
		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_ITOOLS; }
		}
	}
	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_ITOOLS = "__iTools__";
	}
}
