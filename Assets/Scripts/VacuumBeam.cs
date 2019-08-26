using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumBeam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Do something with the collision
        var other = collision.gameObject;
        if(other.layer == 9) // Targettables layer
        {
            // Pull the object towards a position
        }
    }
}
