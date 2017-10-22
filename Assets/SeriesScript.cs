﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class SeriesScript : MonoBehaviour
{
    public MainScript Main { get; set; }
    public float MaxNealson { get; set; }
    public float MaxSeason { get; set; }
    public float MaxEpisode { get; set; }
    public IEnumerable<EpisodeData> Episodes { get; set; }

    private IEnumerable<EpisodeBehavior> episodeBehaviors;

	void Start ()
    {
        List<EpisodeBehavior> behaviors = new List<EpisodeBehavior>();
        foreach (EpisodeData data in Episodes)
        {
            EpisodeBehavior newBehavior = CreateNewEpisodeBox(data);
            behaviors.Add(newBehavior);
        }
        episodeBehaviors = behaviors;
        CreateTitleText();
        CreateSeasonLabelText();
        CreateEpisodeLabelText();
	}

    private void SetTextLabelSettings(TextMeshPro textObject)
    {
        textObject.color = new Color(.5f, .5f, .5f);
        textObject.transform.rotation = Quaternion.Euler(90, 90, 0);
        textObject.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        textObject.alignment = TextAlignmentOptions.Midline;
        textObject.enableWordWrapping = false;
    }

    private void CreateSeasonLabelText()
    {
        GameObject seasonLabel = new GameObject("Season Label");
        TextMeshPro seasonText = seasonLabel.AddComponent<TextMeshPro>();
        seasonText.text = "S\nE\nA\nS\nO\nN";
        seasonText.fontSize = 8;
        SetTextLabelSettings(seasonText);
        seasonText.transform.parent = transform;
        seasonText.transform.localPosition = new Vector3(-MaxSeason / 2, 0, 2f);

        for (int i = 0; i < MaxSeason; i++)
        {
            GameObject seasonNumberLabel = new GameObject("Season" + (i + 1).ToString());
            TextMeshPro seasonNumberText = seasonNumberLabel.AddComponent<TextMeshPro>();
            seasonNumberText.text = (i + 1).ToString();
            seasonNumberText.fontSize = 6;
            seasonNumberLabel.transform.parent = transform;
            seasonNumberLabel.transform.localPosition = new Vector3(-i - 1, 0, .5f);
            SetTextLabelSettings(seasonNumberText);
        }
    }

    private void CreateEpisodeLabelText()
    {
        GameObject episodeLabel = new GameObject("Episode Label");
        TextMeshPro episodeText = episodeLabel.AddComponent<TextMeshPro>();
        episodeText.text = "EPISODE";
        episodeText.fontSize = 8;
        SetTextLabelSettings(episodeText);
        episodeText.transform.parent = transform;
        episodeText.transform.localPosition = new Vector3(2, 0, -MaxEpisode / 2f);
        for (int i = 0; i < MaxEpisode; i++)
        {
            GameObject episodeNumberLabel = new GameObject("Episode " + (i + 1).ToString());
            TextMeshPro episodeNumberText = episodeNumberLabel.AddComponent<TextMeshPro>();
            episodeNumberText.text = (i + 1).ToString();
            episodeNumberText.fontSize = 6;
            episodeNumberLabel.transform.parent = transform;
            episodeNumberLabel.transform.localPosition = new Vector3(.5f, 0, - i - 1);
            SetTextLabelSettings(episodeNumberText);
        }
    }

    private EpisodeBehavior CreateNewEpisodeBox(EpisodeData data)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = data.Season + "." + data.Episode + ":" + data.Title;
        box.GetComponent<MeshRenderer>().material = new Material(Main.BaseMaterial);
        EpisodeBehavior behavior = box.AddComponent<EpisodeBehavior>();
        behavior.Data = data;
        behavior.Main = Main;
        behavior.Series = this;
        box.transform.parent = transform;
        return behavior;
    }

    private void CreateTitleText()
    {
        GameObject titleObject = new GameObject("Title");
        TextMeshPro titleText = titleObject.AddComponent<TextMeshPro>();
        titleText.text = name;
        titleText.fontSize = 36;
        titleText.transform.rotation = Quaternion.Euler(90, 90, 0);
        titleText.transform.parent = transform;
        titleText.transform.localPosition = new Vector3(-MaxSeason - 4, 0, -MaxEpisode / 2);
        titleText.color = new Color(.5f, .5f, .5f);
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.enableWordWrapping = false;
    }
}