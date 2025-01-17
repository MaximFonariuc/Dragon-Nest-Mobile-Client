// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Scene/WaterWarpPlaneBlend_UV2" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "gray" {}
		_BumpTex ("Bump (A)", 2D) = "bump" {}
		_Cube ("Cubemap", CUBE) = "" {}
		_Color("Water Color",Color)=(0.5,0.75,0.5,1)
		_vector("x:U_Speed  y:V_Speed   z:WarpScale   w:ReflWarpScale",vector)=(3,1,0.12,3)
		_WaterVec("x:turbidity y:BottomLumi z:ReflectScale w:AlphaScale",vector)=(1.5,2,0.3,1.65)
	}

	SubShader 
	{
		Tags { "RenderType"="Transparent""Queue"="Transparent" }
		LOD 100
		Pass 
		{
			Blend  SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma multi_compile_fog 
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t 
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				fixed4 color :COLOR;
			};

			struct v2f 
			{
				half4 vertex : SV_POSITION;
				half2 uv : TEXCOORD0;
				half2 uvBump : TEXCOORD1;
				half3 view : TEXCOORD2;
				fixed4 color : TEXCOORD3;
				UNITY_FOG_COORDS(4)
			};
			sampler2D _MainTex;
			sampler2D _BumpTex;
			samplerCUBE _Cube; 
			half4 _MainTex_ST;
			half4 _BumpTex_ST;
			half4 _vector;
			half4 _WaterVec;
			fixed4 _Color;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.uvBump = TRANSFORM_TEX(v.texcoord1,_BumpTex);
				o.view= normalize( WorldSpaceViewDir( v.vertex)) ;
				o.color=v.color; 
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}

			float4 pow4(float4 x)
			{
				return x*x*x*x;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				half3 normal=half3(0,1,0);
				fixed4 bump0 = tex2D (_BumpTex, i.uvBump+float2(_vector.x,_vector.y)*_Time.x)*2-1; 
				fixed4 bump1 = tex2D (_BumpTex, i.uvBump*0.5+float2(_vector.x,_vector.y)*_Time.x*float2(0.15,-0.5))*2-1; 
				fixed4 bump=(bump0+bump1)*0.5;
				fixed4 MainT_Warp = tex2D (_MainTex, i.uv+bump.xy*_vector.z*0.2)*0.8; 
				fixed4 col=lerp(MainT_Warp*_Color*_WaterVec.y,lerp(_Color,pow4(_Color),min(1,i.color.a)),min(1,i.color.a*_WaterVec.x));
				half3 refluv=reflect(-i.view,normalize( normal+float3(bump.x,1,bump.y)*_vector.z*_vector.w));
				fixed4 mc = lerp(col,texCUBE (_Cube, refluv),_WaterVec.z);
				fixed ndv=max(0.0001,dot(normalize(normal+float3(bump.x,1,bump.y)*_vector.z*_vector.w),i.view.xyz));
				fixed4 c=lerp(mc,col,min(0.5,ndv));
				c.a=i.color.a*_WaterVec.w;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG 
		}
	}
}
