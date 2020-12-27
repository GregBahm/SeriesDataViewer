using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

public class MainScript : MonoBehaviour
{
    public static MainScript Instance;

    public Transform RootTransform;

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

    [Range(0, 1)]
    public float BarGloss;
    [Range(0, 1)]
    public float BarMetallic;
    [Range(0,1)]
    public float BarEmissive;
    public Color BarTint;

    public float HeightScale;
    public float HighestNelson { get; set; }

    public TextAsset[] SeriesAssets;
    private List<SeriesBehavior> eachSeries;

    public GameObject EpisodePrefab;
    public int ShowToShow;

    public SeriesBehavior ShownSeries { get { return eachSeries[ShowToShow]; } }
    
    private void Awake()
    {
        Instance = this;
    }

    void Start ()
    {
        eachSeries = new List<SeriesBehavior>();
        foreach (TextAsset dataSource in SeriesAssets)
        {
            eachSeries.Add(LoadShow(dataSource));
        }
        HighestNelson = eachSeries.Max(item => item.Episodes.Max(ep => ep.NealsonRating));
    }

    private SeriesBehavior LoadShow(TextAsset dataSource)
    {
        GameObject obj = new GameObject(dataSource.name);
        SeriesBehavior ret = obj.AddComponent<SeriesBehavior>();
        ret.Episodes = DataLoader.LoadData(dataSource);
        obj.transform.SetParent(RootTransform, false);
        return ret;
    }

    private void Update()
    {
        ShowToShow = ShowToShow % eachSeries.Count;
        UpdateSeriesVisibility();
        UpdateShaderParameters();
    }

    private void UpdateShaderParameters()
    {
        Shader.SetGlobalFloat("_BarGlossiness", BarGloss);
        Shader.SetGlobalFloat("_BarMetallic", BarMetallic);
        Shader.SetGlobalFloat("_BarEmissiveStrength", BarEmissive);
        Shader.SetGlobalColor("_BarTint", BarTint);
    }

    private void UpdateSeriesVisibility()
    {
        for (int i = 0; i < eachSeries.Count; i++)
        {
            var series = eachSeries[i];
            series.gameObject.SetActive(i == ShowToShow);
        }
    }
}