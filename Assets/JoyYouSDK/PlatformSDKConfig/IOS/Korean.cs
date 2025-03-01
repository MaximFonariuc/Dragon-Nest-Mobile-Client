using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitKoreanAttribute : JoyYouSDKAttribute
	{
		private static string createParams(

			 string naverClientId
			, string naverClientSecret
			, string naverClientName
			, string naverCallbackIntentUrl

			, string googleClientId

			)
		{
			string json = "{{\n {0} \n}}";
			Dictionary<string, string> paramsDict = new Dictionary<string, string>();
			paramsDict.Add("NaverClientId", naverClientId);
			paramsDict.Add("NaverClientSecret", naverClientSecret);
			paramsDict.Add("NaverClientName", naverClientName);
			paramsDict.Add("NaverCallbackIntentUrl", naverCallbackIntentUrl);

			paramsDict.Add("GoogleClientId", googleClientId);


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
		public InitKoreanAttribute(
				int gameId
				, string naverClientId
				, string naverClientSecret
				, string naverClientName
				, string naverCallbackIntentUrl
				, string googleClientId

				, string iapPaymentCB

				, string notifyObjName
				, bool isOriPortrait
				, bool isOriLandscapeLeft
				, bool isOriLandscapeRight
				, bool isOriPortraitUpsideDown
				, bool logEnable
		)
			: base(gameId, createParams(
							   naverClientId
							   , naverClientSecret
							   , naverClientName
							   , naverCallbackIntentUrl
							   , googleClientId)
				  , notifyObjName
				  , true, true, 0
				  , iapPaymentCB
				  , isOriPortrait
				  , isOriLandscapeLeft
				  , isOriLandscapeRight
				  , isOriPortraitUpsideDown, logEnable)
		{
		}

		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_KOREAN; }
		}

	}

	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_KOREAN = "__Korean__";
	}
}
