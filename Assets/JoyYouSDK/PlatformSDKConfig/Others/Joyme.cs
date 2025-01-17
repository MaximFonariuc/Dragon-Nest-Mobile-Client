using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitJoymeComponentAttribute : JoyYouComponentAttribute
	{
		public string theAppKey       { get; private set; }
		public string theAppSecret    { get; private set; }
		public string theRazorKey     { get; private set; }
		// public int    theQQAppId      { get; private set; }
		public string theQQAppKey     { get; private set; }
		public string theWeixinAppId  { get; private set; }
		public string theWeixinAppKey { get; private set; }
		public string theWeiboAppKey  { get; private set; }

		public bool logEnable         { get; private set; }
		public bool debugEnable       { get; private set; }

		public InitJoymeComponentAttribute(
			string appKey
			, string appSecret
			, string razorKey
			// , int    qqAppId
			, string qqAppKey
			, string weixinAppId
			, string weixinAppKey
			, string weiboAppKey
			, bool   debugEnable
			, bool   logEnable
			, bool   initInUse)
		{
			theAppKey        = appKey;
			theAppSecret     = appSecret;
			theRazorKey      = razorKey;
			// this.theQQAppId  = qqAppId;
			theQQAppKey      = qqAppKey;
			theWeixinAppId   = weixinAppId;
			theWeixinAppKey  = weixinAppKey;
			theWeiboAppKey   = weiboAppKey;

			this.logEnable   = logEnable;
			this.debugEnable = debugEnable;

			isDelayInit      = initInUse;
		}

		public InitJoymeComponentAttribute(string appKey, string appSecret, string razorKey,/* int qqAppId, */ string qqAppKey, string weixinAppId, string weixinAppKey, string weiboAppKey, bool debugEnable, bool logEnable)
			: this(appKey, appSecret, razorKey,/* qqAppId, */ qqAppKey, weixinAppId, weixinAppKey, weiboAppKey, debugEnable, logEnable, true)
		{
		}

		public override void DoInit()
		{
			base.DoInit();

			JoyYouSDKAttribute.ParamsCollector cl = new JoyYouSDKAttribute.ParamsCollector();
			cl.AddItemPair("appKey", theAppKey);
			cl.AddItemPair("appSecret", theAppSecret);
			cl.AddItemPair("razorKey", theRazorKey);
			// cl.AddItemPair("qqAppId", theQQAppId);
			cl.AddItemPair("qqAppKey", theQQAppKey);
			cl.AddItemPair("weixinAppId", theWeixinAppId);
			cl.AddItemPair("weixinAppKey", theWeixinAppKey);
			cl.AddItemPair("weiboAppKey", theWeiboAppKey);
			cl.AddItemPair("=ITF=", this.GetType().Name);
			cl.AddItemPair("debug", debugEnable ? 1 : 0);
			cl.AddItemPair("log", logEnable ? 1 : 0);
			JoyYouNativeInterface.InitGameRecordItf(theAppKey, cl.GetJsonData());
		}
	}
}
