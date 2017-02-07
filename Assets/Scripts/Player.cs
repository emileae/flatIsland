using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Player : MonoBehaviour {

	public float gravity = -3.0f;
	public float speed = 0.2f;

	public bool canPay = false;

	private CharacterController controller;

	private Vector3 direction = Vector3.zero;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		float inputH = Input.GetAxisRaw("Horizontal");

		direction = Vector3.right * inputH;

		controller.Move(direction * speed + Vector3.up * gravity);
	}
}
