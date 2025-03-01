//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

#if UNITY_FLASH || UNITY_WP8 || UNITY_METRO
#define USE_SIMPLE_DICTIONARY
#endif

using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// UI Panel is responsible for collecting, sorting and updating widgets in addition to generating widgets' geometry.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Panel")]
public class UIPanel : UIRect, UILib.IUIPanel
{
	/// <summary>
	/// List of active panels.
	/// </summary>

	static public BetterList<UIPanel> list = new BetterList<UIPanel>();

	public enum RenderQueue
	{
		Automatic,
		StartAt,
		Explicit,
	}

	public delegate void OnGeometryUpdated ();

	/// <summary>
	/// Notification triggered when the panel's geometry get rebuilt. It's mainly here for debugging purposes.
	/// </summary>

	public OnGeometryUpdated onGeometryUpdated;

    public bool showVertexCount = false;
    public int vertexCount = 0;
	/// <summary>
	/// Whether this panel will show up in the panel tool (set this to 'false' for dynamically created temporary panels)
	/// </summary>

	public bool showInPanelTool = true;

	/// <summary>
	/// Whether normals and tangents will be generated for all meshes
	/// </summary>
	
	public bool generateNormals = false;

	/// <summary>
	/// Whether widgets drawn by this panel are static (won't move). This will improve performance.
	/// </summary>

	public bool widgetsAreStatic = false;

	/// <summary>
	/// Whether widgets will be culled while the panel is being dragged.
	/// Having this on improves performance, but turning it off will reduce garbage collection.
	/// </summary>

	public bool cullWhileDragging = false;



	/// <summary>
	/// Optimization flag. Makes the assumption that the panel's geometry
	/// will always be on screen and the bounds don't need to be re-calculated.
	/// </summary>

	public bool alwaysOnScreen = false;

	/// <summary>
	/// By default, non-clipped panels use the camera's bounds, and the panel's position has no effect.
	/// If you want the panel's position to actually be used with anchors, set this field to 'true'.
	/// </summary>

	public bool anchorOffset = false;

	/// <summary>
	/// By default all panels manage render queues of their draw calls themselves by incrementing them
	/// so that the geometry is drawn in the proper order. You can alter this behaviour.
	/// </summary>

	public RenderQueue renderQueue = RenderQueue.Automatic;

	/// <summary>
	/// Render queue used by the panel. The default value of '3000' is the equivalent of "Transparent".
	/// This property is only used if 'renderQueue' is set to something other than "Automatic".
	/// </summary>

	public int startingRenderQueue = 3500;

	/// <summary>
	/// List of widgets managed by this panel. Do not attempt to modify this list yourself.
	/// </summary>

	[System.NonSerialized]
	public BetterList<UIWidget> widgets = new BetterList<UIWidget>();

	/// <summary>
	/// List of draw calls created by this panel. Do not attempt to modify this list yourself.
	/// </summary>

	[System.NonSerialized]
	public BetterList<UIDrawCall> drawCalls = new BetterList<UIDrawCall>();

	/// <summary>
	/// Matrix that will transform the specified world coordinates to relative-to-panel coordinates.
	/// </summary>

	[System.NonSerialized]
	public Matrix4x4 worldToLocal = Matrix4x4.identity;

	/// <summary>
	/// Cached clip range passed to the draw call's shader.
	/// </summary>

	[System.NonSerialized]
	public Vector4 drawCallClipRange = new Vector4(0f, 0f, 1f, 1f);

	public delegate void OnClippingMoved (UIPanel panel);

	/// <summary>
	/// Event callback that's triggered when the panel's clip region gets moved.
	/// </summary>

	public OnClippingMoved onClipMove;

	// Panel's alpha (affects the alpha of all widgets)
	[HideInInspector][SerializeField] float mAlpha = 1f;
	public bool useMerge = false;
	static public bool GlobalUseMerge = false;
	static public bool SelectUseMerge = false;
	// Clipping rectangle
	[HideInInspector][SerializeField] UIDrawCall.Clipping mClipping = UIDrawCall.Clipping.None;
	[HideInInspector][SerializeField] Vector4 mClipRange = new Vector4(0f, 0f, 300f, 200f);
	[HideInInspector][SerializeField] Vector2 mClipSoftness = new Vector2(4f, 4f);
	[HideInInspector][SerializeField] int mDepth = 0;
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
	[HideInInspector][SerializeField] int mSortingOrder = 0;
#endif
	// Whether a full rebuild of geometry buffers is required
	bool mRebuild = false;
	bool mResized = false;

	Camera mCam;
	[SerializeField] Vector2 mClipOffset = Vector2.zero;

	float mCullTime = 0f;
	float mUpdateTime = 0f;
	int mMatrixFrame = -1;
	int mAlphaFrameID = 0;
	int mLayer = -1;

	// Values used for visibility checks
	static float[] mTemp = new float[4];
	Vector2 mMin = Vector2.zero;
	Vector2 mMax = Vector2.zero;
	bool mHalfPixelOffset = false;
	bool mSortWidgets = false;

    [HideInInspector]
    [SerializeField]
    public bool NeedUpdate = true;

    [System.NonSerialized]
    public bool mNeedUpdateRuntime = true;

    UIScrollView sv;
	/// <summary>
	/// Helper property that returns the first unused depth value.
	/// </summary>

	static public int nextUnusedDepth
	{
		get
		{
			int highest = int.MinValue;
			for (int i = 0; i < list.size; ++i)
				highest = Mathf.Max(highest, list[i].depth);
			return (highest == int.MinValue) ? 0 : highest + 1;
		}
	}

	/// <summary>
	/// Whether the rectangle can be anchored.
	/// </summary>

	public override bool canBeAnchored { get { return mClipping != UIDrawCall.Clipping.None; } }

	/// <summary>
	/// Panel's alpha affects everything drawn by the panel.
	/// </summary>

	public override float alpha
	{
		get
		{
			return mAlpha;
		}
		set
		{
			float val = Mathf.Clamp01(value);

			if (mAlpha != val)
			{
				mAlphaFrameID = -1;
				mResized = true;
				mAlpha = val;
				SetDirty();
			}
		}
	}

	/// <summary>
	/// Panels can have their own depth value that will change the order with which everything they manage gets drawn.
	/// </summary>

	public int depth
	{
		get
		{
			return mDepth;
		}
		set
		{
			if (mDepth != value)
			{
				mDepth = value;
#if UNITY_EDITOR
				NGUITools.SetDirty(this);
#endif
				list.Sort(CompareFunc);
			}
		}
	}

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
	/// <summary>
	/// Sorting order value for the panel's draw calls, to be used with Unity's 2D system.
	/// </summary>

	public int sortingOrder
	{
		get
		{
			return mSortingOrder;
		}
		set
		{
			if (mSortingOrder != value)
			{
				mSortingOrder = value;
#if UNITY_EDITOR
				NGUITools.SetDirty(this);
#endif
				UpdateDrawCalls();
			}
		}
	}
#endif

	/// <summary>
	/// Function that can be used to depth-sort panels.
	/// </summary>

	static public int CompareFunc (UIPanel a, UIPanel b)
	{
		if (a != b && a != null && b != null)
		{
			if (a.mDepth < b.mDepth) return -1;
			if (a.mDepth > b.mDepth) return 1;
			return (a.GetInstanceID() < b.GetInstanceID()) ? -1 : 1;
		}
		return 0;
	}

	/// <summary>
	/// Panel's width in pixels.
	/// </summary>

	public float width { get { return GetViewSize().x; } }

	/// <summary>
	/// Panel's height in pixels.
	/// </summary>

	public float height { get { return GetViewSize().y; } }

	/// <summary>
	/// Whether the panel's drawn geometry needs to be offset by a half-pixel.
	/// </summary>

	public bool halfPixelOffset { get { return mHalfPixelOffset; } }

	/// <summary>
	/// Whether the camera is used to draw UI geometry.
	/// </summary>

