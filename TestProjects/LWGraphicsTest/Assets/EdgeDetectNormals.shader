Shader "PostProcessing/EdgeDetectNormals" {
	
    HLSLINCLUDE
	
		#include "PostProcessing/Shaders/StdLib.hlsl"

		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv[5] : TEXCOORD0;
		};
	
		struct v2fd {
			float4 pos : SV_POSITION;
			float2 uv[2] : TEXCOORD0;
		};

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
		uniform float4 _MainTex_TexelSize;
		half4 _MainTex_ST;

		TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
		half4 _CameraDepthTexture_ST;
				
		uniform half4 _BgColor;
		uniform half4 _FgColor;
		uniform half4 _SobelColor;
		uniform half _SampleDistance;
		uniform float _Exponent;
		
	 
		v2fd vertD( AttributesDefault v )
		{
			v2fd o;
			o.pos = float4(v.vertex.xy, 0.0, 1.0);
		
			float2 uv = TransformTriangleVertexToUV(v.vertex.xy);

			#if UNITY_UV_STARTS_AT_TOP
			uv = uv * float2(1.0,-1.0) + float2(0.0, 1.0);
			#endif
	
			o.uv[0] = UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST);

			o.uv[1] = uv;
		
			return o;
		}
		
		inline float LinearOrthographicDepth(float z)
		{    	
    	z *= _ZBufferParams.x;
    	return (1.0 - z) / (_ZBufferParams.y);
		}

		// pretty much also just a sobel filter, except for that edges "outside" the silhouette get discarded
		//  which makes it compatible with other depth based post fx
		float4 fragD(v2fd i) : SV_Target 
		{
			// inspired by borderlands implementation of popular "sobel filter"
			float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1], _CameraDepthTexture_ST));
			float centerDepth = LinearOrthographicDepth(d);
			float4 depthsDiag;
			float4 depthsAxis;

			float2 uvDist = _SampleDistance * _MainTex_TexelSize.xy;

			depthsDiag.x = LinearOrthographicDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]+uvDist, _CameraDepthTexture_ST))); // TR
			depthsDiag.y = LinearOrthographicDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]+uvDist*float2(-1,1), _CameraDepthTexture_ST))); // TL
			depthsDiag.z = LinearOrthographicDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]-uvDist*float2(-1,1), _CameraDepthTexture_ST))); // BR
			depthsDiag.w = LinearOrthographicDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]-uvDist, _CameraDepthTexture_ST))); // BL

			depthsAxis.x = LinearOrthographicDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]+uvDist*float2(0,1), _CameraDepthTexture_ST))); // T
			depthsAxis.y = LinearOrthographicDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]-uvDist*float2(1,0), _CameraDepthTexture_ST))); // L
			depthsAxis.z = LinearOrthographicDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]+uvDist*float2(1,0), _CameraDepthTexture_ST))); // R
			depthsAxis.w = LinearOrthographicDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]-uvDist*float2(0,1), _CameraDepthTexture_ST))); // B

			// make it work nicely with depth based image effects such as depth of field:
			depthsDiag =  max(depthsDiag, centerDepth.xxxx);
			depthsAxis =  max(depthsAxis, centerDepth.xxxx);
			
			depthsDiag -= centerDepth;
			depthsAxis /= centerDepth;

			const float4 HorizDiagCoeff = float4(1,1,-1,-1);
			const float4 VertDiagCoeff = float4(-1,1,-1,1);
			const float4 HorizAxisCoeff = float4(1,0,0,-1);
			const float4 VertAxisCoeff = float4(0,1,-1,0);

			float4 SobelH = depthsDiag * HorizDiagCoeff + depthsAxis * HorizAxisCoeff;
			float4 SobelV = depthsDiag * VertDiagCoeff + depthsAxis * VertAxisCoeff;

			float SobelX = dot(SobelH, float4(1,1,1,1));
			float SobelY = dot(SobelV, float4(1,1,1,1));
			// float Sobel = sqrt(SobelX * SobelX + SobelY * SobelY);
			float Sobel = sqrt(SobelX * SobelX + SobelY * SobelY);
			float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv[0]);
			Sobel = 1.0-pow(saturate(Sobel), _Exponent);
			return lerp(_BgColor, lerp(_SobelColor, color.g * _FgColor, Sobel), color.r);
		}
    ENDHLSL

Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off
		HLSLPROGRAM
		#pragma target 3.0   
		#pragma vertex vertD
		#pragma fragment fragD
		ENDHLSL
  }
}

Fallback off
	
} // shader
