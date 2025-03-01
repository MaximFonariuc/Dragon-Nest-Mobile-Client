using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitFacebookComponentAttribute : JoyYouComponentAttribute
	{
		public string FB_ID { get; private set; }
		public string FB_NAME { get; private set; }
		public InitFacebookComponentAttribute(
			string facebookId
			, string displayName
		)
			: this(facebookId, displayName, true)
		{
		}

		public InitFacebookComponentAttribute(string facebookId, string displayName, bool isStaticLoad)
		{
			this.FB_ID = facebookId;
			this.FB_NAME = displayName;
			this.isStaticLoad = isStaticLoad;
		}
	}

}
