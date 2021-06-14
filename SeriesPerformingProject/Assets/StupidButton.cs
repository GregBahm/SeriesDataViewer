using System;
using UnityEngine;

public class StupidButton : MonoBehaviour
{
    [SerializeField]
    private ButtonInteractionStyles interactionStyle;
    public ButtonInteractionStyles InteractionStyle => interactionStyle;

    [SerializeField]
    private bool toggled;
    public bool Toggled { get => toggled; set => toggled = value; }

    private ButtonState oldState;
    public ButtonState State { get; private set; }

    public event EventHandler Clicked;

#if UNITY_EDITOR
    [SerializeField]
    private bool testClick;
#endif

    private void Start()
    {
        State = ButtonState.Ready;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (testClick)
        {
            testClick = false;
            OnClicked();
        }
#endif
        State = UpdateState();
        if (State == ButtonState.Pressed && oldState != ButtonState.Pressed)
        {
            OnClicked();
        }
        oldState = State;
    }

    private void OnClicked()
    {
        if (interactionStyle == ButtonInteractionStyles.ToggleButton)
        {
            Toggled = !Toggled;
        }
        EventHandler handler = Clicked;
        handler?.Invoke(this, EventArgs.Empty);
        Debug.Log("click");
    }

    private ButtonState UpdateState()
    {
        Vector3 localPos = GetLocalFingerPosition();
        bool pressed = localPos.magnitude < 1f;
        if (pressed)
        {
            return ButtonState.Pressed;
        }

        return ButtonState.Ready;
    }

    private Vector3 GetLocalFingerPosition()
    {
        Vector3 fingerPos = Hands.Instance.RightHandProxy.IndexTip.position;
        return transform.InverseTransformPoint(fingerPos);
    }

    public enum ButtonInteractionStyles
    {
        ToggleButton,
        ClickButton
    }
}

public enum ButtonState
{
    Disabled,
    Ready,
    Pressed
}