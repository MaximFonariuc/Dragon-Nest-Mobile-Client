using System;
using com.tencent.pandora;
using System.Collections.Generic;
public class Lua_UITextureRGBA32 : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UITextureRGBA32 o;
			o=new UITextureRGBA32();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UITextureRGBA32");
		createTypeMetatable(l,constructor, typeof(UITextureRGBA32),typeof(UITexture));
	}
}
