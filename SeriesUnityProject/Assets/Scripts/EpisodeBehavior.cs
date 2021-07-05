using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EpisodeBehavior : MonoBehaviour
{
    private MaterialPropertyBlock episodeBlock;

    public EpisodeData Data { get; set; }
    public ShowBehavior Series { get; set; }
    public Color Color { get; set; }
    public Color EmissiveColor { get; set; }

    public MeshRenderer Renderer { get; private set; }

    private void Awake()
    {
        if(episodeBlock == null)
        {
            episodeBlock = new MaterialPropertyBlock();
        }
    }

    private void Start()
    {
        Renderer = GetComponent<MeshRenderer>();
    }

    float drilledLightingFactor;
    float drilledLightingGlow;
    float drilledScaleFactor;

    private float GetDrillLightTarget()
    {
        if (MainScript.Instance.DrilledEpisode == null)
        {
            return 1f;
        }
        return MainScript.Instance.DrilledEpisode == this ? 2 : .5f;
    }
    private float GetDrillLightEmissiveTarget()
    {
        return MainScript.Instance.DrilledEpisode == this ? 1 : 0f;
    }

    private float GetDrillScaleTarget()
    {
        return MainScript.Instance.DrilledEpisode == this ? .5f : 0;
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

        Color = GetColor(baseImdb, drilledLightingFactor);
        EmissiveColor = Color * MainScript.Instance.BarEmissive * drilledLightingGlow;

        Color = Color.Lerp(Color, Color.white, MainScript.Instance.ShowColor);
        EmissiveColor = Color.Lerp(Color, Color.black, MainScript.Instance.ShowColor * MainScript.Instance.NealsonOrImdb);

        episodeBlock.SetColor("_BaseColor", Color);
        episodeBlock.SetColor("_EmissiveColor", EmissiveColor);
        Renderer.SetPropertyBlock(episodeBlock);
        //Mat.SetColor("_BaseColor", Color);
        //Mat.SetColor("_EmissiveColor", EmissiveColor);
        //Mat.SetFloat("_Smoothness", MainScript.Instance.BarGloss);
        //Mat.SetFloat("_Metallic", MainScript.Instance.BarMetallic);
    }
    private Color GetColor(float baseImdb, float drilledLightingFactor)
    {
        float scaled = (MainScript.Instance.ColorKeys.Length - 1) * baseImdb;
        int lowKey = Mathf.FloorToInt(scaled);
        int highKey = Mathf.CeilToInt(scaled);
        float lerp = scaled % 1;
        Color lowColor = MainScript.Instance.ColorKeys[lowKey];
        Color highColor = MainScript.Instance.ColorKeys[highKey];
        Color col = Color.Lerp(lowColor, highColor, lerp);
        col *= col.a * 10;

        col *= drilledLightingFactor;
        return col;
    }
}