using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyConstants
{
	public const string CHARACTER_TAG = "Player";
	public const string ENEMY_TAG = "Enemy";

	public const float MAP_WIDTH_BOUND = 55f;
	public const float MAP_HEIGHT_BOUND = 30f;

	public const float SPAWNING_COOL_DOWN = 2f;

	public const int IDLE = 0;
	public const int FOLLOWING = 1;
	public const int FLEEING = 2;
	public const int SEEKING = 3;
	public const int DEAD = 4;
	public const int SAFE = 5;


	public const float FOLLOW_DISTANCE = 4f;
	public const float KEEP_DISTANCE = 1.5f;
	public const float SEPERATE_DISTANCE = 1f;
	public const float AVOID_ANGLE = 45f;


	public const float FOLLOW_FORCE_SCALE = 10f;
	public const float SEPERATE_FORCE_SCALE = 10f;
	public const float ALIGN_FORCE_SCALE = 1f;
	public const float GIVE_WAY_FORCE_SCALE = 10f;
	public const float FLEE_FORCE_SCALE = 10f;
}
