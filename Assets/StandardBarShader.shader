Shader "Custom/StandardBarShader"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200
        Stencil {
            Ref 2
            Comp always
            Pass replace
        }

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows
            #pragma target 3.0

            struct Input
            {
                float2 uv_MainTex;
            };

            half _BarGlossiness;
            half _BarMetallic;
            half _BarEmissiveStrength;

            fixed4 _BarTint;

            float _ImdbParam;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_INSTANCING_BUFFER_END(Props)

            float3 GetColor()
            {
                float3 goodColor = float3(0, 2, 1);
                float3 badColor = float3(2, 0, .5);
                float3 color = lerp(badColor, goodColor, _ImdbParam);
                color = pow(color, .5) * 3 - 2;
                //float minColor = max(color.x, color.y);
                //color = color / minColor;
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