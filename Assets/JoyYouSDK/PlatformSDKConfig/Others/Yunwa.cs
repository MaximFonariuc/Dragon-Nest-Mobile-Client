using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitYunwaSDKParamAttribute : JoyYouComponentAttribute
	{
		public string appId { get; set; }
		public string appSecret {get;set;}
		public InitYunwaSDKParamAttribute()
		{

		}

		public override void DoInit ()
		{
			base.DoInit ();

		}
	}
}
