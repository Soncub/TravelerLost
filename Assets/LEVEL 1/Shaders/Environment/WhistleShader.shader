Shader "Custom/WhistleShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionIntensity ("Emission Intensity", Range(0, 5)) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        CGPROGRAM
        #pragma surface surf Lambert
        
        struct Input
        {
            float2 uv_MainTex;
        };
        
        fixed4 _Color;
        fixed4 _EmissionColor;
        float _EmissionIntensity;
        
        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = _Color.rgb;
            o.Emission = _EmissionColor.rgb * _EmissionIntensity;
        }
        
        ENDCG
    }
    
    FallBack "Diffuse"
}
