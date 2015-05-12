using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaveController : MonoBehaviour
{
	public GameObject enemy;
	public GameObject reactor;
	public GameObject player;
	public Text gameOverText;
	public int[] enemyCount;
	public float[] spawnDelay;
	public float[] waveTime;
	private int currentWave = 0;
	private float spawnTimer = 0;

	void Update()
	{
		if(waveTime[currentWave] <= 0)
		{
			if(currentWave + 1 < waveTime.Length)
			{
				currentWave++;
				spawnTimer = 0;
			}
		}
		if(spawnTimer <= 0 && enemyCount[currentWave] >= 1)
		{
			spawnTimer = spawnDelay[currentWave];
			enemyCount[currentWave] -= 1;
			Transform spawn = transform.GetChild((int)Random.Range(0, 3.999f));
			GameObject newEnemy = (GameObject)Instantiate(enemy, spawn.position, spawn.rotation);
			newEnemy.GetComponent<EnemyController>().reactor = reactor;
			newEnemy.GetComponent<EnemyController>().player = player;
			newEnemy.transform.SetParent(transform);
		}
		waveTime[currentWave] -= Time.deltaTime;
		spawnTimer -= Time.deltaTime;
		if(currentWave == waveTime.Length - 1 && enemyCount[currentWave] == 0 && transform.childCount == 4)
		{
			gameOverText.text = "You Win!";
		}
	}
}
