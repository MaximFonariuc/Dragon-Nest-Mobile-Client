#ifndef UI_INCLUDED
#define UI_INCLUDED
	
half4 _ClipRange0 = half4(0.0, 0.0, 0.0, 0.0);
half4 _ClipArgs0 = half4(1.0, 1.0, 0.0, 1.0);
//half4 _ClipRange1 = half4(0.0, 0.0, 1.0, 1.0);
//half4 _ClipArgs1 = half4(1000.0, 1000.0, 0.0, 1.0);
//half4 _ClipRange2 = half4(0.0, 0.0, 1.0, 1.0);
//half4 _ClipArgs2 = half4(1000.0, 1000.0, 0.0, 1.0);

half2 Rotate(half2 v, half2 rot)
{
	half2 ret;
	ret.x = v.x * rot.y - v.y * rot.x;
	ret.y = v.x * rot.x + v.y * rot.y;
	return ret;
}

inline half2 Clip1(half2 xy)
{
	return xy * _ClipRange0.zw + _ClipRange0.xy;
}

//inline half2 Clip2(half2 xy)
//{
//	return Rotate(xy, _ClipArgs1.zw) * _ClipRange1.zw + _ClipRange1.xy;
//}
//
//inline half2 Clip3(half2 xy)
//{
//	return Rotate(xy, _ClipArgs2.zw) * _ClipRange2.zw + _ClipRange2.xy;
//}

inline fixed4 UISample(sampler2D tex, sampler2D mask, fixed4 color, half2 uv)
{
	fixed colMask = sign(color.r + color.g + color.b);
	fixed3 col = tex2D(tex, uv).rgb;
	half grey = Luminance(col.rgb);
	fixed4 final = fixed4(fixed3(grey, grey, grey)*(1 - colMask) + col*colMask*color.rgb, color.a);
	final.a *= tex2D(mask, uv).a;
	return final;
}

inline fixed4 UISample(sampler2D tex, fixed4 color, half2 uv)
{
	fixed colMask = sign(color.r + color.g + color.b);
	fixed4 col = tex2D(tex, uv);
	half grey = Luminance(col.rgb);
	return fixed4(fixed3(grey, grey, grey)*(1 - colMask) + col*colMask * color.rgb, color.a*col.a);
}

//inline fixed4 UISampleRGBA32(sampler2D tex, fixed4 color, half2 uv)
//{
//	fixed colMask = sign(color.r + color.g + color.b);
//	fixed4 col = tex2D(tex, uv);
//	half grey = Luminance(col.rgb);
//	return fixed4(fixed3(grey, grey, grey)*(1 - colMask) + col.rgb * colMask * color.rgb, col.a * color.a);
//}

inline fixed4 UISampleText(sampler2D tex, fixed4 color, half2 uv)
{
	fixed4 col = color;
	col.a *= tex2D(tex, uv).a;
	return col;
}
inline fixed4 UISampleFx(sampler2D tex, sampler2D mask, fixed4 color, half2 uv)
{
	fixed4 final = tex2D(tex, uv) * color;
	final.a *= tex2D(mask, uv).a;
	return final;
}
inline fixed4 UI1Clip(fixed4 final, half2 worldPos)
{
	half2 factor = (half2(1.0, 1.0) - abs(worldPos.xy)) * _ClipArgs0.xy;
	final.a *= clamp(min(factor.x, factor.y), 0.0, 1.0);
	return final;
}

//inline fixed4 UI2Clip(fixed4 final, half4 worldPos)
//{
//	half2 factor = (half2(1.0, 1.0) - abs(worldPos.xy)) * _ClipArgs0.xy;
//	half f = min(factor.x, factor.y);
//
//	// Second clip region
//	factor = (half2(1.0, 1.0) - abs(worldPos.zw)) * _ClipArgs1.xy;
//	f = min(f, min(factor.x, factor.y));
//
//	final.a *= clamp(f, 0.0, 1.0);
//	return final;
//}
//
//inline fixed4 UI3Clip(fixed4 final, half4 worldPos, half2 worldPos2)
//{
//	// First clip region
//	float2 factor = (float2(1.0, 1.0) - abs(worldPos.xy)) * _ClipArgs0.xy;
//	float f = min(factor.x, factor.y);
//
//	// Second clip region
//	factor = (half2(1.0, 1.0) - abs(worldPos.zw)) * _ClipArgs1.xy;
//	f = min(f, min(factor.x, factor.y));
//
//	// Third clip region
//	factor = (half2(1.0, 1.0) - abs(worldPos2)) * _ClipArgs2.xy;
//	f = min(f, min(factor.x, factor.y));
//
//	final.a *= clamp(f, 0.0, 1.0);
//	return final;
//}

inline fixed4 UI1ClipFade(fixed4 final, half2 worldPos)
{
	// Softness factor
	half2 factor = (half2(1.0, 1.0) - abs(worldPos.xy)) * _ClipArgs0.xy;

	half fade = clamp(min(factor.x, factor.y), 0.0, 1.0);
	final.a *= fade;
	final.rgb = lerp(half3(0.0, 0.0, 0.0), final.rgb, fade);
	return final;
}

float InRange(float2 uv, float4 ranges)
{
	float2 value = step(uv, ranges.xy) * step(ranges.zw, uv);
	return value.x * value.y;
}
//inline fixed4 UI2ClipFade(fixed4 final, half4 worldPos)
//{
//	half2 factor = (half2(1.0, 1.0) - abs(worldPos.xy)) * _ClipArgs0.xy;
//	half f = min(factor.x, factor.y);
//
//	// Second clip region
//	factor = (half2(1.0, 1.0) - abs(worldPos.zw)) * _ClipArgs1.xy;
//	f = min(f, min(factor.x, factor.y));
//
//	half fade = clamp(f, 0.0, 1.0);
//	final.a *= fade;
//	final.rgb = lerp(half3(0.0, 0.0, 0.0), final.rgb, fade);
//	return final;
//}
//
//inline fixed4 UI3ClipFade(fixed4 final, half4 worldPos, half2 worldPos2)
//{
//	// First clip region
//	half2 factor = (half2(1.0, 1.0) - abs(worldPos.xy)) * _ClipArgs0.xy;
//	half f = min(factor.x, factor.y);
//
//	// Second clip region
//	factor = (half2(1.0, 1.0) - abs(worldPos.zw)) * _ClipArgs1.xy;
//	f = min(f, min(factor.x, factor.y));
//
//	// Third clip region
//	factor = (half2(1.0, 1.0) - abs(worldPos2)) * _ClipArgs2.xy;
//	f = min(f, min(factor.x, factor.y));
//
//	half fade = clamp(f, 0.0, 1.0);
//	final.a *= fade;
//	final.rgb = lerp(half3(0.0, 0.0, 0.0), final.rgb, fade);
//	return final;
//}
#endif //UI_INCLUDED