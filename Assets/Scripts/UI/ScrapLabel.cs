using UnityEngine;

public class ScrapLabel : MonoBehaviour
{
    public TMPro.TextMeshProUGUI nameText, descriptionText;

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
