using UnityEngine;
using UnityEngine.UI;
using System.Collections;

	[RequireComponent (typeof (CharacterController))]
public class PlayerController : MonoBehaviour {


	public float rotationSpeed = 450;
	public float walkSpeed = 5;
	public float runSpeed = 8;
	private float acceleration = 5;


	private Quaternion targetRotation;
	private Vector3 currentVelocityMod;

	public float playerHealth;
	public Transform hand;
	public Gun[] guns;
	public string[] gunList;
	public Text gunText;
	public Gun equippedGun;
	private CharacterController controller;
	private Camera cam;
	public Text healthText;

	public bool isShooting;
	public bool alive;

	private Material mat;
	private Color originalCol;
	private float fadePercent;
	private float deathTime;
	private bool fading;
	
	void Start()
	{
		controller = GetComponent<CharacterController>();
		cam = Camera.main;
		isShooting = false;
		alive = true;
		deathTime = 0f;
		playerHealth = 100f;
		gunText.text = gunList [0];
		EquipGun (0);
	}

	public void EquipGun(int i){
		if (equippedGun) {
			Destroy (equippedGun.gameObject);
		}

		equippedGun = Instantiate (guns [i], hand.position, hand.rotation) as Gun;
		equippedGun.transform.parent = hand;
		gunText.text = gunList [i];
	}



	public void Update()
	{
		healthText.text = "Player Health: " + playerHealth;
		if (alive) {
			ControlMouse ();
			if (equippedGun && !Input.GetButton ("Run")) {
				if (Input.GetButtonDown ("Shoot")) {
					equippedGun.Shoot ();
					isShooting = true;
				} else if (Input.GetButton ("Shoot")) {
					equippedGun.ShootContinuous ();
				} else {
					isShooting = false;
				}
			}

			if (Input.GetButtonDown ("Weapon 1")) {
				EquipGun (0);
			} else if (Input.GetButtonDown ("Weapon 2")) {
				EquipGun (1);
			} else if (Input.GetButtonDown ("Weapon 3")) {
				EquipGun (2);
			} else if (Input.GetButtonDown ("Next Weapon") && equippedGun.gunID < (guns.Length - 1)) {
				print (equippedGun.gunID);
				EquipGun (equippedGun.gunID + 1);
				print (equippedGun.gunID);
			} else if (Input.GetButtonDown ("Next Weapon") && equippedGun.gunID == (guns.Length - 1)) {
				print (equippedGun.gunID);
				EquipGun (0);
				print (equippedGun.gunID);
			} else if (Input.GetButtonDown ("Previous Weapon") && equippedGun.gunID > 0) {
				EquipGun (equippedGun.gunID - 1);
			} else if (Input.GetButtonDown ("Previous Weapon") && equippedGun.gunID == 0) {
				EquipGun (guns.Length - 1);
			}

			if (playerHealth <= 0) {
				alive = false;
			}
		} else {
			if(deathTime == 0f){
				mat = GetComponent<Renderer> ().material;
				originalCol = mat.color;
				deathTime = Time.time;
			}
			else{
				fadePercent += Time.deltaTime * 0.2f;
				
				if(fadePercent < .6){
					mat.color = Color.Lerp (originalCol, new Color(.47f,.46f,.46f,.7f), fadePercent * 1.3f);
				}
					
				if (fadePercent >= .6 && fadePercent < 1) {
					mat.color = Color.Lerp (new Color(.47f,.46f,.46f,.7f), Color.clear, (fadePercent-.6f) * 2.5f);
				}
			
			}
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
}
