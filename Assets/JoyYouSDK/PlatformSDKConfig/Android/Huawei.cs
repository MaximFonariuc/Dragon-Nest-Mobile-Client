using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidHuaweiAttribute : JoyYouSDKAttribute
	{
		public InitAndroidHuaweiAttribute(
			string appId
			, string cpId
			, string payId
			, string fubiaoPrivateKey
			, string payPublicKey
			, string payPrivateKey
			, string companyName
			, string noficationObjectName,
			bool isOriPortrait,
			bool isOriLandscapeLeft,
			bool isOriLandscapeRight,
			bool isOriPortraitUpsideDown,
			bool logEnable)
			: base(0, paramsData(appId, cpId, payId, fubiaoPrivateKey, payPublicKey, payPrivateKey, companyName), noficationObjectName, true, true, 0, "",
				   isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}

		private static string paramsData(
			string appId
			, string cpId
			, string payId
			, string fubiaoPrivateKey
			, string payPublicKey
			, string payPrivateKey
			, string companyName
			)
		{
			ParamsCollector cl = new ParamsCollector();
			cl.AddItemPair("appId", appId);
			cl.AddItemPair("cpId", cpId);
			cl.AddItemPair("payId", payId);
			cl.AddItemPair("fubiaoPrivateKey", fubiaoPrivateKey);
			cl.AddItemPair("payPublicKey", payPublicKey);
			cl.AddItemPair("payPrivateKey", payPrivateKey);
			cl.AddItemPair("companyName", companyName);
			return cl.GetJsonData();
		}

		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_HUAWEI_ANDROID; }
		}
	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_HUAWEI_ANDROID = "__Android_Huawei__";
	}
}
