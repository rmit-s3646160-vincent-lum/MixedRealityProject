using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 eulerAngle;
    float dt;

    // Update is called once per frame
    void FixedUpdate()
    {
        dt = Time.deltaTime;
        transform.Rotate(eulerAngle);
    }
}
