﻿using System;
using com.tencent.pandora;
using System.Collections.Generic;
public class Lua_UnityEngine_Vector4 : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			int argc = LuaDLL.pua_gettop(l);
			UnityEngine.Vector4 o;
			if(argc==5){
				System.Single a1;
				checkType(l,2,out a1);
				System.Single a2;
				checkType(l,3,out a2);
				System.Single a3;
				checkType(l,4,out a3);
				System.Single a4;
				checkType(l,5,out a4);
				o=new UnityEngine.Vector4(a1,a2,a3,a4);
				pushValue(l,true);
				pushValue(l,o);
				return 2;
			}
			else if(argc==4){
				System.Single a1;
				checkType(l,2,out a1);
				System.Single a2;
				checkType(l,3,out a2);
				System.Single a3;
				checkType(l,4,out a3);
				o=new UnityEngine.Vector4(a1,a2,a3);
				pushValue(l,true);
				pushValue(l,o);
				return 2;
			}
			else if(argc==3){
				System.Single a1;
				checkType(l,2,out a1);
				System.Single a2;
				checkType(l,3,out a2);
				o=new UnityEngine.Vector4(a1,a2);
				pushValue(l,true);
				pushValue(l,o);
				return 2;
			}
			return error(l,"New object failed.");
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SqrMagnitude(IntPtr l) {
		try {
			UnityEngine.Vector4 self;
			checkType(l,1,out self);
			var ret=self.SqrMagnitude();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int LerpUnclamped_s(IntPtr l) {
		try {
			UnityEngine.Vector4 a1;
			checkType(l,1,out a1);
			UnityEngine.Vector4 a2;
			checkType(l,2,out a2);
			System.Single a3;
			checkType(l,3,out a3);
			var ret=UnityEngine.Vector4.LerpUnclamped(a1,a2,a3);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Distance_s(IntPtr l) {
		try {
			UnityEngine.Vector4 a1;
			checkType(l,1,out a1);
			UnityEngine.Vector4 a2;
			checkType(l,2,out a2);
			var ret=UnityEngine.Vector4.Distance(a1,a2);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Magnitude_s(IntPtr l) {
		try {
			UnityEngine.Vector4 a1;
			checkType(l,1,out a1);
			var ret=UnityEngine.Vector4.Magnitude(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Min_s(IntPtr l) {
		try {
			UnityEngine.Vector4 a1;
			checkType(l,1,out a1);
			UnityEngine.Vector4 a2;
			checkType(l,2,out a2);
			var ret=UnityEngine.Vector4.Min(a1,a2);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Max_s(IntPtr l) {
		try {
			UnityEngine.Vector4 a1;
			checkType(l,1,out a1);
			UnityEngine.Vector4 a2;
			checkType(l,2,out a2);
			var ret=UnityEngine.Vector4.Max(a1,a2);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int op_Addition(IntPtr l) {
		try {
			UnityEngine.Vector4 a1;
			checkType(l,1,out a1);
			UnityEngine.Vector4 a2;
			checkType(l,2,out a2);
			var ret=a1+a2;
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int op_Subtraction(IntPtr l) {
		try {
			UnityEngine.Vector4 a1;
			checkType(l,1,out a1);
			UnityEngine.Vector4 a2;
			checkType(l,2,out a2);
			var ret=a1-a2;
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int op_Equality(IntPtr l) {
		try {
			UnityEngine.Vector4 a1;
			checkType(l,1,out a1);
			UnityEngine.Vector4 a2;
			checkType(l,2,out a2);
			var ret=(a1==a2);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SqrMagnitude_s(IntPtr l) {
		try {
			UnityEngine.Vector4 a1;
			checkType(l,1,out a1);
			var ret=UnityEngine.Vector4.SqrMagnitude(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_kEpsilon(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UnityEngine.Vector4.kEpsilon);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_x(IntPtr l) {
		try {
			UnityEngine.Vector4 self;
			checkType(l,1,out self);
			pushValue(l,true);
			pushValue(l,self.x);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_x(IntPtr l) {
		try {
			UnityEngine.Vector4 self;
			checkType(l,1,out self);
			System.Single v;
			checkType(l,2,out v);
			self.x=v;
			setBack(l,self);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_y(IntPtr l) {
		try {
			UnityEngine.Vector4 self;
			checkType(l,1,out self);
			pushValue(l,true);
			pushValue(l,self.y);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_y(IntPtr l) {
		try {
			UnityEngine.Vector4 self;
			checkType(l,1,out self);
			System.Single v;
			checkType(l,2,out v);
			self.y=v;
			setBack(l,self);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_z(IntPtr l) {
		try {
			UnityEngine.Vector4 self;
			checkType(l,1,out self);
			pushValue(l,true);
			pushValue(l,self.z);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_z(IntPtr l) {
		try {
			UnityEngine.Vector4 self;
			checkType(l,1,out self);
			System.Single v;
			checkType(l,2,out v);
			self.z=v;
			setBack(l,self);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_w(IntPtr l) {
		try {
			UnityEngine.Vector4 self;
			checkType(l,1,out self);
			pushValue(l,true);
			pushValue(l,self.w);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_w(IntPtr l) {
		try {
			UnityEngine.Vector4 self;
			checkType(l,1,out self);
			System.Single v;
			checkType(l,2,out v);
			self.w=v;
			setBack(l,self);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_normalized(IntPtr l) {
		try {
			UnityEngine.Vector4 self;
			checkType(l,1,out self);
			pushValue(l,true);
			pushValue(l,self.normalized);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_magnitude(IntPtr l) {
		try {
			UnityEngine.Vector4 self;
			checkType(l,1,out self);
			pushValue(l,true);
			pushValue(l,self.magnitude);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sqrMagnitude(IntPtr l) {
		try {
			UnityEngine.Vector4 self;
			checkType(l,1,out self);
			pushValue(l,true);
			pushValue(l,self.sqrMagnitude);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_zero(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UnityEngine.Vector4.zero);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_one(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UnityEngine.Vector4.one);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int getItem(IntPtr l) {
		try {
			UnityEngine.Vector4 self;
			checkType(l,1,out self);
			int v;
			checkType(l,2,out v);
			var ret = self[v];
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int setItem(IntPtr l) {
		try {
			UnityEngine.Vector4 self;
			checkType(l,1,out self);
			int v;
			checkType(l,2,out v);
			float c;
			checkType(l,3,out c);
			self[v]=c;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.Vector4");
		addMember(l,SqrMagnitude);
		addMember(l,LerpUnclamped_s);
		addMember(l,Distance_s);
		addMember(l,Magnitude_s);
		addMember(l,Min_s);
		addMember(l,Max_s);
		addMember(l,op_Addition);
		addMember(l,op_Subtraction);
		addMember(l,op_Equality);
		addMember(l,SqrMagnitude_s);
		addMember(l,getItem);
		addMember(l,setItem);
		addMember(l,"kEpsilon",get_kEpsilon,null,false);
		addMember(l,"x",get_x,set_x,true);
		addMember(l,"y",get_y,set_y,true);
		addMember(l,"z",get_z,set_z,true);
		addMember(l,"w",get_w,set_w,true);
		addMember(l,"normalized",get_normalized,null,true);
		addMember(l,"magnitude",get_magnitude,null,true);
		addMember(l,"sqrMagnitude",get_sqrMagnitude,null,true);
		addMember(l,"zero",get_zero,null,false);
		addMember(l,"one",get_one,null,false);
		createTypeMetatable(l,constructor, typeof(UnityEngine.Vector4),typeof(System.ValueType));
	}
}
