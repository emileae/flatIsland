using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Player : MonoBehaviour {

	public float gravity = -3.0f;
	public float speed = 0.2f;

	private Blackboard blackboard = null;

	public int carryingCoins = 100;

	public bool canPay = false;
	public bool canCollect = false;
	private bool isPaying = false;
	private bool isCollecting = false;
	public GameObject activeBuilding;
	public Building buildingScript;

	// animations
	private Animator anim;


	private CharacterController controller;
	private NavMeshAgent agent;

	private Vector3 direction = Vector3.zero;

	// calling men
	private bool whistling = false;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
//		agent = GetComponent<NavMeshAgent>();

		if (!blackboard) {
			blackboard = GameObject.Find ("Blackboard").GetComponent<Blackboard> ();
		}

		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// moving
		float inputH = Input.GetAxisRaw ("Horizontal");
		direction = Vector3.right * inputH;
		controller.Move (direction * speed + Vector3.up * gravity);

		if (direction != Vector3.zero) {
			anim.SetBool ("Walking", true);
		} else {
			anim.SetBool ("Walking", false);
		}

		// rotate
//		Vector3 targetAngles;
//		float smooth = 1.0f;
		if (inputH > 0) {
//			targetAngles = transform.eulerAngles + 180f * Vector3.up; // what the new angles should be
//			transform.eulerAngles = Vector3.Lerp (transform.eulerAngles, targetAngles, smooth * Time.deltaTime);
			Quaternion rotation = Quaternion.LookRotation(Vector3.right);
			transform.rotation = rotation;
		} else if (inputH < 0) {
//			targetAngles = transform.eulerAngles - 180f * Vector3.up; // what the new angles should be
//			transform.eulerAngles = Vector3.Lerp (transform.eulerAngles, targetAngles, smooth * Time.deltaTime);
			Quaternion rotation = Quaternion.LookRotation(-Vector3.right);
			transform.rotation = rotation;
		}

//		agent.Move(direction);

		// action
		float inputV = Input.GetAxisRaw ("Vertical");

		// call men
		bool whistle = Input.GetButton("Fire3");

		if (inputV < 0 && canPay && carryingCoins > 0) {
			if (!isPaying) {
				if (!buildingScript.occupied) {
					isPaying = true;
					StartCoroutine (Pay ());
				}
			}
		}
		if (inputV > 0 && canCollect) {
			if (!isCollecting) {
				isCollecting = true;
				if (buildingScript.storedCoins > 0) {
					StartCoroutine (Collect ());
				} else {
					isCollecting = false;
				}
			}
		}

		if (whistle && !whistling) {
			whistling = true;
			StartCoroutine (Whistle());
		}
	}

	IEnumerator Pay(){
		yield return new WaitForSeconds (0.1f);
		Debug.Log ("Pay");
		carryingCoins -= 1;
		isPaying = false;
		if (activeBuilding) {
			buildingScript.GetPaid ();
		}
	}

	public void ReturnCoins(int returnedCoins){
		carryingCoins += returnedCoins;
	}

	IEnumerator Collect(){
		yield return new WaitForSeconds (0.1f);
		Debug.Log ("Collect");
		carryingCoins += 1;
		buildingScript.storedCoins -= 1;
		isCollecting = false;
	}

	IEnumerator Whistle(){
		Debug.Log ("Play dramatic whistle / bugle animation.....");
		blackboard.CallNPCsToPlayer (gameObject);
		yield return new WaitForSeconds (1.0f);
		whistling = false;
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		if (hit.gameObject.tag == "NPC") {
			NavMeshAgent npcAgent = hit.gameObject.GetComponent<NavMeshAgent> ();
			npcAgent.Move (Vector3.forward);
		}
	}

}
