using System;
using com.tencent.pandora;
using System.Collections.Generic;
public class Lua_UITexture : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UITexture o;
			o=new UITexture();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Refresh(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			self.Refresh();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int MakePixelPerfect(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
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
			UITexture self=(UITexture)checkSelf(l);
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
	static public int SetTexture(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			self.SetTexture(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetRuntimeTexture(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			UnityEngine.Texture a1;
			checkType(l,2,out a1);
			System.Boolean a2;
			checkType(l,3,out a2);
			self.SetRuntimeTexture(a1,a2);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int FillMat(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			UnityEngine.Material a1;
			checkType(l,2,out a1);
			self.FillMat(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetTextureListType_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=UITexture.GetTextureListType(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_mtexType(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.mtexType);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_mtexType(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			System.Byte v;
			checkType(l,2,out v);
			self.mtexType=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_mIsRuntimeLoad(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.mIsRuntimeLoad);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_mIsRuntimeLoad(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.mIsRuntimeLoad=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_texPath(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.texPath);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_texPath(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			System.String v;
			checkType(l,2,out v);
			self.texPath=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_shaderName(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.shaderName);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_shaderName(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			System.String v;
			checkType(l,2,out v);
			self.shaderName=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_mTexture(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.mTexture);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_mTexture(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			UnityEngine.Texture v;
			checkType(l,2,out v);
			self.mTexture=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_mTexture1(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.mTexture1);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_mTexture1(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			UnityEngine.Texture v;
			checkType(l,2,out v);
			self.mTexture1=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sepTexAlpha(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UITexture.sepTexAlpha);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sepTexAlpha(IntPtr l) {
		try {
			UnityEngine.Shader v;
			checkType(l,2,out v);
			UITexture.sepTexAlpha=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_colorTex(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UITexture.colorTex);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_colorTex(IntPtr l) {
		try {
			UnityEngine.Shader v;
			checkType(l,2,out v);
			UITexture.colorTex=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sepTexAlphaH2(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UITexture.sepTexAlphaH2);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sepTexAlphaH2(IntPtr l) {
		try {
			UnityEngine.Shader v;
			checkType(l,2,out v);
			UITexture.sepTexAlphaH2=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_colorTexH2(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UITexture.colorTexH2);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_colorTexH2(IntPtr l) {
		try {
			UnityEngine.Shader v;
			checkType(l,2,out v);
			UITexture.colorTexH2=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sepTexAlphaH4(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UITexture.sepTexAlphaH4);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sepTexAlphaH4(IntPtr l) {
		try {
			UnityEngine.Shader v;
			checkType(l,2,out v);
			UITexture.sepTexAlphaH4=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_colorTexH4(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UITexture.colorTexH4);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_colorTexH4(IntPtr l) {
		try {
			UnityEngine.Shader v;
			checkType(l,2,out v);
			UITexture.colorTexH4=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_horizontally2(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UITexture.horizontally2);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_horizontally2(IntPtr l) {
		try {
			System.Byte v;
			checkType(l,2,out v);
			UITexture.horizontally2=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_horizontally4(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UITexture.horizontally4);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_horizontally4(IntPtr l) {
		try {
			System.Byte v;
			checkType(l,2,out v);
			UITexture.horizontally4=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_vertically2(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UITexture.vertically2);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_vertically2(IntPtr l) {
		try {
			System.Byte v;
			checkType(l,2,out v);
			UITexture.vertically2=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_vertically4(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UITexture.vertically4);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_vertically4(IntPtr l) {
		try {
			System.Byte v;
			checkType(l,2,out v);
			UITexture.vertically4=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_normalTex(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UITexture.normalTex);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_normalTex(IntPtr l) {
		try {
			System.Byte v;
			checkType(l,2,out v);
			UITexture.normalTex=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_mainTexture(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.mainTexture);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_mainTexture(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			UnityEngine.Texture v;
			checkType(l,2,out v);
			self.mainTexture=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_alphaTexture(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.alphaTexture);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_material(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.material);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_material(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			UnityEngine.Material v;
			checkType(l,2,out v);
			self.material=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_shader(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.shader);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_shader(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			UnityEngine.Shader v;
			checkType(l,2,out v);
			self.shader=v;
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
			UITexture self=(UITexture)checkSelf(l);
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
			UITexture self=(UITexture)checkSelf(l);
			UITexture.Flip v;
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
	static public int get_premultipliedAlpha(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.premultipliedAlpha);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_uvRect(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.uvRect);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_uvRect(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			UnityEngine.Rect v;
			checkValueType(l,2,out v);
			self.uvRect=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_drawingDimensions(IntPtr l) {
		try {
			UITexture self=(UITexture)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.drawingDimensions);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UITexture");
		addMember(l,Refresh);
		addMember(l,MakePixelPerfect);
		addMember(l,OnFill);
		addMember(l,SetTexture);
		addMember(l,SetRuntimeTexture);
		addMember(l,FillMat);
		addMember(l,GetTextureListType_s);
		addMember(l,"mtexType",get_mtexType,set_mtexType,true);
		addMember(l,"mIsRuntimeLoad",get_mIsRuntimeLoad,set_mIsRuntimeLoad,true);
		addMember(l,"texPath",get_texPath,set_texPath,true);
		addMember(l,"shaderName",get_shaderName,set_shaderName,true);
		addMember(l,"mTexture",get_mTexture,set_mTexture,true);
		addMember(l,"mTexture1",get_mTexture1,set_mTexture1,true);
		addMember(l,"sepTexAlpha",get_sepTexAlpha,set_sepTexAlpha,false);
		addMember(l,"colorTex",get_colorTex,set_colorTex,false);
		addMember(l,"sepTexAlphaH2",get_sepTexAlphaH2,set_sepTexAlphaH2,false);
		addMember(l,"colorTexH2",get_colorTexH2,set_colorTexH2,false);
		addMember(l,"sepTexAlphaH4",get_sepTexAlphaH4,set_sepTexAlphaH4,false);
		addMember(l,"colorTexH4",get_colorTexH4,set_colorTexH4,false);
		addMember(l,"horizontally2",get_horizontally2,set_horizontally2,false);
		addMember(l,"horizontally4",get_horizontally4,set_horizontally4,false);
		addMember(l,"vertically2",get_vertically2,set_vertically2,false);
		addMember(l,"vertically4",get_vertically4,set_vertically4,false);
		addMember(l,"normalTex",get_normalTex,set_normalTex,false);
		addMember(l,"mainTexture",get_mainTexture,set_mainTexture,true);
		addMember(l,"alphaTexture",get_alphaTexture,null,true);
		addMember(l,"material",get_material,set_material,true);
		addMember(l,"shader",get_shader,set_shader,true);
		addMember(l,"flip",get_flip,set_flip,true);
		addMember(l,"premultipliedAlpha",get_premultipliedAlpha,null,true);
		addMember(l,"uvRect",get_uvRect,set_uvRect,true);
		addMember(l,"drawingDimensions",get_drawingDimensions,null,true);
		createTypeMetatable(l,constructor, typeof(UITexture),typeof(UIWidget));
	}
}
