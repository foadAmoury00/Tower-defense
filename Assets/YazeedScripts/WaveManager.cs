using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public GameObject enemyPrefab;
        public int enemyCount;
        public float spawnRate;
        public float timeBeforeNextWave = 5f;
    }

    [Header("Wave Settings")]
    public Wave[] waves;
    public Transform spawnPoint;
    public Transform targetTower;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private int enemiesAlive = 0;

    void Start()
    {
        StartCoroutine(StartWaves());
    }

    IEnumerator StartWaves()
    {
        while (currentWaveIndex < waves.Length)
        {
            yield return StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            currentWaveIndex++;

            // «·«‰ Ÿ«— ﬁ»· «·„ÊÃ… «· «·Ì…
            if (currentWaveIndex < waves.Length)
            {
                Debug.Log("Next wave in: " + waves[currentWaveIndex].timeBeforeNextWave + " seconds");
                yield return new WaitForSeconds(waves[currentWaveIndex].timeBeforeNextWave);
            }
        }

        Debug.Log("All waves completed!");
    }

    IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;
        Debug.Log("Starting wave: " + wave.waveName);

        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave.enemyPrefab);
            enemiesAlive++;
            yield return new WaitForSeconds(1f / wave.spawnRate);
        }

        isSpawning = false;

        // «·«‰ Ÿ«— Õ Ï  ‰ ÂÌ «·„ÊÃ… «·Õ«·Ì…
        yield return new WaitUntil(() => enemiesAlive <= 0);
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        //  ⁄ÌÌ‰ «·Âœ› ··⁄œÊ
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null && targetTower != null)
        {
            enemyAI.targetTower = targetTower;
        }
    }

    // «” œ⁄ Â–Â «·œ«·… ⁄‰œ„« Ì„Ê  ⁄œÊ
    public void OnEnemyDeath()
    {
        enemiesAlive--;
        Debug.Log("Enemy died! Enemies alive: " + enemiesAlive);
    }
}