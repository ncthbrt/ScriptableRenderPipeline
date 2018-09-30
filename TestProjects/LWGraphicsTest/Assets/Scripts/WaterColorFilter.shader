Shader "PostProcessing/WaterColorFilter" {
    HLSLINCLUDE

        #include "PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_WobbTex, sampler_WobbTex);
        TEXTURE2D_SAMPLER2D(_PaperTex, sampler_PaperTex);
        
		uniform float _WobbScale;
        uniform float _WobbPower;
        uniform float _EdgeSize;
        uniform float _EdgePower;        
        uniform float _PaperScale;
        uniform float _PaperPower;
        uniform float4 _MainTex_TexelSize;

        float4 ColorMod(float4 c, float d) {
            return (c - (c - c * c) * (d - 1));
        }
        
        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));
            color.rgb = luminance.xxx;
            return color;
        }

        
        float4 FragDefault(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);            
            float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));
            return luminance.xxxx;
        }

        float4 FragEdgeDarkening(VaryingsDefault i): SV_Target {
            float2 uv_offset = _EdgeSize * _MainTex_TexelSize.xy;
            float4 src_l = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, i.texcoord + float2(-uv_offset.x, 0));
            float4 src_r = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, i.texcoord + float2(+uv_offset.x, 0));
            float4 src_b = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, i.texcoord + float2(0, -uv_offset.y));
            float4 src_t = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, i.texcoord + float2(0, +uv_offset.y));
            float4 src = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            float4 grad = abs(src_r - src_l) + abs(src_b - src_t);
            float intens = saturate(0.333 * (grad.x + grad.y + grad.z));
            float d = _EdgePower * intens + 1;
            return ColorMod(src, d);
        }

        struct v2f_wobb {
            float2 texcoord_Main : TEXCOORD0;
            float2 texcoord_Wobb : TEXCOORD1;
            float4 vertex : SV_POSITION;
        };

        v2f_wobb VertWobb(AttributesDefault v) {
            float aspect = _ScreenParams.x / _ScreenParams.y;

            v2f_wobb o;
            o.vertex = float4(v.vertex.xy, 0.0, 1.0);                     
            o.texcoord_Main = TransformTriangleVertexToUV(v.vertex.xy);
            #if UNITY_UV_STARTS_AT_TOP
                o.texcoord_Main = o.texcoord_Main * float2(1.0, -1.0) + float2(0.0, 1.0);
            #endif
            
            o.texcoord_Wobb = o.texcoord_Main * float2(aspect, 1) * _WobbScale;
            return o;
        }

        float4 FragWobbb(v2f_wobb i) : SV_Target {
            // float4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord_Main);
            float2 wobb = SAMPLE_TEXTURE2D(_WobbTex, sampler_WobbTex, i.texcoord_Wobb).wy * 2 - 1;
            float4 src = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord_Main + wobb * _WobbPower);
            return src;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        // Wobbing
        Pass
        {
            HLSLPROGRAM
     

                #pragma vertex VertWobb
                #pragma fragment FragWobbb

            ENDHLSL
        }
        // Edge Darkening
        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment FragEdgeDarkening
            ENDHLSL
        }

        // Paper Layer
        // Pass 
        // {
        //     HLSLPROGRAM
        //         #pragma vertex VertDefault
        //         #pragma fragment Frag
        //     ENDHLSL
        // }
    }
}
	
//     HLSLINCLUDE
	
// 		#include "PostProcessing/Shaders/StdLib.hlsl"

// 		struct v2f {
// 			float4 pos : SV_POSITION;
// 			float2 uv[5] : TEXCOORD0;
// 		};
	
// 		struct v2fd {
// 			float4 pos : SV_POSITION;
// 			float2 uv[2] : TEXCOORD0;
// 		};
    
// 		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
// 		// TEXTURE2D_SAMPLER2D(_WobbTex, sampler_MainTex);


// 	// Properties {
// 	// 	_MainTex ("Texture", 2D) = "white" {}
// 	// 	_WobbTex ("Wobbing", 2D) = "grey" {}
// 	// 	_WobbScale ("Wob Tex Scale", Float) = 1
// 	// 	_WobbPower ("Wobbing Power", Float) = 1
// 	// 	_EdgeSize ("Edge Size", Float) = 1
// 	// 	_EdgePower ("Edge Power", Float) = 1
// 	// 	_PaperTex ("Paper", 2D) = "grey" {}
// 	// 	_PaperScale ("Paper Scale", Float) = 1
// 	// 	_PaperPower ("Paper Power", Float) = 1
// 	// }
// 	SubShader {
// 		Cull Off ZWrite Off ZTest Always

