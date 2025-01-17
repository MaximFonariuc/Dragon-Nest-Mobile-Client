#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class RadialBlur : ImageEffectBase
{
    public float blurStrength = 6.0f;
    public float blurWidth = 0.7f;

    void Awake()
    {
        enabled = false;
        m_shaderName = "Hidden/radialBlur";
        //if (!SystemInfo.supportsRenderTextures)
        //{
        //    enabled = false;
        //    return;
        //}
    }
    void OnEnable()
    {
    }
    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        // Create the accumulation texture
        //if (accumTexture == null || accumTexture.width != source.width || accumTexture.height != source.height)
        //{
        //    DestroyImmediate(accumTexture);
        //    accumTexture = new RenderTexture(source.width, source.height, 0);
        //    accumTexture.hideFlags = HideFlags.HideAndDontSave;
        //    Graphics.Blit(source, accumTexture);
        //}

        material.SetTexture("_MainTex", source);
        material.SetFloat("_BlurStrength", blurStrength);
        material.SetFloat("_BlurWidth", blurWidth);
        material.SetFloat("_iHeight", 1);
        material.SetFloat("_iWidth", 1);
        //accumTexture.MarkRestoreExpected();

       // Graphics.Blit(source, accumTexture, material);
       // Graphics.Blit(accumTexture, dest);

        Graphics.Blit(source, dest, material);
    }
}
#endif