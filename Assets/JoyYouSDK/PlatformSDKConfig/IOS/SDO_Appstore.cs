using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitSDOAppleAttribute : JoyYouSDKAttribute
	{
		public InitSDOAppleAttribute(
			int appId
			, string notifyObjName
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
		)
			: base(
				  appId
				  , null
				  , notifyObjName
				  , true
				  , true
				  , 0
				  , null
				  , isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown
				  , logEnable)
		{

		}

		public override void InitSDK()
		{
			Type t = typeof(JoyYouSDK);
			InitSDOPushAttribute attr = t.GetCustomAttribute<InitSDOPushAttribute>(false);
			if (attr != null)
			{
				base.closeRechargeAlertMsg = attr.AppId + ";" + attr.AppKey;

			}
			/*
			InitBuglyComponentAttribute comp = t.GetCustomAttribute<InitBuglyComponentAttribute>(false);
			if (comp != null)
			{
				base.closeRechargeAlertMsg += ";" + comp.BuglyAppId ;
			}
			Debug.Log(base.closeRechargeAlertMsg);*/
			base.InitSDK();
		}

		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_SDO_APPLE; }
		}
	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_SDO_APPLE = "__SDO_APPLE__";
	}
}
