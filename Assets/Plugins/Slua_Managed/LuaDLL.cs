using System;
using System.Runtime.InteropServices;

namespace com.tencent.pandora
{
#pragma warning disable 414
    public class MonoPInvokeCallbackAttribute : System.Attribute
    {
        private Type type;
        public MonoPInvokeCallbackAttribute(Type t)
        {
            type = t;
        }
    }
#pragma warning restore 414

    public enum LuaTypes : int
    {
        LUA_TNONE = -1,
        LUA_TNIL = 0,
        LUA_TBOOLEAN = 1,
        LUA_TLIGHTUSERDATA = 2,
        LUA_TNUMBER = 3,
        LUA_TSTRING = 4,
        LUA_TTABLE = 5,
        LUA_TFUNCTION = 6,
        LUA_TUSERDATA = 7,
        LUA_TTHREAD = 8,
    }

    public enum LuaGCOptions
    {
        LUA_GCSTOP = 0,
        LUA_GCRESTART = 1,
        LUA_GCCOLLECT = 2,
        LUA_GCCOUNT = 3,
        LUA_GCCOUNTB = 4,
        LUA_GCSTEP = 5,
        LUA_GCSETPAUSE = 6,
        LUA_GCSETSTEPMUL = 7,
    }

    public enum LuaThreadStatus : int
    {
        LUA_YIELD = 1,
        LUA_ERRRUN = 2,
        LUA_ERRSYNTAX = 3,
        LUA_ERRMEM = 4,
        LUA_ERRERR = 5,
    }

    public sealed class LuaIndexes
    {
#if LUA_5_3
        // for lua5.3
        public static int LUA_REGISTRYINDEX = -1000000 - 1000;
#else
        // for lua5.1 or luajit
        public static int LUA_REGISTRYINDEX = -10000;
        public static int LUA_GLOBALSINDEX = -10002;
#endif
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ReaderInfo
    {
        public String chunkData;
        public bool finished;
    }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int LuaCSFunction(IntPtr luaState);
#else
	public delegate int LuaCSFunction(IntPtr luaState);
#endif

    public delegate string LuaChunkReader(IntPtr luaState, ref ReaderInfo data, ref uint size);

    public delegate int LuaFunctionCallback(IntPtr luaState);
    public class LuaDLL
    {
        public static int LUA_MULTRET = -1;
#if UNITY_IPHONE && !UNITY_EDITOR
		const string LUADLL = "__Internal";
#else
        const string LUADLL = "slua";
#endif

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaS_openextlibs(IntPtr L);

