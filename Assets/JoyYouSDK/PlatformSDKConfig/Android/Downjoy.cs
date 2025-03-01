using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidDownjoyAttribute : JoyYouSDKAttribute
	{
		public InitAndroidDownjoyAttribute(
			int appId
			, string appKey
			, int merchantId
			, int serverSeqNum
			, string notifyObjName
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
			)
			: base(appId, appKey + "__JOYYOU__" + merchantId.ToString() + "__JOYYOU__" + serverSeqNum.ToString()
			, notifyObjName, true, true, 0, null, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}

		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_DOWNJOY_ANDROID; }
		}
	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_DOWNJOY_ANDROID = "__Android_Downjoy__";
	}

}
