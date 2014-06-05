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
		else
			Wander();
	}


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

	void OnCollisionEnter(Collision c)
	{
		if(c.gameObject.tag == "Wall")
		{
			myTrans.eulerAngles = new Vector3(myTrans.eulerAngles.x, myTrans.eulerAngles.y + 180, myTrans.eulerAngles.z);
		}
	}
}
