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

	void Start ()
    {
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
}
