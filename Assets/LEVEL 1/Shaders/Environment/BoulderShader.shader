Shader "Custom/BoulderShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LightColor ("Light Color", Color) = (1,1,1,1)
        _DarkColor ("Dark Color", Color) = (0.5,0.5,0.5,1)
        _OutlineWidth ("Outline Width", Range(0.001, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        // Outline Pass
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            Cull Front  // Render back faces to create an outline
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                float3 normal = normalize(v.normal) * _OutlineWidth;
                v.vertex += float4(normal, 0); // Push vertices outward
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(0,0,0,1); // Black outline
            }
            ENDCG
        }

        // Main Boulder Pass
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            Cull Back  // Render front faces normally

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _LightColor;
            float4 _DarkColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float lightIntensity = dot(normalize(i.worldNormal), float3(0, 1, 0));
                float gradient = smoothstep(0.3, 0.7, lightIntensity);
                return lerp(_DarkColor, _LightColor, gradient);
            }
            ENDCG
        }
    }
}