using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        //transform.position = new Vector3(mainCamera.transform.position.x, 0, mainCamera.transform.position.z);
        transform.position = mainCamera.transform.position;
    }
}
