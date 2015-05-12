using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Reactor : MonoBehaviour
{
	public float health;
	public float maxHealth;
	private bool justDamaged;
	private bool alive;
	private float timeDamaged;
	public float repairSpeed;
	public Text reactorText;
	public Slider reactorSlider;

	void Start () {
		health = 500;
		maxHealth = health;
		repairSpeed = 2;
		justDamaged = false;
		reactorSlider.value = health;
		alive = true;
	}

	void Update () {
		if (health <= 0) {
			alive = false;
			health = 0;
			reactorText.text = "Reactor Failure";
		}
		else{
			reactorText.text = "Reactor Health: " + (int)health;
		}

		if (justDamaged) {
			if (Time.time > timeDamaged + 5) {
				justDamaged = false;
			}
		} else if (alive) {
			if(health < maxHealth){
				health = health + (Time.deltaTime * repairSpeed);
			}
		}

		reactorSlider.value = health;
	}

	public void TakeDamage(float damage)
	{
		health -= damage;
		justDamaged = true;
		timeDamaged = Time.time;
	}
}
