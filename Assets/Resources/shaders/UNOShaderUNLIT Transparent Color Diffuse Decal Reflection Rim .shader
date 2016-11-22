// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//Version=1.6
Shader"UNOShader/_Library/UNLIT/UNOShaderUNLIT Transparent Color Diffuse Decal Reflection Rim "
{
	Properties
	{
		_BRDF("BRDF Shading", 2D) = "white" {}
		_BRDFBrightness ("BRDF Brightness", Range (0, 0)) = 0
		_Color ("Color (A)Opacity", Color) = (1,1,1,1)
		_MainTex ("Diffuse Texture", 2D) = "white" {}
		_MainTexOpacity ("Diffuse Opacity", Range (0, 1)) = 1
		_DecalColor ("Decal Tint", Color) = (1,1,1,1)
		_DecalTex ("Decal Texture (A)Opacity", 2D) = "black" {}
		_Cube ("Cubemap", Cube) = "white" {}
		_Metallic ("Metallic", Range(0, 1)) = 0
		_CubeOpacity ("Ref Opacity", Range (0, 1)) = 1
		_CubeFresnel ("Ref Fresnel", Range (0, 5)) = 1
		_CubeFresnel ("Ref Fresnel", Range (0, 5)) = 1
		_CubeIntensity ("Ref Intensity", Range (1, 3)) = 1
		_LightmapTex ("Lightmap Texture", 2D) = "gray" {}
		_RimColor ("Rim Color (A)Opacity", Color) = (1,1,1,1)
		_RimFresnel ("Rim Fresnel", Range (0, 5)) = 1
		_RimIntensity ("Rim Intensity", Range (1, 10)) = 1
		_Transparency ("Transparency", Range(0,1)) = 1
		_TransparencyFresnel ("Edge Fresnel", Range(0,5)) = 0
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		_MasksTex ("Masks", 2D) = "white" {}
		//--------------------- Xray Shader Features  --------------------------------
		 [HideInInspector] _XRAYEDGE("Xray edge", Float) = 0.0
		//---------------------  Shader Features  --------------------------------
		[HideInInspector] _lmUV1("Lightmap UV", Float) = 0.0
		[HideInInspector] _maskTex("Texture Masks", Float) = 0.0
		[HideInInspector] _mathPixel("Math", Float) = 0.0	
		[HideInInspector] _BASE("Shade Base", Float) = 0.0
		[HideInInspector] _AMBIENT("Ambient Light", Float) = 0.0
		[HideInInspector] _LDIR("Directional Light", Float) = 0.0
		[HideInInspector] _REFCUSTOM("Custom Reflection", Float) = 0.0
		[HideInInspector] _CUSTOMLIGHTMAP("Custom Lightmap", Float) = 0.0
		[HideInInspector] _CUSTOMSHADOW("Custom Shadow", Float) = 0.0
		[HideInInspector] _EDGETRANSPARENCY("Edge transparency", Float) = 0.0
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}
			Offset -1.0,0
			Blend SrcAlpha OneMinusSrcAlpha // --- not needed when doing cutout
		Pass
			{
			Name "ForwardBase"
			Tags
			{
				"RenderType" = "Transparent"
				"Queue" = "Transparent"
				"LightMode" = "ForwardBase"
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"
			#pragma multi_compile_fwdbase
			#pragma shader_feature lmUV1_ON lmUV1_OFF
			#pragma shader_feature REFCUSTOM_OFF REFCUSTOM_ON
			#pragma shader_feature maskTex_ON maskTex_OFF
			#pragma shader_feature mathPixel_ON mathPixel_OFF
			#pragma shader_feature BASE_UNLIT BASE_BRDF
			#pragma shader_feature AMBIENT_OFF AMBIENT_ON
			#pragma shader_feature LDIR_OFF LDIR_ON
			#pragma shader_feature CUSTOMLIGHTMAP_OFF CUSTOMLIGHTMAP_ON
			#pragma shader_feature CUSTOMSHADOW_OFF CUSTOMSHADOW_ON
			#pragma exclude_renderers d3d11_9x
			#pragma shader_feature NONE_EDGETRANSPARENCY NORMAL_EDGETRANSPARENCY INVERTED_EDGETRANSPARENCY
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

			fixed _Transparency;
			fixed _TransparencyFresnel;
			#if maskTex_ON
			sampler2D _MasksTex;
			half4 _MasksTex_ST;
			#endif
			sampler2D _BRDF;
			half4 _BRDF_ST;
			fixed _BRDFBrightness;
			fixed4 _Color;

			sampler2D _MainTex;
			half4 _MainTex_ST;
			fixed _MainTexOpacity;
			half4x4 _MainTexMatrix;

			fixed4 _DecalColor;
			sampler2D _DecalTex;
			half4 _DecalTex_ST;
			half4x4 _DecalTexMatrix;

			fixed _Metallic;
			samplerCUBE _Cube;
			fixed _CubeOpacity;
			fixed _CubeFresnel;
			fixed _CubeIntensity;

			half3 DecodeRGBM(half4 rgbm)
			{
			fixed MaxRange=8;
			return rgbm.rgb * (rgbm.a * MaxRange);
			}


			fixed4 _RimColor;
			fixed _RimFresnel;
			fixed _RimIntensity;

			#ifdef CUSTOMLIGHTMAP_ON
				sampler2D _LightmapTex;
				half4 _LightmapTex_ST;
			#endif

			fixed4 _UNOShaderShadowColor;
			struct customData
			{
				half4 vertex : POSITION;
				half3 normal : NORMAL;
				half4 tangent : TANGENT;
				fixed2 texcoord : TEXCOORD0;
				fixed2 texcoord1 : TEXCOORD1;
			};
			struct v2f // = vertex to fragment ( pass vertex data to pixel pass )
			{
				half4 pos : SV_POSITION;
				fixed4 vc : COLOR;
				half4 Ndot : COLOR1;
				fixed4 uv : TEXCOORD0;
				fixed4 uv2 : TEXCOORD1;
				half4 posWorld : TEXCOORD2;//position of vertex in world;
				half4 normalDir : TEXCOORD3;//vertex Normal Direction in world space
				half4 viewRefDir : TEXCOORD4;
				UNITY_FOG_COORDS(5)
				LIGHTING_COORDS(6, 7)
			};
			v2f vert (customData v)
			{
				v2f o;
				o.normalDir = fixed4 (0,0,0,0);
				o.Ndot = fixed4(0,0,0,0);
				o.posWorld = fixed4 (0,0,0,0);
				o.normalDir.xyz = UnityObjectToWorldNormal(v.normal);
				o.posWorld.xyz = mul(unity_ObjectToWorld, v.vertex);
			//--- Vectors
				half3 normalDirection = normalize(half3( mul(half4(v.normal, 0.0), unity_WorldToObject).xyz ));
				half3 lightDirection = normalize(half3(_WorldSpaceLightPos0.xyz));
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - o.posWorld.xyz);// world space

				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);//UNITY_MATRIX_MVP is a matrix that will convert a model's vertex position to the projection space
				o.vc = fixed4(1,1,1,1);;// Vertex Colors
				//__________________________________ Transparency master _____________________________________
				fixed edgeTransparency = _Transparency;
				fixed fresnel = dot(viewDirection, o.normalDir.xyz);
				#if NORMAL_EDGETRANSPARENCY
					edgeTransparency = pow(clamp(fresnel,0,1),_TransparencyFresnel)* _Transparency;
				#endif
				#if INVERTED_EDGETRANSPARENCY
					edgeTransparency =  pow(clamp((1-fresnel),0,1),_TransparencyFresnel)* _Transparency;
				#endif
				o.vc.a *=  edgeTransparency;
				o.uv = fixed4(0,0,0,0);
				o.uv.xy = TRANSFORM_TEX (v.texcoord, _MainTex); // this allows you to offset uvs and such	
				o.uv.xy = mul(_MainTexMatrix, fixed4(o.uv.xy,0,1)); // this allows you to rotate uvs and such with script help
				o.uv.zw = TRANSFORM_TEX (v.texcoord, _DecalTex); // this allows you to offset uvs and such
				o.uv.zw = mul(_DecalTexMatrix, fixed4(o.uv.zw,0,1)); // this allows you to rotate uvs and such with script help
				o.uv2 = fixed4(0,0,0,0);
				o.uv2.xy = v.texcoord1; //--- regular uv2
				o.uv2.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw; //Unity matrix lightmap uvs
				o.viewRefDir = fixed4(0,0,0,0);
				half3 viewNormal = normalize(WorldSpaceViewDir(v.vertex));
				o.viewRefDir.xyz = reflect(-viewNormal, o.normalDir);
				o.viewRefDir.xyz = BoxProjectedCubemapDirection(o.viewRefDir, o.posWorld, unity_SpecCube0_ProbePosition, unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax);
				o.Ndot.w = pow((1-(clamp(dot(viewDirection, o.normalDir.xyz),0,1) )),_CubeFresnel) * _CubeOpacity;
				o.Ndot.z = pow((1-(clamp(dot(viewDirection, o.normalDir.xyz),0,1) )),_RimFresnel)* _RimColor.a;
				#ifdef BASE_UNLIT
					o.Ndot.x = max(0.0, dot(normalDirection, lightDirection));//NdotL  light falloff
				#endif
				#ifdef BASE_BRDF
					o.Ndot.x = dot(normalDirection, lightDirection)*.5 +.5;//NdotL  light falloff
				#endif
				o.Ndot.y = clamp((dot(viewDirection, o.normalDir.xyz)) * 1.2 -.2 ,0.01,.99);

			//============================= Lights ================================
				fixed3 vLights = fixed3 (0,0,0);

				//___________________________ LightProbes Shade SH9 Math  __________________________________________
				fixed3 ambience = fixed3(1,1,1);
						ambience = ShadeSH9 (half4(o.normalDir.xyz,1.0)).rgb;
				#ifdef AMBIENT_ON
				vLights += ambience;
				#endif

				o.normalDir.w = vLights.r;
				o.viewRefDir.w = vLights.g;
				o.posWorld.w = vLights.b;
				TRANSFER_VERTEX_TO_FRAGMENT(o) // This sets up the vertex attributes required for lighting and passes them through to the fragment shader.
			//_________________________________________ FOG  __________________________________________
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag (v2f i) : COLOR  // i = in gets info from the out of the v2f vert
			{
				fixed4 resultRGB = fixed4(1,1,1,0);
				fixed3 vLights = fixed3(i.normalDir.w,i.viewRefDir.w,i.posWorld.w);
			//__________________________________ Vectors _____________________________________
				half3 normalDirection = normalize(i.normalDir.xyz);
				half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);//  half3 _WorldSpaceCameraPos.xyz built in gets camera Position
				half fresnel = dot(viewDirection, normalDirection);
				#if maskTex_ON
			//__________________________________ Masks _____________________________________
				fixed4 T_Masks = tex2D(_MasksTex, i.uv.xy);
				#endif
			//__________________________________ Color Base _____________________________________
				resultRGB = _Color;
			//__________________________________ Diffuse _____________________________________
				fixed4 T_Diffuse = tex2D(_MainTex, i.uv.xy);
				resultRGB *= T_Diffuse;
				resultRGB = lerp(_Color,fixed4(resultRGB.rgb,1),(T_Diffuse.a * _MainTexOpacity));

			//__________________________________ Decal _____________________________________
				fixed4 T_Decal = tex2D(_DecalTex, i.uv.zw) * _DecalColor;
				resultRGB = lerp(resultRGB,fixed4(T_Decal.rgb,1),T_Decal.a);

			//__________________________________ Lightmap _____________________________________
			//--- lightmap unity ---
				#ifdef CUSTOMLIGHTMAP_OFF
					#ifdef LIGHTMAP_ON
						fixed4 Lightmap = fixed4(DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2)),1);
						resultRGB.rgb *= resultRGB*Lightmap.rgb;
					#endif
				#endif

			//--- custom lightmap ---
				#ifdef CUSTOMLIGHTMAP_ON
					#if lmUV1_ON
						fixed4 Lightmap = tex2D(_LightmapTex, i.uv);
					#endif
					#if lmUV1_OFF
						fixed4 Lightmap = tex2D(_LightmapTex, i.uv2);
					#endif
					Lightmap.rgb = DecodeLightmap(Lightmap);
					resultRGB.rgb *= Lightmap.rgb;
				#endif

			//__________________________________ Lighting _____________________________________
				fixed atten = LIGHT_ATTENUATION(i); // This gets the shadow and attenuation values combined.
				fixed NdotL = i.Ndot.x;
				fixed NdotV = i.Ndot.y;
				fixed4 T_BRDF = tex2D(_BRDF, fixed2(NdotV,NdotL));
				fixed3 Lights = vLights;
				vLights = lerp(vLights, 1, _BRDFBrightness);

				#ifdef BASE_UNLIT
					#ifdef AMBIENT_OFF
						vLights = fixed3 (1,1,1);
					#endif
					#ifdef LDIR_ON
						Lights = vLights + ( NdotL * _LightColor0.rgb);
					#endif
					#ifdef LDIR_OFF
						Lights = vLights;
					#endif
					resultRGB.rgb *= Lights;
				#endif

				#ifdef BASE_BRDF
					NdotL = T_BRDF.r;
					NdotL = clamp( (NdotL ) + (T_BRDF.a) ,0,1);
					#ifdef LDIR_ON
						#ifdef AMBIENT_ON
							resultRGB.rgb = resultRGB.rgb * (vLights  + (NdotL * _LightColor0.rgb)) ;
						#else
							resultRGB.rgb = lerp(resultRGB.rgb * vLights, resultRGB.rgb *(1 + (NdotL*_LightColor0.rgb) ), NdotL);
						#endif
					#endif
					#ifdef LDIR_OFF
							resultRGB.rgb = lerp(resultRGB.rgb * vLights, resultRGB.rgb, NdotL);
					#endif
				#endif

			//__________________________________ Reflection _____________________________________
				half4 Cubemap = fixed4(0,0,0,0);
					half3 viewRefDir = i.viewRefDir.xyz;
					fixed RefOpacity = i.Ndot.w;

			#ifdef REFCUSTOM_ON
			//--------------------------- Custom Cubemap -------------------
				Cubemap = texCUBE(_Cube, viewRefDir); //traditional cube sampling
				//Cubemap = SampleCubeReflection(_Cube, viewRefDir, -7);//Sample Cube reflection with mip map range
			#endif

			#ifdef REFCUSTOM_OFF
			//--------------------------- Unity Cubemap -------------------
				Cubemap = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, viewRefDir);//unity cubemap 
				//Cubemap = SampleCubeReflection(unity_SpecCube0, viewRefDir,-7);//Sample Cube reflection with mip map range
			#endif

			//--- Decode Cubemap HDR
				Cubemap.rgb = DecodeHDR (Cubemap, unity_SpecCube0_HDR);//Basic one I think...
				//Cubemap.rgb = DecodeHDR_NoLinearSupportInSM2 (Cubemap, unity_SpecCube0_HDR);//From unitys BRDF decoding
				Cubemap.rgb *= _CubeIntensity;

			//--- Metallic math --
				fixed4 resultRGBnl = resultRGB;
				fixed3 CubemapM = resultRGB * Cubemap;
				fixed3 CubemapN = lerp(Cubemap,resultRGB + Cubemap * unity_ColorSpaceDielectricSpec.rgb,resultRGB.a);
				Cubemap = fixed4(lerp(CubemapN,CubemapM,_Metallic).rgb,resultRGB.a + Cubemap.a);

				#if maskTex_ON
					RefOpacity *= T_Masks.r;
				#endif
				resultRGB = lerp (resultRGBnl ,Cubemap, RefOpacity);

			//__________________________________ Rim _____________________________________
				#ifdef BASE_UNLIT
						fixed RimOpacity = i.Ndot.z;
							# ifdef LIGHTMAP_ON
								RimOpacity *= Luminance(Lightmap);
							#endif
				#endif

				#ifdef BASE_BRDF
					fixed RimOpacity = T_BRDF.b *_RimColor.a;
							# ifdef LIGHTMAP_ON
								RimOpacity *= Luminance(Lightmap);
							#endif
				#endif
				#if maskTex_ON
				RimOpacity *= T_Masks.b;
				#endif
				resultRGB = lerp (resultRGB, _RimColor * _RimIntensity, RimOpacity);

			//__________________________________ Mask Occlussion _____________________________________
				#if maskTex_ON
				//--- Oclussion from alpha
				resultRGB.rgb = resultRGB.rgb * T_Masks.g;
				#endif

			//__________________________________ Fog  _____________________________________
				UNITY_APPLY_FOG(i.fogCoord, resultRGB);

			//__________________________________ Vertex Alpha _____________________________________
				resultRGB.a *= i.vc.a;

				resultRGB.a =  clamp(resultRGB.a,0,1);
			//__________________________________ result Final  _____________________________________
				return resultRGB;
			}
			ENDCG
		}//-------------------------------Pass-------------------------------
	} //-------------------------------SubShader-------------------------------
	Fallback "UNOShader/_Library/Helpers/VertexUNLIT Transparent"
	CustomEditor "UNOShader_UNLIT"
}