using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{


	public float rotationSpeed = 450;
	public float walkSpeed = 5;
	public float runSpeed = 8;
	private float acceleration = 5;
	public GameObject reactor;
	private Quaternion targetRotation;
	private Vector3 currentVelocityMod;
	public float health;
	public float maxHealth;
	public Transform hand;
	public Gun[] guns;
	public string[] gunList;
	public int[,] gunInfo = new int[10, 2];
	public Text gunText;
	public Gun equippedGun;
	private CharacterController controller;
	private Camera cam;
	public Text healthText;
	public Text ammoCountText;
	public Text ammoLoadedText;
	public Text gameOverText;
	public bool isShooting;
	private bool revButton;
	public bool alive;
	private Material mat;
	private Color originalCol;
	private float fadePercent;
	private float deathTime;
	private bool fading;
	private bool justDamaged;
	public Slider healthSlider;
	
	void Start()
	{
		controller = GetComponent<CharacterController>();
		cam = Camera.main;
		isShooting = false;
		alive = true;
		deathTime = 0f;
		maxHealth = 100f;
		health = maxHealth;
		healthSlider.value = health;
		gunText.text = gunList[0];
		justDamaged = false;

		for(int i = 0; i < guns.Length; i++)
		{
			gunInfo[i, 0] = guns[i].ammoCount;
			gunInfo[i, 1] = guns[i].ammoLoaded;
		}

		EquipGun(0);

	}
	
	public void EquipGun(int i)
	{
		if(equippedGun)
		{
			gunInfo[equippedGun.gunID, 0] = equippedGun.ammoCount;
			gunInfo[equippedGun.gunID, 1] = equippedGun.ammoLoaded;
			Destroy(equippedGun.gameObject);
		}
		equippedGun = Instantiate(guns[i], hand.position, hand.rotation) as Gun;
		equippedGun.transform.parent = hand;
		gunText.text = gunList[i];
		equippedGun.ammoCount = gunInfo[equippedGun.gunID, 0];
		equippedGun.ammoLoaded = gunInfo[equippedGun.gunID, 1];
		ammoCountText.text = "" + equippedGun.ammoLoaded;
		ammoLoadedText.text = "/" + equippedGun.ammoNotLoaded;
	}

	public void TakeDamage(float damage)
	{
		justDamaged = true;
		health -= damage;
		if(health < 0)
		{
			health = 0;
		}
		healthSlider.value = health;
	}

	public void Update()
	{
		if(equippedGun.gunID == 3 && !isShooting)
		{
			if(equippedGun.firing.isPlaying)
			{
				equippedGun.firing.Stop();
			}
		}

		revButton = Input.GetButton("Shoot") || Input.GetButton("Rev");
		
		if(health > maxHealth)
		{
			health = maxHealth;
		}

		if(equippedGun && !equippedGun.reloading)
		{
			equippedGun.ammoNotLoaded = equippedGun.ammoCount - equippedGun.ammoLoaded;
			ammoCountText.text = "" + equippedGun.ammoLoaded;
			ammoLoadedText.text = "/" + equippedGun.ammoNotLoaded;
		}
		else if(equippedGun && equippedGun.reloading)
		{
			ammoCountText.text = "";
			ammoLoadedText.text = "Reloading: " + (equippedGun.reloadEndTime - Time.time);
		}

		healthText.text = "Player Health: " + health;

		if(alive)
		{
			ControlMouse();

			if(equippedGun && !Input.GetButton("Run"))
			{
				if(Input.GetButtonDown("Shoot"))
				{
					string didShoot = equippedGun.Shoot();
					isShooting = didShoot.Equals("shot");
					if(didShoot.Equals("click"))
					{
						GetComponent<AudioSource>().Play();
					}
				}
				else if(Input.GetButton("Shoot"))
				{
					equippedGun.ShootContinuous();
				}
				else
				{
					isShooting = false;
				}
				
				if(equippedGun.gunID == 3 && revButton)
				{
					if(equippedGun.revAmount < 1)
					{
						if(!equippedGun.spinup.isPlaying)
						{equippedGun.spinup.Play();}
						equippedGun.revAmount += equippedGun.revSpeed * Time.deltaTime;
					}
					if(equippedGun.revAmount > 1)
					{
						equippedGun.revAmount = 1;
					}
					if(equippedGun.revAmount == 1 && !equippedGun.spinning.isPlaying)
					{
						equippedGun.spinning.Play();
					}
				}
				if(equippedGun.gunID == 3 && !revButton)
				{
					if(equippedGun.revAmount < 1 && equippedGun.spinup.isPlaying)
					{
						equippedGun.spinup.Stop();
						equippedGun.spindown.Play();
					}
					if(equippedGun.revAmount == 1 && equippedGun.spinning.isPlaying)
					{
						equippedGun.spinning.Stop();
						equippedGun.spindown.Play();
					}
				}
			}
			else if(Input.GetButton("Run"))
			{
				if(equippedGun.gunID == 3)
				{
					if(equippedGun.revAmount < 1 && equippedGun.spinup.isPlaying)
					{
						equippedGun.spinup.Stop();
						equippedGun.spindown.Play();
					}
					if(equippedGun.revAmount == 1 && equippedGun.spinning.isPlaying)
					{
						equippedGun.spinning.Stop();
						equippedGun.spindown.Play();
					}
				}
				isShooting = false;
			}

			if(Input.GetButtonDown("Weapon 1"))
			{
				EquipGun(0);
			}
			else if(Input.GetButtonDown("Weapon 2"))
			{
				EquipGun(1);
			}
			else if(Input.GetButtonDown("Weapon 3"))
			{
				EquipGun(2);
			}
			else if(Input.GetButtonDown("Next Weapon") && equippedGun.gunID < (guns.Length - 1))
			{
				EquipGun(equippedGun.gunID + 1);
			}
			else if(Input.GetButtonDown("Next Weapon") && equippedGun.gunID == (guns.Length - 1))
			{
				EquipGun(0);
			}
			else if(Input.GetButtonDown("Previous Weapon") && equippedGun.gunID > 0)
			{
				EquipGun(equippedGun.gunID - 1);
			}
			else if(Input.GetButtonDown("Previous Weapon") && equippedGun.gunID == 0)
			{
				EquipGun(guns.Length - 1);
			}
			else if(Input.GetButtonDown("Reload"))
			{
				equippedGun.reload();
			}

			if(justDamaged)
			{
				gameObject.GetComponentInChildren<Light>().color = new Color(1f, 0f, 0f, 1f);
			}
			else
			{
				gameObject.GetComponentInChildren<Light>().color = Color.Lerp(gameObject.GetComponentInChildren<Light>().color, Color.white, 5f * Time.deltaTime);
			}
			justDamaged = false;
			if(health >= 50)
			{
				healthSlider.GetComponentInChildren<Image>().color = new Color(((100 - health) / 50), 1f, 0f, 1f);
			}
			if(health < 50)
			{
				healthSlider.GetComponentInChildren<Image>().color = new Color(1f, (health / 50), 0f, 1f);
			}

			if(health <= 0)
			{
				alive = false;
				if(equippedGun.gunID == 3)
				{
					equippedGun.spinning.Stop();
					equippedGun.firing.Stop();
					if(equippedGun.revAmount > 0)
					{
						equippedGun.spindown.Play();
					}
				}
			}
		}
		else
		{
			if(deathTime == 0f)
			{
				mat = GetComponent<Renderer>().material;
				originalCol = mat.color;
				deathTime = Time.time;
			}
			else
			{
				fadePercent += Time.deltaTime * 0.2f;
				
				if(fadePercent < .6)
				{
					mat.color = Color.Lerp(originalCol, new Color(.47f, .46f, .46f, .7f), fadePercent * 1.3f);
				}
					
				if(fadePercent >= .6 && fadePercent < 1)
				{
					mat.color = Color.Lerp(new Color(.47f, .46f, .46f, .7f), Color.clear, (fadePercent - .6f) * 2.5f);
				}
			
			}
		}
		if(equippedGun.revAmount > 0 && (!revButton || Input.GetButton("Run") || !alive))
		{
			equippedGun.revAmount -= equippedGun.revSpeed * Time.deltaTime;
			if(equippedGun.revAmount < 0)
			{
				equippedGun.revAmount = 0;
			}
		}
		if(!alive || reactor.GetComponent<Reactor>().health <= 0)
		{
			gameOverText.text = "Game Over";
		}

	}

	void ControlMouse()
	{

		Vector3 mousePos = Input.mousePosition;
		mousePos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.transform.position.y - transform.position.y));
		targetRotation = Quaternion.LookRotation(mousePos - new Vector3(transform.position.x, 0, transform.position.z));
		transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, rotationSpeed * Time.deltaTime);

		Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
	
		currentVelocityMod = Vector3.MoveTowards(currentVelocityMod, input, acceleration * Time.deltaTime);
		Vector3 motion = currentVelocityMod;

		motion *= (Mathf.Abs(input.x) == 1 && Mathf.Abs(input.z) == 1) ? .7f : 1;
		motion *= (Input.GetButton("Run")) ? runSpeed : walkSpeed;
		motion *= 1 - (0.5f * equippedGun.revAmount);
		motion += Vector3.up * -8;
		
		controller.Move(motion * Time.deltaTime);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Health Collectible"))
		{
			health += other.gameObject.GetComponent<HealthPickup>().health;
			other.gameObject.SetActive(false);
			healthSlider.value = health;
		}
		if (other.gameObject.CompareTag ("Ammo Collectible"))
		{
			equippedGun.ammoCount += other.gameObject.GetComponent<AmmoPickup>().ammo;
			other.gameObject.SetActive(false);
		}
	}
	

}
