using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScrapConstants;

public class ScrapMovement : MonoBehaviour
{
	private Rigidbody rigidbody;
	private Collider collider;
	private ScrapInteraction scrapInteraction;

	public float maxSpeed = 5f;


	private void Awake()
	{
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
				//FaceForward();
				//if (rigidbody.velocity.magnitude > maxSpeed)
				//{
				//	rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
				//}
				break;
			case State.manipulating:
				break;
			case State.notPlaced:
				break;
			case State.beingPlaced:
				break;

		}
	}

	private void FaceForward()
	{
		if (rigidbody.velocity != Vector3.zero)
		{
			float angle = rigidbody.velocity.z < 0 ?
				Vector3.Angle(new Vector3(1, 0, 0), rigidbody.velocity.normalized)
				: -Vector3.Angle(new Vector3(1, 0, 0), rigidbody.velocity.normalized);
			transform.rotation = Quaternion.Euler(0, angle, 0);
		}
	}

	public void StopMove()
	{
		rigidbody.velocity = Vector3.zero;
	}

	public void ApplyForce(Vector3 force)
	{
		rigidbody.AddForce(force, ForceMode.Impulse);
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

	private void OnCollisionStay(Collision collision)
	{
        if (scrapInteraction.GetState() != State.manipulating && scrapInteraction.GetState() != State.beingPlaced)
        {
            ChangeState(State.beingPlaced);
        }

        /*
		if (scrapInteraction.GetState() == State.notPlaced)
		{
			if (collision.gameObject.layer == PLATFORM_LAYER)
			{
				ChangeState(State.beingPlaced);
			}
			else if (collision.gameObject.tag == SCRAP_TAG)
			{
				if (collision.gameObject.GetComponent<ScrapInteraction>().GetState() == State.beingPlaced)
				{
					ChangeState(State.beingPlaced);
				}
			}
		}*/
    }


}
