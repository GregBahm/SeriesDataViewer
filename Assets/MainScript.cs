﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

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

    public float HeightScale;
    public float HighestNelson { get; set; }

    public TextAsset[] SeriesAssets;
    private List<SeriesScript> eachSeries;

    public GameObject EpisodePrefab;
    public int ShowToShow;
    
    private void Awake()
    {
        Instance = this;
    }

    void Start ()
    {
        eachSeries = new List<SeriesScript>();
        foreach (TextAsset dataSource in SeriesAssets)
        {
            eachSeries.Add(LoadShow(dataSource));
        }
        HighestNelson = eachSeries.Max(item => item.MaxNealson);
    }

    private SeriesScript LoadShow(TextAsset dataSource)
    {
        GameObject obj = new GameObject(dataSource.name);
        SeriesScript ret = obj.AddComponent<SeriesScript>();
        ret.Episodes = DataLoader.LoadData(dataSource);
        ret.MaxNealson = ret.Episodes.Max(item => item.NealsonRating);
        ret.MaxSeason = ret.Episodes.Max(item => item.Season);
        ret.MaxEpisode = ret.Episodes.Max(item => item.Episode);
        obj.transform.SetParent(RootTransform, false);
        return ret;
    }

    private void Update()
    {
        for (int i = 0; i < eachSeries.Count; i++)
        {
            var series = eachSeries[i];
            series.gameObject.SetActive(i == ShowToShow);
        }
    }
}