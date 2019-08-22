using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static EnemyConstants;

public class EnemyManager : MonoBehaviour
{
	private EnemyMovement movement;
	private EnemySteer steer;


	private void Awake()
	{
		movement = GetComponent<EnemyMovement>();
		steer = GetComponent<EnemySteer>();
	}

	private void FixedUpdate()
	{
        movement.ApplyForce(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized);
	}
}
