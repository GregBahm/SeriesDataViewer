using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(MainScript))]
public class SeriesSelector : MonoBehaviour
{
    public static SeriesSelector Instance { get; private set; }

    public GameObject ShowSelectorPrefab;
    public RectTransform ShowsCollection;
    private SelectableShowBehavior[] showSelectors;

    public Color ShownColor;
    public Color BaseColor;
    public Color HoverColor;
    public Color ClickingColor;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        showSelectors = CreateShowSelectables().ToArray();
    }

    private IEnumerable<SelectableShowBehavior> CreateShowSelectables()
    {
        int i = 0;
        foreach (TextAsset series in MainScript.Instance.SeriesAssets)
        {
            GameObject selectable = Instantiate(ShowSelectorPrefab);
            selectable.GetComponentInChildren<TextMeshProUGUI>().text = series.name;
            selectable.transform.SetParent(ShowsCollection, false);
            SelectableShowBehavior behavior = selectable.GetComponent<SelectableShowBehavior>();
            behavior.ShowIndex = i;
            yield return behavior;
            i++;
        }
    }
}
