using System;
using System.Collections.Generic;
namespace com.tencent.pandora {
	[LuaBinder(3)]
	public class BindCustom {
		public static Action<IntPtr>[] GetBindList() {
			Action<IntPtr>[] list= {
				Lua_com_tencent_pandora_CSharpInterface.reg,
				Lua_com_tencent_pandora_Logger.reg,
				Lua_UIWidgetContainer.reg,
				Lua_UIButtonColor.reg,
				Lua_UIButton.reg,
				Lua_UIGrid.reg,
				Lua_UIPlayTween.reg,
				Lua_UIPopupList.reg,
				Lua_UIProgressBar.reg,
				Lua_UIScrollView.reg,
				Lua_UISlider.reg,
				Lua_UIToggle.reg,
				Lua_UITweener.reg,
				Lua_UIAnchor.reg,
				Lua_UIAtlas.reg,
				Lua_UIInput.reg,
				Lua_UIRect.reg,
				Lua_UIWidget.reg,
				Lua_UILabel.reg,
				Lua_UISprite.reg,
				Lua_UITexture.reg,
				Lua_UITextureRGBA32.reg,
				Lua_EventDelegate.reg,
				Lua_UIEventListener.reg,
				Lua_NGUITools.reg,
			};
			return list;
		}
	}
}
