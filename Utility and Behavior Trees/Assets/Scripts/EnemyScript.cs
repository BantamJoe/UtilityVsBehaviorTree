using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour
{
	public int baseHealth = 100;
	public int health = 100;
	public float moveSpeed = 5.0f;
	public float threatRange = 15.0f;
	public float potTurnAmount = 90.0f;
	public float minWanderTime = 2.5f;
	public float curWanderTime = 2.5f;
	public float shotsPerSecond = 1.0f;
	public float shootTime = 1.0f;
	public float curShootTime = 1.0f;
	public int healPerSecond = 10;
	public float curHealTime = 1.0f;

	public bool wandering = false;

	public string enemyTag = "fill me in";

	public GameObject[] friends;
	public GameObject[] enemies;
	public GameObject nearestEnemy;
	public GameObject bullet;
	public GameObject healSpot;
	public GameObject healItem;

	public bool isBTAI;
	public bool isUAI;

	void Awake()
	{
		health = baseHealth;
		shootTime = 1.0f / shotsPerSecond;
		curShootTime = shootTime;
		enemies = GameObject.FindGameObjectsWithTag(enemyTag);
		friends = GameObject.FindGameObjectsWithTag(gameObject.tag);

		if(isBTAI)
			healSpot = GameObject.FindGameObjectWithTag("BTHeal");
		if(isUAI)
			healSpot = GameObject.FindGameObjectWithTag("UHeal");
	}
}
