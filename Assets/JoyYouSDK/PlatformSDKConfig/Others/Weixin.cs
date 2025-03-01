using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitWXSDKParamAttribute : JoyYouComponentAttribute
	{
		public string appId { get; set; }
		public string appSecret {get;set;}
		public InitWXSDKParamAttribute(string appId, string appSecret)
		{
			this.appId = appId;
			this.appSecret = appSecret;
		}

		public override void DoInit ()
		{
			base.DoInit ();
			string dataFormat = "{{ \"appId\":\"{0}\", \"appSecrect\":\"{1}\" }}";
			string initData = string.Format(dataFormat, appId, appSecret);
			JoyYouNativeInterface.ShareSdkInit((int)SHARE_TYPE.SHARE_WEIXIN, initData);
		}
	}
}
