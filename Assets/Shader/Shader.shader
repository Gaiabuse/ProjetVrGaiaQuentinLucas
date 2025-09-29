Shader "Custom/MyURPUnlit"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }
        
        Pass
        {
            Name "UnlitPass"
            
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            
            CBUFFER_START(UnityPerMaterial);
            half4 _BaseColor;
            half4 _MainTex_ST;
            CBUFFER_END

            struct Attributes
            {
                float3 positionLS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalsLS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalsWS : TEXCOORD1;
            };
            
            Varyings Vertex(Attributes a)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(a.positionLS);
                output.uv = TRANSFORM_TEX(a.uv,_MainTex);
                output.normalsWS = TransformObjectToWorldNormal(a.normalsLS);
                return output;
            }
            
                half4 Fragment(Varyings v) : SV_Target
                {
                    half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,v.uv);
                    return _BaseColor * tex;
                }
            ENDHLSL
        }
    }
}