using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidSingaporeMalaysiaAttribute : JoyYouSDKAttribute
	{
		public InitAndroidSingaporeMalaysiaAttribute(
			int gameId
			, string appSecret
			, string lang
			, string notifyObjName
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
			)
			: base(gameId, appSecret, notifyObjName, true, true, 0, lang, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}
		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_SM_ANDROID; }
		}
	}
	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_SM_ANDROID = "__Android_Singapore&Malaysia__";
	}
}
