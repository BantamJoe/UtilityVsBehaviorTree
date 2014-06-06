using UnityEngine;
using System.Collections;

public class AIUtilityScript : MonoBehaviour
{
	EnemyScript enemyScript;
	Transform myTrans;
	GameObject self;
	Rigidbody rBody;
	
	float enemyDistance = 9000.0f;

	float calcUtilityTime = .3f;
	float curCalUtilityTime = .3f;

	float threatUtil = 0.0f;
	float panicUtil = 0.0f;

	public bool fight = false;
	public bool wander = false;
	public bool heal = false;

	int enemiesAlive = 0;
	int friendsAlive = 0;

	float randomBehaviorRange = .1f;
	
	void Start()
	{
		self = gameObject;
		enemyScript = self.GetComponent<EnemyScript>();
		myTrans = self.transform;
		rBody = self.rigidbody;
		CalcUtil();
	}
	
	// Update is called once per frame
	void Update ()
	{
		curCalUtilityTime -= Time.deltaTime;
		if(curCalUtilityTime <= 0.0f)
		{
			curCalUtilityTime += calcUtilityTime;
			CalcUtil();
		}
		if(fight)
			Fight ();
		if(heal)
			Heal();
		if(wander)
			Wander();
	}
	#region calculations
	void CalcUtil()
	{
		fight = wander = heal = false;

		CalcThreat();
		CalcPanic();

		if(threatUtil >= panicUtil && threatUtil > 0)
		{
			if(threatUtil - panicUtil < randomBehaviorRange && panicUtil > 0)
			{
				int r = Random.Range(0,2);
				if(r == 0)
					fight = true;
				else
					heal = true;
			}
			else 
			{
				fight = true;
			}
		}
		else if(panicUtil > threatUtil)
		{
			if(panicUtil - threatUtil < randomBehaviorRange && threatUtil > 0)
			{
				int r = Random.Range(0,2);
				if(r == 0)
					heal = true;
				else
					fight = true;
			}
			else
				heal = true;
		}
		else
		{
			wander = true;
		}

		/*
		if(panicUtil > threatUtil && panicUtil > 0 && enemyScript.health < enemyScript.baseHealth)
		{
			heal = true;
			wander = false;
			fight = false;
		}

		if(threatUtil > 0 && !heal)
		{
			fight = true;
			wander = false;
		}

		if(!fight && !heal)
		{
			wander = true;
		}
		*/

		/*if(fight && threatUtil < .0f)//for later when he is more advanced may not consider things threats
		{
			fight = false;
			wander = true;
			heal = false;
		}
		if(heal && panicUtil < .0f)//later if he doesnt see needing to heal as important
		{
			wander = true;
			heal = false;
			fight = false;
		}*/
	}

	void CalcThreat()
	{
		threatUtil = 0.0f;

		//higher the closer an enemy is 
		GetNearestEnemy();
		//if(Vector3.Distance(enemyScript.nearestEnemy.transform.position, myTrans.position) <= enemyScript.threatRange)
		//	threatUtil += (enemyScript.threatRange - enemyDistance) / enemyScript.threatRange; //max value is 1

		threatUtil += (float)(enemyScript.baseHealth -  enemyScript.health) / (float)(enemyScript.baseHealth);

		enemiesAlive = 0;

		GetEnemiesAlive();

		threatUtil += (float)enemiesAlive / (float)enemyScript.enemies.Length;

		if(enemyDistance > enemyScript.threatRange)
			threatUtil = 0.0f;


		//percent of enemies to friends

		//normailze panic util- add up all max values of the calculations and divide the curent by the max

		//threatUtil /= ((float)(enemyScript.baseHealth / (float)enemyScript.baseHealth)) + ((float)enemiesAlive /  (float)(friendsAlive));
		threatUtil /= 2.0f;

	}

	void CalcPanic()
	{
		//higher the more enemies that are alive
		//higher the more health that is missing
		//higher the greater the distance from heal spot
		panicUtil = 0.0f;

		friendsAlive = 0;
		GetFriendsAlive();
		GetEnemiesAlive();

		//the percent of enemies team alive compared to the percent of my team alive
		panicUtil += (float)enemiesAlive / (float)enemyScript.enemies.Length;
		panicUtil -= (float)(friendsAlive -1) / (float)(enemyScript.friends.Length -1);
		//panicUtil += ((float)(enemyScript.friends.Length-1) - (float)(friendsAlive-1))/ (float)(enemyScript.friends.Length-1);
		panicUtil += ((float)(enemyScript.baseHealth - enemyScript.health) / (float)enemyScript.baseHealth);

		panicUtil /= 3;//total potentail is 2

		if(enemyScript.health == enemyScript.baseHealth)
			panicUtil = 0.0f;
	}

