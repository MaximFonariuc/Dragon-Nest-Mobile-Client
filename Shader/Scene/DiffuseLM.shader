Shader "Custom/Scene/DiffuseLM" 
{
	Properties 
	{
		[Header(scene diffuse with lightmap)]
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
			#define LM
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"			
			ENDCG
		}
	} 
	FallBack Off
}