	public bool usedForUI { get { return (mCam != null && mCam.orthographic); } }

	/// <summary>
	/// Directx9 pixel offset, used for drawing.
	/// </summary>

	public Vector3 drawCallOffset
	{
		get
		{
			if (mHalfPixelOffset && mCam != null && mCam.orthographic)
			{
				Vector2 size = GetWindowSize();
				float mod = (1f / size.y) / mCam.orthographicSize;
				return new Vector3(-mod, mod);
			}
			return Vector3.zero;
		}
	}

	/// <summary>
	/// Clipping method used by all draw calls.
	/// </summary>

	public UIDrawCall.Clipping clipping
	{
		get
		{
			return mClipping;
		}
		set
		{
			if (mClipping != value)
			{
				mResized = true;
				mClipping = value;
				mMatrixFrame = -1;
#if UNITY_EDITOR
				if (!Application.isPlaying) UpdateDrawCalls();
#endif
			}
		}
	}

	UIPanel mParentPanel = null;

	/// <summary>
	/// Reference to the parent panel, if any.
	/// </summary>

	public UIPanel parentPanel { get { return mParentPanel; } }

	/// <summary>
	/// Number of times the panel's contents get clipped.
	/// </summary>

	public int clipCount
	{
		get
		{
			int count = 0;
			UIPanel p = this;

			while (p != null)
			{
				if (p.mClipping == UIDrawCall.Clipping.SoftClip) ++count;
				p = p.mParentPanel;
			}
			return count;
		}
	}


	/// <summary>
	/// Whether the panel will actually perform clipping of children.
	/// </summary>

	public bool hasClipping { get { return mClipping == UIDrawCall.Clipping.SoftClip; } }

	/// <summary>
	/// Whether the panel will actually perform clipping of children.
	/// </summary>

	public bool hasCumulativeClipping { get { return clipCount != 0; } }

	[System.Obsolete("Use 'hasClipping' or 'hasCumulativeClipping' instead")]
	public bool clipsChildren { get { return hasCumulativeClipping; } }

	/// <summary>
	/// Clipping area offset used to make it possible to move clipped panels (scroll views) efficiently.
	/// Scroll views move by adjusting the clip offset by one value, and the transform position by the inverse.
	/// This makes it possible to not have to rebuild the geometry, greatly improving performance.
	/// </summary>

	public Vector2 clipOffset
	{
		get
		{
			return mClipOffset;
		}
		set
		{
			if (Mathf.Abs(mClipOffset.x - value.x) > 0.001f ||
				Mathf.Abs(mClipOffset.y - value.y) > 0.001f)
			{
				mResized = true;
				mCullTime = (mCullTime == 0f) ? 0.001f : RealTime.time + 0.15f;
				mClipOffset = value;
				mMatrixFrame = -1;

				// Call the event delegate
				if (onClipMove != null) onClipMove(this);
#if UNITY_EDITOR
				if (!Application.isPlaying) UpdateDrawCalls();
#endif
			}
		}
	}

	/// <summary>
	/// Clipping position (XY) and size (ZW).
	/// Note that you should not be modifying this property at run-time to reposition the clipping. Adjust clipOffset instead.
	/// </summary>

	//[System.Obsolete("Use 'finalClipRegion' or 'baseClipRegion' instead")]
	//public Vector4 clipRange
	//{
	//	get
	//	{
	//		return baseClipRegion;
	//	}
	//	set
	//	{
	//		baseClipRegion = value;
	//	}
	//}

	/// <summary>
	/// Clipping position (XY) and size (ZW).
	/// Note that you should not be modifying this property at run-time to reposition the clipping. Adjust clipOffset instead.
	/// </summary>

	public Vector4 baseClipRegion
	{
		get
		{
			return mClipRange;
		}
		set
		{
			if (Mathf.Abs(mClipRange.x - value.x) > 0.001f ||
				Mathf.Abs(mClipRange.y - value.y) > 0.001f ||
				Mathf.Abs(mClipRange.z - value.z) > 0.001f ||
				Mathf.Abs(mClipRange.w - value.w) > 0.001f)
			{
				mResized = true;
				mCullTime = (mCullTime == 0f) ? 0.001f : RealTime.time + 0.15f;
				mClipRange = value;
				mMatrixFrame = -1;
                				
				if (sv != null) sv.UpdatePosition();
				if (onClipMove != null) onClipMove(this);
#if UNITY_EDITOR
				if (!Application.isPlaying) UpdateDrawCalls();
#endif
			}
		}
	}
    //public Vector4 WorldClipRegion
    //{
    //    get
    //    {

    //    }
    //}
    /// <summary>
    /// Final clipping region after the offset has been taken into consideration. XY = center, ZW = size.
    /// </summary>

    public Vector4 finalClipRegion
	{
		get
		{
			Vector2 size = GetViewSize();

			if (mClipping != UIDrawCall.Clipping.None)
			{
				return new Vector4(mClipRange.x + mClipOffset.x, mClipRange.y + mClipOffset.y, size.x, size.y);
			}
			return new Vector4(0f, 0f, size.x, size.y);
		}
	}

	/// <summary>
	/// Clipping softness is used if the clipped style is set to "Soft".
	/// </summary>

	public Vector2 clipSoftness
	{
		get
		{
			return mClipSoftness;
		}
		set
		{
			if (mClipSoftness != value)
			{
				mClipSoftness = value;
#if UNITY_EDITOR
				if (!Application.isPlaying) UpdateDrawCalls();
#endif
			}
		}
	}

	// Temporary variable to avoid GC allocation
	static Vector3[] mCorners = new Vector3[4];

	/// <summary>
	/// Local-space corners of the panel's clipping rectangle. The order is bottom-left, top-left, top-right, bottom-right.
	/// </summary>

	public override Vector3[] localCorners
	{
		get
		{
			if (mClipping == UIDrawCall.Clipping.None)
			{
				Vector2 size = GetViewSize();

				float x0 = -0.5f * size.x;
				float y0 = -0.5f * size.y;
				float x1 = x0 + size.x;
				float y1 = y0 + size.y;

				Transform wt = (mCam != null) ? mCam.transform : null;

				if (wt != null)
				{
					mCorners[0] = wt.TransformPoint(x0, y0, 0f);
					mCorners[1] = wt.TransformPoint(x0, y1, 0f);
					mCorners[2] = wt.TransformPoint(x1, y1, 0f);
					mCorners[3] = wt.TransformPoint(x1, y0, 0f);

					wt = cachedTransform;

					for (int i = 0; i < 4; ++i)
						mCorners[i] = wt.InverseTransformPoint(mCorners[i]);
				}
				else
				{
					mCorners[0] = new Vector3(x0, y0);
					mCorners[1] = new Vector3(x0, y1);
					mCorners[2] = new Vector3(x1, y1);
					mCorners[3] = new Vector3(x1, y0);
				}
			}
			else
			{
				float x0 = mClipOffset.x + mClipRange.x - 0.5f * mClipRange.z;
				float y0 = mClipOffset.y + mClipRange.y - 0.5f * mClipRange.w;
				float x1 = x0 + mClipRange.z;
				float y1 = y0 + mClipRange.w;

				mCorners[0] = new Vector3(x0, y0);
				mCorners[1] = new Vector3(x0, y1);
				mCorners[2] = new Vector3(x1, y1);
				mCorners[3] = new Vector3(x1, y0);
			}
			return mCorners;
		}
	}

	/// <summary>
	/// World-space corners of the panel's clipping rectangle. The order is bottom-left, top-left, top-right, bottom-right.
	/// </summary>

