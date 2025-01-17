//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Simple progress bar that fills itself based on the specified value.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/NGUI Progress Bar")]
public class UIProgressBar : UIWidgetContainer
{
	public enum FillDirection
	{
		LeftToRight,
		RightToLeft,
		BottomToTop,
		TopToBottom,
	}

	/// <summary>
	/// Current slider. This value is set prior to the callback function being triggered.
	/// </summary>

	static public UIProgressBar current;

	/// <summary>
	/// Delegate triggered when the scroll bar stops being dragged.
	/// Useful for things like centering on the closest valid object, for example.
	/// </summary>
    public bool bHideThumbAtEnds = true;

	public OnDragFinished onDragFinished;
	public delegate void OnDragFinished ();

    /// <summary>
    /// the Foreground sprite to be enabled when the value equals zero if bHideFgAtEnds = true
    /// adder:luo da shuai 2016/7/12
    /// </summary>
    public bool bHideFgAtEnds = true;

    /// <summary>
    /// the Foreground sprite need to use fill direction radial 360, but ForceUpdate reset it.
    /// adder:pyc 2016/11/4
    /// </summary>
    public bool UseFillDir = false;

	/// <summary>
	/// Object that acts as a thumb.
	/// </summary>

	public Transform thumb;

	[HideInInspector][SerializeField] public UIWidget mBG;
	[HideInInspector][SerializeField] public UIWidget mFG;
    [HideInInspector][SerializeField] public UIWidget mDG;
    [HideInInspector][SerializeField] protected GameObject mFullFx;
    [HideInInspector][SerializeField] protected GameObject mFx;

	[HideInInspector][SerializeField] protected float mValue = 1f;
	[HideInInspector][SerializeField] protected FillDirection mFill = FillDirection.LeftToRight;
    //[HideInInspector][SerializeField] protected float mDynamicThreshold = 0.005f;

	protected Transform mTrans;
	protected bool mIsDirty = false;
	protected Camera mCam;
	protected float mOffset = 0f;

    //protected bool mIsShowDG = false;
    protected float mDynamicVal = 0f;

	/// <summary>
	/// Number of steps the slider should be divided into. For example 5 means possible values of 0, 0.25, 0.5, 0.75, and 1.0.
	/// </summary>

	public int numberOfSteps = 0;

	/// <summary>
	/// Callbacks triggered when the scroll bar's value changes.
	/// </summary>

	public List<EventDelegate> onChange = new List<EventDelegate>();

    private UIWidget thumbw;
    private Collider thumbCol;
	/// <summary>
	/// Cached for speed.
	/// </summary>

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	/// <summary>
	/// Camera used to draw the scroll bar.
	/// </summary>

	public Camera cachedCamera { get { if (mCam == null) mCam = NGUITools.FindCameraForLayer(gameObject.layer); return mCam; } }

	/// <summary>
	/// Widget used for the foreground.
	/// </summary>

	public UIWidget foregroundWidget { get { return mFG; } set { if (mFG != value) { mFG = value; mIsDirty = true; } } }

	/// <summary>
	/// Widget used for the background.
	/// </summary>

	public UIWidget backgroundWidget { get { return mBG; } set { if (mBG != value) { mBG = value; mIsDirty = true; } } }

	/// <summary>
	/// The scroll bar's direction.
	/// </summary>

	public FillDirection fillDirection
	{
		get
		{
			return mFill;
		}
		set
		{
			if (mFill != value)
			{
				mFill = value;
				ForceUpdate();
			}
		}
	}

	/// <summary>
	/// Modifiable value for the scroll bar, 0-1 range.
	/// </summary>

	public float value
	{
		get
		{
			if (numberOfSteps > 1) return Mathf.Round(mValue * (numberOfSteps - 1)) / (numberOfSteps - 1);
			return mValue;
		}
		set
		{
			float val = Mathf.Clamp01(value);

			if (mValue != val)
			{
				float before = this.value;
				mValue = val;

				if (before != this.value)
				{
					//ForceUpdate();
				    mIsDirty = true;

                    // modified by rongxutao
                    // if foreground is a sliced image, value 0 will make a minwidth > 0 sprite stay, so disable it
                    if (val == 0.0f)
				    {
				        if(bHideFgAtEnds) mFG.enabled = false;
                        if (thumb != null && bHideThumbAtEnds)  thumb.gameObject.SetActive(false);
				    }
                    else if (val == 1.0f)
                    {
                        mFG.enabled = true;
                        if (thumb != null && bHideThumbAtEnds) thumb.gameObject.SetActive(false);
                    }
				    else
				    {
				        mFG.enabled = true;
                        if (thumb != null && bHideThumbAtEnds) thumb.gameObject.SetActive(true);
				    }

				    if (mFullFx != null)
				    {
                        if (val == 1.0f)
                        {
                            mFullFx.SetActive(true);
                        }
                        else
                        {
                            mFullFx.SetActive(false);
                        } 
				    }
				   
                    //if (before - val > mDynamicThreshold)
                    //{
                    //    mIsShowDG = true;
                    //    mDynamicStartVal = before;
                    //    mIsDirty = true;
                    //}

					if (current == null && NGUITools.GetActive(this) && EventDelegate.IsValid(onChange))
					{
						current = this;
						EventDelegate.Execute(onChange);
						current = null;
					}
				}
#if UNITY_EDITOR
				if (!Application.isPlaying)
					NGUITools.SetDirty(this);
#endif
			}
		}
	}