// 		CGINCLUDE			
// 			#include "UnityCG.cginc"

// 			struct appdata {
// 				float4 vertex : POSITION;
// 				float2 uv : TEXCOORD0;
// 			};
// 			struct v2f {
// 				float2 uv_Main : TEXCOORD0;
// 				float4 vertex : SV_POSITION;
// 			};

// 			sampler2D _MainTex;
// 			float4 _MainTex_TexelSize;

// 			float4 ColorMod(float4 c, float d) {
// 				return c - (c - c * c) * (d - 1);
// 			}

// 			v2f vert(appdata v) {
// 				v2f o;
// 				o.vertex = UnityObjectToClipPos(v.vertex);
// 				o.uv_Main = v.uv;
// 				return o;
// 			}
// 		ENDCG

// 		// Wobbing
// 		Pass {
// 			CGPROGRAM
// 			#pragma vertex vert_wobb
// 			#pragma fragment frag

// 			struct v2f_wobb {
// 				float2 uv_Main : TEXCOORD0;
// 				float2 uv_Wobb : TEXCOORD1;
// 				float4 vertex : SV_POSITION;
// 			};

// 			sampler2D _WobbTex;
// 			float _WobbScale;
// 			float _WobbPower;

// 			v2f_wobb vert_wobb(appdata v) {
// 				float aspect = _ScreenParams.x / _ScreenParams.y;

// 				v2f_wobb o;
// 				o.vertex = UnityObjectToClipPos(v.vertex);
// 				o.uv_Main = v.uv;
// 				o.uv_Wobb = v.uv * float2(aspect, 1) * _WobbScale;
// 				return o;
// 			}
			
// 			fixed4 frag(v2f_wobb i) : SV_Target {
// 				fixed2 wobb = tex2D(_WobbTex, i.uv_Wobb).wy * 2 - 1;
// 				fixed4 src = tex2D(_MainTex, i.uv_Main + wobb * _WobbPower);
// 				return src;
// 			}
// 			ENDCG
// 		}

// 		// Edge Darkening
// 		Pass {
// 			CGPROGRAM
// 			#pragma vertex vert
// 			#pragma fragment frag

// 			float _EdgeSize;
// 			float _EdgePower;

// 			fixed4 frag(v2f i) : SV_Target {
// 				float2 uv_offset = _MainTex_TexelSize.xy * _EdgeSize;
// 				fixed4 src_l = tex2D(_MainTex, i.uv_Main + float2(-uv_offset.x, 0));
// 				fixed4 src_r = tex2D(_MainTex, i.uv_Main + float2(+uv_offset.x, 0));
// 				fixed4 src_b = tex2D(_MainTex, i.uv_Main + float2(           0, -uv_offset.y));
// 				fixed4 src_t = tex2D(_MainTex, i.uv_Main + float2(           0, +uv_offset.y));
// 				fixed4 src = tex2D(_MainTex, i.uv_Main);

// 				fixed4 grad = abs(src_r - src_l) + abs(src_b - src_t);
// 				float intens = saturate(0.333 * (grad.x + grad.y + grad.z));
// 				float d = _EdgePower * intens + 1;
// 				return ColorMod(src, d);
// 			}
// 			ENDCG
// 		}

// 		// Paper Layer
// 		Pass {
// 			CGPROGRAM
// 			#pragma vertex vert_paper
// 			#pragma fragment frag

// 			struct v2f_paper {
// 				float2 uv_Main : TEXCOORD0;
// 				float2 uv_Paper : TEXCOORD1;
// 				float4 vertex : SV_POSITION;
// 			};

// 			sampler2D _PaperTex;
// 			float _PaperScale;
// 			float _PaperPower;

// 			v2f_paper vert_paper(appdata v) {
// 				float aspect = _ScreenParams.x / _ScreenParams.y;

// 				v2f_paper o;
// 				o.vertex = UnityObjectToClipPos(v.vertex);
// 				o.uv_Main = v.uv;
// 				o.uv_Paper = v.uv * float2(aspect, 1) * _PaperScale;
// 				return o;
// 			}
			
// 			fixed4 frag(v2f_paper i) : SV_Target {
// 				fixed4 src = tex2D(_MainTex, i.uv_Main);
// 				fixed paper = tex2D(_PaperTex, i.uv_Paper).x;

// 				float d = _PaperPower * (paper - 0.5) + 1;
// 				return ColorMod(src, d);
// 			}

// 			ENDCG
// 		}
// 	}
// }