	void GetFriendsAlive()
	{
		for(int i = 0; i < enemyScript.friends.Length; i++)
		{
			if(enemyScript.friends[i] != null)
				friendsAlive ++;
		}
	}

	void GetEnemiesAlive()
	{
		for(int i = 0; i < enemyScript.enemies.Length; i++)
		{
			if(enemyScript.enemies[i] != null)
				enemiesAlive ++;
		}
	}
	#endregion

	#region Fight
	void GetNearestEnemy()
	{
		enemyDistance = 9000;
		enemyScript.nearestEnemy = enemyScript.enemies[0];
		for(int i = 0; i < enemyScript.enemies.Length; i++)
		{
			if(enemyScript.enemies[i] !=  null)
			{
				//Debug.Log (Vector3.Distance(myTrans.position, enemyScript.enemies[i].transform.position));
				if(enemyDistance > Vector3.Distance(myTrans.position, enemyScript.enemies[i].transform.position))
				{
					enemyDistance = Vector3.Distance(myTrans.position, enemyScript.enemies[i].transform.position);
					enemyScript.nearestEnemy = enemyScript.enemies[i];
				}
			}
		}
	}
	
	void FaceNearestEnemy()
	{
		rBody.velocity = Vector3.zero;
		myTrans.LookAt(enemyScript.nearestEnemy.transform);
	}
	
	void Fight()
	{
		if(enemyScript.nearestEnemy == null)
			CalcUtil();
		else
		{
			
			FaceNearestEnemy();
			
			enemyScript.curShootTime -=Time.deltaTime;
			if(enemyScript.curShootTime <= 0.0f)
			{
				Instantiate(enemyScript.bullet, myTrans.position, myTrans.rotation);
				enemyScript.curShootTime = enemyScript.shootTime;
			}
		}
	}
	#endregion

	#region Heal
	void Heal()
	{
		FaceHealPoint();
		if(NearHealPoint())
			Healing();
		else
			MoveForward();
	}
	
	void FaceHealPoint()
	{
		myTrans.LookAt(enemyScript.healSpot.transform.position);
	}
	
	bool NearHealPoint()
	{
		if(Vector3.Distance(myTrans.position, enemyScript.healSpot.transform.position) <= enemyScript.threatRange)
			return true;
		
		return false;
	}
	
	void Healing()
	{
		rBody.velocity = Vector3.zero;
		enemyScript.curHealTime -= Time.deltaTime;
		
		if(enemyScript.curHealTime <= 0.0)
		{
			Instantiate(enemyScript.healItem, myTrans.position + Vector3.up, enemyScript.healItem.transform.rotation);
			enemyScript.health += enemyScript.healPerSecond;
			enemyScript.curHealTime += 1.0f;
			if(enemyScript.health > enemyScript.baseHealth)
				enemyScript.health = enemyScript.baseHealth;
		}
	}
	
	#endregion
	
	#region Wander
	void Wander()
	{
		if(!enemyScript.wandering)
			enemyScript.wandering = true;
		
		enemyScript.curWanderTime -= Time.deltaTime;
		
		if(enemyScript.curWanderTime <= 0.0f)
		{
			RandomFacing();
			enemyScript.curWanderTime = enemyScript.minWanderTime;
		}
		
		MoveForward();
	}
	
	void RandomFacing()
	{
		float turnAmount = Random.Range(-enemyScript.potTurnAmount, enemyScript.potTurnAmount);
		myTrans.eulerAngles = new Vector3(myTrans.eulerAngles.x, myTrans.eulerAngles.y + turnAmount, myTrans.eulerAngles.z);
	}
	
	void MoveForward()
	{
		rBody.velocity = Vector3.zero;
		rBody.AddForce(myTrans.forward * enemyScript.moveSpeed, ForceMode.VelocityChange);
	}
	#endregion

	void OnCollisionEnter(Collision c)
	{
		if(c.gameObject.tag == "Wall")
		{
			myTrans.eulerAngles = new Vector3(myTrans.eulerAngles.x, myTrans.eulerAngles.y + 180, myTrans.eulerAngles.z);
		}
	}
}
