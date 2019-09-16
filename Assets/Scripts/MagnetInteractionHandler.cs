using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class MagnetInteractionHandler : MonoBehaviour, IMixedRealityPointerHandler
{
    public float distanceOffset = 0.5f;
    public float smoothing = 0.001f;
    private Vector3 translation = Vector3.zero;

    [SerializeField] List<uint> pointers;

    Vector3 grabOffset;
    Vector3 rotateStartPoint;

    private TwoHandMoveLogic moveLogic;


    private Rigidbody rb;
    private bool wasKinematic;


    void Awake()
    {
        pointers = new List<uint>();
        moveLogic = new TwoHandMoveLogic();
        rb = GetComponent<Rigidbody>();
        wasKinematic = rb.isKinematic;
        Application.targetFrameRate = 100;
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        pointers.Add(eventData.Pointer.PointerId);

        if(pointers.Count == 1)
        {
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            Vector3 offset = (transform.position - eventData.Pointer.Result.Details.Point) + Vector3.Normalize(transform.position - eventData.Pointer.Position) * distanceOffset;

            MixedRealityPose pointerPose = new MixedRealityPose(eventData.Pointer.Position, eventData.Pointer.Rotation);
            MixedRealityPose hostPose = new MixedRealityPose(transform.position, transform.rotation);
            //MixedRealityPose hostPose = new MixedRealityPose(eventData.Pointer.Position + offset, transform.rotation); // Edited to bring object to the controller

            moveLogic.Setup(pointerPose, eventData.Pointer.Result.Details.Point, hostPose, transform.localScale);
        }
        else
        {
            rotateStartPoint = eventData.Pointer.Position;
        }

        eventData.Use();
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        if(pointers.First() == eventData.Pointer.PointerId)
        {
            MixedRealityPose pointerPose = new MixedRealityPose(eventData.Pointer.Position, eventData.Pointer.Rotation);
            Vector3 targetPosition = moveLogic.Update(pointerPose, transform.rotation, transform.localScale, false, true, MovementConstraintType.None) + translation;
            float lerpAmount = 1.0f - Mathf.Pow(smoothing, Time.deltaTime);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, lerpAmount);
            transform.position = smoothedPosition;

            // Move toward controller
            Vector3 offset = (transform.position - eventData.Pointer.Result.Details.Point) + Vector3.Normalize(transform.position - eventData.Pointer.Position) * distanceOffset;
            smoothedPosition = Vector3.Lerp(transform.position, eventData.Pointer.Position + offset, lerpAmount);
            transform.position = smoothedPosition;
        }
        else
        {

            // Rotate if not first pointer
            float rotationAmount = (Camera.main.WorldToScreenPoint(rotateStartPoint).x - Camera.main.WorldToScreenPoint(eventData.Pointer.Position).x) * 0.25f;
            transform.Rotate(0, rotationAmount, 0);

            rotateStartPoint = eventData.Pointer.Position;

        }
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        pointers.Remove(eventData.Pointer.PointerId);

        if(pointers.Count == 0)
        {
            if (rb != null)
            {
                rb.isKinematic = wasKinematic;
            }
        }

        eventData.Use();
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
    }

}
