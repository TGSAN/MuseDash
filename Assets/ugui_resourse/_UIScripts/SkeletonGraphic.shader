
Shader "Spine/SkeletonGraphic" {  
    Properties {  
        _MainTex ("Main Texture", 2D) = "black" {}  

    }  
      
    SubShader {  
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }  
        LOD 100  
  
        Cull Off  
        ZWrite Off  
        Blend One OneMinusSrcAlpha  
        Lighting Off  
  
        Stencil {  
            Ref 1  
            Comp Equal  
        }  
  
        Pass {  
            ColorMaterial AmbientAndDiffuse  
            SetTexture [_MainTex] {  
                Combine texture * primary  
            }  
        }  
    }  
} 