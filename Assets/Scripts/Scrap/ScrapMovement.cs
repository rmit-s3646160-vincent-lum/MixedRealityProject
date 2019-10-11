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
			case State.initial:
				ApplyForce(scrapSteer.GetForce());
				UpdatePosition();
				break;
			case State.manipulating:
				break;
			case State.notPlaced:
				break;
			case State.beingPlaced:
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

	public void ChangeState(State state)
	{
		switch (state)
		{
			case State.initial:
				scrapInteraction.SetState(State.initial);
				break;
			case State.manipulating:
				scrapInteraction.SetState(State.manipulating);
				rigidbody.constraints = RigidbodyConstraints.None;
				break;
			case State.notPlaced:
				scrapInteraction.SetState(State.notPlaced);
				break;
			case State.beingPlaced:
				rigidbody.constraints = RigidbodyConstraints.FreezeAll;
				scrapInteraction.SetState(State.beingPlaced);
				break;

		}
	}

	public void StartManipulating()
	{
		ChangeState(State.manipulating);
	}

	public void StopManipulating()
	{
		ChangeState(State.notPlaced);
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
