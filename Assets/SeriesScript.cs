using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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
        TextMesh titleText = titleObject.AddComponent<TextMesh>();
        titleText.text = name;
        titleText.characterSize = .5f;
        titleText.fontSize = 80;
        titleText.transform.rotation = Quaternion.Euler(90, 90, 0);
        titleText.transform.parent = transform;
        titleText.transform.localPosition = new Vector3(-MaxSeason - 1, 0, -MaxEpisode / 2);
        titleText.color = new Color(.5f, .5f, .5f);
        titleText.alignment = TextAlignment.Center;
        titleText.anchor = TextAnchor.UpperCenter;
    }
}
