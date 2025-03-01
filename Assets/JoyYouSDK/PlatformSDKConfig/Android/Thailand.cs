using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidThailandAttribute : JoyYouSDKAttribute
	{
		public InitAndroidThailandAttribute(
				string serverId
				, string notifyObjName
				, bool isOriPortrait
				, bool isOriLandscapeLeft
				, bool isOriLandscapeRight
				, bool isOriPortraitUpsideDown
				, bool logEnable
			)
			: base(0, "", notifyObjName, true, true, 0, serverId, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{
		}
		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_THAILAND_ANDROID; }
		}
	}
	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_THAILAND_ANDROID = "__Android_Thailand__";
	}
}
