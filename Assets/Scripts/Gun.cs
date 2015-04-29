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
	public GunType gunType;
	public float rpm;
	public Transform spawn;
	public Transform shellEjectionPoint;
	public Rigidbody shell;
	public Text gunText;
	private LineRenderer tracer;

	//System
	private float secondsBetweenShots;
	private float nextPossibleShootTime;

	void Start ()
	{
		secondsBetweenShots = 60 / rpm;
		if (GetComponent<LineRenderer> ()) {
			tracer = GetComponent<LineRenderer> ();
		}

		gunType = GunType.Semi;
	}

	public void Shoot ()
	{

		if (CanShoot ()) {
			Ray ray = new Ray (spawn.position, spawn.forward);
			RaycastHit hit;

			float shotDistance = 20;

			if (Physics.Raycast (ray, out hit, shotDistance)) {
				shotDistance = hit.distance;
			}

			nextPossibleShootTime = Time.time + secondsBetweenShots;

			GetComponent<AudioSource> ().Play ();

			if (tracer) {
				StartCoroutine ("RenderTracer", ray.direction * shotDistance);
			}

			Rigidbody newShell = Instantiate (shell, shellEjectionPoint.position, Quaternion.Euler (transform.rotation.eulerAngles + new Vector3 (90, 0, 0))) as Rigidbody;
			newShell.AddForce (shellEjectionPoint.forward * Random.Range (150f, 200f) + spawn.forward * Random.Range (-10f, 10f));
		}


	}

	public void ShootContinuous ()
	{
		if (gunType == GunType.Auto) {
			Shoot ();
		}


	}

	public void SwitchWeapon(char wpn)
	{
		if (wpn.Equals ('1')) {
			gunType = GunType.Semi;
			gunText.text = "Pistol";
		} else if (wpn.Equals ('2')) {
			gunType = GunType.Auto;
			gunText.text = "Assault Rifle";
		}
	}

	private bool CanShoot ()
	{
		bool canShoot = true;

		if (Time.time < nextPossibleShootTime) {
			canShoot = false;
		}

		return canShoot;
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
