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

        // Main Boulder Pass (Handles all light sources)
        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float4 _LightColor;
        float4 _DarkColor;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Sample texture
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);

            // Get lighting intensity (supports all lights)
            float lightIntensity = saturate(dot(IN.worldNormal, normalize(_WorldSpaceLightPos0.xyz)));

            // Smooth transition from dark to light
            float gradient = smoothstep(0.2, 0.8, lightIntensity);
            fixed4 lightingColor = lerp(_DarkColor, _LightColor, gradient);

            // Apply shading and texture
            o.Albedo = texColor.rgb * lightingColor.rgb;
        }
        ENDCG
    }
}
