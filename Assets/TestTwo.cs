using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTwo : MonoBehaviour
{
    public float Height;

    public float Speed;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float val = Mathf.Cos(Time.time * Speed);
        transform.position = startPos + new Vector3(0, val * Height, 0);
    }
}
