using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpisodeDrillDownManager : MonoBehaviour
{
    public static EpisodeDrillDownManager Instance { get; private set; }
    public EpisodeBehavior DrilledEpisode { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        DrilledEpisode = GetDrilledEpisode();
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