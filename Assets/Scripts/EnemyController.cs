using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
	public Transform goal;

	private NavMeshAgent agent;

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		agent.destination = goal.position;
	}
}
