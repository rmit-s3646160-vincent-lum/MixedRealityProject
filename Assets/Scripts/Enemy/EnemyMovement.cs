using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
	private Rigidbody rigidbody;

	public float maxSpeed = 5f;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
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
		FaceForward();
		if (rigidbody.velocity.magnitude > maxSpeed)
		{
			rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
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
