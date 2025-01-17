using System;
using com.tencent.pandora;
using System.Collections.Generic;
public class Lua_UISlider : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UISlider o;
			o=new UISlider();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UISlider");
		createTypeMetatable(l,constructor, typeof(UISlider),typeof(UIProgressBar));
	}
}
