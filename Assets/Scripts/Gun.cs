using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour
{

	public enum GunType
	{
		Semi,
		Burst,
		Auto
	}
	;
	public int gunID;
	public GunType gunType;
	public float rpm;
	public Transform spawn;
	public Transform shellEjectionPoint;
	public Rigidbody shell;
	public Text gunText;
	public float damage;
	private LineRenderer tracer;

	//ammo
	public int ammoCount;
	public int clipSize;
	public int ammoLoaded;
	public int ammoNotLoaded;
	public bool reloading;
	public float reloadEndTime;
	public float reloadTime;

	//System
	private float secondsBetweenShots;
	private float nextPossibleShootTime;
	public float revAmount = 0;
	public float revSpeed;
	public float rotationSpeed;

	void Start()
	{
		secondsBetweenShots = 60 / rpm;
		if(GetComponent<LineRenderer>())
		{
			tracer = GetComponent<LineRenderer>();
		}
		tracer.enabled = false;

		if(ammoLoaded > clipSize)
		{
			ammoLoaded = clipSize;
		}

		reloading = false;
		ammoNotLoaded = ammoCount - ammoLoaded;

		nextPossibleShootTime = Time.time + secondsBetweenShots;
	}

	public void Update()
	{
		if(Time.time >= reloadEndTime && reloading)
		{
			reloading = false;
			if(ammoCount >= clipSize)
			{
				ammoLoaded = clipSize;
			}
			else
			{
				ammoLoaded = ammoCount;
			}
			
			ammoNotLoaded = ammoCount - ammoLoaded;
		}

		if(!reloading && ammoLoaded == 0 && ammoCount > 0)
		{
			reload();
		}

		transform.GetChild(0).Rotate(new Vector3(0, rotationSpeed * Time.deltaTime * revAmount, 0), Space.Self);
	}

	public string Shoot()
	{	
		string message = CanShoot();
		if(message.Equals("shot"))
		{
			ammoCount -= 1;
			ammoLoaded -= 1;

			Ray ray = new Ray(spawn.position, spawn.forward);
			RaycastHit hit;

			float shotDistance = 20;

			if(Physics.Raycast(ray, out hit, shotDistance))
			{
				shotDistance = hit.distance;
				if(hit.collider.gameObject.tag == "Enemy")
				{
					hit.collider.GetComponent<EnemyController>().health -= damage;
					hit.collider.GetComponent<EnemyController>().Flinch();
				}

				if(hit.collider.gameObject.tag == "Reactor")
				{
					hit.collider.gameObject.GetComponent<Reactor>().TakeDamage(damage);
				}

			}


			nextPossibleShootTime = Time.time + secondsBetweenShots;

			GetComponent<AudioSource>().Play();

			if(tracer)
			{
				StartCoroutine("RenderTracer", ray.direction * shotDistance);
			}

			Rigidbody newShell = Instantiate(shell, shellEjectionPoint.position, Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(90, Random.value * 30, 0))) as Rigidbody;
			newShell.AddForce(shellEjectionPoint.forward * Random.Range(100f, 150f) + spawn.forward * Random.Range(-50f, 50f));
		}

		return message;
	}

	public void reload()
	{

		if(!reloading && ammoLoaded != clipSize)
		{
			reloadEndTime = Time.time + reloadTime;
			reloading = true;
		}


	}

	public void ShootContinuous()
	{
		if(gunType == GunType.Auto)
		{
			Shoot();
		}


	}

	private string CanShoot()
	{
		string message = "shot";

		if(Time.time < nextPossibleShootTime)
		{
			message = "too soon";
		}

		if(ammoLoaded == 0)
		{
			message = "click";
		}

		if(reloading)
		{
			message = "reloading";
		}

		if(gunID == 3 && revAmount < 1)
		{
			message = "rev";
		}

		return message;
	}

	IEnumerator RenderTracer(Vector3 hitPoint)
	{
		tracer.enabled = true;
		tracer.SetPosition(0, spawn.position);
		tracer.SetPosition(1, spawn.position + hitPoint);
		yield return null;
		tracer.enabled = false;
	}
}
