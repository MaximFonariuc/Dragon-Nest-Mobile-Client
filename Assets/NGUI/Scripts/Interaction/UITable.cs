//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// All children added to the game object with this script will be arranged into a table
/// with rows and columns automatically adjusting their size to fit their content
/// (think "table" tag in HTML).
/// </summary>

[AddComponentMenu("NGUI/Interaction/Table")]
public class UITable : UIWidgetContainer
{
    public delegate Bounds CalculateBounds (Transform trans, bool considerInactive);
	public delegate void OnReposition ();

	public enum Direction
	{
		Down,
		Up,
	}

	public enum Sorting
	{
		None,
		Alphabetic,
		Horizontal,
		Vertical,
		Custom,
	}

	/// <summary>
	/// How many columns there will be before a new line is started. 0 means unlimited.
	/// </summary>

	public int columns = 0;

	/// <summary>
	/// Which way the new lines will be added.
	/// </summary>

	public Direction direction = Direction.Down;

	/// <summary>
	/// How to sort the grid's elements.
	/// </summary>

	public Sorting sorting = Sorting.None;

	/// <summary>
	/// Whether inactive children will be discarded from the table's calculations.
	/// </summary>

	public bool hideInactive = true;

	/// <summary>
	/// Whether the parent container will be notified of the table's changes.
	/// </summary>

	public bool keepWithinPanel = false;

	/// <summary>
	/// Padding around each entry, in pixels.
	/// </summary>

	public Vector2 padding = Vector2.zero;

	/// <summary>
	/// Delegate function that will be called when the table repositions its content.
	/// </summary>

	public OnReposition onReposition;

	protected UIPanel mPanel;
	protected bool mInitDone = false;
	protected bool mReposition = false;
	protected List<Transform> mChildren = new List<Transform>();

	// Use the 'sorting' property instead
	[HideInInspector][SerializeField] bool sorted = false;

    UIScrollView sv;
    CalculateBounds calculateBoundsCb;
	/// <summary>
	/// Reposition the children on the next Update().
	/// </summary>

	public bool repositionNow { set { if (value) { mReposition = true; enabled = true; } } }

	/// <summary>
	/// Returns the list of table's children, sorted alphabetically if necessary.
	/// </summary>

	public List<Transform> children
	{
		get
		{
			if (mChildren.Count == 0)
			{
				Transform myTrans = transform;
				mChildren.Clear();

				for (int i = 0; i < myTrans.childCount; ++i)
				{
					Transform child = myTrans.GetChild(i);
					if (child && child.gameObject && (!hideInactive || NGUITools.GetActive(child.gameObject)))
						mChildren.Add(child);
				}
				
				if (sorting != Sorting.None || sorted)
				{
					if (sorting == Sorting.Alphabetic) mChildren.Sort(UIGrid.SortByName);
					else if (sorting == Sorting.Horizontal) mChildren.Sort(UIGrid.SortHorizontal);
					else if (sorting == Sorting.Vertical) mChildren.Sort(UIGrid.SortVertical);
					else Sort(mChildren);
				}
			}
			return mChildren;
		}
	}

	/// <summary>
	/// Want your own custom sorting logic? Override this function.
	/// </summary>

	protected virtual void Sort (List<Transform> list) { list.Sort(UIGrid.SortByName); }

	/// <summary>
	/// Positions the grid items, taking their own size into consideration.
	/// </summary>

	protected void RepositionVariableSize (List<Transform> children, CalculateBounds calculateBounds = null)
	{
        if(calculateBounds == null)
            calculateBounds = NGUIMath.CalculateRelativeWidgetBounds;

		float xOffset = 0;
		float yOffset = 0;

		int cols = columns > 0 ? children.Count / columns + 1 : 1;
		int rows = columns > 0 ? columns : children.Count;

		Bounds[,] bounds = new Bounds[cols, rows];
		Bounds[] boundsRows = new Bounds[rows];
		Bounds[] boundsCols = new Bounds[cols];

		int x = 0;
		int y = 0;

		for (int i = 0, imax = children.Count; i < imax; ++i)
		{
			Transform t = children[i];
            Bounds b = calculateBounds(t, !hideInactive);

			Vector3 scale = t.localScale;
			b.min = Vector3.Scale(b.min, scale);
			b.max = Vector3.Scale(b.max, scale);
			bounds[y, x] = b;

			boundsRows[x].Encapsulate(b);
			boundsCols[y].Encapsulate(b);

			if (++x >= columns && columns > 0)
			{
				x = 0;
				++y;
			}
		}

		x = 0;
		y = 0;

		for (int i = 0, imax = children.Count; i < imax; ++i)
		{
			Transform t = children[i];
			Bounds b = bounds[y, x];
			Bounds br = boundsRows[x];
			Bounds bc = boundsCols[y];

			Vector3 pos = t.localPosition;
			pos.x = xOffset + b.extents.x - b.center.x;
			pos.x += b.min.x - br.min.x + padding.x;

			if (direction == Direction.Down)
			{
				pos.y = -yOffset - b.extents.y - b.center.y;
				pos.y += (b.max.y - b.min.y - bc.max.y + bc.min.y) * 0.5f - padding.y;
			}
			else
			{
				pos.y = yOffset + (b.extents.y - b.center.y);
				pos.y -= (b.max.y - b.min.y - bc.max.y + bc.min.y) * 0.5f - padding.y;
			}

			xOffset += br.max.x - br.min.x + padding.x * 2f;

			t.localPosition = pos;

			if (++x >= columns && columns > 0)
			{
				x = 0;
				++y;

				xOffset = 0f;
				yOffset += bc.size.y + padding.y * 2f;
			}
		}
	}

