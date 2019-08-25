﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
    private LineRenderer lr;
    private RaycastHit hit;
    private float maxLen = 1000;
    public LayerMask layermask;

    private GameObject highlighted;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hit)){
            CastLaser(hit);
            HighlightTarget(hit);

        }
    }

    public void CastLaser(RaycastHit hit)
    {
        lr.SetPosition(0, transform.position);

        if (hit.collider)
        {
            lr.SetPosition(1, hit.point);
        }
        else
        {
            lr.SetPosition(1, transform.forward * maxLen);
        }
    }

    public void HighlightTarget(RaycastHit hit)
    {
        var other = hit.collider.gameObject;
        if(1 << other.layer == layermask)
        {
            Outline outline;
            if (!highlighted)
            {
                // Enable outline of new object
                outline = other.GetComponent<Outline>();
                if (outline)
                    outline.enabled = true;

                highlighted = other;
            }
            else if(other.GetInstanceID() != highlighted.GetInstanceID())
            {
                // If the hit is a new object, disable outline of previous object
                outline = highlighted.GetComponent<Outline>();
                if (outline)
                    outline.enabled = false;

                // Enable outline of new object
                outline = other.GetComponent<Outline>();
                if (outline)
                    outline.enabled = true;

                highlighted = other;
            }
        }
        else
        {
            // Disable outline
            if (highlighted)
            {
                highlighted.GetComponent<Outline>().enabled = false;
                highlighted = null;
            }
        }
    }
}
