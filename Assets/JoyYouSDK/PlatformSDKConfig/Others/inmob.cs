using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAdvertisementAttribute : JoyYouSDKAttribute
	{
		public InitAdvertisementAttribute(
			string notiObjname
			, string propertyId
			, string defaultContentId
			, bool logEnable)
			: base(0, propertyId, notiObjname, true, true, 0, defaultContentId, false, false, false, false, logEnable)
		{

		}

		public override void InitSDK()
		{
			string propertyId = this.appKey;
			string defaultContentId = this.closeRechargeAlertMsg;
			bool logEnable = this.logEnable;
			JoyYouNativeInterface.initAdv(propertyId, defaultContentId, logEnable);
		}
		public override string NAME
		{
			get { return "__NONE_ADV__DEFAULT__"; }
		}
	}
}
