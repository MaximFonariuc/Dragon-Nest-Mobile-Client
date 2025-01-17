Shader "Custom/Scene/CutoutDiffuseMaskLM_VMove" 
{
	Properties
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Mask ("Mask (A)", 2D) = "white" {}
		_PannerPara("X-位移距离 Y-位移距离",vector)=(1,1,1,1)
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
}