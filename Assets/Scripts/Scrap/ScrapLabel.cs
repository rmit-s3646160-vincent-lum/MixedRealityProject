using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrapLabel : MonoBehaviour
{
    [TextArea]
    public string text;
    public bool alwaysShow = false;
    public GameObject labelPrefab;
    Transform label;
    Transform canvas;
    IMixedRealityPointer pointer;

    void Start()
    {
        // Get canvas
        var canvasComponent = FindObjectOfType<Canvas>();
        if(labelPrefab == null || canvasComponent == null)
            Destroy(this);

        canvas = canvasComponent.transform;

        // Instantiate label
        label = Instantiate(labelPrefab, canvas).transform;
        label.name = text;
        label.gameObject.SetActive(false);

        // Set text
        var labelText = label.GetComponentInChildren<TextMeshProUGUI>();
        if (labelText != null)
            labelText.text = text;
    }

    void Update()
    {
        if(pointer != null)
        {
            label.position = Camera.main.WorldToScreenPoint(pointer.Result.Details.Point);
        }
    }

    public void OnHoverEnter(FocusEventData eventData)
    {
        if (alwaysShow)
            return;

        label.gameObject.SetActive(true);
        pointer = eventData.Pointer;
    }

    public void OnHoverExit(FocusEventData eventData)
    {
        if (alwaysShow)
            return;

        label.gameObject.SetActive(false);
        pointer = null;
    }

    public void OnDestroy()
    {
        Destroy(label);
    }
}
