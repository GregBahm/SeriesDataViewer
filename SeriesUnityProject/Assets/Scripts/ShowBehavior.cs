﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using System.Collections.ObjectModel;

public class ShowBehavior : MonoBehaviour
{
    public ReadOnlyCollection<EpisodeData> Episodes { get; set; }
    public ReadOnlyCollection<EpisodeBehavior> EpisodeBehaviors { get; private set; }
    public float MaxNealson { get; private set; }
    public float MaxSeason { get; private set; }
    public float MaxEpisode { get; private set; }

    private Transform labelsTransform;
    private List<TextMeshPro> labels;
    private StageBoxManager stageBox;
    public Transform InnerStageBox { get { return stageBox.InnerBox; } }
    
    public float ScoreMid { get; private set; }

    void Start ()
    {
        MaxNealson = Episodes.Max(item => item.NealsonRating);
        MaxSeason = Episodes.Max(item => item.Season);
        MaxEpisode = Episodes.Max(item => item.Episode);
        
        EpisodeBehaviors = CreateEpisodeBoxes().ToList().AsReadOnly();
        labelsTransform = new GameObject("Labels").transform;
        labelsTransform.SetParent(transform, false);

        labels = new List<TextMeshPro>();
        labels.AddRange(CreateEpisodeLabel());
        labels.AddRange(CreateSeasonLabel());
        labels.AddRange(CreateSeasonNumbers());
        labels.AddRange(CreateEpisodeNumbers());
        stageBox = new StageBoxManager(transform, MaxSeason, MaxEpisode);

        ScoreMid = GetScoreMid();
	}

    private float GetScoreMid()
    {
        float maxY = Episodes.Max(item => item.ImdbRating);
        float minY = Episodes.Min(item => item.ImdbRating);
        return (maxY + minY) / 2;
    }

