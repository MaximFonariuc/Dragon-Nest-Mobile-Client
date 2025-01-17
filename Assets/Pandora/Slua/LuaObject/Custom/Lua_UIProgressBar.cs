using System;
using com.tencent.pandora;
using System.Collections.Generic;
public class Lua_UIProgressBar : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UIProgressBar o;
			o=new UIProgressBar();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ForceUpdate(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			self.ForceUpdate();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetDynamicGround(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			System.Single a1;
			checkType(l,2,out a1);
			System.Int32 a2;
			checkType(l,3,out a2);
			self.SetDynamicGround(a1,a2);
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
			pushValue(l,UIProgressBar.current);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_current(IntPtr l) {
		try {
			UIProgressBar v;
			checkType(l,2,out v);
			UIProgressBar.current=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_bHideThumbAtEnds(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.bHideThumbAtEnds);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_bHideThumbAtEnds(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.bHideThumbAtEnds=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onDragFinished(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			UIProgressBar.OnDragFinished v;
			int op=LuaDelegation.checkDelegate(l,2,out v);
			if(op==0) self.onDragFinished=v;
			else if(op==1) self.onDragFinished+=v;
			else if(op==2) self.onDragFinished-=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_bHideFgAtEnds(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.bHideFgAtEnds);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_bHideFgAtEnds(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.bHideFgAtEnds=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_UseFillDir(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.UseFillDir);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_UseFillDir(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.UseFillDir=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_thumb(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.thumb);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_thumb(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			UnityEngine.Transform v;
			checkType(l,2,out v);
			self.thumb=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_mBG(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.mBG);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_mBG(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			UIWidget v;
			checkType(l,2,out v);
			self.mBG=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_mFG(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.mFG);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_mFG(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			UIWidget v;
			checkType(l,2,out v);
			self.mFG=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_mDG(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.mDG);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_mDG(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			UIWidget v;
			checkType(l,2,out v);
			self.mDG=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_numberOfSteps(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.numberOfSteps);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_numberOfSteps(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			System.Int32 v;
			checkType(l,2,out v);
			self.numberOfSteps=v;
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
			UIProgressBar self=(UIProgressBar)checkSelf(l);
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
			UIProgressBar self=(UIProgressBar)checkSelf(l);
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
	static public int get_cachedTransform(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.cachedTransform);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_cachedCamera(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.cachedCamera);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_foregroundWidget(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.foregroundWidget);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_foregroundWidget(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			UIWidget v;
			checkType(l,2,out v);
			self.foregroundWidget=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_backgroundWidget(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.backgroundWidget);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_backgroundWidget(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			UIWidget v;
			checkType(l,2,out v);
			self.backgroundWidget=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_fillDirection(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.fillDirection);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_fillDirection(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			UIProgressBar.FillDirection v;
			checkEnum(l,2,out v);
			self.fillDirection=v;
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
			UIProgressBar self=(UIProgressBar)checkSelf(l);
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
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.value=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_alpha(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.alpha);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_alpha(IntPtr l) {
		try {
			UIProgressBar self=(UIProgressBar)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.alpha=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UIProgressBar");
		addMember(l,ForceUpdate);
		addMember(l,SetDynamicGround);
		addMember(l,"current",get_current,set_current,false);
		addMember(l,"bHideThumbAtEnds",get_bHideThumbAtEnds,set_bHideThumbAtEnds,true);
		addMember(l,"onDragFinished",null,set_onDragFinished,true);
		addMember(l,"bHideFgAtEnds",get_bHideFgAtEnds,set_bHideFgAtEnds,true);
		addMember(l,"UseFillDir",get_UseFillDir,set_UseFillDir,true);
		addMember(l,"thumb",get_thumb,set_thumb,true);
		addMember(l,"mBG",get_mBG,set_mBG,true);
		addMember(l,"mFG",get_mFG,set_mFG,true);
		addMember(l,"mDG",get_mDG,set_mDG,true);
		addMember(l,"numberOfSteps",get_numberOfSteps,set_numberOfSteps,true);
		addMember(l,"onChange",get_onChange,set_onChange,true);
		addMember(l,"cachedTransform",get_cachedTransform,null,true);
		addMember(l,"cachedCamera",get_cachedCamera,null,true);
		addMember(l,"foregroundWidget",get_foregroundWidget,set_foregroundWidget,true);
		addMember(l,"backgroundWidget",get_backgroundWidget,set_backgroundWidget,true);
		addMember(l,"fillDirection",get_fillDirection,set_fillDirection,true);
		addMember(l,"value",get_value,set_value,true);
		addMember(l,"alpha",get_alpha,set_alpha,true);
		createTypeMetatable(l,constructor, typeof(UIProgressBar),typeof(UIWidgetContainer));
	}
}
