using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
	public List<Transform> spawningSpots;
	public GameObject survivorPrefab;
	private List<GameObject> survivors = new List<GameObject>();
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
		Transform spot = spawningSpots[Random.Range(0, spawningSpots.Count)];
		foreach (GameObject survivor in survivors)
		{
			if (!survivor.activeSelf)
			{
				survivor.transform.SetParent(spot);
				survivor.transform.position = spot.position;
				survivor.SetActive(true);
				survivor.GetComponent<Rigidbody2D>().AddForce(GetRandomForce());
				return;
			}
		}
		GameObject newEnemy = Instantiate(survivorPrefab, spot.position, spot.rotation, transform);
		newEnemy.GetComponent<Rigidbody2D>().AddForce(GetRandomForce());
		survivors.Add(newEnemy);
	}

	private Vector2 GetRandomForce()
	{
		return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 10f;
	}
}
