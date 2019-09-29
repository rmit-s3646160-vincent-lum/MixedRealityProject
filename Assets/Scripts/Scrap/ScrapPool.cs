using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapPool : MonoBehaviour
{
	public List<Transform> spawningSpots;
	public List<GameObject> scrapPrefabs;
	public int maxNum;
	public List<GameObject> scraps = new List<GameObject>();
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void PullOneEnemy()
	{
		if (scraps.Count >= maxNum)
		{
			return;
		}
		Transform spot = spawningSpots[Random.Range(0, spawningSpots.Count)];
		GameObject newScrap = Instantiate(scrapPrefabs[Random.Range(0, scrapPrefabs.Count)], spot.position, spot.rotation, transform);
		newScrap.GetComponent<ScrapSteer>().scrapPool = this;
		newScrap.GetComponent<RespawnOnFall>().scrapPool = this;
		scraps.Add(newScrap);
	}

	private Vector2 GetRandomForce()
	{
		return new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * 10f;
	}
}
