using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationHandleManager : MonoBehaviour
{
    Mesh mesh;
    Vector3[] baseVertices;
    Vector3[] newVertices;

    public Transform RotationHandleRoot;
    public MeshFilter MeshFilter;
    public Transform HandlePip;

    public HololensInputManager ToolManager;

    public float TubeThickness = 0.02f;
    private float currentTubeThickness;

    private Transform rotationHandleHelper;

    private void Start()
    {
        rotationHandleHelper = new GameObject("Rotation Handle Helper").transform;
        mesh = MeshFilter.mesh;
        mesh.MarkDynamic();
        baseVertices = new Vector3[mesh.vertices.Length];
        newVertices = new Vector3[mesh.vertices.Length];
        mesh.vertices.CopyTo(baseVertices, 0);
        baseVertices.CopyTo(newVertices, 0);

        UpdateMesh();
    }
    private void UpdateMesh()
    {
        for (var i = 0; i < newVertices.Length; i++)
        {
            newVertices[i] = GetNewVert(baseVertices[i]);
        }

        mesh.vertices = newVertices;
        mesh.RecalculateBounds();

        HandlePip.localScale = new Vector3(currentTubeThickness * 8, currentTubeThickness, currentTubeThickness * 4);
    }

    private Vector3 GetNewVert(Vector3 baseVert)
    {
        Vector3 basePoint = new Vector3(baseVert.x, baseVert.y, 0).normalized;
        Vector3 fromBaseline = (baseVert - basePoint).normalized;
        Vector3 fromBasePoint = fromBaseline * currentTubeThickness;
        return basePoint + fromBasePoint;
    }

    private void Update()
    {
        UpdateTubeThickness();
        UpdateMesh();
    }

    public void UpdatePassiveRotation()
    {
        Vector3 indexTip = Hands.Instance.LeftHandProxy.IndexTip.position;
        Vector3 thumbTip = Hands.Instance.LeftHandProxy.ThumbTip.position;
        Vector3 rotationUp = thumbTip - indexTip;
        rotationHandleHelper.up = Vector3.Lerp(rotationHandleHelper.up, rotationUp, Time.deltaTime * 16);
        rotationHandleHelper.position = (indexTip + thumbTip) / 2;
        RotationHandleRoot.LookAt(rotationHandleHelper.position, rotationHandleHelper.up);
    }

    internal void UpdateActiveRotation(Vector3 pinchPoint)
    {
        Plane plane = new Plane(rotationHandleHelper.up, RotationHandleRoot.position);
        Vector3 projectedPoint = plane.ClosestPointOnPlane(pinchPoint);
        RotationHandleRoot.LookAt(projectedPoint, rotationHandleHelper.up);
    }

    private void UpdateTubeThickness()
    {
        bool isActive = ToolManager.toolMode == HololensInputManager.LeftHandToolMode.Rotate;
        float targetTubeThickness = isActive ? TubeThickness : 0;
        currentTubeThickness = Mathf.Lerp(currentTubeThickness, targetTubeThickness, Time.deltaTime * 15);
    }
}
