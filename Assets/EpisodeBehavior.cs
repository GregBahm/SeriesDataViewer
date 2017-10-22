using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class EpisodeBehavior : MonoBehaviour
{
    public EpisodeData Data { get; set; }
    public SeriesScript Mothership { get; set; }

    private Material Mat;

    private void Start()
    {
        Mat = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        float baseImdb = (Data.ImdbRating - Mothership.ImdbMin) / (Mothership.ImdbMax - Mothership.ImdbMin);
        float adjustedImdb = baseImdb - .5f;

        float nealsonHeightPos = Data.NealsonRating / 2;
        float nealsonScale = Data.NealsonRating;

        float imdbTop = Mothership.MaxNealson - Mothership.ImdbScale / 2;
        float imdbBotom = Mothership.ImdbScale / 2;

        float imdbHeightPos = Mathf.Lerp(imdbBotom, imdbTop, baseImdb);
        float imdbScale = Mothership.ImdbScale;

        float heightPos = Mathf.Lerp(nealsonHeightPos, imdbHeightPos, Mothership.NealsonOrImdb);
        float heightScale = Mathf.Lerp(nealsonScale, imdbScale, Mothership.NealsonOrImdb);

        transform.localScale = new Vector3(Mothership.SpaceBetweenSeasons, heightScale, Mothership.SpaceBetweenEpisodes);
        transform.localPosition = new Vector3(-Data.Season, heightPos, Data.Episode);
        Mat.SetFloat("_SeasonParam", Data.Season / Mothership.MaxSeason);
        Mat.SetFloat("_EpisodeParam", Data.Episode / Mothership.MaxEpisode);
        Mat.SetFloat("_ImdbParam", baseImdb);
        Mat.SetFloat("_NealsonOrImdb", Mothership.NealsonOrImdb);
    }
}