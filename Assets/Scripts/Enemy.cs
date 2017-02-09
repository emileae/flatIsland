using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

	private Blackboard blackboard;

	private NavMeshAgent agent;
	public bool goToDestination = false;
	public GameObject target = null;

	// HP / damage
	public int hp = 10;

	// list of buildings to destroy
	public List<Transform> buildings;

	// Enemy types
	public bool buildingBreaker = false;
	public bool goldEater = false;
	public bool npcEater = false;
	public bool npcStealer = false; // maybe an enemy that leads NPCs off a cliff / into the sea

	public bool attacking = false;
	public GameObject building;
	public Building buildingScript;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		if (!blackboard) {
			blackboard = GameObject.Find ("Blackboard").GetComponent<Blackboard> ();
		}
		buildings = blackboard.buildings;

		GoToNearestBuilding();

	}
	
	// Update is called once per frame
	void Update () {
		if (goToDestination && target != null) {
			agent.SetDestination (target.transform.position);
		}
	}

	IEnumerator WatchForNewTargets(){
		yield return new WaitForSeconds(10.0f);
		GoToNearestBuilding ();
	}

	void GoToNearestBuilding ()
	{
		bool gotInitialIndex = false;
		float minDistance = 0;
		int minIndex = 0;
		for (int i = 0; i < buildings.Count; i++) {
			// can also filter the type of buildings here... only based on built buildings for now
			if (blackboard.buildingScripts [i].built) {
				float distance = (transform.position - buildings [i].position).sqrMagnitude;
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

		Debug.Log ("gotInitialIndex " + gotInitialIndex);
		Debug.Log ("minIndex " + minIndex);

		if (gotInitialIndex) {
			target = buildings [minIndex].gameObject;
			goToDestination = true;
		} else {
			StartCoroutine(WatchForNewTargets());
		}

	}

	public void Attack ()
	{
		if (buildingBreaker && buildingScript != null) {
//			BuildingParameters parameters = new BuildingParameters (buildingScript.level, buildingScript);
			Debug.Log ("Attack building");
			if (buildingScript.built) {
				DecrementHP ();
			}
		}
	}

	void DecrementHP ()
	{
		StartCoroutine (RemoveHP ());
	}

	IEnumerator RemoveHP ()
	{
		yield return new WaitForSeconds (1.0f);
		buildingScript.hp -= 1;
		if (buildingScript.hp > 0) {
			DecrementHP ();
		} else {
			Debug.Log("DESTROYED BUILDING.... MOVE TO NEXT TARGET");
			buildingScript.built = false;
//			buildings.Remove(building.transform);
			GoToNearestBuilding();
		}
	}

}
