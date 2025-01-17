using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Projector))]
public class ShapeProjectorsControl : MonoBehaviour
{
    #region variables

    private Projector projector;

    public bool enableAnim = false;
    public Color tintColor = Color.red;
    public Color outlineColor = Color.white;

    public float colorTransition = 1.1f;
    public float colorScale = 1.0f;
    public float outlineWidth = 0.1f;
    public float outlineScale = 1.0f;
    public float angle = 0.0f;
    public float argAccuracy = 0.01f;

    private Material projectorMat;

    private Color innerTintColor = Color.red;
    private Color innerOutlineColor = Color.white;

    private Vector4 innerArg = new Vector4(1.01f, 1.0f, 0.1f, 1.0f);
    private float innerAngle = 0.0f;

    private bool hasAngle = false;
    private static float colorT = 1 / 256.0f;
    #endregion

    void Awake()
    {        
        if (projector == null)
            projector = GetComponent<Projector>();
        if (projector != null)
        {
            projectorMat = projector.material;
            if (projectorMat != null)
            {
                innerTintColor = projectorMat.GetColor("_TintColor");
                innerOutlineColor = projectorMat.GetColor("_OutlineColor");
                innerArg = projectorMat.GetVector("_Arg");
                hasAngle = projectorMat.HasProperty("_Angle");
                if (hasAngle)
                    innerAngle = projectorMat.GetFloat("_Angle");
            }
        }
    }

    void Update()
    {
        if (enableAnim && projectorMat != null)
        {
            //color
            float deltaR = Mathf.Abs(innerTintColor.r - tintColor.r);
            float deltaG = Mathf.Abs(innerTintColor.g - tintColor.g);
            float deltaB = Mathf.Abs(innerTintColor.b - tintColor.b);
            float deltaA = Mathf.Abs(innerTintColor.a - tintColor.a);

            if (deltaR >= colorT || deltaG >= colorT || deltaB >= colorT || deltaA >= colorT)
            {
                innerTintColor = tintColor;
                projectorMat.SetColor("_TintColor", tintColor);
            }
            deltaR = Mathf.Abs(innerOutlineColor.r - outlineColor.r);
            deltaG = Mathf.Abs(innerOutlineColor.g - outlineColor.g);
            deltaB = Mathf.Abs(innerOutlineColor.b - outlineColor.b);
            deltaA = Mathf.Abs(innerOutlineColor.a - outlineColor.a);

            if (deltaR >= colorT || deltaG >= colorT || deltaB >= colorT || deltaA >= colorT)
            {
                innerOutlineColor = outlineColor;
                projectorMat.SetColor("_OutlineColor", outlineColor);
            }

            float deltaColorTransition = Mathf.Abs(innerArg.x - colorTransition);
            float deltaColorScale = Mathf.Abs(innerArg.y - colorScale);
            float deltaOutlineWidth = Mathf.Abs(innerArg.z - outlineWidth);
            float deltaOutlineScale = Mathf.Abs(innerArg.w - outlineScale);

            if (deltaColorTransition >= argAccuracy || 
                deltaColorScale > argAccuracy||
                deltaOutlineWidth > argAccuracy ||
                deltaOutlineScale > argAccuracy)
            {
                innerArg.x = colorTransition;
                innerArg.y = colorScale;
                innerArg.z = outlineWidth;
                innerArg.w = outlineScale;
                projectorMat.SetVector("_Arg", innerArg);
            }
            if (hasAngle)
            {
                float deltaAngle = Mathf.Abs(innerAngle - angle);
                if (deltaAngle >= argAccuracy)
                {
                    projectorMat.SetFloat("_Angle", angle);
                }
            }

        }
    }   
}
