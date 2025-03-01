using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitThailandAttribute : JoyYouSDKAttribute
	{
		private static string createParams(
			string productId
			, string location
			, string serverId
		   )
		{
			string json = "{{\n {0} \n}}";
			Dictionary<string, string> paramsDict = new Dictionary<string, string>();
			paramsDict.Add("productId", productId);
			paramsDict.Add("location", location);

			paramsDict.Add("serverId", serverId);

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
		public InitThailandAttribute(
				string productId
				, string location
				, string serverId
				, string notifyObjName
				, bool isOriPortrait
				, bool isOriLandscapeLeft
				, bool isOriLandscapeRight
				, bool isOriPortraitUpsideDown
				, bool logEnable
			)
			: base(0,
				createParams(productId, location, serverId)
				, notifyObjName, true, true, 0, "", isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{
		}

		public override string NAME
		{
			get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_THAILAND; }
		}
	}
	public sealed partial class JoyYouSDKPlatformFilterAttribute
	{
		public const string PLATFORM_NAME_THAILAND = "__Thailand__";
	}
}
