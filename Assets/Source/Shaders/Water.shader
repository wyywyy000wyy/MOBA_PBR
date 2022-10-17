Shader "YSTech/Water"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _flowDir("FlowDir", Vector) = (1,1,1,1)
        _flowSpeed("_flowSpeed", Float) = 1.0
        _WaterColor("Color", Color) = (1,1,1)
        _SpecularShiny("SpecularShiny", Float) = 100
        _Specular("_Specular", Float) = 2
        _Diffuse("_Diffuse", Float) = 0.5 
        _Cubemap("Cubemap", Cube) = "" {}
    }
    SubShader
    {
        Tags{"RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True" "ShaderModel"="4.5"}
        
        LOD 100
            // Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}
            // Blend[_SrcBlend][_DstBlend]
            Blend SrcAlpha OneMinusSrcAlpha
            // ZTest GEqual
            // ZTest Off
            ZWrite Off
            // ZWrite[_ZWrite]
            // Cull[_Cull]

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0
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
                float3 normalWS : TEXCOORD2;
                float3 viewDirectionWS : TEXCOORD3;
                float4 uvMirror : TEXCOORD4;
                float4 uvRefract : TEXCOORD5;
            };




            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _RenderOpaquePassTexture;
            sampler2D _RenderRefractPassTexture;

            samplerCUBE _Cubemap;

            float4x4 virtualMatrix;
            float _flowSpeed;
            float4 _flowDir;
            float3 _WaterColor;

            float _SpecularShiny;
            float _Specular;
            float _Diffuse;

            v2f vert(appdata v)
            {
                v2f o;
                o.positionWS = TransformObjectToWorld(v.vertex.xyz);
                o.positionCS = TransformWorldToHClip(o.positionWS);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.viewDirectionWS = _WorldSpaceCameraPos.xyz - o.positionWS;
                //float4 mirrorPos = mul(virtualMatrix, float4(o.positionWS, 1));
                float4 mirrorPos = mul(virtualMatrix, v.vertex);
                o.uvMirror = mirrorPos;
                o.uvRefract = o.positionCS;
                //o.uvMirror.xy = (mirrorPos.xy / mirrorPos.w + 1) * 0.5;
                return o;
            }

            float4 getNoise(float2 uv, float time) {
                float2 uv0 = (uv / 103.0) + float2(time / 17.0, time / 29.0);
                float2 uv1 = uv / 107.0 - float2(time / -19.0, time / 31.0);
                float2 uv2 = uv / float2(8907.0, 9803.0) + float2(time / 101.0, time / 97.0);
                float2 uv3 = uv / float2(1091.0, 1027.0) - float2(time / 109.0, time / -113.0);
                float4 noise = tex2D(_MainTex, uv3)
                +tex2D(_MainTex, uv1) +
                    tex2D(_MainTex, uv2) +
                    tex2D(_MainTex, uv3);
                noise *= 0.25;
                noise.x = noise.w * 2 - 1;
                noise.z = noise.z * 2 - 1;
                // noise.xy = noise.xy * 2 - 1.0;
                return noise;// noise * 0.5 - 1.0;
            }

            half4 frag(v2f i) : SV_Target
            {
                // sample the texture
                _flowSpeed = 5;
                float2 uv = (i.positionWS.xz + (_flowDir.xz * _flowSpeed * _Time.x)) * 0.1;

                float4 noise = getNoise(i.positionWS.xz * 0.5, _Time.x * _flowSpeed);

                //float3 normal = tex2D(_MainTex, uv).rgb;

                float3 normal = normalize(noise.xyz * float3(1.5, 1.0, 1.5));
                // float3 normal = normalize(noise.xzy);// *float3(1.5, 1.0, 1.5));



                Light light = GetMainLight();

                float3 viewDirection = SafeNormalize(-i.viewDirectionWS);
                float3 eyeDirection = SafeNormalize(i.viewDirectionWS);

                float3 reflection = normalize(reflect(-viewDirection, normal));
                float specdirection = max(0, dot(light.direction, reflection));
                float3 specularLight = pow(specdirection, _SpecularShiny) * light.color * _Specular;
                float3 diffuseLight = max(dot(light.direction, normal), 0.0) * light.color * _Diffuse;



                float NoL = saturate(dot(normal, light.direction));
                float theta = max(0.0, dot(eyeDirection, normal));
                float F0 = 0.2;
                float reflectance = F0 + (1 - F0) * pow(1 - theta, 5.0);

                float3 coord = i.uvMirror.xyz / i.uvMirror.w;
                float2 uvMirror = float2(coord.xy + coord.z * normal.xz * 0.05);

                // half4 refColor = tex2D(_RenderOpaquePassTexture, (i.uvMirror.xy / i.uvMirror.w + 1) * 0.5);
                half4 refColor = tex2D(_RenderOpaquePassTexture, (uvMirror + 1) * 0.5);
                // half4 refractColor = tex2D(_RenderRefractPassTexture, (i.positionCS.xy / i.positionCS.w + 1) * 0.5);
                float3 uvr = (i.uvRefract.xyz / i.uvRefract.w + 1) * 0.5;

                uvr.xy = uvr.xy + uvr.z * normal.xz * 0.05;
                // uvr.y = 1 - uvr.y;

                half4 refractColor = tex2D(_RenderRefractPassTexture, uvr.xy);
                //half4 refColor = textureProjExternal(_RenderOpaquePassTexture, i.uvMirror);


                float3 scatter = max(0.0, dot(normal, eyeDirection)) * _WaterColor;
                //float3 reflectionSample = half3(1, 1, 1);
                float4 reflectionSample = refColor;// texCUBE(_Cubemap, eyeDirection).rgb;
                float3 diffuseColor = _WaterColor * diffuseLight;// +scatter;
                //diffuseColor = normal;
                //float3 waterColor = lerp(diffuseColor, half3(0.1f, 0.1f, 0.1f) + reflectionSample * 0.9 + reflectionSample * specularLight, reflectance);
                //float3 waterColor = lerp(diffuseColor, reflectionSample, reflectance);
                float4 waterColor = lerp(refractColor, refColor, reflectance);

                //float3 waterColor = diffuseLight * _WaterColor + reflectionSample * specularLight;
                //half4 col = half4(waterColor * _WaterColor, reflectance);
                half4 col = half4(_WaterColor, 1) * waterColor;// half4(waterColor, 1);

                // col = half4(diffuseColor, 1);
                // col = tex2D(_RenderOpaquePassTexture, i.uv);
                // col = refractColor;
                // return float4(normal.xyz,1);
                return waterColor;
            }

                //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


                            ENDHLSL
                        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"

}
