using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScrapManager : MonoBehaviour
{
	private ScrapMovement movement;
	private ScrapSteer steer;
	private ScrapFlockDetection enemyFlockDetection;


	private void Awake()
	{
		movement = GetComponent<ScrapMovement>();
		steer = GetComponent<ScrapSteer>();
		enemyFlockDetection = GetComponentInChildren<ScrapFlockDetection>();
	}

	private void FixedUpdate()
	{
		movement.ApplyForce(steer.GetFlockForce(enemyFlockDetection.GetFlockList()));
	}
}
