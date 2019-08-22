using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyConstants;

public class EnemyFlock : MonoBehaviour
{
	private List<EnemyManager> Enemys = new List<EnemyManager>();
	private Transform characterTrans;
	private Rigidbody2D characterRigidbody2D;

	// Start is called before the first frame update
	void Start()
	{
		var character = GameObject.FindWithTag(CHARACTER_TAG);
		characterTrans = character.transform;
		characterRigidbody2D = character.GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void AddEnemy(EnemyManager Enemy)
	{
		Enemys.Add(Enemy);
	}

	public void RemoveEnemy(EnemyManager Enemy)
	{
		Enemys.Remove(Enemy);
	}

	public List<EnemyManager> GetEnemys()
	{
		return Enemys;
	}

	public Transform GetCharacterTrans()
	{
		return characterTrans;
	}

	public Rigidbody2D GetCharacterRigidbody2D()
	{
		return characterRigidbody2D;
	}
}
