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
		LOD 400 	
		Pass 
		{
			Tags { "LightMode" = "Vertex" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#define CUTOUT
			#define MASKTEX
			#define ANIM
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
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
			#define CUTOUT
			#define MASKTEX
			#define LM
			#define ANIM
			
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}
		Pass 
		{  
			Tags { "LightMode" = "VertexLM"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#define CUTOUT
			#define MASKTEX
			#define LM
			#define ANIM
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}
		UsePass "Custom/Common/META"
		UsePass "Custom/Common/CASTSHADOWCUTOUT"
	} 
	SubShader 
	{
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"  }
		LOD 100		
		Pass 
		{ 
			//Pc
			Tags { "LightMode" = "VertexLMRGBM" }
			CGPROGRAM			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#define CUTOUT
			#define MASKTEX
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
			#define CUTOUT
			#define MASKTEX
			#define LM
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"	
			ENDCG
		}
	}
}