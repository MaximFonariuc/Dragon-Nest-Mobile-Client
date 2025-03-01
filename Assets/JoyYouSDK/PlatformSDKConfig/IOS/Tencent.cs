using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitTencentAttribute : JoyYouSDKAttribute
	{
		public InitTencentAttribute(
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
			: base(0, qqAppId + "__JOYYOU__" + qqAppKey + "__JOYYOU__" + weixinAppId + "__JOYYOU__" + msdkKey + "__JOYYOU__" + midasId, notifyObjName, true, true, 0, url4cb, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{
			this.qqAppId = qqAppId;
			this.qqAppKey = qqAppKey;
			this.weixinAppId = weixinAppId;
			this.midasId = midasId;
			this.msdkKey = msdkKey;
			this.logEnable = logEnable;
		}

		public int qqAppId { get; set; }
		public string qqAppKey { get; set; }
		public string weixinAppId { get; set; }
		public string midasId { get; set; }
		public string msdkKey { get; set; }
        public new bool logEnable { get; private set; }
        public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_TENCENT; }
		}
	}
	
	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_TENCENT = "__Tencent__";
	}
}
