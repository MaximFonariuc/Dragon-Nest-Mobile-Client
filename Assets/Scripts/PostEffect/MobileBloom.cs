using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class MobileBloom : MobilePostEffectsBase
{
    public enum Resolution {
		Low = 0,
		High = 1,
	}

	public enum BlurType {
		Standard = 0,
		Sgx = 1,
	}
    public enum BlurDir
    {
        vertical = 0,
        horizontal = 1,
        both = 2
    }
    [Range(0.0f, 1.5f)]
    public float threshhold = 0.25f;

    [Range(0.0f, 2.5f)]
    public float intensity = 0.75f;

    [Range(0.25f, 5.5f)]
    public float blurSize = 1.0f;
	
    Resolution resolution = Resolution.Low;
    [Range(1, 4)]
    private int blurIterations = 1;

    private BlurType blurType = BlurType.Sgx;

    public BlurDir blurDir = BlurDir.vertical;

    public Shader fastBloomShader;
    private Material fastBloomMaterial = null;
	
    new bool CheckResources ()
    {	
        CheckSupport (false);	
	
        if(fastBloomShader==null)
            fastBloomShader = Shader.Find("Hidden/FastBloom");
        if (fastBloomMaterial==null)
            fastBloomMaterial = CheckShaderAndCreateMaterial (fastBloomShader, fastBloomMaterial);
		
        //if(!isSupported)
        //    ReportAutoDisable ();
        return isSupported;				
    }

    void OnDisable()
    {
        if (fastBloomMaterial)
            DestroyImmediate(fastBloomMaterial);
    }
	
    void OnRenderImage (RenderTexture source,RenderTexture destination) 
    {	
        if(CheckResources() == false) 
        {
            //Graphics.Blit (source, destination);
            return;
        }

        int divider = resolution == Resolution.Low ? 4 : 2;
        float widthMod = resolution == Resolution.Low ? 0.5f : 1.0f;

        fastBloomMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod, 0.0f, threshhold, intensity));
        source.filterMode = FilterMode.Bilinear;

        int rtW = source.width / divider;
        int rtH = source.height / divider;

        // downsample
        RenderTexture rt = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
        rt.filterMode = FilterMode.Bilinear;
        Graphics.Blit (source, rt, fastBloomMaterial, 1);

        int passOffs = blurType == BlurType.Standard ? 0 : 2;
		
        for(int i = 0; i < blurIterations; i++) 
        {
            fastBloomMaterial.SetVector ("_Parameter", new Vector4 (blurSize * widthMod + (i*1.0f), 0.0f, threshhold, intensity));
            RenderTexture rt2 = null;
            if (blurDir == BlurDir.vertical || blurDir == BlurDir.both)
            {
                // vertical blur
                rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, fastBloomMaterial, 2 + passOffs);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;
            }

            if (blurDir == BlurDir.horizontal || blurDir == BlurDir.both)
            {
                // horizontal blur
                rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, fastBloomMaterial, 3 + passOffs);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;
            }

        }

        fastBloomMaterial.SetTexture("_Bloom", rt);

        Graphics.Blit(source, destination, fastBloomMaterial, 0);

        RenderTexture.ReleaseTemporary(rt);
    }	
}