//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using AnimationOrTween;
using System.Collections.Generic;

/// <summary>
/// Play the specified tween on click.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Play Tween")]
public class UIPlayTween : MonoBehaviour
{
	static public UIPlayTween current;

	/// <summary>
	/// Target on which there is one or more tween.
	/// </summary>

	public GameObject tweenTarget;

	/// <summary>
	/// If there are multiple tweens, you can choose which ones get activated by changing their group.
	/// </summary>

	public int tweenGroup = 0;

	/// <summary>
	/// Which event will trigger the tween.
	/// </summary>

	public Trigger trigger = Trigger.OnClick;

	/// <summary>
	/// Direction to tween in.
	/// </summary>

	public Direction playDirection = Direction.Forward;

	/// <summary>
	/// Whether the tween will be reset to the start or end when activated. If not, it will continue from where it currently is.
	/// </summary>

	public bool resetOnPlay = false;

	/// <summary>
	/// Whether the tween will be reset to the start if it's disabled when activated.
	/// </summary>

	public bool resetIfDisabled = false;

	/// <summary>
	/// What to do if the tweenTarget game object is currently disabled.
	/// </summary>

	public EnableCondition ifDisabledOnPlay = EnableCondition.DoNothing;

	/// <summary>
	/// What to do with the tweenTarget after the tween finishes.
	/// </summary>

	public DisableCondition disableWhenFinished = DisableCondition.DoNotDisable;

	/// <summary>
	/// Whether the tweens on the child game objects will be considered.
	/// </summary>

	public bool includeChildren = false;

	/// <summary>
	/// Event delegates called when the animation finishes.
	/// </summary>

	public List<EventDelegate> onFinished = new List<EventDelegate>();

	// Deprecated functionality, kept for backwards compatibility
	[HideInInspector][SerializeField] GameObject eventReceiver;
	[HideInInspector][SerializeField] string callWhenFinished;

	//UITweener[] mTweens;
    List<UITweener> mTweens = new List<UITweener>();
	bool mStarted = false;
	int mActive = 0;
	bool mActivated = false;
    [System.NonSerialized]
    public EventDelegate finishCb;
    [System.NonSerialized]
    public EventDelegate finishEndCB;
    [System.NonSerialized]
    private UIToggle toggle;
    [System.NonSerialized]
    EventDelegate.Callback onToggle;
    [System.NonSerialized]
    EventDelegate.Callback onFinish;
	void Awake ()
	{
		// Remove deprecated functionality if new one is used
		if (eventReceiver != null && EventDelegate.IsValid(onFinished))
		{
			eventReceiver = null;
			callWhenFinished = null;
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}
        finishCb = new EventDelegate(OnFinished);
        finishEndCB = new EventDelegate(OnFinished);
        toggle = GetComponent<UIToggle>();
        if (toggle != null)
            onToggle = new EventDelegate.Callback(OnToggle);
	}

	void Start()
	{
		mStarted = true;

		if (tweenTarget == null)
		{
			tweenTarget = gameObject;
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}
	}

	void OnEnable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		if (mStarted) OnHover(UICamera.IsHighlighted(gameObject));

		if (UICamera.currentTouch != null)
		{
			if (trigger == Trigger.OnPress || trigger == Trigger.OnPressTrue)
				mActivated = (UICamera.currentTouch.pressed == gameObject);

			if (trigger == Trigger.OnHover || trigger == Trigger.OnHoverTrue)
				mActivated = (UICamera.currentTouch.current == gameObject);
		}

