using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ScrapConstants;
using Random = UnityEngine.Random;

public class ScrapMovement : MonoBehaviour
{
	public ScrapSteer scrapSteer;
	private Rigidbody rigidbody;
	private Collider collider;
	private ScrapInteraction scrapInteraction;

	private float maxSpeed = 1f;
	private Vector3 currentVelocity;

	private void Awake()
	{
		currentVelocity = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
		rigidbody = GetComponent<Rigidbody>();
		scrapInteraction = GetComponent<ScrapInteraction>();
		collider = GetComponent<Collider>();
	}
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	private void FixedUpdate()
	{
		switch (scrapInteraction.GetState())
		{
			case ScrapState.initial:
				//ApplyForce(scrapSteer.GetForce());
				//UpdatePosition();
				break;
			case ScrapState.manipulating:
				break;
			case ScrapState.notPlaced:
				break;
			case ScrapState.placed:
				break;

		}
	}

	private void UpdatePosition()
	{
		transform.position += currentVelocity * Time.deltaTime;
		FaceForward();
	}

	private void FaceForward()
	{
		if (currentVelocity != Vector3.zero)
		{
			float angle = currentVelocity.z < 0 ?
				Vector3.Angle(new Vector3(1, 0, 0), currentVelocity.normalized)
				: -Vector3.Angle(new Vector3(1, 0, 0), currentVelocity.normalized);
			transform.rotation = Quaternion.Euler(0, angle, 0);
		}
	}

	public void ApplyForce(Vector3 force)
	{
		currentVelocity += force;
		if (currentVelocity.magnitude > maxSpeed)
		{
			currentVelocity.Normalize();
			currentVelocity *= maxSpeed;
		}
	}

	public void ChangeState(ScrapState state)
	{
		switch (state)
		{
			case ScrapState.initial:
				scrapInteraction.SetState(ScrapState.initial);
				break;
			case ScrapState.manipulating:
				scrapInteraction.SetState(ScrapState.manipulating);
				rigidbody.constraints = RigidbodyConstraints.None;
				break;
			case ScrapState.notPlaced:
				scrapInteraction.SetState(ScrapState.notPlaced);
				break;
			case ScrapState.placed:
				rigidbody.constraints = RigidbodyConstraints.FreezeAll;
				scrapInteraction.SetState(ScrapState.placed);
				break;

		}
	}

	public void StartManipulating()
	{
		ChangeState(ScrapState.manipulating);
	}

	public void StopManipulating()
	{
		ChangeState(ScrapState.notPlaced);
	}

	/*
	private void OnCollisionStay(Collision collision)
	{
        if (scrapInteraction.GetState() != State.manipulating && scrapInteraction.GetState() != State.beingPlaced)
        {
            ChangeState(State.beingPlaced);
            OnPlacement.Invoke();
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if (scrapInteraction.GetState() != State.manipulating && scrapInteraction.GetState() != State.notPlaced)
        {
            ChangeState(State.notPlaced);
            rigidbody.constraints = RigidbodyConstraints.None;
        }
    }*/


}
