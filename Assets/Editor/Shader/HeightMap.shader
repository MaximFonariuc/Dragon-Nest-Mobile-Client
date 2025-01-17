// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Editor/Heightmap" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "black" {}
	}
		Category{
		Tags{ "RenderType" = "Opaque" }
		Cull off Lighting Off ZWrite Off Fog{ Mode Off }
		ColorMask RGBA
		SubShader{
		Pass{

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0
		#include "UnityCG.cginc"


		struct appdata_t {
		half4 vertex : POSITION;
		half2 texcoord : TEXCOORD0;
	};

	struct v2f {
		half4 vertex : SV_POSITION;
		half2 uv : TEXCOORD0;
	};

	sampler2D _MainTex;
	half4 _MainTex_ST;
	v2f vert(appdata_t v)
	{
		v2f o = (v2f)0;
		float4 pos = v.vertex;
		float4 height = tex2Dlod(_MainTex,float4(v.texcoord.xy, 0, 0));
		pos.z = height;
		o.vertex = UnityObjectToClipPos(pos);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		return tex2D(_MainTex, i.uv);
	}
		ENDCG
	}
	}
	}
}
