using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Player : MonoBehaviour {

	public float gravity = -3.0f;
	public float speed = 0.2f;

	private Blackboard blackboard = null;

	public int totalCoins = 100;

	public bool canPay = false;
	private bool isPaying = false;
	public GameObject activeBuilding;
	public Building buildingScript;


	private CharacterController controller;

	private Vector3 direction = Vector3.zero;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();

		if (!blackboard) {
			blackboard = GameObject.Find ("Blackboard").GetComponent<Blackboard> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		// moving
		float inputH = Input.GetAxisRaw("Horizontal");
		direction = Vector3.right * inputH;
		controller.Move(direction * speed + Vector3.up * gravity);

		// action
		float inputV = Input.GetAxisRaw("Vertical");

		if (inputV < 0 && canPay && totalCoins > 0) {
			if (!isPaying) {
				Debug.Log ("purchased??: " + buildingScript.purchased);
				if (!buildingScript.purchased) {
//					Debug.Log ("call Pay()");
					isPaying = true;
					StartCoroutine (Pay ());
				}
			}
		}
	}

	IEnumerator Pay(){
		yield return new WaitForSeconds (0.5f);
		Debug.Log ("Pay");
		totalCoins -= 1;
		isPaying = false;
		if (activeBuilding) {
			buildingScript.GetPaid ();
		}
	}

	public void ReturnCoins(int returnedCoins){
		totalCoins += returnedCoins;
	}

}
