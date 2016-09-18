Shader "Custom/pencil_sketch_depth" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_PencilTex0("Pencil Texture0",2D) = "white" {}
		_PencilTex1("Pencil Texture1",2D) = "white" {}
		_PencilTex2("Pencil Texture2",2D) = "white" {}
		_PencilTex3("Pencil Texture3",2D) = "white" {}
		_PencilTex4("Pencil Texture4",2D) = "white" {}
		_PencilTex5("Pencil Texture5",2D) = "white" {}
		_PaperTex("Paper Texture",2D) = "white" {}
		_TileFactor ("Tile Factor", Float) = 1
		_TileFactor2 ("Tile Factor2", Float) = 1
	}
	SubShader 
	{
		Pass {  
            CGPROGRAM  
            #pragma exclude_renderers gles
            #pragma vertex vert 
            #pragma fragment frag  
            #include "UnityCG.cginc" 
            #pragma target 3.0 
            uniform sampler2D _MainTex;
            sampler2D_float _CameraDepthTexture;
            uniform sampler2D _PencilTex0;
            uniform sampler2D _PencilTex1;
            uniform sampler2D _PencilTex2;
            uniform sampler2D _PencilTex3;
            uniform sampler2D _PencilTex4;
            uniform sampler2D _PencilTex5;
            uniform sampler2D _PaperTex;
            fixed _TileFactor;
            fixed _TileFactor2;
            fixed4 _MainTex_ST; 
            half4 _MainTex_TexelSize;
            //half _GamaAmount;
            struct v2f 
            {
		        float4 pos : POSITION;
		        half2 uv : TEXCOORD0;
		        half2 MapOffset : TEXCOORD1; 
	        };
	        v2f vert( appdata_img v ) 
	        {
		       v2f o; 
		       o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		       o.uv = v.texcoord;
		       o.MapOffset= _MainTex_TexelSize.xy;
		       return o;
	        }
            fixed4 frag(v2f i) : COLOR  
            {  
                
                fixed3 c = tex2D(_MainTex,i.uv);
                fixed4 Paper =tex2D(_PaperTex,i.uv);
                fixed grey0 = Luminance(c);
                float d = 1 - saturate(SAMPLE_DEPTH_TEXTURE (_CameraDepthTexture, i.uv + i.MapOffset));
                //InnerLum
                fixed LastPercent = 1.0;
                fixed Hatch0Percent = saturate(( grey0 - 0.8 ) / 0.2);
                LastPercent -= Hatch0Percent;
                fixed Hatch1Percent = (1-saturate(abs( grey0 - 0.8 )/ 0.2)) * max( LastPercent,0 );
                LastPercent -= Hatch1Percent;
                fixed Hatch2Percent = (1-saturate(abs( grey0 - 0.6 )/ 0.2)) * max( LastPercent,0 );
                LastPercent -= Hatch2Percent;
                fixed Hatch3Percent = (1-saturate(abs( grey0 - 0.4 )/ 0.2)) * max( LastPercent,0 );
                LastPercent -= Hatch3Percent;
                fixed Hatch4Percent = (1-saturate(abs( grey0 - 0.2 )/ 0.2)) * max( LastPercent,0 );
                LastPercent -= Hatch4Percent;
                fixed Hatch5Percent = (1-saturate(abs( grey0 - 0.0 )/ 0.2)) * max( LastPercent,0 );
                LastPercent -= Hatch5Percent;
                fixed2 UV = i.uv * _TileFactor + fixed2(0,d) * _TileFactor;
                fixed4 hatchTex0 = tex2D(_PencilTex0, UV) ;
				fixed4 hatchTex1 = tex2D(_PencilTex1, UV) ;
				fixed4 hatchTex2 = tex2D(_PencilTex2, UV) ;
				fixed4 hatchTex3 = tex2D(_PencilTex3, UV) ;
				fixed4 hatchTex4 = tex2D(_PencilTex4, UV) ;
				fixed4 hatchTex5 = tex2D(_PencilTex5, UV) ;
                //Line
                fixed3 cOffset = tex2D(_MainTex, i.uv + i.MapOffset);
                fixed3 RGBDiff = abs(cOffset-c);
                fixed greys1 = Luminance(RGBDiff);
                greys1 = min(greys1,1);
                fixed4 FinalColor = fixed4((1-greys1).xxx,1);
                FinalColor *= FinalColor;
                FinalColor *= FinalColor;
                fixed4 PencilColor = hatchTex0 * Hatch0Percent + hatchTex1 * Hatch1Percent + hatchTex2 * Hatch2Percent + hatchTex3 * Hatch3Percent + hatchTex4 * Hatch4Percent + hatchTex5 * Hatch5Percent;
                return FinalColor * Paper * PencilColor;
            }  
              
            ENDCG  
        }  
	} 
	FallBack "Diffuse"
}
