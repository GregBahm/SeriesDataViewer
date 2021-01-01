using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static ShowSelector Instance { get; private set; }

    public GameObject ShowSelectorPrefab;
    public RectTransform ShowsCollection;
    private ShowSelectorItem[] showSelectors;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        showSelectors = CreateShowSelectables().ToArray();
    }

    private IEnumerable<ShowSelectorItem> CreateShowSelectables()
    {
        int i = 0;
        foreach (TextAsset series in MainScript.Instance.SeriesAssets)
        {
            GameObject selectable = Instantiate(ShowSelectorPrefab);
            selectable.GetComponentInChildren<TextMeshProUGUI>().text = series.name;
            selectable.transform.SetParent(ShowsCollection, false);
            ShowSelectorItem behavior = selectable.GetComponent<ShowSelectorItem>();
            behavior.ShowIndex = i;
            yield return behavior;
            i++;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseInteractionManager.Instance.ShowSelectorHovered = true;
        MouseInteractionManager.Instance.UiHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseInteractionManager.Instance.ShowSelectorHovered = false;
        MouseInteractionManager.Instance.UiHovered = false;
    }
}
