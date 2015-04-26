// Shader created with Shader Forge v1.13 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.13;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,nrsp:0,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,bsrc:0,bdst:7,culm:0,dpts:2,wrdp:False,dith:0,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:8199,x:34006,y:32985,varname:node_8199,prsc:2|emission-9502-OUT,alpha-1798-OUT;n:type:ShaderForge.SFN_Color,id:3879,x:32792,y:32802,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_3879,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:3202,x:32792,y:32984,varname:node_3202,prsc:2,ntxv:0,isnm:False|TEX-2567-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:2567,x:32590,y:33026,ptovrint:False,ptlb:Color Texture,ptin:_ColorTexture,varname:node_2567,glob:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2dAsset,id:3846,x:32618,y:33477,ptovrint:False,ptlb:Alpha Texture,ptin:_AlphaTexture,varname:node_3846,glob:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:308,x:32820,y:33435,varname:node_308,prsc:2,ntxv:0,isnm:False|TEX-3846-TEX;n:type:ShaderForge.SFN_ValueProperty,id:4021,x:32814,y:33163,ptovrint:False,ptlb:Emissive,ptin:_Emissive,varname:node_4021,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:9919,x:33235,y:33067,varname:node_9919,prsc:2|A-3287-OUT,B-4021-OUT;n:type:ShaderForge.SFN_Multiply,id:3287,x:33008,y:32930,varname:node_3287,prsc:2|A-3879-RGB,B-3202-RGB;n:type:ShaderForge.SFN_VertexColor,id:4744,x:33235,y:33276,varname:node_4744,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5184,x:33502,y:33137,varname:node_5184,prsc:2|A-9919-OUT,B-4744-RGB;n:type:ShaderForge.SFN_Multiply,id:1798,x:33506,y:33445,varname:node_1798,prsc:2|A-4744-A,B-308-R;n:type:ShaderForge.SFN_Multiply,id:9502,x:33737,y:33155,varname:node_9502,prsc:2|A-5184-OUT,B-1798-OUT;proporder:3879-2567-3846-4021;pass:END;sub:END;*/

Shader "Custom/PartEmissive" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _ColorTexture ("Color Texture", 2D) = "white" {}
        _AlphaTexture ("Alpha Texture", 2D) = "white" {}
        _Emissive ("Emissive", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            Blend One OneMinusSrcAlpha
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
            uniform sampler2D _ColorTexture; uniform float4 _ColorTexture_ST;
            uniform sampler2D _AlphaTexture; uniform float4 _AlphaTexture_ST;
            uniform float _Emissive;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float4 node_3202 = tex2D(_ColorTexture,TRANSFORM_TEX(i.uv0, _ColorTexture));
                float4 node_308 = tex2D(_AlphaTexture,TRANSFORM_TEX(i.uv0, _AlphaTexture));
                float node_1798 = (i.vertexColor.a*node_308.r);
                float3 emissive = ((((_Color.rgb*node_3202.rgb)*_Emissive)*i.vertexColor.rgb)*node_1798);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,node_1798);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
