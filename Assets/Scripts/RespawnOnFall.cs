using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOnFall : MonoBehaviour
{
	public ScrapPool scrapPool;
	public Transform spawn;

    private Vector3 spawnPos;

    private void Start()
    {
        if (spawn == null)
        {
            spawnPos = transform.position;
        }
        else
        {
            spawnPos = spawn.position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
	{
		if (transform.position.y < -50)
		{
            Respawn();
		}
	}

    private void Respawn()
    {
        if(scrapPool != null)
        {
            transform.SetParent(scrapPool.transform);
            GetComponent<ScrapInteraction>().SetState(ScrapConstants.ScrapState.initial);
        }
        else if (spawnPos != null)
        {
            transform.position = spawnPos;
            GetComponent<ScrapInteraction>().SetState(ScrapConstants.ScrapState.initial);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
