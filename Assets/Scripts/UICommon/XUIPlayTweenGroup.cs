using AnimationOrTween;
using UILib;
using UnityEngine;

public class XUIPlayTweenGroup : MonoBehaviour,IXUIPlayTweenGroup
{
    public UIPlayTween[] m_tweenControls;
    void Awake()
    {
        if(m_tweenControls == null || m_tweenControls.Length == 0)
            m_tweenControls = GetComponentsInChildren<UIPlayTween>();
        
    }

    [ContextMenu("Execute")]
    void Excute()
    {

        ResetTween(true);
        PlayTween(false);
    }

    public void PlayTween(bool bForward)
    {
        if (m_tweenControls == null || m_tweenControls.Length == 0) return;
        for (int i = 0, length = m_tweenControls.Length; i < length; i++)
            m_tweenControls[i].Play(bForward);
    }

    public void ResetTween(bool bForward)
    {
        if (m_tweenControls == null || m_tweenControls.Length == 0) return;
        for (int i = 0, length = m_tweenControls.Length; i < length; i++)
            m_tweenControls[i].Reset(bForward);
    }

    public void StopTween()
    {
        if (m_tweenControls == null || m_tweenControls.Length == 0) return;
        for (int i = 0, length = m_tweenControls.Length; i < length; i++)
            m_tweenControls[i].Stop();
    }
}
