using AnimationOrTween;
using UILib;
using UnityEngine;

namespace Assets.Scripts.UICommon
{
    class XUITweener: MonoBehaviour, IXUITweener
    {
        public UITweener TargetTweener;
        public float Duration
        {
            get
            {
                if (m_uiTweer != null)
                {
                    return m_uiTweer.duration;
                }
                return 0;
            }
        }
        private void Awake()
        {
            if (TargetTweener != null) m_uiTweer = TargetTweener;
            else m_uiTweer = GetComponent<UITweener>();

            if (null == m_uiTweer)
            {
                Debug.LogError("null == m_uiTweer " + gameObject.name);
            }
        }
        private UITweener m_uiTweer;
    }
}
