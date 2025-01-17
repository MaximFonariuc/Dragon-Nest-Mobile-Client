// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Scene/SeaOfFlowers"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Mask ("Mask (R)", 2D) = "white" {}
		_LightMap ("LightMap", 2D) = "white" {}
		_PannerPara("X-位移距离 Y-速度 Z-UV缩放 W-晃动间隔",vector)=(1,1,1,1)
		_LMPara("X-Terrain大小 Y-Unused Z-加 W-乘",vector)=(50,1,0,1)
	}
	SubShader
	{
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"  }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			sampler2D _Mask;
			sampler2D _LightMap;
			float4 _MainTex_ST;
			float4 _PannerPara;
			float4 _LMPara;
			
			v2f vert (appdata v)
			{
				v2f o;
				//o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos =  mul(unity_ObjectToWorld,v.vertex);
				float4 WP = o.worldPos;
				float M_x = sin((_Time.y*_PannerPara.y*_PannerPara.w + length(WP.xyz))/max(_PannerPara.w,0.1))*fmod(v.uv.y*_PannerPara.z,1)*0.1*_PannerPara.x;
				float M_z = sin((_Time.y*_PannerPara.y*0.75*_PannerPara.w + length(WP.xyz))/max(_PannerPara.w,0.1))*fmod(v.uv.y*_PannerPara.z,1)*0.1*_PannerPara.x;
				o.vertex = mul(UNITY_MATRIX_VP, WP + float4(M_x,0,M_z, 0));
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 lightmap01 = fixed4(DecodeLightmap(tex2D(_LightMap, (i.worldPos.xz)/_LMPara.x)),1);
				fixed mask = tex2D(_Mask, i.uv).r;
				clip(mask-0.5);
				fixed4 c=(lightmap01+_LMPara.z)*_LMPara.w*col;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG
		}
	}
}
