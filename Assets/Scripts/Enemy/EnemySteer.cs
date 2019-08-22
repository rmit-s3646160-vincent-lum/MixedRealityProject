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

	public Vector2 GetFlockForce(EnemyFlock flock)
	{
		Vector2 force = new Vector2();
		force += Follow(flock);
		force += Seperate(flock);
		force += KeepDistanceFromCharacter(flock);
		force += Align(flock);
		force += GiveWay(flock);
		return force;
	}

	private Vector2 Follow(EnemyFlock flock)
	{
		Vector2 force = flock.GetCharacterTrans().position - transform.position;
		if (force.magnitude > FOLLOW_DISTANCE)
		{
			return force.normalized * FOLLOW_FORCE_SCALE;
		}
		return Vector2.zero;
	}

	private Vector2 Seperate(EnemyFlock flock)
	{
		Vector2 force = Vector2.zero;
		foreach (EnemyManager survivor in flock.GetEnemys())
		{
			Vector2 difference = transform.position - survivor.transform.position;
			if (difference.magnitude < SEPERATE_DISTANCE)
			{
				force += difference;
			}
		}
		return force.normalized * SEPERATE_FORCE_SCALE;
	}

	private Vector2 KeepDistanceFromCharacter(EnemyFlock flock)
	{
		Vector2 force = Vector2.zero;
		Vector2 difference = transform.position - flock.GetCharacterTrans().position;
		if (difference.magnitude <= KEEP_DISTANCE)
		{
			force = difference;
		}
		return force.normalized * SEPERATE_FORCE_SCALE;
	}

	private Vector2 Align(EnemyFlock flock)
	{
		if (flock.GetCharacterRigidbody2D().velocity == Vector2.zero)
		{
			return Vector2.zero;
		}
		Vector2 force = Vector2.zero;
		foreach (EnemyManager survivor in flock.GetEnemys())
		{
			force += survivor.GetComponent<Rigidbody2D>().velocity;
		}
		return force.normalized * ALIGN_FORCE_SCALE;
	}

	private Vector2 GiveWay(EnemyFlock flock)
	{
		Vector2 force = Vector2.zero;
		Vector2 playerForward = flock.GetCharacterRigidbody2D().velocity;
		if (playerForward == Vector2.zero)
		{
			return force;
		}
		playerForward.Normalize();
		Vector2 playetToThis = transform.position - flock.GetCharacterTrans().position;
		playetToThis.Normalize();
		float angle = Vector2.Angle(playerForward, playetToThis);
		if (angle <= AVOID_ANGLE)
		{
			force = playetToThis - playerForward;
		}
		return force.normalized * GIVE_WAY_FORCE_SCALE;
	}

	public Vector2 GetFleeingForce(List<Transform> enemyTransforms)
	{
		Vector2 force = Vector2.zero;
		force += Flee(enemyTransforms);
		return force;
	}


	private Vector2 Flee(List<Transform> enemyTransforms)
	{
		Vector2 force = Vector2.zero;
		foreach (Transform enemyTrans in enemyTransforms)
		{
			Vector2 difference = transform.position - enemyTrans.position;
			force += difference.normalized / difference.magnitude;
		}
		return force.normalized * FLEE_FORCE_SCALE;
	}

}
