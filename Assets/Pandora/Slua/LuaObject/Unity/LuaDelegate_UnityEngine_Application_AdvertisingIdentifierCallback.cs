
using System;
using System.Collections.Generic;

namespace com.tencent.pandora
{
    public partial class LuaDelegation : LuaObject
    {

        static internal int checkDelegate(IntPtr l,int p,out UnityEngine.Application.AdvertisingIdentifierCallback ua) {
            int op = extractFunction(l,p);
			if(LuaDLL.pua_isnil(l,p)) {
				ua=null;
				return op;
			}
            else if (LuaDLL.pua_isuserdata(l, p)==1)
            {
                ua = (UnityEngine.Application.AdvertisingIdentifierCallback)checkObj(l, p);
                return op;
            }
            LuaDelegate ld;
            checkType(l, -1, out ld);
			LuaDLL.pua_pop(l,1);
            if(ld.d!=null)
            {
                ua = (UnityEngine.Application.AdvertisingIdentifierCallback)ld.d;
                return op;
            }
			
			l = LuaState.get(l).L;
            ua = (string a1,bool a2,string a3) =>
            {
                int error = pushTry(l);

				pushValue(l,a1);
				pushValue(l,a2);
				pushValue(l,a3);
				ld.pcall(3, error);
				LuaDLL.pua_settop(l, error-1);
			};
			ld.d=ua;
			return op;
		}
	}
}
