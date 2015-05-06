using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Reactor : MonoBehaviour
{
	public float health;
	public Text reactorText;

	void Start () {
		health = 500;
	}

	void Update () {
		if (health <= 0) {
			reactorText.text = "Reactor Failure";
		}
		else{
			reactorText.text = "Reactor Health: " + health;
		}
	}
}
