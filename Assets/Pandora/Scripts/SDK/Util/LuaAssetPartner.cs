using UnityEngine;
using System.Collections.Generic;

namespace com.tencent.pandora
{
    /// <summary>
    /// 用来保存lua代码资源的组件
    /// </summary>
    public class LuaAssetPartner : MonoBehaviour
    {
        public List<string> listLuaFileName = new List<string>();
		public List<string> listLuaContent = new List<string>();
    }
}