	/// <summary>
	/// Recalculate the position of all elements within the table, sorting them alphabetically if necessary.
	/// </summary>

	[ContextMenu("Execute")]
	public virtual void Reposition ()
	{
		if (Application.isPlaying && !mInitDone && NGUITools.GetActive(this))
		{
			mReposition = true;
			return;
		}

		if (!mInitDone) Init();

		mReposition = false;
		Transform myTrans = transform;
		mChildren.Clear();
		List<Transform> ch = children;
		if (ch.Count > 0) RepositionVariableSize(ch);

		if (keepWithinPanel && mPanel != null)
		{
			mPanel.ConstrainTargetToBounds(myTrans, true);
			//UIScrollView sv = mPanel.GetComponent<UIScrollView>();
			if (sv != null) sv.UpdateScrollbars(true);
		}

		if (onReposition != null)
			onReposition();
	}

	/// <summary>
	/// Position the grid's contents when the script starts.
	/// </summary>

	protected virtual void Start ()
	{
        calculateBoundsCb = _CalculateBoundsOnlyOneLevel;
		Init();
        sv = mPanel.GetComponent<UIScrollView>();
		Reposition();
		enabled = false;
	}

	/// <summary>
	/// Find the necessary components.
	/// </summary>

	protected virtual void Init ()
	{
		mInitDone = true;
		mPanel = NGUITools.FindInParents<UIPanel>(gameObject);
	}

	/// <summary>
	/// Is it time to reposition? Do so now.
	/// </summary>

	protected virtual void LateUpdate ()
	{
		if (mReposition) Reposition();
		enabled = false;
	}

    public void RepositionOnlyOneLevel()
    {
        if (!mInitDone) Init();

        mReposition = false;
        Transform myTrans = transform;
        mChildren.Clear();
        List<Transform> ch = children;
        if (ch.Count > 0) RepositionVariableSize(ch, calculateBoundsCb);
    }

    List<UIWidget> widgets = new List<UIWidget>();
    protected Bounds _CalculateBoundsOnlyOneLevel(Transform trans, bool considerInactive)
    {
        Transform relativeTo = trans;
        Transform content = trans;

        if (content != null)
        {
            widgets.Clear();
            int childCount = content.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Transform t = content.GetChild(i);
                if (!considerInactive && !t.gameObject.activeSelf)
                    continue;
                UIWidget w = t.GetComponent<UIWidget>();
                if (w != null)
                    widgets.Add(w);
            }
            if (widgets.Count > 0)
            {
                Vector3 vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                Vector3 vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

                Matrix4x4 toLocal = relativeTo.worldToLocalMatrix;
                bool isSet = false;
                Vector3 v;

                for (int i = 0, imax = widgets.Count; i < imax; ++i)
                {
                    UIWidget w = widgets[i];
                    if (!considerInactive && !w.enabled) continue;

                    Vector3[] corners = w.worldCorners;

                    for (int j = 0; j < 4; ++j)
                    {
                        //v = root.InverseTransformPoint(corners[j]);
                        v = toLocal.MultiplyPoint3x4(corners[j]);
                        vMax = Vector3.Max(v, vMax);
                        vMin = Vector3.Min(v, vMin);
                    }
                    isSet = true;
                }

                if (isSet)
                {
                    Bounds b = new Bounds(vMin, Vector3.zero);
                    b.Encapsulate(vMax);
                    return b;
                }
            }
        }
        return new Bounds(Vector3.zero, Vector3.zero);
    }
}
