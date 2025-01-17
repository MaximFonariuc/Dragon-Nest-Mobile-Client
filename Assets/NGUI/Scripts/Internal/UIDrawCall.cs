//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

//#define SHOW_HIDDEN_OBJECTS

using UnityEngine;
using System.Collections.Generic;
using XUtliPoolLib;
public interface IDrawCallMat
{
    void FillMat(Material mat);
}
/// <summary>
/// This is an internally-created script used by the UI system. You shouldn't be attaching it manually.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Internal/Draw Call")]
public class UIDrawCall : MonoBehaviour
{
	static BetterList<UIDrawCall> mActiveList = new BetterList<UIDrawCall>();
	static BetterList<UIDrawCall> mInactiveList = new BetterList<UIDrawCall>();


    static Dictionary<Shader, Queue<Material>>  matCache = new Dictionary<Shader, Queue<Material>>();

   
    static string[] uiShaderName = new string[] {
        "Custom/UI/SeparateColorAlpha",
        "Custom/UI/Text",
        "Custom/UI/Additive",
        "Custom/UI/ColorTexture",
        "Custom/UI/RGBA",
        "Custom/UI/Mask",
        "Custom/UI/RenderTexture",
        "Custom/UI/WhiteAnim",
        "Custom/UI/TextureList2",
        "Custom/UI/TextureList4",
        "Custom/UI/Merge"};

    static string[] uiShaderABName = new string[] {
        "SeparateColorAlpha",
        "Text",
        "Additive",
        "ColorTexture",
        "RGBA",
        "Mask",
        "RenderTexture",
        "WhiteAnim",
        "TextureList2",
        "TextureList4",
        "Merge"};
    static Shader[] uiShader = new Shader[uiShaderName.Length];

	/// <summary>
	/// List of active draw calls.
	/// </summary>

	static public BetterList<UIDrawCall> activeList { get { return mActiveList; } }

	/// <summary>
	/// List of inactive draw calls. Only used at run-time in order to avoid object creation/destruction.
	/// </summary>

	static public BetterList<UIDrawCall> inactiveList { get { return mInactiveList; } }

	public enum Clipping : int
	{
		None = 0,
		SoftClip = 3,				// Alpha-based clipping with a softened edge
		ConstrainButDontClip = 4,	// No actual clipping, but does have an area
	}

	[HideInInspector][System.NonSerialized] public int depthStart = int.MaxValue;
	[HideInInspector][System.NonSerialized] public int depthEnd = int.MinValue;
	[HideInInspector][System.NonSerialized] public UIPanel manager;
	[HideInInspector][System.NonSerialized] public UIPanel panel;
	[HideInInspector][System.NonSerialized] public bool alwaysOnScreen = false;
	[HideInInspector][System.NonSerialized] public FastListV3 verts = new FastListV3();
	[HideInInspector][System.NonSerialized] public FastListV3 norms = new FastListV3();
	[HideInInspector][System.NonSerialized] public BetterList<Vector4> tans = new BetterList<Vector4>();
	[HideInInspector][System.NonSerialized] public FastListV2 uvs = new FastListV2();
	[HideInInspector][System.NonSerialized] public FastListColor32 cols = new FastListColor32();

	public bool useMerge = false;

	Material		mMaterial;		// Material used by this screen
	Texture			mTexture;		// Main texture used by the material
    IDrawCallMat    mdcMat;       // Main texture used by the material
    public List<Texture>   mTextureList = null;
	public List<Texture>   mAlphaTextureList = null;
	Shader			mShader;		// Shader used by the dynamically created material
	int				mClipCount = 0;	// Number of times the draw call's content is getting clipped
	Transform		mTrans;			// Cached transform
	Mesh			mMesh;			// First generated mesh
	MeshFilter		mFilter;		// Mesh filter for this draw call
	MeshRenderer	mRenderer;		// Mesh renderer for this screen
	Material		mDynamicMat;	// Instantiated material
	int[]			mIndices;		// Cached indices

	bool mRebuildMat = true;
	int mRenderQueue = 3500;
	int mTriangles = 0;
	/// <summary>
	/// Whether the draw call has changed recently.
	/// </summary>

