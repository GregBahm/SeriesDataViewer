using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
    public SceneReplayer Recorder;

    public float ShotTime;
    private float TimeToNextShot;
    private int shotIndex = 0;
    private const string OutputFolder = "F:\\SeriesDataViewerOutput\\";
    private bool advance; 

    void Start()
    {
        TimeToNextShot = ShotTime;
    }

    void Update()
    {
        Recorder.Mode = SceneReplayer.ReplayerMode.ExactTime;
        if(advance)
        {
            Recorder.currentFrame++;
            advance = false;
        }
        if(TimeToNextShot <= 0 && Recorder.currentFrame < Recorder.readableFrameCount)
        {
            TakeShot();
            advance = true;
        }
        TimeToNextShot -= Time.deltaTime;
    }

    private void TakeShot()
    {
        string shotPath = OutputFolder + "shot" + shotIndex.ToString("D6") + ".png";
        ScreenCapture.CaptureScreenshot(shotPath);
        shotIndex++;
        TimeToNextShot = ShotTime;
    }
}