	public override Vector3[] worldCorners
	{
		get
		{
			if (mClipping == UIDrawCall.Clipping.None)
			{
				Vector2 size = GetViewSize();

				float x0 = -0.5f * size.x;
				float y0 = -0.5f * size.y;
				float x1 = x0 + size.x;
				float y1 = y0 + size.y;

				Transform wt = (mCam != null) ? mCam.transform : null;

				if (wt != null)
				{
					mCorners[0] = wt.TransformPoint(x0, y0, 0f);
					mCorners[1] = wt.TransformPoint(x0, y1, 0f);
					mCorners[2] = wt.TransformPoint(x1, y1, 0f);
					mCorners[3] = wt.TransformPoint(x1, y0, 0f);
				}
			}
			else
			{
				float x0 = mClipOffset.x + mClipRange.x - 0.5f * mClipRange.z;
				float y0 = mClipOffset.y + mClipRange.y - 0.5f * mClipRange.w;
				float x1 = x0 + mClipRange.z;
				float y1 = y0 + mClipRange.w;

				Transform wt = cachedTransform;

				mCorners[0] = wt.TransformPoint(x0, y0, 0f);
				mCorners[1] = wt.TransformPoint(x0, y1, 0f);
				mCorners[2] = wt.TransformPoint(x1, y1, 0f);
				mCorners[3] = wt.TransformPoint(x1, y0, 0f);
			}
			return mCorners;
		}
	}

	/// <summary>
	/// Get the sides of the rectangle relative to the specified transform.
	/// The order is left, top, right, bottom.
	/// </summary>

	public override Vector3[] GetSides (Transform relativeTo)
	{
		if (mClipping != UIDrawCall.Clipping.None || anchorOffset)
		{
			Vector2 size = GetViewSize();
			Vector2 cr = (mClipping != UIDrawCall.Clipping.None) ? (Vector2)mClipRange + mClipOffset : Vector2.zero;

			float x0 = cr.x - 0.5f * size.x;
			float y0 = cr.y - 0.5f * size.y;
			float x1 = x0 + size.x;
			float y1 = y0 + size.y;
			float cx = (x0 + x1) * 0.5f;
			float cy = (y0 + y1) * 0.5f;

			Matrix4x4 mat = cachedTransform.localToWorldMatrix;

			mCorners[0] = mat.MultiplyPoint3x4(new Vector3(x0, cy));
			mCorners[1] = mat.MultiplyPoint3x4(new Vector3(cx, y1));
			mCorners[2] = mat.MultiplyPoint3x4(new Vector3(x1, cy));
			mCorners[3] = mat.MultiplyPoint3x4(new Vector3(cx, y0));

			if (relativeTo != null)
			{
				for (int i = 0; i < 4; ++i)
					mCorners[i] = relativeTo.InverseTransformPoint(mCorners[i]);
			}
			return mCorners;
		}
		return base.GetSides(relativeTo);
	}

	/// <summary>
	/// Invalidating the panel should reset its alpha.
	/// </summary>

	public override void Invalidate (bool includeChildren)
	{
		mAlphaFrameID = -1;
		base.Invalidate(includeChildren);
	}

	/// <summary>
	/// Widget's final alpha, after taking the panel's alpha into account.
	/// </summary>

	public override float CalculateFinalAlpha (int frameID)
	{
		if (mAlphaFrameID != frameID)
		{
			mAlphaFrameID = frameID;
			UIRect pt = parent;
			finalAlpha = (parent != null) ? pt.CalculateFinalAlpha(frameID) * mAlpha : mAlpha;
		}
		return finalAlpha;
	}

	/// <summary>
	/// Set the panel's rectangle.
	/// </summary>

	public override void SetRect (float x, float y, float width, float height)
	{
		int finalWidth = Mathf.FloorToInt(width + 0.5f);
		int finalHeight = Mathf.FloorToInt(height + 0.5f);

		finalWidth = ((finalWidth >> 1) << 1);
		finalHeight = ((finalHeight >> 1) << 1);

		Transform t = cachedTransform;
		Vector3 pos = t.localPosition;
		pos.x = Mathf.Floor(x + 0.5f);
		pos.y = Mathf.Floor(y + 0.5f);

		if (finalWidth < 2) finalWidth = 2;
		if (finalHeight < 2) finalHeight = 2;

		baseClipRegion = new Vector4(pos.x, pos.y, finalWidth, finalHeight);

		if (isAnchored)
		{
			t = t.parent;

			if (leftAnchor.target) leftAnchor.SetHorizontal(t, x);
			if (rightAnchor.target) rightAnchor.SetHorizontal(t, x + width);
			if (bottomAnchor.target) bottomAnchor.SetVertical(t, y);
			if (topAnchor.target) topAnchor.SetVertical(t, y + height);
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}
	}

	/// <summary>
	/// Returns whether the specified rectangle is visible by the panel. The coordinates must be in world space.
	/// </summary>

	public bool IsVisible (Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		UpdateTransformMatrix();

		// Transform the specified points from world space to local space
		a = worldToLocal.MultiplyPoint3x4(a);
		b = worldToLocal.MultiplyPoint3x4(b);
		c = worldToLocal.MultiplyPoint3x4(c);
		d = worldToLocal.MultiplyPoint3x4(d);

		mTemp[0] = a.x;
		mTemp[1] = b.x;
		mTemp[2] = c.x;
		mTemp[3] = d.x;

		float minX = Mathf.Min(mTemp);
		float maxX = Mathf.Max(mTemp);

		mTemp[0] = a.y;
		mTemp[1] = b.y;
		mTemp[2] = c.y;
		mTemp[3] = d.y;

		float minY = Mathf.Min(mTemp);
		float maxY = Mathf.Max(mTemp);

		if (maxX < mMin.x) return false;
		if (maxY < mMin.y) return false;
		if (minX > mMax.x) return false;
		if (minY > mMax.y) return false;

		return true;
	}

	/// <summary>
	/// Returns whether the specified world position is within the panel's bounds determined by the clipping rect.
	/// </summary>

	public bool IsVisible (Vector3 worldPos)
	{
		if (mAlpha < 0.001f) return false;
		if (mClipping == UIDrawCall.Clipping.None ||
			mClipping == UIDrawCall.Clipping.ConstrainButDontClip) return true;

		UpdateTransformMatrix();

		Vector3 pos = worldToLocal.MultiplyPoint3x4(worldPos);
		if (pos.x < mMin.x) return false;
		if (pos.y < mMin.y) return false;
		if (pos.x > mMax.x) return false;
		if (pos.y > mMax.y) return false;
		return true;
	}

	/// <summary>
	/// Returns whether the specified widget is visible by the panel.
	/// </summary>

	public bool IsVisible (UIWidget w)
	{
		if ((mClipping == UIDrawCall.Clipping.None || mClipping == UIDrawCall.Clipping.ConstrainButDontClip) && !w.hideIfOffScreen)
		{
			if (mParentPanel == null || clipCount == 0) return true;
		}

		UIPanel p = this;
		Vector3[] corners = w.worldCorners;

		while (p != null)
		{
			if (!IsVisible(corners[0], corners[1], corners[2], corners[3])) return false;
			p = p.mParentPanel;
		}
		return true;
	}

	/// <summary>
	/// Whether the specified widget is going to be affected by this panel in any way.
	/// </summary>

	public bool Affects (UIWidget w)
	{
		if (w == null) return false;

		UIPanel expected = w.panel;
		if (expected == null) return false;

		UIPanel p = this;

		while (p != null)
		{
			if (p == expected) return true;
			if (!p.hasCumulativeClipping) return false;
			p = p.mParentPanel;
		}
		return false;
	}

	/// <summary>
	/// Causes all draw calls to be re-created on the next update.
	/// </summary>

	[ContextMenu("Force Refresh")]
	public void RebuildAllDrawCalls () { mRebuild = true; }

	/// <summary>
	/// Invalidate the panel's draw calls, forcing them to be rebuilt on the next update.
	/// This call also affects all children.
	/// </summary>

	public void SetDirty ()
	{
		for (int i = 0; i < drawCalls.size; ++i)
			drawCalls.buffer[i].isDirty = true;
		Invalidate(true);
	}

	/// <summary>
	/// Cache components.
	/// </summary>