	[System.NonSerialized]
	public bool isDirty = false;

    public static int MainTexShaderID = Shader.PropertyToID("_MainTex");
    public static int MaskTexShaderID = Shader.PropertyToID("_Mask");
    public static int MainTex1ShaderID = Shader.PropertyToID("_MainTex1");
    public static int ClipRange0ShaderID = Shader.PropertyToID("_ClipRange0");
    public static int ClipArgs0ShaderID = Shader.PropertyToID("_ClipArgs0");
    public static int UVScaleShaderID = Shader.PropertyToID("_UVScale");
    public static int UVRangeShaderID = Shader.PropertyToID("_UVRange");

    /// <summary>
    /// Render queue used by the draw call.
    /// </summary>

    public int renderQueue
	{
		get
		{
			return mRenderQueue;
		}
		set
		{
			if (mRenderQueue != value)
			{
				mRenderQueue = value;
				if (mDynamicMat != null)
				{
					mDynamicMat.renderQueue = value;
#if UNITY_EDITOR
					if (mRenderer != null) mRenderer.enabled = isActive;
#endif
				}
			}
		}
	}

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
	/// <summary>
	/// Renderer's sorting order, to be used with Unity's 2D system.
	/// </summary>

	public int sortingOrder
	{
		get { return (mRenderer != null) ? mRenderer.sortingOrder : 0; }
		set { if (mRenderer != null && mRenderer.sortingOrder != value) mRenderer.sortingOrder = value; }
	}
#endif

	/// <summary>
	/// Final render queue used to draw the draw call's geometry.
	/// </summary>

	public int finalRenderQueue
	{
		get
		{
			return (mDynamicMat != null) ? mDynamicMat.renderQueue : mRenderQueue;
		}
	}

#if UNITY_EDITOR

	/// <summary>
	/// Whether the draw call is currently active.
	/// </summary>

	public bool isActive
	{
		get
		{
			return mActive;
		}
		set
		{
			if (mActive != value)
			{
				mActive = value;

				if (mRenderer != null)
				{
					mRenderer.enabled = value;
					NGUITools.SetDirty(gameObject);
				}
			}
		}
	}
	bool mActive = true;
#endif

	/// <summary>
	/// Transform is cached for speed and efficiency.
	/// </summary>

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	/// <summary>
	/// Material used by this screen.
	/// </summary>

	public Material baseMaterial
	{
		get
		{
			return mMaterial;
		}
		set
		{
			if (mMaterial != value)
			{
				mMaterial = value;
				mRebuildMat = true;
			}
		}
	}

	/// <summary>
	/// Dynamically created material used by the draw call to actually draw the geometry.
	/// </summary>

	public Material dynamicMaterial { get { return mDynamicMat; } }

	/// <summary>
	/// Texture used by the material.
	/// </summary>

	public Texture mainTexture
	{
		get
		{
			return mTexture;
		}
		set
		{
			mTexture = value;
			if (mDynamicMat != null) mDynamicMat.mainTexture = value;
		}
	}
    public IDrawCallMat DCMat
    {
        set
        {
            mdcMat = value;
        }
    }
    //public Texture alphaTexture
    //{
    //    get
    //    {
    //        return mAlphaTexture;
    //    }
    //    set
    //    {
    //        mAlphaTexture = value;
    //    }
    //}
	/// <summary>
	/// Shader used by the material.
	/// </summary>

	public Shader shader
	{
		get
		{
			return mShader;
		}
		set
		{
			if (mShader != value)
			{
				mShader = value;
				mRebuildMat = true;
			}
		}
	}

	/// <summary>
	/// The number of triangles in this draw call.
	/// </summary>

	public int triangles { get { return (mMesh != null) ? mTriangles : 0; } }

	/// <summary>
	/// Whether the draw call is currently using a clipped shader.
	/// </summary>

	public bool isClipped { get { return mClipCount != 0; } }

