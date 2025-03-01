using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitDownjoyAttribute : JoyYouSDKAttribute
	{
		public InitDownjoyAttribute(
			int appId
			, string appKey
			, int merchantId
			, int serverId
			, string notiObjname
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable)
			: base(appId, appKey, notiObjname, true, true, merchantId, serverId.ToString(),
					isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}

		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_DOWNJOY; }
		}
	}
	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_DOWNJOY = "__Downjoy__";
	}
}
