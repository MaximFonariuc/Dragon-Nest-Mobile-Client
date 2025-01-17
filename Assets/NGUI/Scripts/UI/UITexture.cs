//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using XUtliPoolLib;

/// <summary>
/// If you don't have or don't wish to create an atlas, you can simply use this script to draw a texture.
/// Keep in mind though that this will create an extra draw call with each UITexture present, so it's
/// best to use it only for backgrounds or temporary visible widgets.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Texture")]
public class UITexture : UIWidget, IDrawCallMat
{
	public enum Flip
	{
		Nothing,
		Horizontally,
		Vertically,
		Both,
	}

    //public enum TextureType
    //{
    //    Normal,
    //    List,
    //}
    /// <summary>
    /// TextureList
    /// </summary>
    public enum TexType
    {
        Horizontally2 = 0,
        Horizontally4,
        Vertically2,
        Vertically4,
        Normal
    }

    //[HideInInspector][SerializeField]
    //public TextureType mType;

    [HideInInspector]
    [SerializeField]
    public byte mtexType;
    [HideInInspector]
    [SerializeField]
    public bool mIsRuntimeLoad;
    [HideInInspector][SerializeField]
    protected Rect mRect = new Rect(0f, 0f, 1f, 1f);
	[HideInInspector][SerializeField]
    protected Flip mFlip = Flip.Nothing;
    [HideInInspector]
    [SerializeField]
    public string texPath;
    [HideInInspector]
    [SerializeField]
    public string shaderName;

    [HideInInspector]
    [System.NonSerialized]
    public Texture mTexture;
    [HideInInspector]
    [System.NonSerialized]
    public Texture mTexture1;

    protected Texture mAlphaTexture;
    protected Texture mAlphaTexture1;
    protected Material mMat;
    [HideInInspector]
    [System.NonSerialized]
    public Shader mShader;

    public static Shader sepTexAlpha = null;
    public static Shader colorTex = null;
    public static Shader sepTexAlphaH2 = null;
    public static Shader colorTexH2 = null;
    public static Shader sepTexAlphaH4 = null;
    public static Shader colorTexH4 = null;

    public static byte horizontally2 = 0;
    public static byte horizontally4 = 1;
    public static byte vertically2 = 2;
    public static byte vertically4 = 3;
    public static byte normalTex = 4;

    private static Vector4 UVScaleHorizontally2 = new Vector4(-0.5f, 0.0f, 2.0f, 1.0f);
    private static Vector4 UVRangeHorizontally2 = new Vector4(1.0f, 1.0f, 0.5f, 0.0f);
    private static Vector4 UVScaleVertically2 = new Vector4(0.0f, -0.5f, 1.0f, 2.0f);
    private static Vector4 UVRangeVertically2 = new Vector4(1.0f, 1.0f, 0.0f, 0.5f);

    private static Vector4 UVScaleHorizontally4 = new Vector4(2.0f, 0.5f, -1.0f, 0.5f);
    private static Vector4 UVRangeHorizontally4 = new Vector4(0.5f, 1.0f, 0.0f, 0.0f);
    private static Vector4 UVScaleVertically4 = new Vector4(0.5f, 2.0f, 0.5f, -1.0f);
    private static Vector4 UVRangeVertically4 = new Vector4(1.0f, 0.5f, 0.0f, 0.0f);
    private static Vector2 UVRect0 = new Vector2(0, 0);
    private static Vector2 UVRect1 = new Vector2(0, 1);
    private static Vector2 UVRect2 = new Vector2(1, 1);
    private static Vector2 UVRect3 = new Vector2(1, 0);
    private static string maskTexPath = "Materials/zhezhao";
    /// <summary>
    /// Texture used by the UITexture. You can set it directly, without the need to specify a material.
    /// </summary>

    public override Texture mainTexture
	{
		get
		{
            return mTexture;
        }
		set
		{
            if (mTexture != value)
            {
                RemoveFromPanel();
                mTexture = value;
                MarkAsChanged();
            }
		}
	}

