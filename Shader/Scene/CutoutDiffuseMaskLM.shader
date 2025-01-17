//Scene cutout lightmap
Shader "Custom/Scene/CutoutDiffuseMaskLM" 
{
	Properties 
	{
		_MainTex ("Base (RGB) ", 2D) = "white" {}
		_Mask ("Mask (R)", 2D) = "white" {}
	}
	SubShader 
	{  
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"  }	  
		LOD 100		
		Pass 
		{  
			//Moblie
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#define CUTOUT
			#define MASKTEX
			#define LM
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"			
			ENDCG
		}
	} 
	FallBack Off
}