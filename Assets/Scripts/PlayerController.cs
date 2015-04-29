using UnityEngine;
using System.Collections;

	[RequireComponent (typeof (CharacterController))]
public class PlayerController : MonoBehaviour {

	//Handling
	public float rotationSpeed = 450;
	public float walkSpeed = 5;
	public float runSpeed = 8;
	private float acceleration = 5;

	//System
	private Quaternion targetRotation;
	private Vector3 currentVelocityMod;

	//Components
	public Gun gun;
	private CharacterController controller;
	private Camera cam;

	public bool isShooting;
	
	void Start()
	{
		controller = GetComponent<CharacterController>();
		cam = Camera.main;
		isShooting = false;
	}

	void Update()
	{
		ControlMouse ();
		//ControlWASD():

		if (Input.GetButtonDown ("Shoot") && !Input.GetButton ("Run")) {
			gun.Shoot ();
			isShooting = true;
		} else if (Input.GetButton ("Shoot") && !Input.GetButton ("Run")) {
			gun.ShootContinuous();
		} else {isShooting = false;}

		if (Input.GetButtonDown ("Weapon 1")) {
			gun.SwitchWeapon ('1');
		} else if (Input.GetButtonDown ("Weapon 2")) {
			gun.SwitchWeapon ('2');
		}

	}

	void ControlMouse(){

		Vector3 mousePos = Input.mousePosition;
		mousePos = cam.ScreenToWorldPoint (new Vector3 (mousePos.x, mousePos.y, cam.transform.position.y - transform.position.y));
		targetRotation = Quaternion.LookRotation(mousePos - new Vector3(transform.position.x,0,transform.position.z));
		transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetRotation.eulerAngles.y, rotationSpeed * Time.deltaTime);

		Vector3 input = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		//No acceleration for .GetAxisRaw compared to .GetAxis		
		currentVelocityMod = Vector3.MoveTowards (currentVelocityMod, input, acceleration * Time.deltaTime);
		Vector3 motion = currentVelocityMod;
		//adds acceleration

		motion *= (Mathf.Abs (input.x) == 1 && Mathf.Abs (input.z) == 1) ? .7f : 1;
		motion *= (Input.GetButton ("Run")) ? runSpeed : walkSpeed;
		motion += Vector3.up * -8;
		
		controller.Move (motion * Time.deltaTime);
	}

	void ControlWASD(){
		Vector3 input = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		//No acceleration for .GetAxisRaw compared to .GetAxis
		
		
		if (input != Vector3.zero) { //So that it doesn't snap back to forward
			targetRotation = Quaternion.LookRotation (input);
			//Makes player look in direction of travel
			transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y,targetRotation.eulerAngles.y,rotationSpeed * Time.deltaTime);
		}
		//acceleration
		currentVelocityMod = Vector3.MoveTowards (currentVelocityMod, input, acceleration * Time.deltaTime);
		Vector3 motion = input;

		motion *= (Mathf.Abs (input.x) == 1 && Mathf.Abs (input.z) == 1) ? .7f : 1;
		motion *= (Input.GetButton("Run"))?runSpeed:walkSpeed;
		motion += Vector3.up * -8;
		
		controller.Move(motion * Time.deltaTime);
	}
}
