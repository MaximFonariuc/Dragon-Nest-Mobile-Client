﻿using System;
using com.tencent.pandora;
using System.Collections.Generic;
public class Lua_UIButtonColor : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UIButtonColor o;
			o=new UIButtonColor();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetState(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			UIButtonColor.State a1;
			checkEnum(l,2,out a1);
			System.Boolean a2;
			checkType(l,3,out a2);
			self.SetState(a1,a2);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_tweenTarget(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.tweenTarget);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_tweenTarget(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			UnityEngine.GameObject v;
			checkType(l,2,out v);
			self.tweenTarget=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_hover(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.hover);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_hover(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			UnityEngine.Color v;
			checkType(l,2,out v);
			self.hover=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_pressed(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.pressed);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_pressed(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			UnityEngine.Color v;
			checkType(l,2,out v);
			self.pressed=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_disabledColor(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.disabledColor);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_disabledColor(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			UnityEngine.Color v;
			checkType(l,2,out v);
			self.disabledColor=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_changeStateSprite(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.changeStateSprite);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_changeStateSprite(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.changeStateSprite=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_duration(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.duration);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_duration(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			System.Single v;
			checkType(l,2,out v);
			self.duration=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_state(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.state);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_state(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			UIButtonColor.State v;
			checkEnum(l,2,out v);
			self.state=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_defaultColor(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.defaultColor);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_defaultColor(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			UnityEngine.Color v;
			checkType(l,2,out v);
			self.defaultColor=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isEnabled(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.isEnabled);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_isEnabled(IntPtr l) {
		try {
			UIButtonColor self=(UIButtonColor)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.isEnabled=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UIButtonColor");
		addMember(l,SetState);
		addMember(l,"tweenTarget",get_tweenTarget,set_tweenTarget,true);
		addMember(l,"hover",get_hover,set_hover,true);
		addMember(l,"pressed",get_pressed,set_pressed,true);
		addMember(l,"disabledColor",get_disabledColor,set_disabledColor,true);
		addMember(l,"changeStateSprite",get_changeStateSprite,set_changeStateSprite,true);
		addMember(l,"duration",get_duration,set_duration,true);
		addMember(l,"state",get_state,set_state,true);
		addMember(l,"defaultColor",get_defaultColor,set_defaultColor,true);
		addMember(l,"isEnabled",get_isEnabled,set_isEnabled,true);
		createTypeMetatable(l,constructor, typeof(UIButtonColor),typeof(UIWidgetContainer));
	}
}
