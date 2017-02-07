using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	private NavMeshAgent agent;
	public bool goToDestination = false;
	public GameObject target = null;

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
}
