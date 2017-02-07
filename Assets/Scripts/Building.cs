using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	private Blackboard blackboard = null;

	public int cost = 3;
	public bool purchased = false;
	private int coinsPaid = 0;
	private bool paymentCompleted = false;

	public GameObject coinslot;
	public GameObject coin;

	// Use this for initialization
	void Start () {
		if (!blackboard) {
			blackboard = GameObject.Find ("Blackboard").GetComponent<Blackboard> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GetPaid(){
		coinsPaid += 1;
		if (coinsPaid >= cost) {
			paymentCompleted = true;
			coinsPaid = 0;
			purchased = true;
			blackboard.CallNearestNPC (gameObject);
		}
	}

	void OnTriggerEnter (Collider col)
	{
		GameObject go = col.gameObject;

		if (go.tag == "Player") {
			Player playerScript = go.GetComponent<Player>(); 
			playerScript.canPay = true;
			playerScript.activeBuilding = gameObject;
			playerScript.buildingScript = gameObject.GetComponent<Building>();
		}
		if (go.tag == "NPC") {
			NPC npcScript = go.GetComponent<NPC>();
			if (npcScript.target == gameObject) {
				npcScript.busy = true;
				npcScript.StartWork ();
			}
		}
	}

	void OnTriggerExit (Collider col)
	{
		GameObject go = col.gameObject;

		if (go.tag == "Player") {
			Player playerScript = go.GetComponent<Player>(); 
			playerScript.canPay = false;
			playerScript.activeBuilding = null;
			playerScript.buildingScript = null;

			// failed payment
			if (!paymentCompleted && coinsPaid > 0){
				playerScript.ReturnCoins (coinsPaid);
				coinsPaid = 0;
			}

			if (paymentCompleted) {
				coinsPaid = 0;
				paymentCompleted = false;
			}
		}
	}

}
