// The MIT License (MIT)

// Copyright 2015 Siney/Pangweiwei siney@yeah.net
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.



namespace com.tencent.pandora
{
	using System;
	using System.Collections.Generic;
	using System.Collections;
	using System.IO;
	using System.Text;
	using System.Runtime.InteropServices;
#if !SLUA_STANDALONE
    using UnityEngine;
#endif
	abstract public class LuaVar : IDisposable
	{
		protected LuaState state = null;
		protected int valueref = 0;



		public IntPtr L
		{
			get
			{
				return state.L;
			}
		}

		public int Ref
		{
			get
			{
				return valueref;
			}
		}

		public LuaVar()
		{
			state = null;
		}

		public LuaVar(LuaState l, int r)
		{
			state = l;
			valueref = r;
		}

		public LuaVar(IntPtr l, int r)
		{
			state = LuaState.get(l);
			valueref = r;
		}

		~LuaVar()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposeManagedResources)
		{
			if (valueref != 0)
			{
				LuaState.UnRefAction act = (IntPtr l, int r) =>
				{
					LuaDLL.pua_unref(l, r);
				};
				state.gcRef(act, valueref);
				valueref = 0;
			}
		}

		public void push(IntPtr l)
		{
			LuaDLL.pua_getref(l, valueref);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is LuaVar)
			{
				return this == (LuaVar)obj;
			}
			return false;
		}

		public static bool operator ==(LuaVar x, LuaVar y)
		{
			if ((object)x == null || (object)y == null)
				return (object)x == (object)y;

			return Equals(x, y) == 1;
		}

		public static bool operator !=(LuaVar x, LuaVar y)
		{
			if ((object)x == null || (object)y == null)
				return (object)x != (object)y;

			return Equals(x, y) != 1;
		}

		static int Equals(LuaVar x, LuaVar y)
		{
			x.push(x.L);
			y.push(x.L);
			int ok = LuaDLL.pua_equal(x.L, -1, -2);
			LuaDLL.pua_pop(x.L, 2);
			return ok;
		}
	}

	public class LuaThread : LuaVar
	{
		public LuaThread(IntPtr l, int r)
			: base(l, r)
		{ }
	}

	public class LuaDelegate : LuaFunction
	{
		public object d;

		public LuaDelegate(IntPtr l, int r)
			: base(l, r)
		{
		}

		public override void Dispose(bool disposeManagedResources)
		{
			if (valueref != 0)
			{
				LuaState.UnRefAction act = (IntPtr l, int r) =>
				{
					LuaObject.removeDelgate(l, r);
					LuaDLL.pua_unref(l, r);
				};
				state.gcRef(act, valueref);
				valueref = 0;
			}

		}

	}

	public class LuaFunction : LuaVar
	{
		public LuaFunction(LuaState l, int r)
			: base(l, r)
		{
		}

		public LuaFunction(IntPtr l, int r)
			: base(l, r)
		{
		}

		public bool pcall(int nArgs, int errfunc)
		{

			if (!state.isMainThread())
			{
                SLogger.LogError("Can't call lua function in bg thread");
				return false;
			}

			LuaDLL.pua_getref(L, valueref);

			if (!LuaDLL.pua_isfunction(L, -1))
			{
				LuaDLL.pua_pop(L, 1);
				throw new Exception("Call invalid function.");
			}

			LuaDLL.pua_insert(L, -nArgs - 1);
			if (LuaDLL.pua_pcall(L, nArgs, -1, errfunc) != 0)
			{
				LuaDLL.pua_pop(L, 1);
				return false;
			}
			return true;
		}

		public object pcall(int nARgs) {
			int error = LuaObject.pushTry (state.L);
			LuaDLL.pua_insert (state.L, -nARgs);

			if (innerCall (nARgs, error)) {
				return state.topObjects (error - 1);
			}
			
			return null;
		}

		bool innerCall(int nArgs, int errfunc)
		{
			bool ret = pcall(nArgs, errfunc);
			LuaDLL.pua_remove(L, errfunc);
			return ret;
		}


		public object call()
		{
			int error = LuaObject.pushTry(state.L);
			if (innerCall(0, error))
			{
				return state.topObjects(error - 1);
			}
			return null;
		}

		public object call(params object[] args)
		{
			int error = LuaObject.pushTry(state.L);

			for (int n = 0; args != null && n < args.Length; n++)
			{
				LuaObject.pushVar(L, args[n]);
			}

			if (innerCall(args != null ? args.Length : 0, error))
			{
				return state.topObjects(error - 1);
			}

			return null;
		}

		public object call(object a1)
		{
			int error = LuaObject.pushTry(state.L);

			LuaObject.pushVar(state.L, a1);
			if (innerCall(1, error))
			{
				return state.topObjects(error - 1);
			}


			return null;
		}

		public object call(object a1, object a2)
		{
			int error = LuaObject.pushTry(state.L);

			LuaObject.pushVar(state.L, a1);
			LuaObject.pushVar(state.L, a2);
			if (innerCall(2, error))
			{
				return state.topObjects(error - 1);
			}
			return null;
		}

		public object call(object a1, object a2, object a3)
		{
			int error = LuaObject.pushTry(state.L);

			LuaObject.pushVar(state.L, a1);
			LuaObject.pushVar(state.L, a2);
			LuaObject.pushVar(state.L, a3);
			if (innerCall(3, error))
			{
				return state.topObjects(error - 1);
			}
			return null;
		}

		// you can add call method with specific type rather than object type to avoid gc alloc, like
		// public object call(int a1,float a2,string a3,object a4)
		
		// using specific type to avoid type boxing/unboxing
	}

	public class LuaTable : LuaVar, IEnumerable<LuaTable.TablePair>
	{


		public struct TablePair
		{
			public object key;
			public object value;
		}
		public LuaTable(IntPtr l, int r)
			: base(l, r)
		{
		}

		public LuaTable(LuaState l, int r)
			: base(l, r)
		{
		}

		public LuaTable(LuaState state)
			: base(state, 0)
		{

			LuaDLL.pua_newtable(L);
			valueref = LuaDLL.puaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
		}
		public object this[string key]
		{
			get
			{
				return state.getObject(valueref, key);
			}

			set
			{
				state.setObject(valueref, key, value);
			}
		}

		public object this[int index]
		{
			get
			{
				return state.getObject(valueref, index);
			}

			set
			{
				state.setObject(valueref, index, value);
			}
		}

		public object invoke(string func, params object[] args)
		{
			LuaFunction f = (LuaFunction)this[func];
			if (f != null)
			{
				return f.call(args);
			}
			throw new Exception(string.Format("Can't find {0} function", func));
		}

		public int length()
		{
			int n = LuaDLL.pua_gettop(L);
			push(L);
			int l = LuaDLL.pua_rawlen(L, -1);
			LuaDLL.pua_settop(L, n);
			return l;
		}

		public class Enumerator : IEnumerator<TablePair>, IDisposable
		{
			LuaTable t;
			int indext = -1;
			TablePair current = new TablePair();
			int iterPhase = 0;

			public Enumerator(LuaTable table)
			{
				t = table;
				Reset();
			}

			public bool MoveNext()
			{
				if (indext < 0)
					return false;

				if (iterPhase == 0)
				{
					LuaDLL.pua_pushnil(t.L);
					iterPhase = 1;
				}
				else
					LuaDLL.pua_pop(t.L, 1);

				bool ret = LuaDLL.pua_next(t.L, indext) > 0;
				if (!ret) iterPhase = 2;

				return ret;
			}

			public void Reset()
			{
				LuaDLL.pua_getref(t.L, t.Ref);
				indext = LuaDLL.pua_gettop(t.L);
			}

			public void Dispose()
			{
				if (iterPhase == 1)
					LuaDLL.pua_pop(t.L, 2);

				LuaDLL.pua_remove(t.L, indext);
			}

			public TablePair Current
			{
				get
				{
					current.key = LuaObject.checkVar(t.L, -2);
					current.value = LuaObject.checkVar(t.L, -1);
					return current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}
		}

		public IEnumerator<TablePair> GetEnumerator()
		{
			return new LuaTable.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

	}





	public class LuaState : IDisposable
	{
        private static readonly char[] DOT = new char[] { '.' };
        IntPtr l_;
		int mainThread = 0;
		internal WeakDictionary<int, LuaDelegate> delgateMap = new WeakDictionary<int, LuaDelegate>();

		public IntPtr L
		{
			get
			{

				if (!isMainThread())
				{
                    SLogger.LogError("Can't access lua in bg thread");
					throw new Exception("Can't access lua in bg thread");
				}

				if (l_ == IntPtr.Zero)
				{
                    SLogger.LogError("LuaState had been destroyed, can't used yet");
					throw new Exception("LuaState had been destroyed, can't used yet");
				}

				return l_;
			}
			set
			{
				l_ = value;
			}
		}

		public IntPtr handle
		{
			get
			{
				return L;
			}
		}

		public delegate byte[] LoaderDelegate(string fn);
		public delegate void OutputDelegate(string msg);

		static public LoaderDelegate loaderDelegate;
		static public OutputDelegate logDelegate;
		static public OutputDelegate errorDelegate;


		public delegate void UnRefAction(IntPtr l, int r);
		struct UnrefPair
		{
			public UnRefAction act;
			public int r;
		}
		Queue<UnrefPair> refQueue;
		public int PCallCSFunctionRef = 0;


		public static LuaState main;
		static Dictionary<IntPtr, LuaState> statemap = new Dictionary<IntPtr, LuaState>();
		static IntPtr oldptr = IntPtr.Zero;
		static LuaState oldstate = null;
		static public LuaCSFunction errorFunc = new LuaCSFunction(errorReport);

		public bool isMainThread()
		{
			return System.Threading.Thread.CurrentThread.ManagedThreadId == mainThread;
		}

		static public LuaState get(IntPtr l)
		{
			if (l == oldptr)
				return oldstate;

			LuaState ls;
			if (statemap.TryGetValue(l, out ls))
			{
				oldptr = l;
				oldstate = ls;
				return ls;
			}

			LuaDLL.pua_getglobal(l, "__main_state");
			if (LuaDLL.pua_isnil(l, -1))
			{
				LuaDLL.pua_pop(l, 1);
				return null;
			}

			IntPtr nl = LuaDLL.pua_touserdata(l, -1);
			LuaDLL.pua_pop(l, 1);
			if (nl != l)
				return get(nl);
			return null;
		}

		public LuaState()
		{
			mainThread = System.Threading.Thread.CurrentThread.ManagedThreadId;

			L = LuaDLL.puaL_newstate();
			statemap[L] = this;
			if (main == null) main = this;

			refQueue = new Queue<UnrefPair>();
			ObjectCache.make(L);

			LuaDLL.pua_atpanic(L, panicCallback);

			LuaDLL.puaL_openlibs(L);

			string PCallCSFunction = @"
local assert = assert
local function check(ok,...)
	assert(ok, ...)
	return ...
end
return function(cs_func)
	return function(...)
		return check(cs_func(...))
	end
end
";

			LuaDLL.pua_dostring(L, PCallCSFunction);
			PCallCSFunctionRef = LuaDLL.puaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);

			pcall(L, init);
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int init(IntPtr L)
		{
			LuaDLL.pua_pushlightuserdata(L, L);
			LuaDLL.pua_setglobal(L, "__main_state");

			LuaDLL.pua_pushcfunction(L, print);
			LuaDLL.pua_setglobal(L, "print");

			LuaDLL.pua_pushcfunction(L, pcall);
			LuaDLL.pua_setglobal(L, "pcall");

			pushcsfunction(L, include);
			LuaDLL.pua_setglobal(L, "include");

			pushcsfunction(L, import);
			LuaDLL.pua_setglobal(L, "import");


			string resumefunc = @"
local resume = coroutine.resume
local function check(co, ok, err, ...)
	if not ok then UnityEngine.Debug.LogError(debug.traceback(co,err)) end
	return ok, err, ...
end
coroutine.resume=function(co,...)
	return check(co, resume(co,...))
end
";

			// overload resume function for report error
			LuaState.get(L).doString(resumefunc);

#if UNITY_ANDROID
			// fix android performance drop with JIT on according to luajit mailist post
			LuaState.get(L).doString("if jit then require('jit.opt').start('sizemcode=256','maxmcode=256') for i=1,1000 do end end");
#endif

			pushcsfunction(L, dofile);
			LuaDLL.pua_setglobal(L, "dofile");

			pushcsfunction(L, loadfile);
			LuaDLL.pua_setglobal(L, "loadfile");

			pushcsfunction(L, loader);
			int loaderFunc = LuaDLL.pua_gettop(L);

			LuaDLL.pua_getglobal(L, "package");
#if LUA_5_3
			LuaDLL.pua_getfield(L, -1, "searchers");
#else
			LuaDLL.pua_getfield(L, -1, "loaders");
#endif
			int loaderTable = LuaDLL.pua_gettop(L);

			// Shift table elements right
			for (int e = LuaDLL.pua_rawlen(L, loaderTable) + 1; e > 2; e--)
			{
				LuaDLL.pua_rawgeti(L, loaderTable, e - 1);
				LuaDLL.pua_rawseti(L, loaderTable, e);
			}
			LuaDLL.pua_pushvalue(L, loaderFunc);
			LuaDLL.pua_rawseti(L, loaderTable, 2);
			LuaDLL.pua_settop(L, 0);
			return 0;
		}

		public void Close()
		{
			if (L != IntPtr.Zero)
			{
				if (LuaState.main == this)
				{
                    SLogger.Log("Finalizing Lua State.");
					// be careful, if you close lua vm, make sure you don't use lua state again,
					// comment this line as default for avoid unexpected crash.
					LuaDLL.pua_close(L);

					ObjectCache.del(L);
					ObjectCache.clear();

					statemap.Clear();
					oldptr = IntPtr.Zero;
					oldstate = null;
					L = IntPtr.Zero;

					LuaState.main = null;
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
		}

		public virtual void Dispose(bool dispose)
		{
			if (dispose)
			{
				Close();
			}
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		public static int errorReport(IntPtr L)
		{
			LuaDLL.pua_getglobal(L, "debug");
			LuaDLL.pua_getfield(L, -1, "traceback");
			LuaDLL.pua_pushvalue(L, 1);
			LuaDLL.pua_pushnumber(L, 2);
			LuaDLL.pua_call(L, 2, 1);
			LuaDLL.pua_remove(L, -2);
            SLogger.LogError(LuaDLL.pua_tostring(L, -1));
			if (errorDelegate != null)
			{
				errorDelegate(LuaDLL.pua_tostring(L, -1));
			}
			LuaDLL.pua_pop(L, 1);
			return 0;
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		internal static int import(IntPtr l) {
			LuaObject.pushValue(l, true);
			return 1;
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		internal static int include(IntPtr l)
		{
			try
			{
				LuaDLL.puaL_checktype(l, 1, LuaTypes.LUA_TSTRING);
				string str = LuaDLL.pua_tostring(l, 1);

				string[] ns = str.Split(DOT);

				LuaDLL.pua_pushglobaltable(l);

				for (int n = 0; n < ns.Length; n++)
				{
					LuaDLL.pua_getfield(l, -1, ns[n]);
					if (!LuaDLL.pua_istable(l, -1))
					{
						return LuaObject.error(l, "expect {0} is type table", ns);
					}
					LuaDLL.pua_remove(l, -2);
				}

				LuaDLL.pua_pushnil(l);
				while (LuaDLL.pua_next(l, -2) != 0)
				{
					string key = LuaDLL.pua_tostring(l, -2);
					LuaDLL.pua_getglobal(l, key);
					if (!LuaDLL.pua_isnil(l, -1))
					{
						LuaDLL.pua_pop(l, 1);
						return LuaObject.error(l, "{0} had existed, import can't overload it.", key);
					}
					LuaDLL.pua_pop(l, 1);
					LuaDLL.pua_setglobal(l, key);
				}

				LuaDLL.pua_pop(l, 1);

				LuaObject.pushValue(l, true);
				return 1;
			}
			catch (Exception e)
			{
				return LuaObject.error(l, e);
			}
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		internal static int pcall(IntPtr L)
		{
			int status;
			if (LuaDLL.pua_type(L, 1) != LuaTypes.LUA_TFUNCTION)
			{
				return LuaObject.error(L, "arg 1 expect function");
			}
			LuaDLL.puaL_checktype(L, 1, LuaTypes.LUA_TFUNCTION);
			status = LuaDLL.pua_pcall(L, LuaDLL.pua_gettop(L) - 1, LuaDLL.LUA_MULTRET, 0);
			LuaDLL.pua_pushboolean(L, (status == 0));
			LuaDLL.pua_insert(L, 1);
			return LuaDLL.pua_gettop(L);  /* return status + all results */
		}

		internal static void pcall(IntPtr l, LuaCSFunction f)
		{
			int err = LuaObject.pushTry(l);
			LuaDLL.pua_pushcfunction(l, f);
			if (LuaDLL.pua_pcall(l, 0, 0, err) != 0)
			{
				LuaDLL.pua_pop(l, 1);
			}
			LuaDLL.pua_remove(l, err);
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		internal static int print(IntPtr L)
		{
			int n = LuaDLL.pua_gettop(L);
			string s = "";

			LuaDLL.pua_getglobal(L, "tostring");

			for (int i = 1; i <= n; i++)
			{
				if (i > 1)
				{
					s += "    ";
				}

				LuaDLL.pua_pushvalue(L, -1);
				LuaDLL.pua_pushvalue(L, i);

				LuaDLL.pua_call(L, 1, 1);
				s += LuaDLL.pua_tostring(L, -1);
				LuaDLL.pua_pop(L, 1);
			}
			LuaDLL.pua_settop(L, n);
            SLogger.Log(s);
			if (logDelegate != null)
			{
				logDelegate(s);
			}
			return 0;
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		internal static int loadfile(IntPtr L)
		{
			loader(L);

			if (LuaDLL.pua_isnil(L, -1))
			{
				string fileName = LuaDLL.pua_tostring(L, 1);
				return LuaObject.error(L, "Can't find {0}", fileName);
			}
			return 2;
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		internal static int dofile(IntPtr L)
		{
			int n = LuaDLL.pua_gettop(L);

			loader(L);
			if (!LuaDLL.pua_toboolean(L, -2))
			{
				return 2;
			}
			else
			{
				if (LuaDLL.pua_isnil(L, -1))
				{
					string fileName = LuaDLL.pua_tostring(L, 1);
					return LuaObject.error(L, "Can't find {0}", fileName);
				}
				int k = LuaDLL.pua_gettop(L);
				LuaDLL.pua_call(L, 0, LuaDLL.LUA_MULTRET);
				k = LuaDLL.pua_gettop(L);
				return k - n;
			}
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static public int panicCallback(IntPtr l)
		{
			string reason = string.Format("unprotected error in call to Lua API ({0})", LuaDLL.pua_tostring(l, -1));
			throw new Exception(reason);
		}

		static public void pushcsfunction(IntPtr L, LuaCSFunction function)
		{
			LuaDLL.pua_getref(L, get(L).PCallCSFunctionRef);
			LuaDLL.pua_pushcclosure(L, function, 0);
			LuaDLL.pua_call(L, 1, 1);
		}

		public object doString(string str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);

			object obj;
			if (doBuffer(bytes, "temp buffer", out obj))
				return obj;
			return null; ;
		}

		public object doString(string str, string chunkname)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);

			object obj;
			if (doBuffer(bytes, chunkname, out obj))
				return obj;
			return null; ;
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		internal static int loader(IntPtr L)
		{
			string fileName = LuaDLL.pua_tostring(L, 1);
			byte[] bytes = loadFile(fileName);
			if (bytes != null)
			{
				if (LuaDLL.puaL_loadbuffer(L, bytes, bytes.Length, "@" + fileName) == 0)
				{
					LuaObject.pushValue(L, true);
					LuaDLL.pua_insert(L, -2);
					return 2;
				}
				else
				{
					string errstr = LuaDLL.pua_tostring(L, -1);
					return LuaObject.error(L, errstr);
				}
			}
			LuaObject.pushValue(L, true);
			LuaDLL.pua_pushnil(L);
			return 2;
		}

		public object doFile(string fn)
		{
			byte[] bytes = loadFile(fn);
			if (bytes == null)
			{
                SLogger.LogError(string.Format("Can't find {0}", fn));
				return null;
			}

			object obj;
			if (doBuffer(bytes, "@" + fn, out obj))
				return obj;
			return null;
		}

	    /// <summary>
	    /// Ensure remove BOM from bytes
	    /// </summary>
	    /// <param name="bytes"></param>
	    /// <returns></returns>
	    public static byte[] CleanUTF8Bom(byte[] bytes)
	    {
            if (bytes.Length > 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                var oldBytes = bytes;
                bytes = new byte[bytes.Length - 3];
                Array.Copy(oldBytes, 3, bytes, 0, bytes.Length);
            }
            return bytes;
	    }

		public bool doBuffer(byte[] bytes, string fn, out object ret)
        {        
            // ensure no utf-8 bom, LuaJIT can read BOM, but Lua cannot!
		    bytes = CleanUTF8Bom(bytes);
            ret = null;
			int errfunc = LuaObject.pushTry(L);
			if (LuaDLL.puaL_loadbuffer(L, bytes, bytes.Length, fn) == 0)
			{
				if (LuaDLL.pua_pcall(L, 0, LuaDLL.LUA_MULTRET, errfunc) != 0)
				{
					LuaDLL.pua_pop(L, 2);
					return false;
				}
				LuaDLL.pua_remove(L, errfunc); // pop error function
				ret = topObjects(errfunc - 1);
				return true;
			}
			string err = LuaDLL.pua_tostring(L, -1);
			LuaDLL.pua_pop(L, 2);
			throw new Exception(err);
		}

		internal static byte[] loadFile(string fn)
		{
			try
			{
				byte[] bytes;
				if (loaderDelegate != null)
					bytes = loaderDelegate(fn);
				else
				{
#if !SLUA_STANDALONE
					fn = fn.Replace(".", "/");
					TextAsset asset = (TextAsset)Resources.Load(fn);
					if (asset == null)
						return null;
					bytes = asset.bytes;
#else
				    bytes = File.ReadAllBytes(fn);
#endif
				}
				return bytes;
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}


		internal object getObject(string key)
		{
			LuaDLL.pua_pushglobaltable(L);
			object o = getObject(key.Split(DOT));
			LuaDLL.pua_pop(L, 1);
			return o;
		}

		internal void setObject(string key, object v)
		{
			LuaDLL.pua_pushglobaltable(L);
			setObject(key.Split(DOT), v);
			LuaDLL.pua_pop(L, 1);
		}

		internal object getObject(string[] remainingPath)
		{
			object returnValue = null;
			for (int i = 0; i < remainingPath.Length; i++)
			{
				LuaDLL.pua_pushstring(L, remainingPath[i]);
				LuaDLL.pua_gettable(L, -2);
				returnValue = this.getObject(L, -1);
				LuaDLL.pua_remove(L, -2);
				if (returnValue == null) break;
			}
			return returnValue;
		}


		internal object getObject(int reference, string field)
		{
			int oldTop = LuaDLL.pua_gettop(L);
			LuaDLL.pua_getref(L, reference);
			object returnValue = getObject(field.Split(DOT));
			LuaDLL.pua_settop(L, oldTop);
			return returnValue;
		}

		internal object getObject(int reference, int index)
		{
			if (index >= 1)
			{
				int oldTop = LuaDLL.pua_gettop(L);
				LuaDLL.pua_getref(L, reference);
				LuaDLL.pua_rawgeti(L, -1, index);
				object returnValue = getObject(L, -1);
				LuaDLL.pua_settop(L, oldTop);
				return returnValue;
			}
			throw new IndexOutOfRangeException();
		}

		internal object getObject(int reference, object field)
		{
			int oldTop = LuaDLL.pua_gettop(L);
			LuaDLL.pua_getref(L, reference);
			LuaObject.pushObject(L, field);
			LuaDLL.pua_gettable(L, -2);
			object returnValue = getObject(L, -1);
			LuaDLL.pua_settop(L, oldTop);
			return returnValue;
		}

		internal void setObject(string[] remainingPath, object o)
		{
			int top = LuaDLL.pua_gettop(L);
			for (int i = 0; i < remainingPath.Length - 1; i++)
			{
				LuaDLL.pua_pushstring(L, remainingPath[i]);
				LuaDLL.pua_gettable(L, -2);
			}
			LuaDLL.pua_pushstring(L, remainingPath[remainingPath.Length - 1]);
			LuaObject.pushVar(L, o);
			LuaDLL.pua_settable(L, -3);
			LuaDLL.pua_settop(L, top);
		}


		internal void setObject(int reference, string field, object o)
		{
			int oldTop = LuaDLL.pua_gettop(L);
			LuaDLL.pua_getref(L, reference);
			setObject(field.Split(DOT), o);
			LuaDLL.pua_settop(L, oldTop);
		}

		internal void setObject(int reference, int index, object o)
		{
			if (index >= 1)
			{
				int oldTop = LuaDLL.pua_gettop(L);
				LuaDLL.pua_getref(L, reference);
				LuaObject.pushVar(L, o);
				LuaDLL.pua_rawseti(L, -2, index);
				LuaDLL.pua_settop(L, oldTop);
				return;
			}
			throw new IndexOutOfRangeException();
		}

		internal void setObject(int reference, object field, object o)
		{
			int oldTop = LuaDLL.pua_gettop(L);
			LuaDLL.pua_getref(L, reference);
			LuaObject.pushObject(L, field);
			LuaObject.pushObject(L, o);
			LuaDLL.pua_settable(L, -3);
			LuaDLL.pua_settop(L, oldTop);
		}

		internal object topObjects(int from)
		{
			int top = LuaDLL.pua_gettop(L);
			int nArgs = top - from;
			if (nArgs == 0)
				return null;
			else if (nArgs == 1)
			{
				object o = LuaObject.checkVar(L, top);
				LuaDLL.pua_pop(L, 1);
				return o;
			}
			else
			{
				object[] o = new object[nArgs];
				for (int n = 1; n <= nArgs; n++)
				{
					o[n - 1] = LuaObject.checkVar(L, from + n);

				}
				LuaDLL.pua_settop(L, from);
				return o;
			}
		}

		object getObject(IntPtr l, int p)
		{
			p = LuaDLL.pua_absindex(l, p);
			return LuaObject.checkVar(l, p);
		}

		public LuaFunction getFunction(string key)
		{
			return (LuaFunction)this[key];
		}

		public LuaTable getTable(string key)
		{
			return (LuaTable)this[key];
		}


		public object this[string path]
		{
			get
			{
				return this.getObject(path);
			}
			set
			{
				this.setObject(path, value);
			}
		}

		public void gcRef(UnRefAction act, int r)
		{
			UnrefPair u = new UnrefPair();
			u.act = act;
			u.r = r;
			lock (refQueue)
			{
				refQueue.Enqueue(u);
			}
		}

		public void checkRef()
		{
			int cnt = 0;
			// fix il2cpp lock issue on iOS
			lock (refQueue)
			{
				cnt = refQueue.Count;
			}

			var l = L;
			for (int n = 0; n < cnt; n++)
			{
				UnrefPair u;
				lock (refQueue)
				{
					u = refQueue.Dequeue();
				}
				u.act(l, u.r);
			}
		}
	}
}
