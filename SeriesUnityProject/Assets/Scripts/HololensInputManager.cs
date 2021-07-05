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
    public ProxyButton ToggleColor;

    public Vector3 RotationHelperPos { get { return rotationHelper.position; } }

    public RotationHandleManager RotationHandle;

    public SkinnedMeshRenderer ManipulationCursor;

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

    private void OnMoveClicked(object sender, EventArgs e)
    {
        toolMode = LeftHandToolMode.Move;
        UpdateToolButtonToggles();
    }

    private void OnRotateClicked(object sender, EventArgs e)
    {
        toolMode = LeftHandToolMode.Rotate;
        UpdateToolButtonToggles();
    }

    private void OnScaleClicked(object sender, EventArgs e)
    {
        toolMode = LeftHandToolMode.Scale;
        UpdateToolButtonToggles();
    }

    private void UpdateToolButtonToggles()
    {
        MoveButton.Toggled = toolMode == LeftHandToolMode.Move;
        RotateButton.Toggled = toolMode == LeftHandToolMode.Rotate;
        ScaleButton.Toggled = toolMode == LeftHandToolMode.Scale;
    }

    void Update()
    {
        UpdatePinchAndDrag();
        UpdateDrillDown();
        UpdateImdbToNelson();
        UpdateManipulationCursor();
    }

    private void UpdateManipulationCursor()
    {
        float prog = PinchDetector.FingerDistance / .1f;
        ManipulationCursor.SetBlendShapeWeight(0, prog * 100);
        if(PinchDetector.Pinching)
        {
            ManipulationCursor.transform.position = translationHelper.position;
        }
        else
        {
            ManipulationCursor.transform.position = PinchDetector.PinchPoint.position;
        }
        ManipulationCursor.transform.rotation = PinchDetector.PinchPoint.rotation;
        UpdateColor();
    }

    private void UpdateColor()
    {
        float target = ToggleColor.Toggled ? 1 : 0;
        MainScript.Instance.ShowColor = Mathf.Lerp(MainScript.Instance.ShowColor, target, Time.deltaTime * 10);
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

            RotationHandle.RotationHandleRoot.SetParent(null, true);
            MainStage.SetParent(RotationHandle.RotationHandleRoot, true);
        }
        if(wasRotating && !PinchDetector.Pinching)
        {
            MainStage.SetParent(null, true);
            RotationHandle.RotationHandleRoot.SetParent(MainStage, true);
        }
        if(PinchDetector.Pinching)
        {
            translationHelper.position = GetDeadzoneMovement();
            //RotationHandle.UpdateActiveRotation(translationHelper.position);
        }
        else
        {
            RotationHandle.UpdatePassiveRotation();
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
        EpisodeBehavior drilledEpisode = GetClosestEpisode();
        MainScript.Instance.DrilledEpisode = drilledEpisode;
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
