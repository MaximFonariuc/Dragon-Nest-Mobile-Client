namespace com.tencent.pandora
{
    using System;
    using System.Runtime.InteropServices;
    using System.Reflection;
    using System.Collections;
    using System.Text;
    using System.Security;



    /**     Modify Record:
     
    pua_xmove：        return void
    //pua_gc：           LuaDLLWrapper： enum->int。 
      
    pua_objlen：　　   lua 5.1：  luaS_objlen　size_t->int
    pua_rawlen:        lua 5.3：  luaS_rawlen　size_t->int
     
    pua_setmetatable： lua 5.1 return int
                       lua 5.3 return void
     
    //pua_type：         LuaDLLWrapper：  return int->enum
    pua_isnumber：　   LuaDLLWrapper：　return bool->int  
    pua_isstring:      LuaDLLWrapper：　return bool->int
    pua_iscfunction:   LuaDLLWrapper：　return bool->int
 
    pua_call:          5.1 return int->void
     
    pua_toboolean:     LuaDLLWrapper：   return bool->int
     
    pua_atpanic:       return void->intptr 
     
    pua_pushboolean:   LuaDLLWrapper： bool ->int
    pua_pushlstring:   LuaDLLWrapper: luaS_pushlstring. size_t->int

    puaL_getmetafield:  LuaDLLWrapper: return bool->int
    puaL_loadbuffe:     LuaDLLWrapper  luaLS_loadbuffer  size_t  CharSet
     
    pua_error:         return void->int
    pua_checkstack：　　LuaDLLWrapper　return bool->int


    **/

    public class LuaDLLWrapper
    {

#if UNITY_IPHONE && !UNITY_EDITOR
	const string LUADLL = "__Internal";
#else
	const string LUADLL = "slua";
#endif

#if pua_5_3
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaS_rawlen(IntPtr luaState, int index);
#else
		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaS_objlen(IntPtr luaState, int stackPos);
#endif


        //[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int pua_gc(IntPtr luaState, int what, int data);

        //[DllImport(LUADLL,CallingConvention=CallingConvention.Cdecl)]
        //public static extern int pua_type(IntPtr luaState, int index);

        [DllImport(LUADLL,CallingConvention=CallingConvention.Cdecl)]
		public static extern int pua_isnumber(IntPtr luaState, int index);


        [DllImport(LUADLL,CallingConvention=CallingConvention.Cdecl)]
		public static extern int pua_isstring(IntPtr luaState, int index);

        [DllImport(LUADLL,CallingConvention=CallingConvention.Cdecl)]
		public static extern int pua_iscfunction(IntPtr luaState, int index);

        [DllImport(LUADLL,CallingConvention=CallingConvention.Cdecl)]
		public static extern int pua_toboolean(IntPtr luaState, int index);


        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_pushboolean(IntPtr luaState, int value);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaS_pushlstring(IntPtr luaState, byte[] str, int size);


        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int puaL_getmetafield(IntPtr luaState, int stackPos, string field);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaLS_loadbuffer(IntPtr luaState, byte[] buff, int size, string name);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_checkstack(IntPtr luaState, int extra);
    }


}
