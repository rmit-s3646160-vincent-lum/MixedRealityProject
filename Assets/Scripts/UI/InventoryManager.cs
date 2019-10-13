using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
	public List<Sprite> itemSprites;
	public List<GameObject> itemPrefabs;
	public SpriteRenderer[] items = new SpriteRenderer[4];
	public Transform spawnPos;
	public Transform scroller;
	private int curPage;
	private int maxPage;
	private float scrollerMaxPositionY;

	// Start is called before the first frame update
	void Start()
	{
		maxPage = itemSprites.Count / 4;
		scrollerMaxPositionY = scroller.localPosition.y;
		UpdatePage();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void SpawnSelectedItem(int index)
	{
		if (index < 0 || index >= 4)
		{
			return;
		}
		index += curPage * 4;
		Instantiate(itemPrefabs[index], spawnPos.position, spawnPos.rotation, spawnPos);
	}

	public void PageUp()
	{
		if (curPage > 0)
		{
			curPage--;
			UpdatePage();
		}
	}

	public void PageDown()
	{
		if (curPage < maxPage)
		{
			curPage++;
			UpdatePage();
		}
	}

	private void UpdatePage()
	{
		scroller.localPosition = Vector3.up * scrollerMaxPositionY / maxPage * curPage;
		int index;
		for (int i = 0; i < 4; i++)
		{
			index = curPage * 4 + i;
			if (index <= itemSprites.Count - 1)
			{
				items[i].sprite = itemSprites[index];
				items[i].gameObject.SetActive(true);
			}
			else
			{
				items[i].gameObject.SetActive(false);
			}
		}
	}
}
