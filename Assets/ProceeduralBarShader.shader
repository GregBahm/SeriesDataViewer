Shader "Unlit/ProceeduralBarShader"
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
            
			struct MeshData
			{
				float3 Position;
				float3 Normal;
			};

            struct BoxData
            {
                float Season;
                float Episode;
                float ImdbRating;
                float NealsonRating;
                float DataAvailable;
            };

            StructuredBuffer<MeshData> _MeshBuffer;
            StructuredBuffer<BoxData> _DataBuffer;

            float4x4 _MasterMatrix;

			struct v2f
			{
                float3 normal : NORMAL;
				float4 vertex : SV_POSITION;
                float3 objSpace : TEXCOORD1;
                float3 worldSpace : TEXCOORD2;
                float DataAvailable : TEXCOORD3;
                float ImdbParam : TEXCOORD4;
			};

            float _NealsonOrImdb;
            float _HeightScale;
            float _ImdbScale;
            float _SpaceBetweenSeasons;
            float _SpaceBetweenEpisodes;
            float _ImdbMin;
            float _ImdbMax;
            float _HighestNelson;

            float GetImdbParam(float imdbRating)
            {
                return (imdbRating - _ImdbMin) / (_ImdbMax - _ImdbMin);
            }

            float3 GetBoxPos(MeshData meshData, BoxData boxData)
            {
                //meshData.Position + float3(boxData.Season, 0, boxData.Episode);

                float nealsonScale = boxData.NealsonRating / _HighestNelson * _HeightScale;

                float imdbParam = GetImdbParam(boxData.ImdbRating);
                float imdbTop = _HeightScale - _ImdbScale / 2;
                float imdbBotom = _ImdbScale / 2;
                float imdbHeightPos = lerp(imdbBotom, imdbTop, imdbParam);

                float nealsonY = nealsonScale * (meshData.Position.y + .5);
                float imdbY = imdbHeightPos + _ImdbScale * meshData.Position.y;

                float retY = lerp(nealsonY, imdbY, _NealsonOrImdb);;
                float retX = _SpaceBetweenSeasons * meshData.Position.x + -boxData.Season;
                float retZ = _SpaceBetweenEpisodes * meshData.Position.z + -boxData.Episode;
                return float3(retX, retY, retZ);
            }

            v2f vert(uint meshId : SV_VertexID, uint instanceId : SV_InstanceID)
            {
                MeshData meshData = _MeshBuffer[meshId];
                BoxData boxData = _DataBuffer[instanceId];

                float3 vertPos = GetBoxPos(meshData, boxData);
                float4 worldPos = mul(_MasterMatrix, float4(vertPos, 1));

				v2f o;
                o.vertex = UnityObjectToClipPos(worldPos);
				o.normal = meshData.Normal;
				o.objSpace = meshData.Position;
                o.worldSpace = worldPos;
                o.DataAvailable = boxData.DataAvailable;
                o.ImdbParam = GetImdbParam(boxData.ImdbRating);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
                float shade = abs(i.objSpace.y) * 2;
                
                float normalShade = i.normal.z + i.normal.x;
                shade = abs(pow(shade, 5));

                float3 color = lerp(float3(1, 0, 0), float3(0, 1, 0), i.ImdbParam);
                float minColor = max(color.x, color.y);
                color = color / minColor;
                color += color * shade;
                color /= 2;

                float objX = 1 - abs(i.objSpace.x * 2);
                float objZ = 1 - abs(i.objSpace.z * 2);
                float dataAvailableAlpha = 1 - min(objX, objZ);
                float3 dataAvailableColor = lerp(1, color, pow(dataAvailableAlpha, 4));
                color = lerp (color, dataAvailableColor, (1 - i.DataAvailable) * (1 - _NealsonOrImdb));
                return float4(color, 1);
			}
			ENDCG
		}
	}
}
