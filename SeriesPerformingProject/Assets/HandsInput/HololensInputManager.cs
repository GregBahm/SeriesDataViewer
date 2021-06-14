using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HololensInputManager : MonoBehaviour
{

    public float Deadzone = .1f;

    private Transform translationHelper;
    private Transform rotationHelper;

    public Transform DrillDownTip;
    public float DrilldownThreshold;

    public PinchDetector PinchDetector;

    public Transform MainStage;

    public ProxyButton MoveButton;
    public ProxyButton RotateButton;
    public ProxyButton ScaleButton;

    public ProxyButton ImdbToNelsonToggle;

    public enum LeftHandToolMode
    {
        Move,
        Rotate,
        Scale
    }

    public LeftHandToolMode toolMode;

    private void Start()
    {
        translationHelper = new GameObject("Translation Helper").transform;
        rotationHelper = new GameObject("Rotation Helper").transform;
        MoveButton.Clicked += OnMoveClicked;
        RotateButton.Clicked += OnRotateClicked;
        ScaleButton.Clicked += OnScaleClicked;
    }

    private void OnScaleClicked(object sender, EventArgs e)
    {
        toolMode = LeftHandToolMode.Scale;
    }

    private void OnRotateClicked(object sender, EventArgs e)
    {
        toolMode = LeftHandToolMode.Rotate;
    }

    private void OnMoveClicked(object sender, EventArgs e)
    {
        toolMode = LeftHandToolMode.Move;
    }

    void Update()
    {
        UpdatePinchAndDrag();
        if(!PinchDetector.Pinching)
        {
            UpdateDrillDown();
        }
        UpdateImdbToNelson();
    }

    private void UpdateImdbToNelson()
    {
        float target = ImdbToNelsonToggle.Toggled ? 1 : 0;
        MainScript.Instance.NealsonOrImdb = Mathf.Lerp(MainScript.Instance.NealsonOrImdb, target, Time.deltaTime * 10);
    }

    private void UpdatePinchAndDrag()
    {
        switch (toolMode)
        {
            case LeftHandToolMode.Move:
                UpdateDragging();
                break;
            case LeftHandToolMode.Rotate:
                UpdateRotating();
                break;
            case LeftHandToolMode.Scale:
            default:
                UpdateResizing();
                break;
        }
    }

    private float startDist;
    private float startScale;
    private bool wasResizing;
    private void UpdateResizing()
    {
        if (!wasResizing && PinchDetector.PinchBeginning)
        {
            translationHelper.position = PinchDetector.PinchPoint.position;
            startDist = (MainStage.position - translationHelper.position).magnitude;
            startScale = MainStage.localScale.x;
        }
        if(PinchDetector.Pinching)
        {
            translationHelper.position = GetDeadzoneMovement();
            float newDist = (MainStage.position - translationHelper.position).magnitude;
            float diff = newDist / startDist;
            float newScale = startScale * diff;
            MainStage.localScale = new Vector3(newScale, newScale, newScale);
        }
    }

    private Vector3 rotationUp;
    private bool wasRotating;
    private void UpdateRotating()
    {
        if(!wasRotating && PinchDetector.PinchBeginning)
        {
            translationHelper.position = PinchDetector.PinchPoint.position;
            rotationHelper.position = MainStage.position;
            rotationUp = MainStage.up;
            rotationHelper.LookAt(translationHelper, rotationUp);
            MainStage.SetParent(rotationHelper, true);
        }
        if(PinchDetector.Pinching)
        {
            translationHelper.position = GetDeadzoneMovement();

            rotationHelper.LookAt(translationHelper, MainStage.up);
        }
        if(wasRotating && !PinchDetector.Pinching)
        {
            MainStage.SetParent(null, true);
        }
        wasRotating = PinchDetector.Pinching;
    }

    private Vector3 transformHelperStartPos;
    private Vector3 mainStageStartPos;
    private bool wasDragging;
    private void UpdateDragging()
    {
        if(!wasDragging && PinchDetector.PinchBeginning)
        {
            translationHelper.position = PinchDetector.PinchPoint.position;
            transformHelperStartPos = translationHelper.position;
            mainStageStartPos = MainStage.position;
        }
        if(PinchDetector.Pinching)
        {
            translationHelper.position = GetDeadzoneMovement();
            MainStage.position = mainStageStartPos + (translationHelper.position - transformHelperStartPos);
        }
        wasDragging = PinchDetector.Pinching;
    }

    private void UpdateDrillDown()
    {
        EpisodeBehavior closestEpisode = GetClosestEpisode();
        EpisodeDrillDownManager.Instance.DrilledEpisode = closestEpisode;
    }
    
    private Vector3 GetDeadzoneMovement()
    {
        Vector3 toTarget = PinchDetector.PinchPoint.position - translationHelper.position;
        float distToTarget = toTarget.magnitude;
        float deadDist = Mathf.Max(0, distToTarget - Deadzone);
        return translationHelper.position + toTarget.normalized * deadDist;
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
