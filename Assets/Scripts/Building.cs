using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	public int cost = 3;

	public GameObject coinslot;
	public GameObject coin;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider col)
	{
		GameObject go = col.gameObject;

		if (go.tag == "Player") {
			Player playerScript = go.GetComponent<Player>(); 
			playerScript.canPay = true;
		}
	}

	void OnTriggerExit (Collider col)
	{
		GameObject go = col.gameObject;

		if (go.tag == "Player") {
			Player playerScript = go.GetComponent<Player>(); 
			playerScript.canPay = false;
		}
	}

}
