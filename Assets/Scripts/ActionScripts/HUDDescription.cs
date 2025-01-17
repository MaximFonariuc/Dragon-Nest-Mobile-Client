using UnityEngine;
using XUtliPoolLib;
using System.Collections;

public class HUDDescription : MonoBehaviour, IXHUDDescription

{
	/// <summary>
	/// Curve used to move entries with time.
	/// </summary>
	
	public AnimationCurve offsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(3f, 40f) });
	
	/// <summary>
	/// Curve used to fade out entries with time.
	/// </summary>
	
	public AnimationCurve alphaCurve = new AnimationCurve(new Keyframe[] { new Keyframe(1f, 1f), new Keyframe(3f, 0f) });
	
	/// <summary>
	/// Curve used to scale the entries.
	/// </summary>
	
	public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.25f, 1f) });

	public AnimationCurve GetPosCurve()
	{
		return offsetCurve;
	}

    public AnimationCurve GetAlphaCurve()
    {
        return alphaCurve;
    }

	public AnimationCurve GetScaleCurve()
	{
		return scaleCurve;
	}

    public bool Deprecated
    {
        get;
        set;
    }
}
