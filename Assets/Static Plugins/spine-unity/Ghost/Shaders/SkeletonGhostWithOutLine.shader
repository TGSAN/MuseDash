//Shader written by Alex Dixon
Shader "Spine/SkeletonGhostWithOutLine" 
{
    Properties 
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _OutLineColor("OutLine Color", Color) = (0,0,0,1)
        _OutLine("OutLine Width", Range(0.0,0.03)) = 0.005
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_TextureFade ("Texture Fade Out", Range(0,1)) = 0
		_Size("Size", range(1,2048)) = 256//size      
    }
    
//    CGINCLUDE
//    	#include "UnityCG.cginc"
//    	struct appdata{
//    		float4 vertex : POSITION;	
//    		float3 normal : NORMAL;
//    	};
//    	
//    	struct v2f {
//    		float4 pos : POSITION;
//    		float4 color: COLOR;
//    	};
//    	
//    	uniform float _OutLine;
//    	uniform float4 _OutLineColor;
//    	
//    	v2f vert(appdata v){
//    		v2f o;
//    		o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
//    		float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV,v.normal);
//    		float2 offset = TransformViewToProjection(norm.xy);
//    		o.pos.xy += offset * o.pos.z * _OutLine;
//    		o.color = _OutLineColor;
//    		return o;
//    	}
//    ENDCG
    
    
    // sobel
    
