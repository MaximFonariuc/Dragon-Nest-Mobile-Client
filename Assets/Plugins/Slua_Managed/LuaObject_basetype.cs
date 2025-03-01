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

using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
namespace com.tencent.pandora
{

/* https://msdn.microsoft.com/zh-cn/library/s1ax56ch.aspx
 * 
 * null								LUA_TNIL
 * Value Types:
 * 	enum
 * 	struct:
 * 		Numeric types: 
 *	 		Integral Types: 		LUA_TNUMBER
 * 				sbyte  = SByte
 * 				byte   = Byte
 * 				char   = Char
 * 				short  = Int16
 * 				ushort = UInt16
 * 				int	   = Int32
 * 				uint   = UInt32
 * 				long   = Int64
 * 				ulong  = UInt64
 * 			Floating-Point Types: 	LUA_TNUMBER
 * 				float  = Single
 * 				double = Double
 * 		bool = Boolean				LUA_TBOOLEAN
 * 		User defined structs		LUA_TTABLE(Vector...) || non_cached@LUA_TUSERDATA
 * Reference Types:
 *  string							LUA_TSTRING
 * 	delegate						LUA_TFUNCTION
 * 	class、System.Type				LUA_TTABLE || cached@LUA_TUSERDATA
 * 	object							cached@LUA_TUSERDATA
 *  char[]							LUA_TSTRING
 * 	T[]								LUA_TTABLE limit support
 *  interface, dynamic 				unsupport
 * IntPtr							LUA_TLIGHTUSERDATA
 * 
 * 
 * every type should implement: 
 * 		public static bool checkType(IntPtr l, int p, out T v)
 *		public static void pushValue(IntPtr l, T v)
 * 
*/	
	public partial class LuaObject
	{
#region enum
		static public bool checkEnum<T>(IntPtr l, int p, out T o) where T : struct
		{
			int i = (int) LuaDLL.puaL_checkinteger (l, p);
			o = (T)Enum.ToObject(typeof(T), i);

			return true;
		}

		public static void pushEnum(IntPtr l, int e)
		{
			pushValue(l, e);
		}
#endregion

#region Integral Types
	#region sbyte
		public static bool checkType(IntPtr l, int p, out sbyte v)
		{
			v = (sbyte)LuaDLL.puaL_checkinteger(l, p);
			return true;
		}
		
		public static void pushValue(IntPtr l, sbyte v)
		{
			LuaDLL.pua_pushinteger(l, v);
		}

	#endregion

		#region byte
		static public bool checkType(IntPtr l, int p, out byte v)
		{
			v = (byte)LuaDLL.puaL_checkinteger(l, p);
			return true;
		}

		public static void pushValue(IntPtr l, byte i)
		{
			LuaDLL.pua_pushinteger(l, i);
		}

		// why doesn't have a checkArray<byte[]> function accept lua string?
		// I think you should did a Buffer class to wrap byte[] pass/accept between mono and lua vm
		#endregion

		#region char
		static public bool checkType(IntPtr l, int p,out char c)
		{
			c = (char)LuaDLL.puaL_checkinteger(l, p);
			return true;
		}

		public static void pushValue(IntPtr l, char v)
		{
			LuaDLL.pua_pushinteger(l, v);
		}

		static public bool checkArray(IntPtr l, int p, out char[] pars)
		{
			LuaDLL.puaL_checktype(l, p, LuaTypes.LUA_TSTRING);
			string s;
			checkType(l, p, out s);
			pars = s.ToCharArray();
			return true;
		}
		#endregion

		#region short
		static public bool checkType(IntPtr l, int p, out short v)
		{
			v = (short)LuaDLL.puaL_checkinteger(l, p);
			return true;
		}
		
		public static void pushValue(IntPtr l, short i)
		{
			LuaDLL.pua_pushinteger(l, i);
		}
		#endregion

		#region ushort
		static public bool checkType(IntPtr l, int p, out ushort v)
		{
			v = (ushort)LuaDLL.puaL_checkinteger(l, p);
			return true;
		}
                
		public static void pushValue(IntPtr l, ushort v)
		{
		    LuaDLL.pua_pushinteger(l, v);
		}
               
		#endregion

		#region int
		static public bool checkType(IntPtr l, int p, out int v)
		{
			v = (int)LuaDLL.puaL_checkinteger(l, p);
			return true;
		}
                
		public static void pushValue(IntPtr l, int i)
		{
			LuaDLL.pua_pushinteger(l, i);
		}
		
		#endregion
		
		#region uint
		static public bool checkType(IntPtr l, int p, out uint v)
		{
			v = (uint)LuaDLL.puaL_checkinteger(l, p);
			return true;
		}
		
