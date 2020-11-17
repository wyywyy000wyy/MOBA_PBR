Shader "YSTech/GroundGlass"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		_NormalTex("Normal Tex", 2D) = "white" {}
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
            float4 _MainTex_ST;

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
				//float3 viewpos = UnityObjectToViewPos(vertex.xyz);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				half3 normal = tex2D(_NormalTex, i.uv).rgb;
				normal = normal.xyz * 2 - 1;
				//half3 normal = UnpackNormal(tex2D(_NormalTex, i.uv));
				float3 worldNormal =  normalize(mul((float3x3)transpose(UNITY_MATRIX_M), normal.xyz));
				float3 viewDir = normalize(i.viewDir);
				float3 H = normalize(i.lightDir + viewDir);

				float dx = 0.6;

				float4 diffuse = dx *  max(0, dot(worldNormal, i.lightDir));

				float shininess = 256;
				float4 specular = (1 - dx) * pow(max(0, dot(H, worldNormal)), shininess);

				col = col * (diffuse + specular);
				col.rgb = i.lightDir;
				col.rgb = normal;
                // apply fog
                return col;
            }
            ENDCG
        }
    }
}
