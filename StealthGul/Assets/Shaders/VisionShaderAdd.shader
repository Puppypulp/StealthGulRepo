// Shader created with Shader Forge v1.13 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.13;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,nrsp:0,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:False,dith:0,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:7848,x:33653,y:32676,varname:node_7848,prsc:2|emission-9227-OUT;n:type:ShaderForge.SFN_Color,id:1090,x:32881,y:32762,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_1090,prsc:2,glob:False,c1:0,c2:1,c3:0.9586205,c4:1;n:type:ShaderForge.SFN_TexCoord,id:6192,x:32274,y:32958,varname:node_6192,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector1,id:1455,x:32295,y:33212,varname:node_1455,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Subtract,id:7512,x:32496,y:32994,varname:node_7512,prsc:2|A-6192-UVOUT,B-1455-OUT;n:type:ShaderForge.SFN_Length,id:7725,x:32688,y:32994,varname:node_7725,prsc:2|IN-7512-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5274,x:32715,y:33243,ptovrint:False,ptlb:AlphaPower,ptin:_AlphaPower,varname:node_5274,prsc:2,glob:False,v1:2;n:type:ShaderForge.SFN_Power,id:3185,x:32881,y:32994,varname:node_3185,prsc:2|VAL-7725-OUT,EXP-5274-OUT;n:type:ShaderForge.SFN_Multiply,id:1139,x:33183,y:32854,varname:node_1139,prsc:2|A-1090-RGB,B-3185-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1061,x:33155,y:32716,ptovrint:False,ptlb:Intensity,ptin:_Intensity,varname:node_1061,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:9227,x:33404,y:32682,varname:node_9227,prsc:2|A-1061-OUT,B-1139-OUT;proporder:1090-5274-1061;pass:END;sub:END;*/

Shader "Custom/VisionShaderAdd" {
    Properties {
        _Color ("Color", Color) = (0,1,0.9586205,1)
        _AlphaPower ("AlphaPower", Float ) = 2
        _Intensity ("Intensity", Float ) = 1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _AlphaPower;
            uniform float _Intensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float3 emissive = (_Intensity*(_Color.rgb*pow(length((i.uv0-0.5)),_AlphaPower)));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
