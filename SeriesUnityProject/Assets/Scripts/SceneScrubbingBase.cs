using System.Linq;
using UnityEngine;

public abstract class SceneScrubbingBase : MonoBehaviour
{
    public const string OutputFolder = @"F:\SeriesDataViewerOutput\Recording\";

    protected Transform[] handTransforms;

    protected Transform[] GetAllHandJoints()
    {
        return Hands.Instance.LeftHandProxy.AllJoints
            .Where(item => item != null).Concat(Hands.Instance.RightHandProxy.AllJoints
            .Where(item => item != null)).ToArray();
    }

}
