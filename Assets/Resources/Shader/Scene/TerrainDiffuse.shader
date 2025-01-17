Shader "Custom/Scene/TerrainDiffuse" 
{
	Properties
	{
		_Control("Control (RGBA)", 2D) = "red" {}
		_Splat3("Layer 3 (A)", 2D) = "white" {}
		_Splat2("Layer 2 (B)", 2D) = "white" {}
		_Splat1("Layer 1 (G)", 2D) = "white" {}
		_Splat0("Layer 0 (R)", 2D) = "white" {}
		[HideInInspector]_MainTex("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector]_Color("Main Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags{ "Queue" = "Geometry-99" "RenderType" = "Opaque"}
		Pass
		{
			Tags { "LightMode" = "Vertex" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile __ CUSTOM_SHADOW_ON 
			#pragma target 3.0
			#define TERRAIN
			#define TERRAIN_FIRSTPASS
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}
		Pass
		{
			Tags { "LightMode" = "VertexLMRGBM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile __ CUSTOM_SHADOW_ON 
			#pragma target 3.0
			#define LM
			#define TERRAIN
			#define TERRAIN_FIRSTPASS
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}
		Pass
		{
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile __ CUSTOM_SHADOW_ON 
			#pragma target 3.0
			#define LM
			#define TERRAIN
			#define TERRAIN_FIRSTPASS			
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}
	}
}