	/// <summary>
	/// Create an appropriate material for the draw call.
	/// </summary>

    
	void CreateMaterial ()
	{
        string shaderName = null;
        Shader shader = null;
        if (mShader != null)
        {
            shader = mShader;
            shaderName = mShader.name;
        }
        else if (mMaterial != null)
        {
            shader = mMaterial.shader;
            shaderName = shader.name;
        }
        else
        {
            shaderName = "Custom/UI/SeparateColorAlpha";
        }

        // Figure out the normal shader's name
        if (shaderName.Contains("GUI/Text Shader"))
        {
            shaderName = "Custom/UI/Text";
            shader = null;
        }           

        // Try to find the new shader       
        mClipCount = panel.clipCount;

        if (shader == null)
            shader = GetDefaultShader(shaderName);
        //if (shader == null)
        //{
        //    shader = Shader.Find(shaderName);
        //}
        if (shader == null)
        {
            XDebug.singleton.AddErrorLog2("{0} shader doesn't have a clipped shader version for {1} clip regions", shaderName, mClipCount);
            shader = uiShader[0];
        }

        if (mMaterial != null)
        {
            if (mDynamicMat != null)
            {                
                mDynamicMat.shader = shader;
            }
            else
            {
                mDynamicMat = GetMaterial(shader);
            }
            if (mDynamicMat.HasProperty("_Mask"))
                mDynamicMat.SetTexture("_Mask", null);
            mDynamicMat.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
            mDynamicMat.CopyPropertiesFromMaterial(mMaterial);

        }
        else
		{
            if (mDynamicMat != null)
            {                
                mDynamicMat.shader = shader;
            }
            else
            {
                mDynamicMat = GetMaterial(shader);
            }
            if (mDynamicMat.HasProperty("_Mask"))
                mDynamicMat.SetTexture("_Mask", null);
            mDynamicMat.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
        }
        
        if (useMerge)
        {
            for (int i = 0; i < mAlphaTextureList.Count; ++i)
            {
                mDynamicMat.SetTexture("_Mask" + i, mAlphaTextureList[i]);
            }
        }
        else
        {
            if (mdcMat != null)
                mdcMat.FillMat(mDynamicMat);

            //if (mtxLst != null && mtxLst.TryGetType() == UITexture.TextureType.List)
            //{
            //    mtxLst.SetTexInterface(mDynamicMat);
            //}
            //else if (mAlphaTexture != null && mDynamicMat.HasProperty("_Mask"))
            //{
            //    mDynamicMat.SetTexture("_Mask", mAlphaTexture);
            //}
        }

        // If there is a valid shader, assign it to the custom material
    }

    static Vector4 defaultClipRange = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
    static Vector4 defaultClipAngle = new Vector4(1000.0f, 1000.0f, 0.0f, 1.0f);
    /// <summary>
    /// Rebuild the draw call's material.
    /// </summary>

    Material RebuildMaterial ()
	{
		// Destroy the old material
		//NGUITools.DestroyImmediate(mDynamicMat);

		// Create a new material
		CreateMaterial();
		mDynamicMat.renderQueue = mRenderQueue;

        // Assign the main texture
        if (useMerge)
        {
            for (int i = 0; i < mTextureList.Count; ++i)
            {
                mDynamicMat.SetTexture("_MainTex" + i, mTextureList[i]);
            }
        }
        else if (mdcMat == null)
        {
            if (mTexture != null)
            {
                mDynamicMat.mainTexture = mTexture;
            }
            else
            {
                mDynamicMat.mainTexture = null;
            }
        }
        // Update the renderer
        if (mRenderer != null) mRenderer.sharedMaterial = mDynamicMat;

        return mDynamicMat;
	}

	/// <summary>
	/// Update the renderer's materials.
	/// </summary>

	void UpdateMaterials ()
	{
        if (panel == null)
            return;
		// If clipping should be used, we need to find a replacement shader
		if (mRebuildMat || mDynamicMat == null || mClipCount != panel.clipCount)
		{
			RebuildMaterial();
			mRebuildMat = false;
		}
		else if (mRenderer.sharedMaterial != mDynamicMat)
		{
#if UNITY_EDITOR
			Debug.LogError("Hmm... This point got hit!");
#endif
			mRenderer.sharedMaterials = new Material[] { mDynamicMat };
		}
	}

	/// <summary>
	/// Set the draw call's geometry.
	/// </summary>

