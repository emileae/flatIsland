using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	private Blackboard blackboard = null;

	// building basics
	private Building thisScript;
//	public int buildCost = 5;
	public bool built = false;
//	public float buildTime = 15.0f;
	public bool occupied = false;// used to prevent more than one NPC building the building / crafting an item
	public GameObject npcTarget = null;
	public int hp = 0;

	// Base
	public int storedCoins = 0;

	// item basics
//	public int cost = 3;
//	public float makeTime = 5.0f;

	public bool purchased = false;
	private int coinsPaid = 0;
	private bool paymentCompleted = false;

	public GameObject coinslot;
	public GameObject coin;

	// building categories
	public bool craftItem = false;
	public bool earnCoins = false;
	public bool upgrade = false;

	// Building types & Levels
	public int level = 0;
	public int maxLevel = 1;
	public bool isBase = false;
	public bool fishingSpot = false;
	public bool garden = false;

	// Specific scripts for each building type
//	public FishingSpot fishingSpotScript = null;

	// Use this for initialization
	void Start ()
	{
		if (!blackboard) {
			blackboard = GameObject.Find ("Blackboard").GetComponent<Blackboard> ();
		}
		if (!npcTarget) {
			foreach (Transform child in transform) {
				if (child.CompareTag ("NPCTarget")) {
					npcTarget = child.gameObject;
				}
			}

			// if no match was found
			if (npcTarget == null) {
				npcTarget = gameObject;
			}
		}

		if (isBase) {
			
		}

		thisScript = gameObject.GetComponent<Building>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GetPaid ()
	{
		BuildingParameters parameters = new BuildingParameters (level, thisScript);
		coinsPaid += 1;
		if (!built) {
			if (coinsPaid >= parameters.buildCost) {
				paymentCompleted = true;
				coinsPaid = 0;
//				purchased = true;
				blackboard.CallNearestNPC (npcTarget);
				occupied = true;
			}
		} else {
			if (coinsPaid >= parameters.cost) {
				paymentCompleted = true;
				coinsPaid = 0;
//				purchased = true;
				blackboard.CallNearestNPC (npcTarget);
				occupied = true;// the building becomes occupied as soon as NPC is called, not when NPC arrives... that opens up time for double payments of Player
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		GameObject go = col.gameObject;

		if (go.tag == "Player") {
			Player playerScript = go.GetComponent<Player> (); 
			playerScript.canPay = true;
			if (isBase) {
				playerScript.canCollect = true;
			}
			playerScript.activeBuilding = gameObject;
			playerScript.buildingScript = gameObject.GetComponent<Building> ();

			if (!playerScript.buildingScript.occupied) {
				blackboard.ShowCost(playerScript.buildingScript);
			}
		}
		if (go.tag == "NPC") {
			NPC npcScript = go.GetComponent<NPC> ();
			Debug.Log("NPC arrived at a building target? " + (npcScript.target == npcTarget));
			if (npcScript.target == npcTarget) {
				Debug.Log ("Arrived at target destination ---- NPC");
//				npcScript.StopMoving();
				npcScript.building = gameObject;
				npcScript.buildingScript = gameObject.GetComponent<Building> ();
				npcScript.StartWork ();
			}
		}
		if (go.tag == "Enemy") {
			Enemy enemyScript = go.GetComponent<Enemy> ();
			if (enemyScript.target == npcTarget) {
				Debug.Log ("Arrived at target destination ---- Enemy");
				enemyScript.building = gameObject;
				enemyScript.buildingScript = gameObject.GetComponent<Building> ();
				enemyScript.Attack();
			}
		}
	}

	void OnTriggerExit (Collider col)
	{
		GameObject go = col.gameObject;

		if (go.tag == "Player") {
			Player playerScript = go.GetComponent<Player> (); 
			playerScript.canPay = false;
			if (isBase) {
				playerScript.canCollect = false;
			}
			playerScript.activeBuilding = null;
			playerScript.buildingScript = null;

			// failed payment
			if (!paymentCompleted && coinsPaid > 0) {
				playerScript.ReturnCoins (coinsPaid);
				coinsPaid = 0;
			}

			if (paymentCompleted) {
				coinsPaid = 0;
				paymentCompleted = false;
			}
		}

		if (go.tag == "NPC") {
			NPC npcScript = go.GetComponent<NPC> ();
			npcScript.building = null;
			npcScript.buildingScript = null;
			if (npcScript.target == npcTarget) {
				occupied = false;// if NPC leaves building its then available for another NPC
				// TODO
				// potential bug here, where NPC may still be amrked as busy even though task isn't complete
				// Coroutine may still be running even if NPX isn't at station...
				// consider clearing work if NPC moves out of station
				// if NPC is not busy then exiting clears the buildscript
				// otherwise NPC is still busy and may have been pushed out of work area
				if (npcScript.busy) {
					Debug.Log ("StopWork Coroutine");
					npcScript.StopWork ();
					// NPC no longer pursues the task if pushed out of work area...
					npcScript.goToDestination = false;
					npcScript.target = null;
					npcScript.busy = false;
				}
			}
		}

	}

	public void DepositCoin(){
		storedCoins += 1;
	}

	public void Upgrade ()
	{
		if (level < maxLevel){
			level += 1;
			if (isBase) {
				blackboard.UpgradeFishingSpots();
				blackboard.UpgradeGardens();
			}
			ChangeMesh(level);
		}
	}

	public void ChangeMesh (int level)
	{
		if (level == 0) {
			GameObject unBuiltGeometry = transform.Find("UnBuilt").gameObject;
			unBuiltGeometry.SetActive(false);
		}
		GameObject visibleGeometry = transform.Find("Built" + level).gameObject;
		visibleGeometry.SetActive(true);
	}

}
