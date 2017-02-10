using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;

public class NPC : MonoBehaviour {

	private Blackboard blackboard;

	private NavMeshAgent agent;
	public bool goToDestination = false;
	public GameObject target = null;

	public bool busy = false;
	public GameObject building;
	public Building buildingScript;

	// working functions
	private IEnumerator work;

	// money earning
	private bool depositingCoins = false;
	public int coinsHeld = 0;

	// Enemies
	public bool alert = false;
	private bool attacking = false;
	public GameObject enemy = null;
	public Enemy enemyScript = null;

	// Called to player
	public Vector3 playerDestination;
	public bool goToPlayer = false;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		work = Work ();

		if (!blackboard) {
			blackboard = GameObject.Find ("Blackboard").GetComponent<Blackboard> ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		// go to a normal destination/target
		if (goToDestination && target != null) {
			busy = true;
			agent.SetDestination (target.transform.position);
		}

		// attack enemy
		if (alert) {
			if (!attacking) {
				attacking = true;
				StartCoroutine (Attack ());
			}
		}

		// go to player
		if (goToPlayer && playerDestination != null) {
			target =null;
			agent.SetDestination (playerDestination);
			busy = false;
			goToPlayer = false;
		}

	}

	/// <summary>
	///  might be useful to determin if navMesh agent has reached its destination
	/// </summary>
	/// <returns><c>true</c> if this instance has reached target; otherwise, <c>false</c>.</returns>
//	void HasReachedTarget ()
//	{
//		if (!agent.pathPending)
//		 {
//			if (agent.remainingDistance <= agent.stoppingDistance)
//		     {
//				if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
//		         {
//		             target = null;
//		         }
//		     }
//		 }
//	}


	public void GoToTarget(GameObject destination){
		if (target == destination) {
//			Debug.Log ("Already there....");
			goToDestination = true;
			StartWork ();
		}else{
			target = destination;
			goToDestination = true;
		}

	}

	public void StartWork ()
	{
		Debug.Log ("Should start work?");
		if (buildingScript) {
//			Debug.Log ("Should start work? --> Coroutine");
			work = Work ();
			if (buildingScript.isBase && depositingCoins) {
				DepositCoins();
			} else {
				StartCoroutine (work);
			}
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
//		Debug.Log ("Start work");

		BuildingParameters parameters = new BuildingParameters (buildingScript.level, buildingScript);


//		Debug.Log ("workTime " + parameters.workTime);
//		Debug.Log ("animation state " + parameters.animationState);

		workTime = parameters.workTime;

//		if (buildingScript.built) {
//
//			if (buildingScript.fishingSpot) {
//				
//			}else if (buildingScript.garden){
//				
//			}
//
//			isMaking = true;
//			workTime = buildingScript.makeTime;
//			Debug.Log ("Show making animation");
//		} else {
//			isBuilding = true;
//			workTime = buildingScript.buildTime;
//			Debug.Log ("Show building animation");
//		}

		yield return new WaitForSeconds (workTime);

		// set building's built status to 'built'
		if (!buildingScript.built) {
			buildingScript.built = true;
			buildingScript.hp = parameters.hp;
		}

		// telling the building we're finished the work
		buildingScript.occupied = false;

		goToDestination = false;

		busy = false;

		// collect earnings
		coinsHeld += parameters.coinsEarned;

		if (parameters.coinsEarned > 0) {
//			Debug.Log ("Drop off coins at base.....");
			target = blackboard.baseScript.npcTarget;
			depositingCoins = true;
			busy = true;
			goToDestination = true;
		}

	}

	public void DepositCoins ()
	{
		if (coinsHeld > 0) {
			StartCoroutine (DropOffCoins ());
		}
	}

	IEnumerator DropOffCoins ()
	{
		// multiply coinsHeld by the deposit time per coin....
		float coinDropoffTime = 2.0f;
		yield return new WaitForSeconds (coinDropoffTime);
//		Debug.Log("dropped off a coin... coins left: " + coinsHeld);
		coinsHeld -= 1;
		buildingScript.storedCoins += 1;
		if (coinsHeld > 0) {
			DepositCoins ();
		} else {
			depositingCoins = false;
			busy = false;
			goToDestination = false;
		}

	}

	IEnumerator Attack(){
		yield return new WaitForSeconds (2.0f);
		Debug.Log ("Attack the enemy !!!!!!!!!");
		if (enemy != null && enemyScript != null) {
			enemyScript.hp -= 1;
			attacking = false;
		}
	}
}
