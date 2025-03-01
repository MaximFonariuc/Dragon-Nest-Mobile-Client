using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidUCAttribute : JoyYouSDKAttribute
	{
		public InitAndroidUCAttribute(
			int cpId//appId
			, int uc_game_id
			, int serverId
			, bool useStandardUI
			, string notiObjname
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable)
			: base(cpId, uc_game_id.ToString(), notiObjname, true, useStandardUI, serverId, "",
					isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}
		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_UC_ANDROID; }
		}
	}
	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_UC_ANDROID = "__Android_UC__";
	}
}
