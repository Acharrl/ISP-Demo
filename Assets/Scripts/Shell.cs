using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour
{

	private float lifeTime = 5;
	private Material mat;
	private Color originalCol;
	private float fadePercent;
	private float deathTime;
	private bool fading;

	void Start ()
	{
		mat = GetComponent<Renderer> ().material;
		originalCol = mat.color;
		deathTime = Time.time + lifeTime;

		//StartCoroutine ("Fade");
	}

	void Update ()
	{
		if (fading) {
			fadePercent += Time.deltaTime * 0.2f;
			mat.color = Color.Lerp (originalCol, Color.clear, fadePercent);

			if (fadePercent >= 1) {
				Destroy (gameObject);
			}
		}
		else {
			if (Time.time > deathTime) {
				fading = true;
			}
		}
	}

	void OnTriggerEnter (Collider c)
	{
		if (c.tag == "Ground") {
			GetComponent<Rigidbody> ().Sleep ();
			//If shells collide with ground, they stop animating
		}
	}
		
}
