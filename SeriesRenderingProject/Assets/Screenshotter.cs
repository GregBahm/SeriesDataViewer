using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
    public float ShotTime;
    private float TimeToNextShot;
    public float RotationPerShot = 1;
    private float RotationRemaining = 360f;
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
            transform.Rotate(0, RotationPerShot, 0, Space.World);
            advance = false;
        }
        if(TimeToNextShot <= 0 && RotationRemaining > 0)
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
        RotationRemaining -= RotationPerShot;
    }
}
