using UnityEngine;
using System.Collections;

public class ManagerScript : MonoBehaviour
{
	public GUIText btText;
	public GUIText uText;

	public KeyCode restartKey;

	GameObject[] bts;
	GameObject[] us;

	int btAlive = 0;
	int uAlive = 0;

	// Use this for initialization
	void Start () 
	{
		bts = GameObject.FindGameObjectsWithTag("BTAI");
		us = GameObject.FindGameObjectsWithTag("UAI");

		uAlive = us.Length;
		btAlive = bts.Length;
	}
	
	// Update is called once per frame
	void Update () 
	{
		btAlive = 0;

		for(int i = 0; i < bts.Length; i++)
		{
			if(bts[i] != null)
				btAlive ++;
		}


		uAlive = 0;
		
		for(int i = 0; i < us.Length; i++)
		{
			if(us[i] != null)
				uAlive ++;
		}

		btText.text = "BT Alive : " + btAlive;
		uText.text = "Utility Alive : " + uAlive;

		if(Input.GetKeyDown(restartKey))
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
