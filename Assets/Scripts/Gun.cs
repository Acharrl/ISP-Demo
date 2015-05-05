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
		Auto}
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

	void Start ()
	{
		secondsBetweenShots = 60 / rpm;
		if (GetComponent<LineRenderer> ()) {
			tracer = GetComponent<LineRenderer> ();
		}

		if (ammoLoaded > clipSize) {
			ammoLoaded = clipSize;
		}

		reloading = false;
		ammoNotLoaded = ammoCount - ammoLoaded;
				
	}

	public void Shoot ()
	{
		if (CanShoot ()) {
			ammoCount -= 1;
			ammoLoaded -= 1;

			Ray ray = new Ray (spawn.position, spawn.forward);
			RaycastHit hit;

			float shotDistance = 20;

			if (Physics.Raycast (ray, out hit, shotDistance)) {
				shotDistance = hit.distance;
				if (hit.collider.gameObject.tag == "Enemy")
				{
					hit.collider.gameObject.GetComponent<EnemyController>().health -= damage;
				}
			}


			nextPossibleShootTime = Time.time + secondsBetweenShots;

			GetComponent<AudioSource> ().Play ();

			if (tracer) {
				StartCoroutine ("RenderTracer", ray.direction * shotDistance);
			}

			Rigidbody newShell = Instantiate (shell, shellEjectionPoint.position, Quaternion.Euler (transform.rotation.eulerAngles + new Vector3 (90, Random.value * 30, 0))) as Rigidbody;
			newShell.AddForce (shellEjectionPoint.forward * Random.Range (100f, 150f) + spawn.forward * Random.Range (-50f, 50f));
		}


	}

	public void reload()
	{

		if (!reloading && ammoLoaded != clipSize) {
			reloadEndTime = Time.time + reloadTime;
			reloading = true;
		}
		if (ammoCount >= clipSize) {
			ammoLoaded = clipSize;
		}
		else {ammoLoaded = ammoCount;}

		ammoNotLoaded = ammoCount - ammoLoaded;

	}

	public void ShootContinuous ()
	{
		if (gunType == GunType.Auto) {
			Shoot ();
		}


	}
	

	private bool CanShoot ()
	{
		bool canShoot = true;

		if (Time.time < nextPossibleShootTime) {
			canShoot = false;
		}

		if (ammoLoaded == 0) {
			canShoot = false;
		}

		if (reloading) {
			canShoot = false;
		}

		return canShoot;
	}

	public bool isReloading()
	{
		if (Time.time >= reloadEndTime) {
			reloading = false;
		}
		return reloading;
	}

	IEnumerator RenderTracer (Vector3 hitPoint)
	{
		tracer.enabled = true;
		tracer.SetPosition (0, spawn.position);
		tracer.SetPosition (1, spawn.position + hitPoint);
		yield return null;
		tracer.enabled = false;
	}
}
