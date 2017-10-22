using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SeriesScript : MonoBehaviour
{
    [Range(0, 1)]
    public float NealsonOrImdb;

    [Range(0, 1)]
    public float SpaceBetweenSeasons;
    [Range(0, 1)]
    public float SpaceBetweenEpisodes;
    
    [Range(0, 10)]
    public float ImdbMin;
    [Range(0, 10)]
    public float ImdbMax;
    public float ImdbScale;

    public Material BaseMaterial;

    public float MaxNealson { get; set; }
    public float MaxSeason { get; set; }
    public float MaxEpisode { get; set; }

    private IEnumerable<EpisodeBehavior> episodeBehaviors;

	void Start ()
    {
        IEnumerable<EpisodeData> episodes = DataLoader.LoadData();

        MaxNealson = episodes.Max(item => item.NealsonRating);
        MaxSeason = episodes.Max(item => item.Season);
        MaxEpisode = episodes.Max(item => item.Episode);

        List<EpisodeBehavior> behaviors = new List<EpisodeBehavior>();

        GameObject masterBox = new GameObject("Boxes");
        foreach (EpisodeData item in episodes)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.GetComponent<MeshRenderer>().material = new Material(BaseMaterial);
            EpisodeBehavior behavior = box.AddComponent<EpisodeBehavior>();
            behavior.Data = item;
            behavior.Mothership = this;
            behaviors.Add(behavior);
            box.transform.parent = masterBox.transform;
        }
        episodeBehaviors = behaviors;
	}
}
