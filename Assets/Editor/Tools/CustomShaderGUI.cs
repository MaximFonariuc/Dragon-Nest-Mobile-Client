using System;
using UnityEngine;
using UnityEditor;

public class CustomShaderGUI : ShaderGUI
{
    public enum ESplitMode
    {
        None = 0,
        Split2
    }

    public enum ESplitType
    {
        Horizontally,
        Vertically,
    }

    private static class Styles
    {
        public static GUIStyle optionsButton = "PaneOptions";

        public static GUIContent uvSetLabel = new GUIContent("UV Set");
        public static GUIContent[] uvSetOptions = new GUIContent[] { new GUIContent("UV channel 0"), new GUIContent("UV channel 1") };

        public static string emptyTootip = "";
        public static string mainTexText = "Main Tex Albedo (RGB)";
        public static string splitTexText = "Second Tex Albedo (RGB)";
        public static string maskTexText = "Mask Tex Alpha(R)";
        public static string splitmaskTexText = "Second Mask Tex Alpha(R)";

        public static string rimColorText = "Rim Color Rim Light Color";
        public static string lightArgsText = "LightArg x: Diffuse Scale y: Light Scale z: not use w: Rim Power";
        public static string effectColorText = "Effect Color";
        
        public static string splitModeText = "Smoothness Smoothness scale factor";
        public static string splitMode = "Split Mode";
        public static string splitType = "Split Type";
        public static string whiteSpaceString = " ";
        public static string primaryTexsText = "Textures";
        public static readonly string[] splitNames = Enum.GetNames(typeof(ESplitMode));
        public static readonly string[] splitTypeNames = Enum.GetNames(typeof(ESplitType));
    }
    MaterialProperty splitMode = null;
    MaterialProperty splitType = null;
    MaterialProperty mainTex = null;    
    MaterialProperty splitTex = null;
    MaterialProperty maskTex = null;
    MaterialProperty splitMaskTex = null;
    MaterialProperty rimColor = null;
    MaterialProperty lightArgs = null;
    MaterialProperty uvScale = null;
    MaterialProperty uvRange = null;
    MaterialProperty color = null;
    ESplitMode m_blendMode = ESplitMode.Split2;

    MaterialEditor m_MaterialEditor;

    bool m_FirstTimeApply = true;
    static string m_KeywordSplitTex = "ENABLE_SPLIT";

