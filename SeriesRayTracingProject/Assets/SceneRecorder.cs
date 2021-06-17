using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using System.IO;

public class SceneRecorder : MonoBehaviour
{
    private MaterialPropertyBlock episodeMaterialBlock;
    public const string OutputFolder = @"F:\SeriesDataViewerOutput\Recording\";
    private long index;

    private RecorderMode lastMode = RecorderMode.Wait;
    public RecorderMode Mode;

    private Transform[] handTransforms;

    public enum RecorderMode
    {
        Read,
        Write,
        Wait
    }

    private int lastFrame;
    public int currentFrame;
    //[Range(0, 1)]
    //public float Time;
    public int readableFrameCount;

    private void Start()
    {
        episodeMaterialBlock = new MaterialPropertyBlock();
        readableFrameCount = Directory.GetFiles(OutputFolder).Length;

        handTransforms = Hands.Instance.LeftHandProxy.AllJoints
            .Where(item => item != null).Concat(Hands.Instance.RightHandProxy.AllJoints
            .Where(item => item != null)).ToArray();
    }

    private void FixedUpdate()
    {
        if(lastMode != Mode)
        {
            SetMode();
        }
        if(Mode == RecorderMode.Write)
        {
            //WriteFrame();
            //index++;
        }
        if(Mode == RecorderMode.Read)
        {
            //currentFrame = (int)((readableFrameCount - 1) * Time);
            if(lastFrame != currentFrame)
            {
                LoadData(LoadRecord(currentFrame));
            }
            lastFrame = currentFrame;
        }
        lastMode = Mode;
    }

    private void SetMode()
    {
        bool readMode = Mode == RecorderMode.Read;
        MainScript.Instance.transform.gameObject.SetActive(!readMode);
        foreach(ShowBehavior show in MainScript.Instance.EachSeries)
        {
            foreach (EpisodeBehavior episode in show.EpisodeBehaviors)
            {
                episode.enabled = !readMode;
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

    private void WriteFrame()
    {
        FrameRecord record = GetRecord();
        string path = OutputFolder + index + ".json";
        string asJason = JsonUtility.ToJson(record, true);
        File.WriteAllText(path, asJason);
    }

    private FrameRecord GetRecord()
    {
        List<TransformRecord> handData = handTransforms.Select(item => GetTransformRecord(item)).ToList();
        List<EpisodeRecord> episodes = MainScript.Instance.ShownSeries.EpisodeBehaviors
            .Select(item => ToEpisodeRecord(item)).ToList();
        FrameRecord ret = new FrameRecord();
        ret.ShowToShow = MainScript.Instance.ShowToShow;
        ret.Episodes = episodes;
        ret.HandData = handData;
        ret.RootTransform = GetTransformRecord(MainScript.Instance.RootTransform);
        return ret;
    }

    private EpisodeRecord ToEpisodeRecord(EpisodeBehavior item)
    {
        EpisodeRecord ret = new EpisodeRecord();
        ret.Transform = GetTransformRecord(item.transform);
        ret.Color = item.Color;
        ret.EmissiveColor = item.EmissiveColor;
        return ret;
    }

    private TransformRecord GetTransformRecord(Transform item)
    {
        TransformRecord ret = new TransformRecord();
        ret.LocalScale = item.localScale;
        ret.Position = item.position;
        ret.Rotation = item.rotation;
        return ret;
    }
}

[Serializable]
public class FrameRecord
{
    public List<TransformRecord> HandData;
    public int ShowToShow;
    public List<EpisodeRecord> Episodes;
    public TransformRecord RootTransform;
}

[Serializable]
public struct EpisodeRecord
{
    public TransformRecord Transform;
    public Color Color;
    public Color EmissiveColor;
}

[Serializable]
public struct TransformRecord
{
    public Vector3 Position;
    public Vector3 LocalScale;
    public Quaternion Rotation;
}