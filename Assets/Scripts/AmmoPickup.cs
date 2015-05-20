using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviour {
	
	public int ammo;
	
	void Start () {
		ammo = 50;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.Rotate (new Vector3 (0,30,0) * Time.deltaTime, Space.World);
		
	}
}
