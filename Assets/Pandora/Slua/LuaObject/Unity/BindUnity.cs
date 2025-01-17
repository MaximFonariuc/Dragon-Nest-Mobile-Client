using System;
using System.Collections.Generic;
namespace com.tencent.pandora {
	[LuaBinder(0)]
	public class BindUnity {
		public static Action<IntPtr>[] GetBindList() {
			Action<IntPtr>[] list= {
				Lua_UnityEngine_Application.reg,
				Lua_UnityEngine_SystemInfo.reg,
				Lua_UnityEngine_Object.reg,
				Lua_UnityEngine_Component.reg,
				Lua_UnityEngine_Behaviour.reg,
				Lua_UnityEngine_GameObject.reg,
				Lua_UnityEngine_Vector3.reg,
				Lua_UnityEngine_MonoBehaviour.reg,
				Lua_UnityEngine_Random.reg,
				Lua_UnityEngine_Time.reg,
				Lua_UnityEngine_Transform.reg,
				Lua_UnityEngine_Color.reg,
				Lua_UnityEngine_Vector2.reg,
				Lua_UnityEngine_Vector4.reg,
			};
			return list;
		}
	}
}
