﻿using System;
using com.tencent.pandora;
using System.Collections.Generic;
public class Lua_UIToggle : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UIToggle o;
			o=new UIToggle();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ForceSetActive(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			System.Boolean a1;
			checkType(l,2,out a1);
			self.ForceSetActive(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetActiveToggle_s(IntPtr l) {
		try {
			System.Int32 a1;
			checkType(l,1,out a1);
			var ret=UIToggle.GetActiveToggle(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_list(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UIToggle.list);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_list(IntPtr l) {
		try {
			BetterList<UIToggle> v;
			checkType(l,2,out v);
			UIToggle.list=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_current(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UIToggle.current);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_current(IntPtr l) {
		try {
			UIToggle v;
			checkType(l,2,out v);
			UIToggle.current=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_group(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.group);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_group(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			System.Int32 v;
			checkType(l,2,out v);
			self.group=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_activeSprite(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.activeSprite);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_activeSprite(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			UIWidget v;
			checkType(l,2,out v);
			self.activeSprite=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_activeSprite2(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.activeSprite2);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_activeSprite2(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			UIWidget v;
			checkType(l,2,out v);
			self.activeSprite2=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_activeAnimation(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.activeAnimation);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_activeAnimation(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			UnityEngine.Animation v;
			checkType(l,2,out v);
			self.activeAnimation=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_startsActive(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.startsActive);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_startsActive(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.startsActive=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_instantTween(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.instantTween);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_instantTween(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.instantTween=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_optionCanBeNone(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.optionCanBeNone);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_optionCanBeNone(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.optionCanBeNone=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onChange(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.onChange);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onChange(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			System.Collections.Generic.List<EventDelegate> v;
			checkType(l,2,out v);
			self.onChange=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_value(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.value);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_value(IntPtr l) {
		try {
			UIToggle self=(UIToggle)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.value=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UIToggle");
		addMember(l,ForceSetActive);
		addMember(l,GetActiveToggle_s);
		addMember(l,"list",get_list,set_list,false);
		addMember(l,"current",get_current,set_current,false);
		addMember(l,"group",get_group,set_group,true);
		addMember(l,"activeSprite",get_activeSprite,set_activeSprite,true);
		addMember(l,"activeSprite2",get_activeSprite2,set_activeSprite2,true);
		addMember(l,"activeAnimation",get_activeAnimation,set_activeAnimation,true);
		addMember(l,"startsActive",get_startsActive,set_startsActive,true);
		addMember(l,"instantTween",get_instantTween,set_instantTween,true);
		addMember(l,"optionCanBeNone",get_optionCanBeNone,set_optionCanBeNone,true);
		addMember(l,"onChange",get_onChange,set_onChange,true);
		addMember(l,"value",get_value,set_value,true);
		createTypeMetatable(l,constructor, typeof(UIToggle),typeof(UIWidgetContainer));
	}
}
