#define FXPRO_EFFECT
//#define BLOOMPRO_EFFECT
//#define GLOWPRO_EFFECT
//#define DOFPRO_EFFECT

#if FXPRO_EFFECT
#define GLOWPRO_EFFECT
#define BLOOMPRO_EFFECT
//#define DOFPRO_EFFECT
#define MOBILEDOFPRO_EFFECT
#endif

using UnityEngine;

using System.Collections.Generic;

#if FXPRO_EFFECT
using FxProNS;
#elif BLOOMPRO_EFFECT
using BloomProNS;
#elif GLOWPRO_EFFECT
using GlowProNS;
#elif DOFPRO_EFFECT
using DOFProNS;
#elif MOBILEDOFPRO_EFFECT
using MobileDOFProNS;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
#if FXPRO_EFFECT
[AddComponentMenu("Image Effects/FxPro™")]
public class FxPro : MonoBehaviour, XUtliPoolLib.IFxPro
#elif BLOOMPRO_EFFECT
[AddComponentMenu( "Image Effects/BloomPro™" )]
public class BloomPro : MonoBehaviour
#elif GLOWPRO_EFFECT
[AddComponentMenu( "Image Effects/GlowPro™" )]
public class GlowPro : MonoBehaviour
#elif DOFPRO_EFFECT
[AddComponentMenu( "Image Effects/DOF Pro™" )]
public class DOFPro : MonoBehaviour	
#endif
{
    public EffectsQuality Quality = EffectsQuality.Normal;

	private static Material _mat;

    public static Material Mat
    {
        get
        {
            if (null == _mat)
            {
                Shader shader = XUtliPoolLib.ShaderManager.singleton.FindShader("FxPro", "Hidden/FxPro");
                _mat = new Material(shader)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
            }
            //_mat = new Material( Shader.Find("Hidden/FxPro") )  {
            //        hideFlags = HideFlags.HideAndDontSave
            //    };

            return _mat;
        }
    }

    private static Material _tapMat;

    private static Material TapMat
    {
        get
        {
            if ( null == _tapMat )
            {
                Shader shader = XUtliPoolLib.ShaderManager.singleton.FindShader("FxProTap", "Hidden/FxProTap");
                _tapMat = new Material(shader)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
            }

            return _tapMat;
        }
    }

    //Bloom
#if BLOOMPRO_EFFECT
    public bool BloomEnabled = true;
    public BloomHelperParams BloomParams = new BloomHelperParams();
    public bool VisualizeBloom = false;
#endif

	public Texture2D LensDirtTexture = null;
	
	[Range(0f, 2f)]
	public float LensDirtIntensity = 1f;
	

	public bool ChromaticAberration = false;
    public bool ChromaticAberrationPrecise = false;

    [Range(1f, 2.5f)]
    public float ChromaticAberrationOffset = 1f;

    [Range( 0f, 1f )]
    public float SCurveIntensity = .5f;


    public bool LensCurvatureEnabled = false;
    [Range( 1f, 2f )]
    public float LensCurvaturePower = 1.1f;
    public bool LensCurvaturePrecise = false;

    [Range( 0f, 1f )]
    public float FilmGrainIntensity = .15f;
    [Range( 1f, 10f )]
    public float FilmGrainTiling = 4f;

    [Range( 0f, 1f )]
    public float VignettingIntensity = .5f;

    //Depth of Field
#if DOFPRO_EFFECT
    public bool DOFEnabled = false;
    public bool BlurCOCTexture = true;
    public DOFHelperParams DOFParams = new DOFHelperParams();

    public bool VisualizeCOC = false;
#endif

#if MOBILEDOFPRO_EFFECT
    public bool MobileDOFEnabled = false;
    [Range(0f, 1f)]
    public float DOF_FadeInFadeOut = 1;

    //public bool MobileBlurCOCTexture = true;
    public MobileDOFHelperParams MobileDOFParams = new MobileDOFHelperParams();

    //  public bool MobileVisualizeCOC = false;
#endif

    private const bool VisualizeLensCurvature = false;
    //private Texture2D _gridTexture;

    private List<Texture2D> _filmGrainTextures;
    
    //Color Grading
    public bool ColorEffectsEnabled = true;

    public Texture2D colorGradingLut;
    //[Range(0f, 1f)]
    //public float Adapted_lum = 0.5f;
#if ((UNITY_EDITOR) || (UNITY_STANDALONE) || (UNITY_IOS))
    public void Start() 
	{

	
		if (!SystemInfo.supportsImageEffects)
		{
			Debug.LogError("Image effects are not supported on this platform.");
			enabled = false;
			return;
		}

        if ( VisualizeLensCurvature )
        {
            //_gridTexture = Resources.Load( "lens_curvature_grid" ) as Texture2D;

            //if ( null == _gridTexture )
            //    Debug.LogError( "null == _gridTexture" );

//            Debug.Log( "loaded grid tex" );
        }

        _filmGrainTextures = new List<Texture2D>();

        for ( int i = 1; i <= 4; i++ )
        {
            var resourceName = "filmgrain_0" + i;

            var curTex = Resources.Load( resourceName ) as Texture2D;

            if ( null == curTex )
            {
                //Debug.LogError( "Unable to load grain texture '" + resourceName + "'" );
                continue;
            }

            _filmGrainTextures.Add( curTex );
        }
	}

	public void Init(bool searchForNonDepthmapAlphaObjects) {
        Mat.SetFloat( "_DirtIntensity", Mathf.Exp( LensDirtIntensity ) - 1f );

        if (null == LensDirtTexture || LensDirtIntensity <= 0f) {
            Mat.DisableKeyword( "LENS_DIRT_ON" );
            Mat.EnableKeyword( "LENS_DIRT_OFF" );
        } else {
            Mat.SetTexture( "_LensDirtTex", LensDirtTexture );
            Mat.EnableKeyword( "LENS_DIRT_ON" );
            Mat.DisableKeyword( "LENS_DIRT_OFF" );
        }

#if BLOOMPRO_EFFECT
        if ( ChromaticAberration )
        {
            Mat.EnableKeyword( "CHROMATIC_ABERRATION_ON" );
            Mat.DisableKeyword( "CHROMATIC_ABERRATION_OFF" );
        } else
        {
            Mat.EnableKeyword( "CHROMATIC_ABERRATION_OFF" );
            Mat.DisableKeyword( "CHROMATIC_ABERRATION_ON" );
        }
#endif

        if (GetComponent<Camera>().allowHDR) {
            Shader.EnableKeyword( "FXPRO_HDR_ON" );
        } else {
            Shader.EnableKeyword( "FXPRO_HDR_OFF" );
        }

        Mat.SetFloat( "_SCurveIntensity", SCurveIntensity );

        //
        //Depth of Field
#if DOFPRO_EFFECT
        if (DOFEnabled) {

            if (null == DOFParams.EffectCamera) {
                DOFParams.EffectCamera = GetComponent<Camera>();
            }

            //Validating DOF parameters
            DOFParams.DepthCompression = Mathf.Clamp( DOFParams.DepthCompression, 2f, 8f );

            DOFHelper.Instance.SetParams( DOFParams );
			DOFHelper.Instance.Init( searchForNonDepthmapAlphaObjects );
			
			Mat.DisableKeyword( "DOF_DISABLED" );
            Mat.EnableKeyword( "DOF_ENABLED" );

            //Less blur when using fastest quality
            if (!DOFParams.DoubleIntensityBlur)
                DOFHelper.Instance.SetBlurRadius( (Quality == EffectsQuality.Fastest || Quality == EffectsQuality.Fast) ? 3 : 5 );
            else
                DOFHelper.Instance.SetBlurRadius( (Quality == EffectsQuality.Fastest || Quality == EffectsQuality.Fast) ? 5 : 10 );
        } else {
            Mat.EnableKeyword( "DOF_DISABLED" );
            Mat.DisableKeyword( "DOF_ENABLED" );
        }
#else
        //Mat.EnableKeyword( "DOF_DISABLED" );
        //Mat.DisableKeyword( "DOF_ENABLED" );
#endif

#if MOBILEDOFPRO_EFFECT

        if (MobileDOFEnabled)
        {

            if (null == MobileDOFParams.EffectCamera)
            {
                MobileDOFParams.EffectCamera = GetComponent<Camera>();
            }

            //Validating DOF parameters
            //MobileDOFParams.DepthCompression = Mathf.Clamp(MobileDOFParams.DepthCompression, 2f, 8f);

            MobileDOFHelper.Instance.SetParams(MobileDOFParams);
            MobileDOFHelper.Instance.Init(searchForNonDepthmapAlphaObjects);

            Mat.DisableKeyword("MOBILEDOF_DISABLED");
            Mat.EnableKeyword("MOBILEDOF_ENABLED");

            //Less blur when using fastest quality
            //if (!MobileDOFParams.DoubleIntensityBlur)
            MobileDOFHelper.Instance.SetBlurRadius((Quality == EffectsQuality.Fastest || Quality == EffectsQuality.Fast) ? 3 : 5);
            //else
            // MobileDOFHelper.Instance.SetBlurRadius((Quality == EffectsQuality.Fastest || Quality == EffectsQuality.Fast) ? 5 : 10);
        }
        else
        {

            Mat.EnableKeyword("MOBILEDOF_DISABLED");
            Mat.DisableKeyword("MOBILEDOF_ENABLED");
        }
#endif

        //
        //Bloom
#if BLOOMPRO_EFFECT
        if (BloomEnabled) {
            BloomParams.Quality = Quality;
            BloomHelper.Instance.SetParams(BloomParams);
            BloomHelper.Instance.Init();

            Mat.DisableKeyword("BLOOM_DISABLED");
            Mat.EnableKeyword("BLOOM_ENABLED");
        } else {
            Mat.EnableKeyword( "BLOOM_DISABLED" );
            Mat.DisableKeyword( "BLOOM_ENABLED" );
        }
#endif

        if ( LensCurvatureEnabled )
        {
            UpdateLensCurvatureZoom();
            Mat.SetFloat( "_LensCurvatureBarrelPower", LensCurvaturePower );
        }

	    if ( FilmGrainIntensity >= .001f )
	    {
	        Mat.SetFloat( "_FilmGrainIntensity", FilmGrainIntensity );
            Mat.SetFloat( "_FilmGrainTiling", FilmGrainTiling );

            Mat.EnableKeyword( "FILM_GRAIN_ON" );
            Mat.DisableKeyword( "FILM_GRAIN_OFF" );
	    } else {
            Mat.EnableKeyword( "FILM_GRAIN_OFF" );
            Mat.DisableKeyword( "FILM_GRAIN_ON" );
	    }

        if ( VignettingIntensity <= 001f )
        {
            Mat.SetFloat( "_VignettingIntensity", VignettingIntensity );

            Mat.EnableKeyword( "VIGNETTING_ON" );
            Mat.DisableKeyword( "VIGNETTING_OFF" );
        } else
        {
            Mat.EnableKeyword( "VIGNETTING_OFF" );
            Mat.DisableKeyword( "VIGNETTING_ON" );
        }
        Mat.SetFloat("_VignettingIntensity", VignettingIntensity);
        //Mat.SetFloat("_adapted_lum", Adapted_lum);
	    
		if ( ColorEffectsEnabled ) {
            Mat.EnableKeyword( "COLOR_FX_ON" );
            Mat.DisableKeyword( "COLOR_FX_OFF" );

            Mat.SetTexture("_RgbTex", colorGradingLut);

        } else {
            Mat.EnableKeyword( "COLOR_FX_OFF" );
            Mat.DisableKeyword( "COLOR_FX_ON" );
        }
	}
	
	public void OnEnable() {
		Init( true );
	}

    public void OnDisable()
	{
		if(null != Mat)
			DestroyImmediate(Mat);
		
		RenderTextureManager.Instance.Dispose();

#if DOFPRO_EFFECT
        DOFHelper.Instance.Dispose();
#endif

#if MOBILEDOFPRO_EFFECT
        MobileDOFHelper.Instance.Dispose();
#endif

#if BLOOMPRO_EFFECT
        BloomHelper.Instance.Dispose();
		#endif
	}

    //
    //Settings:
    //
    //High:     10 blur, 5 samples
    //Normal:   5 blur, 5 samples
    //Fast:     5 blur, 3 samples
    //Fastest:  5 blur, 3 samples, 2 pre-samples



    public void OnValidate()
	{
        if(this.enabled)
		    Init( false );
	}
	
	public static RenderTexture DownsampleTex( RenderTexture input, float downsampleBy ) {
		RenderTexture tempRenderTex =  RenderTextureManager.Instance.RequestRenderTexture( Mathf.RoundToInt( (float)input.width / downsampleBy ), Mathf.RoundToInt( (float)input.height / downsampleBy ), input.depth, input.format);
		tempRenderTex.filterMode = FilterMode.Bilinear;
		
		//Downsample pass
//		Graphics.Blit(input, tempRenderTex, _mat, 1);

        const float off = 1f;
        Graphics.BlitMultiTap( input, tempRenderTex, TapMat,
            new Vector2( -off, -off ),
            new Vector2( -off, off ),
            new Vector2( off, off ),
            new Vector2( off, -off )
        );
		
		return tempRenderTex;
	}
	
	private RenderTexture ApplyColorEffects( RenderTexture input  )
	{
		if ( !ColorEffectsEnabled || colorGradingLut == null)
			return input;
	
		RenderTexture tempRenderTex = RenderTextureManager.Instance.RequestRenderTexture( input.width, input.height, input.depth, input.format );
		
		Graphics.Blit( input, tempRenderTex, Mat, 5 );
		
		return tempRenderTex;
	}
	
	private RenderTexture ApplyLensCurvature( RenderTexture input  )
	{
		if ( !LensCurvatureEnabled )
				return input;
				
		RenderTexture tempRenderTex = RenderTextureManager.Instance.RequestRenderTexture( input.width, input.height, input.depth, input.format );
		
//			if ( VisualizeLensCurvature )
//			{
//				Graphics.Blit( _gridTexture, destination, Mat, LensCurvaturePrecise ? 3 : 4 );
//				return;
//			}
		
		Graphics.Blit( input, tempRenderTex, Mat, LensCurvaturePrecise ? 3 : 4 );
		
		return tempRenderTex;
	}
	
	private RenderTexture ApplyChromaticAberration( RenderTexture input  )
	{
	    if (!ChromaticAberration) return null;

		RenderTexture tempRenderTex =  RenderTextureManager.Instance.RequestRenderTexture( input.width, input.height, input.depth, input.format);
		tempRenderTex.filterMode = FilterMode.Bilinear;
		
		//Chromatic aberration pass
		Graphics.Blit(input, tempRenderTex, Mat, 2);

        Mat.SetTexture("_ChromAberrTex", tempRenderTex);	//Chromatic abberation texture

		return tempRenderTex;
	}

    Vector2 ApplyLensCurvature( Vector2 uv, float barrelPower, bool precise )
    {
        uv = uv * 2f - Vector2.one;

        uv.x *= GetComponent<Camera>().aspect * 2f;

        float theta = Mathf.Atan2( uv.y, uv.x );

        //return float2(theta, theta);

        float radius = uv.magnitude;

        if ( precise )
            radius = Mathf.Pow( radius, barrelPower );
        else
            radius = Mathf.Lerp( radius, radius * radius, Mathf.Clamp01( barrelPower - 1f ) );

        uv.x = radius * Mathf.Cos( theta );
        uv.y = radius * Mathf.Sin( theta );

        uv.x /= GetComponent<Camera>().aspect * 2f;

        return 0.5f * (uv + Vector2.one);
    }

    private void UpdateLensCurvatureZoom()
    {
        Vector2 cornerCoords = ApplyLensCurvature( new Vector2( 1f, 1f ), LensCurvaturePower, LensCurvaturePrecise );

        //Debug.Log( "cornerCoords: " + cornerCoords );

        float lensCurvatureZoom = 1f / cornerCoords.x;

        //lensCurvatureZoom /= camera.aspect;

        //Debug.Log( "lensCurvatureZoom: " + lensCurvatureZoom );

        Mat.SetFloat( "_LensCurvatureZoom", lensCurvatureZoom );
    }

    private void UpdateFilmGrain()
    {
        if ( FilmGrainIntensity >= .001f )
        {
            int curTex = Random.Range( 0, 3 );
            Mat.SetTexture( "_FilmGrainTex", _filmGrainTextures[curTex] );

            //Debug.Log( "curTex: " + curTex );

            int grainChannel = Random.Range( 0, 3 );

            switch ( grainChannel )
            {
                case 0:
                    Mat.SetVector( "_FilmGrainChannel", new Vector4(1f, 0f, 0f, 0f) );
                    break;

                case 1:
                    Mat.SetVector( "_FilmGrainChannel", new Vector4( 0f, 1f, 0f, 0f ) );
                    break;

                case 2:
                    Mat.SetVector( "_FilmGrainChannel", new Vector4( 0f, 0f, 1f, 0f ) );
                    break;

                case 3:
                    Mat.SetVector( "_FilmGrainChannel", new Vector4( 0f, 0f, 0f, 1f ) );
                    break;
            }
        }
    }

    void RenderEffects(RenderTexture source, RenderTexture destination)
    {
        source.filterMode = FilterMode.Bilinear;

        //UpdateFilmGrain();

        RenderTexture chromaticAberrationTex = source;
		RenderTexture curRenderTex = source;
        RenderTexture srcProcessed;

		srcProcessed = ApplyColorEffects(source);

		RenderTextureManager.Instance.SafeAssign( ref srcProcessed, ApplyLensCurvature(srcProcessed) );

        //Render chromatic aberration at full res
        if ( ChromaticAberrationPrecise )
            chromaticAberrationTex = ApplyChromaticAberration( srcProcessed );

        //Optimization - render all at 1/2 resolution
		RenderTextureManager.Instance.SafeAssign( ref curRenderTex, DownsampleTex( srcProcessed, 2f ) );

        if (Quality == EffectsQuality.Fastest)
            RenderTextureManager.Instance.SafeAssign( ref curRenderTex, DownsampleTex( curRenderTex, 2f ) );

        //
        //Depth of Field
        //
        //Optimization: being rendered at 1/2 resolution
        //
#if DOFPRO_EFFECT
        RenderTexture cocRenderTex = null, dofRenderTex = null;
        if (DOFEnabled) {
            if (null == DOFParams.EffectCamera)
            {
                Debug.LogError("null == DOFParams.camera");
                return;
            }

            cocRenderTex = RenderTextureManager.Instance.RequestRenderTexture(curRenderTex.width, curRenderTex.height, curRenderTex.depth, curRenderTex.format);

            DOFHelper.Instance.RenderCOCTexture(curRenderTex, cocRenderTex, BlurCOCTexture ? 1.5f : 0f);

            if (VisualizeCOC)
            {
                Graphics.Blit(cocRenderTex, destination, DOFHelper.Mat, 3);
                RenderTextureManager.Instance.ReleaseRenderTexture(cocRenderTex);
                RenderTextureManager.Instance.ReleaseRenderTexture(curRenderTex);
                return;
            }

            dofRenderTex = RenderTextureManager.Instance.RequestRenderTexture(curRenderTex.width, curRenderTex.height, curRenderTex.depth, curRenderTex.format);

            DOFHelper.Instance.RenderDOFBlur(curRenderTex, dofRenderTex, cocRenderTex);

            Mat.SetTexture("_DOFTex", dofRenderTex);
            Mat.SetTexture("_COCTex", cocRenderTex);

            //Make bloom DOF-based?
            //RenderTextureManager.Instance.SafeAssign(ref curRenderTex, dofRenderTex);
            //Graphics.Blit(dofRenderTex, destination);
           
        }
        else
        {
            if (null != DOFParams.EffectCamera)
            {
                DOFParams.EffectCamera.depthTextureMode = DepthTextureMode.None;
            }
        }
#endif

#if MOBILEDOFPRO_EFFECT
        RenderTexture MobiledofRenderTex = null;
        if (MobileDOFEnabled)
        {
            if (null == MobileDOFParams.EffectCamera)
            {
                Debug.LogError("null == MobileDOFParams.camera");
                return;
            }

            MobiledofRenderTex = RenderTextureManager.Instance.RequestRenderTexture(curRenderTex.width, curRenderTex.height, curRenderTex.depth, curRenderTex.format);
            MobileDOFHelper.Instance.RenderMobileDOFBlur(curRenderTex, MobiledofRenderTex);
            Mat.SetTexture("_MobileDOFTex", MobiledofRenderTex);
            DOF_FadeInFadeOut = Mathf.Clamp01(DOF_FadeInFadeOut);
            Mat.SetFloat("_DOF_FadeInFadeOut", DOF_FadeInFadeOut);
            Graphics.Blit(MobiledofRenderTex, destination);
        }
        else
        {
            if (null != MobileDOFParams.EffectCamera)
            {
                MobileDOFParams.EffectCamera.depthTextureMode = DepthTextureMode.None;
            }
        }
#endif

        //Render chromatic aberration at half res
        if (!ChromaticAberrationPrecise)
            chromaticAberrationTex = ApplyChromaticAberration( curRenderTex );

        //Graphics.Blit( chromaticAberrationTex, destination );
        //return;

        //Render bloom
#if BLOOMPRO_EFFECT
        if (BloomEnabled) {
            RenderTexture bloomTexture = RenderTextureManager.Instance.RequestRenderTexture(curRenderTex.width, curRenderTex.height, curRenderTex.depth, curRenderTex.format);
            BloomHelper.Instance.RenderBloomTexture(curRenderTex, bloomTexture);

            Mat.SetTexture("_BloomTex", bloomTexture);

            if ( VisualizeBloom )
            {
                
                Graphics.Blit( bloomTexture, destination );
                return;
            }
        }
#endif
        //destination.DiscardContents();
        //Final composite pass
        Graphics.Blit( srcProcessed, destination, Mat, 0 );

#if DOFPRO_EFFECT
        RenderTextureManager.Instance.ReleaseRenderTexture( cocRenderTex );
        RenderTextureManager.Instance.ReleaseRenderTexture( dofRenderTex );
#endif

#if MOBILEDOFPRO_EFFECT
        //RenderTextureManager.Instance.ReleaseRenderTexture(cocRenderTex);
        RenderTextureManager.Instance.ReleaseRenderTexture(MobiledofRenderTex);
#endif

        RenderTextureManager.Instance.ReleaseRenderTexture( curRenderTex );
        RenderTextureManager.Instance.ReleaseRenderTexture( chromaticAberrationTex );
    }


    [ImageEffectTransformsToLDR]
    public void OnRenderImage( RenderTexture source, RenderTexture destination )
	{
	    RenderEffects(source, destination);
		RenderTextureManager.Instance.ReleaseAllRenderTextures();
	}

    public void SetDofFade(float fade)
    {

#if MOBILEDOFPRO_EFFECT
        //DOF_FadeInFadeOut = fade;
#endif
    }
#else
    public void SetDofFade(float fade)
    {
    }
#endif
    public void Enable(bool enable)
    {
        this.enabled = enable;
    }
}
//Bloom:
//full screen chromab: 6.7ms; without - 4.9ms; 1/4 screen chromab: 5.2ms
//
//5 samples: 4.3 ms; 3 samples :3.6ms
//2 pre-samples, 3 total: 3.1 ms +27%
//
//HQ: 4.8ms
//NQ: 4.3ms
//FQ: 3.9ms
//FSQ: 3.4ms