    /// <summary>
    /// Texture used by the UITexture. You can set it directly, without the need to specify a material.
    /// </summary>
    public override Texture alphaTexture
    {
        get
        {
            return mAlphaTexture;
        }
    }
	/// <summary>
	/// Material used by the widget.
	/// </summary>

	public override Material material
	{
		get
		{
			return mMat;
		}
		set
		{
			if (mMat != value)
			{
				RemoveFromPanel();
				mShader = null;
				mMat = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Shader used by the texture when creating a dynamic material (when the texture was specified, but the material was not).
	/// </summary>

	public override Shader shader
	{
		get
		{
            if (mMat != null)
                return mMat.shader;
            if (mShader == null)
                ResetShader();
            return mShader;
		}
		set
		{
			if (mShader != value)
			{
				RemoveFromPanel();
				mShader = value;
				mMat = null;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Sprite texture setting.
	/// </summary>

	public Flip flip
	{
		get
		{
			return mFlip;
		}
		set
		{
			if (mFlip != value)
			{
				mFlip = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Whether the texture is using a premultiplied alpha material.
	/// </summary>

	public bool premultipliedAlpha
	{
		get
		{
			return false;
		}
	}

	/// <summary>
	/// UV rectangle used by the texture.
	/// </summary>

	public Rect uvRect
	{
		get
		{
			return mRect;
		}
		set
		{
			if (mRect != value)
			{
				mRect = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Widget's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
	/// This function automatically adds 1 pixel on the edge if the texture's dimensions are not even.
	/// It's used to achieve pixel-perfect sprites even when an odd dimension widget happens to be centered.
	/// </summary>

	public override Vector4 drawingDimensions
	{
		get
		{
			Vector2 offset = pivotOffset;

			float x0 = -offset.x * mWidth;
			float y0 = -offset.y * mHeight;
			float x1 = x0 + mWidth;
			float y1 = y0 + mHeight;

            int w = 1,h = 1;
            if(mtexType == normalTex)
            {
                Texture tex = mainTexture;
                w = (tex != null) ? tex.width : mWidth;
                h = (tex != null) ? tex.height : mHeight;
            }
            else
            {
                GetRealSize(out w, out h);
            }

			if ((w & 1) != 0) x1 -= (1f / w) * mWidth;
			if ((h & 1) != 0) y1 -= (1f / h) * mHeight;

			return new Vector4(
				mDrawRegion.x == 0f ? x0 : Mathf.Lerp(x0, x1, mDrawRegion.x),
				mDrawRegion.y == 0f ? y0 : Mathf.Lerp(y0, y1, mDrawRegion.y),
				mDrawRegion.z == 1f ? x1 : Mathf.Lerp(x0, x1, mDrawRegion.z),
				mDrawRegion.w == 1f ? y1 : Mathf.Lerp(y0, y1, mDrawRegion.w));
		}
	}

    static Shader GetSepTexAlpha()
    {
        if (sepTexAlpha == null)
        {
            //sepTexAlpha = Shader.Find("Custom/UI/SeparateColorAlpha");
            sepTexAlpha = XUtliPoolLib.ShaderManager.singleton.FindShader("SeparateColorAlpha", "Custom/UI/SeparateColorAlpha");
        }
        return sepTexAlpha;
    }

    protected static Shader GetColorTex()
    {
        if (colorTex == null)
        {
            //colorTex = Shader.Find("Custom/UI/ColorTexture");
            colorTex = XUtliPoolLib.ShaderManager.singleton.FindShader("ColorTexture", "Custom/UI/ColorTexture");
        }
        return colorTex;
    }


    protected virtual void ResetShader()
    {
        if(!string.IsNullOrEmpty(shaderName))
        {
            if (mShader == null)
                mShader = UIDrawCall.GetDefaultShader(shaderName);
            return;
        }
        if (mTexture == null)
        {
            mShader = null;
            return;
        }

        if (mtexType == horizontally2 || mtexType == vertically2)
        {
            if (colorTexH2 == null)
            {
                //colorTexH2 = Shader.Find("Custom/UI/TextureList2");
                colorTexH2 = XUtliPoolLib.ShaderManager.singleton.FindShader("TextureList2", "Custom/UI/TextureList2");
            }
            mShader = colorTexH2;
        }
        else if (mtexType == horizontally4 || mtexType == vertically4)
        {
            if (colorTexH4 == null)
            {
                //colorTexH4 = Shader.Find("Custom/UI/TextureList4");
                colorTexH4 = XUtliPoolLib.ShaderManager.singleton.FindShader("TextureList4", "Custom/UI/TextureList4");
            }
            mShader = colorTexH4;
        }
        else if(mAlphaTexture == null)
        {
            mShader = GetColorTex();
        }
        else
        {
            mShader = GetSepTexAlpha();
        }

    }
    private void UnLoadTexture()
    {
        if (mTexture != null)
        {
            XResourceLoaderMgr.singleton.UnSafeDestroyShareResource(texPath, ".png", mTexture);
            mTexture = null;
        }
        if (mAlphaTexture != null)
        {
            if (shaderName == "Custom/UI/Mask")
            {
                XResourceLoaderMgr.singleton.UnSafeDestroyShareResource(maskTexPath, ".png", mAlphaTexture);
            }
            else
            {
                XResourceLoaderMgr.singleton.UnSafeDestroyShareResource("atlas/UI/Alpha/" + mAlphaTexture.name, ".png", mAlphaTexture);
            }
            
            mAlphaTexture = null;
        }
        if(mTexture1!=null)
        {
            XResourceLoaderMgr.singleton.UnSafeDestroyShareResource(texPath + "_2", ".png", mTexture1);
            mTexture1 = null;
        }
    }

    protected virtual void LoadTexture()
    {
        if (string.IsNullOrEmpty(texPath))
        {            
            mTexture1 = null;
            mAlphaTexture = null;
            mAlphaTexture1 = null;
            texPath = "";
            RemoveFromPanel();
            mTexture = null;
            MarkAsChanged();
        }
        else
        {
            Texture tex = XResourceLoaderMgr.singleton.GetSharedResource<Texture>(texPath, ".png") as Texture;
            if (tex != null)
            {
                switch (mtexType)
                {
                    case 0://Horizontally2
                    case 2://Vertically2
                        {
                            mTexture1 = XResourceLoaderMgr.singleton.GetSharedResource<Texture>(texPath + "_2", ".png") as Texture;

                        }
                        break;
                    case 1://Horizontally4
                    case 3://Vertically4
                        {
                            mTexture1 = null;
                        }
                        break;
                    case 4://normal                        
                        {
                            mAlphaTexture = XResourceLoaderMgr.singleton.GetSharedResource<Texture>("atlas/UI/Alpha/" + tex.name + "_A", ".png", false);
                        }                        
                        break;
                }
            }
            mainTexture = tex;
        }
    }

    //protected virtual void InitAlphaTexture(bool init)
    //{
    //    if (hasLoadAlpha)
    //        return;
    //    hasLoadAlpha = true;
    //    if (mTexture == null)
    //    {
    //        DestroyTex(null, mAlphaTexture);
    //        if (mMat != null)
    //        {
    //            mMat.SetTexture("_MainTex", null);
    //        }
    //    }
    //    else
    //    {
    //        mAlphaTexture = XResourceLoaderMgr.singleton.GetSharedResource<Texture>("atlas/UI/Alpha/" + mTexture.name + "_A", ".png", false);
    //    }
    //    ResetShader();
    //}
    protected override void OnStart ()
    {
        base.OnStart();
#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(texPath))
#else
        if (!mIsRuntimeLoad && !string.IsNullOrEmpty(texPath))
#endif
        {
            mtexType = GetTextureListType(texPath);
            LoadTexture();            
        }
        ResetShader();
    }
//#if UNITY_EDITOR
    public void Refresh()
    {
        LoadTexture();
        ResetShader();
    }
    //    protected override void OnEnable()
    //    {
    //        base.OnEnable();
    //        LoadTexture();
    //        ResetShader();
    //}
//#endif
    protected override void OnDestroy()
    {        
        //Destroy Texture
        if (mMat != null)
        {
            mMat.SetTexture("_MainTex", null);
        }
        UnLoadTexture();
        base.OnDestroy();
    }

    /// <summary>
    /// Adjust the scale of the widget to make it pixel-perfect.
    /// </summary>

    public override void MakePixelPerfect ()
	{
        if(mtexType == normalTex)
        {
            Texture tex = mainTexture;

            if (tex != null)
            {
                int x = tex.width;
                if ((x & 1) == 1) ++x;

                int y = tex.height;
                if ((y & 1) == 1) ++y;

                width = x;
                height = y;
            }
        }
        else
        {
            int w = 1;
            int h = 1;
            GetRealSize(out w, out h);
            if ((w & 1) == 1) ++w;
            if ((h & 1) == 1) ++h;

            width = w;
            height = h;
            base.MakePixelPerfect();
        }
		base.MakePixelPerfect();
	}

	/// <summary>
	/// Virtual function called by the UIPanel that fills the buffers.
	/// </summary>

	public override void OnFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        if (mtexType == normalTex)
        {
            if (mTexture == null)
                return;
            Color colF = color;
            colF.a = finalAlpha;
            Color32 col = premultipliedAlpha ? NGUITools.ApplyPMA(colF) : colF;

            Vector4 v = drawingDimensions;

            verts.Add(new Vector3(v.x, v.y));
            verts.Add(new Vector3(v.x, v.w));
            verts.Add(new Vector3(v.z, v.w));
            verts.Add(new Vector3(v.z, v.y));

            if (mFlip == Flip.Horizontally)
            {
                uvs.Add(new Vector2(mRect.xMax, mRect.yMin));
                uvs.Add(new Vector2(mRect.xMax, mRect.yMax));
                uvs.Add(new Vector2(mRect.xMin, mRect.yMax));
                uvs.Add(new Vector2(mRect.xMin, mRect.yMin));
            }
            else if (mFlip == Flip.Vertically)
            {
                uvs.Add(new Vector2(mRect.xMin, mRect.yMax));
                uvs.Add(new Vector2(mRect.xMin, mRect.yMin));
                uvs.Add(new Vector2(mRect.xMax, mRect.yMin));
                uvs.Add(new Vector2(mRect.xMax, mRect.yMax));
            }
            else if (mFlip == Flip.Both)
            {
                uvs.Add(new Vector2(mRect.xMax, mRect.yMin));
                uvs.Add(new Vector2(mRect.xMax, mRect.yMax));
                uvs.Add(new Vector2(mRect.xMin, mRect.yMax));
                uvs.Add(new Vector2(mRect.xMin, mRect.yMin));
            }
            else
            {
                uvs.Add(new Vector2(mRect.xMin, mRect.yMin));
                uvs.Add(new Vector2(mRect.xMin, mRect.yMax));
                uvs.Add(new Vector2(mRect.xMax, mRect.yMax));
                uvs.Add(new Vector2(mRect.xMax, mRect.yMin));
            }

            cols.Add(col);
            cols.Add(col);
            cols.Add(col);
            cols.Add(col);
        }
        else
        {
            if (mTexture == null)
                return;
            Color colF = color;
            colF.a = finalAlpha;
            Color32 col = colF;

            Vector4 v = drawingDimensions;

            verts.Add(new Vector3(v.x, v.y));
            verts.Add(new Vector3(v.x, v.w));
            verts.Add(new Vector3(v.z, v.w));
            verts.Add(new Vector3(v.z, v.y));


            uvs.Add(UVRect0);
            uvs.Add(UVRect1);
            uvs.Add(UVRect2);
            uvs.Add(UVRect3);

            cols.Add(col);
            cols.Add(col);
            cols.Add(col);
            cols.Add(col);
        }
    }

    public void SetTexture(string path)
    {
        if (texPath != path)
        {
            mtexType = GetTextureListType(path);
            UnLoadTexture();
            texPath = path;
            LoadTexture();
            ResetShader();
        }
    }
    

    public void SetRuntimeTexture(Texture tex, bool autoDestroy = true)
    {
        if (mTexture != tex)
        {
            if (texPath == "" && mTexture != null && autoDestroy)
            {
                UnityEngine.Object.Destroy(mTexture);
            }
            texPath = "";
            mtexType = normalTex;
            mainTexture = tex;
            ResetShader();
            if (shaderName == "Custom/UI/Mask")
            {
                if (mAlphaTexture == null)
                    mAlphaTexture = XResourceLoaderMgr.singleton.GetSharedResource<Texture>(maskTexPath, ".png");
            }
        }
    }
    public void FillMat(Material mat)
    {
        if (mat == null)
            return;
        mat.SetTexture(UIDrawCall.MainTexShaderID, mTexture);
        switch (mtexType)
        {
            case 0://Horizontally2
            case 2://Vertically2
                {
                    mat.SetTexture(UIDrawCall.MainTex1ShaderID, mTexture1);
                    if (mtexType == horizontally2)
                    {
                        mat.SetVector(UIDrawCall.UVScaleShaderID, UVScaleHorizontally2);
                        mat.SetVector(UIDrawCall.UVRangeShaderID, UVRangeHorizontally2);
                    }
                    else
                    {
                        mat.SetVector(UIDrawCall.UVScaleShaderID, UVScaleVertically2);
                        mat.SetVector(UIDrawCall.UVRangeShaderID, UVRangeVertically2);
                    }
                }
                break;
            case 1://Horizontally4
            case 3://Vertically4
                {
                    if (mtexType == horizontally4)
                    {
                        mat.SetVector(UIDrawCall.UVScaleShaderID, UVScaleHorizontally4);
                        mat.SetVector(UIDrawCall.UVRangeShaderID, UVRangeHorizontally4);
                    }
                    else
                    {
                        mat.SetVector(UIDrawCall.UVScaleShaderID, UVScaleVertically4);
                        mat.SetVector(UIDrawCall.UVRangeShaderID, UVRangeVertically4);
                    }
                }
                break;
            case 4://normalTex
                if (mat.HasProperty("_Mask"))
                {
                    mat.SetTexture("_Mask", mAlphaTexture);
                }
                break;
        }
    }

#region TexList
    public static byte GetTextureListType(string path)
    {
        if (path.EndsWith("h2Split"))
            return horizontally2;
        if (path.EndsWith("h4Split"))
            return horizontally4;
        if (path.EndsWith("v2Split"))
            return vertically2;
        if (path.EndsWith("v4Split"))
            return vertically4;
        return normalTex;
    }
    private void GetRealSize(out int width, out int height)
    {
        if (mtexType == horizontally2)
        {
            width = (mTexture != null) ? mTexture.width : mWidth;
            width *= 2;
            height = (mTexture != null) ? mTexture.height : mHeight;
        }
        else if (mtexType == vertically2)
        {
            width = (mTexture != null) ? mTexture.width : mWidth;
            height = (mTexture != null) ? mTexture.height : mHeight;
            height *= 2;
        }
        else
        {
            width = (mTexture != null) ? mTexture.width : mWidth;
            height = (mTexture != null) ? mTexture.height : mHeight;
        }
    }  

#endregion

  
}

