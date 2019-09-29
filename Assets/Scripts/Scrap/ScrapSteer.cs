using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapSteer : MonoBehaviour
{
	public ScrapPool scrapPool;
	private float maxBound = 6;
	private float minBound = 4;

	private float seperateDistance = 1f;

	private float seperateForceScale = 0.1f;
	private float boundForceScale = 0.1f;

	public Vector3 GetForce()
	{
		Vector3 force = Vector3.zero;
		force += Seperate();
		force += Bound();
		return force;
	}

	private Vector3 Seperate()
	{
		Vector3 force = Vector3.zero;
		Debug.Log(scrapPool.scraps);
		foreach (GameObject scrap in scrapPool.scraps)
		{
			if (scrap == gameObject)
			{
				continue;
			}
			Vector3 difference = transform.position - scrap.transform.position;
			if (difference.magnitude <= seperateDistance)
			{
				force += difference.normalized / (1 / difference.magnitude);
			}
		}
		return force * seperateForceScale;
	}

	private Vector3 Bound()
	{
		Vector3 force = Vector3.zero;
		Vector3 position = transform.position;
		position.y = 0;
		force += position.magnitude > maxBound ? -position : Vector3.zero;
		force += position.magnitude < minBound ? position : Vector3.zero;
		return force.normalized * boundForceScale;
	}
}
