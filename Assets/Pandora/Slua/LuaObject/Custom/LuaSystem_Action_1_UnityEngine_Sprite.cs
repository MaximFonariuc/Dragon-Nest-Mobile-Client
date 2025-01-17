
using System;
using System.Collections.Generic;

namespace com.tencent.pandora
{
    public partial class LuaDelegation : LuaObject
    {

        static internal int checkDelegate(IntPtr l,int p,out System.Action<UnityEngine.Sprite> ua) {
            int op = extractFunction(l,p);
			if(LuaDLL.pua_isnil(l,p)) {
				ua=null;
				return op;
			}
            else if (LuaDLL.pua_isuserdata(l, p)==1)
            {
                ua = (System.Action<UnityEngine.Sprite>)checkObj(l, p);
                return op;
            }
            LuaDelegate ld;
            checkType(l, -1, out ld);
			LuaDLL.pua_pop(l,1);
            if(ld.d!=null)
            {
                ua = (System.Action<UnityEngine.Sprite>)ld.d;
                return op;
            }
			
			l = LuaState.get(l).L;
            ua = (UnityEngine.Sprite a1) =>
            {
                int error = pushTry(l);

				pushValue(l,a1);
				ld.pcall(1, error);
				LuaDLL.pua_settop(l, error-1);
			};
			ld.d=ua;
			return op;
		}
	}
}
