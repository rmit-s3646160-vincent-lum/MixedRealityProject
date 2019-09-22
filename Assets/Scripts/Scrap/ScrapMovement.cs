using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScrapConstants;

public class ScrapMovement : MonoBehaviour
{
	private Rigidbody rigidbody;
	private ScrapInteraction scrapInteraction;

	public float maxSpeed = 5f;


	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
		scrapInteraction = GetComponent<ScrapInteraction>();
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
				FaceForward();
				if (rigidbody.velocity.magnitude > maxSpeed)
				{
					rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
				}
				break;
			case State.manipulating:
				break;
			case State.notPlaced:
				break;
			case State.beingPlaced:
				rigidbody.isKinematic = true;
				rigidbody.constraints = RigidbodyConstraints.FreezeAll;
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

}
