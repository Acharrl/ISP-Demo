using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour {

	public float health;

	void Start () {
		health = 50;

	}
	
	// Update is called once per frame
	void Update () {

		transform.Rotate (new Vector3 (0,30,0) * Time.deltaTime, Space.World);
	
	}
}
