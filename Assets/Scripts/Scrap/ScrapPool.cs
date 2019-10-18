using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapPool : MonoBehaviour
{
	public int maxNum = 18;
    public List<GameObject> scrapPrefabs;
    public List<GameObject> scraps;

    GridObjectCollection grid;

	// Start is called before the first frame update
	void Start()
	{
        grid = GetComponent<GridObjectCollection>();
        scraps = new List<GameObject>(maxNum);
        SpawnScrap(maxNum);
	}

    public void SpawnScrap()
    {
        SpawnScrap(1);
    }

	public void SpawnScrap(int num)
	{
        for(int i = 0; i < num; i++)
        {
            if (scraps.Count >= maxNum)
                break;

            GameObject newScrap = Instantiate(scrapPrefabs[Random.Range(0, scrapPrefabs.Count)], transform);
            newScrap.transform.Rotate(GetRandomEulers()); // Set random rotation
            newScrap.GetComponent<Rotator>().eulerAngle = GetRandomForce(Random.Range(0.2f, 0.5f)); // Set random angular velocity
            newScrap.GetComponent<RespawnOnFall>().scrapPool = this;
            scraps.Add(newScrap);
        }
        grid.UpdateCollection();
    }

    private Vector2 GetRandomForce(float strength)
	{
		return new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized * strength;
	}

    private Vector3 GetRandomEulers()
    {
        return new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }
}