		public static void pushValue(IntPtr l, uint o)
		{
			LuaDLL.pua_pushnumber(l, o);
		}
		#endregion

		#region long
		static public bool checkType(IntPtr l, int p, out long v)
		{
#if LUA_5_3
            v = (long)LuaDLL.puaL_checkinteger(l, p);
#else
			v = (long)LuaDLL.puaL_checknumber(l, p);
#endif
			return true;
		}
		
		public static void pushValue(IntPtr l, long i)
		{
#if LUA_5_3
            LuaDLL.pua_pushinteger(l,i);
#else
			LuaDLL.pua_pushnumber(l, i);
#endif
		}
		
		#endregion

		#region ulong
		static public bool checkType(IntPtr l, int p, out ulong v)
		{
#if LUA_5_3
			v = (ulong)LuaDLL.puaL_checkinteger(l, p);
#else
			v = (ulong)LuaDLL.puaL_checknumber(l, p);
#endif
			return true;
		}
		
		public static void pushValue(IntPtr l, ulong o)
		{
			#if LUA_5_3
			LuaDLL.pua_pushinteger(l, (long)o);
			#else
			LuaDLL.pua_pushnumber(l, o);
			#endif
		}
		#endregion
		

#endregion

#region Floating-Point Types
		#region float
		public static bool checkType(IntPtr l, int p, out float v)
		{
			v = (float)LuaDLL.puaL_checknumber(l, p);
			return true;
		}

		public static void pushValue(IntPtr l, float o)
		{
			LuaDLL.pua_pushnumber(l, o);
		}
		
		#endregion

		#region double
		static public bool checkType(IntPtr l, int p, out double v)
		{
			v = LuaDLL.puaL_checknumber(l, p);
			return true;
		}
		
		public static void pushValue(IntPtr l, double d)
		{
			LuaDLL.pua_pushnumber(l, d);
		}
		
		#endregion
#endregion

		#region bool
		static public bool checkType(IntPtr l, int p, out bool v)
		{
			LuaDLL.puaL_checktype(l, p, LuaTypes.LUA_TBOOLEAN);
			v = LuaDLL.pua_toboolean(l, p);
			return true;
		}
		
		public static void pushValue(IntPtr l, bool b)
		{
			LuaDLL.pua_pushboolean(l, b);
		}
	
		#endregion

		#region string
		static public bool checkType(IntPtr l, int p, out string v)
		{
			if(LuaDLL.pua_isuserdata(l,p)>0)
			{
				object o = checkObj(l, p);
				if (o is string)
				{
					v = o as string;
					return true;
				}
			}
			else if (LuaDLL.pua_isstring(l, p))
			{
				v = LuaDLL.pua_tostring(l, p);
				return true;
			}
			
			v = null;
			return false;
		}

		static public bool checkBinaryString(IntPtr l,int p,out byte[] bytes){
			if(LuaDLL.pua_isstring(l,p)){
				bytes = LuaDLL.pua_tobytes(l, p);
				return true;
			}
			bytes = null;
			return false;
		}

		public static void pushValue(IntPtr l, string s)
		{
			LuaDLL.pua_pushstring(l, s);
		}
		

		#endregion

		#region IntPtr
		static public bool checkType(IntPtr l, int p, out IntPtr v)
		{
			v = LuaDLL.pua_touserdata(l, p);
			return true;
		}
		#endregion

		#region LuaType
		static public bool checkType(IntPtr l, int p, out LuaDelegate f)
		{
			LuaState state = LuaState.get(l);

			p = LuaDLL.pua_absindex(l, p);
			LuaDLL.puaL_checktype(l, p, LuaTypes.LUA_TFUNCTION);

			LuaDLL.pua_getglobal(l, DelgateTable);
			LuaDLL.pua_pushvalue(l, p);
			LuaDLL.pua_gettable(l, -2); // find function in __LuaDelegate table
			if (LuaDLL.pua_isnil(l, -1))
			{ // not found
				LuaDLL.pua_pop(l, 1); // pop nil
				f = newDelegate(l, p);
			}
			else
			{
				int fref = LuaDLL.pua_tointeger(l, -1);
				LuaDLL.pua_pop(l, 1); // pop ref value;
				f = state.delgateMap[fref];
				if (f == null)
				{
					f = newDelegate(l, p);
				}
			}
			LuaDLL.pua_pop(l, 1); // pop DelgateTable
			return true;
		}
		
