using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitRecNowComponentAttribute : JoyYouComponentAttribute
	{
		public string theAppKey { get; private set; }
		public string theWeixinKey { get; private set; }
		public string theQQZoneKey { get; private set; }

		public InitRecNowComponentAttribute(string appKey, string weixinKey, string qqZoneKey, bool initInUse)
		{
			theAppKey = appKey;
			theWeixinKey = weixinKey;
			theQQZoneKey = qqZoneKey;
			isDelayInit = initInUse;
		}

		public InitRecNowComponentAttribute(string appKey, string weixinKey, string qqZoneKey)
			: this(appKey, weixinKey, qqZoneKey, true)
		{
		}

		public override void DoInit()
		{
			base.DoInit();
			JoyYouNativeInterface.InitGameRecordItf(theAppKey, theWeixinKey + ";" + theQQZoneKey);
		}
	}

}
