using System;
using com.tencent.pandora;
using System.Collections.Generic;
public class Lua_UISprite : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UISprite o;
			o=new UISprite();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetAtlasSprite(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			var ret=self.GetAtlasSprite();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int MakePixelPerfect(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			self.MakePixelPerfect();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OnFill(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			BetterList<UnityEngine.Vector3> a1;
			checkType(l,2,out a1);
			BetterList<UnityEngine.Vector2> a2;
			checkType(l,3,out a2);
			BetterList<UnityEngine.Color32> a3;
			checkType(l,4,out a3);
			self.OnFill(a1,a2,a3);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetAtlas(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			self.SetAtlas(a1);
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
			pushValue(l,UISprite.current);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_current(IntPtr l) {
		try {
			UISprite v;
			checkType(l,2,out v);
			UISprite.current=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_atlasPath(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.atlasPath);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_atlasPath(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			System.String v;
			checkType(l,2,out v);
			self.atlasPath=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_centerType(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.centerType);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_centerType(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			UISprite.AdvancedType v;
			checkEnum(l,2,out v);
			self.centerType=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_leftType(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.leftType);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_leftType(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			UISprite.AdvancedType v;
			checkEnum(l,2,out v);
			self.leftType=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_rightType(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.rightType);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_rightType(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			UISprite.AdvancedType v;
			checkEnum(l,2,out v);
			self.rightType=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_bottomType(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.bottomType);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_bottomType(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			UISprite.AdvancedType v;
			checkEnum(l,2,out v);
			self.bottomType=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_topType(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.topType);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_topType(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			UISprite.AdvancedType v;
			checkEnum(l,2,out v);
			self.topType=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onClick(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.onClick);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onClick(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			System.Collections.Generic.List<EventDelegate> v;
			checkType(l,2,out v);
			self.onClick=v;
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
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.isEnabled);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_type(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.type);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_type(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			UISprite.Type v;
			checkEnum(l,2,out v);
			self.type=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_flip(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushEnum(l,(int)self.flip);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_flip(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			UISprite.Flip v;
			checkEnum(l,2,out v);
			self.flip=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_FillScale(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.FillScale);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_material(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.material);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_atlas(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.atlas);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_atlas(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			UIAtlas v;
			checkType(l,2,out v);
			self.atlas=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_spriteName(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.spriteName);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_spriteName(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			string v;
			checkType(l,2,out v);
			self.spriteName=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isValid(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.isValid);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_fillDirection(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
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
			UISprite self=(UISprite)checkSelf(l);
			UISprite.FillDirection v;
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
	static public int get_fillAmount(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.fillAmount);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_fillAmount(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.fillAmount=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_invert(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.invert);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_invert(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.invert=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_border(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.border);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_minWidth(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.minWidth);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_minHeight(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.minHeight);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_drawingDimensions(IntPtr l) {
		try {
			UISprite self=(UISprite)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.drawingDimensions);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UISprite");
		addMember(l,GetAtlasSprite);
		addMember(l,MakePixelPerfect);
		addMember(l,OnFill);
		addMember(l,SetAtlas);
		addMember(l,"current",get_current,set_current,false);
		addMember(l,"atlasPath",get_atlasPath,set_atlasPath,true);
		addMember(l,"centerType",get_centerType,set_centerType,true);
		addMember(l,"leftType",get_leftType,set_leftType,true);
		addMember(l,"rightType",get_rightType,set_rightType,true);
		addMember(l,"bottomType",get_bottomType,set_bottomType,true);
		addMember(l,"topType",get_topType,set_topType,true);
		addMember(l,"onClick",get_onClick,set_onClick,true);
		addMember(l,"isEnabled",get_isEnabled,null,true);
		addMember(l,"type",get_type,set_type,true);
		addMember(l,"flip",get_flip,set_flip,true);
		addMember(l,"FillScale",get_FillScale,null,true);
		addMember(l,"material",get_material,null,true);
		addMember(l,"atlas",get_atlas,set_atlas,true);
		addMember(l,"spriteName",get_spriteName,set_spriteName,true);
		addMember(l,"isValid",get_isValid,null,true);
		addMember(l,"fillDirection",get_fillDirection,set_fillDirection,true);
		addMember(l,"fillAmount",get_fillAmount,set_fillAmount,true);
		addMember(l,"invert",get_invert,set_invert,true);
		addMember(l,"border",get_border,null,true);
		addMember(l,"minWidth",get_minWidth,null,true);
		addMember(l,"minHeight",get_minHeight,null,true);
		addMember(l,"drawingDimensions",get_drawingDimensions,null,true);
		createTypeMetatable(l,constructor, typeof(UISprite),typeof(UIWidget));
	}
}
