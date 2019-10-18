using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScrapConstants
{
	public const string CHARACTER_TAG = "Player";
	public const string SCRAP_TAG = "Scrap";
	public const int PLATFORM_LAYER = 9;

	public const float SPAWNING_COOL_DOWN = 2f;


	public enum ScrapState
	{
		initial,
		manipulating,
		notPlaced,
		beingPlaced
	}

}