	void Awake ()
	{
		mGo = gameObject;
		mTrans = transform;

		mHalfPixelOffset = (Application.platform == RuntimePlatform.WindowsPlayer ||
			//Application.platform == RuntimePlatform.XBOX360 ||
			//Application.platform == RuntimePlatform.WindowsWebPlayer ||
			Application.platform == RuntimePlatform.WindowsEditor);

		// Only DirectX 9 needs the half-pixel offset
		if (mHalfPixelOffset) mHalfPixelOffset = (SystemInfo.graphicsShaderLevel < 40);

        sv = GetComponent<UIScrollView>();
	}

    public void SetEnable()
    {
        base.OnEnable();
    }

	/// <summary>
	/// Remember the parent panel, if any.
	/// </summary>
	protected override void OnEnable ()
	{
		base.OnEnable();
		Transform parent = cachedTransform.parent;
		mParentPanel = (parent != null) ? NGUITools.FindInParents<UIPanel>(parent.gameObject) : null;
        UpdateNeedUpdate();
	}

	/// <summary>
	/// Find the parent panel, if we have one.
	/// </summary>

	public override void ParentHasChanged ()
	{
		base.ParentHasChanged();
		Transform parent = cachedTransform.parent;
		mParentPanel = (parent != null) ? NGUITools.FindInParents<UIPanel>(parent.gameObject) : null;
        UpdateNeedUpdate();
	}

    public override void SetPanel(UIPanel p)
    {
        Transform parent = cachedTransform.parent;
        mParentPanel = (parent != null) ? NGUITools.FindInParents<UIPanel>(parent.gameObject) : null;
        UpdateNeedUpdate();
    }

	/// <summary>
	/// Layer is used to ensure that if it changes, widgets get moved as well.
	/// </summary>

	protected override void OnStart ()
	{
		mLayer = mGo.layer;
		UICamera uic = UICamera.FindCameraForLayer(mLayer);
		mCam = (uic != null) ? uic.cachedCamera : NGUITools.FindCameraForLayer(mLayer);
#if !UNITY_STANDALONE
        //delayUpdateOnStart = true;
#endif
	}

	/// <summary>
	/// Mark all widgets as having been changed so the draw calls get re-created.
	/// </summary>

	protected override void OnInit ()
	{
		base.OnInit();

		// Apparently having a rigidbody helps
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

		mRebuild = true;
		mAlphaFrameID = -1;
		mMatrixFrame = -1;

		list.Add(this);
		list.Sort(CompareFunc);
	}

	/// <summary>
	/// Destroy all draw calls we've created when this script gets disabled.
	/// </summary>

	protected override void OnDisable ()
	{
		for (int i = 0; i < drawCalls.size; ++i)
		{
			UIDrawCall dc = drawCalls.buffer[i];
			if (dc != null) UIDrawCall.Destroy(dc);
		}
		
		drawCalls.Clear();
		list.Remove(this);

		mAlphaFrameID = -1;
		mMatrixFrame = -1;
		
		if (list.size == 0)
		{
			UIDrawCall.ReleaseAll();
            mLaterUpdateFrame = -1;
            mUpdateFrame = -1;
		}
		base.OnDisable();
	}

	/// <summary>
	/// Update the world-to-local transform matrix as well as clipping bounds.
	/// </summary>

	void UpdateTransformMatrix (bool force = false)
	{
		int fc = Time.frameCount;

        if (mMatrixFrame != fc || force)
		{
			mMatrixFrame = fc;
			worldToLocal = cachedTransform.worldToLocalMatrix;

			Vector2 size = GetViewSize() * 0.5f;

			float x = mClipOffset.x + mClipRange.x;
			float y = mClipOffset.y + mClipRange.y;

			mMin.x = x - size.x;
			mMin.y = y - size.y;
			mMax.x = x + size.x;
			mMax.y = y + size.y;
		}
	}

	/// <summary>
	/// Update the edges after the anchors have been updated.
	/// </summary>

	protected override void OnAnchor ()
	{
		// No clipping = no edges to anchor
		if (mClipping == UIDrawCall.Clipping.None) return;

		Transform trans = cachedTransform;
		Transform parent = trans.parent;

		Vector2 size = GetViewSize();
		Vector2 offset = trans.localPosition;

		float lt, bt, rt, tt;

		// Attempt to fast-path if all anchors match
		if (leftAnchor.target == bottomAnchor.target &&
			leftAnchor.target == rightAnchor.target &&
			leftAnchor.target == topAnchor.target)
		{
			Vector3[] sides = leftAnchor.GetSides(parent);

			if (sides != null)
			{
				lt = NGUIMath.Lerp(sides[0].x, sides[2].x, leftAnchor.relative) + leftAnchor.absolute;
				rt = NGUIMath.Lerp(sides[0].x, sides[2].x, rightAnchor.relative) + rightAnchor.absolute;
				bt = NGUIMath.Lerp(sides[3].y, sides[1].y, bottomAnchor.relative) + bottomAnchor.absolute;
				tt = NGUIMath.Lerp(sides[3].y, sides[1].y, topAnchor.relative) + topAnchor.absolute;
			}
			else
			{
				// Anchored to a single transform
				Vector2 lp = GetLocalPos(leftAnchor, parent);
				lt = lp.x + leftAnchor.absolute;
				bt = lp.y + bottomAnchor.absolute;
				rt = lp.x + rightAnchor.absolute;
				tt = lp.y + topAnchor.absolute;
			}
		}
		else
		{
			// Left anchor point
			if (leftAnchor.target)
			{
				Vector3[] sides = leftAnchor.GetSides(parent);

				if (sides != null)
				{
					lt = NGUIMath.Lerp(sides[0].x, sides[2].x, leftAnchor.relative) + leftAnchor.absolute;
				}
				else
				{
					lt = GetLocalPos(leftAnchor, parent).x + leftAnchor.absolute;
				}
			}
			else lt = mClipRange.x - 0.5f * size.x;

			// Right anchor point
			if (rightAnchor.target)
			{
				Vector3[] sides = rightAnchor.GetSides(parent);

				if (sides != null)
				{
					rt = NGUIMath.Lerp(sides[0].x, sides[2].x, rightAnchor.relative) + rightAnchor.absolute;
				}
				else
				{
					rt = GetLocalPos(rightAnchor, parent).x + rightAnchor.absolute;
				}
			}
			else rt = mClipRange.x + 0.5f * size.x;

			// Bottom anchor point
			if (bottomAnchor.target)
			{
				Vector3[] sides = bottomAnchor.GetSides(parent);

				if (sides != null)
				{
					bt = NGUIMath.Lerp(sides[3].y, sides[1].y, bottomAnchor.relative) + bottomAnchor.absolute;
				}
				else
				{
					bt = GetLocalPos(bottomAnchor, parent).y + bottomAnchor.absolute;
				}
			}
			else bt = mClipRange.y - 0.5f * size.y;

			// Top anchor point
			if (topAnchor.target)
			{
				Vector3[] sides = topAnchor.GetSides(parent);

				if (sides != null)
				{
					tt = NGUIMath.Lerp(sides[3].y, sides[1].y, topAnchor.relative) + topAnchor.absolute;
				}
				else
				{
					tt = GetLocalPos(topAnchor, parent).y + topAnchor.absolute;
				}
			}
			else tt = mClipRange.y + 0.5f * size.y;
		}

		// Take the offset into consideration
		lt -= offset.x + mClipOffset.x;
		rt -= offset.x + mClipOffset.x;
		bt -= offset.y + mClipOffset.y;
		tt -= offset.y + mClipOffset.y;

		// Calculate the new position, width and height
		float newX = Mathf.Lerp(lt, rt, 0.5f);
		float newY = Mathf.Lerp(bt, tt, 0.5f);
		float w = rt - lt;
		float h = tt - bt;

		float minx = Mathf.Max(20f, mClipSoftness.x);
		float miny = Mathf.Max(20f, mClipSoftness.y);

		if (w < minx) w = minx;
		if (h < miny) h = miny;

		// Update the clipping range
		baseClipRegion = new Vector4(newX, newY, w, h);
	}
    static int mUpdateFrame = -1;
    void Update()
    {
        if (mUpdateFrame != Time.frameCount)
        {
            mUpdateFrame = Time.frameCount;
            for (int i = 0; i < list.size; ++i)
            {
                UIPanel p = list.buffer[i];
                if (p.isUpdate && p.mNeedUpdateRuntime)
                {
                    for (int j = 0, jmax = p.widgets.size; j < jmax; ++j)
                    {
                        UIWidget w = p.widgets[j];
                        if (w != null)
                            w.ManualUpdate();
                    }                    
                }
            }
        }
    }
    Vector4 clip_range = new Vector4(0.0f, 0.0f, 10000.0f, 10000.0f);

