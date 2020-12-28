using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MainScript))]
public class MouseInteractionManager : MonoBehaviour
{
    private MainScript main;

    private DragDetector leftDragDetector;

    [SerializeField]
    private float dragStartDistance = 2;

    [SerializeField]
    private float scrollSpeed = 0.1f;
    [SerializeField]
    private float minZoom = 10;
    [SerializeField]
    private float maxZoom = 100;
    [SerializeField]
    private float orbitSpeed = 0.5f;
    [SerializeField]
    private float panSpeed = 0.1f;

    private Vector3 orbitScreenStart;
    private Vector3 orbitStartAngle;
    public Transform OrbitPoint { get; private set; }

    public Transform RootTransform;

    public bool OrbitDisabled { get; set; }
    
    void Start()
    {
        main = GetComponent<MainScript>();
        leftDragDetector = new DragDetector(dragStartDistance);
        OrbitPoint = new GameObject("Stage Orbit").transform;

        RootTransform.SetParent(OrbitPoint, true);
        StartOrbit();
    }

    void Update()
    {
        if(!Input.GetMouseButton(0))
        {
            OrbitDisabled = false;
        }
        if (!OrbitDisabled)
        {
            HandleOrbit();
        }
        //cameraInteraction.HandleMouseScrollwheel();
    }
    
    private void HandleOrbit()
    {
        if (Input.GetMouseButton(0))
        {
            if (leftDragDetector.IsDragging)
            {
                ContinueOrbit();
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    leftDragDetector.DragStartPos = Input.mousePosition;
                    StartOrbit();
                }
                else
                {
                    leftDragDetector.UpdateIsDragging();
                }
            }
        }
        else
        {
            leftDragDetector.IsDragging = false;
        }
    }

    public void StartOrbit()
    {
        orbitScreenStart = Input.mousePosition;
        orbitStartAngle = OrbitPoint.eulerAngles;
    }

    public void ContinueOrbit()
    {
        Vector3 screenDelta = orbitScreenStart - Input.mousePosition;
        float yaw = -screenDelta.x * orbitSpeed - orbitStartAngle.y;
        float startAngle = orbitStartAngle.x;
        if (startAngle < 180)
        {
            startAngle += 360;
        }
        float pitch = -screenDelta.y * orbitSpeed + startAngle;
        //pitch = Mathf.Clamp(pitch, 280, 440);
        OrbitPoint.rotation = Quaternion.Euler(pitch, -yaw, 0);
    }

    public void HandleMouseScrollwheel()
    {
        Vector3 oldPos = Camera.main.transform.localPosition;
        float scaleFactor = 1f - (Input.mouseScrollDelta.y * scrollSpeed);
        float newPos = oldPos.z * scaleFactor;
        newPos = Mathf.Clamp(newPos, minZoom, maxZoom);

        Camera.main.transform.localPosition = new Vector3(oldPos.x, oldPos.y, newPos);
    }

    private class DragDetector
    {
        public Vector3 DragStartPos { get; set; }
        public bool IsDragging { get; set; }
        private readonly float dragStartDistance;

        public DragDetector(float dragStartDistance)
        {
            this.dragStartDistance = dragStartDistance;
        }

        public void UpdateIsDragging()
        {
            IsDragging = (DragStartPos - Input.mousePosition).magnitude > dragStartDistance;
        }
    }
}