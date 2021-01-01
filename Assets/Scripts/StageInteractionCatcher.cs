using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageInteractionCatcher : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseInteractionManager.Instance.UiHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseInteractionManager.Instance.UiHovered = false;
    }
}
