using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.tencent.pandora
{
    public class PanelOnDestroyHook : MonoBehaviour
    {
        private void OnDestroy()
        {
            //Pandora界面销毁时给PandoraLua层派发一个消息
            //当Pandora界面被游戏直接销毁而不是通过PandoraLua层接口销毁时，提供一个PandoraLua层可以处理的回调时机
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("type", "panelDestroy");
            dict.Add("content", this.gameObject.name);
            Pandora.Instance.Do(dict);
        }
    }
}