	/// <summary>
	/// Allows to easily change the scroll bar's alpha, affecting both the foreground and the background sprite at once.
	/// </summary>

	public float alpha
	{
		get
		{
			if (mFG != null) return mFG.alpha;
			if (mBG != null) return mBG.alpha;
			return 1f;
		}
		set
		{
			if (mFG != null)
			{
                BoxCollider bc = mFG.DefaultBoxCollider;
				mFG.alpha = value;
                if (bc != null) bc.enabled = mFG.alpha > 0.001f;
			}

			if (mBG != null)
			{
                BoxCollider bc = mBG.DefaultBoxCollider;
				mBG.alpha = value;
                if (bc != null) bc.enabled = mBG.alpha > 0.001f;
			}

			//if (thumb != null)
			{
				//UIWidget w = thumb.GetComponent<UIWidget>();

                if (thumbw != null)
				{
                    thumbw.alpha = value;
                    if (thumbCol != null)
                        thumbCol.enabled = thumbw.alpha > 0.001f;
				}
			}
		}
	}

	/// <summary>
	/// Whether the progress bar is horizontal in nature. Convenience function.
	/// </summary>

	protected bool isHorizontal { get { return (mFill == FillDirection.LeftToRight || mFill == FillDirection.RightToLeft); } }

	/// <summary>
	/// Whether the progress bar is inverted in its behaviour. Convenience function.
	/// </summary>

	protected bool isInverted { get { return (mFill == FillDirection.RightToLeft || mFill == FillDirection.TopToBottom); } }

	/// <summary>
	/// Register the event listeners.
	/// </summary>

	protected void Start ()
	{
        Upgrade();
        if (thumb != null)
        {
            thumbw = thumb.GetComponent<UIWidget>();
            thumbCol = thumbw.GetComponent<Collider>();
        }
           
		if (Application.isPlaying)
		{
			if (mFG == null)
			{
				//Debug.LogWarning("Progress bar needs a foreground widget to work with", this);
				enabled = false;
				return;
			}

			if (mBG != null) mBG.autoResizeBoxCollider = true;

			OnStart();

			if (current == null && onChange != null)
			{
				current = this;
				EventDelegate.Execute(onChange);
				current = null;
			}
		}
		ForceUpdate();
	}

	/// <summary>
	/// Used to upgrade from legacy functionality.
	/// </summary>

	protected virtual void Upgrade () { }

	/// <summary>
	/// Functionality for derived classes.
	/// </summary>

	protected virtual void OnStart() { }

	/// <summary>
	/// Update the value of the scroll bar if necessary.
	/// </summary>

