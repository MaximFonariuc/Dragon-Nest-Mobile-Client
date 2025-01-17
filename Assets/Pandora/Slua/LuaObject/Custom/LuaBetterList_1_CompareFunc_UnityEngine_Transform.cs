
using System;
using System.Collections.Generic;

namespace com.tencent.pandora
{
    public partial class LuaDelegation : LuaObject
    {

        static internal int checkDelegate(IntPtr l,int p,out BetterList<UnityEngine.Transform>.CompareFunc ua) {
            int op = extractFunction(l,p);
			if(LuaDLL.pua_isnil(l,p)) {
				ua=null;
				return op;
			}
            else if (LuaDLL.pua_isuserdata(l, p)==1)
            {
                ua = (BetterList<UnityEngine.Transform>.CompareFunc)checkObj(l, p);
                return op;
            }
            LuaDelegate ld;
            checkType(l, -1, out ld);
			LuaDLL.pua_pop(l,1);
            if(ld.d!=null)
            {
                ua = (BetterList<UnityEngine.Transform>.CompareFunc)ld.d;
                return op;
            }
			
			l = LuaState.get(l).L;
            ua = (UnityEngine.Transform a1,UnityEngine.Transform a2) =>
            {
                int error = pushTry(l);

				pushValue(l,a1);
				pushValue(l,a2);
				ld.pcall(2, error);
				int ret;
				checkType(l,error+1,out ret);
				LuaDLL.pua_settop(l, error-1);
				return ret;
			};
			ld.d=ua;
			return op;
		}
	}
}
