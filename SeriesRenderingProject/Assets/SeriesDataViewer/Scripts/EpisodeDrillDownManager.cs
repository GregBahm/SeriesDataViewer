using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EpisodeDrillDownManager : MonoBehaviour
{
    public static EpisodeDrillDownManager Instance { get; private set; }

    public CanvasGroup AlphaElement;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI SeasonEpisode;
    public TextMeshProUGUI ImdbRating;
    public TextMeshProUGUI Nelson;

    public Color InactiveTitleColor;
    public Color ActiveTitleColor;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdateAlphaElement();
        if(MainScript.Instance.DrilledEpisode != null)
        {
            EpisodeBehavior episode = MainScript.Instance.DrilledEpisode;
            Title.text = episode.Data.Title;
            SeasonEpisode.text = "Season " + episode.Data.Season + " episode " + episode.Data.Episode;
            ImdbRating.text = episode.Data.ImdbRating.ToString();
            Nelson.text = episode.Data.NealsonRating.ToString();
        }
    }

    private void UpdateAlphaElement()
    {
        float alphaTarget = MainScript.Instance.DrilledEpisode != null ? 1f : 0;
        AlphaElement.alpha = Mathf.Lerp(alphaTarget, AlphaElement.alpha, Time.deltaTime * 30);
    }
}