	protected void Update ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		if (mIsDirty) ForceUpdate();
	}

	/// <summary>
	/// Invalidate the scroll bar.
	/// </summary>

	protected void OnValidate ()
	{
		// For some bizarre reason Unity calls this function on prefabs, even if prefabs
		// are not actually used in the scene, nor selected in inspector. Dafuq?
		if (NGUITools.GetActive(this))
		{
			Upgrade();
			mIsDirty = true;
			float val = Mathf.Clamp01(mValue);
			if (mValue != val) mValue = val;
			if (numberOfSteps < 0) numberOfSteps = 0;
			else if (numberOfSteps > 20) numberOfSteps = 20;
			ForceUpdate();
		}
		else
		{
			float val = Mathf.Clamp01(mValue);
			if (mValue != val) mValue = val;
			if (numberOfSteps < 0) numberOfSteps = 0;
			else if (numberOfSteps > 20) numberOfSteps = 20;
		}
	}

	/// <summary>
	/// Drag the scroll bar by the specified on-screen amount.
	/// </summary>

	protected float ScreenToValue (Vector2 screenPos)
	{
		// Create a plane
		Transform trans = cachedTransform;
		Plane plane = new Plane(trans.rotation * Vector3.back, trans.position);

		// If the ray doesn't hit the plane, do nothing
		float dist;
		Ray ray = cachedCamera.ScreenPointToRay(screenPos);
		if (!plane.Raycast(ray, out dist)) return value;

		// Transform the point from world space to local space
		return LocalToValue(trans.InverseTransformPoint(ray.GetPoint(dist)));
	}

	/// <summary>
	/// Calculate the value of the progress bar given the specified local position.
	/// </summary>

	protected virtual float LocalToValue (Vector2 localPos)
	{
		if (mFG != null)
		{
			Vector3[] corners = mFG.localCorners;
			Vector3 size = (corners[2] - corners[0]);

			if (isHorizontal)
			{
				float diff = (localPos.x - corners[0].x) / size.x;
				return isInverted ? 1f - diff : diff;
			}
			else
			{
				float diff = (localPos.y - corners[0].y) / size.y;
				return isInverted ? 1f - diff : diff;
			}
		}
		return value;
	}

	/// <summary>
	/// Update the value of the scroll bar.
	/// </summary>

	public virtual void ForceUpdate ()
	{
		mIsDirty = false;
        
		if (mFG != null)
		{
			UISprite sprite = mFG as UISprite;

			if (isHorizontal)
			{
				if (sprite != null && sprite.type == UISprite.Type.Filled)
				{
					sprite.fillDirection = UISprite.FillDirection.Horizontal;
					sprite.invert = isInverted;
					sprite.fillAmount = value;
				}
				else
				{
					mFG.drawRegion = isInverted ?
						new Vector4(1f - value, 0f, 1f, 1f) :
						new Vector4(0f, 0f, value, 1f);

				    if (mFx != null)
				    {
                        mFx.SetActive(true);
                        UIWidget w = mFx.GetComponentInChildren<UIWidget>();
				        if (w != null)
				        {
				            w.drawRegion = mFG.drawRegion;
				        }
				    }
				}

			    if (mDG != null)
			    {
                    if (mDynamicVal > 0)
                    {
                        mDG.gameObject.SetActive(true);
                        mDG.drawRegion = isInverted
                            ? new Vector4(1f - mDynamicVal, 0f, 1f, 1f)
                            : new Vector4(0f, 0f, mDynamicVal, 1f);
                    }
                    else
                    {
                        mDG.gameObject.SetActive(false);
                    }
			    }
                

                //if (mIsShowDG == true && mDG != null && !isInverted)
                //{
                //    mDG.gameObject.SetActive(true);
                //    mDG.drawRegion = new Vector4(0f, 0f, mDynamicStartVal, 1f);
                //    mDynamicStartVal -= RealTime.deltaTime;
                //    mIsDirty = true;

                //    if (mDynamicStartVal <= this.value)
                //    {
                //        mDG.gameObject.SetActive(false);
                //        mIsShowDG = false;
                //    }
                //}
               
			}
			else if (sprite != null && sprite.type == UISprite.Type.Filled)
			{
                if (!UseFillDir)
                {
                    sprite.fillDirection = UISprite.FillDirection.Vertical;
                    sprite.invert = isInverted;
                }
				sprite.fillAmount = value;
			}
			else
			{
				mFG.drawRegion = isInverted ?
					new Vector4(0f, 1f - value, 1f, 1f) :
					new Vector4(0f, 0f, 1f, value);
			}
		}

		if (thumb != null && (mFG != null || mBG != null))
		{
			Vector3[] corners = (mFG != null) ? mFG.localCorners : mBG.localCorners;

			Vector4 br = (mFG != null) ? mFG.border : mBG.border;
			corners[0].x += br.x;
			corners[1].x += br.x;
			corners[2].x -= br.z;
			corners[3].x -= br.z;

			corners[0].y += br.y;
			corners[1].y -= br.w;
			corners[2].y -= br.w;
			corners[3].y += br.y;

			Transform t = (mFG != null) ? mFG.cachedTransform : mBG.cachedTransform;
			for (int i = 0; i < 4; ++i) corners[i] = t.TransformPoint(corners[i]);

            if (UseFillDir)
            {
                SetThumbRotation();
            }
            else
            {
                if (isHorizontal)
                {
                    Vector3 v0 = Vector3.Lerp(corners[0], corners[1], 0.5f);
                    Vector3 v1 = Vector3.Lerp(corners[2], corners[3], 0.5f);
                    SetThumbPosition(Vector3.Lerp(v0, v1, isInverted ? 1f - value : value));
                }
                else
                {
                    Vector3 v0 = Vector3.Lerp(corners[0], corners[3], 0.5f);
                    Vector3 v1 = Vector3.Lerp(corners[1], corners[2], 0.5f);
                    SetThumbPosition(Vector3.Lerp(v0, v1, isInverted ? 1f - value : value));
                }
            }
		}
	}

    public void SetDynamicGround( float length, int depth)
    {
        float before = mDynamicVal;
        mDynamicVal = length;

        if(mDG != null)
            mDG.depth = depth;

        if(before != mDynamicVal)
            mIsDirty = true;
    }

	/// <summary>
	/// Set the position of the thumb to the specified world coordinates.
	/// </summary>

	protected void SetThumbPosition (Vector3 worldPos)
	{
		Transform t = thumb.parent;

		if (t != null)
		{
			worldPos = t.InverseTransformPoint(worldPos);
			worldPos.x = Mathf.Round(worldPos.x);
			worldPos.y = Mathf.Round(worldPos.y);
			worldPos.z = 0f;

			if (Vector3.Distance(thumb.localPosition, worldPos) > 0.001f)
				thumb.localPosition = worldPos;
		}
		else if (Vector3.Distance(thumb.position, worldPos) > 0.00001f)
			thumb.position = worldPos;
	}

    protected void SetThumbRotation()
    {
        if (mFG == null)
            return;
        UISprite sprite = mFG as UISprite;
        float directionValue = sprite.invert ? -1f : 1f;
        thumb.localEulerAngles = new Vector3(0f, 0f, sprite.FillScale * 360f * value * directionValue);
    }
}
