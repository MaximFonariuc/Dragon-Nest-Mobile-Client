using UnityEngine;
using System.Collections;
using XUtliPoolLib;

public class XDlgControler : MonoBehaviour
{
    public GameObject m_CachedDlg;

    private IXGameUI m_gameui = null;
    //private bool bFirstFrame;
    void Awake()
    {
        if (m_CachedDlg == null)
        {
            m_CachedDlg = gameObject;
        }

    	if(m_CachedDlg.GetComponent<UISprite>() == null)
    	{
            m_CachedDlg.AddComponent<UISprite>();
    	}

    	transform.GetComponent<UIPlayTween>().tweenTarget = m_CachedDlg;

        m_gameui = XInterfaceMgr.singleton.GetInterface<IXGameUI>(XCommon.singleton.XHash("XGameUI"));
        GameObject tpl = m_gameui.DlgControllerTpl;

        bool group1 = false;
        bool group2 = false;

        TweenScale[] scales = m_CachedDlg.GetComponents<TweenScale>();
        for (int i = 0; i < scales.Length; ++i)
        {
            if (scales[i].tweenGroup == 1) group1 = true;
            if (scales[i].tweenGroup == 2) group2 = true;
        }

        scales = tpl.GetComponents<TweenScale>();
        for (int i = 0; i < scales.Length; ++i)
        {
            if (group1 && scales[i].tweenGroup == 1) continue;
            if (group2 && scales[i].tweenGroup == 2) continue;
            TweenScale s = m_CachedDlg.AddComponent<TweenScale>();
            s.from = scales[i].from;
            s.to = scales[i].to;
            s.style = scales[i].style;
            s.animationCurve = scales[i].animationCurve;
            s.duration = scales[i].duration;
            s.delay = scales[i].delay;
            s.tweenGroup = scales[i].tweenGroup;
            s.ignoreTimeScale = scales[i].ignoreTimeScale;

            s.enabled = false;
        }

        group1 = false;
        group2 = false;
        TweenPosition[] positions = m_CachedDlg.GetComponents<TweenPosition>();
        for (int i = 0; i < positions.Length; ++i)
        {
            if (positions[i].tweenGroup == 1) group1 = true;
            if (positions[i].tweenGroup == 2) group2 = true;
        }

        positions = tpl.GetComponents<TweenPosition>();
        for (int i = 0; i < positions.Length; ++i)
        {
            if (group1 && positions[i].tweenGroup == 1) continue;
            if (group2 && positions[i].tweenGroup == 2) continue;
            TweenPosition p = m_CachedDlg.AddComponent<TweenPosition>();
            p.from = positions[i].from;
            p.to = positions[i].to;
            p.style = positions[i].style;
            p.animationCurve = positions[i].animationCurve;
            p.duration = positions[i].duration;
            p.delay = positions[i].delay;
            p.tweenGroup = positions[i].tweenGroup;
            p.ignoreTimeScale = positions[i].ignoreTimeScale;

            p.enabled = false;
        }

        group1 = false;
        group2 = false;
        TweenAlpha[] alphas = m_CachedDlg.GetComponents<TweenAlpha>();
        for (int i = 0; i < alphas.Length; ++i)
        {
            if (alphas[i].tweenGroup == 1) group1 = true;
            if (alphas[i].tweenGroup == 2) group2 = true;
        }

        alphas = tpl.GetComponents<TweenAlpha>();
        for (int i = 0; i < alphas.Length; ++i)
        {
            if (group1 && alphas[i].tweenGroup == 1) continue;
            if (group2 && alphas[i].tweenGroup == 2) continue;
            TweenAlpha a = m_CachedDlg.AddComponent<TweenAlpha>();
            a.from = alphas[i].from;
            a.to = alphas[i].to;
            a.style = alphas[i].style;
            a.animationCurve = alphas[i].animationCurve;
            a.duration = alphas[i].duration;
            a.delay = alphas[i].delay;
            a.tweenGroup = alphas[i].tweenGroup;
            a.ignoreTimeScale = alphas[i].ignoreTimeScale;

            a.enabled = false;
        }
    }
}
