﻿using UnityEngine;
using System.Collections;

public class EnemySphereController : MonoBehaviour
{
	private Transform sphere;
	private float rotationSpeed;
	private Vector3 originalPos;
	private float flinchRadius = 1;

	void Start()
	{
		sphere = transform.GetChild(0);
		originalPos = Random.Range(0.25f, 0.5f) * Vector3.forward;
		sphere.localPosition = originalPos;
		transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), 0));
		rotationSpeed = Random.Range(180, 360);
	}

	void Update()
	{
		transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
		if(flinchRadius - 1 > Time.deltaTime)
		{
			flinchRadius -= 2 * Time.deltaTime;
		}
		else
		{
			flinchRadius = 1;
		}
		sphere.localPosition = flinchRadius * originalPos;
	}

	public void Flinch()
	{
		flinchRadius = 2;
	}
}
