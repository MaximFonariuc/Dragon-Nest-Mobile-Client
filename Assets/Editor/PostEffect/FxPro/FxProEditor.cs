#define FXPRO_EFFECT
//#define BLOOMPRO_EFFECT
//#define DOFPRO_EFFECT

#if FXPRO_EFFECT
#define BLOOMPRO_EFFECT
//#define DOFPRO_EFFECT
#define MOBILEDOFPRO_EFFECT
#endif

using System.IO;

using UnityEngine;
using UnityEditor;
using FxProNS;

#if FXPRO_EFFECT
[CustomEditor(typeof(FxPro))]
public class FxProEditor : Editor
#elif BLOOMPRO_EFFECT
[CustomEditor( typeof( BloomPro ) )]
public class BloomProEditor : Editor
#elif DOFPRO_EFFECT
[CustomEditor( typeof( DOFPro ) )]
public class DOFProEditor : Editor
#endif
{
    private Texture2D _logo;

    private SerializedObject _serializedObj;

    private SerializedProperty _quality;

    //private SerializedProperty _lensDirtTexture, _lensDirtIntensity;
    private SerializedProperty _chromaticAberration, _chromaticAberrationPrecise, _chromaticAberrationOffset;

    private SerializedProperty _lensCurvatureEnabled, _lensCurvaturePrecise, _lensCurvatureBarrelPower;

    private SerializedProperty _filmGrainIntensity, _filmGrainTiling;

    private SerializedProperty _vignettingIntensity;

    private SerializedProperty _SCurveIntensity;
    //Bloom properties
#if BLOOMPRO_EFFECT
    private SerializedProperty _bloomParameters, _bloomEnabled, _visualizeBloom, _bloomTint, _bloomThreshold, _bloomIntensity, _bloomSoftness;
    private bool _visualizeBloomWasEnabled = false;
#endif

    //DOF properties
#if DOFPRO_EFFECT
    private SerializedProperty _dofParameters, _dofEnabled, _visualizeCoc,
                                _bokehEnabled, _bokehThreshold, _bokehGain, _bokehBias,
                                _blurCocTexture, _useUnityDepthBuffer, _autoFocus, _autoFocusLayerMask, _autoFocusSpeed,
                                _focalLengthMultiplier, _depthCompression, _dofTarget, _dofBlurSize, _dofDoubleIntensity;
    private bool _visualizeCocWasEnabled = false;
#endif

#if MOBILEDOFPRO_EFFECT
    private SerializedProperty _MobiledofParameters, _MobiledofTarget, _MobileDOFEnabled, FocalLengthMultiplier, DOFBlurSize, NonTargetFocalDist;
    private bool _visualizeCocWasEnabled = false;
#endif

    private SerializedProperty _colorEffectsEnabled, _colorGradingLut;

    private EffectsQuality _prevQuality;

    private bool _chromaticAberrationPreciseWasEnabled = false;
    private SerializedProperty _adapted_lum;
    void OnEnable()
    {
        _serializedObj = new SerializedObject(target);

        _quality = _serializedObj.FindProperty("Quality");

        //_lensDirtTexture = _serializedObj.FindProperty( "LensDirtTexture" );
        //_lensDirtIntensity = _serializedObj.FindProperty( "LensDirtIntensity" );
        _chromaticAberration = _serializedObj.FindProperty("ChromaticAberration");
        _chromaticAberrationPrecise = _serializedObj.FindProperty("ChromaticAberrationPrecise");
        _chromaticAberrationOffset = _serializedObj.FindProperty("ChromaticAberrationOffset");

        _lensCurvatureEnabled = _serializedObj.FindProperty("LensCurvatureEnabled");
        _lensCurvaturePrecise = _serializedObj.FindProperty("LensCurvaturePrecise");
        _lensCurvatureBarrelPower = _serializedObj.FindProperty("LensCurvaturePower");

        _filmGrainIntensity = _serializedObj.FindProperty("FilmGrainIntensity");
        _filmGrainTiling = _serializedObj.FindProperty("FilmGrainTiling");

        _vignettingIntensity = _serializedObj.FindProperty("VignettingIntensity");

        _SCurveIntensity = _serializedObj.FindProperty("SCurveIntensity");

#if BLOOMPRO_EFFECT
        //Bloom
        _bloomParameters = _serializedObj.FindProperty("BloomParams");

        _bloomEnabled = _serializedObj.FindProperty("BloomEnabled");

        _visualizeBloom = _serializedObj.FindProperty("VisualizeBloom");

        _bloomTint = _bloomParameters.FindPropertyRelative("BloomTint");

        _bloomThreshold = _bloomParameters.FindPropertyRelative("BloomThreshold");
        _bloomIntensity = _bloomParameters.FindPropertyRelative("BloomIntensity");
        _bloomSoftness = _bloomParameters.FindPropertyRelative("BloomSoftness");
#endif

        //DOF
#if DOFPRO_EFFECT
        _dofParameters           = _serializedObj.FindProperty( "DOFParams" );

        _dofEnabled              = _serializedObj.FindProperty( "DOFEnabled" );
        _blurCocTexture          = _serializedObj.FindProperty( "BlurCOCTexture" );
        _visualizeCoc            = _serializedObj.FindProperty( "VisualizeCOC" );

        _bokehEnabled            = _dofParameters.FindPropertyRelative( "BokehEnabled" );
        _bokehThreshold          = _dofParameters.FindPropertyRelative( "BokehThreshold" );
        _bokehGain               = _dofParameters.FindPropertyRelative( "BokehGain" );
        _bokehBias               = _dofParameters.FindPropertyRelative( "BokehBias" );

        _dofBlurSize             = _dofParameters.FindPropertyRelative( "DOFBlurSize" );
        _useUnityDepthBuffer     = _dofParameters.FindPropertyRelative( "UseUnityDepthBuffer" );
        _autoFocus               = _dofParameters.FindPropertyRelative( "AutoFocus" );
        _autoFocusLayerMask      = _dofParameters.FindPropertyRelative( "AutoFocusLayerMask" );
        _autoFocusSpeed          = _dofParameters.FindPropertyRelative( "AutoFocusSpeed" );
        _focalLengthMultiplier   = _dofParameters.FindPropertyRelative( "FocalLengthMultiplier" );
        _depthCompression        = _dofParameters.FindPropertyRelative( "DepthCompression" );
        _dofDoubleIntensity      = _dofParameters.FindPropertyRelative( "DoubleIntensityBlur" );
        _dofTarget               = _dofParameters.FindPropertyRelative( "Target" );
#endif

#if MOBILEDOFPRO_EFFECT
        _MobiledofParameters = _serializedObj.FindProperty("MobileDOFParams");
        _MobileDOFEnabled = _serializedObj.FindProperty("MobileDOFEnabled");
        _MobiledofTarget = _MobiledofParameters.FindPropertyRelative("Target");
        FocalLengthMultiplier = _MobiledofParameters.FindPropertyRelative("FocalLengthMultiplier");
        DOFBlurSize = _MobiledofParameters.FindPropertyRelative("DOFBlurSize");
        NonTargetFocalDist = _MobiledofParameters.FindPropertyRelative("NonTargetFocalDist");
#endif

        _colorEffectsEnabled = _serializedObj.FindProperty("ColorEffectsEnabled");

        _colorGradingLut = _serializedObj.FindProperty("colorGradingLut");


        _adapted_lum = _serializedObj.FindProperty("Adapted_lum");

        //Load dynamic resources
        string pluginPath = FilePathToAssetPath(GetPluginPath());

        //var lensTexture = Resources.LoadAssetAtPath<Texture2D>(pluginPath + "/Textures/lens_01.png");

        //if (null == _lensDirtTexture.objectReferenceValue) {
        //    _lensDirtTexture.objectReferenceValue = lensTexture;
        //    _serializedObj.ApplyModifiedProperties();
        //}

        string logoPath = pluginPath + "/FxPro";
        logoPath = Path.Combine(logoPath, "banner.png");

        _logo = AssetDatabase.LoadAssetAtPath<Texture2D>(logoPath);

        if (null == _logo)
            Debug.LogError("null == logo");

        _prevQuality = null == _quality ? EffectsQuality.Normal : (EffectsQuality)_quality.enumValueIndex;

    }

    //	private string AssetPathToFilePath(string assetPath) {
    //		return Application.dataPath + "/" + assetPath.Remove( assetPath.IndexOf("Assets/"), "Assets/".Length);
    //	}

    private static string FilePathToAssetPath(string filePath)
    {
        int indexOfAssets = filePath.ToLower().LastIndexOf("assets");

        return filePath.Substring(indexOfAssets);
    }

    private string GetPluginPath()
    {
        MonoScript ms = MonoScript.FromScriptableObject(this);
        string scriptPath = AssetDatabase.GetAssetPath(ms);

        var directoryInfo = Directory.GetParent(scriptPath).Parent;
        return directoryInfo != null ? directoryInfo.FullName : null;
    }

    private void ValidateSettings()
    {
        //
        //Quality

        //Changed quality?
        if ((EffectsQuality)_quality.enumValueIndex != _prevQuality)
        {
#if DOFPRO_EFFECT
            //Set default quality values
            if ((EffectsQuality)_quality.enumValueIndex == EffectsQuality.Fastest) {
                _chromaticAberration.boolValue = false;
                _blurCocTexture.boolValue = false;
                _useUnityDepthBuffer.boolValue = false;
            }

            //Fastest => any other
            if (_prevQuality == EffectsQuality.Fastest) {
                _chromaticAberration.boolValue = true;
                _blurCocTexture.boolValue = true;
            }

#endif

            if ((EffectsQuality)_quality.enumValueIndex == EffectsQuality.Fastest ||
                 (EffectsQuality)_quality.enumValueIndex == EffectsQuality.Fast)
            {
                _chromaticAberrationPrecise.boolValue = false;
                _lensCurvaturePrecise.boolValue = false;
            }

            _prevQuality = (EffectsQuality)_quality.enumValueIndex;
        }

        //
        //Visualizations
#if DOFPRO_EFFECT
        //Turned on Bloom visualization?
        if ( _visualizeBloom.boolValue && !_visualizeBloomWasEnabled  )
        {
            _visualizeCoc.boolValue = false;
        }

        //Turned on COC visualization?
        if ( _visualizeCoc.boolValue && !_visualizeCocWasEnabled )
        {
            _visualizeBloom.boolValue = false;
        }


        //Optimizing DOF blur size
        if ( _dofBlurSize.floatValue < 1f && _dofDoubleIntensity.boolValue )
        {
            _dofBlurSize.floatValue = 1f;
        }


        _visualizeBloomWasEnabled = _visualizeBloom.boolValue;
        _visualizeCocWasEnabled = _visualizeCoc.boolValue;
        _chromaticAberrationPreciseWasEnabled = _chromaticAberrationPrecise.boolValue;
#endif
    }

    public override void OnInspectorGUI()
    {
        _serializedObj.Update();

        EditorGUILayout.Space();

        float bannerWidth = Screen.width - 35;
        EditorGUILayout.LabelField(new GUIContent(_logo), GUILayout.Width(bannerWidth), GUILayout.Height(bannerWidth / 6));
        //EditorGUILayout.LabelField( new GUIContent( logo ), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) );

        Color bgColor = GUI.backgroundColor = new Color(.74f, .74f, 1f, 1f);
        //Color contentColor = GUI.contentColor = new Color(.8f, .8f, 1f, 1f);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_quality, new GUIContent("Quality", "Set to a lower value if you experience performance issues."));

        EditorGUILayout.Space();
        //EditorGUILayout.Space();

        //
        //Bloom
#if BLOOMPRO_EFFECT
        EditorGUILayout.PropertyField(_bloomEnabled, new GUIContent("Bloom", "Makes bright pixels bloom."));

        if (_bloomEnabled.boolValue)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_visualizeBloom, new GUIContent("Visualize", "Bloom visualization. Use for testing purposes only."));

            EditorGUILayout.Space();

            GUI.backgroundColor = Color.white;

            EditorGUILayout.PropertyField(_bloomTint, new GUIContent("Bloom Tint"));

            GUI.backgroundColor = bgColor;

            //bloomEnabled.boolValue = EditorGUILayout.BeginToggleGroup( new GUIContent( "Bloom", "Makes bright pixels bloom." ), bloomEnabled.boolValue );
            if (!((FxPro)target).GetComponent<Camera>().allowHDR)
                EditorGUILayout.PropertyField(_bloomThreshold, new GUIContent("Bloom Threshold", "Higher value = less blooming pixels. Set close to zero for a dreamy look."));

            EditorGUILayout.PropertyField(_bloomIntensity, new GUIContent("Bloom Intensity", "Higher value = brighter bloom."));
            EditorGUILayout.PropertyField(_bloomSoftness, new GUIContent("Bloom Softness", "Lower value = harder bloom edge. Higher value = softer bloom."));

            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();
        //EditorGUILayout.Space();
        EditorGUILayout.Space();
#endif

        //
        //Depth of Field
#if DOFPRO_EFFECT
        EditorGUILayout.PropertyField( _dofEnabled, new GUIContent( "Depth of Field", "Blurs out-of-focus areas." ) );

        if (_dofEnabled.boolValue)
	    {
            //EditorGUILayout.Space();

            EditorGUILayout.BeginVertical( "box" );

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField( _visualizeCoc, new GUIContent( "Visualize", "Circle of Confusion (bluriness) visualization. Use for testing purposes only." ) );

            EditorGUILayout.Space();

            if ( (EffectsQuality)_quality.enumValueIndex == EffectsQuality.Normal || (EffectsQuality)_quality.enumValueIndex == EffectsQuality.High )
            {
                EditorGUILayout.PropertyField( _bokehEnabled, new GUIContent( "Bokeh", "Enables bokeh. Performance-heavy, use only on higher-end platforms. " +
                                                                                       "Not suitable for mobile devices." ) );

                if ( _bokehEnabled.boolValue )
                {
                    EditorGUILayout.PropertyField( _bokehThreshold, new GUIContent( "\tHighlight Threshold" ) );
                    EditorGUILayout.PropertyField( _bokehGain, new GUIContent( "\tBokeh Gain" ) );
                    //EditorGUILayout.PropertyField( _bokehBias, new GUIContent( "\tBokeh Edge Bias" ) );
                }

                EditorGUILayout.Space();
            } else
            {
                _bokehEnabled.boolValue = false;
            }

	        //dofEnabled.boolValue = EditorGUILayout.BeginToggleGroup( new GUIContent("Depth of Field", "Blurs out-of-focus areas."), dofEnabled.boolValue);

            EditorGUILayout.PropertyField( _blurCocTexture, new GUIContent( "Blur COC", "Makes DOF look correct at edges of objects. Has performance impact." ) );

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField( _autoFocus, new GUIContent( "Autofocus", "Makes camera focus automatically on objects in the center of the screen." +
	                                                                             "Requires a collider attached to focusable objects.") );

            if (_autoFocus.boolValue) EditorGUILayout.PropertyField( _autoFocusLayerMask, new GUIContent( "\tLayers", "Autofocus Speed" ) );

            if (_autoFocus.boolValue) EditorGUILayout.PropertyField( _autoFocusSpeed, new GUIContent( "\tSpeed", "Autofocus Speed" ) );

            if (!_autoFocus.boolValue) EditorGUILayout.PropertyField( _dofTarget, new GUIContent( "Focus On", "Drop here a scene object that you'd like the camera to focus on." ) );

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField( _useUnityDepthBuffer, new GUIContent( "Use Unity Depth Buffer", "It's recommended to disable this option on mobile devices, " +
	                                                                                                    "and to make all shaders output depth to alpha channel (refer to manual for details)"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField( _focalLengthMultiplier, new GUIContent( "Focal Length Multiplier", "Higher values result in shallower depth-of-field." ) );
            //DOFParams.focalDistMultiplier = EditorGUILayout.FloatField("Focal Dist Multiplier", DOFParams.focalDistMultiplier);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField( _depthCompression, new GUIContent( "Depth Compression", "Compresses depth-map by moving camera's far clipping plane closer to improve COC-map quality." +
                                                                                                         "In most cases it's recommended to use the default value.") );

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField( _dofBlurSize, new GUIContent( "DOF Strength", "Higher values will make out-of-focus areas appear blurrier." ) );

            EditorGUILayout.PropertyField( _dofDoubleIntensity, new GUIContent( "Double Intensity", "Increases DOF blur intensity without decreasing quality. Has a large performance impact (x4 times slower). Can be used only if [DOF Strength >= 1.0]." ) );

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
	    }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //EditorGUILayout.Space();
#endif

#if MOBILEDOFPRO_EFFECT
        EditorGUILayout.PropertyField(_MobileDOFEnabled, new GUIContent("DOF", "开启景深效果"));

        if (_MobileDOFEnabled.boolValue)
        {

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_MobiledofTarget, new GUIContent("焦点目标位置"));

            EditorGUILayout.Space();


            EditorGUILayout.PropertyField(FocalLengthMultiplier, new GUIContent("模糊范围"));
            EditorGUILayout.PropertyField(DOFBlurSize, new GUIContent("模糊强度"));
            if (_MobiledofTarget.objectReferenceValue == null)
                EditorGUILayout.PropertyField(NonTargetFocalDist, new GUIContent("焦点距离", "没有焦点目标时的聚焦距离"));

            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();
        //EditorGUILayout.Space();
        EditorGUILayout.Space();
#endif


        //General effects
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.Space();

            //EditorGUILayout.PropertyField( _lensDirtIntensity, new GUIContent( "Lens Dirt", "Lens Dirt Intensity" ) );

            //if (_lensDirtIntensity.floatValue > .0001f) EditorGUILayout.PropertyField( _lensDirtTexture, new GUIContent( "\tTexture", "Lens Dirt Texture" ) );

            //EditorGUILayout.Space();


            EditorGUILayout.PropertyField(_chromaticAberration, new GUIContent("Chromatic Aberration", "Simulates real camera lens' chromatic aberration."));

            if (_chromaticAberration.boolValue)
            {
                EditorGUILayout.PropertyField(_chromaticAberrationPrecise, new GUIContent("\tPrecise", "Makes the chromatic aberration look thinner. Runs a little bit slower when enabled."));
                EditorGUILayout.PropertyField(_chromaticAberrationOffset, new GUIContent("\tOffset", "Larger value gives larger chromatic aberration effect."));
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_lensCurvatureEnabled, new GUIContent("Lens Curvature"));

            if (_lensCurvatureEnabled.boolValue)
            {
                EditorGUILayout.PropertyField(_lensCurvaturePrecise, new GUIContent("\tPrecise", "May affect performance, especially on mobile devices."));
                EditorGUILayout.PropertyField(_lensCurvatureBarrelPower, new GUIContent("\tPower", "Lens curvature barrel power."));
            }

            EditorGUILayout.Space();


            EditorGUILayout.PropertyField(_filmGrainIntensity, new GUIContent("Film Grain"));
            if (_filmGrainIntensity.floatValue >= .001f)
            {
                EditorGUILayout.PropertyField(_filmGrainTiling, new GUIContent("\tScale"));
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_vignettingIntensity, new GUIContent("Vignetting"));
            EditorGUILayout.Space();
            //EditorGUILayout.PropertyField(_adapted_lum, new GUIContent("adapted_lum"));
            //EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Color effects
        EditorGUILayout.PropertyField(_colorEffectsEnabled, new GUIContent("Color Grading"));

        if (_colorEffectsEnabled.boolValue)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Lut"));
            EditorGUILayout.PropertyField(_colorGradingLut, new GUIContent("\tColor Lut"));
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        ValidateSettings();

        _serializedObj.ApplyModifiedProperties();
    }
}