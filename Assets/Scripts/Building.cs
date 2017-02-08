﻿using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	private Blackboard blackboard = null;

	// building basics
	public int buildCost = 5;
	public bool built = false;
	public float buildTime = 15.0f;

	// item basics
	public int cost = 3;
	public float makeTime = 5.0f;

	public bool purchased = false;
	private int coinsPaid = 0;
	private bool paymentCompleted = false;

	public GameObject coinslot;
	public GameObject coin;

	// Building types
	public bool fishingSpot;
	public bool rodShop;
//	public bool gunShop;
//	public bool hammerShop;

	// Use this for initialization
	void Start () {
		if (!blackboard) {
			blackboard = GameObject.Find ("Blackboard").GetComponent<Blackboard> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GetPaid ()
	{
		coinsPaid += 1;
		if (!built) {
			if (coinsPaid >= buildCost) {
				paymentCompleted = true;
				coinsPaid = 0;
				purchased = true;
				blackboard.CallNearestNPC (gameObject);
			}
		} else {
			if (coinsPaid >= cost) {
				paymentCompleted = true;
				coinsPaid = 0;
				purchased = true;
				blackboard.CallNearestNPC (gameObject);
			}
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
				npcScript.building = gameObject;
				npcScript.buildingScript = gameObject.GetComponent<Building>();
				npcScript.StartWork ();
			}
		}
	}

	void OnTriggerExit (Collider col)
	{
		GameObject go = col.gameObject;

		if (go.tag == "Player") {
			Player playerScript = go.GetComponent<Player> (); 
			playerScript.canPay = false;
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
			if (npcScript.target == gameObject) {
				// TODO
				// potential bug here, where NPC may still be amrked as busy even though task isn't complete
				// Coroutine may still be running even if NPX isn't at station...
				// consider clearing work if NPC moves out of station
				// if NPC is not busy then exiting clears the buildscript
				// otherwise NPC is still busy and may have been pushed out of work area
				if (!npcScript.busy) {
					npcScript.building = null;
					npcScript.buildingScript = null;
				} else {
					npcScript.StopWork();
					// NPC no longer pursues the task if pushed out of work area...
					npcScript.goToDestination = false;
					npcScript.target = null;
					npcScript.busy = false;
					npcScript.building = null;
					npcScript.buildingScript = null;
				}
			}
		}

	}

	public void GetResult (NPC npcScript)
	{

		// List each building type and its reward.
		// TODO
		// might want to move this into separate scripts??
		if (built) {
			if (rodShop) {
				Debug.Log ("made a rod");
			} else if (fishingSpot) {
				Debug.Log ("tell NPC to start fishing");
			}
		} else {
			if (rodShop) {
				Debug.Log ("built rodShop");
			} else if (fishingSpot) {
				Debug.Log ("built fishingSpot");
			}
		}

	}

}
