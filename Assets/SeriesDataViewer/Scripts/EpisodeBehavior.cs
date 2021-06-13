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

    float drilledLightingFactor;
    float drilledLightingGlow;
    float drilledScaleFactor;

    private float GetDrillLightTarget()
    {
        if (EpisodeDrillDownManager.Instance.DrilledEpisode == null)
        {
            return 1f;
        }
        return EpisodeDrillDownManager.Instance.DrilledEpisode == this ? 2 : .5f;
    }
    private float GetDrillLightEmissiveTarget()
    {
        return EpisodeDrillDownManager.Instance.DrilledEpisode == this ? 1 : 0f;
    }

    private float GetDrillScaleTarget()
    {
        return EpisodeDrillDownManager.Instance.DrilledEpisode == this ? .5f : 0;
    }

    private void UpdateDrillFactor()
    {
        float targetDrilledFactor = GetDrillLightTarget();
        drilledLightingFactor = Mathf.Lerp(targetDrilledFactor, drilledLightingFactor, Time.deltaTime * 15);

        float targetDrilledGlow = GetDrillLightEmissiveTarget();
        drilledLightingGlow = Mathf.Lerp(targetDrilledGlow, drilledLightingGlow, Time.deltaTime * 15);

        float targetDrillScale = GetDrillScaleTarget();
        drilledScaleFactor = Mathf.Lerp(targetDrillScale, drilledScaleFactor, Time.deltaTime * 15);
    }

    private void Update()
    {
        UpdateDrillFactor();

        float baseImdb = Data.ImdbRating / 10;

        float nealsonScale = Data.NealsonRating / MainScript.Instance.HighestNelson * MainScript.Instance.HeightScale + .5f;
        float nealsonHeightPos = nealsonScale / 2 - .5f;

        float imdbTop = MainScript.Instance.HeightScale - MainScript.Instance.ImdbScale / 2;
        float imdbBotom = MainScript.Instance.ImdbScale / 2;

        float imdbHeightPos = Mathf.LerpUnclamped(imdbBotom, imdbTop, baseImdb - .5f);
        float imdbScale = MainScript.Instance.ImdbScale;

        float heightPos = Mathf.Lerp(nealsonHeightPos, imdbHeightPos, MainScript.Instance.NealsonOrImdb);
        float heightScale = Mathf.Lerp(nealsonScale, imdbScale, MainScript.Instance.NealsonOrImdb);

        transform.localScale = new Vector3(MainScript.Instance.SpaceBetweenSeasons, heightScale, MainScript.Instance.SpaceBetweenEpisodes);
        transform.localScale += new Vector3(drilledScaleFactor, drilledScaleFactor, drilledScaleFactor);
        transform.localPosition = new Vector3(-(Data.Season - 1), heightPos, - (Data.Episode - 1));

        Color col = GetColor(baseImdb, drilledLightingFactor);
        Mat.SetColor("_BaseColor", col);
        Mat.SetColor("_EmissiveColor", col * MainScript.Instance.BarEmissive * drilledLightingGlow);
        Mat.SetFloat("_Smoothness", MainScript.Instance.BarGloss);
        Mat.SetFloat("_Metallic", MainScript.Instance.BarMetallic);
    }

    private static readonly Color goodColor = new Color(0, 2, 1);
    private static readonly Color badColor = new Color(2, 0, .5f );
    private static readonly Color dimmedColor = new Color(.5f, 0, 0);

    private Color GetColor(float baseImdb, float drilledLightingFactor)
    {
        float remap = Mathf.Pow(baseImdb, 2.5f);
        Color col = Color.Lerp(badColor, goodColor, remap);
        col = new Color(ModifyAethetic(col.r), ModifyAethetic(col.g), ModifyAethetic(col.b));
        col *= drilledLightingFactor;
        //col = Color.Lerp(col, dimmedColor, drilledLightingFactor);
        //col = Color.Lerp(MainScript.Instance.BarTint, col, MainScript.Instance.BarTint.a);
        return col;
    }

    private float ModifyAethetic(float val)
    {
        return Mathf.Pow(val, .5f) * 3 - 2;
    }
}