	public void UpdateGeometry ()
	{
		int count = verts.size;

		// Safety check to ensure we get valid values
		if (count > 0 && (count == uvs.size && count == cols.size) && (count % 4) == 0)
		{
			// Cache all components
			if (mFilter == null) mFilter = gameObject.GetComponent<MeshFilter>();
            if (mFilter == null) mFilter = gameObject.AddComponent<MeshFilter>();

            if (verts.size < 65000)
            {
                // Populate the index buffer
                int indexCount = (verts.Count >> 1) * 3;
				bool setIndices = (mIndices == null || mIndices.Length != indexCount);

				// Create the mesh
				if (mMesh == null)
				{
					mMesh = new Mesh();

                    mMesh.hideFlags = HideFlags.DontSave;
					mMesh.name = (mMaterial != null) ? mMaterial.name : "Mesh";
#if !UNITY_3_5
					mMesh.MarkDynamic();
#endif
					setIndices = true;
				}
#if !UNITY_FLASH
				// If the buffer length doesn't match, we need to trim all buffers
				bool trim = (uvs.buffer.Length != verts.buffer.Length) ||
					(cols.buffer.Length != verts.buffer.Length) ||
					(norms.buffer != null && norms.buffer.Length != verts.buffer.Length) ||
					(tans.buffer != null && tans.buffer.Length != verts.buffer.Length);

				// Non-automatic render queues rely on Z position, so it's a good idea to trim everything
				if (!trim && panel.renderQueue != UIPanel.RenderQueue.Automatic)
					trim = (mMesh == null || mMesh.vertexCount != verts.buffer.Length);

				// NOTE: Apparently there is a bug with Adreno devices:
				// http://www.tasharen.com/forum/index.php?topic=8415.0
#if !UNITY_4_3 || !UNITY_ANDROID
				// If the number of vertices in the buffer is less than half of the full buffer, trim it
				if (!trim && (verts.size << 1) < verts.buffer.Length) trim = true;
#endif
				mTriangles = (verts.size >> 1);
				if (trim || verts.buffer.Length > 65000)
				{
					if (trim || mMesh.vertexCount != verts.size)
					{
						mMesh.Clear();
						setIndices = true;
					}

					mMesh.vertices = verts.ToArray();
					mMesh.uv = uvs.ToArray();
					mMesh.colors32 = cols.ToArray();

					if (norms != null) mMesh.normals = norms.ToArray();
					{
						if (tans != null) mMesh.tangents = tans.ToArray();
					}

					//if (uv2s != null) mMesh.uv2 = uv2s.ToArray();
					/*
					if(useMerge)
					{
						Vector4[] buff = tans.ToArray();
						if(buff.Length != mMesh.vertices.Length)
						{
							Vector4[] buff4 = new Vector4[mMesh.vertices.Length];
							for(int i = 0 ; i < Mathf.Min (buff.Length,buff4.Length);++i)
							{
								buff4[i] = buff[i];
							}

							mMesh.tangents = buff4;
						}
					}else
					*/


				}
				else
				{
					if (mMesh.vertexCount != verts.buffer.Length)
					{
						mMesh.Clear();
						setIndices = true;
					}

					mMesh.vertices = verts.buffer;
					mMesh.uv = uvs.buffer;
					mMesh.colors32 = cols.buffer;

					if (norms != null) mMesh.normals = norms.buffer;
					if (tans != null) mMesh.tangents = tans.buffer;

					

				}
#else
				mTriangles = (verts.size >> 1);

				if (mMesh.vertexCount != verts.size)
				{
					mMesh.Clear();
					setIndices = true;
				}

				mMesh.vertices = verts.ToArray();
				mMesh.uv = uvs.ToArray();
				mMesh.colors32 = cols.ToArray();

				if (norms != null) mMesh.normals = norms.ToArray();
				if (tans != null) mMesh.tangents = tans.ToArray();
#endif
                if (setIndices)
                {
                    mIndices = GenerateCachedIndexBuffer(verts.Count, indexCount);                    
                    mMesh.triangles = mIndices;

                }

#if !UNITY_FLASH
				if (trim || !alwaysOnScreen)
#endif
					mMesh.RecalculateBounds();

				mFilter.mesh = mMesh;
			}
			else
			{
				mTriangles = 0;
				if (mFilter.mesh != null) mFilter.mesh.Clear();
				Debug.LogError("Too many vertices on one panel: " + verts.size);
			}

			if (mRenderer == null) mRenderer = gameObject.GetComponent<MeshRenderer>();

			if (mRenderer == null)
			{
				mRenderer = gameObject.AddComponent<MeshRenderer>();
#if UNITY_EDITOR
				mRenderer.enabled = isActive;
#endif
                mRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                mRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                mRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mRenderer.receiveShadows = false;
                mRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            }
           

            UpdateMaterials();
		}
		else
		{
			if (mFilter.mesh != null) mFilter.mesh.Clear();
			Debug.LogError("UIWidgets must fill the buffer with 4 vertices per quad. Found " + count);
		}

		verts.Clear();
		uvs.Clear();
		cols.Clear();
		norms.Clear();
		tans.Clear();
	}

