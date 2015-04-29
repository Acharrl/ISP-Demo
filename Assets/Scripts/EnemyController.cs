using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
	public Transform reactor;
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
		if(other.gameObject.tag == "Player")
		{
			Vector3 direction = other.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);
			if(angle < fovAngle / 2)
			{
				RaycastHit hit;
				if(Physics.Raycast(transform.position, direction.normalized, out hit, fovRange))
				{
					if(hit.collider.gameObject.tag == "Player")
					{
						agent.destination = other.gameObject.transform.position;
					}
				}
			}
		}
	}
}
