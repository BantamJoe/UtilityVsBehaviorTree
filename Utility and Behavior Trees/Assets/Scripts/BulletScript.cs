using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour
{
	public string tagToHurt = "Fill me in";
	public static int DAMAGE = 10;
	public float moveSpeed = 6.0f;

	Transform myTrans;

	// Use this for initialization
	void Start () 
	{
		myTrans = gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Translate(myTrans.forward * moveSpeed * Time.deltaTime, Space.World);
	}

	void OnTriggerEnter(Collider c)
	{
		if(c.tag == tagToHurt)
		{
			EnemyScript es;
			es = c.gameObject.GetComponent<EnemyScript>();
			es.health -= DAMAGE;
			if(es.health <= 0)
				Destroy(c.gameObject);

			Destroy(gameObject);
		}

		if(c.tag == "Wall")
		{
			Destroy(gameObject);
		}
	}
}
