using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Projector))]
public class ProjectorsControl : MonoBehaviour
{
    //public delegate void TransitionCompleted();
    //public event TransitionCompleted OnPulseCompleted;

    #region variables

    private Projector projector;

    public bool enableAnim = false;
    public Color color = Color.white;
    public float falloff = 1.0f;
    public float amplify = 1.0f;
    public float argAccuracy = 0.01f;


    private Texture texture = null;    

    private Material projectorMat;
    private Color innerColor = Color.white;
    private Vector4 innerArg = new Vector4(1.0f, 1.0f, 0, 0);
    private Texture innerTexture = null;

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
                if (projectorMat.HasProperty("_Color"))
                    innerColor = projectorMat.GetColor("_Color");
                if (projectorMat.HasProperty("_Args"))
                    innerArg = projectorMat.GetVector("_Args");
                if (projectorMat.HasProperty("_ShadowTex"))
                    innerTexture = projectorMat.GetTexture("_ShadowTex");
            }
        }
    }

    void Update()
    {
        if (enableAnim && projectorMat != null)
        {
            //color
            float deltaR = Mathf.Abs(innerColor.r - color.r);
            float deltaG = Mathf.Abs(innerColor.g - color.g);
            float deltaB = Mathf.Abs(innerColor.b - color.b);
            float deltaA = Mathf.Abs(innerColor.a - color.a);

            if (deltaR >= colorT || deltaG >= colorT || deltaB >= colorT || deltaA >= colorT)
            {
                innerColor = color;
                projectorMat.SetColor("_Color", color);
            }

            float deltaFalloff = Mathf.Abs(innerArg.x - falloff);
            float deltaAmp = Mathf.Abs(innerArg.y - amplify);
            if (deltaFalloff >= argAccuracy || deltaAmp > argAccuracy)
            {
                innerArg.x = falloff;
                innerArg.y = amplify;
                projectorMat.SetVector("_Args", innerArg);
            }

            if (texture != null && innerTexture != texture)
            {
                innerTexture = texture;
                projectorMat.SetTexture("_ShadowTex", innerTexture);
            }
        }
    }
    //void FixedUpdate()
    //{
    //    if (rotate)
    //        transform.Rotate(Vector3.forward * rotationSpeed * rotationOffset * Time.deltaTime);

    //    if (pulse)
    //    {
    //        float pulseSize = projector.orthographicSize;
    //        float stepAmount = Time.deltaTime * pulseSpeed;

    //        if (!pulseFlip)
    //            pulseSize = Mathf.Lerp(pulseSize, pulseMax, stepAmount);
    //        else
    //            pulseSize = Mathf.Lerp(pulseSize, pulseMin, stepAmount);


    //        if (pulseTime < 1)
    //            pulseTime += stepAmount;
    //        else
    //        {
    //            pulseTime = 0;

    //            if (pulseLoop)
    //                pulseFlip = !pulseFlip;
    //            else
    //                pulse = false;

    //            if (OnPulseCompleted != null)
    //                OnPulseCompleted();
    //        }

    //        projector.orthographicSize = pulseSize;
    //    }

    //    if (colorBlend && colors.Count > 0)
    //    {
    //        float stepAmount = Time.deltaTime * colorSpeed;

    //        projector.material.color = Color.Lerp(projector.material.color, colors[colorIndex], stepAmount);

    //        if (colorTime < 1)
    //            colorTime += stepAmount;
    //        else
    //        {
    //            colorTime = 0;

    //            if (colorIndex < colors.Count - 1)
    //                colorIndex++;
    //            else
    //                colorIndex = 0;
    //        }
    //    }
    //}

    ///// <summary>
    ///// The default size of the projector.
    ///// </summary>
    //public float DefaultSize
    //{
    //    get
    //    {
    //        if (projector == null)
    //            projector = GetComponent<Projector>();

    //        if (projector != null)
    //            return projector.orthographicSize;

    //        return 0;
    //    }

    //    set
    //    {
    //        if (projector == null)
    //            projector = GetComponent<Projector>();

    //        if (projector != null)
    //            projector.orthographicSize = value * Scale;
    //    }
    //}

    ///// <summary>
    ///// The default texture used by the projector.
    ///// </summary>
    //public Texture DefaultTexture
    //{
    //    get
    //    {
    //        if (projector == null)
    //            projector = GetComponent<Projector>();

    //        if (projector != null && projector.material != null)
    //            return projector.material.GetTexture("_ShadowTex");

    //        return null;
    //    }

    //    set
    //    {
    //        if (projector == null)
    //            projector = GetComponent<Projector>();

    //        if (projector != null && projector.material != null)
    //            projector.material.SetTexture("_ShadowTex", value);
    //    }
    //}

    ///// <summary>
    ///// The default color used by the projector.
    ///// </summary>
    //public Color DefaultColor
    //{
    //    get
    //    {
    //        if (projector == null)
    //            projector = GetComponent<Projector>();

    //        if (projector != null && projector.material != null)
    //            return projector.material.GetColor("_Color");

    //        return Color.black;
    //    }

    //    set
    //    {
    //        if (projector == null)
    //            projector = GetComponent<Projector>();

    //        if (projector != null && projector.material != null)
    //            projector.material.SetColor("_Color", value);
    //    }
    //}

    ///// <summary>
    ///// Adjusts the scale of the current projector based on a defined size value.
    ///// </summary>
    ///// <param name="size"></param>
    ///// <param name="scale"></param>
    //public void SetScale(float size, float scale)
    //{
    //    Scale = scale;
    //    DefaultSize = size;

    //    pulseMin *= scale;
    //    pulseMax *= scale;
    //}

    ///// <summary>
    ///// Sets the projectors material.
    ///// </summary>
    ///// <param name="material"></param>
    //public void SetMaterial(Material material)
    //{
    //    if (projector == null)
    //        projector = GetComponent<Projector>();

    //    if (projector != null)
    //        projector.material = material;
    //}

    ///// <summary>
    ///// Updates default values when creating the prefab.
    ///// </summary>
    //public void Initialize()
    //{
    //    if (projector == null)
    //        projector = GetComponent<Projector>();

    //    projector.orthographic = true;
    //}

    ///// <summary>
    ///// Forces an updated to reflect the current values.
    ///// </summary>
    //public void Refresh()
    //{
    //    Vector3 lastRotation = this.transform.localRotation.eulerAngles;
    //    this.transform.localRotation = Quaternion.Euler(lastRotation.x, rotation, lastRotation.z);
    //}
}
