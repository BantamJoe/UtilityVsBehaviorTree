using UnityEngine;
using System.Collections;

public class AIBehaviorTreeScript : MonoBehaviour
{
	EnemyScript enemyScript;
	Transform myTrans;
	GameObject self;
	Rigidbody rBody;

	float enemyDistance = 9000.0f;

	void Start()
	{
		self = gameObject;
		enemyScript = self.GetComponent<EnemyScript>();
		myTrans = self.transform;
		rBody = self.rigidbody;
	}

	void Update()
	{
		BehaviorTree();
	}


	void BehaviorTree()
	{
		if(NearEnemy())
			Fight();
		else if(NeedHealth())
			Heal ();
		else
			Wander();
	}

	#region Fight
	bool NearEnemy()
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

		if(enemyDistance <= enemyScript.threatRange)
			return true;

		return false;
	}

	void FaceNearestEnemy()
	{
		rBody.velocity = Vector3.zero;
		myTrans.LookAt(enemyScript.nearestEnemy.transform);
	}

	void Fight()
	{
		FaceNearestEnemy();

		enemyScript.curShootTime -=Time.deltaTime;
		if(enemyScript.curShootTime <= 0.0f)
		{
			Instantiate(enemyScript.bullet, myTrans.position, myTrans.rotation);
			enemyScript.curShootTime = enemyScript.shootTime;
		}
	}
	#endregion

	#region Heal
	bool NeedHealth()
	{
		if(enemyScript.health < 100)
			return true;

		return false;
	}

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
	bool Wander()
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

		return true;
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