	const int maxIndexBufferCache = 20;

#if UNITY_FLASH
	List<int[]> mCache = new List<int[]>(maxIndexBufferCache);
#else
	static List<int[]> mCache = new List<int[]>(maxIndexBufferCache);
#endif

	/// <summary>
	/// Generates a new index buffer for the specified number of vertices (or reuses an existing one).
	/// </summary>

	int[] GenerateCachedIndexBuffer (int vertexCount, int indexCount)
	{
		for (int i = 0, imax = mCache.Count; i < imax; ++i)
		{
			int[] ids = mCache[i];
			if (ids != null && ids.Length == indexCount)
				return ids;
		}

		int[] rv = new int[indexCount];
		int index = 0;

		for (int i = 0; i < vertexCount; i += 4)
		{
			rv[index++] = i;
			rv[index++] = i + 1;
			rv[index++] = i + 2;

			rv[index++] = i + 2;
			rv[index++] = i + 3;
			rv[index++] = i;
		}

		if (mCache.Count > maxIndexBufferCache) mCache.RemoveAt(0);
		mCache.Add(rv);
		return rv;
	}

	/// <summary>
	/// This function is called when it's clear that the object will be rendered.
	/// We want to set the shader used by the material, creating a copy of the material in the process.
	/// We also want to update the material's properties before it's actually used.
	/// </summary>

	void OnWillRenderObject ()
	{
		UpdateMaterials();
        if (mClipCount == 0)
        {
            mDynamicMat.SetVector("_ClipRange0", defaultClipRange);
            mDynamicMat.SetVector("_ClipArgs0", defaultClipAngle);
            return;
        }
        else
        {
            SetClipping(panel.ClipRange, panel.clipSoftness);
        }        
    }

	/// <summary>
	/// Set the shader clipping parameters.
	/// </summary>
    void SetClipping(Vector4 cr, Vector2 soft)
    {
        //angle *= -Mathf.Deg2Rad;

        Vector2 sharpness = new Vector2(1000.0f, 1000.0f);
        if (soft.x > 0f) sharpness.x = cr.z / soft.x;
        if (soft.y > 0f) sharpness.y = cr.w / soft.y;

        mDynamicMat.SetVector("_ClipRange0", new Vector4(-cr.x / cr.z, -cr.y / cr.w, 1f / cr.z, 1f / cr.w));
        mDynamicMat.SetVector("_ClipArgs0", new Vector4(sharpness.x, sharpness.y, 0, 1));
    }
    /// <summary>
    /// The material should be rebuilt when the draw call is enabled.
    /// </summary>

    void OnEnable () { mRebuildMat = true; }

	/// <summary>
	/// Clear all references.
	/// </summary>

	void OnDisable ()
	{
		depthStart = int.MaxValue;
		depthEnd = int.MinValue;
		panel = null;
		manager = null;
        mMaterial = null;
        mTexture = null;
        if (mDynamicMat != null)
        {
            Queue<Material> queue = GetMaterialQueue(mDynamicMat.shader);
            if(queue!=null)
            {
                queue.Enqueue(mDynamicMat);
            }            
        }
        
        //NGUITools.DestroyImmediate(mDynamicMat);
        mDynamicMat = null;
	}