    public Vector4 ClipRange
    {
        get
        {
            return clip_range;
        }
    }
    void UpdateClipRange()
    {
        float clip_x_min = -1000000.0f;
        float clip_x_max = 1000000.0f;
        float clip_y_min = -1000000.0f;
        float clip_y_max = 1000000.0f;
        UIPanel currentPanel = this;
        
        Vector3 v0 = cachedTransform.rotation.eulerAngles;
        while (currentPanel != null)
        {
            if (currentPanel.hasClipping)
            {
                float angle = 0f;
                Vector4 cr = currentPanel.drawCallClipRange;

                // Clipping regions past the first one need additional math
                if (currentPanel != this)
                {
                    Vector3 pos = currentPanel.cachedTransform.InverseTransformPoint(cachedTransform.position);
                    cr.x -= pos.x;
                    cr.y -= pos.y;
                    
                    Vector3 v1 = currentPanel.cachedTransform.rotation.eulerAngles;
                    Vector3 diff = v1 - v0;

                    diff.x = NGUIMath.WrapAngle(diff.x);
                    diff.y = NGUIMath.WrapAngle(diff.y);
                    diff.z = NGUIMath.WrapAngle(diff.z);
#if UNITY_EDITOR
                    if (Mathf.Abs(diff.x) > 0.001f || Mathf.Abs(diff.y) > 0.001f)
                        Debug.LogWarning("Panel can only be clipped properly if X and Y rotation is left at 0", this);
#endif
                    angle = diff.z;
                }

                float min_x = cr.x - cr.z;
                float max_x = cr.x + cr.z;
                float min_y = cr.y - cr.w;
                float max_y = cr.y + cr.w;

                if (min_x > clip_x_min) clip_x_min = min_x;
                if (max_x < clip_x_max) clip_x_max = max_x;

                if (min_y > clip_y_min) clip_y_min = min_y;
                if (max_y < clip_y_max) clip_y_max = max_y;                
            }
            currentPanel = currentPanel.parentPanel;
        }
        float size_x = (clip_x_max - clip_x_min) * 0.5f;
        float size_y = (clip_y_max - clip_y_min) * 0.5f;

        clip_range.x = clip_x_min + size_x;
        clip_range.y = clip_y_min + size_y;
        clip_range.z = size_x;
        clip_range.w = size_y;
    }

    public void UpdateNeedUpdate()
    {
        if (!NeedUpdate)
        {
            mNeedUpdateRuntime = false;
            return;
        }

        UIPanel parentP = mParentPanel;
        while (parentP != null)
        {
            if (!parentP.NeedUpdate)
            {
                mNeedUpdateRuntime = false;
                return;
            }
            parentP = parentP.parentPanel;
            if (parentP == null || root == null || parentP.gameObject == root.gameObject)
            {
                break;
            }
        }
        mNeedUpdateRuntime = true;
    }
	static int mLaterUpdateFrame = -1;

	/// <summary>
	/// Update all panels and draw calls.
	/// </summary>
    /// 
    public int updatePreFrame = 1;
	public int updateFrameIndex = 0;
	public int updateFrame = 1;
    int currentFrame = 0;
	//static int skipFrame = 1;
    bool isUpdate = false;
    void LateUpdate ()
	{
        if (mLaterUpdateFrame != Time.frameCount)
        {
            mLaterUpdateFrame = Time.frameCount;
            YBillboard.IsUpdate = mLaterUpdateFrame % 4 == 0;
            // Update each panel in order
            for (int i = 0; i < list.size; ++i)
            {
                UIPanel p = list.buffer[i];
                p.isUpdate = false;
				if (p.updatePreFrame > 1)
				{
                    p.currentFrame++;
					p.currentFrame %= p.updatePreFrame;
					if (p.currentFrame == updateFrameIndex)
                        p.isUpdate = true;
                }
                else
                {
                    p.isUpdate = true;
                }
                if (p.isUpdate && p.mNeedUpdateRuntime)
                {
                    p.UpdateSelf();
                }
            }			

			int rq = 3500;
           
            // Update all draw calls, making them draw in the right order
            for (int i = 0; i < list.size; ++i)
			{
				UIPanel p = list.buffer[i];
                if (p.mNeedUpdateRuntime)
                {
//#if DEBUG
//                    currentPanelName = this.name;
//#endif
                    if (p.renderQueue == RenderQueue.Automatic)
                    {
                        p.startingRenderQueue = rq;
                        p.UpdateDrawCalls();
                        rq += p.drawCalls.size;
                    }
                    else if (p.renderQueue == RenderQueue.StartAt)
                    {
                        p.UpdateDrawCalls();
                        if (p.drawCalls.size != 0)
                            rq = Mathf.Max(rq, p.startingRenderQueue + p.drawCalls.size);
                    }
                    else // Explicit
                    {
                        p.UpdateDrawCalls();
                        if (p.drawCalls.size != 0)
                            rq = Mathf.Max(rq, p.startingRenderQueue + 1);
                    }
                }				
			}
		}
	}

    /// <summary>
    /// Update the panel, all of its widgets and draw calls.
    /// </summary>

    
	void UpdateSelf ()
	{
		mUpdateTime = RealTime.time;

		UpdateTransformMatrix(true);
		UpdateLayers();
		UpdateWidgets();
       
        if (mRebuild)
		{
			mRebuild = false;
			FillAllDrawCalls();
		}
		else
		{
			for (int i = 0; i < drawCalls.size; )
			{
				UIDrawCall dc = drawCalls.buffer[i];

				if (dc.isDirty && !FillDrawCall(dc))
				{
					UIDrawCall.Destroy(dc);
					drawCalls.RemoveAt(i);
					continue;
				}
				++i;
			}
		}
	}

	/// <summary>
	/// Immediately sort all child widgets.
	/// </summary>

	public void SortWidgets ()
	{
		mSortWidgets = false;
		widgets.Sort(UIWidget.PanelCompareFunc);
	}


