using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyConstants;

public class EnemyFleeDetection : MonoBehaviour
{
	private EnemyManager survivorManager;

	// Start is called before the first frame update
	void Start()
	{
		survivorManager = GetComponentInParent<EnemyManager>();
	}

	// Update is called once per frame
	void Update()
	{

	}
}

