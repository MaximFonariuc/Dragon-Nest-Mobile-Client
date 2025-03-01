using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SDK
{
	public sealed class InitNaverComponentAttribute : JoyYouComponentAttribute
	{
		public string URLScheme { get; private set; }
		public InitNaverComponentAttribute() { URLScheme = ""; }
		public InitNaverComponentAttribute(string urlscheme) { URLScheme = urlscheme; }
	}

}
