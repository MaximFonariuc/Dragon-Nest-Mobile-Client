// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


#ifndef COMMONBASIC_INCLUDED
#define COMMONBASIC_INCLUDED 

#include "CommonHead_Include.cginc"

//#ifdef LIGHTON
uniform fixed4 lightColor;
uniform float4 lightDir;
//#endif //LIGHTON
uniform fixed uirim;
uniform float4 cameraPos;
v2f vert(a2v v) 
{  
	v2f o = (v2f)0;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord;
#ifdef UV2
	o.uv1 = VertexUV2(v);
#endif//UV2

#ifdef SKINTEX
	SkinUVMask(o);
#endif//SKINTEX

#ifndef NONORMAL
	//1.calc normal
	o.normal = normalize(mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL));

#ifdef VIEWDIR
		o.viewDir = normalize( _WorldSpaceCameraPos - mul(unity_ObjectToWorld,float4(v.vertex.xyz, 1.0)).xyz);	
		#ifdef REFLECTUV
			o.refluv =  reflect(-o.viewDir, o.normal);
		#endif //REFLECTUV
#endif //VIEWDIR 

	#ifdef MATCAP
			half2 capCoord;
			capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz, v.normal);
			capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz, v.normal);
			o.cap = capCoord * 0.5 + 0.5;
	#endif//MATCAP
#endif//NONORMAL

#ifdef LIGHTON
	o.vertexLighting = fixed3(0,0,0);
	//SHLight
	#ifdef SHLIGHTON
		o.vertexLighting = saturate(ShadeSH9 (normalize(float4(o.normal, 1.0))));
	#endif	//SHLIGHTON
	//vertex light 
	#ifdef VERTEXLIGHTON
		fixed ambientLighting = Luminance(UNITY_LIGHTMODEL_AMBIENT.rgb)*1.2;//UNITY_LIGHTMODEL_AMBIENT.rgb;
		fixed3 diffuseReflection = lightColor.rgb * max(0.001, dot(o.normal, lightDir.xyz));
		o.vertexLighting += fixed3(ambientLighting,ambientLighting,ambientLighting) + diffuseReflection;
	#endif //VERTEXLIGHTON 
#endif //LIGHTON
	
	return o;  
} 

#ifdef RIMLIGHT
fixed4 _RimColor;
#endif//RIMLIGHT

#ifdef BLINNPHONG
fixed4 _SpecColor;
#endif //BLINNPHONG

#if defined(GLASS)|defined(METALREFL)|defined(MATCAP)
#include "../Include/Effect_Include.cginc"
#endif //GLASS|METALREFL|MATCAP

#ifdef BLINK
fixed4 _Color;
#endif


#ifdef DEFAULTBASECOLOR
sampler2D _MainTex;
fixed4 BasicColor(in v2f i, inout fixed4 maskColor)
{
#ifdef ENABLE_SPLIT
	fixed4 c = Combine2Tex(_MainTex,_MainTex1,i.uv,_UVScale,_UVRange);
#else
	fixed4 c = tex2D(_MainTex, i.uv);
#endif//ENABLE_SPLIT

#ifdef MASKTEX
	#ifdef ENABLE_SPLIT
		maskColor = Combine2Tex(_Mask,_Mask1,i.uv,_UVScale,_UVRange);
	#else
		maskColor = tex2D(_Mask, i.uv);
	#endif//ENABLE_SPLIT
#endif//MASKTEX

#ifdef CUTOUT
	#ifdef CUTOUTG
		c.a = maskColor.g;
	#else
		c.a = maskColor.r;
	#endif
#else
	c.a = 1;
#endif//CUTOUT

	return c;
}
#endif

fixed4 frag(v2f i) : COLOR 
{
#ifdef VIEWDIR	
		if(uirim>0.5)
		{
			i.viewDir = half3(0,0,-1);
#ifdef REFLECTUV
			i.refluv = reflect(-i.viewDir, i.normal);
#endif //REFLECTUV
		}

#endif //VIEWDIR 

	fixed4 maskColor = fixed4(1,1,1,1);
	fixed4 basicColor = BasicColor(i, maskColor);	
	
#ifdef CUTOUT
#ifdef SMALLCUTOUT
	clip(basicColor.a - 0.15);
#else
	clip(basicColor.a - 0.5);
#endif
	
#endif //CUTOUT 

#ifdef BLINK
	fixed blink = 1 - _Color.w;
	basicColor.rgb = basicColor.rgb + fixed3(blink, blink,blink);
#endif//BLINK 

	fixed4 c = basicColor;

#if defined(GLASS)|defined(METALREFL)|defined(MATCAP)
	Effect(i, maskColor, c);
#endif //MATCAP
	
#ifdef LIGHTON 	
	#ifdef VERTEXLIGHTON

		#ifndef ORIGINAL_LIGHT
			c.rgb = c.rgb*_LightArgs.x+c.rgb*_LightArgs.y*i.vertexLighting.rgb;
		#else
			c.rgb *= i.vertexLighting.rgb;
		#endif//ORIGINAL_LIGHT
		#ifdef TESTLIGHTING
			c.rgb = basicColor.rgb;
		#endif //TESTLIGHTING
	#else//VERTEXLIGHTON
		#ifdef LAMBERT	
			//lighting	
			fixed3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb;
			fixed3 lightDirection = lightDir.xyz;
			float nDl = max(0.0, dot(i.normal, lightDir.xyz));
			fixed3 diffuseReflection = lightColor.rgb * nDl*1.8;
#ifndef ORIGINAL_LIGHT
			c.rgb *= _LightArgs.x + _LightArgs.y*(ambientLighting + diffuseReflection + i.vertexLighting);
#else
			c.rgb *= ambientLighting + diffuseReflection + i.vertexLighting;
#endif
			
			#ifdef TESTLIGHTING
				c.rgb = basicColor.rgb;
			#endif //TESTLIGHTING
		#else//LAMBERT
			#ifdef BLINNPHONG
				float3 lightDirection = lightDir.xyz;
				half3 h = normalize (lightDirection + i.viewDir);	
				fixed diff = max (0, dot (i.worldNormal, lightDirection));
				float nh = max (0, dot (i.worldNormal, h));
#ifndef ORIGINAL_LIGHT
				float spec = pow (nh, _LightArgs.x*128.0) * _LightArgs.y;
#else
				float spec = pow (nh, 128.0);
#endif
				#ifdef TESTLIGHTING
					c.rgb = (lightColor0.rgb * diff + lightColor0.rgb * _SpecColor.rgb * spec) * 2;
				#else
					c.rgb = (c.rgb*_LightArgs.w + (1-_LightArgs.w)*c.rgb *lightColor0.rgb * diff + lightColor0.rgb * _SpecColor.rgb * spec) * 2;
				#endif//TESTLIGHTING
			#endif //BLINNPHONG
		#endif //LAMBERT
	#endif //VERTEXLIGHTON

#endif //LIGHTON

#ifdef RIMLIGHT
	half rim = 1-saturate(dot(i.viewDir, i.normal));
	c.rgb += saturate(_RimColor.rgb * pow(rim, _LightArgs.w));
#endif//RIMLIGHT

#ifdef EMISSION
	fixed4 e = EmissionColor(i,c, maskColor);
	c = c + e;
#endif //EMISSION

	return c;  
} 
#endif //COMMONBASIC_INCLUDED