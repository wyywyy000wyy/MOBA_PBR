Shader "YSTech/RectifiedShadow"
{
    SubShader
    {
        //Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        //ZWrite On
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}


        Pass
        {
            Cull Off
            //ZTest Always ZWrite On ColorMask 0
            ZTest On
            ZWrite On
            //Blend SrcAlpha Zero
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
            };

            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.positionWS = TransformObjectToWorld(v.vertex.xyz);

                float a;
                float y;

                o.positionCS = TransformWorldToHClip(o.positionWS);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float a = i.positionCS.z / i.positionCS.w;
            a = (a + 1) * 0.5;
                return float4(a,a,a,1);
            }

//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


            ENDHLSL
        }
    }
}
