using System;
using com.tencent.pandora;
using System.Collections.Generic;
public class Lua_com_tencent_pandora_CSharpInterface : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			com.tencent.pandora.CSharpInterface o;
			o=new com.tencent.pandora.CSharpInterface();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int NowMilliseconds_s(IntPtr l) {
		try {
			var ret=com.tencent.pandora.CSharpInterface.NowMilliseconds();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int PlaySound_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			com.tencent.pandora.CSharpInterface.PlaySound(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Report_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.Int32 a2;
			checkType(l,2,out a2);
			System.Int32 a3;
			checkType(l,3,out a3);
			com.tencent.pandora.CSharpInterface.Report(a1,a2,a3);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReportError_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.Int32 a2;
			checkType(l,2,out a2);
			com.tencent.pandora.CSharpInterface.ReportError(a1,a2);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int CloneAndAddToParent_s(IntPtr l) {
		try {
			UnityEngine.GameObject a1;
			checkType(l,1,out a1);
			System.String a2;
			checkType(l,2,out a2);
			UnityEngine.GameObject a3;
			checkType(l,3,out a3);
			var ret=com.tencent.pandora.CSharpInterface.CloneAndAddToParent(a1,a2,a3);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetParent_s(IntPtr l) {
		try {
			UnityEngine.GameObject a1;
			checkType(l,1,out a1);
			UnityEngine.GameObject a2;
			checkType(l,2,out a2);
			com.tencent.pandora.CSharpInterface.SetParent(a1,a2);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetPanelParent_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			UnityEngine.GameObject a2;
			checkType(l,2,out a2);
			com.tencent.pandora.CSharpInterface.SetPanelParent(a1,a2);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetPanelParent_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=com.tencent.pandora.CSharpInterface.GetPanelParent(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int IsDebug_s(IntPtr l) {
		try {
			var ret=com.tencent.pandora.CSharpInterface.IsDebug();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetPlatformDescription_s(IntPtr l) {
		try {
			var ret=com.tencent.pandora.CSharpInterface.GetPlatformDescription();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetSDKVersion_s(IntPtr l) {
		try {
			var ret=com.tencent.pandora.CSharpInterface.GetSDKVersion();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int UnloadUnusedAssets_s(IntPtr l) {
		try {
			com.tencent.pandora.CSharpInterface.UnloadUnusedAssets();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int WriteCookie_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.String a2;
			checkType(l,2,out a2);
			var ret=com.tencent.pandora.CSharpInterface.WriteCookie(a1,a2);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReadCookie_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=com.tencent.pandora.CSharpInterface.ReadCookie(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetUserData_s(IntPtr l) {
		try {
			var ret=com.tencent.pandora.CSharpInterface.GetUserData();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetRemoteConfig_s(IntPtr l) {
		try {
			var ret=com.tencent.pandora.CSharpInterface.GetRemoteConfig();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetIconSprite_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.String a2;
			checkType(l,2,out a2);
			System.Action<UnityEngine.Sprite> a3;
			LuaDelegation.checkDelegate(l,3,out a3);
			com.tencent.pandora.CSharpInterface.GetIconSprite(a1,a2,a3);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ShowImage_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.String a2;
			checkType(l,2,out a2);
			UnityEngine.GameObject a3;
			checkType(l,3,out a3);
			System.Boolean a4;
			checkType(l,4,out a4);
			System.UInt32 a5;
			checkType(l,5,out a5);
			com.tencent.pandora.CSharpInterface.ShowImage(a1,a2,a3,a4,a5);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ShowPortriat_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.String a2;
			checkType(l,2,out a2);
			UnityEngine.GameObject a3;
			checkType(l,3,out a3);
			System.Boolean a4;
			checkType(l,4,out a4);
			System.UInt32 a5;
			checkType(l,5,out a5);
			com.tencent.pandora.CSharpInterface.ShowPortriat(a1,a2,a3,a4,a5);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int CacheImage_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			com.tencent.pandora.CSharpInterface.CacheImage(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int IsImageCached_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=com.tencent.pandora.CSharpInterface.IsImageCached(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int LoadAssetBundle_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.UInt32 a2;
			checkType(l,2,out a2);
			com.tencent.pandora.CSharpInterface.LoadAssetBundle(a1,a2);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int LoadGameObject_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.Boolean a2;
			checkType(l,2,out a2);
			System.UInt32 a3;
			checkType(l,3,out a3);
			com.tencent.pandora.CSharpInterface.LoadGameObject(a1,a2,a3);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int LoadImage_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.Boolean a2;
			checkType(l,2,out a2);
			System.UInt32 a3;
			checkType(l,3,out a3);
			com.tencent.pandora.CSharpInterface.LoadImage(a1,a2,a3);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetAsset_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=com.tencent.pandora.CSharpInterface.GetAsset(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int CacheAsset_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			com.tencent.pandora.CSharpInterface.CacheAsset(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int IsAssetCached_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=com.tencent.pandora.CSharpInterface.IsAssetCached(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReleaseAsset_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			com.tencent.pandora.CSharpInterface.ReleaseAsset(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int LoadWww_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.String a2;
			checkType(l,2,out a2);
			System.Boolean a3;
			checkType(l,3,out a3);
			System.UInt32 a4;
			checkType(l,4,out a4);
			com.tencent.pandora.CSharpInterface.LoadWww(a1,a2,a3,a4);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int CreatePanel_s(IntPtr l) {
		try {
			System.UInt32 a1;
			checkType(l,1,out a1);
			System.String a2;
			checkType(l,2,out a2);
			com.tencent.pandora.CSharpInterface.CreatePanel(a1,a2);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetPanel_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=com.tencent.pandora.CSharpInterface.GetPanel(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int HidePanel_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			com.tencent.pandora.CSharpInterface.HidePanel(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int HideAllPanel_s(IntPtr l) {
		try {
			com.tencent.pandora.CSharpInterface.HideAllPanel();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int DestroyPanel_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			com.tencent.pandora.CSharpInterface.DestroyPanel(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int DestroyAllPanel_s(IntPtr l) {
		try {
			com.tencent.pandora.CSharpInterface.DestroyAllPanel();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetTotalSwitch_s(IntPtr l) {
		try {
			var ret=com.tencent.pandora.CSharpInterface.GetTotalSwitch();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetFunctionSwitch_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=com.tencent.pandora.CSharpInterface.GetFunctionSwitch(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int LuaCallGame_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			com.tencent.pandora.CSharpInterface.LuaCallGame(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int CallBroker_s(IntPtr l) {
		try {
			System.UInt32 a1;
			checkType(l,1,out a1);
			System.String a2;
			checkType(l,2,out a2);
			System.Int32 a3;
			checkType(l,3,out a3);
			com.tencent.pandora.CSharpInterface.CallBroker(a1,a2,a3);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int IOSPay_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=com.tencent.pandora.CSharpInterface.IOSPay(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int AndroidPay_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=com.tencent.pandora.CSharpInterface.AndroidPay(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OpenPage_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.String a2;
			checkType(l,2,out a2);
			System.Boolean a3;
			checkType(l,3,out a3);
			System.String a4;
			checkType(l,4,out a4);
			System.Boolean a5;
			checkType(l,5,out a5);
			com.tencent.pandora.CSharpInterface.OpenPage(a1,a2,a3,a4,a5);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ClosePage_s(IntPtr l) {
		try {
			com.tencent.pandora.CSharpInterface.ClosePage();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ShowPage_s(IntPtr l) {
		try {
			com.tencent.pandora.CSharpInterface.ShowPage();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int HidePage_s(IntPtr l) {
		try {
			com.tencent.pandora.CSharpInterface.HidePage();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int WritePageMessage_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			com.tencent.pandora.CSharpInterface.WritePageMessage(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OnPageLoaded_s(IntPtr l) {
		try {
			com.tencent.pandora.CSharpInterface.OnPageLoaded();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OnPageMessage_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			com.tencent.pandora.CSharpInterface.OnPageMessage(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OnPageBack_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			com.tencent.pandora.CSharpInterface.OnPageBack(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetFullUrl_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.String a2;
			checkType(l,2,out a2);
			var ret=com.tencent.pandora.CSharpInterface.GetFullUrl(a1,a2);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_IsIOSPlatform(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,com.tencent.pandora.CSharpInterface.IsIOSPlatform);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_PauseDownloading(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,com.tencent.pandora.CSharpInterface.PauseDownloading);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_PauseDownloading(IntPtr l) {
		try {
			bool v;
			checkType(l,2,out v);
			com.tencent.pandora.CSharpInterface.PauseDownloading=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_PauseSocketSending(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,com.tencent.pandora.CSharpInterface.PauseSocketSending);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_PauseSocketSending(IntPtr l) {
		try {
			bool v;
			checkType(l,2,out v);
			com.tencent.pandora.CSharpInterface.PauseSocketSending=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"com.tencent.pandora.CSharpInterface");
		addMember(l,NowMilliseconds_s);
		addMember(l,PlaySound_s);
		addMember(l,Report_s);
		addMember(l,ReportError_s);
		addMember(l,CloneAndAddToParent_s);
		addMember(l,SetParent_s);
		addMember(l,SetPanelParent_s);
		addMember(l,GetPanelParent_s);
		addMember(l,IsDebug_s);
		addMember(l,GetPlatformDescription_s);
		addMember(l,GetSDKVersion_s);
		addMember(l,UnloadUnusedAssets_s);
		addMember(l,WriteCookie_s);
		addMember(l,ReadCookie_s);
		addMember(l,GetUserData_s);
		addMember(l,GetRemoteConfig_s);
		addMember(l,GetIconSprite_s);
		addMember(l,ShowImage_s);
		addMember(l,ShowPortriat_s);
		addMember(l,CacheImage_s);
		addMember(l,IsImageCached_s);
		addMember(l,LoadAssetBundle_s);
		addMember(l,LoadGameObject_s);
		addMember(l,LoadImage_s);
		addMember(l,GetAsset_s);
		addMember(l,CacheAsset_s);
		addMember(l,IsAssetCached_s);
		addMember(l,ReleaseAsset_s);
		addMember(l,LoadWww_s);
		addMember(l,CreatePanel_s);
		addMember(l,GetPanel_s);
		addMember(l,HidePanel_s);
		addMember(l,HideAllPanel_s);
		addMember(l,DestroyPanel_s);
		addMember(l,DestroyAllPanel_s);
		addMember(l,GetTotalSwitch_s);
		addMember(l,GetFunctionSwitch_s);
		addMember(l,LuaCallGame_s);
		addMember(l,CallBroker_s);
		addMember(l,IOSPay_s);
		addMember(l,AndroidPay_s);
		addMember(l,OpenPage_s);
		addMember(l,ClosePage_s);
		addMember(l,ShowPage_s);
		addMember(l,HidePage_s);
		addMember(l,WritePageMessage_s);
		addMember(l,OnPageLoaded_s);
		addMember(l,OnPageMessage_s);
		addMember(l,OnPageBack_s);
		addMember(l,GetFullUrl_s);
		addMember(l,"IsIOSPlatform",get_IsIOSPlatform,null,false);
		addMember(l,"PauseDownloading",get_PauseDownloading,set_PauseDownloading,false);
		addMember(l,"PauseSocketSending",get_PauseSocketSending,set_PauseSocketSending,false);
		createTypeMetatable(l,constructor, typeof(com.tencent.pandora.CSharpInterface));
	}
}
