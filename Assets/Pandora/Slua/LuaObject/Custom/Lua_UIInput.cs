﻿using System;
using com.tencent.pandora;
using System.Collections.Generic;
public class Lua_UIInput : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Validate(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			var ret=self.Validate(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Submit(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			self.Submit();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int UpdateLabel(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			self.UpdateLabel();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int RemoveFocus(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			self.RemoveFocus();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SaveValue(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			self.SaveValue();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int LoadValue(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			self.LoadValue();
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
			pushValue(l,UIInput.current);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_current(IntPtr l) {
		try {
			UIInput v;
			checkType(l,2,out v);
			UIInput.current=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_selection(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UIInput.selection);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_selection(IntPtr l) {
		try {
			UIInput v;
			checkType(l,2,out v);
			UIInput.selection=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_label(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.label);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_label(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			UILabel v;
			checkType(l,2,out v);
			self.label=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_inputType(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.inputType);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_inputType(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			UIInput.InputType v;
			checkEnum(l,2,out v);
			self.inputType=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_keyboardType(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.keyboardType);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_keyboardType(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			UIInput.KeyboardType v;
			checkEnum(l,2,out v);
			self.keyboardType=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_validation(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.validation);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_validation(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			UIInput.Validation v;
			checkEnum(l,2,out v);
			self.validation=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_characterLimit(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.characterLimit);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_characterLimit(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			System.Int32 v;
			checkType(l,2,out v);
			self.characterLimit=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_savedAs(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.savedAs);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_savedAs(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			System.String v;
			checkType(l,2,out v);
			self.savedAs=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_selectOnTab(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.selectOnTab);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_selectOnTab(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			UnityEngine.GameObject v;
			checkType(l,2,out v);
			self.selectOnTab=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_activeTextColor(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.activeTextColor);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_activeTextColor(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			UnityEngine.Color v;
			checkType(l,2,out v);
			self.activeTextColor=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_caretColor(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.caretColor);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_caretColor(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			UnityEngine.Color v;
			checkType(l,2,out v);
			self.caretColor=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_selectionColor(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.selectionColor);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_selectionColor(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			UnityEngine.Color v;
			checkType(l,2,out v);
			self.selectionColor=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onSubmit(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.onSubmit);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onSubmit(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			System.Collections.Generic.List<EventDelegate> v;
			checkType(l,2,out v);
			self.onSubmit=v;
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
			UIInput self=(UIInput)checkSelf(l);
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
			UIInput self=(UIInput)checkSelf(l);
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
	static public int get_onKeyTriggered(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.onKeyTriggered);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onKeyTriggered(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			System.Collections.Generic.List<EventDelegate> v;
			checkType(l,2,out v);
			self.onKeyTriggered=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onValidate(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			UIInput.OnValidate v;
			int op=LuaDelegation.checkDelegate(l,2,out v);
			if(op==0) self.onValidate=v;
			else if(op==1) self.onValidate+=v;
			else if(op==2) self.onValidate-=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_recentKey(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.recentKey);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_recentKey(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			UnityEngine.KeyCode v;
			checkEnum(l,2,out v);
			self.recentKey=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_defaultText(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.defaultText);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_defaultText(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			string v;
			checkType(l,2,out v);
			self.defaultText=v;
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
			UIInput self=(UIInput)checkSelf(l);
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
			UIInput self=(UIInput)checkSelf(l);
			string v;
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
	static public int get_isSelected(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.isSelected);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_isSelected(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.isSelected=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_cursorPosition(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.cursorPosition);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_cursorPosition(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.cursorPosition=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_selectionStart(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.selectionStart);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_selectionStart(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.selectionStart=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_selectionEnd(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.selectionEnd);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_selectionEnd(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.selectionEnd=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_caret(IntPtr l) {
		try {
			UIInput self=(UIInput)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.caret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UIInput");
		addMember(l,Validate);
		addMember(l,Submit);
		addMember(l,UpdateLabel);
		addMember(l,RemoveFocus);
		addMember(l,SaveValue);
		addMember(l,LoadValue);
		addMember(l,"current",get_current,set_current,false);
		addMember(l,"selection",get_selection,set_selection,false);
		addMember(l,"label",get_label,set_label,true);
		addMember(l,"inputType",get_inputType,set_inputType,true);
		addMember(l,"keyboardType",get_keyboardType,set_keyboardType,true);
		addMember(l,"validation",get_validation,set_validation,true);
		addMember(l,"characterLimit",get_characterLimit,set_characterLimit,true);
		addMember(l,"savedAs",get_savedAs,set_savedAs,true);
		addMember(l,"selectOnTab",get_selectOnTab,set_selectOnTab,true);
		addMember(l,"activeTextColor",get_activeTextColor,set_activeTextColor,true);
		addMember(l,"caretColor",get_caretColor,set_caretColor,true);
		addMember(l,"selectionColor",get_selectionColor,set_selectionColor,true);
		addMember(l,"onSubmit",get_onSubmit,set_onSubmit,true);
		addMember(l,"onChange",get_onChange,set_onChange,true);
		addMember(l,"onKeyTriggered",get_onKeyTriggered,set_onKeyTriggered,true);
		addMember(l,"onValidate",null,set_onValidate,true);
		addMember(l,"recentKey",get_recentKey,set_recentKey,true);
		addMember(l,"defaultText",get_defaultText,set_defaultText,true);
		addMember(l,"value",get_value,set_value,true);
		addMember(l,"isSelected",get_isSelected,set_isSelected,true);
		addMember(l,"cursorPosition",get_cursorPosition,set_cursorPosition,true);
		addMember(l,"selectionStart",get_selectionStart,set_selectionStart,true);
		addMember(l,"selectionEnd",get_selectionEnd,set_selectionEnd,true);
		addMember(l,"caret",get_caret,null,true);
		createTypeMetatable(l,null, typeof(UIInput),typeof(UnityEngine.MonoBehaviour));
	}
}
