Shader "Custom/Shader" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		struct Input 
        {
			float2 uv_MainTex;
		};


    float _SeasonParam;
    float _EpisodeParam;
    float _ImdbParam;
    float _NealsonParam;
    float _NealsonOrImdb;
    float _MaxHeight;

		UNITY_INSTANCING_CBUFFER_START(Props)
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) 
        {
            float3 color = lerp(float3(1, 0, 0), float3(0, 1, 0), _ImdbParam);
            o.Albedo = color / 2;// float3(_SeasonParam, _SeasonParam, _SeasonParam);
            o.Emission = color / 2;
			o.Metallic = 0;
			o.Smoothness = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
