using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScrapConstants;

public class ScrapSpawner : MonoBehaviour
{
	public ScrapPool scrapPool;
	private float timeElapsed;
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		timeElapsed += Time.deltaTime;
		if (timeElapsed >= SPAWNING_COOL_DOWN)
		{
			scrapPool.SpawnScrap();
			timeElapsed = 0;
		}
	}
}
