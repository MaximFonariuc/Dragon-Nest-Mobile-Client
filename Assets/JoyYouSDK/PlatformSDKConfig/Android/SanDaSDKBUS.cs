using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidSDKBUSSDAttribute : JoyYouSDKAttribute
	{
		public InitAndroidSDKBUSSDAttribute(
			int appId
			//, string appKey
			, string notifyObjName
			, string url4cb
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
			)
			: base(appId, null, notifyObjName, true, true, 0, url4cb, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}
		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_SDKBUSSD_ANDROID; }
		}
	}
	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_SDKBUSSD_ANDROID = "__AndroidSDKBUSSD__";
	}
}