	/// <summary>
	/// Cleanup.
	/// </summary>

	void OnDestroy ()
	{
        Clear();
        ClearReference();
        if (mMaterial != null)
        {
            mMaterial = null;
        }
		NGUITools.DestroyImmediate(mMesh);
	}

    public void ClearReference()
    {
        mTexture = null;
        if (mDynamicMat != null)
        {
            Queue<Material> queue = GetMaterialQueue(mDynamicMat.shader);
            if (queue != null)
            {
                queue.Enqueue(mDynamicMat);
            }
                
            mDynamicMat = null;
            mRebuildMat = true;
        }
    }

    public void Clear()
    {
        verts.Clear();
        norms.Clear();
        tans.Clear();
        uvs.Clear();
        cols.Clear();
    }

    private static Queue<Material> GetMaterialQueue(Shader shader)
    {
        Queue<Material> queue = null;
        if(!matCache.TryGetValue(shader,out queue))
        {
            queue = new Queue<Material>();
            matCache.Add(shader, queue);
        }
        return queue;        
    }
    private static void ClearMaterialQueue()
    {
        Dictionary<Shader, Queue<Material>>.Enumerator it = matCache.GetEnumerator();
        while (it.MoveNext())
        {
            Queue<Material> queue = it.Current.Value;
            if (queue != null)
            {
                while (queue.Count > 0)
                {
                    Material mat = queue.Dequeue();
                    NGUITools.DestroyImmediate(mat);
                }
            }
        }
        it.Dispose();
        matCache.Clear();
    }
    private Material GetMaterial(Shader shader)
    {
        Material mat = null;
        Queue <Material> queue = GetMaterialQueue(shader);
        if (queue != null)
        {
            if (queue.Count > 0)
            {
                mat = queue.Dequeue();
                if (mat != null&&mat.HasProperty("_MainTex"))
                {
                    mat.mainTextureOffset = Vector2.zero;
                    mat.mainTextureScale = Vector2.one;
                    mat.shader = shader;
                    return mat;
                }
            }
        }
        mat = new Material(shader);
        mat.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
        return mat;
    }
	/// <summary>
	/// Return an existing draw call.
	/// </summary>

    static public UIDrawCall Create(UIPanel panel, Material mat, Texture tex, IDrawCallMat dcMat, Shader shader)
	{
#if UNITY_EDITOR
		string name = null;
		if (tex != null) name = tex.name;
		else if (shader != null) name = shader.name;
		else if (mat != null) name = mat.name;
        return Create(name, panel, mat, tex, dcMat, shader);
#else
		return Create(null, panel, mat, tex, dcMat, shader);
#endif
    }

    public void FillMergeIndex(int index,int count)
	{
		Vector2 offset = new Vector2 (index*2.0f,index*2.0f);
		for (int i = uvs.Count - count; i < uvs.Count; ++i) 
		{
			uvs[i] += offset;
		}
    }

    static public UIDrawCall CreateMerge(UIPanel pan, Material mat, Shader shader)
    {
        UIDrawCall dc = Create(pan.name);
        dc.gameObject.layer = pan.cachedGameObject.layer;
        dc.baseMaterial = mat;
        dc.mainTexture = null;
		dc.shader = shader;
		dc.renderQueue = pan.startingRenderQueue;
		#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
		dc.sortingOrder = pan.sortingOrder;
		#endif
		dc.manager = pan;
		dc.useMerge = true;
		return dc;
	}

	/// <summary>
	/// Create a new draw call, reusing an old one if possible.
	/// </summary>

    static UIDrawCall Create(string name, UIPanel pan, Material mat, Texture tex, IDrawCallMat dcMat, Shader shader)
	{
		UIDrawCall dc = Create(name);
		dc.gameObject.layer = pan.cachedGameObject.layer;
		dc.baseMaterial = mat;
		dc.mainTexture = tex;
        dc.DCMat = dcMat;
		dc.shader = shader;
		dc.renderQueue = pan.startingRenderQueue;
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
		dc.sortingOrder = pan.sortingOrder;
#endif
		dc.manager = pan;
		dc.useMerge = false;
		return dc;
	}

