using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitTongbutuiSDKParamAttribute : JoyYouSDKAttribute
	{
		public InitTongbutuiSDKParamAttribute(
			int appId,
			string appKey,
			string noficationObjectName,
			bool isLongConnect,
			bool rechargeEnable,
			int rechargeAmount,
			string closeRechargeAlertMsg,
			bool isOriPortrait,
			bool isOriLandscapeLeft,
			bool isOriLandscapeRight,
			bool isOriPortraitUpsideDown,
			bool logEnable)
			: base(appId, appKey, noficationObjectName, isLongConnect, rechargeEnable, rechargeAmount, closeRechargeAlertMsg,
				   isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}

		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_TONGBU; }
		}
	}
	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_TONGBU = "__Tongbutui__";
	}
}
