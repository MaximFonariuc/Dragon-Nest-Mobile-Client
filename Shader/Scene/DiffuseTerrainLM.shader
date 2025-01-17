Shader "Custom/Scene/DiffuseTerrainLM" 
{
	Properties 
	{
		_MainTex ("Base (RGB) ", 2D) = "white" {}
	}
	SubShader 
	{  
		Tags { "RenderType"="Opaque" }	  
		LOD 100
		Pass 
		{  
			//Moblie
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ CUSTOM_SHADOW_ON 
			#define LM
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"			
			ENDCG
		}
	} 
	FallBack Off
}