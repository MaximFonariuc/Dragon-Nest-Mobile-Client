using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidLenovoAttribute : JoyYouSDKAttribute
	{
		public InitAndroidLenovoAttribute(
			string appId
			, string payKey
			, string notifyObjName
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
			)
			: base(0, CreateParams(appId, payKey), notifyObjName, true, true, 0, "", isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}

		private static string CreateParams(
			string appId
			, string payKey)
		{
			ParamsCollector cl = new ParamsCollector();
			cl.AddItemPair("appId", appId);
			cl.AddItemPair("payKey", payKey);
			return cl.GetJsonData();
		}

		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_LENOVO_ANDROID; }
		}
	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_LENOVO_ANDROID = "__Android_Lenovo__";
	}

}
