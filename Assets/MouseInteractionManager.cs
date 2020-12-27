using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(MainScript))]
    [RequireComponent(typeof(CameraInteraction))]
    public class MouseInteractionManager : MonoBehaviour
    {
        private MainScript main;
        private CameraInteraction cameraInteraction;

        private DragDetector leftDragDetector;

        [SerializeField]
        private float dragStartDistance = 2;

        void Start()
        {
            main = GetComponent<MainScript>();
            cameraInteraction = GetComponent<CameraInteraction>();
            leftDragDetector = new DragDetector(dragStartDistance);
        }

        void Update()
        {
            HandleCentering();
            HandleOrbit();
            cameraInteraction.HandleMouseScrollwheel();
        }

        private void HandleCentering()
        {
            float yTarget = main.ShownSeries.GetHeightForCameraOrbit();
            cameraInteraction.OrbitPoint.localPosition = new Vector3(0, yTarget, 0);
        }

        private void HandleOrbit()
        {
            if (Input.GetMouseButton(0))
            {
                if (leftDragDetector.IsDragging)
                {
                    cameraInteraction.ContinueOrbit();
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        leftDragDetector.DragStartPos = Input.mousePosition;
                        cameraInteraction.StartOrbit();
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
}