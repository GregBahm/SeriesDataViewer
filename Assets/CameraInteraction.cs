using System;
using UnityEngine;

namespace Interaction
{
    public class CameraInteraction : MonoBehaviour
    {
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

        private void Start()
        {
            OrbitPoint = new GameObject("Camera Orbit").transform;
            OrbitPoint.LookAt(Camera.main.transform, Vector3.up);
            Camera.main.transform.SetParent(OrbitPoint, true);
            StartOrbit();
        }

        public void HandleMouseScrollwheel()
        {
            Vector3 oldPos = Camera.main.transform.localPosition;
            float scaleFactor = 1f - (Input.mouseScrollDelta.y * scrollSpeed);
            float newPos = oldPos.z * scaleFactor;
            newPos = Mathf.Clamp(newPos, minZoom, maxZoom);

            Camera.main.transform.localPosition = new Vector3(oldPos.x, oldPos.y, newPos);
        }

        public void StartOrbit()
        {
            orbitScreenStart = Input.mousePosition;
            orbitStartAngle = OrbitPoint.eulerAngles;
        }

        public void ContinueOrbit()
        {
            Vector3 screenDelta = orbitScreenStart - Input.mousePosition;
            float yaw = -screenDelta.x * orbitSpeed + orbitStartAngle.y;
            float startAngle = orbitStartAngle.x;
            if(startAngle < 180)
            {
                startAngle += 360;
            }
            float pitch = -screenDelta.y * orbitSpeed + startAngle;
            pitch = Mathf.Clamp(pitch, 280, 440);
            OrbitPoint.rotation = Quaternion.Euler(pitch, yaw, 0);
        }
    }
}