using UnityEngine;
using System.Collections;

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
	void Update () {
		if (goToDestination && target != null) {
			busy = true;
			agent.SetDestination(target.transform.position);
		}
	}

	public void StartWork ()
	{
		if (buildingScript) {
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
		Debug.Log ("Start work");
		if (buildingScript.built) {
			isMaking = true;
			Debug.Log ("Show making animation");
		} else {
			isBuilding = true;
			Debug.Log ("Show building animation");
		}
		yield return new WaitForSeconds (15.0f);
		Debug.Log ("Finished work");

		// telling the building we're finished the work
		buildingScript.GetResult(gameObject.GetComponent<NPC>());

		goToDestination = false;
		target = null;
		busy = false;
	}
}
