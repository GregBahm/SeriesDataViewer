using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRattlerScript : MonoBehaviour
{
    [Range(0, 90)]
    public float Rotation;

    private bool tick;
    public bool Vibrate;

    private bool suspend;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            suspend = !suspend;
        }

        if(!suspend)
        {
            float rot = Rotation * (tick ? 1f : -1f);
            transform.Rotate(0, rot, 0);
            if (Vibrate)
            {
                tick = !tick;
            }
        }
    }
}
