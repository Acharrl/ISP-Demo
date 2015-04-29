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

			if(fadePercent < .6){
			mat.color = Color.Lerp (originalCol, new Color(.47f,.46f,.46f,.7f), fadePercent * 1.3f);
			}

			if (fadePercent >= .6 && fadePercent < 1) {
				mat.color = Color.Lerp (new Color(.47f,.46f,.46f,.7f), Color.clear, (fadePercent-.6f) * 2.5f);
			}

			if(fadePercent > 1)
			{
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
