// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


#ifndef SCENE_INCLUDED
#define SCENE_INCLUDED

#include "UnityCG.cginc"

#ifdef ADDCOLOR
fixed4 _Color;
#endif
sampler2D _MainTex;

#ifdef MASKTEX
sampler2D _Mask;
#endif

#ifdef ANIM
float4 _PannerPara;
#endif

#ifdef TERRAIN
float4 _LightMap_ST;
sampler2D _TerrainLightMap;
sampler2D _Control;
float4 _Splat0_ST;
float4 _Splat1_ST;
sampler2D _Splat0, _Splat1;

#ifdef TERRAIN_FIRSTPASS
float4 _Splat2_ST;
float4 _Splat3_ST;
sampler2D _Splat2, _Splat3;
#endif

#endif


float4x4 custom_World2Shadow;
sampler2D _CustomShadowMapTexture;
float shadowScale;
float _windSpeed;

v2f vert(appdata_t v)
{
	v2f o = (v2f)0;

#ifdef ANIM
	float4 wpos = mul(unity_ObjectToWorld, v.vertex);
	float mask = saturate(v.uv.y)*0.1;
	float M_x = _windSpeed*mask*_PannerPara.x;
	float M_z =_windSpeed*mask*_PannerPara.y;
	o.vertex = mul(UNITY_MATRIX_VP, wpos + float4(M_x, 0, M_z, 0));
#else
	o.vertex = UnityObjectToClipPos(v.vertex);	
#endif

#ifdef TERRAIN
	o.tc_Control = v.uv;
	o.uv = TRANSFORM_TEX(v.uv, _Splat0);
	o.uv1 = TRANSFORM_TEX(v.uv, _Splat1);
	#ifdef TERRAIN_FIRSTPASS
		o.uv3 = TRANSFORM_TEX(v.uv, _Splat2);
		o.uv4 = TRANSFORM_TEX(v.uv, _Splat3);
	#endif//TERRAIN_FIRSTPASS
#else//TERRAIN
	o.uv = v.uv;
#ifdef UV2
	o.uv2 = v.uv2;
#endif//UV2
#endif//TERRAIN
		
#ifdef VC
	o.color = v.color;
#endif

#ifdef LM
#ifdef INPUTLIGHTMAP
	o.uv2.xy = v.uv.xy * _LightMap_ST.xy + _LightMap_ST.zw;
#else
	o.uv2.xy = v.uv2.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
#endif
#if defined(WP)|defined(CUSTOM_SHADOW_ON)
	o.worldPos = mul(unity_ObjectToWorld,float4(v.vertex.xyz, 1.0)).xyz;
#endif

	UNITY_TRANSFER_FOG(o, o.vertex);
	return o;
}

fixed4 frag(v2f i) : SV_Target
{
#ifdef MASKTEX
	fixed4 mask = tex2D(_Mask, i.uv);
#else
	fixed4 mask = fixed4(1,1,1,1);
#endif
#ifdef CUTOUT
	clip(mask.r - 0.5);
#endif

#ifdef TERRAIN
	half4 splat_control = tex2D(_Control, i.tc_Control);
	half weight = dot(splat_control, half4(1, 1, 1, 1));
	fixed4 mixedDiffuse = fixed4(0,0,0,0);

	mixedDiffuse += splat_control.r * tex2D(_Splat0, i.uv);
	mixedDiffuse += splat_control.g * tex2D(_Splat1, i.uv1);
#ifdef TERRAIN_FIRSTPASS
	mixedDiffuse += splat_control.b * tex2D(_Splat2, i.uv3);
	mixedDiffuse += splat_control.a * tex2D(_Splat3, i.uv4);
#endif

	fixed4 col = fixed4(mixedDiffuse.rgb, weight);
#else
	fixed4 col = tex2D(_MainTex, i.uv);
#endif
	

#ifdef ADDCOLOR
	col.rgb *= _Color.rgb;
#endif

#ifdef LM
	#ifdef INPUTLIGHTMAP
		col.rgb *= DecodeLightmap(tex2D(_TerrainLightMap, i.uv2.xy)).rgb;
	#else
		col.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2.xy)).rgb;
	#endif
	
#endif

#ifdef CUSTOM_SHADOW_ON
	float4 shadow = mul(custom_World2Shadow, float4(i.worldPos.xyz, 1));
	float2 coord = float2(shadow.xy / shadow.w);
	float depth = 1 - step(SAMPLE_DEPTH_TEXTURE(_CustomShadowMapTexture, coord.xy + float2(0.15, 0.0)), 0.99)*(col.r+ col.g+ col.b)*shadowScale;
	col.rgb *= depth;
#endif

#ifdef EMISSION
	col.rgb = EmissionColor(i,col, mask);
#endif //EMISSION
	col.a = mask.r + 0.01;

	UNITY_APPLY_FOG(i.fogCoord, col);

	return col;
}

#endif //SCENE_INCLUDED