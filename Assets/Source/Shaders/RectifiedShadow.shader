Shader "YSTech/RectifiedShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _flowDir("FlowDir", Vector) = (1,1,1,1)
        _flowSpeed ("FlowSpeed", Float) = 1.0
        _WaterColor("Color", Color) = (1,1,1)
        _SpecularShiny("SpecularShiny", Float) = 100
        _Specular("_Specular", Float) = 2
        _Diffuse("_Diffuse", Float) = 0.5
        _Cubemap("Cubemap", Cube) = "" {}

    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        //ZTest Off
        //ZWrite On


        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS     : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.positionWS = TransformObjectToWorld(v.vertex.xyz);
                o.positionCS = TransformWorldToHClip(o.positionWS);
                //o.uvMirror.xy = (mirrorPos.xy / mirrorPos.w + 1) * 0.5;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float a = i.positionCS.z;// / i.positionCS.w;
                return float4(a,a,a,1);
            }

//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


            ENDHLSL
        }
    }
}
