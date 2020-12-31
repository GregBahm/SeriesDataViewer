﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EpisodeBehavior : MonoBehaviour
{
    public EpisodeData Data { get; set; }
    public ShowBehavior Series { get; set; }

    private Material Mat;

    private void Start()
    {
        Mat = GetComponent<MeshRenderer>().material;
    }

    private float GetImdbVal(float sourceImdbVal)
    {
        return (sourceImdbVal - MainScript.Instance.ImdbMin) / (MainScript.Instance.ImdbMax - MainScript.Instance.ImdbMin);
    }

    private void Update()
    {
        float baseImdb = GetImdbVal(Data.ImdbRating);
        float baseForHeight = GetImdbVal(Data.ImdbRating - 5);

        float nealsonScale = Data.NealsonRating / MainScript.Instance.HighestNelson * MainScript.Instance.HeightScale;
        float nealsonHeightPos = nealsonScale / 2;

        float imdbTop = MainScript.Instance.HeightScale - MainScript.Instance.ImdbScale / 2;
        float imdbBotom = MainScript.Instance.ImdbScale / 2;

        float imdbHeightPos = Mathf.LerpUnclamped(imdbBotom, imdbTop, baseForHeight);
        float imdbScale = MainScript.Instance.ImdbScale;

        float heightPos = Mathf.Lerp(nealsonHeightPos, imdbHeightPos, MainScript.Instance.NealsonOrImdb);
        float heightScale = Mathf.Lerp(nealsonScale, imdbScale, MainScript.Instance.NealsonOrImdb);

        transform.localScale = new Vector3(MainScript.Instance.SpaceBetweenSeasons, heightScale, MainScript.Instance.SpaceBetweenEpisodes);
        transform.localPosition = new Vector3(-(Data.Season - 1), heightPos, - (Data.Episode - 1));
        Mat.SetFloat("_SeasonParam", Data.Season / Series.MaxSeason);
        Mat.SetFloat("_EpisodeParam", Data.Episode / Series.MaxEpisode);
        Mat.SetFloat("_ImdbParam", baseImdb);
        Mat.SetFloat("_NealsonOrImdb", MainScript.Instance.NealsonOrImdb);
        Mat.SetFloat("_DataAvailable", Data.NealsonRating > 0 ? 1 : 0);
    }
}