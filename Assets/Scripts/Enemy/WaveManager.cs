using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager
    : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public bool randomSpawnPoints = true;
    public GameObject enemyPrefab; // ضع برفاب العدو هنا

    [Header("Difficulty Settings - Up to Level 5")]
    public float[] healthMultipliers = { 1.0f, 1.3f, 1.6f, 2.0f, 2.5f };
    public float[] damageMultipliers = { 1.0f, 1.2f, 1.4f, 1.7f, 2.0f };
    public float[] speedMultipliers = { 1.0f, 1.1f, 1.2f, 1.3f, 1.4f };
    public int[] enemyCounts = { 5, 8, 12, 15, 18 };

    [Header("Current Wave Info")]
    public int currentWaveNumber = 1;
    public int currentWaveLevel = 1;
    public const int MAX_LEVEL = 5;

    private bool isSpawning = false;
    private int enemiesAlive = 0;

    void Start()
    {
        StartCoroutine(EndlessWaves());
    }

    IEnumerator EndlessWaves()
    {
        while (true)
        {
            Wave currentWave = CreateWave(currentWaveNumber);
            yield return StartCoroutine(SpawnWave(currentWave));

            currentWaveNumber++;
            yield return new WaitForSeconds(currentWave.timeBeforeNextWave);
        }
    }

    Wave CreateWave(int waveNumber)
    {
        int waveLevel = Mathf.Min(waveNumber, MAX_LEVEL);
        currentWaveLevel = waveLevel;
        int levelIndex = waveLevel - 1;

        return new Wave
        {
            waveName = "Wave " + waveNumber + " (Level " + waveLevel + ")",
            enemyCount = enemyCounts[levelIndex],
            spawnRate = 1f + (levelIndex * 0.2f),
            timeBeforeNextWave = 3f,
            waveLevel = waveLevel
        };
    }

    IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;
        Debug.Log($"🚀 STARTING {wave.waveName}");

        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave.waveLevel);
            enemiesAlive++;
            yield return new WaitForSeconds(1f / wave.spawnRate);
        }

        isSpawning = false;
        yield return new WaitUntil(() => enemiesAlive <= 0);
        Debug.Log($"✅ {wave.waveName} COMPLETED!");
    }

    void SpawnEnemy(int waveLevel)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("❌ No enemy prefab assigned!");
            return;
        }

        Vector3 spawnPosition = GetSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        ApplyWaveDifficulty(enemy, waveLevel);
        RegisterEnemyDeathEvent(enemy);
    }

    Vector3 GetSpawnPosition()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return transform.position;

        GameObject spawnPoint = spawnPoints[spawnPoints.Length -1].gameObject;

        if (randomSpawnPoints)
            spawnPoint.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        else
            spawnPoint.transform.position = spawnPoints[enemiesAlive % spawnPoints.Length].position;

        if (spawnPoint.activeSelf)
            return spawnPoint.transform.position;
        else
            return transform.position;
    }

    void ApplyWaveDifficulty(GameObject enemy, int waveLevel)
    {
        int levelIndex = Mathf.Min(waveLevel - 1, MAX_LEVEL - 1);

        // تعديل HealthComponent
        HealthComponent healthComponent = enemy.GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            try
            {
                var field = typeof(HealthComponent).GetField("maxHealth",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (field != null)
                {
                    float baseHealth = (float)field.GetValue(healthComponent);
                    float newHealth = baseHealth * healthMultipliers[levelIndex];
                    healthComponent.setMaxHealth(newHealth);
                    healthComponent.RenewHealth();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Health modification error: {e.Message}");
            }
        }

        // تعديل EnemyAI
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.attackDamage = Mathf.RoundToInt(enemyAI.attackDamage * damageMultipliers[levelIndex]);
            enemyAI.walkSpeed *= speedMultipliers[levelIndex];
            enemyAI.runSpeed *= speedMultipliers[levelIndex];

            if (waveLevel >= 2)
            {
                enemyAI.enableMerge = true;

                for (int spawnIndex = 3; spawnIndex < 3*2; spawnIndex++)
                {
                    spawnPoints[spawnIndex].gameObject.SetActive(true);
                }


                if (waveLevel >= 3)
                {
                    for (int spawnIndex = 3*2; spawnIndex < 3*3; spawnIndex++)
                    {
                        spawnPoints[spawnIndex].gameObject.SetActive(true);
                    }
                    enemyAI.mergeDamageBonus = 15;
                    enemyAI.mergeSizeMultiplier = 1.3f;
                }

                if (waveLevel >= 4)
                {
                    for (int spawnIndex = 3 * 3; spawnIndex < 3 * 4; spawnIndex++)
                    {
                        spawnPoints[spawnIndex].gameObject.SetActive(true);
                    }
                }

                    if (waveLevel >= 5)
                {
                    enemyAI.mergeDamageBonus = 20;
                    enemyAI.mergeSizeMultiplier = 1.5f;
                }
            }
        }
    }

    void RegisterEnemyDeathEvent(GameObject enemy)
    {
        HealthComponent healthComponent = enemy.GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            healthComponent.OnDeath += OnEnemyDeath;
        }
    }

    public void OnEnemyDeath()
    {
        enemiesAlive--;
    }

    // كلاس Wave الداخلي
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public int enemyCount;
        public float spawnRate;
        public float timeBeforeNextWave = 3f;
        public int waveLevel = 1;
    }

    public string GetCurrentWaveInfo()
    {
        return $"Wave: {currentWaveNumber} | Level: {currentWaveLevel}/5";
    }
}