    private void Update()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        stageBox.UpdateStage(MainScript.Instance.NealsonOrImdb);
    }

    private IEnumerable<EpisodeBehavior> CreateEpisodeBoxes()
    {
        List<EpisodeBehavior> behaviors = new List<EpisodeBehavior>();
        Transform episodesTransform = new GameObject("Episodes").transform;
        episodesTransform.transform.SetParent(transform, false);
        episodesTransform.localPosition = new Vector3((MaxSeason - 1) / 2, 0, (MaxEpisode - 1) / 2);

        foreach (EpisodeData data in Episodes)
        {
            EpisodeBehavior newBehavior = CreateNewEpisodeBox(data, episodesTransform);
            behaviors.Add(newBehavior);
        }
        return behaviors;
    }
    
    private void SetTextLabelSettings(TextMeshPro textObject)
    {
        textObject.color = Color.white;
        textObject.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        textObject.alignment = TextAlignmentOptions.Midline;
        textObject.enableWordWrapping = false;
    }

    private IEnumerable<TextMeshPro> CreateSeasonNumbers()
    {
        for (int i = 0; i < MaxSeason; i++)
        {
            float x = ((MaxEpisode + 2) / 2) + .01f;
            float z = -(MaxSeason - i) + .5f + (MaxSeason / 2);

            TextMeshPro seasonNumberTextA = MakeNumberLabel("Season", i + 1);
            seasonNumberTextA.transform.localPosition = new Vector3(-z, -0.6f, x);
            seasonNumberTextA.transform.localRotation = Quaternion.Euler(0, 180, 0);
            yield return seasonNumberTextA;

            TextMeshPro seasonNumberTextB = MakeNumberLabel("Season", i + 1);
            seasonNumberTextB.transform.localPosition = new Vector3(-z, -0.6f, -x);
            seasonNumberTextB.transform.localRotation = Quaternion.Euler(0, 0, 0);
            yield return seasonNumberTextB;
        }
    }

    private IEnumerable<TextMeshPro> CreateEpisodeNumbers()
    {
        List<TextMeshPro> ret = new List<TextMeshPro>();
        for (int i = 0; i < MaxEpisode; i++)
        {
            float z = ((MaxSeason + 2) / 2) + .01f;
            float x = -i - .5f + (MaxEpisode / 2);

            TextMeshPro episodeNumberTextA = MakeNumberLabel("Episode", i + 1);
            episodeNumberTextA.transform.localPosition = new Vector3(-z, -0.6f, x);
            episodeNumberTextA.transform.localRotation = Quaternion.Euler(0, 90, 0);
            ret.Add(episodeNumberTextA);

            TextMeshPro episodeNumberTextB = MakeNumberLabel("Episode", i + 1);
            episodeNumberTextB.transform.localPosition = new Vector3(z, -0.6f, x);
            episodeNumberTextB.transform.localRotation = Quaternion.Euler(0, -90, 0);
            ret.Add(episodeNumberTextB);
        }
        return ret;
    }

    private TextMeshPro MakeNumberLabel(string labelText, int i)
    {
        GameObject numberLabel = new GameObject(labelText + " " + i + " Label");
        TextMeshPro numberText = numberLabel.AddComponent<TextMeshPro>();
        numberText.text = i.ToString();
        numberText.fontSize = 6;
        SetTextLabelSettings(numberText);
        numberText.transform.SetParent(labelsTransform, false);
        numberText.transform.localScale = Vector3.one;
        return numberText;
    }

    private IEnumerable<TextMeshPro> CreateSeasonLabel()
    {
        float x = ((MaxEpisode + 2) / 2) + .01f;

        TextMeshPro seasonLabelA = MakeLabel("SEASON");
        seasonLabelA.transform.localRotation = Quaternion.Euler(0, 180, 0);
        seasonLabelA.transform.localPosition = new Vector3(0, -1.5f, x);
        yield return seasonLabelA;

        TextMeshPro seasonLabelB = MakeLabel("SEASON");
        seasonLabelB.transform.localRotation = Quaternion.Euler(0, 0, 0);
        seasonLabelB.transform.localPosition = new Vector3(0, -1.5f, -x);
        yield return seasonLabelB;
    }

    private IEnumerable<TextMeshPro> CreateEpisodeLabel()
    {
        float z = ((MaxSeason + 2) / 2) + .01f;

        TextMeshPro episodeTextA = MakeLabel("EPISODE");

        episodeTextA.transform.localPosition = new Vector3(-z, -1.5f, 0);
        episodeTextA.transform.localRotation = Quaternion.Euler(0, 90, 0);
        yield return episodeTextA;

        TextMeshPro episodeTextB = MakeLabel("EPISODE");

        episodeTextB.transform.localPosition = new Vector3(z, -1.5f, 0);
        episodeTextB.transform.localRotation = Quaternion.Euler(0, -90, 0);
        yield return episodeTextB;
    }

    private TextMeshPro MakeLabel(string labelText)
    {
        GameObject episodeLabel = new GameObject(labelText + " Label");
        TextMeshPro episodeText = episodeLabel.AddComponent<TextMeshPro>();
        episodeText.text = labelText;
        episodeText.fontSize = 8;
        SetTextLabelSettings(episodeText);
        episodeText.transform.SetParent(labelsTransform, false);
        episodeText.transform.localScale = Vector3.one;
        return episodeText;
    }

    private EpisodeBehavior CreateNewEpisodeBox(EpisodeData data, Transform episodesTransform)
    {
        GameObject box = GameObject.Instantiate(MainScript.Instance.EpisodePrefab);
        box.name = data.Season + "." + data.Episode + ":" + data.Title;
        EpisodeBehavior behavior = box.GetComponent<EpisodeBehavior>();
        behavior.Data = data;
        behavior.Series = this;
        box.transform.SetParent(episodesTransform, false);
        return behavior;
    }

    private class StageBoxManager
    {
        private readonly Transform stageTransform;
        private Vector3 innerboxBaseScale;
        public Transform InnerBox { get; }

        public void UpdateStage(float nealsonOrImdb)
        {
            InnerBox.localScale = innerboxBaseScale * (1 - nealsonOrImdb);
        }

        public StageBoxManager(Transform baseTransform, float maxSeason, float maxEpisode)
        {
            this.stageTransform = new GameObject("Stage").transform;
            stageTransform.SetParent(baseTransform, false);

            innerboxBaseScale = new Vector3(maxSeason, 2f, maxEpisode);
            Vector3 innerboxPos = new Vector3(0, -0.99f, 0);

            Vector3 imdbXScale = new Vector3(maxSeason + 2, 2.2f, 1);
            Vector3 imdbYScale = new Vector3(1, 2.2f, maxEpisode + 2);

            Vector3 imdbPosXPos = new Vector3(0, -1.1f, (maxEpisode + 1) / 2);
            Vector3 imdbNegXPos = new Vector3(0, -1.1f, -(maxEpisode + 1) / 2);
            Vector3 imdbPosYPos = new Vector3((maxSeason + 1) / 2, -1.1f, 0);
            Vector3 imdbNegYPos = new Vector3(-(maxSeason + 1) / 2, -1.1f, 0);

            CreateStageBox(imdbXScale, imdbPosXPos);
            CreateStageBox(imdbXScale, imdbNegXPos);
            CreateStageBox(imdbYScale, imdbPosYPos);
            CreateStageBox(imdbYScale, imdbNegYPos);
            InnerBox = CreateStageBox(innerboxBaseScale, innerboxPos, true);
        }

        private Transform CreateStageBox(Vector3 boxScale, Vector3 boxPosition, bool isInnerBox = false)
        {
            GameObject stageBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stageBox.name = "Stage Box";
            Destroy(stageBox.GetComponent<BoxCollider>());
            stageBox.GetComponent<MeshRenderer>().sharedMaterial = isInnerBox ? MainScript.Instance.InnerStageBoxMat : MainScript.Instance.StageBoxMat;
            stageBox.transform.SetParent(stageTransform, false);

            stageBox.transform.localPosition = boxPosition;
            stageBox.transform.localScale = boxScale;
            return stageBox.transform;
        }
    }
}
