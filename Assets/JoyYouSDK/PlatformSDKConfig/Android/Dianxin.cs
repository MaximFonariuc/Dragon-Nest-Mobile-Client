using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidDianXinAttribute : JoyYouSDKAttribute
	{
		public InitAndroidDianXinAttribute(
			string appkey
			, string gamename
			, string notifyObjName
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
			)
			: base(0, appkey, notifyObjName, true, true, 0, gamename, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}
		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_DIANXIN_ANDROID; }
		}
	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_DIANXIN_ANDROID = "__Android_DianXin__";
	}

}
