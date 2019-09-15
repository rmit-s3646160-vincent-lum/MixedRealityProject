using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapPool : MonoBehaviour
{
	public List<Transform> spawningSpots;
	public GameObject enemyPrefab;
	public int maxNum;
	private List<GameObject> enemies = new List<GameObject>();
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
		if (enemies.Count >= maxNum)
		{
			return;
		}
		Transform spot = spawningSpots[Random.Range(0, spawningSpots.Count)];
		foreach (GameObject survivor in enemies)
		{
			if (!survivor.activeSelf)
			{
				survivor.transform.SetParent(spot);
				survivor.transform.position = spot.position;
				survivor.SetActive(true);
				return;
			}
		}
		GameObject newEnemy = Instantiate(enemyPrefab, spot.position, spot.rotation, transform);
		enemies.Add(newEnemy);
	}

	private Vector2 GetRandomForce()
	{
		return new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * 10f;
	}
}
