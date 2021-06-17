using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
    public SceneRecorder Recorder;

    public float ShotTime;
    private float TimeToNextShot;
    private int shotIndex = 0;
    private const string OutputFolder = "F:\\SeriesDataViewerOutput\\";
    private bool advance; // If you take the shot and move the camera in the same update, it will clear the ray accumulation

    void Start()
    {
        TimeToNextShot = ShotTime;
    }

    void Update()
    {
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
