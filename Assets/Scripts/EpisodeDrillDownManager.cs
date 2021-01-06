using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EpisodeDrillDownManager : MonoBehaviour
{
    public static EpisodeDrillDownManager Instance { get; private set; }
    public EpisodeBehavior DrilledEpisode { get; private set; }

    public CanvasGroup AlphaElement;
    public TextMeshProUGUI Title;
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
            ImdbRating.text = DrilledEpisode.Data.ImdbRating.ToString();
            Nelson.text = DrilledEpisode.Data.NealsonRating.ToString();
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