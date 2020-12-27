using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class SeriesScript : MonoBehaviour
{
    public float MaxNealson { get; set; }
    public float MaxSeason { get; set; }
    public float MaxEpisode { get; set; }
    public IEnumerable<EpisodeData> Episodes { get; set; }

    private int episodeCount;
    
    void Start ()
    {
        episodeCount = Episodes.Count();
        CreateEpisodeBoxes();
        Transform labels = new GameObject("Labels").transform;
        labels.transform.parent = transform;
        CreateSeasonLabelText(labels);
        CreateEpisodeLabelText(labels);
	}

    private void CreateEpisodeBoxes()
    {
        List<EpisodeBehavior> behaviors = new List<EpisodeBehavior>();
        Transform episodesTransform = new GameObject("Episodes").transform;
        episodesTransform.transform.SetParent(transform, false);
        foreach (EpisodeData data in Episodes)
        {
            EpisodeBehavior newBehavior = CreateNewEpisodeBox(data, episodesTransform);
            behaviors.Add(newBehavior);
        }
    }
    
    private void SetTextLabelSettings(TextMeshPro textObject)
    {
        textObject.color = Color.white;
        textObject.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        textObject.alignment = TextAlignmentOptions.Midline;
        textObject.enableWordWrapping = false;
    }

    private void CreateSeasonLabelText(Transform labelsTransform)
    {
        GameObject seasonLabel = new GameObject("Season Label");
        TextMeshPro seasonText = seasonLabel.AddComponent<TextMeshPro>();
        seasonText.text = "S\nE\nA\nS\nO\nN";
        seasonText.fontSize = 8;
        SetTextLabelSettings(seasonText);
        seasonText.transform.rotation = Quaternion.Euler(90, 90, 0);
        seasonText.transform.SetParent(transform, false);
        seasonText.transform.localPosition = new Vector3(-MaxSeason / 2, 0, 2f);
        seasonText.transform.parent = labelsTransform;

        for (int i = 0; i < MaxSeason; i++)
        {
            GameObject seasonNumberLabel = new GameObject("Season" + (i + 1).ToString());
            TextMeshPro seasonNumberText = seasonNumberLabel.AddComponent<TextMeshPro>();
            seasonNumberText.text = (i + 1).ToString();
            seasonNumberText.fontSize = 6;
            seasonNumberLabel.transform.SetParent(transform, false);
            seasonNumberLabel.transform.localPosition = new Vector3(-i - 1, 0, .5f);
            SetTextLabelSettings(seasonNumberText);
            seasonNumberLabel.transform.rotation = Quaternion.Euler(90, 90, 90);
            seasonNumberLabel.transform.parent = labelsTransform;
        }
    }

    private void CreateEpisodeLabelText(Transform labelsTransform)
    {
        GameObject episodeLabel = new GameObject("Episode Label");
        TextMeshPro episodeText = episodeLabel.AddComponent<TextMeshPro>();
        episodeText.text = "EPISODE";
        episodeText.fontSize = 8;
        SetTextLabelSettings(episodeText);
        episodeText.transform.rotation = Quaternion.Euler(90, 90, 0);
        episodeText.transform.SetParent(transform, false);
        episodeText.transform.localPosition = new Vector3(-(MaxSeason + 2), 0, -MaxEpisode / 2f);
        episodeText.transform.parent = labelsTransform;
        for (int i = 0; i < MaxEpisode; i++)
        {
            GameObject episodeNumberLabel = new GameObject("Episode " + (i + 1).ToString());
            TextMeshPro episodeNumberText = episodeNumberLabel.AddComponent<TextMeshPro>();
            episodeNumberText.text = (i + 1).ToString();
            episodeNumberText.fontSize = 6;
            episodeNumberLabel.transform.SetParent(transform, false);
            episodeNumberLabel.transform.localPosition = new Vector3(- (MaxSeason + 1), 0, - i - 1);
            SetTextLabelSettings(episodeNumberText);
            episodeNumberLabel.transform.rotation = Quaternion.Euler(90, 90, 90);
            episodeNumberLabel.transform.parent = labelsTransform;
        }
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
}
