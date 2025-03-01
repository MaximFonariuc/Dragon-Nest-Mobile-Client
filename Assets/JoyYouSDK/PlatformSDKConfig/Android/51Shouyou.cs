using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitAndroid51ShouyouAttribute : JoyYouSDKAttribute
	{
        public InitAndroid51ShouyouAttribute(
            string appKey
            , string appSecret
            , string channelId
            , string notifyObjName
            , bool isOriPortrait
            , bool isOriLandscapeLeft
            , bool isOriLandscapeRight
            , bool isOriPortraitUpsideDown
            , bool logEnable
            )
            : base(0, paramsData(appKey, appSecret, channelId), notifyObjName, true, true, 0, "", isOriPortrait, isOriLandscapeLeft, isOriLandscapeRight, isOriPortraitUpsideDown, logEnable)
		{

		}
        
       private static string paramsData(
            string appKey
            , string appSecret
            , string channelId
            )
        {
            ParamsCollector cl = new ParamsCollector();
            cl.AddItemPair("appKey", appKey);
            cl.AddItemPair("appSecret", appSecret);
            cl.AddItemPair("channelId", channelId);
            return cl.GetJsonData();
        }

        


        public override string NAME
		{
            get { return JoyYouSDKPlatformFilterAttribute.PLATFORM_NAME_51SY_ANDROID; }
		}

	}

    public sealed partial class JoyYouSDKPlatformFilterAttribute
    {
        public const string PLATFORM_NAME_51SY_ANDROID = "__Android_51_Shouyou__";
    }

}
