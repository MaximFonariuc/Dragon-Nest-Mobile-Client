Shader "Custom/Editor/UVPreview"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[Toggle(ENABLE_UV2)] _UV2 ("UV2?", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature ENABLE_UV2
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(float4(v.uv.x, 0, v.uv.y, 0));
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.uv1, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
#ifdef ENABLE_UV2
				fixed4 col = tex2D(_MainTex, i.uv1);
#else
				fixed4 col = tex2D(_MainTex, i.uv);
#endif
				return col;
			}
			ENDCG
		}
	}
}
