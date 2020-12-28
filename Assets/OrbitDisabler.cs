using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OrbitDisabler : MonoBehaviour, IPointerDownHandler
{
    public MouseInteractionManager Orbiter;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Orbiter.OrbitDisabled = true;
    }
}
