using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitSDOPushAttribute : JoyYouComponentAttribute
	{
		public string AppId { get; private set; }
		public string AppKey { get; private set; }
		public InitSDOPushAttribute(string appId, string appKey) { AppId = appId; AppKey = appKey; }
	}

	

}
