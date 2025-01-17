using System;
using com.tencent.pandora;
using System.Collections.Generic;
public class Lua_com_tencent_pandora_Logger : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Log_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			com.tencent.pandora.Logger.Log(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int LogInfo_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			com.tencent.pandora.Logger.LogInfo(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int LogWarning_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			com.tencent.pandora.Logger.LogWarning(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int LogError_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			com.tencent.pandora.Logger.LogError(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_ERROR(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,com.tencent.pandora.Logger.ERROR);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_WARNING(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,com.tencent.pandora.Logger.WARNING);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_INFO(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,com.tencent.pandora.Logger.INFO);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_DEBUG(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,com.tencent.pandora.Logger.DEBUG);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_HandleLog(IntPtr l) {
		try {
			System.Action<System.String,System.String,System.Int32> v;
			int op=LuaDelegation.checkDelegate(l,2,out v);
			if(op==0) com.tencent.pandora.Logger.HandleLog=v;
			else if(op==1) com.tencent.pandora.Logger.HandleLog+=v;
			else if(op==2) com.tencent.pandora.Logger.HandleLog-=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_Enable(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,com.tencent.pandora.Logger.Enable);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_Enable(IntPtr l) {
		try {
			System.Boolean v;
			checkType(l,2,out v);
			com.tencent.pandora.Logger.Enable=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_LogLevel(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,com.tencent.pandora.Logger.LogLevel);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_LogLevel(IntPtr l) {
		try {
			System.Int32 v;
			checkType(l,2,out v);
			com.tencent.pandora.Logger.LogLevel=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"com.tencent.pandora.Logger");
		addMember(l,Log_s);
		addMember(l,LogInfo_s);
		addMember(l,LogWarning_s);
		addMember(l,LogError_s);
		addMember(l,"ERROR",get_ERROR,null,false);
		addMember(l,"WARNING",get_WARNING,null,false);
		addMember(l,"INFO",get_INFO,null,false);
		addMember(l,"DEBUG",get_DEBUG,null,false);
		addMember(l,"HandleLog",null,set_HandleLog,false);
		addMember(l,"Enable",get_Enable,set_Enable,false);
		addMember(l,"LogLevel",get_LogLevel,set_LogLevel,false);
		createTypeMetatable(l,null, typeof(com.tencent.pandora.Logger),typeof(UnityEngine.MonoBehaviour));
	}
}
