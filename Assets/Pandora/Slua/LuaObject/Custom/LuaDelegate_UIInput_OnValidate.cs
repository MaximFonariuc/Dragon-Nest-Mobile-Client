﻿
using System;
using System.Collections.Generic;

namespace com.tencent.pandora
{
    public partial class LuaDelegation : LuaObject
    {

        static internal int checkDelegate(IntPtr l,int p,out UIInput.OnValidate ua) {
            int op = extractFunction(l,p);
			if(LuaDLL.pua_isnil(l,p)) {
				ua=null;
				return op;
			}
            else if (LuaDLL.pua_isuserdata(l, p)==1)
            {
                ua = (UIInput.OnValidate)checkObj(l, p);
                return op;
            }
            LuaDelegate ld;
            checkType(l, -1, out ld);
			LuaDLL.pua_pop(l,1);
            if(ld.d!=null)
            {
                ua = (UIInput.OnValidate)ld.d;
                return op;
            }
			
			l = LuaState.get(l).L;
            ua = (string a1,int a2,System.Char a3) =>
            {
                int error = pushTry(l);

				pushValue(l,a1);
				pushValue(l,a2);
				pushValue(l,a3);
				ld.pcall(3, error);
				System.Char ret;
				checkType(l,error+1,out ret);
				LuaDLL.pua_settop(l, error-1);
				return ret;
			};
			ld.d=ua;
			return op;
		}
	}
}
