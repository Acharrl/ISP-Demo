using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaveController : MonoBehaviour
{
	public GameObject[] enemy;
	public GameObject reactor;
	public GameObject player;
	public Text gameOverText;
	public Text scoreCounter;
	private int score = 0;
	public int[] enemyCount;
	public float[] spawnDelay;
	public float[] waveTime;
	public float[] enemyProportion;
	private int currentWave = 0;
	private float spawnTimer = 0;
	private float waveTextTimer = 0;

	void Start()
	{
		WaveText();
	}

	void Update()
	{
		if(waveTime[currentWave] <= 0)
		{
			if(currentWave + 1 < waveTime.Length)
			{
				currentWave++;
				spawnTimer = 0;
				WaveText();
			}
		}
		if(spawnTimer <= 0 && enemyCount[currentWave] >= 1)
		{
			spawnTimer = spawnDelay[currentWave];
			enemyCount[currentWave] -= 1;
			Transform spawn = transform.GetChild((int)Random.Range(0, 3.999f));
			float seed = Random.value;
			int enemyNumber = 0;
			for(int i = 0; i < enemyProportion.Length; i++)
			{
				seed -= enemyProportion[i];
				if(seed <= 0)
				{
					enemyNumber = i;
					break;
				}
			}
			GameObject newEnemy = (GameObject)Instantiate(enemy[enemyNumber], spawn.position, spawn.rotation);
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

		if(player.GetComponent<PlayerController>().isShooting)
		{
			for(int i = 4; i < transform.childCount; i++)
			{
				transform.GetChild(i).GetComponent<EnemyController>().TargetPlayer();
			}
		}
		waveTextTimer -= Time.deltaTime;
		if(waveTextTimer <= 0 & waveTextTimer > -5)
		{
			gameOverText.text = "";
			waveTextTimer = -10;
		}
	}

	public void IncrementScore()
	{
		score++;
		scoreCounter.text = "Enemies Slain: " + score;
	}

	private void WaveText()
	{
		gameOverText.text = "Wave " + (currentWave + 1).ToString();
		waveTextTimer = 5;
	}
}
