using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidTencentAttribute : JoyYouSDKAttribute
	{
		public InitAndroidTencentAttribute(
			int qqAppId
			, string qqAppKey
			, string weixinAppId
			, string midasId
			, string msdkKey
			, string notifyObjName
			, string url4cb
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
			)
			: base(0, qqAppId + "__JOYYOU__" + qqAppKey + "__JOYYOU__" + weixinAppId + "__JOYYOU__" +  msdkKey + "__JOYYOU__" + midasId, notifyObjName, true, true, 0, url4cb, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}

		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_TENCENT_ANDROID; }
		}
	}
	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_TENCENT_ANDROID = "__Android_Tencent__";
	}
}
