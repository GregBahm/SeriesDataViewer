using UnityEngine;

using System.IO;

public class SceneReplayer : SceneScrubbingBase
{
    private int lastFrame;
    public int currentFrame;

    [Range(0, 1)]
    public float Time;

    private ReplayerMode oldMode;
    public ReplayerMode Mode;

    public int readableFrameCount;
    private MaterialPropertyBlock episodeMaterialBlock;

    public enum ReplayerMode
    {
        Waiting,
        ScrubTime,
        ExactTime
    }

    private void Start()
    {
        readableFrameCount = Directory.GetFiles(OutputFolder).Length;
        handTransforms = GetAllHandJoints();
        episodeMaterialBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if(Mode != ReplayerMode.Waiting && oldMode == ReplayerMode.Waiting)
        {
            ActivateReplayMode();
        }
        if(Mode == ReplayerMode.ScrubTime)
        {
            currentFrame = (int)((readableFrameCount - 1) * Time);
        }
        if (lastFrame != currentFrame)
        {
            LoadData(LoadRecord(currentFrame));
        }
        lastFrame = currentFrame;

        oldMode = Mode;
    }

    private void ActivateReplayMode()
    {
        MainScript.Instance.transform.gameObject.SetActive(false);
        foreach (ShowBehavior show in MainScript.Instance.EachSeries)
        {
            foreach (EpisodeBehavior episode in show.EpisodeBehaviors)
            {
                episode.enabled = false;
            }
        }
    }

    private FrameRecord LoadRecord(int frameToRead)
    {
        string path = OutputFolder + frameToRead + ".json";
        string fileData = File.ReadAllText(path);
        return JsonUtility.FromJson<FrameRecord>(fileData);
    }

    private void LoadData(FrameRecord data)
    {
        for (int i = 0; i < data.HandData.Count; i++)
        {
            ApplyTransformRecord(handTransforms[i], data.HandData[i]);
        }
        ApplyTransformRecord(MainScript.Instance.RootTransform, data.RootTransform);
        for (int i = 0; i < MainScript.Instance.EachSeries.Count; i++)
        {
            bool active = i == data.ShowToShow;
            MainScript.Instance.EachSeries[i].gameObject.SetActive(active);
        }
        ShowBehavior show = MainScript.Instance.EachSeries[data.ShowToShow];
        for (int i = 0; i < show.EpisodeBehaviors.Count; i++)
        {
            SetEpisode(data.Episodes[i], show.EpisodeBehaviors[i]);
        }
    }

    private void SetEpisode(EpisodeRecord record, EpisodeBehavior episode)
    {
        ApplyTransformRecord(episode.transform, record.Transform);
        episodeMaterialBlock.SetColor("_BaseColor", record.Color);
        episodeMaterialBlock.SetColor("_EmissiveColor", record.EmissiveColor);
        episode.Renderer.SetPropertyBlock(episodeMaterialBlock);
    }

    private void ApplyTransformRecord(Transform transform, TransformRecord transformRecord)
    {
        transform.position = transformRecord.Position;
        transform.rotation = transformRecord.Rotation;
        transform.localScale = transformRecord.LocalScale;
    }
}
