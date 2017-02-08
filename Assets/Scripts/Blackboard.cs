﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blackboard : MonoBehaviour {

	public List<Transform> npcs = new List<Transform> ();
	public List<NPC> npcScripts = new List<NPC> ();

	private bool callingQueuedTargets = false;
	public List<GameObject> waitingNPCTargets = new List<GameObject> ();

	// Use this for initialization
	void Start () {
		GameObject npcContainer = GameObject.Find ("NPCs");
		foreach (Transform child in npcContainer.transform)
		{
			npcs.Add (child);
			npcScripts.Add (child.GetComponent<NPC> ());
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (waitingNPCTargets.Count > 0 && !callingQueuedTargets) {
			callingQueuedTargets = true;
			Debug.Log("Call Queued targets.... start");
			StartCoroutine (CallQueuedTargets());
		}
	}

	public void CallNearestNPC (GameObject target)
	{

		// TODO
		// queue NPC calls when there are no available NPCs.....

		bool gotInitialIndex = false;
		float minDistance = 0;
		int minIndex = 0;
		for (int i = 0; i < npcScripts.Count; i++) {
			if (!npcScripts [i].busy) {
				float distance = (npcs [i].position - target.transform.position).sqrMagnitude;
				if (!gotInitialIndex) {
					minDistance = distance;
					minIndex = i;
					gotInitialIndex = true;
				}

				if (distance < minDistance) {
					minDistance = distance;
					minIndex = i;
				}
			}
		}

		// no NPCs found... place in queue
		if (!gotInitialIndex) {
			Debug.Log ("NO NPCSSSSSZZZZZ");
			if (!waitingNPCTargets.Contains (target)) {
				waitingNPCTargets.Add (target);
			}
		} else {
			// now send NPC to target
			NPC closestNPC = npcScripts [minIndex];
			closestNPC.target = target;
			closestNPC.goToDestination = true;

			// remove from list of queued targets if its in the list
			if (waitingNPCTargets.Contains (target)) {
				waitingNPCTargets.Remove (target);
			}
		}

	}

	IEnumerator CallQueuedTargets(){
		Debug.Log("Call Queued targets.... wait");
		yield return new WaitForSeconds (5.0f);
		Debug.Log ("Called a Queued target......" + waitingNPCTargets [0]);
		CallNearestNPC (waitingNPCTargets [0]);
		callingQueuedTargets = false;
	}
}
