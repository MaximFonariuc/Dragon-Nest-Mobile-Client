
#ifndef SCENEHEAD_INCLUDED
#define SCENEHEAD_INCLUDED

#include "UnityCG.cginc"

struct appdata_t
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
#if defined(LM)|defined(UV2)
	half2 uv2 : TEXCOORD1;
#endif
#ifdef VC
	fixed4 color : COLOR;
#endif
};

struct v2f
{
	float4 vertex : SV_POSITION;
	half2 uv : TEXCOORD0;
#if defined(LM)|defined(UV2)
	half2 uv2 : TEXCOORD1;
#endif
	UNITY_FOG_COORDS(2)
#if defined( WP)|defined(CUSTOM_SHADOW_ON)
	float3 worldPos : TEXCOORD3;
#endif
#ifdef TERRAIN
	half2 tc_Control : TEXCOORD4;
	half2 uv1 : TEXCOORD5;
#endif
#ifdef TERRAIN_FIRSTPASS
	half2 uv3 : TEXCOORD6;
	half2 uv4 : TEXCOORD7;
#endif

#ifdef VC
	fixed4 color : COLOR;
#endif
};

#endif //SCENE_INCLUDED