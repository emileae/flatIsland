using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

public class NPC : MonoBehaviour {

	private NavMeshAgent agent;
	public bool goToDestination = false;
	public GameObject target = null;

	public bool busy = false;
	public GameObject building;
	public Building buildingScript;

	// different types of work state for each NPC
	private bool isBuilding = false;
	private bool isMaking = false;

	// working functions
	private IEnumerator work;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		work = Work ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (goToDestination && target != null) {
			busy = true;
			agent.SetDestination (target.transform.position);
		}

		// this is one option to call StartWork.... but might not always be able to get that close to the exact target... a large building with target as center for example
//		if (target != null) {
//			float sqrDistanceFromTarget = (transform.position - target.transform.position).sqrMagnitude;
//			if (sqrDistanceFromTarget < 0.2) {
//				Debug.Log ("Start work.....!>!>!>!>!>!>!>!>!");
//			}
//		}
	}

	public void GoToTarget(GameObject destination){
		if (target == destination) {
			Debug.Log ("Already there....");
			goToDestination = true;
			StartWork ();
		}else{
			target = destination;
			goToDestination = true;
		}

	}

	public void StartWork ()
	{
		Debug.Log("Should start work?");
		if (buildingScript) {
			Debug.Log("Should start work? --> Coroutine");
			work = Work ();
			StartCoroutine (work);
		}
	}

	public void StopWork ()
	{
		if (buildingScript) {
			StopCoroutine (work);
		}
	}

	IEnumerator Work ()
	{
		float workTime = 0.0f;
		Debug.Log ("Start work");
		if (buildingScript.built) {
			isMaking = true;
			workTime = buildingScript.makeTime;
			Debug.Log ("Show making animation");
		} else {
			isBuilding = true;
			workTime = buildingScript.buildTime;
			Debug.Log ("Show building animation");
		}

		yield return new WaitForSeconds (workTime);
		Debug.Log ("Finished work");

		// telling the building we're finished the work
		buildingScript.GetResult(gameObject.GetComponent<NPC>());
		// no longer need building script
//		building = null;
//		buildingScript = null;

		goToDestination = false;

		// only reset target once NPC has exited the building... trying to ensure that NPC still works even if trigger enter isn't called 
		// in the case of overlapping NPCs
//		target = null;
		busy = false;
	}
}
