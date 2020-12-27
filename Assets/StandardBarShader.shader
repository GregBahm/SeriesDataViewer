Shader "Custom/StandardBarShader"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _BarGlossiness;
        half _BarMetallic;
        half _BarEmissiveStrength;

        fixed4 _BarTint;

        float _SeasonParam;
        float _EpisodeParam;
        float _ImdbParam;
        float _NealsonOrImdb;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        float3 GetColor()
        {
            float3 color = lerp(float3(1, 0, 0), float3(0, 1, 0), _ImdbParam);
            float minColor = max(color.x, color.y);
            color = color / minColor;
            return color;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 color = GetColor();
            o.Albedo = lerp(_BarTint.xyz, color, _BarTint.a);
            o.Emission = color * _BarEmissiveStrength;
            o.Metallic = _BarMetallic;
            o.Smoothness = _BarGlossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