    MaterialProperty FindProperties(string name, MaterialProperty[] props)
    {
        for (int i = 0; i < props.Length; ++i)
        {
            MaterialProperty mp = props[i];
            if (mp.name == name)
            {
                return mp;
            }
        }
        return null;
    }
    public void FindProperties(MaterialProperty[] props)
    {
        splitMode = FindProperties("_Split", props);
        splitType = FindProperties("_H", props);
        mainTex = FindProperties("_MainTex", props);
        splitTex = FindProperties("_MainTex1", props);
        maskTex = FindProperties("_Mask", props);
        splitMaskTex = FindProperties("_Mask1", props);
        rimColor = FindProperties("_RimColor", props);
        lightArgs = FindProperties("_LightArgs", props);
        uvScale = FindProperties("_LightArgs", props);
        uvRange = FindProperties("_UVRange", props);
        color = FindProperties("_Color", props);
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        FindProperties(props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
        m_MaterialEditor = materialEditor;
        Material material = materialEditor.target as Material;

        // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
        // material to a standard shader.
        // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
        if (m_FirstTimeApply)
        {
            MaterialChanged(material);
            m_FirstTimeApply = false;
        }

        ShaderPropertiesGUI(material);
    }

    public void ShaderPropertiesGUI(Material material)
    {
        // Use default labelWidth
        EditorGUIUtility.labelWidth = 0f;

        // Detect any changes to the material
        EditorGUI.BeginChangeCheck();
        {
            SplitModePopup(material);

            // Primary properties
            GUILayout.Label(Styles.primaryTexsText, EditorStyles.boldLabel);
            m_MaterialEditor.TextureProperty(mainTex, Styles.mainTexText, false);

            if (splitMode == null || (ESplitMode)splitMode.floatValue == ESplitMode.Split2)
            {
                if (splitTex != null)
                    m_MaterialEditor.TextureProperty(splitTex, Styles.splitTexText, false);
            }
            if (maskTex != null)
            {
                m_MaterialEditor.TextureProperty(maskTex, Styles.maskTexText, false);
                if (splitMode == null || (ESplitMode)splitMode.floatValue == ESplitMode.Split2)
                {
                    if (splitMaskTex != null)
                        m_MaterialEditor.TextureProperty(splitMaskTex, Styles.splitmaskTexText, false);
                }
            }
            if (rimColor != null)
            {
                m_MaterialEditor.ShaderProperty(rimColor, Styles.rimColorText);
            }
            if (lightArgs != null)
            {
                GUILayout.Label(Styles.lightArgsText);
                m_MaterialEditor.ShaderProperty(lightArgs, "");
            }
            if (color != null && material.shader != null && material.shader.name.Contains("NoLight"))
            {
                m_MaterialEditor.ShaderProperty(color, Styles.effectColorText);
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            MaterialChanged(material);
        }
    }

    void SplitModePopup(Material material)
    {
        if(splitMode!=null)
        {
            var mode = (ESplitMode)splitMode.floatValue;

            EditorGUI.BeginChangeCheck();
            mode = (ESplitMode)EditorGUILayout.Popup(Styles.splitMode, (int)mode, Styles.splitNames);
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo(Styles.splitMode);
                splitMode.floatValue = (float)mode;
            }

            if (splitType != null && mode != ESplitMode.None)
            {
                var st = (ESplitType)splitType.floatValue;

                EditorGUI.BeginChangeCheck();
                st = (ESplitType)EditorGUILayout.Popup(Styles.splitType, (int)st, Styles.splitTypeNames);
                if (EditorGUI.EndChangeCheck())
                {
                    m_MaterialEditor.RegisterPropertyChangeUndo(Styles.splitType);
                    splitType.floatValue = (float)st;
                }
            }
        }



    }

    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);        
        MaterialChanged(material);
    }

    public static void SetupMaterialWithSplitMode(Material material, ESplitMode splitMode)
    {
        //switch (splitMode)
        //{
        //    case BlendMode.Opaque:
        //        material.SetOverrideTag("RenderType", "");
        //        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        //        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        //        material.SetInt("_ZWrite", 1);
        //        material.DisableKeyword("_ALPHATEST_ON");
        //        material.DisableKeyword("_ALPHABLEND_ON");
        //        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //        material.renderQueue = -1;
        //        break;
        //    case BlendMode.Cutout:
        //        material.SetOverrideTag("RenderType", "TransparentCutout");
        //        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        //        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        //        material.SetInt("_ZWrite", 1);
        //        material.EnableKeyword("_ALPHATEST_ON");
        //        material.DisableKeyword("_ALPHABLEND_ON");
        //        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
        //        break;
        //    case BlendMode.Fade:
        //        material.SetOverrideTag("RenderType", "Transparent");
        //        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        //        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //        material.SetInt("_ZWrite", 0);
        //        material.DisableKeyword("_ALPHATEST_ON");
        //        material.EnableKeyword("_ALPHABLEND_ON");
        //        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        //        break;
        //    case BlendMode.Transparent:
        //        material.SetOverrideTag("RenderType", "Transparent");
        //        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        //        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //        material.SetInt("_ZWrite", 0);
        //        material.DisableKeyword("_ALPHATEST_ON");
        //        material.DisableKeyword("_ALPHABLEND_ON");
        //        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        //        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        //        break;
        //}
    }

    void MaterialChanged(Material material)
    {
        if(splitMode!=null&& splitType!=null)
        {
            if ((ESplitMode)splitMode.floatValue == ESplitMode.Split2)
            {
                SetKeyword(material, "ENABLE_SPLIT", true);
                if ((ESplitType)splitType.floatValue == ESplitType.Horizontally)
                {
                    material.SetVector("_UVScale", new Vector4(-0.5f, 0.0f, 2.0f, 1.0f));
                    material.SetVector("_UVRange", new Vector4(1.0f, 1.0f, 0.5f, 0.0f));
                }
                else
                {
                    material.SetVector("_UVScale", new Vector4(0.0f, -0.5f, 1.0f, 2.0f));
                    material.SetVector("_UVRange", new Vector4(1.0f, 1.0f, 0.0f, 0.5f));
                }
            }
            else
            {
                SetKeyword(material, "ENABLE_SPLIT", false);
            }
        }        
    }

    static void SetKeyword(Material m, string keyword, bool state)
    {
        if (state)
            m.EnableKeyword(keyword);
        else
            m.DisableKeyword(keyword);
    }
}
