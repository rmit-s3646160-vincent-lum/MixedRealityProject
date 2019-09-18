using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using static ScrapConstants;

public class ScrapInteraction : MonoBehaviour
{
	[SerializeField] private State state;

	public float offsetDistance = 2f;
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
	}

	public void OnPointerDown(MixedRealityPointerEventData eventData)
	{
		SetState(State.manipulating);
		pointers.Add(eventData.Pointer.PointerId);

		if (pointers.Count == 1)
		{
			if (rb != null)
			{
				rb.isKinematic = true;
			}

			MixedRealityPose pointerPose = new MixedRealityPose(eventData.Pointer.Position, eventData.Pointer.Rotation);
			MixedRealityPose hostPose = new MixedRealityPose(transform.position, transform.rotation);

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
		if (pointers.First() == eventData.Pointer.PointerId)
		{
			MixedRealityPose pointerPose = new MixedRealityPose(eventData.Pointer.Position, eventData.Pointer.Rotation);
			Vector3 targetPosition = moveLogic.Update(pointerPose, transform.rotation, transform.localScale, false, true, MovementConstraintType.None) + translation;

            Vector3 grabOffset = transform.position - eventData.Pointer.Result.Details.Point;
            Vector3 offsetFromController = Vector3.Normalize(targetPosition - eventData.Pointer.Position) * offsetDistance;
            Vector3 distanceVector = targetPosition - eventData.Pointer.Position;
            targetPosition = targetPosition - distanceVector + offsetFromController + grabOffset;

            float lerpAmount = 1.0f - Mathf.Pow(smoothing, Time.deltaTime);
			Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, lerpAmount);
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

		if (pointers.Count == 0)
		{
            ReleaseScrap(eventData);
            //ShootScrap(eventData);
		}

		eventData.Use();
	}

	public void OnPointerClicked(MixedRealityPointerEventData eventData)
	{
	}

    public void ShootScrap(MixedRealityPointerEventData eventData)
    {
        if (rb != null)
        {
            rb.isKinematic = wasKinematic;
            if (eventData.Pointer.Controller != null)
            {
                float speed = 20;
                Vector3 direction = eventData.Pointer.Result.Details.Point - eventData.Pointer.Position;
                rb.velocity = direction.normalized * speed;
            }
        }
    }

    public void ReleaseScrap(MixedRealityPointerEventData eventData)
    {
        if (rb != null)
        {
            rb.isKinematic = wasKinematic;
            if (eventData.Pointer.Controller != null)
            {
                rb.velocity = eventData.Pointer.Controller.Velocity;
            }
        }
    }

	public void SetState(State state)
	{
		this.state = state;
	}

	public State GetState()
	{
		return state;
	}
}
