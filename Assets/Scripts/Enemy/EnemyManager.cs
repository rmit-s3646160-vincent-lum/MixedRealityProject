using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyManager : MonoBehaviour
{
	private EnemyMovement movement;
	private EnemySteer steer;
	private EnemyFlockDetection enemyFlockDetection;


	private void Awake()
	{
		movement = GetComponent<EnemyMovement>();
		steer = GetComponent<EnemySteer>();
		enemyFlockDetection = GetComponentInChildren<EnemyFlockDetection>();
	}

	private void FixedUpdate()
	{
		movement.ApplyForce(steer.GetFlockForce(enemyFlockDetection.GetFlockList()));
	}
}
