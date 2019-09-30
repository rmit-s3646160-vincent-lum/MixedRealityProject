using UnityEngine;

public class ScrapLabel : MonoBehaviour
{
    public TMPro.TextMeshPro nameText, descriptionText;

    private void Update()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }

    public void SetNameText(string text)
    {
        if(nameText != null)
            nameText.text = text;
    }

    public void SetDescriptionText(string text)
    {
        if (descriptionText != null)
            descriptionText.text = text;
    }

    public void ShowDescription(bool value)
    {
        descriptionText.gameObject.SetActive(value);
    }
}
