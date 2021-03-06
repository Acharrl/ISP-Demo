﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
	public GameObject reactor;
	public GameObject player;
	public float fovAngle;
	public float senseRange;
	public float health;
	public float damage;
	public float attackDelay;
	private NavMeshAgent agent;
	private string target = "reactor";
	private Vector3 direction;
	public float attackTimer;
	public float deathTimer = 0;
	public float dropChance;

	public GameObject newHealth;
	
	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		attackTimer = 0;
		dropChance = 15;
	}
	
	void Update()
	{
		direction = player.transform.position - transform.position;

		if(target.Equals("player") && agent.remainingDistance < 0.2 && direction.magnitude > senseRange)
		{
			target = "reactor";
		}
		if(target.Equals("reactor"))
		{
			agent.destination = reactor.transform.position + Vector3.ClampMagnitude((transform.position - reactor.transform.position).normalized, 2.49f);
		}
		if(health <= 0 && deathTimer == 0)
		{
			Kill();
		}
		if(Time.deltaTime >= attackTimer)
		{
			attackTimer = 0;
		}
		else
		{
			attackTimer -= Time.deltaTime;
		}
		if(target.Equals("reactor") && (transform.position - reactor.transform.position).magnitude < 2.9 && attackTimer <= 0 && deathTimer == 0)
		{
			reactor.GetComponent<Reactor>().TakeDamage(damage);
			attackTimer = attackDelay;
		}
		if(deathTimer > 0)
		{
			if(deathTimer > 1)
			{
				Destroy(gameObject);
			}
			deathTimer += Time.deltaTime;
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if(other.gameObject == player)
		{
			if(direction.magnitude <= senseRange)
			{
				TargetPlayer();
				if(direction.magnitude < 1.2 * transform.localScale.x && attackTimer <= 0 && player.GetComponent<PlayerController>().alive && deathTimer == 0)
				{
					player.GetComponent<PlayerController>().TakeDamage(damage);
					attackTimer = attackDelay;
				}
			}
			else
			{
				float angle = Vector3.Angle(direction, transform.forward);
				if(angle < fovAngle / 2)
				{
					RaycastHit hit;
					if(Physics.Raycast(transform.position, direction.normalized, out hit, GetComponent<SphereCollider>().radius))
					{
						if(hit.collider.gameObject == player)
						{
							TargetPlayer();
						}
					}
				}
			}
		}
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject == reactor && target.Equals("reactor"))
		{
			transform.LookAt(new Vector3(reactor.transform.position.x, transform.position.y, reactor.transform.position.z));
		}
	}
	
	void Sleep()
	{
		agent.Stop();
		GetComponent<Rigidbody>().Sleep();
	}
	
	void Wake()
	{
		agent.Resume();
		GetComponent<Rigidbody>().WakeUp();
	}
	
	public void TargetPlayer()
	{
		if(!agent)
		{
			agent = GetComponent<NavMeshAgent>();
		}
		target = "player";
		Wake();
		agent.destination = player.transform.position + Vector3.ClampMagnitude((transform.position - player.transform.position).normalized, 1);
	}

	public void Flinch()
	{
		for(int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).GetComponent<EnemySphereController>().Flinch();
		}
	}

	void Kill()
	{
		transform.parent.GetComponent<WaveController>().IncrementScore();
		GetComponent<CapsuleCollider>().enabled = false;
		for(int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).GetComponent<EnemySphereController>().Fly();
		}
		deathTimer = 0.01f;

		if(Random.Range(1,100) <= dropChance)
		{
			Instantiate(newHealth, transform.position, transform.rotation);
		}
	}
}
