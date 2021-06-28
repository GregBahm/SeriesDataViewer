using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTraceMeshTests : MonoBehaviour
{
    Mesh mesh;
    Vector3[] baseVertices;
    Vector3[] newVertices;

    public float TubeThickness;

    [Range(0, 1)]
    public float AngularWeight;
    public float AngularWeightRamp;
    public float ClampAdd;

    public bool ContinuouslyUpdate;

    public Transform HandlePip;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
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

        HandlePip.localScale = new Vector3(TubeThickness * 8, TubeThickness, TubeThickness * 4);
    }

    void Update()
    {
        if(ContinuouslyUpdate)
        {
            UpdateMesh();
        }
    }

    private Vector3 GetNewVert(Vector3 baseVert)
    {
        Vector3 basePoint = new Vector3(baseVert.x, baseVert.y, 0).normalized;
        Vector3 fromBaseline = (baseVert - basePoint).normalized;
        Vector3 fromBasePoint = fromBaseline * TubeThickness;

        float degree = Vector2.SignedAngle(basePoint, Vector2.up);
        float pow = Mathf.Abs(degree) / 180;
        pow = 1 - ((1 - pow) * ClampAdd);
        pow = Mathf.Clamp01(pow);
        pow = Mathf.Pow(pow, AngularWeightRamp);
        Vector3 finalBasepoint = Vector3.Lerp(fromBasePoint, fromBasePoint * pow * 2, AngularWeight);

        return basePoint + finalBasepoint;
    }
}
