﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOnFall : MonoBehaviour
{
    public Transform spawnPos;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.position.y < -50)
        {
            if(spawnPos != null)
            {
                transform.position = spawnPos.position;
            }
            else
            {
                transform.position = Camera.main.transform.position;
            }

            Rigidbody rb = GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.velocity = Vector3.zero;
            }
        }
    }
}
