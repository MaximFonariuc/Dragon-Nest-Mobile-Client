using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitBuglyComponentAttribute : JoyYouComponentAttribute
	{
		public string BuglyAppId {get; set;}
		public InitBuglyComponentAttribute(string appId) 
		{
			this.BuglyAppId = appId;
		}
	}
}
