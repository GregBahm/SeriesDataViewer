using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableShowBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int ShowIndex;
    public Image Backdrop;
    private bool isHovered;

    private void Update()
    {
        Color targetColor = GetTargetColor();
        Backdrop.color = targetColor;
    }

    private Color GetTargetColor()
    {
        bool isClicking = isHovered && Input.GetMouseButton(0);
        if(isClicking)
        {
            return SeriesSelector.Instance.ClickingColor;
        }
        bool isShown = MainScript.Instance.ShowToShow == ShowIndex;
        if(isShown)
        {
            return SeriesSelector.Instance.ShownColor;
        }
        return isHovered ? SeriesSelector.Instance.HoverColor : SeriesSelector.Instance.BaseColor;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        isHovered = false;
    }

    public void Clicked()
    {
        MainScript.Instance.ShowToShow = ShowIndex;
    }
}
