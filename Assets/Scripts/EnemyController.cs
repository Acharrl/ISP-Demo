using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
	public GameObject reactor;
	public GameObject player;
	public float fovAngle;
	public float fovRange;
	public float senseRange;
	public float health;
	public float damage;
	public float attackDelay;
	private NavMeshAgent agent;
	private bool targetingReactor;
	private float attackTimer;

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		targetingReactor = true;
		attackTimer = 0;
	}

	void Update()
	{
		if(agent.remainingDistance < 0.2)
		{
			targetingReactor = true;
		}
		if(targetingReactor)
		{
			agent.destination = reactor.transform.position + Vector3.ClampMagnitude((transform.position - reactor.transform.position).normalized, 2.49f);
		}
		if(health <= 0)
		{
			Destroy(gameObject);
		}
		if(Time.deltaTime >= attackTimer)
		{
			attackTimer = 0;
		}
		else
		{
			attackTimer -= Time.deltaTime;
		}
	}

	void OnTriggerStay(Collider other)
	{
		if(other.gameObject == player)
		{
			Vector3 direction = player.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);
			if(angle < fovAngle / 2)
			{
				RaycastHit hit;
				if(Physics.Raycast(transform.position, direction.normalized, out hit, fovRange))
				{
					if(hit.collider.gameObject == player)
					{
						TargetPlayer();
						if(direction.magnitude < 1.2 && attackTimer == 0 && player.GetComponent<PlayerController>().alive)
						{
							player.GetComponent<PlayerController>().health -= damage;
							attackTimer = attackDelay;
						}
					}
				}
			}
			if(player.GetComponent<PlayerController>().isShooting)
			{
				TargetPlayer();
			}
			else if((player.transform.position - transform.position).magnitude <= senseRange)
				{
					TargetPlayer();
				}
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject == reactor && targetingReactor)
		{
			targetingReactor = false;
			Sleep();
			transform.LookAt(new Vector3(reactor.transform.position.x, transform.position.y, reactor.transform.position.z));
			print("hi");
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

	void TargetPlayer()
	{
		targetingReactor = false;
		Wake();
		agent.destination = player.transform.position + Vector3.ClampMagnitude((transform.position - player.transform.position).normalized, 1);
	}
}
