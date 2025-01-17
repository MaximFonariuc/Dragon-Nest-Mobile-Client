using AnimationOrTween;
using UILib;
using UnityEngine;

public class XUIPlayTween : MonoBehaviour, IXUITweenTool
{
    private bool m_bPlayForward;
    public bool bPlayForward { get { return m_bPlayForward; } }
    public int TweenGroup { get { return m_uiPlayTween.tweenGroup; } }

    private void Awake()
    {
        m_uiPlayTween = GetComponent<UIPlayTween>();

        if (null == m_uiPlayTween)
        {
            Debug.LogError("null == m_uiPlayTween " + gameObject.name);
        }
        oneCycleFinish = new EventDelegate(OnOneCycleFinish);
    }

    [ContextMenu("Execute")]
    public void Excute()
    {
        ResetTween(true);
        PlayTween(true);
    }

    public void PlayTween(bool bForward, float duaration = -1.0f)
    {
        //if (null != m_objectToPlay)
        //    m_uiPlayTween.tweenTarget = m_objectToPlay;

        m_uiPlayTween.Play(bForward);
        m_bPlayForward = bForward;

        if (duaration > 0.0f)
        {
            m_repeatStartTime = Time.time;
            m_repeatDuaration = duaration;
            EventDelegate.Add(m_uiPlayTween.onFinished, oneCycleFinish);
        }
    }

    public void ResetTween(bool bForward)
    {
        m_uiPlayTween.Reset(bForward);
    }

    public void ResetTweenByGroup(bool bForward, int resetGroup)
    {
    	m_uiPlayTween.ResetByGroup(bForward, resetGroup);
    }

    public void ResetTweenByCurGroup(bool bForward)
    {
        m_uiPlayTween.ResetByGroup(bForward, m_uiPlayTween.tweenGroup);
    }

    public void ResetTweenExceptGroup(bool bForward, int exceptGroup)
    {
        m_uiPlayTween.ResetExceptGroup(bForward, exceptGroup);
    }

    public void StopTween()
    {
        m_uiPlayTween.Stop();
    }

    public void StopTweenByGroup(int resetGroup)
    {
        m_uiPlayTween.StopByGroup(resetGroup);
    }

    public void StopTweenExceptGroup(int exceptGroup)
    {
        m_uiPlayTween.StopExceptGroup(exceptGroup);
    }

    public void SetTargetGameObject(GameObject go)
    {
        //m_objectToPlay = go;
        m_uiPlayTween.tweenTarget = go;
    }

    public void SetPositionTweenPos(int group, Vector3 from, Vector3 to)
    {
        GameObject go = (m_uiPlayTween.tweenTarget == null) ? gameObject : m_uiPlayTween.tweenTarget;
        TweenPosition[] _TweenPosition = go.GetComponents<TweenPosition>();

        for (int i = 0; i < _TweenPosition.Length; i++)
        {
            if (_TweenPosition[i].tweenGroup == group)
            {
                _TweenPosition[i].from = from;
                _TweenPosition[i].to = to;
            }
        }
    }

    public void SetTweenGroup(int group)
    {
        m_uiPlayTween.tweenGroup = group;
    }

    public void SetTweenEnabledWhenFinish(bool enabled)
    {
        if (enabled)
        {
            m_uiPlayTween.disableWhenFinished = DisableCondition.DoNotDisable;
        }
        else
        {
            m_uiPlayTween.disableWhenFinished = DisableCondition.DisableAfterForward;
        }
    }

    public void RegisterOnFinishEventHandler(OnTweenFinishEventHandler eventHandler)
    {
        m_OnFinishHandler = eventHandler;

       // m_uiPlayTween.customFinishCallback = OnFinish;

        EventDelegate.Add(m_uiPlayTween.onFinished, OnFinish);
    }

    void OnFinish()
    {
        if (m_OnFinishHandler != null)
        {
            m_OnFinishHandler(this);
        }
    }

    // 指定tween的时间长度时，每播完一次，跑到这里，如果还有剩余时间，继续跑
    void OnOneCycleFinish()
    {
        if (Time.time - m_repeatStartTime < m_repeatDuaration)
            m_uiPlayTween.Play(m_bPlayForward);
        else
        {
            EventDelegate.Remove(m_uiPlayTween.onFinished, oneCycleFinish);
        }
    }

    private UIPlayTween m_uiPlayTween;
    private GameObject m_objectToPlay;
    private OnTweenFinishEventHandler m_OnFinishHandler = null;

    private EventDelegate oneCycleFinish;
    private float m_repeatStartTime;  // 重复播放的开始时间
    private float m_repeatDuaration;  // 重复播放的时间长度
}
