using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HololensInputManager : MonoBehaviour
{
    public float Deadzone = .1f;

    private Transform transformHelper;

    public Transform DrillDownTip;
    public float DrilldownThreshold;

    public PinchDetector PinchDetector;

    public Transform MainStage;

    public enum LeftHandToolMode
    {
        Dragging,
        Rotating,
        Resizing
    }

    public LeftHandToolMode toolMode;

    private void Start()
    {
        transformHelper = new GameObject("Transform Helper").transform;
    }

    void Update()
    {
        UpdatePinchAndDrag();
        UpdateDrillDown();
    }

    private void UpdatePinchAndDrag()
    {
        switch (toolMode)
        {
            case LeftHandToolMode.Dragging:
                UpdateDragging();
                break;
            case LeftHandToolMode.Rotating:
                UpdateRotating();
                break;
            case LeftHandToolMode.Resizing:
            default:
                UpdateResizing();
                break;
        }
    }

    private bool wasResizing;
    private void UpdateResizing()
    {
        throw new NotImplementedException();
    }

    private bool wasRotating;
    private void UpdateRotating()
    {
        throw new NotImplementedException();
    }

    private Vector3 transformHelperStartPos;
    private Vector3 mainStageStartPos;
    private bool wasDragging;
    private void UpdateDragging()
    {
        if(!wasDragging && PinchDetector.PinchBeginning)
        {
            transformHelper.position = PinchDetector.PinchPoint.position;
            transformHelperStartPos = transformHelper.position;
            mainStageStartPos = MainStage.position;
        }
        if(PinchDetector.Pinching)
        {
            transformHelper.position = GetDeadzoneMovement();
            MainStage.position = mainStageStartPos + (transformHelper.position - transformHelperStartPos);
        }
        wasDragging = PinchDetector.PinchBeginning;
    }

    private void UpdateDrillDown()
    {
        EpisodeBehavior closestEpisode = GetClosestEpisode();
        EpisodeDrillDownManager.Instance.DrilledEpisode = closestEpisode;
    }
    
    private Vector3 GetDeadzoneMovement()
    {
        Vector3 toTarget = PinchDetector.PinchPoint.position - transformHelper.position;
        float distToTarget = toTarget.magnitude;
        float deadDist = Mathf.Max(0, distToTarget - Deadzone);
        return transformHelper.position + toTarget.normalized * deadDist;
    }

    private EpisodeBehavior GetClosestEpisode()
    {
        ShowBehavior activeShow = MainScript.Instance.ShownSeries;
        float minDist = DrilldownThreshold;
        EpisodeBehavior ret = null;
        foreach(EpisodeBehavior episode in activeShow.EpisodeBehaviors)
        {
            BoxCollider collider = episode.GetComponent<BoxCollider>();
            Vector3 boundsPoint = collider.ClosestPointOnBounds(DrillDownTip.position);
            float dist = (DrillDownTip.position - boundsPoint).magnitude;
            if(dist < minDist)
            {
                ret = episode;
                minDist = dist;
            }
        }
        return ret;
    }
}
