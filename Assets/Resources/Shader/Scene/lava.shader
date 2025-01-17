Shader "Custom/Scene/lava"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_AddTex ("Texture", 2D) = "black" {}
		_color0("",Color)=(1,0,0,1)
		_color1("",Color)=(0.43,0.2,0,1)
		_color2("",Color)=(0.39,0,0.09,1)
		_DirVec("",vector)=(1,1,0.8,0.2)
		_ParaVec("",vector)=(1,0.95,1,0.2)
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
				float2 AddT_uv : TEXCOORD2;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _AddTex;
			float4 _AddTex_ST;
			float4 _DirVec;
			fixed4 _color0;
			fixed4 _color1;
			fixed4 _color2;
			float4 _ParaVec;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.AddT_uv = TRANSFORM_TEX(v.uv, _AddTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				half time=_Time.x*_ParaVec.x;
				fixed4 col0 = tex2D(_MainTex, i.uv+time*_DirVec.xy)*_ParaVec.y;
				fixed4 col1 = tex2D(_MainTex, i.uv*0.75+time*_DirVec.zw)*_ParaVec.y;

				fixed4 col2 = tex2D(_AddTex, i.AddT_uv+time*_ParaVec.zw);
				fixed4 c=lerp(_color2,_color0,pow(col0*col1*2,2));

				fixed4 c1=pow(col0*col1*2+col2,4)*_color1;
				fixed4 col=c+c1;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return  col;
			}
			ENDCG
		}
	}
}
