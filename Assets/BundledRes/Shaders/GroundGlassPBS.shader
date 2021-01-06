Shader "YSTech/GroundGlassPBS"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		_NormalTex("Normal Tex", 2D) = "white" {}
		_SpecularTex("Specular Tex", 2D) = "white" {}
		_MetallicTex("Metallic Tex", 2D) = "white" {}
		_RoughnessTex("Roughness Tex", 2D) = "white" {}
		_AOTex("AO Tex", 2D) = "white" {}
		_LightInstense("Light Instense", Float) = 1
		_Metallic("Metallic", Range(0, 1)) = 0.04
    }
    SubShader
    {
        Tags {"LightMode" = "ForwardBase" "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work

            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc" // for _LightColor0

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				fixed4 diff : COLOR0; // diffuse lighting color
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 lightDir : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
            };

			sampler2D _MainTex;
			sampler2D _NormalTex;

			sampler2D _SpecularTex;
			sampler2D _MetallicTex;
			sampler2D _RoughnessTex;
			sampler2D _AOTex;
			
            float4 _MainTex_ST;
			float _LightInstense;
			float _Metallic;

#define PI (3.14159265)

			float D_GGX(float a2, float NoH)
			{
				float d = (NoH * a2 - NoH) * NoH + 1;	// 2 mad
				return a2 / (PI*d*d);					// 4 mul, 1 rcp
			}

			float Vis_SmithJointApprox(float a2, float NoV, float NoL)
			{
				float a = sqrt(a2);
				float Vis_SmithV = NoL * (NoV * (1 - a) + a);
				float Vis_SmithL = NoV * (NoL * (1 - a) + a);
				return 0.5 * rcp(Vis_SmithV + Vis_SmithL);
			}

			float Vis_Smith(float a2, float NoV, float NoL)
			{
				float Vis_SmithV = NoV + sqrt(NoV * (NoV - NoV * a2) + a2);
				float Vis_SmithL = NoL + sqrt(NoL * (NoL - NoL * a2) + a2);
				return rcp(Vis_SmithV * Vis_SmithL);
			}

			float Vis_SmithJoint(float a2, float NoV, float NoL)
			{
				float Vis_SmithV = NoL * sqrt(NoV * (NoV - NoV * a2) + a2);
				float Vis_SmithL = NoV * sqrt(NoL * (NoL - NoL * a2) + a2);
				return 0.5 * rcp(Vis_SmithV + Vis_SmithL);
			}

			float3 F_Schlick(float3 SpecularColor, float VoH)
			{
				float Fc = pow(1 - VoH, 5);					// 1 sub, 3 mul
				//return Fc + (1 - Fc) * SpecularColor;		// 1 add, 3 mad

				// Anything less than 2% is physically impossible and is instead considered to be shadowing
				//return saturate(50.0 * SpecularColor.g) * Fc + (1 - Fc) * SpecularColor;

				return SpecularColor + (1 - SpecularColor) * Fc;

			}

			float3 Diffuse_Lambert(float3 DiffuseColor)
			{
				return DiffuseColor * (1 / PI);
			}

            v2f vert (appdata v)
            {
                v2f o;

				float3 worldPos = mul(UNITY_MATRIX_M, v.vertex).xyz;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.lightDir = normalize(_WorldSpaceLightPos0.xyz - worldPos);
				//o.lightDir = normalize(float3(1, 1, 1));
				o.lightDir = _WorldSpaceLightPos0.xyz;
				
				o.viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				//o.viewDir = _WorldSpaceCameraPos.xyz;
				
				//float3 viewpos = UnityObjectToViewPos(vertex.xyz);

                return o;
            }

			float DielectricSpecularToF0(float Specular)
			{
				return 0.08f * Specular;
			}

			float3 ComputeF0(float Specular, float3 BaseColor, float Metallic)
			{
				return lerp(DielectricSpecularToF0(Specular).xxx, BaseColor, Metallic.xxx);
			}

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				half3 normal = tex2D(_NormalTex, i.uv).rbg * 2 - 1;
				//half3 normal = UnpackNormal(tex2D(_NormalTex, i.uv));
				float3 N =  normalize(mul((float3x3)transpose(UNITY_MATRIX_M), normal.xyz));
				float3 V = normalize(i.viewDir);
				float3 L = normalize(i.lightDir);
				float3 H = normalize(V + L);
				float NoH = saturate(dot(N, H));
				float NoV = saturate(dot(N, V));
				float NoL = saturate(dot(N, L));
				float VoH = saturate(dot(V, H));

				float specularValue = tex2D(_SpecularTex, i.uv).r;
				float Metallic = _Metallic;// 0.035;// tex2D(_MetallicTex, i.uv).r;
				float Roughness = tex2D(_RoughnessTex, i.uv).r;
				float AO = 1;// tex2D(_AOTex, i.uv).r * NoL;

				//float Roughness;
				float3 SpecularColor = ComputeF0(specularValue, col, _Metallic);

				float a2 = pow(Roughness, 4);

				float D = (D_GGX(a2, VoH));
				float Vis = (Vis_SmithJoint(a2, NoV, NoL));
				float3 F = F_Schlick(SpecularColor, VoH);

				float3 Specular = F * (D * Vis) *NoL;
				float3 BaseColor = col.rgb;
				float3 Diffuse = Diffuse_Lambert(BaseColor - BaseColor * Metallic) *NoL;

				float lightIns = /*AO * */_LightInstense;
				col.rgb = (Specular + Diffuse)* lightIns;
				//col.rgb = (/*Specular + */Diffuse)* lightIns;
				//col.rgb = (Specular + Diffuse) * lightIns;
				//col.rgb = NoL;
				//col.rgb = Specular * lightIns;

                return col;
            }
            ENDCG
        }
    }
}
