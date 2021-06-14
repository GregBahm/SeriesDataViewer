using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EpisodeDrillDownManager : MonoBehaviour
{
    public static EpisodeDrillDownManager Instance { get; private set; }
    public EpisodeBehavior DrilledEpisode { get; set; }

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
        if(DrilledEpisode != null)
        {
            Title.text = DrilledEpisode.Data.Title;
            SeasonEpisode.text = "Season " + DrilledEpisode.Data.Season + " episode " + DrilledEpisode.Data.Episode;
            ImdbRating.text = DrilledEpisode.Data.ImdbRating.ToString();
            Nelson.text = DrilledEpisode.Data.NealsonRating.ToString();
        }
        //UpdateDepthOfField();
    }

    private void UpdateAlphaElement()
    {
        float alphaTarget = DrilledEpisode != null ? 1f : 0;
        AlphaElement.alpha = Mathf.Lerp(alphaTarget, AlphaElement.alpha, Time.deltaTime * 30);
    }
}