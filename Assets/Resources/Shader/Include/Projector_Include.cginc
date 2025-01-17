// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#ifndef PROJECTOR_INCLUDED
#define PROJECTOR_INCLUDED

#include "UnityCG.cginc"



struct appdata_t 
{
	half4 vertex : POSITION;
	half2 texcoord  : TEXCOORD0;
};

struct v2f 
{
	half4 vertex : POSITION;
	half2 uv : TEXCOORD0;
};

float4x4 unity_Projector;

v2f vert(appdata_t v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	half4 shadowUV = mul(unity_Projector, float4(v.vertex.xyz,1));
	o.uv = shadowUV.xy - half2(0.5, 0.5);
	return o;
}

#endif //PROJECTOR_INCLUDED