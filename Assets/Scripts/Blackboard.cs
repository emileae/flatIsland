using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blackboard : MonoBehaviour {

	// buildings
	public List<Transform> buildings = new List<Transform> ();
	public List<Building> buildingScripts = new List<Building> ();
	public Building baseScript = null;

	// NPCs
	public List<Transform> npcs = new List<Transform> ();
	public List<NPC> npcScripts = new List<NPC> ();

	public bool callingQueuedTargets = false;
	public List<GameObject> waitingNPCTargets = new List<GameObject> ();

	// Enemies
	public List<Transform> enemies = new List<Transform> ();
	public List<Enemy> enemyScripts = new List<Enemy> ();

	// Use this for initialization
	void Awake ()
	{

		// populate all NPCs
		GameObject npcContainer = GameObject.Find ("NPCs");
		foreach (Transform child in npcContainer.transform) {
			npcs.Add (child);
			npcScripts.Add (child.GetComponent<NPC> ());
		}

		// populate all buildings
		GameObject buildingContainer = GameObject.Find ("Buildings");
		foreach (Transform child in buildingContainer.transform) {
			buildings.Add (child);
			Building currentBuildingScript = child.GetComponent<Building> ();
			buildingScripts.Add (currentBuildingScript);
			if (currentBuildingScript.isBase) {
				baseScript = child.gameObject.GetComponent<Building>();
			}
		}

		// populate all initial enemies
		GameObject enemyContainer = GameObject.Find ("Enemies");
		foreach (Transform child in enemyContainer.transform) {
			enemies.Add (child);
			Enemy currentEnemyScript = child.GetComponent<Enemy> ();
			enemyScripts.Add (currentEnemyScript);
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (waitingNPCTargets.Count > 0 && !callingQueuedTargets) {
			callingQueuedTargets = true;
//			Debug.Log("Call Queued targets.... start");
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
//			Debug.Log ("NO NPCSSSSSZZZZZ");
			if (!waitingNPCTargets.Contains (target)) {
				waitingNPCTargets.Add (target);
			}
		} else {
			// now send NPC to target
			NPC closestNPC = npcScripts [minIndex];
			closestNPC.GoToTarget (target);
//			closestNPC.target = target;
//			closestNPC.goToDestination = true;

			// remove from list of queued targets if its in the list
			if (waitingNPCTargets.Contains (target)) {
				waitingNPCTargets.Remove (target);
			}
		}

	}

	IEnumerator CallQueuedTargets(){
//		Debug.Log("Call Queued targets.... wait");
		yield return new WaitForSeconds (5.0f);
//		Debug.Log ("Called a Queued target......" + waitingNPCTargets [0]);
		CallNearestNPC (waitingNPCTargets [0]);
		callingQueuedTargets = false;
	}

	public void CallNPCsToPlayer(GameObject player){
		NavMeshHit hit;
		if (NavMesh.SamplePosition(player.transform.position, out hit, 100.0f, NavMesh.AllAreas)) {
//			result = hit.position;
			for (int i = 0; i < npcScripts.Count; i++) {
				npcScripts[i].playerDestination = hit.position;
				npcScripts [i].goToPlayer = true;
			}
		}
//		for (int i = 0; i < npcScripts.Count; i++) {
//			npcScripts[i].target = player;
//			npcScripts [i].goToDestination = true;
//		}
	}

}


// Animation states
// 1 = building
// 2 = fishing
// 3 = gardening
// 4 = payingcoins
// 5 = workshop work

public class BuildingParameters {
	public float workTime = 0.0f;
	public int coinsEarned = 0;
	public int animationState = 0;
	public int hp = 1;

	public BuildingParameters (int level, Building script)
	{
		if (script.built) {
			if (script.fishingSpot) {
				animationState = 2;
				switch (level) {
				case 0:
					workTime = 5.0f;
					coinsEarned = 2;
					hp = 2;
					break;
				case 1:
					workTime = 8.0f;
					coinsEarned = 4;
					hp = 3;
					break;
				default:
					break;
				}
			} else if (script.garden) {
				animationState = 3;
				switch (level) {
				case 0:
					workTime = 5.0f;
					coinsEarned = 3;
					hp = 3;
					break;
				case 1:
					workTime = 8.0f;
					coinsEarned = 5;
					hp = 6;
					break;
				default:
					break;
				}
			} else if (script.isBase) {
				animationState = 5;
				switch (level) {
				case 0:
					workTime = 5.0f;
					break;
				case 1:
					workTime = 8.0f;
					break;
				default:
					break;
				}
			}
		} else {
			animationState = 1;
			if (script.fishingSpot) {
				workTime = 5.0f;
			} else if (script.garden) {
				workTime = 10.0f;
			} else if (script.isBase) {
				workTime = 10.0f;
			}
		}
    }
}
