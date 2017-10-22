﻿Shader "Unlit/UnlitShader"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
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
				float4 vertex : SV_POSITION;
                float3 objSpace : TEXCOORD1;
                float3 worldSpace : TEXCOORD2;
                float3 normal : NORMAL;
			};

            float _SeasonParam;
            float _EpisodeParam;
            float _ImdbParam;
            float _NealsonOrImdb;
            float _DataAvailable;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldSpace = mul(unity_ObjectToWorld, v.vertex);
                o.objSpace = v.vertex;
                o.normal = v.normal;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
                float shade = abs(i.objSpace.y) * 2;
                
                float normalShade = i.normal.z + i.normal.x;
                shade = abs(pow(shade, 5));

                float3 color = lerp(float3(1, 0, 0), float3(0, 1, 0), _ImdbParam);
                float minColor = max(color.x, color.y);
                color = color / minColor;
                color += color * shade;
                color /= 2;

                float objX = 1 - abs(i.objSpace.x * 2);
                float objZ = 1 - abs(i.objSpace.z * 2);
                float dataAvailableAlpha = 1 - min(objX, objZ);
                float3 dataAvailableColor = lerp(1, color, pow(dataAvailableAlpha, 4));
                color = lerp (color, dataAvailableColor, (1 - _DataAvailable) * (1 - _NealsonOrImdb));
                return float4(color, 1);
			}
			ENDCG
		}
	}
        FallBack "Diffuse"
}