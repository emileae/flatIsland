using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	private NavMeshAgent agent;
	public bool goToDestination = false;
	public GameObject target = null;

	public bool busy = false;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		if (goToDestination && target != null) {
			agent.SetDestination(target.transform.position);
		}
	}

	public void StartWork(){
		busy = true;
		StartCoroutine (Work());
	}

	IEnumerator Work(){
		Debug.Log ("Start work");
		yield return new WaitForSeconds (15.0f);
		Debug.Log ("Finished work");
		busy = false;
	}
}
