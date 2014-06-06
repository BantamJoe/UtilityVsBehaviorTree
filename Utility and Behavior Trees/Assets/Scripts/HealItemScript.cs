using UnityEngine;
using System.Collections;

public class HealItemScript : MonoBehaviour 
{
	public float lifeTime = 1.0f;

	
	// Update is called once per frame
	void Update () 
	{
		lifeTime -=Time.deltaTime;
		if(lifeTime <= 0)
			Destroy(gameObject);
	}
}
