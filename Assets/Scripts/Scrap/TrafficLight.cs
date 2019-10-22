using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [SerializeField] GameObject redLight, greenLight;    

    // Start is called before the first frame update
    void Start()
    {
        greenLight.SetActive(false);
    }

    public void SwitchLights()
    {
        redLight.SetActive(!redLight.activeSelf);
        greenLight.SetActive(!greenLight.activeSelf);
    }
}
