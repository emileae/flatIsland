using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blackboard : MonoBehaviour {

	public List<Transform> npcs = new List<Transform> ();
	public List<NPC> npcScripts = new List<NPC> ();

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
	
	}

	public void CallNearestNPC(GameObject target){
		bool gotInitialDistance = false;
		float minDistance = 0;
		int minIndex = 0;
		for (int i = 0; i < npcScripts.Count; i++) {
			Debug.Log ("NPC " + i + " busy: " + npcScripts [i].busy);
			if (!npcScripts [i].busy) {
				float distance = (npcs [i].position - target.transform.position).sqrMagnitude;
				if (!gotInitialDistance) {
					minDistance = distance;
					minIndex = i;
					gotInitialDistance = true;
				}

				if (distance < minDistance) {
					minDistance = distance;
					minIndex = i;
				}
			}
		}

		// now send NPC to target
		NPC closestNPC = npcScripts[minIndex];
		closestNPC.target = target;
		closestNPC.goToDestination = true;

	}
}
