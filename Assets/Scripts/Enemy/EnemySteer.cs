using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyConstants;

public class EnemySteer : MonoBehaviour
{

	private void Awake()
	{
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public Vector3 GetFlockForce(List<Transform> flockList)
	{

		Vector3 force = Vector3.zero;
		if (flockList.Count == 0)
		{
			force += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * 1;
		}
		else
		{
			force += Seperate(flockList);
			force += Align(flockList);
			force += Cohesion(flockList);
		}
		force += Bound(flockList);
		return force;
	}

	private Vector3 Seperate(List<Transform> flockList)
	{
		Vector3 force = Vector3.zero;
		foreach (Transform trans in flockList)
		{
			Vector3 difference = transform.position - trans.position;
			if (difference.magnitude <= SEPERATE_DISTANCE)
			{
				force += difference.normalized / difference.magnitude;
			}
		}
		return force.normalized * SEPERATE_FORCE_SCALE;
	}

	private Vector3 Align(List<Transform> flockList)
	{
		Vector3 force = Vector3.zero;
		foreach (Transform trans in flockList)
		{
			force += trans.GetComponent<Rigidbody>().velocity;
		}
		return force.normalized * ALIGN_FORCE_SCALE;
	}

	private Vector3 Cohesion(List<Transform> flockList)
	{
		Vector3 force = Vector3.zero;
		if (flockList.Count == 0)
		{
			return force;
		}
		int count = 0;
		foreach (Transform trans in flockList)
		{
			count++;
			force += trans.position;
		}
		force /= count;
		force -= transform.position;
		return force.normalized * COHESION_FORCE_SCALE;
	}

	private Vector3 Bound(List<Transform> flockList)
	{
		Vector3 force = Vector3.zero;
		if (Mathf.Abs(transform.position.x) > MAP_WIDTH_BOUND)
		{
			force += new Vector3(-transform.position.x, 0, 0);
		}
		if (Mathf.Abs(transform.position.z) > MAP_HEIGHT_BOUND)
		{
			force += new Vector3(0, 0, -transform.position.z);
		}
		return force.normalized * BOUND_FORCE_SCALE;
	}

	//private Vector2 Align(EnemyFlock flock)
	//{
	//	if (flock.GetCharacterRigidbody2D().velocity == Vector2.zero)
	//	{
	//		return Vector2.zero;
	//	}
	//	Vector2 force = Vector2.zero;
	//	foreach (EnemyManager survivor in flock.GetEnemys())
	//	{
	//		force += survivor.GetComponent<Rigidbody2D>().velocity;
	//	}
	//	return force.normalized * ALIGN_FORCE_SCALE;
	//}

	//private Vector2 GiveWay(EnemyFlock flock)
	//{
	//	Vector2 force = Vector2.zero;
	//	Vector2 playerForward = flock.GetCharacterRigidbody2D().velocity;
	//	if (playerForward == Vector2.zero)
	//	{
	//		return force;
	//	}
	//	playerForward.Normalize();
	//	Vector2 playetToThis = transform.position - flock.GetCharacterTrans().position;
	//	playetToThis.Normalize();
	//	float angle = Vector2.Angle(playerForward, playetToThis);
	//	if (angle <= AVOID_ANGLE)
	//	{
	//		force = playetToThis - playerForward;
	//	}
	//	return force.normalized * GIVE_WAY_FORCE_SCALE;
	//}

	//public Vector2 GetFleeingForce(List<Transform> enemyTransforms)
	//{
	//	Vector2 force = Vector2.zero;
	//	force += Flee(enemyTransforms);
	//	return force;
	//}


	//private Vector2 Flee(List<Transform> enemyTransforms)
	//{
	//	Vector2 force = Vector2.zero;
	//	foreach (Transform enemyTrans in enemyTransforms)
	//	{
	//		Vector2 difference = transform.position - enemyTrans.position;
	//		force += difference.normalized / difference.magnitude;
	//	}
	//	return force.normalized * FLEE_FORCE_SCALE;
	//}

}
