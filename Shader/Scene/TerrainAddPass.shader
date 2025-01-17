Shader "Custom/Scene/TerrainAddPass" 
{
	Properties
	{
		[HideInInspector]_Control("Control (RGBA)", 2D) = "red" {}
		[HideInInspector]_Splat1("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector]_Splat0("Layer 0 (R)", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Geometry-99" "IgnoreProjector" = "True" "RenderType" = "Opaque"}
		Blend SrcAlpha OneMinusSrcAlpha		
		Pass
		{
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			#pragma vertex vertT
			#pragma fragment fragT
			#pragma multi_compile __ CUSTOM_SHADOW_ON 
			#pragma target 3.0
			#define LM
			#define TERRAIN
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}
	}
}
