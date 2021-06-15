using UnityEngine;

public class Hands : MonoBehaviour
{
    public static Hands Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public HandProxy LeftHandProxy => this.leftHandProxy;
    [SerializeField]
    private HandProxy leftHandProxy;
    public HandProxy RightHandProxy => this.rightHandProxy;
    [SerializeField]
    private HandProxy rightHandProxy;
}