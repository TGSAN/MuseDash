Shader "Blur Behind/UI" {
	Properties{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

	SubShader {
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Stencil {
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass {
			CGPROGRAM
			#pragma multi_compile _ BLUR_BEHIND_SET
			#if UNITY_VERSION >= 530
			#pragma multi_compile _ UNITY_UI_ALPHACLIP
			#endif
			#pragma vertex vert
			#pragma fragment frag

			#ifdef BLUR_BEHIND_SET
			#include "UnityCG.cginc"

			#if UNITY_VERSION >= 520
			inline float UnityGet2DClipping(in float2 position, in float4 clipRect)
			{
				float2 inside = step(clipRect.xy, position.xy) * step(position.xy, clipRect.zw);
				return inside.x * inside.y;
			}
			#endif



			struct appdata_t {
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
				half2 viewPos	: TEXCOORD1;
				#if UNITY_VERSION >= 520
				float4 worldPosition : TEXCOORD2;
				#endif
			};



			// Default UI variables

			fixed4 _Color;

			#if UNITY_VERSION >= 520
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;

			#if UNITY_VERSION < 530
			bool _UseClipRect;
			bool _UseAlphaClip;
			#endif

			#endif // UNITY_VERSION >= 520

			sampler2D _MainTex;



			// Blur Behind variables

			float4 _BlurBehindRect;
			sampler2D _BlurBehindTex;



			v2f vert(appdata_t IN) {
				v2f OUT;
				#if UNITY_VERSION >= 520
				OUT.worldPosition = IN.vertex;
				#endif
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1, 1);
				#endif
				OUT.viewPos = OUT.vertex.xy / OUT.vertex.w;
				if (_ProjectionParams.x < 0) { // Is view projection flipped?
					OUT.viewPos.y = -OUT.viewPos.y;
				}
				OUT.viewPos = (OUT.viewPos + 1.0) * 0.5;
				OUT.viewPos = (OUT.viewPos - _BlurBehindRect.xy) / _BlurBehindRect.zw;
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target {
				half4 color = tex2D(_MainTex, IN.texcoord);
				#if UNITY_VERSION >= 520
				color.a += _TextureSampleAdd.a;
				#endif
				color.a *= IN.color.a;

				#if UNITY_VERSION >= 530
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				#ifdef UNITY_UI_ALPHACLIP
				clip(color.a - 0.001);
				#endif

				#elif UNITY_VERSION >= 520
				if (_UseClipRect) {
					color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				}
				if (_UseAlphaClip) {
					clip(color.a - 0.001);
				}

				#else // UNITY_VERSION < 520
				clip(color.a - 0.01);
				#endif

				color.rgb = tex2D(_BlurBehindTex, IN.viewPos).rgb;
				return color;
			}

			

			#else // BLUR_BEHIND_SET // if no Blur Behind component is active discard all
			float4 vert() : SV_POSITION {
				return 0;
			}

			fixed4 frag() : SV_Target {
				discard;
				return 0;
			}
			#endif // BLUR_BEHIND_SET

			ENDCG
		}
	}
}
