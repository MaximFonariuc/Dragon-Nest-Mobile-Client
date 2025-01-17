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
			Tags { "LightMode" = "Vertex" }
			CGPROGRAM
			//vertex&fragment
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ CUSTOM_SHADOW_ON 
			//define
			#define SHLIGHTON
			#define LAMBERT
			#define DEFAULTBASECOLOR
			#define ORIGINAL_LIGHT
			//head
			#include "../Include/CommonHead_Include.cginc"
			//include
			#include "../Include/CommonBasic_Include.cginc"
			ENDCG 
		}

		Pass 
		{
			//Pc
			Tags { "LightMode" = "VertexLMRGBM" }
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile __ CUSTOM_SHADOW_ON 
			#define LM
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}

		Pass 
		{  
			//Moblie
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog 
			#pragma multi_compile __ CUSTOM_SHADOW_ON 
			#define LM
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"			
			ENDCG
		}
		
		UsePass "Custom/Common/META"
		UsePass "Custom/Common/CASTSHADOW"
	} 
	FallBack Off
}