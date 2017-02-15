using UnityEngine;
using System.Collections;

public class ThirdPersonPlayer : MonoBehaviour {

	private Blackboard blackboard = null;

	public float speed = 0.2f;

	public float turnSmoothtime = 0.12f;
	float turnSmoothVelocity;

	public float speedSmoothtime = 0.1f;
	float speedSmoothVelocity;
	float currentSpeed;

	private CharacterController controller;

	Transform cameraT;

	// animations
	private Animator anim;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
//		agent = GetComponent<NavMeshAgent>();

		if (!blackboard) {
			blackboard = GameObject.Find ("Blackboard").GetComponent<Blackboard> ();
		}
		cameraT = Camera.main.transform;
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update ()
	{

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		Vector2 inputDir = input.normalized;

		if (inputDir != Vector2.zero) {
			float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothtime);
		}

		float targetSpeed = speed * inputDir.magnitude;
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothtime);

		Vector3 velocity = transform.forward * currentSpeed;

		Debug.Log("Velocity: " + velocity);

		controller.Move(velocity * Time.deltaTime);

	}
}
