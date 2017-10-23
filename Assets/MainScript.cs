using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class MainScript : MonoBehaviour
{
    [Range(0, 1)]
    public float NealsonOrImdb;

    [Range(0, 1)]
    public float SpaceBetweenSeasons;
    [Range(0, 1)]
    public float SpaceBetweenEpisodes;

    [Range(0, 10)]
    public float ImdbMin;
    [Range(0, 10)]
    public float ImdbMax;
    public float ImdbScale;

    public float HeightScale;
    public float HighestNelson { get; set; }

    public Material BaseMaterial;

    public TextAsset[] SeriesAssets;
    private List<SeriesScript> eachSeries;

    public Mesh BoxMesh;
    private ComputeBuffer meshBuffer;
    private const int MeshBufferStride = sizeof(float) * 3 + sizeof(float) * 3;

    struct MeshData
    {
        public Vector3 Position;
        public Vector3 Normal;
    }

    void Start ()
    {
        meshBuffer = GetMeshBuffer();

        eachSeries = new List<SeriesScript>();
        int episodeCount = 0;
        foreach (TextAsset dataSource in SeriesAssets)
        {
            GameObject obj = new GameObject(dataSource.name);
            SeriesScript script = obj.AddComponent<SeriesScript>();
            script.Episodes = DataLoader.LoadData(dataSource);
            script.MaxNealson = script.Episodes.Max(item => item.NealsonRating);
            script.MaxSeason = script.Episodes.Max(item => item.Season);
            script.MaxEpisode = script.Episodes.Max(item => item.Episode);
            script.Main = this;
            obj.transform.position = new Vector3(0, 0, -episodeCount);
            episodeCount += (int)script.MaxEpisode + 10; // + 5 for margin
            eachSeries.Add(script);
        }
        HighestNelson = eachSeries.Max(item => item.MaxNealson);
    }

    private void Update()
    {
        foreach (SeriesScript series in eachSeries)
        {
            series.SeriesMaterial.SetBuffer("_MeshBuffer", meshBuffer);
            Shader.SetGlobalFloat("_NealsonOrImdb", NealsonOrImdb);
            Shader.SetGlobalFloat("_SpaceBetweenSeasons", SpaceBetweenSeasons);
            Shader.SetGlobalFloat("_SpaceBetweenEpisodes", SpaceBetweenEpisodes);
            Shader.SetGlobalFloat("_ImdbMin", ImdbMin);
            Shader.SetGlobalFloat("_ImdbMax", ImdbMax);
            Shader.SetGlobalFloat("_HighestNelson", HighestNelson);
            Shader.SetGlobalFloat("_HeightScale", HeightScale);
            Shader.SetGlobalFloat("_ImdbScale", ImdbScale);
        }
    }

    private ComputeBuffer GetMeshBuffer()
    {
        int meshBufferCount = BoxMesh.triangles.Length;
        ComputeBuffer ret = new ComputeBuffer(meshBufferCount, MeshBufferStride);

        MeshData[] meshVerts = new MeshData[meshBufferCount];
        for (int i = 0; i < meshBufferCount; i++)
        {
            meshVerts[i].Position = BoxMesh.vertices[BoxMesh.triangles[i]];
            meshVerts[i].Normal = BoxMesh.normals[BoxMesh.triangles[i]];
        }
        ret.SetData(meshVerts);
        return ret;
    }

    private void OnDestroy()
    {
        meshBuffer.Release();
    }
}
