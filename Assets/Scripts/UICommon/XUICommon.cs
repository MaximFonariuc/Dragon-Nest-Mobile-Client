using UnityEngine;
using XUtliPoolLib;

public class XUICommon : XSingleton<XUICommon>
{
    public UIPanel m_uiRootPanel;
    public UIPanel m_inVisiablePanel;
    public void Init(Transform uiRoot)
    {
        m_uiRootPanel = uiRoot.GetComponent<UIPanel>();
        m_uiRootPanel.updateFrame = 3;

        m_inVisiablePanel = null;
        Transform inVisablePanel = m_uiRootPanel != null ? m_uiRootPanel.transform.Find("InVisiblePanel") : null;
        if (inVisablePanel != null)
        {
            m_inVisiablePanel = inVisablePanel.GetComponent<UIPanel>();
        }
    }
    public void SetRootPanelUpdateFreq(int count)
    {
        m_uiRootPanel.updateFrame = count;
    }
    public static void CloneTplTweens(GameObject tpl, GameObject clone)
    {
        if (clone.GetComponent<UIPlayTween>() != null) return;

        UIPlayTween[] tweens = tpl.GetComponents<UIPlayTween>();

        for (int i=0;i<tweens.Length;++i)
        {
            UIPlayTween tween = tweens[i];
            UIPlayTween t = clone.AddComponent<UIPlayTween>();
            t.tweenGroup = tween.tweenGroup;
            t.trigger = tween.trigger;
            t.playDirection = tween.playDirection;
            t.resetIfDisabled = tween.resetIfDisabled;
            t.resetOnPlay = tween.resetOnPlay;
            t.ifDisabledOnPlay = tween.ifDisabledOnPlay;
            t.disableWhenFinished = tween.disableWhenFinished;
        }

        TweenScale[] scales = tpl.GetComponents<TweenScale>();

        for (int i = 0; i < scales.Length; ++i)
        {
            TweenScale tweenScale = scales[i];
            TweenScale s = clone.AddComponent<TweenScale>();
            s.from = tweenScale.from;
            s.to = tweenScale.to;
            s.style = tweenScale.style;
            s.animationCurve = tweenScale.animationCurve;
            s.duration = tweenScale.duration;
            s.delay = tweenScale.delay;
            s.tweenGroup = tweenScale.tweenGroup;
            s.ignoreTimeScale = tweenScale.ignoreTimeScale;

            s.enabled = false;
        }
    }

   
}
