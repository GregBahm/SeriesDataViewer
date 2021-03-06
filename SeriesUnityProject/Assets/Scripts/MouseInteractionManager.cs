﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MouseInteractionManager : MonoBehaviour
{
    public static MouseInteractionManager Instance { get; private set; }

    private DragDetector leftDragDetector;

    [SerializeField]
    private float dragStartDistance = 2;

    [SerializeField]
    private float scrollSpeed = 0.1f;
    [SerializeField]
    private float minZoom = 0.2f;
    [SerializeField]
    private float maxZoom = 0.7f;
    [SerializeField]
    private float panSpeed = 0.1f;

    public bool Panning { get; private set; }
    public bool Orbiting { get; private set; }

    private Vector3 panStartPos;
    private Vector3 contentStartPos;

    private Vector3 orbitScreenStart;
    public Transform OrbitPoint { get; private set; }

    public Transform RootTransform;

    private Vector2 startRotation;
    private Vector2 currentRotation;

    public Slider NealsonOrImdbSlider;

    public RectTransform ShowSelectorRect;

    public RectTransform BackgroundCanvas;

    public bool UiHovered { get; set; }
    public bool StageInteractionEnabled { get; private set; }
    public bool ShowSelectorHovered { get; set; }
    public bool DrilldownEnabled
    {
        get { return StageInteractionEnabled && !Panning && !Orbiting; }
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        leftDragDetector = new DragDetector(dragStartDistance);
        OrbitPoint = new GameObject("Stage Orbit").transform;
        RootTransform.SetParent(OrbitPoint, true);
        OrbitPoint.Rotate(Vector3.up, -40, Space.World);
        OrbitPoint.Rotate(Vector3.left, 10, Space.World);
        currentRotation = new Vector2(40, -10);
        StartOrbit();
    }

    void Update()
    {
        UpdateStageInteractionStatus();
        if(StageInteractionEnabled)
        {
            HandleOrbit();
            HandleMouseScrollwheel();
            HandlePan();
            ClampOrbitPosition();
        }
        UpdateSlider();
        UpdateShowSelector();
        UpdateBackgroundCanvas();
    }

    private void HandlePan()
    {
        if(Input.GetMouseButtonDown(1))
        {
            panStartPos = Input.mousePosition;
            contentStartPos = OrbitPoint.position;
            Panning = true;
        }
        if(!Input.GetMouseButton(1))
        {
            Panning = false;
        }
        if(Panning)
        {
            Vector3 delta = Input.mousePosition - panStartPos;
            delta *= panSpeed;
            OrbitPoint.position = contentStartPos + delta;
        }
    }

    private void ClampOrbitPosition()
    {
        List<Plane> planes = GeometryUtility.CalculateFrustumPlanes(Camera.main).Take(4).ToList();
        planes.Add(new Plane(Camera.main.transform.forward, Camera.main.transform.position + Camera.main.transform.forward * minZoom));
        planes.Add(new Plane(-Camera.main.transform.forward, Camera.main.transform.position + Camera.main.transform.forward * maxZoom));
        foreach (Plane plane in planes)
        {
            float dist = plane.GetDistanceToPoint(OrbitPoint.position);
            if(dist < 0)
            {
                OrbitPoint.position = plane.ClosestPointOnPlane(OrbitPoint.position);
            }
        }
    }

    private void UpdateSlider()
    {
        float toSlide = SliderShift * .1f;
        SliderShift -= toSlide;
        NealsonOrImdbSlider.value += toSlide;
        MainScript.Instance.NealsonOrImdb = NealsonOrImdbSlider.value;
    }

    private float SliderShift;

    public void SliderToggled()
    {
        if(MainScript.Instance.NealsonOrImdb < .5)
        {
            SliderShift = 1 - MainScript.Instance.NealsonOrImdb;
        }
        else
        {
            SliderShift = -MainScript.Instance.NealsonOrImdb;
        }
    }

    private void UpdateBackgroundCanvas()
    {
        BackgroundCanvas.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 230 * Camera.main.aspect);
    }

    private void UpdateShowSelector()
    {
        float showXTarget = ShowSelectorHovered ? 0 : -400;
        float newX = Mathf.Lerp(showXTarget, ShowSelectorRect.anchoredPosition.x, Time.deltaTime * 25);
        ShowSelectorRect.anchoredPosition = new Vector2(newX, 0);
    }

    private void UpdateStageInteractionStatus()
    {
        // If the mouse goes down off stage, don't enable it until it is up on stage
        if (UiHovered)
        {
            StageInteractionEnabled = false;
        }
        if(!Input.GetMouseButton(0) && !UiHovered)
        {
            StageInteractionEnabled = true;
        }
    }

    private void HandleOrbit()
    {
        Orbiting = false;
        if (Input.GetMouseButton(0))
        {
            if (leftDragDetector.IsDragging)
            {
                Orbiting = true;
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