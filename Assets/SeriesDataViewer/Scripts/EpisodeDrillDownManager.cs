using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EpisodeDrillDownManager : MonoBehaviour
{
    public static EpisodeDrillDownManager Instance { get; private set; }
    public EpisodeBehavior DrilledEpisode { get; private set; }

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
        DrilledEpisode = GetDrilledEpisode();
        UpdateTitleColor();
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

    private void UpdateDepthOfField()
    {
        PostProcessVolume volume = Camera.main.GetComponent<PostProcessVolume>();
        DepthOfField depth = volume.profile.GetSetting<DepthOfField>();
        depth.active = DrilledEpisode != null;
        if(DrilledEpisode != null)
        {
            float distance = (Camera.main.transform.position - DrilledEpisode.transform.position).magnitude;
            depth.focusDistance.value = distance;
        }
    }

    private void UpdateTitleColor()
    {
        MainScript.Instance.TitleText.color = Color.Lerp(ActiveTitleColor, InactiveTitleColor, AlphaElement.alpha);
    }

    private void UpdateAlphaElement()
    {
        float alphaTarget = DrilledEpisode != null ? 1f : 0;
        AlphaElement.alpha = Mathf.Lerp(alphaTarget, AlphaElement.alpha, Time.deltaTime * 30);
    }

    private EpisodeBehavior GetDrilledEpisode()
    {
        if (MouseInteractionManager.Instance.DrilldownEnabled)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                return hit.collider.gameObject.GetComponent<EpisodeBehavior>();
            }
        }
        return null;
    }
}