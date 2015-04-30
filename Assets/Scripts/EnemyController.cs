using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
	public GameObject reactor;
	public GameObject player;
	public float fovAngle;
	public float fovRange;

	private NavMeshAgent agent;
	private bool targetingReactor;

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		targetingReactor = true;
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
						targetingReactor = false;
						Wake();
						agent.destination = player.transform.position;
					}
				}
			}
			if(player.GetComponent<PlayerController>().isShooting)
			{
				targetingReactor = false;
				Wake();
				agent.destination = player.transform.position;
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
}
