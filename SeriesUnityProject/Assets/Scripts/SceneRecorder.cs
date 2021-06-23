using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using System.IO;

public class SceneRecorder : SceneScrubbingBase
{
    public bool DoRecord;

    private long index;

    private int lastFrame;
    public int currentFrame;

    private void Start()
    {
        handTransforms = GetAllHandJoints();
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

    private TransformRecord GetTransformRecord(Transform item)
    {
        TransformRecord ret = new TransformRecord();
        ret.LocalScale = item.localScale;
        ret.Position = item.position;
        ret.Rotation = item.rotation;
        return ret;
    }

    private void FixedUpdate()
    {
        if(DoRecord)
        {
            WriteFrame();
            index++;
        }
    }
    private void WriteFrame()
    {
        FrameRecord record = GetRecord();
        string path = OutputFolder + index + ".json";
        string asJason = JsonUtility.ToJson(record, true);
        File.WriteAllText(path, asJason);
    }

    private EpisodeRecord ToEpisodeRecord(EpisodeBehavior item)
    {
        EpisodeRecord ret = new EpisodeRecord();
        ret.Transform = GetTransformRecord(item.transform);
        ret.Color = item.Color;
        ret.EmissiveColor = item.EmissiveColor;
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