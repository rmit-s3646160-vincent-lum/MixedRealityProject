using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControllerInputHandler : MonoBehaviour
{
    public static ControllerInputHandler instance;

    public GameObject inventoryPanel;
    public TextMeshPro popUpMessage;
    public bool shoot = false;

    void Awake()
    {
        // Ensure there is only one GameManager instance
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleShoot()
    {
        shoot = !shoot;

        if(popUpMessage != null)
        {
            if(shoot)
                popUpMessage.text = "Shooting enabled";
            else
                popUpMessage.text = "Shooting disabled";

            popUpMessage.gameObject.SetActive(true);
            StartCoroutine(DisableAfterSeconds(popUpMessage.gameObject, 1));
        }
    }

    public void ToggleInventory()
    {
        if(inventoryPanel != null)
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }

    private IEnumerator DisableAfterSeconds(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }
}
