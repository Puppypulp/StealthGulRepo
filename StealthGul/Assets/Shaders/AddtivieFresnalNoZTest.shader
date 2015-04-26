// Shader created with Shader Forge v1.13 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.13;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,nrsp:0,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,bsrc:0,bdst:0,culm:0,dpts:6,wrdp:False,dith:0,ufog:True,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9740,x:34174,y:32841,varname:node_9740,prsc:2|emission-5751-OUT;n:type:ShaderForge.SFN_NormalVector,id:3149,x:32799,y:32836,prsc:2,pt:False;n:type:ShaderForge.SFN_ViewVector,id:6500,x:32799,y:33006,varname:node_6500,prsc:2;n:type:ShaderForge.SFN_Dot,id:8592,x:33013,y:32917,varname:node_8592,prsc:2,dt:0|A-3149-OUT,B-6500-OUT;n:type:ShaderForge.SFN_OneMinus,id:9930,x:33219,y:32917,varname:node_9930,prsc:2|IN-8592-OUT;n:type:ShaderForge.SFN_Color,id:6097,x:33454,y:32699,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_6097,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:1152,x:33219,y:33128,ptovrint:False,ptlb:FresnalPower,ptin:_FresnalPower,varname:node_1152,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Power,id:2049,x:33454,y:32997,varname:node_2049,prsc:2|VAL-9930-OUT,EXP-1152-OUT;n:type:ShaderForge.SFN_Multiply,id:4582,x:33743,y:32850,varname:node_4582,prsc:2|A-6097-RGB,B-2049-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3802,x:33743,y:33079,ptovrint:False,ptlb:Intensity,ptin:_Intensity,varname:node_3802,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:5751,x:33927,y:32964,varname:node_5751,prsc:2|A-4582-OUT,B-3802-OUT;proporder:6097-1152-3802;pass:END;sub:END;*/

Shader "Custom/AddtivieFresnalNoZTest" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _FresnalPower ("FresnalPower", Float ) = 1
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
            ZTest Always
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
            uniform float _FresnalPower;
            uniform float _Intensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float3 emissive = ((_Color.rgb*pow((1.0 - dot(i.normalDir,viewDirection)),_FresnalPower))*_Intensity);
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