	//static public Material mergeMaterial = null;
    /// <summary>
    /// Fill the geometry fully, processing all widgets and re-creating all draw calls.
    /// </summary>
    static List<Texture> mergetex = new List<Texture>();
    static List<Texture> mergetexFeatures = new List<Texture>();
    static List<Texture> mergealphaTex = new List<Texture>();
    void FillAllDrawCallsMerge()
	{
		//mergeMaterial = Resources.Load ("UIMerge") as Material;

		for (int i = 0; i < drawCalls.size; ++i)
			UIDrawCall.Destroy(drawCalls.buffer[i]);
		drawCalls.Clear();


        Shader sdr = null;
        UIDrawCall dc = null;
        vertexCount = 0;
        if (mSortWidgets) SortWidgets();

        for (int i = 0; i < widgets.size; ++i)
        {
            UIWidget w = widgets.buffer[i];

            if (w.isVisible && w.hasVertices)
            {
                Material mt = w.material;
                Texture mainTx = w.mainTexture;
                Texture featureTx = mainTx;
                Texture alphaTx = null;

                if (w.material != null)
                {
                    if (w.material.HasProperty("_Mask"))
                    {
                        alphaTx = w.material.GetTexture("_Mask");
                    }
                    else
                    {
                        alphaTx = mainTx;
                        mainTx = null;
                    }
                }
                else
                {
                    alphaTx = w.alphaTexture;
                }

                Shader sd = w.shader;

                //find the feature tx index
                int texIndex = -1;
                for (int ii = 0; ii < mergetexFeatures.Count; ++ii)
                {
                    if (mergetexFeatures[ii] == featureTx)
                    {
                        texIndex = ii;
                        break;
                    }
                }

                if (texIndex == -1)
                {
                    if (mergetex.Count == 4 && dc != null && dc.verts.size != 0)
                    {
                        if (dc.mAlphaTextureList == null)
                        {
                            dc.mAlphaTextureList = new List<Texture>();
                        }
                        dc.mAlphaTextureList.Clear();
                        dc.mAlphaTextureList.AddRange(mergealphaTex);
                        if (dc.mTextureList == null)
                        {
                            dc.mTextureList = new List<Texture>();
                        }
                        dc.mTextureList.Clear();
                        dc.mTextureList.AddRange(mergetex);
                        drawCalls.Add(dc);
                        dc.UpdateGeometry();
                        dc = null;
                        mergealphaTex.Clear();
                        mergetex.Clear();
                        mergetexFeatures.Clear();
                    }

                    texIndex = mergetex.Count;
                    mergetex.Add(mainTx);
                    mergetexFeatures.Add(featureTx);
                    mergealphaTex.Add(alphaTx);
                }

                sdr = sd;

                if (sdr != null)
                {
                    if (dc == null)
                    {
                        dc = UIDrawCall.CreateMerge(this, null, UIDrawCall.GetDefaultShader("Custom/UI/Merge"));//Create(this, mat, tex, alphaTex, sdr);
                        dc.depthStart = w.depth;
                        dc.depthEnd = dc.depthStart;
                        dc.panel = this;
                    }
                    else
                    {
                        int rd = w.depth;
                        if (rd < dc.depthStart) dc.depthStart = rd;
                        if (rd > dc.depthEnd) dc.depthEnd = rd;
                    }

                    w.drawCall = dc;

                    if (generateNormals) w.WriteToBuffers(dc.verts, dc.uvs, dc.cols, dc.norms, dc.tans);
                    else w.WriteToBuffers(dc.verts, dc.uvs, dc.cols, null, null);

                    w.textureIndex = texIndex;
                    dc.FillMergeIndex(texIndex, w.geometry.verts.Count);


                    vertexCount += w.geometry.verts.Count;
                    if (vertexCount > 65000)
                    {
                        XUtliPoolLib.XDebug.singleton.AddWarningLog2("Too many vertex in one panel:{0} {1}", this.name, vertexCount.ToString());
                    }
                }
            }
            else if (w.calcRenderQueue)
            {

                w.drawCall = dc;
            }
            else w.drawCall = null;
        }

        if (dc != null && dc.verts.size != 0)
        {
            if (dc.mAlphaTextureList == null)
            {
                dc.mAlphaTextureList = new List<Texture>();
            }
            dc.mAlphaTextureList.Clear();
            dc.mAlphaTextureList.AddRange(mergealphaTex);
            if (dc.mTextureList == null)
            {
                dc.mTextureList = new List<Texture>();
            }
            dc.mTextureList.Clear();
            dc.mTextureList.AddRange(mergetex);
            drawCalls.Add(dc);
            dc.UpdateGeometry();
        }
        mergetex.Clear();
        mergetexFeatures.Clear();
        mergealphaTex.Clear();
        //if(showVertexCount)
        //{
        //    Debug.LogWarning("TotalVertexCount:" + totalVertex.ToString());
        //}
    }

    void FillAllDrawCalls ()
	{
		if (GlobalUseMerge || (SelectUseMerge && useMerge) )
		{
			FillAllDrawCallsMerge ();
			return;
		}

		for (int i = 0; i < drawCalls.size; ++i)
			UIDrawCall.Destroy(drawCalls.buffer[i]);
		drawCalls.Clear();

		Material mat = null;
		Texture tex = null;
        IDrawCallMat dcMat = null;
		Shader sdr = null;
		UIDrawCall dc = null;
        vertexCount = 0;
		if (mSortWidgets) SortWidgets();

		for (int i = 0; i < widgets.size; ++i)
		{
			UIWidget w = widgets.buffer[i];

			if (w.isVisible && w.hasVertices)
			{
				Material mt = w.material;
				Texture tx = w.mainTexture;
                Texture alphaTx = w.alphaTexture;
                IDrawCallMat dcmat = w as IDrawCallMat;
                Shader sd = w.shader;

				if (mat != mt || tex != tx  || sdr != sd)
				{
					if (dc != null && dc.verts.size != 0)
					{
						drawCalls.Add(dc);
						dc.UpdateGeometry();
						dc = null;
					}

					mat = mt;
					tex = tx;
                    dcMat = dcmat;
                    sdr = sd;
				}

				if (mat != null || sdr != null || tex != null)
                {
                    if (dc == null)
                    {
                        dc = UIDrawCall.Create(this, mat, tex, dcMat, sdr);
                        dc.depthStart = w.depth;
						dc.depthEnd = dc.depthStart;
						dc.panel = this;
					}
					else
					{
						int rd = w.depth;
						if (rd < dc.depthStart) dc.depthStart = rd;
						if (rd > dc.depthEnd) dc.depthEnd = rd;
					}

					w.drawCall = dc;

					if (generateNormals) w.WriteToBuffers(dc.verts, dc.uvs, dc.cols, dc.norms, dc.tans);
					else w.WriteToBuffers(dc.verts, dc.uvs, dc.cols, null, null);
                    vertexCount += w.geometry.verts.Count;
                    if (vertexCount > 65000)
                    {
                        XUtliPoolLib.XDebug.singleton.AddWarningLog2("Too many vertex in one panel:{0} {1}", this.name, vertexCount.ToString());
                    }
                }
            }
            else if(w.calcRenderQueue)
            {
                w.drawCall = dc;
            }
			else w.drawCall = null;
		}

		if (dc != null && dc.verts.size != 0)
		{
			drawCalls.Add(dc);
			dc.UpdateGeometry();
		}
        //if(showVertexCount)
        //{
        //    Debug.LogWarning("TotalVertexCount:" + totalVertex.ToString());
        //}

	}

	/// <summary>
	/// Fill the geometry for the specified draw call.
	/// </summary>

	bool FillDrawCallMerge (UIDrawCall dc)
	{
		if (dc != null)
		{
			dc.isDirty = false;
			
			for (int i = 0; i < widgets.size; )
			{
				UIWidget w = widgets[i];
				
				if (w == null)
				{
					#if UNITY_EDITOR
					Debug.LogError("This should never happen");
					#endif
					widgets.RemoveAt(i);
					continue;
				}
				
				if (w.drawCall == dc)
				{
					if (w.isVisible && w.hasVertices)
					{
						if (generateNormals) w.WriteToBuffers(dc.verts, dc.uvs, dc.cols, dc.norms, dc.tans);
						else w.WriteToBuffers(dc.verts, dc.uvs, dc.cols, null, null);
						//w.ClearGeometry();
						dc.FillMergeIndex(w.textureIndex,w.geometry.verts.Count);
					}
					else if (!w.calcRenderQueue)
						w.drawCall = null;
				}
				++i;
			}
			
			if (dc.verts.size != 0)
			{
				dc.UpdateGeometry();
				return true;
			}
		}
		return false;
	}
	
