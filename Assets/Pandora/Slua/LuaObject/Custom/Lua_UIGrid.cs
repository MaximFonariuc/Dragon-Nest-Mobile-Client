﻿using System;
using com.tencent.pandora;
using System.Collections.Generic;
public class Lua_UIGrid : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UIGrid o;
			o=new UIGrid();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetChildList(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			var ret=self.GetChildList();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetChild(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			System.Int32 a1;
			checkType(l,2,out a1);
			var ret=self.GetChild(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int AddChild(IntPtr l) {
		try {
			int argc = LuaDLL.pua_gettop(l);
			if(argc==2){
				UIGrid self=(UIGrid)checkSelf(l);
				UnityEngine.Transform a1;
				checkType(l,2,out a1);
				self.AddChild(a1);
				pushValue(l,true);
				return 1;
			}
			else if(matchType(l,argc,2,typeof(UnityEngine.Transform),typeof(int))){
				UIGrid self=(UIGrid)checkSelf(l);
				UnityEngine.Transform a1;
				checkType(l,2,out a1);
				System.Int32 a2;
				checkType(l,3,out a2);
				self.AddChild(a1,a2);
				pushValue(l,true);
				return 1;
			}
			else if(matchType(l,argc,2,typeof(UnityEngine.Transform),typeof(bool))){
				UIGrid self=(UIGrid)checkSelf(l);
				UnityEngine.Transform a1;
				checkType(l,2,out a1);
				System.Boolean a2;
				checkType(l,3,out a2);
				self.AddChild(a1,a2);
				pushValue(l,true);
				return 1;
			}
			pushValue(l,false);
			LuaDLL.pua_pushstring(l,"No matched override function to call");
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int RemoveChild(IntPtr l) {
		try {
			int argc = LuaDLL.pua_gettop(l);
			if(matchType(l,argc,2,typeof(UnityEngine.Transform))){
				UIGrid self=(UIGrid)checkSelf(l);
				UnityEngine.Transform a1;
				checkType(l,2,out a1);
				var ret=self.RemoveChild(a1);
				pushValue(l,true);
				pushValue(l,ret);
				return 2;
			}
			else if(matchType(l,argc,2,typeof(int))){
				UIGrid self=(UIGrid)checkSelf(l);
				System.Int32 a1;
				checkType(l,2,out a1);
				var ret=self.RemoveChild(a1);
				pushValue(l,true);
				pushValue(l,ret);
				return 2;
			}
			pushValue(l,false);
			LuaDLL.pua_pushstring(l,"No matched override function to call");
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Reposition(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			self.Reposition();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ConstrainWithinPanel(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			self.ConstrainWithinPanel();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int CloseList(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			self.CloseList();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SortByName_s(IntPtr l) {
		try {
			UnityEngine.Transform a1;
			checkType(l,1,out a1);
			UnityEngine.Transform a2;
			checkType(l,2,out a2);
			var ret=UIGrid.SortByName(a1,a2);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SortHorizontal_s(IntPtr l) {
		try {
			UnityEngine.Transform a1;
			checkType(l,1,out a1);
			UnityEngine.Transform a2;
			checkType(l,2,out a2);
			var ret=UIGrid.SortHorizontal(a1,a2);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SortVertical_s(IntPtr l) {
		try {
			UnityEngine.Transform a1;
			checkType(l,1,out a1);
			UnityEngine.Transform a2;
			checkType(l,2,out a2);
			var ret=UIGrid.SortVertical(a1,a2);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_arrangement(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.arrangement);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_arrangement(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			UIGrid.Arrangement v;
			checkEnum(l,2,out v);
			self.arrangement=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sorting(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.sorting);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sorting(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			UIGrid.Sorting v;
			checkEnum(l,2,out v);
			self.sorting=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_pivot(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.pivot);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_pivot(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			UIWidget.Pivot v;
			checkEnum(l,2,out v);
			self.pivot=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_reverse(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.reverse);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_reverse(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.reverse=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_maxPerLine(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.maxPerLine);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_maxPerLine(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			System.Int32 v;
			checkType(l,2,out v);
			self.maxPerLine=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_cellWidth(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.cellWidth);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_cellWidth(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			System.Single v;
			checkType(l,2,out v);
			self.cellWidth=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_cellHeight(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.cellHeight);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_cellHeight(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			System.Single v;
			checkType(l,2,out v);
			self.cellHeight=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_animateSmoothly(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.animateSmoothly);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_animateSmoothly(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.animateSmoothly=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_animateSmoothlySpeed(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.animateSmoothlySpeed);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_animateSmoothlySpeed(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			System.Single v;
			checkType(l,2,out v);
			self.animateSmoothlySpeed=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_hideInactive(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.hideInactive);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_hideInactive(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.hideInactive=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_keepWithinPanel(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.keepWithinPanel);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_keepWithinPanel(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.keepWithinPanel=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onReposition(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			UIGrid.OnReposition v;
			int op=LuaDelegation.checkDelegate(l,2,out v);
			if(op==0) self.onReposition=v;
			else if(op==1) self.onReposition+=v;
			else if(op==2) self.onReposition-=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onCustomSort(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			BetterList<UnityEngine.Transform>.CompareFunc v;
			int op=LuaDelegation.checkDelegate(l,2,out v);
			if(op==0) self.onCustomSort=v;
			else if(op==1) self.onCustomSort+=v;
			else if(op==2) self.onCustomSort-=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_repositionNow(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.repositionNow=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_panel(IntPtr l) {
		try {
			UIGrid self=(UIGrid)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.panel);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UIGrid");
		addMember(l,GetChildList);
		addMember(l,GetChild);
		addMember(l,AddChild);
		addMember(l,RemoveChild);
		addMember(l,Reposition);
		addMember(l,ConstrainWithinPanel);
		addMember(l,CloseList);
		addMember(l,SortByName_s);
		addMember(l,SortHorizontal_s);
		addMember(l,SortVertical_s);
		addMember(l,"arrangement",get_arrangement,set_arrangement,true);
		addMember(l,"sorting",get_sorting,set_sorting,true);
		addMember(l,"pivot",get_pivot,set_pivot,true);
		addMember(l,"reverse",get_reverse,set_reverse,true);
		addMember(l,"maxPerLine",get_maxPerLine,set_maxPerLine,true);
		addMember(l,"cellWidth",get_cellWidth,set_cellWidth,true);
		addMember(l,"cellHeight",get_cellHeight,set_cellHeight,true);
		addMember(l,"animateSmoothly",get_animateSmoothly,set_animateSmoothly,true);
		addMember(l,"animateSmoothlySpeed",get_animateSmoothlySpeed,set_animateSmoothlySpeed,true);
		addMember(l,"hideInactive",get_hideInactive,set_hideInactive,true);
		addMember(l,"keepWithinPanel",get_keepWithinPanel,set_keepWithinPanel,true);
		addMember(l,"onReposition",null,set_onReposition,true);
		addMember(l,"onCustomSort",null,set_onCustomSort,true);
		addMember(l,"repositionNow",null,set_repositionNow,true);
		addMember(l,"panel",get_panel,null,true);
		createTypeMetatable(l,constructor, typeof(UIGrid),typeof(UIWidgetContainer));
	}
}