//    Properties {    
//    	_MainTex ("MainTex", 2D) = "white" {}    
//    	_Size("Size", range(1,2048)) = 256//size      
//    }  
//    SubShader {    
//    	
//     } 
    
    //-------------------------------------------------------
    
    SubShader 
    {
    
      Tags {"Queue"="Transparent" "IgnoreProjector"="False" "RenderType"="Transparent"}
      Fog { Mode Off }
      Blend One OneMinusSrcAlpha		// alpha mode
      ZWrite Off						// if use unity default deepth cache 
	  Cull back							// cut off mode
      
//      	Pass
//        {
//            Cull front
//            offset -5,-1
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #include "UnityCG.cginc"
//            sampler2D _MainTex;
//            float4 _MainTex_ST;
//            struct v2f {
//                float4  pos : SV_POSITION;
//                float2  uv : TEXCOORD0;
//            } ;
//            v2f vert (appdata_base v)
//            {
//                v2f o;
//                o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
//                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
//                return o;
//            }
//            float4 frag (v2f i) : COLOR
//            {
//                return float4(0,0,0,0);
//            }
//            ENDCG
//        }
      
        Pass 
        {
            Tags {"LightMode" = "Always"}                      // This Pass tag is important or Unity may not give it the correct light information.
           		
//           		offset 2,-1
           		
           		CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                //#pragma multi_compile_fwdbase                       // This line tells Unity to compile this pass for forward base.
                
                #include "AutoLight.cginc"
                #include "UnityCG.cginc"
//               	struct vertex_input
//               	{
//               		float4 vertex : POSITION;
//               		float2 texcoord : TEXCOORD0;
//					float4 color : COLOR;
//
//               	};
//                
//                struct vertex_output
//                {
//                    float4  pos         : SV_POSITION;
//                    float2  uv          : TEXCOORD0;
//
//					float4 color : COLOR;
//                };
                
                sampler2D _MainTex;
                
                fixed4 _OutLineColor;
                
                float4 _MainTex_ST;
                struct v2f{
                	float4 pos:SV_POSITION;
                	float3 color:Color;
                	float2 uv:TEXCOORD0;
                };
                
                v2f vert(appdata_base v)
                {
                	v2f o;
                	o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
                	o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                	return o;
                }
                
                float4 frag (v2f i):COLOR
                {
                	float4 texCol = tex2D(_MainTex, i.uv);
                	float4 outp = texCol;
//                	outp.rgb = outp.rgb + (_OutLineColor * (1-outp.a));
					outp.rgb = outp.rgb * outp.a;
                	return outp;
                }
                
                
//                struct vertex_output
//                {
//                    float4  pos         : SV_POSITION;
//                    float2  uv          : TEXCOORD0;
//
//					float4 color : COLOR;
//                };
//                
//                sampler2D _MainTex;
//                fixed4 _Color;
//				fixed _TextureFade;
//                
//                vertex_output vert (vertex_input v)
//                {
//                    vertex_output o;
//                    o.pos = mul( UNITY_MATRIX_MVP, v.vertex);
//                    o.uv = v.texcoord.xy;
//                    o.color = v.color;
//					
//						         
//                    return o;
//                }
//                
//                fixed4 frag(vertex_output i) : COLOR
//                {
//                    fixed4 tex = tex2D(_MainTex, i.uv);
//
//					tex = fixed4(max(_TextureFade, tex.r), max(_TextureFade, tex.g), max(_TextureFade, tex.b), tex.a);
//					
//					return tex * ((i.color * _Color) * tex.a);
//
//
//
//					//float finalAlpha = tex.a * i.color.a * _Color.a;
//
//                    /*
//                    TODO:  Add basic lighting stuff in later?
//
//                    fixed4 c;
//					c.rgb = (UNITY_LIGHTMODEL_AMBIENT.rgb * tex.rgb);       // Ambient term. Only do this in Forward Base. It only needs calculating once.
//                    c.rgb += tex.rgb; // Diffuse and specular.
//					//Unity 4: c.rgb = (UNITY_LIGHTMODEL_AMBIENT.rgb * tex.rgb * 2);       // Ambient term. Only do this in Forward Base. It only needs calculating once.
//					//Unity 4: c.rgb += (tex.rgb * _LightColor0.rgb * diff) * (atten * 2); // Diffuse and specular.
//                    c.a = tex.a;  // + _LightColor0.a * atten;
//
//                    return c;
//					*/
//                }
            ENDCG
        }
        
        Pass{    
    		Tags{"LightMode"="ForwardBase" }    
		    Cull off    
		    CGPROGRAM    
		    #pragma vertex vert    
		    #pragma fragment frag    
		    #include "UnityCG.cginc"   
		    float _Size;    
		    sampler2D _MainTex;    
		    float4 _MainTex_ST;    
		    struct v2f {      
		    float4 pos:SV_POSITION;      
		    float2 uv_MainTex:TEXCOORD0;          
		    };    
		    v2f vert (appdata_full v) {     
		    	v2f o;      
		    	o.pos=mul(UNITY_MATRIX_MVP,v.vertex);      
		    	o.uv_MainTex = TRANSFORM_TEX(v.texcoord,_MainTex);      
		     	return o;    
		    }    
		    float4 frag(v2f i):COLOR    
		    {      
		    	float3 lum = float3(0.2125,0.7154,0.0721);
		     	//转化为luminance亮度值     
		     	//获取当前点的周围的点      
		     	//并与luminance点积，求出亮度值（黑白图）      
		    	float mc00 = dot(tex2D (_MainTex, i.uv_MainTex-fixed2(1,1)/_Size).rgb, lum);      
		     	float mc10 = dot(tex2D (_MainTex, i.uv_MainTex-fixed2(0,1)/_Size).rgb, lum);      
		     	float mc20 = dot(tex2D (_MainTex, i.uv_MainTex-fixed2(-1,1)/_Size).rgb, lum);      
		     	float mc01 = dot(tex2D (_MainTex, i.uv_MainTex-fixed2(1,0)/_Size).rgb, lum);      
		     	float mc11mc = dot(tex2D (_MainTex, i.uv_MainTex).rgb, lum);     
		     	float mc21 = dot(tex2D (_MainTex, i.uv_MainTex-fixed2(-1,0)/_Size).rgb, lum);      
		     	float mc02 = dot(tex2D (_MainTex, i.uv_MainTex-fixed2(1,-1)/_Size).rgb, lum);      
		     	float mc12 = dot(tex2D (_MainTex, i.uv_MainTex-fixed2(0,-1)/_Size).rgb, lum);      
		     	float mc22 = dot(tex2D (_MainTex, i.uv_MainTex-fixed2(-1,-1)/_Size).rgb, lum);      
		     	//根据过滤器矩阵求出GX水平和GY垂直的灰度值      
		     	float GX = -1 * mc00 + mc20 + -2 * mc01 + 2 * mc21 - mc02 + mc22;      
		    	float GY = mc00 + 2 * mc10 + mc20 - mc02 - 2 * mc12 - mc22;    //        
		     	float G = sqrt(GX*GX+GY*GY);//标准灰度公式     
	//		     float G = abs(GX)+abs(GY);//近似灰度公式//                       
		    	float th = atan(GY/GX);//灰度方向      
		     	float4 c = 0;//                        
		     	c = G>th?1:0;//                        
		     	c = G/th*2;      
		     	c = length(float2(GX,GY));
		     	//length的内部算法就是灰度公式的算法，欧几里得长度  
		     	
		     	c.rgb = c.rgb*c.a;
		     	      
		     	return c;    
		     }    
		     ENDCG    
     	}
        
//        
//        Pass{
//        	Name "BASE"
//        	ZWrite on
//        	ZTest LEqual
//        	Blend SrcAlpha OneMinusSrcAlpha
//        	Material{
//        		Diffuse [_Color]
//        		Ambient [_Color]
//        	}
//        	Lighting On
//        	SetTexture [_MainTex]{
//        		ConstantColor [_Color]
//        		Combine texture * constant
//        	}
//        	SetTexture [_MainTex]{
////        		Combine previous * primary DOUBLE
//        	}
//        	
//        }
        
    }
    //FallBack "Transparent/Cutout/VertexLit"    // Use VertexLit's shadow caster/receiver passes.
}