using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class EpisodeBehavior : MonoBehaviour
{
    public EpisodeData Data { get; set; }
    public MainScript Main { get; set; }
    public SeriesScript Series { get; set; }

    private Material Mat;

    private void Start()
    {
        Mat = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        float baseImdb = (Data.ImdbRating - Main.ImdbMin) / (Main.ImdbMax - Main.ImdbMin);

        float nealsonScale = Data.NealsonRating / Main.HighestNelson * Main.HeightScale;
        float nealsonHeightPos = nealsonScale / 2;

        float imdbTop = Main.HeightScale - Main.ImdbScale / 2;
        float imdbBotom = Main.ImdbScale / 2;

        float imdbHeightPos = Mathf.LerpUnclamped(imdbBotom, imdbTop, baseImdb);
        float imdbScale = Main.ImdbScale;

        float heightPos = Mathf.Lerp(nealsonHeightPos, imdbHeightPos, Main.NealsonOrImdb);
        float heightScale = Mathf.Lerp(nealsonScale, imdbScale, Main.NealsonOrImdb);

        transform.localScale = new Vector3(Main.SpaceBetweenSeasons, heightScale, Main.SpaceBetweenEpisodes);
        transform.localPosition = new Vector3(-Data.Season, heightPos, - Data.Episode);
        Mat.SetFloat("_SeasonParam", Data.Season / Series.MaxSeason);
        Mat.SetFloat("_EpisodeParam", Data.Episode / Series.MaxEpisode);
        Mat.SetFloat("_ImdbParam", baseImdb);
        Mat.SetFloat("_NealsonOrImdb", Main.NealsonOrImdb);
    }
}