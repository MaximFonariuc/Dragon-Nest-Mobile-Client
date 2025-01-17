using System;
using com.tencent.pandora;
using System.Collections.Generic;
public class Lua_UIWidgetContainer : LuaObject {
	static public void reg(IntPtr l) {
		getTypeTable(l,"UIWidgetContainer");
		createTypeMetatable(l,null, typeof(UIWidgetContainer),typeof(UnityEngine.MonoBehaviour));
	}
}
