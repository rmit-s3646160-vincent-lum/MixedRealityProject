using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrapLabelSpawner : MonoBehaviour
{
	public string nameText;
	[TextArea]
	public string descriptionText;
	public bool alwaysShow = false;
	public ScrapLabel labelPrefab;
	Transform label;
	Transform canvas;
	IMixedRealityPointer pointer;

	void Start()
	{
		// Remove script if no label prefab set
		if (labelPrefab == null)
			Destroy(gameObject);

		// Instantiate label
		label = Instantiate(labelPrefab).transform;

		// Set text
		var sl = label.GetComponent<ScrapLabel>();
		sl.SetNameText(nameText);
		sl.SetDescriptionText(descriptionText);
		sl.ShowDescription(false);

		if (!alwaysShow)
			label.gameObject.SetActive(false);
	}

	void Update()
	{
		if (pointer != null)
		{
			label.position = pointer.Result.Details.Point;
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

	public void OnPointerDown(MixedRealityPointerEventData eventData)
	{
		// Show description
		label.GetComponent<ScrapLabel>().ShowDescription(true);
	}

	public void OnPointerUp(MixedRealityPointerEventData eventData)
	{
		// Hide description
		label.GetComponent<ScrapLabel>().ShowDescription(false);
	}

	public void OnDestroy()
	{
        if(label != null)
		    Destroy(label.gameObject);
	}
}
