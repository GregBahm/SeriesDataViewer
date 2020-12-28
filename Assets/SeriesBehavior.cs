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

    private TextMeshPro episodeLabel;
    private TextMeshPro seasonLabel;
    private IEnumerable<TextMeshPro> seasonNumbers;
    private IEnumerable<TextMeshPro> episodeNumbers;

    private Transform stageBox;
    
    public float ScoreMid { get; private set; }

    void Start ()
    {
        MaxNealson = Episodes.Max(item => item.NealsonRating);
        MaxSeason = Episodes.Max(item => item.Season);
        MaxEpisode = Episodes.Max(item => item.Episode);
        
        EpisodeBehaviors = CreateEpisodeBoxes();
        labelsTransform = new GameObject("Labels").transform;
        labelsTransform.SetParent(transform);
        episodeLabel = CreateEpisodeLabel();
        seasonLabel = CreateSeasonLabel();
        seasonNumbers = CreateSeasonNumbers();
        episodeNumbers = CreateEpisodeNumbers();
        stageBox = CreateStageBox();

        ScoreMid = GetScoreMid();
	}

    private float GetScoreMid()
    {
        float maxY = Episodes.Max(item => item.ImdbRating);
        float minY = Episodes.Min(item => item.ImdbRating);
        return (maxY + minY) / 2;
    }

    private Transform CreateStageBox()
    {
        GameObject stageBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        stageBox.name = "Stage Box";
        Destroy(stageBox.GetComponent<BoxCollider>());
        stageBox.GetComponent<MeshRenderer>().sharedMaterial = MainScript.Instance.StageBoxMat;

        stageBox.transform.SetParent(transform);
        stageBox.transform.localScale = new Vector3(MaxEpisode + 1, 2.75f, MaxSeason + 1);
        stageBox.transform.localPosition = new Vector3(0, -1.5f, 0);

        return stageBox.transform;
    }

    private void Update()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        Color textColor = Color.Lerp(Color.white, Color.black, MainScript.Instance.NealsonOrImdb);
        episodeLabel.color = textColor;
        seasonLabel.color = textColor;
        foreach (TextMeshPro item in seasonNumbers)
        {
            item.color = textColor;
        }
        foreach (TextMeshPro item in episodeNumbers)
        {
            item.color = textColor;
        }
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

    private TextMeshPro CreateSeasonLabel()
    {
        GameObject seasonLabel = new GameObject("Season Label");
        TextMeshPro seasonText = seasonLabel.AddComponent<TextMeshPro>();
        seasonText.text = "SEASON";
        seasonText.fontSize = 8;
        SetTextLabelSettings(seasonText);

        float x = ((MaxEpisode + 1) / 2) + .01f;

        seasonText.transform.SetParent(transform, false);
        seasonText.transform.rotation = Quaternion.Euler(0, 90, 0);
        seasonText.transform.localPosition = new Vector3(0, -2f, x);
        seasonText.transform.SetParent(labelsTransform);
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
            SetTextLabelSettings(seasonNumberText);
            
            float x = ((MaxEpisode + 1) / 2) + .01f;
            float z = -(MaxSeason - i) + .5f + (MaxSeason / 2);

            seasonNumberLabel.transform.SetParent(transform, false);
            seasonNumberLabel.transform.localPosition = new Vector3(-z, -1f, x);
            seasonNumberLabel.transform.rotation = Quaternion.Euler(0, 90, 0);
            seasonNumberLabel.transform.SetParent(labelsTransform);
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

        float z = ((MaxSeason + 1) / 2) + .01f;

        episodeText.transform.SetParent(transform, false);
        episodeText.transform.localPosition = new Vector3(-z, -2f, 0);
        episodeText.transform.rotation = Quaternion.Euler(0, 0, 0);
        episodeText.transform.SetParent(labelsTransform, true);
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
            SetTextLabelSettings(episodeNumberText);

            float z = ((MaxSeason + 1) / 2) + .01f;
            float x = -i - .5f + (MaxEpisode / 2);

            episodeNumberLabel.transform.SetParent(transform, false);
            episodeNumberLabel.transform.localPosition = new Vector3(-z, -1f, x);
            episodeNumberLabel.transform.rotation = Quaternion.Euler(0, 0, 0);
            episodeNumberLabel.transform.SetParent(labelsTransform, true);
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
