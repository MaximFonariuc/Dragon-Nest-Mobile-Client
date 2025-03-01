using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitQQSDKParamAttribute : JoyYouComponentAttribute
	{
		public string AppId { get; private set; }
		public string AppKey { get; private set; }

		public InitQQSDKParamAttribute(string appId, string appKey)
		{
			this.AppId = appId;
			this.AppKey = appKey;
		}

		public override void DoInit ()
		{
			base.DoInit ();
			string dataFormat = "{{ \"appId\":\"{0}\", \"appKey\":\"{1}\" }}";
			string initData = string.Format(dataFormat, AppId, AppKey);
			JoyYouNativeInterface.ShareSdkInit((int)SHARE_TYPE.SHARE_QQ, initData);
		}
		
	}
}
