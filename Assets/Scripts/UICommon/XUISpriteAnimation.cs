using System;
using UILib;
using UnityEngine;

public delegate void SpriteAnimationFinishEventHandler();
public class XUISpriteAnimation : XUIObject, IXUISpriteAnimation
{
    protected override void OnAwake()
    {
        base.OnAwake();

        m_Animation = GetComponent<UISpriteAnimation>();
        if (m_Animation == null)
        {
            Debug.Log("no UISpriteAnimation component");
        }
    }

    public void SetNamePrefix(string name)
    {
        m_Animation.namePrefix = name;
        m_Animation.Reset();
    }

    public void SetNamePrefix(string atlas, string name)
    {
        m_Animation.sprite.SetAtlas("atlas/UI/" + atlas);
        SetNamePrefix(name);
    }

    public void SetFrameRate(int rate)
    {
        m_Animation.framesPerSecond = rate;
    }

    public void Reset()
    {
        m_Animation.Reset();
        m_Animation.LastLoopFinishTime = RealTime.time;
    }

    public void StopAndReset()
    {
        m_Animation.StopAndReset();
    }

    public void RegisterFinishCallback(SpriteAnimationFinishCallback callback)
    {
        if (callback != null)
        {
            m_FinishCallback = callback;
            m_Animation.FinishHandler = _SpriteAnimationFinished;
        }
    }

    public void MakePixelPerfect()
    {
        if (m_Animation != null && m_Animation.sprite != null)
            m_Animation.sprite.MakePixelPerfect();
    }

    public void _SpriteAnimationFinished()
    {
        if (m_FinishCallback != null)
            m_FinishCallback(this);
    }

   

    private UISpriteAnimation m_Animation;
    SpriteAnimationFinishCallback m_FinishCallback;


}
