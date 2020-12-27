using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(CameraInteraction))]
    public class MouseInteractionManager : MonoBehaviour
    {
        private CameraInteraction cameraInteraction;

        private DragDetector leftDragDetector;
        private DragDetector rightDragDetector;

        [SerializeField]
        private float dragStartDistance = 2;

        // Start is called before the first frame update
        void Start()
        {
            cameraInteraction = GetComponent<CameraInteraction>();
            leftDragDetector = new DragDetector(dragStartDistance);
            rightDragDetector = new DragDetector(dragStartDistance);
        }

        // Update is called once per frame
        void Update()
        {
            HandleOrbit();
            HandlePan();
            cameraInteraction.HandleMouseScrollwheel();
        }

        private void HandlePan()
        {
            if (Input.GetMouseButton(1))
            {
                if (rightDragDetector.IsDragging)
                {
                    cameraInteraction.ContinuePan();
                }
                else
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        rightDragDetector.DragStartPos = Input.mousePosition;
                        cameraInteraction.StartPan();
                    }
                    else
                    {
                        rightDragDetector.UpdateIsDragging();
                    }
                }
            }
            else
            {
                rightDragDetector.IsDragging = false;
            }
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