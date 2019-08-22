
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyConstants;

public class EnemyLeadDetection : MonoBehaviour
{
	private EnemyManager EnemyManager;

	// Start is called before the first frame update
	void Start()
	{
		EnemyManager = GetComponentInParent<EnemyManager>();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