		static public bool checkType(IntPtr l, int p, out LuaThread lt)
		{
			if (LuaDLL.pua_isnil(l, p))
			{
				lt = null;
				return true;
			}
			LuaDLL.puaL_checktype(l, p, LuaTypes.LUA_TTHREAD);
			LuaDLL.pua_pushvalue(l, p);
			int fref = LuaDLL.puaL_ref(l, LuaIndexes.LUA_REGISTRYINDEX);
			lt = new LuaThread(l, fref);
			return true;
		}

        static public bool checkType(IntPtr l, int p, out LuaFunction f)
		{
			if (LuaDLL.pua_isnil(l, p))
			{
				f = null;
				return true;
			}
			LuaDLL.puaL_checktype(l, p, LuaTypes.LUA_TFUNCTION);
			LuaDLL.pua_pushvalue(l, p);
			int fref = LuaDLL.puaL_ref(l, LuaIndexes.LUA_REGISTRYINDEX);
			f = new LuaFunction(l, fref);
			return true;
		}

		static public bool checkType(IntPtr l, int p, out LuaTable t)
		{
			if (LuaDLL.pua_isnil(l, p))
			{
				t = null;
				return true;
			}
			LuaDLL.puaL_checktype(l, p, LuaTypes.LUA_TTABLE);
			LuaDLL.pua_pushvalue(l, p);
			int fref = LuaDLL.puaL_ref(l, LuaIndexes.LUA_REGISTRYINDEX);
			t = new LuaTable(l, fref);
			return true;
		}

		public static void pushValue(IntPtr l, LuaCSFunction f)
		{
			LuaState.pushcsfunction (l, f);
		}
		
		public static void pushValue(IntPtr l, LuaTable t)
		{
			if (t == null)
				LuaDLL.pua_pushnil(l);
			else
				t.push(l);
		}
		#endregion

		#region Type
		private static Type MonoType = typeof(Type).GetType();

		public static Type FindType(string qualifiedTypeName) 
		{
			Type t = Type.GetType(qualifiedTypeName);

			if (t != null)
			{
				return t;
			}
			else
			{
				Assembly[] Assemblies = AppDomain.CurrentDomain.GetAssemblies();
				for (int n = 0; n < Assemblies.Length;n++ )
				{
					Assembly asm = Assemblies[n];
					t = asm.GetType(qualifiedTypeName);
					if (t != null)
						return t;
				}
				return null;
			}
		}


		static public bool checkType(IntPtr l, int p, out Type t)
		{
			string tname = null;
			LuaTypes lt = LuaDLL.pua_type(l, p);
            switch (lt)
            {
                case LuaTypes.LUA_TUSERDATA:
                    object o = checkObj(l, p);
                    if (o.GetType() != MonoType)
                        throw new Exception(string.Format("{0} expect Type, got {1}", p, o.GetType().Name));
                    t = (Type)o;
					return true;
                case LuaTypes.LUA_TTABLE:
                    LuaDLL.pua_pushstring(l, "__type");
                    LuaDLL.pua_rawget(l, p);
                    if (!LuaDLL.pua_isnil(l, -1))
                    {
                        t = (Type)checkObj(l, -1);
                        LuaDLL.pua_pop(l, 1);
                        return true;
                    }
                    else
                    {
                        LuaDLL.pua_pushstring(l, "__fullname");
                        LuaDLL.pua_rawget(l, p);
                        tname = LuaDLL.pua_tostring(l, -1);
                        LuaDLL.pua_pop(l, 2);
                    }
                    break;

                case LuaTypes.LUA_TSTRING:
                    checkType(l, p, out tname);
                    break;
            }

			if (tname == null)
				throw new Exception("expect string or type table");

			t = LuaObject.FindType(tname);
            if (t != null && lt==LuaTypes.LUA_TTABLE)
            {
                LuaDLL.pua_pushstring(l, "__type");
				pushLightObject(l, t);
                LuaDLL.pua_rawset(l, p);
            }
			return t != null;
		}
		#endregion

		#region struct
		static public bool checkValueType<T>(IntPtr l, int p, out T v) where T:struct
		{
			v = (T) checkObj(l, p);
			return true;
		}
		#endregion

		static public bool checkNullable<T>(IntPtr l, int p, out Nullable<T> v) where T : struct
		{
			if (LuaDLL.pua_isnil(l, p))
				v = null;
			else
			{
				object o=checkVar(l, p, typeof(T));
				if (o == null) v = null;
				else v = new Nullable<T>((T)o);
			}
			return true;
		}

		#region object
		static public bool checkType<T>(IntPtr l, int p, out T o) where T:class
		{
			object obj = checkVar(l, p);
			if (obj == null)
			{
				o = null;
				return true;
			}

			o = obj as T;
			if (o == null)
				throw new Exception(string.Format("arg {0} is not type of {1}", p, typeof(T).Name));

			return true;
		}
		#endregion
	}
}
