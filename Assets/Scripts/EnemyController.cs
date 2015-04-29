using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
	public Transform reactor;
	public GameObject player;
	public float fovAngle;
	public float fovRange;

	private NavMeshAgent agent;

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		agent.destination = reactor.position;
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
						agent.destination = player.transform.position;
					}
				}
			}
			else if(player.GetComponent<PlayerController>().isShooting)
			{
				agent.destination = player.transform.position;
			}
		}
	}
}
