using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyConstants
{
	public const string CHARACTER_TAG = "Player";
	public const string ENEMY_TAG = "Enemy";

	public const float MAP_WIDTH_BOUND = 10f;
	public const float MAP_HEIGHT_BOUND = 10f;

	public const float SPAWNING_COOL_DOWN = 2f;

	public const float KEEP_DISTANCE = 1.5f;
	public const float SEPERATE_DISTANCE = 5f;

	public const float FOLLOW_FORCE_SCALE = 1f;
	public const float SEPERATE_FORCE_SCALE = 3f;
	public const float ALIGN_FORCE_SCALE = 1f;
	public const float COHESION_FORCE_SCALE = 2f;
	public const float BOUND_FORCE_SCALE = 2f;

	public const float MAX_SPEED = 5f;
}
