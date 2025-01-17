//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's position.
/// </summary>

[AddComponentMenu("NGUI/Tween/Tween Position")]
public class TweenPosition : UITweener
{
	public Vector3 from;
	public Vector3 to;

    // used when x,y need different curves, we can use 2 tween position, each hold a curve
    public bool nox = false;
    public bool noy = false;
    
	[HideInInspector]
	public bool worldSpace = false;

	Transform mTrans;
	UIRect mRect;

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	[System.Obsolete("Use 'value' instead")]
	public Vector3 position { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public Vector3 value
	{
		get
		{
			return worldSpace ? cachedTransform.position : cachedTransform.localPosition;
		}
		set
		{
			if (mRect == null || !mRect.isAnchored || worldSpace)
			{
			    if (worldSpace)
			    {
                    Vector3 preVector3 = cachedTransform.position;

			        cachedTransform.position.Set(
                        nox ? preVector3.x : value.x, 
                        noy ? preVector3.y : value.y,
                        value.z);
			    }
			    else
			    {
			        Vector3 preVector3 = cachedTransform.localPosition;
			        float d1 = (nox ? preVector3.x : value.x);
			        float d2 = (noy ? preVector3.y : value.y);
                    cachedTransform.localPosition = new Vector3(d1, d2, value.z);
			    }
			}
			else
			{
				value -= cachedTransform.localPosition;

                value.Set(
                    nox ? 0 : value.x,
                    noy ? 0 : value.y,
                    value.z);
                
				NGUIMath.MoveRect(mRect, value.x, value.y);
			}
		}
	}

	void Awake () { mRect = GetComponent<UIRect>(); }

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = from * (1f - factor) + to * factor; }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenPosition Begin (GameObject go, float duration, Vector3 pos)
	{
		TweenPosition comp = UITweener.Begin<TweenPosition>(go, duration);
		comp.from = comp.value;
		comp.to = pos;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}

	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue () { from = value; }

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue () { to = value; }

	[ContextMenu("Assume value of 'From'")]
	void SetCurrentValueToStart () { value = from; }

	[ContextMenu("Assume value of 'To'")]
	void SetCurrentValueToEnd () { value = to; }
}
