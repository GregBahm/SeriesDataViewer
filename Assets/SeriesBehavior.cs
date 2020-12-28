using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class SeriesBehavior : MonoBehaviour
{
    public IEnumerable<EpisodeData> Episodes { get; set; }
    public IEnumerable<EpisodeBehavior> EpisodeBehaviors { get; private set; }
    public float MaxNealson { get; private set; }
    public float MaxSeason { get; private set; }
    public float MaxEpisode { get; private set; }

    private Transform labelsTransform;

    private TextMeshPro EpisodeLabel;
    private TextMeshPro SeasonLabel;
    private IEnumerable<TextMeshPro> SeasonNumbers;
    private IEnumerable<TextMeshPro> EpisodeNumbers;
    
    void Start ()
    {
        MaxNealson = Episodes.Max(item => item.NealsonRating);
        MaxSeason = Episodes.Max(item => item.Season);
        MaxEpisode = Episodes.Max(item => item.Episode);

        transform.localPosition = new Vector3(MaxEpisode / 2, 0, MaxSeason / 2);
        EpisodeBehaviors = CreateEpisodeBoxes();
        labelsTransform = new GameObject("Labels").transform;
        labelsTransform.parent = transform;
        EpisodeLabel = CreateEpisodeLabel();
        SeasonLabel = CreateSeasonLabel();
        SeasonNumbers = CreateSeasonNumbers();
        EpisodeNumbers = CreateEpisodeNumbers();
	}

    private void Update()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        Color textColor = Color.Lerp(Color.white, Color.black, MainScript.Instance.NealsonOrImdb);
        EpisodeLabel.color = textColor;
        SeasonLabel.color = textColor;
        foreach (TextMeshPro item in SeasonNumbers)
        {
            item.color = textColor;
        }
        foreach (TextMeshPro item in EpisodeNumbers)
        {
            item.color = textColor;
        }
    }

    public float GetHeightForCameraOrbit()
    {
        float maxY = EpisodeBehaviors.Max(item => item.gameObject.transform.localPosition.y);
        float minY = EpisodeBehaviors.Min(item => item.gameObject.transform.localPosition.y);
        float ret = (maxY + minY) / 2;
        return ret * transform.lossyScale.y;
    }

    private IEnumerable<EpisodeBehavior> CreateEpisodeBoxes()
    {
        List<EpisodeBehavior> behaviors = new List<EpisodeBehavior>();
        Transform episodesTransform = new GameObject("Episodes").transform;
        episodesTransform.transform.SetParent(transform, false);
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

    private TextMeshPro CreateSeasonLabel()
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
        return seasonText;
    }

    private IEnumerable<TextMeshPro> CreateSeasonNumbers()
    {
        List<TextMeshPro> ret = new List<TextMeshPro>();
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
            ret.Add(seasonNumberText);
        }
        return ret;
    }

    private TextMeshPro CreateEpisodeLabel()
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
        return episodeText;
    }

    private IEnumerable<TextMeshPro> CreateEpisodeNumbers()
    {
        List<TextMeshPro> ret = new List<TextMeshPro>();
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
            ret.Add(episodeNumberText);
        }
        return ret;
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
