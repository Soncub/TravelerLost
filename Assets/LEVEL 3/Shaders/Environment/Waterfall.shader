Shader "Custom/Waterfall"
{
    Properties
    {
        _MainTex ("Water Texture", 2D) = "white" {}
        _WaterColor ("Water Color", Color) = (0.2, 0.5, 0.8, 1)
        _Speed ("Flow Speed", Range(0.1, 5)) = 1
        _Tiling ("Tiling", Range(0.1, 5)) = 1
        _Transparency ("Transparency", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha // Enables transparency

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata_t
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _WaterColor;
            float _Speed;
            float _Tiling;
            float _Transparency;
            CBUFFER_END

            v2f vert(appdata_t v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS);

                // Rotate UVs 90 degrees to ensure proper downward flow
                float2 rotatedUV = float2(v.uv.y, 1.0 - v.uv.x);

                // Scroll UVs downward over time
                rotatedUV.y += _Time.y * _Speed;

                // Apply tiling
                o.uv = rotatedUV * _Tiling;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Sample water texture
                half4 waterTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * _WaterColor;

                // Apply transparency
                waterTex.a *= _Transparency;
                return waterTex;
            }
            ENDHLSL
        }
    }
}
