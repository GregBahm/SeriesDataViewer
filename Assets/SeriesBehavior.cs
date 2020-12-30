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

    private List<TextMeshPro> labels;

    private Transform stageBox;
    
    public float ScoreMid { get; private set; }

    void Start ()
    {
        MaxNealson = Episodes.Max(item => item.NealsonRating);
        MaxSeason = Episodes.Max(item => item.Season);
        MaxEpisode = Episodes.Max(item => item.Episode);
        
        EpisodeBehaviors = CreateEpisodeBoxes();
        labelsTransform = new GameObject("Labels").transform;
        labelsTransform.SetParent(transform, false);

        labels = new List<TextMeshPro>();
        labels.AddRange(CreateEpisodeLabel());
        labels.AddRange(CreateSeasonLabel());
        labels.AddRange(CreateSeasonNumbers());
        labels.AddRange(CreateEpisodeNumbers());
        stageBox = CreateStageBox();

        ScoreMid = GetScoreMid();
	}

    private float GetScoreMid()
    {
        float maxY = Episodes.Max(item => item.ImdbRating);
        float minY = Episodes.Min(item => item.ImdbRating);
        return (maxY + minY) / 2;
    }


    Vector3 stageBoxScale;
    private Transform CreateStageBox()
    {
        GameObject stageBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        stageBox.name = "Stage Box";
        Destroy(stageBox.GetComponent<BoxCollider>());
        stageBox.GetComponent<MeshRenderer>().sharedMaterial = MainScript.Instance.StageBoxMat;

        stageBox.transform.SetParent(transform, false);
        stageBox.transform.localScale = new Vector3(MaxSeason + 1, 2.75f, MaxEpisode + 1);
        stageBox.transform.localPosition = new Vector3(0, -1.5f, 0);
        stageBoxScale = stageBox.transform.localScale;
        return stageBox.transform;
    }

    private void Update()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        Color textColor = Color.Lerp(Color.white, Color.black, MainScript.Instance.NealsonOrImdb);
        foreach (TextMeshPro item in labels)
        {
            item.color = textColor;
        }
        stageBox.localScale = stageBoxScale * (1 - MainScript.Instance.NealsonOrImdb);
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
            float x = ((MaxEpisode + 1) / 2) + .01f;
            float z = -(MaxSeason - i) + .5f + (MaxSeason / 2);

            TextMeshPro seasonNumberTextA = MakeNumberLabel("Season", i + 1);
            seasonNumberTextA.transform.localPosition = new Vector3(-z, -1f, x);
            seasonNumberTextA.transform.localRotation = Quaternion.Euler(0, 180, 0);
            yield return seasonNumberTextA;

            TextMeshPro seasonNumberTextB = MakeNumberLabel("Season", i + 1);
            seasonNumberTextB.transform.localPosition = new Vector3(-z, -1f, -x);
            seasonNumberTextB.transform.localRotation = Quaternion.Euler(0, 0, 0);
            yield return seasonNumberTextB;
        }
    }

    private IEnumerable<TextMeshPro> CreateEpisodeNumbers()
    {
        List<TextMeshPro> ret = new List<TextMeshPro>();
        for (int i = 0; i < MaxEpisode; i++)
        {
            float z = ((MaxSeason + 1) / 2) + .01f;
            float x = -i - .5f + (MaxEpisode / 2);

            TextMeshPro episodeNumberTextA = MakeNumberLabel("Episode", i + 1);
            episodeNumberTextA.transform.localPosition = new Vector3(-z, -1f, x);
            episodeNumberTextA.transform.localRotation = Quaternion.Euler(0, 90, 0);
            ret.Add(episodeNumberTextA);

            TextMeshPro episodeNumberTextB = MakeNumberLabel("Episode", i + 1);
            episodeNumberTextB.transform.localPosition = new Vector3(z, -1f, x);
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
        float x = ((MaxEpisode + 1) / 2) + .01f;

        TextMeshPro seasonLabelA = MakeLabel("SEASON");
        seasonLabelA.transform.localRotation = Quaternion.Euler(0, 180, 0);
        seasonLabelA.transform.localPosition = new Vector3(0, -2f, x);
        yield return seasonLabelA;

        TextMeshPro seasonLabelB = MakeLabel("SEASON");
        seasonLabelB.transform.localRotation = Quaternion.Euler(0, 0, 0);
        seasonLabelB.transform.localPosition = new Vector3(0, -2f, -x);
        yield return seasonLabelB;
    }

    private IEnumerable<TextMeshPro> CreateEpisodeLabel()
    {
        float z = ((MaxSeason + 1) / 2) + .01f;

        TextMeshPro episodeTextA = MakeLabel("EPISODE");

        episodeTextA.transform.localPosition = new Vector3(-z, -2f, 0);
        episodeTextA.transform.localRotation = Quaternion.Euler(0, 90, 0);
        yield return episodeTextA;

        TextMeshPro episodeTextB = MakeLabel("EPISODE");

        episodeTextB.transform.localPosition = new Vector3(z, -2f, 0);
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
}