	/// <summary>
	/// Create a new draw call, reusing an old one if possible.
	/// </summary>

	static UIDrawCall Create (string name)
    {       
        
#if SHOW_HIDDEN_OBJECTS && UNITY_EDITOR
		name = (name != null) ? "_UIDrawCall [" + name + "]" : "DrawCall";
#endif
        if (mInactiveList.size > 0)
		{
			UIDrawCall dc = mInactiveList.Pop();
			mActiveList.Add(dc);
			if (name != null) dc.name = name;
            NGUITools.SetActive(dc.gameObject, true, true,false);
			return dc;
		}

#if UNITY_EDITOR
		// If we're in the editor, create the game object with hide flags set right away
		GameObject go = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags(name,
 #if SHOW_HIDDEN_OBJECTS
			HideFlags.DontSave, typeof(UIDrawCall));
 #else
			HideFlags.HideAndDontSave, typeof(UIDrawCall));
 #endif
		UIDrawCall newDC = go.GetComponent<UIDrawCall>();
#else
		GameObject go = new GameObject(name);
		DontDestroyOnLoad(go);
		UIDrawCall newDC = go.AddComponent<UIDrawCall>();
#endif
		// Create the draw call
		mActiveList.Add(newDC);
		return newDC;
	}
    //static Shader GetShader(int index)
    //{
    //    if(uiShader[index]==null)
    //    {
    //        uiShader[index] = Shader.Find(uiShaderName[index]);
    //    }
    //    return uiShader[index];
    //}
    public static Shader GetDefaultShader(string name)
    {
        for (int i = 0; i < uiShaderName.Length; ++i)
        {
            string shaderName = uiShaderName[i];
            if (shaderName == name)
            {
                if (uiShader[i] == null)
                {
                    //uiShader[i] = Shader.Find(shaderName);
                    uiShader[i] = XUtliPoolLib.ShaderManager.singleton.FindShader(uiShaderABName[i], shaderName);
                    return uiShader[i];
                }
            }
        }
        return Shader.Find(name);
    }
    
	/// <summary>
	/// Clear all draw calls.
	/// </summary>

	static public void ClearAll ()
	{
		bool playing = Application.isPlaying;

		for (int i = mActiveList.size; i > 0; )
		{
			UIDrawCall dc = mActiveList[--i];

			if (dc)
			{
				if (playing) NGUITools.SetActive(dc.gameObject, false);
				else NGUITools.DestroyImmediate(dc.gameObject);
			}
		}
		mActiveList.Clear();
	}

	/// <summary>
	/// Immediately destroy all draw calls.
	/// </summary>

	static public void ReleaseAll ()
	{
		ClearAll();
		ReleaseInactive();
        ClearMaterialQueue();
	}

	/// <summary>
	/// Immediately destroy all inactive draw calls (draw calls that have been recycled and are waiting to be re-used).
	/// </summary>

	static public void ReleaseInactive()
	{
		for (int i = mInactiveList.size; i > 0; )
		{
			UIDrawCall dc = mInactiveList[--i];
			if (dc)
            {
                dc.Clear();
                NGUITools.DestroyImmediate(dc.gameObject);
            }                
		}
		mInactiveList.Clear();
        ClearMaterialQueue();
    }

	/// <summary>
	/// Count all draw calls managed by the specified panel.
	/// </summary>

	static public int Count (UIPanel panel)
	{
		int count = 0;
		for (int i = 0; i < mActiveList.size; ++i)
			if (mActiveList[i].manager == panel) ++count;
		return count;
	}

	/// <summary>
	/// Destroy the specified draw call.
	/// </summary>

	static public void Destroy (UIDrawCall dc)
	{
		if (dc)
		{
            
			if (Application.isPlaying)
			{
				if (mActiveList.Remove(dc))
				{
					NGUITools.SetActive(dc.gameObject, false);                    
					mInactiveList.Add(dc);
				}
			}
			else
			{
				mActiveList.Remove(dc);
				NGUITools.DestroyImmediate(dc.gameObject);
			}
		}
	}
}
