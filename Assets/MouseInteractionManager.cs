﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseInteractionManager : MonoBehaviour
{
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
    public Transform OrbitPoint { get; private set; }

    public Transform RootTransform;

    private Vector2 startRotation;
    private Vector2 currentRotation;

    public Slider NealsonOrImdbSlider;

    public bool OrbitDisabled { get; set; }
    
    void Start()
    {
        leftDragDetector = new DragDetector(dragStartDistance);
        OrbitPoint = new GameObject("Stage Orbit").transform;
        RootTransform.SetParent(OrbitPoint, true);
        OrbitPoint.Rotate(Vector3.up, -40, Space.World);
        OrbitPoint.Rotate(Vector3.left, 10, Space.World);
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
        HandleMouseScrollwheel();
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        MainScript.Instance.NealsonOrImdb = NealsonOrImdbSlider.value;
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
        startRotation = currentRotation;
        orbitScreenStart = Input.mousePosition;
    }

    public void ContinueOrbit()
    {
        Vector3 screenDelta = orbitScreenStart - Input.mousePosition;
        currentRotation = startRotation - new Vector2(screenDelta.x, screenDelta.y);
        OrbitPoint.rotation = Quaternion.identity;
        OrbitPoint.Rotate(Vector3.up, -currentRotation.x, Space.World);
        OrbitPoint.Rotate(Vector3.left, -currentRotation.y, Space.World);
    }

    public void HandleMouseScrollwheel()
    {
        Vector3 oldVect = OrbitPoint.position - Camera.main.transform.position;
        float changeFactor = Input.mouseScrollDelta.y * scrollSpeed;
        Vector3 newVect = oldVect * (1 - changeFactor);
        if(newVect.magnitude > maxZoom)
        {
            newVect = Camera.main.transform.forward * maxZoom;
        }
        if(newVect.magnitude < minZoom)
        {
            newVect = Camera.main.transform.forward * minZoom;
        }
        OrbitPoint.localPosition = Camera.main.transform.position + newVect;
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