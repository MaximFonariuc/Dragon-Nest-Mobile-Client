using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroidKoreanAttribute : JoyYouSDKAttribute
	{
		private static string createParams(
			string oneStoreId
			, string onestoreVerifyURL

			, string naverClientId
			, string naverClientSecret
			, string naverClientName
			, string naverCallbackIntentUrl

			, string naverIapKey
			, string naverVerifyURL

			, string googleClientId
			, string googleIapKey
			, string googleVerifyURL

			)
		{
			string json = "{{\n {0} \n}}";
			Dictionary<string, string> paramsDict = new Dictionary<string, string>();
			paramsDict.Add("OneStoreId", oneStoreId);
			paramsDict.Add("OneStoreVerifyURL", onestoreVerifyURL);

			paramsDict.Add("NaverClientId", naverClientId);
			paramsDict.Add("NaverClientSecret", naverClientSecret);
			paramsDict.Add("NaverClientName", naverClientName);
			paramsDict.Add("NaverCallbackIntentUrl", naverCallbackIntentUrl);

			paramsDict.Add("NaverIapKey", naverIapKey);
			paramsDict.Add("NaverVerifyURL", naverVerifyURL);

			paramsDict.Add("GoogleClientID", googleClientId);
			paramsDict.Add("GoogleIapKey", googleIapKey);
			paramsDict.Add("GoogleVerifyURL", googleVerifyURL);

			StringBuilder sb = new StringBuilder();
			int index = 0;
			string token = "\"";
			string split = ":";
			foreach (KeyValuePair<string, string> pair in paramsDict)
			{
				index++;
				sb.Append(token).Append(pair.Key).Append(token).Append(split).Append(token).Append(pair.Value).Append(token);
				if (index < paramsDict.Count)
				{
					sb.Append(",\n");
				}
			}
			json = string.Format(json, sb.ToString());
			//Debug.Log(json.ToString());
			return json;
		}

		public InitAndroidKoreanAttribute(
			string oneStoreId
			, string onestoreVerifyURL

			, string naverClientId
			, string naverClientSecret
			, string naverClientName
			, string naverCallbackIntentUrl

			, string naverIapKey
			, string naverVerifyURL

			, string googleClientId
			, string googleIapKey
			, string googleVerifyURL

			, string iapStyle

			, string notifyObjName
			, bool isOriPortrait
			, bool isOriLandscapeLeft
			, bool isOriLandscapeRight
			, bool isOriPortraitUpsideDown
			, bool logEnable
			)
			: base(0
			, InitAndroidKoreanAttribute.createParams(
				oneStoreId
				, onestoreVerifyURL

				, naverClientId
				, naverClientSecret
				, naverClientName
				, naverCallbackIntentUrl

				, naverIapKey
				, naverVerifyURL

				, googleClientId
				, googleIapKey
				, googleVerifyURL)
			, notifyObjName
			, true, true, 0, iapStyle
			, isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown
			, logEnable)
		{

		}

		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_KOREAN_ANDROID; }
		}
	}


	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_KOREAN_ANDROID = "__Android_Korean__";
	}
}
