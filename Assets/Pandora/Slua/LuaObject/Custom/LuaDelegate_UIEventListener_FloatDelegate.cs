﻿
using System;
using System.Collections.Generic;

namespace com.tencent.pandora
{
    public partial class LuaDelegation : LuaObject
    {

        static internal int checkDelegate(IntPtr l,int p,out UIEventListener.FloatDelegate ua) {
            int op = extractFunction(l,p);
			if(LuaDLL.pua_isnil(l,p)) {
				ua=null;
				return op;
			}
            else if (LuaDLL.pua_isuserdata(l, p)==1)
            {
                ua = (UIEventListener.FloatDelegate)checkObj(l, p);
                return op;
            }
            LuaDelegate ld;
            checkType(l, -1, out ld);
			LuaDLL.pua_pop(l,1);
            if(ld.d!=null)
            {
                ua = (UIEventListener.FloatDelegate)ld.d;
                return op;
            }
			
			l = LuaState.get(l).L;
            ua = (UnityEngine.GameObject a1,float a2) =>
            {
                int error = pushTry(l);

				pushValue(l,a1);
				pushValue(l,a2);
				ld.pcall(2, error);
				LuaDLL.pua_settop(l, error-1);
			};
			ld.d=ua;
			return op;
		}
	}
}
