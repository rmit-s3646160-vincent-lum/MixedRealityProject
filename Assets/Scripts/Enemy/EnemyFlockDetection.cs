using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyConstants;

public class EnemyFlockDetection : MonoBehaviour
{
	private List<Transform> flockList = new List<Transform>();
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(ENEMY_TAG))
		{
			if (!flockList.Contains(other.transform))
			{
				flockList.Add(other.transform);
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{

		if (other.CompareTag(ENEMY_TAG))
		{
			if (!flockList.Contains(other.transform))
			{
				flockList.Add(other.transform);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag(ENEMY_TAG))
		{
			if (flockList.Contains(other.transform))
			{
				flockList.Remove(other.transform);
			}
		}
	}

	public List<Transform> GetFlockList()
	{
		return flockList;
	}
}
