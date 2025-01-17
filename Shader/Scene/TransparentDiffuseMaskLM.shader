Shader "Custom/Scene/TransparentDiffuseMaskLM" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Mask ("Mask (A)", 2D) = "white" {}
	}
	SubShader 
	{  
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"  }
		LOD 100 
	
		Blend SrcAlpha OneMinusSrcAlpha	
		Pass 
		{  
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define LM
			#define MASKTEX
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}
	} 
}