        // Thread Funcs
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_tothread(IntPtr L, int index);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_xmove(IntPtr from, IntPtr to, int n);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr pua_newthread(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_status(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_pushthread(IntPtr L);


        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_gc(IntPtr luaState, LuaGCOptions what, int data);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr pua_typename(IntPtr luaState, int type);
        public static string pua_typenamestr(IntPtr luaState, LuaTypes type)
        {
            IntPtr p = pua_typename(luaState, (int)type);
            return Marshal.PtrToStringAnsi(p);
        }
        public static string puaL_typename(IntPtr luaState, int stackPos)
        {
            return LuaDLL.pua_typenamestr(luaState, LuaDLL.pua_type(luaState, stackPos));
        }

        public static bool pua_isfunction(IntPtr luaState, int stackPos)
        {
            return pua_type(luaState, stackPos) == LuaTypes.LUA_TFUNCTION;
        }

        public static bool pua_islightuserdata(IntPtr luaState, int stackPos)
        {
            return pua_type(luaState, stackPos) == LuaTypes.LUA_TLIGHTUSERDATA;
        }

        public static bool pua_istable(IntPtr luaState, int stackPos)
        {
            return pua_type(luaState, stackPos) == LuaTypes.LUA_TTABLE;
        }

        public static bool pua_isthread(IntPtr luaState, int stackPos)
        {
            return pua_type(luaState, stackPos) == LuaTypes.LUA_TTHREAD;
        }

        [Obsolete]
        public static void puaL_error(IntPtr luaState, string message)
        {
            //LuaDLL.pua_pushstring(luaState, message);
            //LuaDLL.pua_error(luaState);
        }

        [Obsolete]
        public static void puaL_error(IntPtr luaState, string fmt, params object[] args)
        {
            //string str = string.Format(fmt, args);
            //puaL_error(luaState, str);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern string puaL_gsub(IntPtr luaState, string str, string pattern, string replacement);



        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_isuserdata(IntPtr luaState, int stackPos);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_rawequal(IntPtr luaState, int stackPos1, int stackPos2);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_setfield(IntPtr luaState, int stackPos, string name);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int puaL_callmeta(IntPtr luaState, int stackPos, string name);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr puaL_newstate();


        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_close(IntPtr luaState);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void puaL_openlibs(IntPtr luaState);



        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int puaL_loadstring(IntPtr luaState, string chunk);
        public static int puaL_dostring(IntPtr luaState, string chunk)
        {
            int result = LuaDLL.puaL_loadstring(luaState, chunk);
            if (result != 0)
                return result;

            return LuaDLL.pua_pcall(luaState, 0, -1, 0);
        }
        public static int pua_dostring(IntPtr luaState, string chunk)
        {
            return LuaDLL.puaL_dostring(luaState, chunk);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_createtable(IntPtr luaState, int narr, int nrec);
        public static void pua_newtable(IntPtr luaState)
        {
            LuaDLL.pua_createtable(luaState, 0, 0);
        }

#if LUA_5_3
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_getglobal(IntPtr luaState, string name);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_setglobal(IntPtr luaState, string name);

        public static void pua_insert(IntPtr luaState, int newTop)
        {
            pua_rotate(luaState, newTop, 1);
        }

        public static void pua_pushglobaltable(IntPtr l)
        {
            pua_rawgeti(l, LuaIndexes.LUA_REGISTRYINDEX, 2); 
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_rotate(IntPtr luaState, int index, int n);

        public static int pua_rawlen(IntPtr luaState, int stackPos)
		{
			return LuaDLLWrapper.luaS_rawlen(luaState, stackPos);
		}

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int puaL_loadbufferx(IntPtr luaState, byte[] buff, int size, string name, IntPtr x);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_callk(IntPtr luaState, int nArgs, int nResults,int ctx,IntPtr k);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_pcallk(IntPtr luaState, int nArgs, int nResults, int errfunc,int ctx,IntPtr k);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int luaS_pcall(IntPtr luaState, int nArgs, int nResults, int errfunc);
		
		public static int pua_call(IntPtr luaState, int nArgs, int nResults)
        {
            return pua_callk(luaState, nArgs, nResults, 0, IntPtr.Zero);
        }

        public static int pua_pcall(IntPtr luaState, int nArgs, int nResults, int errfunc)
        {
			return luaS_pcall(luaState, nArgs, nResults, errfunc);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern double pua_tonumberx(IntPtr luaState, int index, IntPtr x);
        public static double pua_tonumber(IntPtr luaState, int index)
        {
            return pua_tonumberx(luaState, index, IntPtr.Zero);
        }        
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64 pua_tointegerx(IntPtr luaState, int index,IntPtr x);
        public static int pua_tointeger(IntPtr luaState, int index)
        {
            return (int)pua_tointegerx(luaState, index, IntPtr.Zero);
        }


        public static int puaL_loadbuffer(IntPtr luaState, byte[] buff, int size, string name)
        {
            return puaL_loadbufferx(luaState, buff, size, name, IntPtr.Zero);
        }

        public static void pua_remove(IntPtr l, int idx)
        {
            pua_rotate(l, (idx), -1);
            pua_pop(l, 1);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_rawgeti(IntPtr luaState, int tableIndex, Int64 index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_rawseti(IntPtr luaState, int tableIndex, Int64 index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_pushinteger(IntPtr luaState, Int64 i);

        public static Int64 puaL_checkinteger(IntPtr luaState, int stackPos) {
			puaL_checktype(luaState, stackPos, LuaTypes.LUA_TNUMBER);
			return pua_tointegerx(luaState, stackPos, IntPtr.Zero);
		}

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int luaS_yield(IntPtr luaState,int nrets);

		public static int pua_yield(IntPtr luaState,int nrets) {
			return luaS_yield(luaState,nrets);
		}


		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int pua_resume(IntPtr L, IntPtr from, int narg);

		public static void pua_replace(IntPtr luaState, int index) {
			pua_copy(luaState, -1, (index));
			pua_pop(luaState, 1);
		}

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void pua_copy(IntPtr luaState,int from,int toidx);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int pua_isinteger(IntPtr luaState, int p);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int pua_compare(IntPtr luaState, int index1, int index2, int op);
		
		public static int pua_equal(IntPtr luaState, int index1, int index2)
		{
			return pua_compare(luaState, index1, index2, 0);
		}

#else
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_resume(IntPtr L, int narg);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_lessthan(IntPtr luaState, int stackPos1, int stackPos2);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_getfenv(IntPtr luaState, int stackPos);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_yield(IntPtr L, int nresults);

        public static void pua_getglobal(IntPtr luaState, string name)
        {
            LuaDLL.pua_pushstring(luaState, name);
            LuaDLL.pua_gettable(luaState, LuaIndexes.LUA_GLOBALSINDEX);
        }

        public static void pua_setglobal(IntPtr luaState, string name)
        {
            LuaDLL.pua_pushstring(luaState, name);
            LuaDLL.pua_insert(luaState, -2);
            LuaDLL.pua_settable(luaState, LuaIndexes.LUA_GLOBALSINDEX);
        }

        public static void pua_pushglobaltable(IntPtr l)
        {
            LuaDLL.pua_pushvalue(l, LuaIndexes.LUA_GLOBALSINDEX);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_insert(IntPtr luaState, int newTop);

        public static int pua_rawlen(IntPtr luaState, int stackPos)
        {
            return LuaDLLWrapper.luaS_objlen(luaState, stackPos);
        }

        public static int pua_strlen(IntPtr luaState, int stackPos)
        {
            return pua_rawlen(luaState, stackPos);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_call(IntPtr luaState, int nArgs, int nResults);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_pcall(IntPtr luaState, int nArgs, int nResults, int errfunc);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern double pua_tonumber(IntPtr luaState, int index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_tointeger(IntPtr luaState, int index);

        public static int puaL_loadbuffer(IntPtr luaState, byte[] buff, int size, string name)
        {
            return LuaDLLWrapper.luaLS_loadbuffer(luaState, buff, size, name);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_remove(IntPtr luaState, int index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_rawgeti(IntPtr luaState, int tableIndex, int index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_rawseti(IntPtr luaState, int tableIndex, int index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_pushinteger(IntPtr luaState, IntPtr i);

        public static void pua_pushinteger(IntPtr luaState, int i)
        {
            pua_pushinteger(luaState, (IntPtr)i);
        }

        public static int puaL_checkinteger(IntPtr luaState, int stackPos)
        {
            puaL_checktype(luaState, stackPos, LuaTypes.LUA_TNUMBER);
            return pua_tointeger(luaState, stackPos);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_replace(IntPtr luaState, int index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_setfenv(IntPtr luaState, int stackPos);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_equal(IntPtr luaState, int index1, int index2);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int puaL_loadfile(IntPtr luaState, string filename);
#endif


        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_settop(IntPtr luaState, int newTop);
        public static void pua_pop(IntPtr luaState, int amount)
        {
            LuaDLL.pua_settop(luaState, -(amount) - 1);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_gettable(IntPtr luaState, int index);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_rawget(IntPtr luaState, int index);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_settable(IntPtr luaState, int index);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_rawset(IntPtr luaState, int index);

#if LUA_5_3
		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void pua_setmetatable(IntPtr luaState, int objIndex);
#else
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_setmetatable(IntPtr luaState, int objIndex);
#endif


        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_getmetatable(IntPtr luaState, int objIndex);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_pushvalue(IntPtr luaState, int index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_gettop(IntPtr luaState);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaTypes pua_type(IntPtr luaState, int index);
        public static bool pua_isnil(IntPtr luaState, int index)
        {
            return (LuaDLL.pua_type(luaState, index) == LuaTypes.LUA_TNIL);
        }

        public static bool pua_isnumber(IntPtr luaState, int index)
        {
            return LuaDLLWrapper.pua_isnumber(luaState, index) > 0;
        }
        public static bool pua_isboolean(IntPtr luaState, int index)
        {
            return LuaDLL.pua_type(luaState, index) == LuaTypes.LUA_TBOOLEAN;
        }
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int puaL_ref(IntPtr luaState, int registryIndex);


        public static void pua_getref(IntPtr luaState, int reference)
        {
            LuaDLL.pua_rawgeti(luaState, LuaIndexes.LUA_REGISTRYINDEX, reference);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void puaL_unref(IntPtr luaState, int registryIndex, int reference);
        public static void pua_unref(IntPtr luaState, int reference)
        {
            LuaDLL.puaL_unref(luaState, LuaIndexes.LUA_REGISTRYINDEX, reference);
        }

        public static bool pua_isstring(IntPtr luaState, int index)
        {
            return LuaDLLWrapper.pua_isstring(luaState, index) > 0;
        }

        public static bool pua_iscfunction(IntPtr luaState, int index)
        {
            return LuaDLLWrapper.pua_iscfunction(luaState, index) > 0;
        }
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_pushnil(IntPtr luaState);

        public static void puaL_checktype(IntPtr luaState, int p, LuaTypes t)
        {
            LuaTypes ct = LuaDLL.pua_type(luaState, p);
            if (ct != t)
            {
                throw new Exception(string.Format("arg {0} expect {1}, got {2}", p, pua_typenamestr(luaState, t), pua_typenamestr(luaState, ct)));
            }
        }

        public static void pua_pushcfunction(IntPtr luaState, LuaCSFunction function)
        {
#if SLUA_STANDALONE
            // Add all LuaCSFunction�� or they will be GC collected!  (problem at windows, .net framework 4.5, `CallbackOnCollectedDelegated` exception)
            GCHandle.Alloc(function);
#endif
            IntPtr fn = Marshal.GetFunctionPointerForDelegate(function);
            pua_pushcclosure(luaState, fn, 0);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr pua_tocfunction(IntPtr luaState, int index);


        public static bool pua_toboolean(IntPtr luaState, int index)
        {
            return LuaDLLWrapper.pua_toboolean(luaState, index) > 0;
        }


        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr luaS_tolstring32(IntPtr luaState, int index, out int strLen);

        public static string pua_tostring(IntPtr luaState, int index)
        {
            int strlen;

            IntPtr str = luaS_tolstring32(luaState, index, out strlen); // fix il2cpp 64 bit

            if (str != IntPtr.Zero)
            {
                return Marshal.PtrToStringAnsi(str, strlen);
            }
            return null;
        }

		public static byte[] pua_tobytes(IntPtr luaState, int index)
		{
			int strlen;

			IntPtr str = luaS_tolstring32(luaState, index, out strlen); // fix il2cpp 64 bit

			if (str != IntPtr.Zero)
			{
				byte[] bytes = new byte[strlen];
				Marshal.Copy(str, bytes,0,strlen);
				return bytes;
			}
			return null;
		}

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr pua_atpanic(IntPtr luaState, LuaCSFunction panicf);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_pushnumber(IntPtr luaState, double number);

        public static void pua_pushboolean(IntPtr luaState, bool value)
        {
            LuaDLLWrapper.pua_pushboolean(luaState, value ? 1 : 0);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_pushstring(IntPtr luaState, string str);

        public static void pua_pushlstring(IntPtr luaState, byte[] str, int size)
        {
            LuaDLLWrapper.luaS_pushlstring(luaState, str, size);
        }




        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int puaL_newmetatable(IntPtr luaState, string meta);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_getfield(IntPtr luaState, int stackPos, string meta);
        public static void puaL_getmetatable(IntPtr luaState, string meta)
        {
            LuaDLL.pua_getfield(luaState, LuaIndexes.LUA_REGISTRYINDEX, meta);
        }
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr puaL_checkudata(IntPtr luaState, int stackPos, string meta);

        public static bool puaL_getmetafield(IntPtr luaState, int stackPos, string field)
        {
            return LuaDLLWrapper.puaL_getmetafield(luaState, stackPos, field) > 0;
        }
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_load(IntPtr luaState, LuaChunkReader chunkReader, ref ReaderInfo data, string chunkName);




        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_error(IntPtr luaState);

        public static bool pua_checkstack(IntPtr luaState, int extra)
        {
            return LuaDLLWrapper.pua_checkstack(luaState, extra) > 0;
        }
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int pua_next(IntPtr luaState, int index);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_pushlightuserdata(IntPtr luaState, IntPtr udata);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void puaL_where(IntPtr luaState, int level);

        public static double puaL_checknumber(IntPtr luaState, int stackPos)
        {
            puaL_checktype(luaState, stackPos, LuaTypes.LUA_TNUMBER);
            return pua_tonumber(luaState, stackPos);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_concat(IntPtr luaState, int n);


        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaS_newuserdata(IntPtr luaState, int val);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaS_rawnetobj(IntPtr luaState, int obj);


        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr pua_touserdata(IntPtr luaState, int index);

        public static int pua_absindex(IntPtr luaState, int index)
        {
            return index > 0 ? index : pua_gettop(luaState) + index + 1;
        }

        public static int pua_upvalueindex(int i)
        {
#if LUA_5_3
            return LuaIndexes.LUA_REGISTRYINDEX - i;
#else
            return LuaIndexes.LUA_GLOBALSINDEX - i;
#endif
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void pua_pushcclosure(IntPtr l, IntPtr f, int nup);

        public static void pua_pushcclosure(IntPtr l, LuaCSFunction f, int nup)
        {
#if SLUA_STANDALONE
            // Add all LuaCSFunction�� or they will be GC collected!  (problem at windows, .net framework 4.5, `CallbackOnCollectedDelegated` exception)
            GCHandle.Alloc(f);
#endif
            IntPtr fn = Marshal.GetFunctionPointerForDelegate(f);
            pua_pushcclosure(l, fn, nup);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaS_checkVector2(IntPtr l, int p, out float x, out float y);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaS_checkVector3(IntPtr l, int p, out float x, out float y, out float z);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaS_checkVector4(IntPtr l, int p, out float x, out float y, out float z, out float w);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaS_checkQuaternion(IntPtr l, int p, out float x, out float y, out float z, out float w);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaS_checkColor(IntPtr l, int p, out float x, out float y, out float z, out float w);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaS_pushVector2(IntPtr l, float x, float y);


        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaS_pushVector3(IntPtr l, float x, float y, float z);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaS_pushVector4(IntPtr l, float x, float y, float z, float w);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaS_pushQuaternion(IntPtr l, float x, float y, float z, float w);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaS_pushColor(IntPtr l, float x, float y, float z, float w);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaS_setDataVec(IntPtr l, int p, float x, float y, float z, float w);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaS_checkluatype(IntPtr l, int p, string t);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaS_pushobject(IntPtr l, int index, string t, bool gco, int cref);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaS_getcacheud(IntPtr l, int index, int cref);


        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaS_subclassof(IntPtr l, int index, string t);
    }
}