	bool FillDrawCall (UIDrawCall dc)
	{
		if (GlobalUseMerge || (SelectUseMerge && useMerge) )
			return FillDrawCallMerge(dc);
		if (dc != null)
		{
			dc.isDirty = false;

			for (int i = 0; i < widgets.size; )
			{
				UIWidget w = widgets[i];

				if (w == null)
				{
#if UNITY_EDITOR
					Debug.LogError("This should never happen");
#endif
					widgets.RemoveAt(i);
					continue;
				}

				if (w.drawCall == dc)
				{
					if (w.isVisible && w.hasVertices)
					{
                        if (generateNormals) w.WriteToBuffers(dc.verts, dc.uvs, dc.cols, dc.norms, dc.tans);
                        else w.WriteToBuffers(dc.verts, dc.uvs, dc.cols, null, null);
                        //w.ClearGeometry();
                    }
                    else if (!w.calcRenderQueue)
                        w.drawCall = null;
				}
				++i;
			}

			if (dc.verts.size != 0)
			{
				dc.UpdateGeometry();
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Update all draw calls associated with the panel.
	/// </summary>

	void UpdateDrawCalls ()
	{
		Transform trans = cachedTransform;
		bool isUI = usedForUI;

		if (clipping != UIDrawCall.Clipping.None)
		{
			drawCallClipRange = finalClipRegion;
			drawCallClipRange.z *= 0.5f;
			drawCallClipRange.w *= 0.5f;
		}
		else drawCallClipRange = Vector4.zero;

		// Legacy functionality
		if (drawCallClipRange.z == 0f) drawCallClipRange.z = Screen.width * 0.5f;
		if (drawCallClipRange.w == 0f) drawCallClipRange.w = Screen.height * 0.5f;

		// DirectX 9 half-pixel offset
		if (halfPixelOffset)
		{
			drawCallClipRange.x -= 0.5f;
			drawCallClipRange.y += 0.5f;
		}

		Vector3 pos;

		// We want the position to always be on even pixels so that the
		// panel's contents always appear pixel-perfect.
		if (isUI)
		{
			Transform parent = cachedTransform.parent;
			pos = cachedTransform.localPosition;

			if (parent != null)
			{
				float x = Mathf.Round(pos.x);
				float y = Mathf.Round(pos.y);

				drawCallClipRange.x += pos.x - x;
				drawCallClipRange.y += pos.y - y;

				pos.x = x;
				pos.y = y;
				pos = parent.TransformPoint(pos);
			}
			pos += drawCallOffset;
		}
		else pos = trans.position;

		Quaternion rot = trans.rotation;
		Vector3 scale = trans.lossyScale;
        UpdateClipRange();
        for (int i = 0; i < drawCalls.size; ++i)
		{
			UIDrawCall dc = drawCalls.buffer[i];

			Transform t = dc.cachedTransform;
			t.position = pos;
			t.rotation = rot;
			t.localScale = scale;

			dc.renderQueue = (renderQueue == RenderQueue.Explicit) ? startingRenderQueue : startingRenderQueue + i;
			dc.alwaysOnScreen = alwaysOnScreen &&
				(mClipping == UIDrawCall.Clipping.None || mClipping == UIDrawCall.Clipping.ConstrainButDontClip);
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
			dc.sortingOrder = mSortingOrder;
#endif
		}
	}

	/// <summary>
	/// Update the widget layers if the panel's layer has changed.
	/// </summary>

	void UpdateLayers ()
	{
		// Always move widgets to the panel's layer
		if (mLayer != cachedGameObject.layer)
		{
			mLayer = mGo.layer;
			UICamera uic = UICamera.FindCameraForLayer(mLayer);
			mCam = (uic != null) ? uic.cachedCamera : NGUITools.FindCameraForLayer(mLayer);
			NGUITools.SetChildLayer(cachedTransform, mLayer);

			for (int i = 0; i < drawCalls.size; ++i)
				drawCalls.buffer[i].gameObject.layer = mLayer;
		}
	}

	bool mForced = false;

	/// <summary>
	/// Update all of the widgets belonging to this panel.
	/// </summary>

	void UpdateWidgets()
	{
#if UNITY_EDITOR
		bool forceVisible = cullWhileDragging ? false : (Application.isPlaying && mCullTime > mUpdateTime);
#else
		bool forceVisible = cullWhileDragging ? false : (mCullTime > mUpdateTime);
#endif
		bool changed = false;

		if (mForced != forceVisible)
		{
			mForced = forceVisible;
			mResized = true;
		}

		bool clipped = hasCumulativeClipping;

		// Update all widgets
		for (int i = 0, imax = widgets.size; i < imax; ++i)
		{
			UIWidget w = widgets.buffer[i];

			// If the widget is visible, update it
			if (w.panel == this && w.enabled)
			{
#if UNITY_EDITOR
				// When an object is dragged from Project view to Scene view, its Z is...
				// odd, to say the least. Force it if possible.
				if (!Application.isPlaying)
				{
					Transform t = w.cachedTransform;

					if (t.hideFlags != HideFlags.HideInHierarchy)
					{
						t = (t.parent != null && t.parent.hideFlags == HideFlags.HideInHierarchy) ?
							t.parent : null;
					}

					if (t != null)
					{
						for (; ; )
						{
							if (t.parent == null) break;
							if (t.parent.hideFlags == HideFlags.HideInHierarchy) t = t.parent;
							else break;
						}

						if (t != null)
						{
							Vector3 pos = t.localPosition;
							pos.x = Mathf.Round(pos.x);
							pos.y = Mathf.Round(pos.y);
							pos.z = 0f;

							if (Vector3.SqrMagnitude(t.localPosition - pos) > 0.0001f)
								t.localPosition = pos;
						}
					}
				}
#endif
				int frame = Time.frameCount;

				// First update the widget's transform
				if (w.UpdateTransform(frame) || mResized)
				{
					// Only proceed to checking the widget's visibility if it actually moved
					bool vis = forceVisible || (w.CalculateCumulativeAlpha(frame) > 0.001f);
					w.UpdateVisibility(vis, forceVisible || ((clipped || w.hideIfOffScreen) ? IsVisible(w) : true));
				}
				
				// Update the widget's geometry if necessary
				if (w.UpdateGeometry(frame))
				{
					changed = true;

					if (!mRebuild)
					{
						if (w.drawCall != null)
						{
							w.drawCall.isDirty = true;
						}
						else
						{
							// Find an existing draw call, if possible
							FindDrawCall(w);
						}
					}
				}
			}
		}

		// Inform the changed event listeners
		if (changed && onGeometryUpdated != null) onGeometryUpdated();
		mResized = false;
	}

	/// <summary>
	/// Insert the specified widget into one of the existing draw calls if possible.
	/// If it's not possible, and a new draw call is required, 'null' is returned
	/// because draw call creation is a delayed operation.
	/// </summary>

	public UIDrawCall FindDrawCall (UIWidget w)
	{
		Material mat = w.material;
		Texture tex = w.mainTexture;
		int depth = w.depth;

		for (int i = 0; i < drawCalls.size; ++i)
		{
			UIDrawCall dc = drawCalls.buffer[i];
			int dcStart = (i == 0) ? int.MinValue : drawCalls.buffer[i - 1].depthEnd + 1;
			int dcEnd = (i + 1 == drawCalls.size) ? int.MaxValue : drawCalls.buffer[i + 1].depthStart - 1;

			if (dcStart <= depth && dcEnd >= depth)
			{
				if (dc.baseMaterial == mat && dc.mainTexture == tex)
				{
					if (w.isVisible)
					{
						w.drawCall = dc;
						if (w.hasVertices) dc.isDirty = true;
						return dc;
					}
				}
				else mRebuild = true;
				return null;
			}
		}
		mRebuild = true;
		return null;
	}

	/// <summary>
	/// Make the following widget be managed by the panel.
	/// </summary>

	public void AddWidget (UIWidget w)
	{
		if (widgets.size == 0)
		{
			widgets.Add(w);
		}
		else if (mSortWidgets)
		{
			widgets.Add(w);
			SortWidgets();
		}
		else if (UIWidget.PanelCompareFunc(w, widgets[0]) == -1)
		{
			widgets.Insert(0, w);
		}
		else
		{
			for (int i = widgets.size; i > 0; )
			{
				if (UIWidget.PanelCompareFunc(w, widgets[--i]) == -1) continue;
				widgets.Insert(i+1, w);
				break;
			}
		}
		FindDrawCall(w);
	}

	/// <summary>
	/// Remove the widget from its current draw call, invalidating everything as needed.
	/// </summary>

	public void RemoveWidget (UIWidget w)
	{
		if (widgets.Remove(w) && w.drawCall != null)
		{
			int depth = w.depth;
			if (depth == w.drawCall.depthStart || depth == w.drawCall.depthEnd)
				mRebuild = true;

			w.drawCall.isDirty = true;
            if (mRebuild)
                w.drawCall.Clear();
			w.drawCall = null;
		}
	}

	/// <summary>
	/// Immediately refresh the panel.
	/// </summary>

	public void Refresh ()
	{
		mRebuild = true;
		if (list.size > 0) list[0].LateUpdate();
	}

	/// <summary>
	/// Calculate the offset needed to be constrained within the panel's bounds.
	/// </summary>

	public virtual Vector3 CalculateConstrainOffset (Vector2 min, Vector2 max)
	{
		Vector4 cr = finalClipRegion;

		float offsetX = cr.z * 0.5f;
		float offsetY = cr.w * 0.5f;

		Vector2 minRect = new Vector2(min.x, min.y);
		Vector2 maxRect = new Vector2(max.x, max.y);
		Vector2 minArea = new Vector2(cr.x - offsetX, cr.y - offsetY);
		Vector2 maxArea = new Vector2(cr.x + offsetX, cr.y + offsetY);

		if (clipping == UIDrawCall.Clipping.SoftClip)
		{
			minArea.x += clipSoftness.x;
			minArea.y += clipSoftness.y;
			maxArea.x -= clipSoftness.x;
			maxArea.y -= clipSoftness.y;
		}
		return NGUIMath.ConstrainRect(minRect, maxRect, minArea, maxArea);
	}

	/// <summary>
	/// Constrain the current target position to be within panel bounds.
	/// </summary>

	public bool ConstrainTargetToBounds (Transform target, ref Bounds targetBounds, bool immediate)
	{
		Vector3 offset = CalculateConstrainOffset(targetBounds.min, targetBounds.max);

		if (offset.sqrMagnitude > 0f)
		{
			if (immediate)
			{
				target.localPosition += offset;
				targetBounds.center += offset;
				SpringPosition sp = target.GetComponent<SpringPosition>();
				if (sp != null) sp.enabled = false;
			}
			else
			{
				SpringPosition sp = SpringPosition.Begin(target.gameObject, target.localPosition + offset, 13f);
				sp.ignoreTimeScale = true;
				sp.worldSpace = false;
			}
			return true;
		}
		return false;
	}

	/// <summary>
	/// Constrain the specified target to be within the panel's bounds.
	/// </summary>

	public bool ConstrainTargetToBounds (Transform target, bool immediate)
	{
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(cachedTransform, target);
		return ConstrainTargetToBounds(target, ref bounds, immediate);
	}

	/// <summary>
	/// Find the UIPanel responsible for handling the specified transform.
	/// </summary>

	static public UIPanel Find (Transform trans) { return Find(trans, false, -1); }

	/// <summary>
	/// Find the UIPanel responsible for handling the specified transform.
	/// </summary>

	static public UIPanel Find (Transform trans, bool createIfMissing) { return Find(trans, createIfMissing, -1); }

	/// <summary>
	/// Find the UIPanel responsible for handling the specified transform.
	/// </summary>

	static public UIPanel Find (Transform trans, bool createIfMissing, int layer)
	{
		UIPanel panel = null;

		while (panel == null && trans != null)
		{
			panel = trans.GetComponent<UIPanel>();
			if (panel != null) return panel;
			if (trans.parent == null) break;
			trans = trans.parent;
		}
		return createIfMissing ? NGUITools.CreateUI(trans, false, layer) : null;
	}

	/// <summary>
	/// Get the size of the game window in pixels.
	/// </summary>

	Vector2 GetWindowSize ()
	{
		UIRoot rt = root;
#if UNITY_EDITOR
		Vector2 size = GetMainGameViewSize();
		if (rt != null) size *= rt.GetPixelSizeAdjustment(Mathf.RoundToInt(size.y));
#else
		Vector2 size = new Vector2(Screen.width, Screen.height);
		if (rt != null) size *= rt.GetPixelSizeAdjustment(Screen.height);
#endif
		return size;
	}

	/// <summary>
	/// Panel's size -- which is either the clipping rect, or the screen dimensions.
	/// </summary>

	public Vector2 GetViewSize ()
	{
		bool clip = (mClipping != UIDrawCall.Clipping.None);
#if UNITY_EDITOR
		Vector2 size = clip ? new Vector2(mClipRange.z, mClipRange.w) : GetMainGameViewSize();
#else
		Vector2 size = clip ? new Vector2(mClipRange.z, mClipRange.w) : new Vector2(Screen.width, Screen.height);
#endif
		if (!clip)
		{
			UIRoot rt = root;
#if UNITY_EDITOR
			if (rt != null) size *= rt.GetPixelSizeAdjustment(Mathf.RoundToInt(size.y));
#else
			if (rt != null) size *= rt.GetPixelSizeAdjustment(Screen.height);
#endif
		}
		return size;
	}

#if UNITY_EDITOR

	static int mSizeFrame = -1;
	static System.Reflection.MethodInfo s_GetSizeOfMainGameView;
	static Vector2 mGameSize = Vector2.one;

	/// <summary>
	/// Major hax to get the size of the game view window.
	/// </summary>

	static public Vector2 GetMainGameViewSize ()
	{
		int frame = Time.frameCount;

		if (mSizeFrame != frame)
		{
			mSizeFrame = frame;

			if (s_GetSizeOfMainGameView == null)
			{
				System.Type type = System.Type.GetType("UnityEditor.GameView,UnityEditor");
				s_GetSizeOfMainGameView = type.GetMethod("GetSizeOfMainGameView",
					System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			}
			mGameSize = (Vector2)s_GetSizeOfMainGameView.Invoke(null, null);
		}
		return mGameSize;
	}

	/// <summary>
	/// Draw a visible pink outline for the clipped area.
	/// </summary>

	void OnDrawGizmos ()
	{
		if (mCam == null) return;

		Vector2 size = GetViewSize();
		GameObject go = UnityEditor.Selection.activeGameObject;
		bool selected = (go != null) && (NGUITools.FindInParents<UIPanel>(go) == this);
		bool clip = (mClipping != UIDrawCall.Clipping.None);

		Transform t = clip ? transform : (mCam != null ? mCam.transform : null);

		if (t != null)
		{
			Vector3 pos = clip ? new Vector3(mClipOffset.x + mClipRange.x, mClipOffset.y + mClipRange.y) : Vector3.zero;
			Gizmos.matrix = t.localToWorldMatrix;

			if (selected)
			{
				if (mClipping == UIDrawCall.Clipping.SoftClip)
				{
					if (UnityEditor.Selection.activeGameObject == gameObject)
					{
						Gizmos.color = new Color(1f, 0f, 0.5f);
						size.x -= mClipSoftness.x * 2f;
						size.y -= mClipSoftness.y * 2f;
						Gizmos.DrawWireCube(pos, size);
					}
					else
					{
						Gizmos.color = new Color(0.5f, 0f, 0.5f);
						Gizmos.DrawWireCube(pos, size);

						Gizmos.color = new Color(1f, 0f, 0.5f);
						size.x -= mClipSoftness.x * 2f;
						size.y -= mClipSoftness.y * 2f;
						Gizmos.DrawWireCube(pos, size);
					}
				}
				else
				{
					Gizmos.color = new Color(1f, 0f, 0.5f);
					Gizmos.DrawWireCube(pos, size);
				}
			}
			else
			{
				Gizmos.color = new Color(0.5f, 0f, 0.5f);
				Gizmos.DrawWireCube(pos, size);
			}
		}
	}
#endif
}