		//UIToggle toggle = GetComponent<UIToggle>();
        if (toggle != null) EventDelegate.Add(toggle.onChange, onToggle);
	}

	void OnDisable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		//UIToggle toggle = GetComponent<UIToggle>();
        if (toggle != null) EventDelegate.Remove(toggle.onChange, onToggle);
	}

	void OnHover (bool isOver)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnHover ||
				(trigger == Trigger.OnHoverTrue && isOver) ||
				(trigger == Trigger.OnHoverFalse && !isOver))
			{
				mActivated = isOver && (trigger == Trigger.OnHover);
				Play(isOver);
			}
		}
	}

	void OnDragOut ()
	{
		if (enabled && mActivated)
		{
			mActivated = false;
			Play(false);
		}
	}

	void OnPress (bool isPressed)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnPress ||
				(trigger == Trigger.OnPressTrue && isPressed) ||
				(trigger == Trigger.OnPressFalse && !isPressed))
			{
				mActivated = isPressed && (trigger == Trigger.OnPress);
				Play(isPressed);
			}
		}
	}

	void OnClick ()
	{
		if (enabled && trigger == Trigger.OnClick)
		{
			Play(true);
		}
	}

	void OnDoubleClick ()
	{
		if (enabled && trigger == Trigger.OnDoubleClick)
		{
			Play(true);
		}
	}

	void OnSelect (bool isSelected)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnSelect ||
				(trigger == Trigger.OnSelectTrue && isSelected) ||
				(trigger == Trigger.OnSelectFalse && !isSelected))
			{
				mActivated = isSelected && (trigger == Trigger.OnSelect);
				Play(isSelected);
			}
		}
	}

	void OnToggle ()
	{
		if (!enabled || UIToggle.current == null) return;
		if (trigger == Trigger.OnActivate ||
			(trigger == Trigger.OnActivateTrue && UIToggle.current.value) ||
			(trigger == Trigger.OnActivateFalse && !UIToggle.current.value))
			Play(UIToggle.current.value);
	}

	void Update ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
        if (NGUITools.mEnableLoadingUpdate && 
            disableWhenFinished != DisableCondition.DoNotDisable && 
            mTweens.Count > 0)
        {
            bool isFinished = true;
			bool properDirection = true;
            int count = mTweens.Count;
            for (int i = 0, imax = count; i < imax; ++i)
			{
				UITweener tw = mTweens[i];
				if (tw.tweenGroup != tweenGroup) continue;

				if (tw.enabled)
				{
					isFinished = false;
					break;
				}
				else if ((int)tw.direction != (int)disableWhenFinished)
				{
					properDirection = false;
				}
			}

			if (isFinished)
			{
				if (properDirection) NGUITools.SetActive(tweenTarget, false);
                mTweens.Clear();
				//mTweens = null;
			}
		}
	}

	/// <summary>
	/// Activate the tweeners.
	/// </summary>

	public void Play (bool forward)
	{
		mActive = 0;
		GameObject go = (tweenTarget == null) ? gameObject : tweenTarget;

		if (!NGUITools.GetActive(go))
		{
			// If the object is disabled, don't do anything
			if (ifDisabledOnPlay != EnableCondition.EnableThenPlay) return;

			// Enable the game object before tweening it
            NGUITools.SetActive(go, true,true, false);
		}

		// Gather the tweening components
        mTweens.Clear();
		if(includeChildren)
            go.GetComponentsInChildren<UITweener>(mTweens);
        else
            go.GetComponents<UITweener>(mTweens);
        int count = mTweens.Count;
		if (count == 0)
		{
			// No tweeners found -- should we disable the object?
			if (disableWhenFinished != DisableCondition.DoNotDisable)
				NGUITools.SetActive(tweenTarget, false);
		}
		else
		{
			bool activated = false;
			if (playDirection == Direction.Reverse) forward = !forward;

			// Run through all located tween components
           
			for (int i = 0, imax = count; i < imax; ++i)
			{
			    if (mTweens == null) break;

			    if (i >= count) break;

				UITweener tw = mTweens[i];

				// If the tweener's group matches, we can work with it
				if (tw.tweenGroup == tweenGroup)
				{
					// Ensure that the game objects are enabled
					if (!activated && !NGUITools.GetActive(go))
					{
						activated = true;
                        NGUITools.SetActive(go, true, true, false);
					}

					++mActive;

					// Toggle or activate the tween component
					if (playDirection == Direction.Toggle)
					{
						// Listen for tween finished messages
                        EventDelegate.Add(tw.onFinished, finishCb, true);
                        EventDelegate.Add(tw.onFinished, finishEndCB, true);
                        tw.Toggle();
					}
					else
					{
                        if (resetOnPlay || (resetIfDisabled && !tw.enabled)) tw.ResetToBeginning(forward);
						// Listen for tween finished messages
                        EventDelegate.Add(tw.onFinished, finishCb, true);
                        EventDelegate.Add(tw.onFinished, finishEndCB, true);
                        tw.Play(forward);
					}
				}
			}
		}
	}

	/// <summary>
	/// Callback triggered when each tween executed by this script finishes.
	/// </summary>

	void OnFinished ()
	{
		if (--mActive == 0 && current == null)
		{
			current = this;
			EventDelegate.Execute(onFinished);

			// Legacy functionality
			if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
				eventReceiver.SendMessage(callWhenFinished, SendMessageOptions.DontRequireReceiver);

			eventReceiver = null;
			current = null;
		}
	}

    public void Reset(bool forward)
    {
        GameObject go = (tweenTarget == null) ? gameObject : tweenTarget;
        mTweens.Clear();
        if (includeChildren)
        {
            go.GetComponentsInChildren<UITweener>(mTweens);
        }
        else
        {
            go.GetComponents<UITweener>(mTweens);
        }
        int count = mTweens.Count;
        for (int i = 0, imax = count; i < imax; ++i)
        {
            mTweens[i].enabled = false;
            mTweens[i].ResetToBeginning(forward);
        }
    }

    public void ResetByGroup(bool forward, int resetGroup)
    {
    	GameObject go = (tweenTarget == null) ? gameObject : tweenTarget;
        mTweens.Clear();
        if (includeChildren)
        {
            go.GetComponentsInChildren<UITweener>(mTweens);
        }
        else
        {
            go.GetComponents<UITweener>(mTweens);
        }
        int count = mTweens.Count;
        for (int i = 0, imax = count; i < imax; ++i)
        {
        	if(mTweens[i].tweenGroup != resetGroup) continue;
            mTweens[i].enabled = false;
            mTweens[i].ResetToBeginning(forward);
        }
    }

    public void ResetExceptGroup(bool forward, int exceptGroup)
    {
    	GameObject go = (tweenTarget == null) ? gameObject : tweenTarget;
        mTweens.Clear();
        if (includeChildren)
        {
            go.GetComponentsInChildren<UITweener>(mTweens);
        }
        else
        {
            go.GetComponents<UITweener>(mTweens);
        }
        int count = mTweens.Count;
        for (int i = 0, imax = count; i < imax; ++i)
        {
        	if(mTweens[i].tweenGroup == exceptGroup) continue;
            mTweens[i].enabled = false;
            mTweens[i].ResetToBeginning(forward);
        }
    }

    public void Stop()
    {
        GameObject go = (tweenTarget == null) ? gameObject : tweenTarget;
        mTweens.Clear();
        if (includeChildren)
        {
            go.GetComponentsInChildren<UITweener>(mTweens);
        }
        else
        {
            go.GetComponents<UITweener>(mTweens);
        }
        int count = mTweens.Count;
        for (int i = 0, imax = count; i < imax; ++i)
        {
            mTweens[i].enabled = false;
        }
    }

    public void StopByGroup(int resetGroup)
    {
        GameObject go = (tweenTarget == null) ? gameObject : tweenTarget;
        mTweens.Clear();
        if (includeChildren)
        {
            go.GetComponentsInChildren<UITweener>(mTweens);
        }
        else
        {
            go.GetComponents<UITweener>(mTweens);
        }
        int count = mTweens.Count;
        for (int i = 0, imax = count; i < imax; ++i)
        {
            if (mTweens[i].tweenGroup != resetGroup) continue;
            mTweens[i].enabled = false;
        }
    }

    public void StopExceptGroup(int exceptGroup)
    {
        GameObject go = (tweenTarget == null) ? gameObject : tweenTarget;
        mTweens.Clear();
        if (includeChildren)
        {
            go.GetComponentsInChildren<UITweener>(mTweens);
        }
        else
        {
            go.GetComponents<UITweener>(mTweens);
        }
        int count = mTweens.Count;
        for (int i = 0, imax = count; i < imax; ++i)
        {
            if (mTweens[i].tweenGroup == exceptGroup) continue;
            mTweens[i].enabled = false;
        }
    }
}
