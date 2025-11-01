using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Temp : MonoBehaviour
{
    [Header("Spawn Settings")]
    public SpawnPoint[] spawnPoints; // مصفوفة من الكلاس الجديد
    public bool randomSpawnPoints = true;
    public GameObject enemyPrefab;

    [Header("Difficulty Settings - Up to Level 5")]
    public float[] healthMultipliers = { 1.0f, 1.3f, 1.6f, 2.0f, 2.5f };
    public float[] damageMultipliers = { 1.0f, 1.2f, 1.4f, 1.7f, 2.0f };
    public float[] speedMultipliers = { 1.0f, 1.1f, 1.2f, 1.3f, 1.4f };
    public int[] enemyCounts = { 5, 8, 12, 15, 18 };

    [Header("Effects & Sounds")]
    public AudioClip waveStartSound;
    public AudioClip waveCompleteSound;
    private AudioSource audioSource;

    [Header("Current Wave Info")]
    public int currentWaveNumber = 1;
    public int currentWaveLevel = 1;
    public const int MAX_LEVEL = 5;

    private bool isSpawning = false;
    private int enemiesAlive = 0;

    [System.Serializable]
    public class SpawnPoint
    {
        public Transform pointTransform; // موقع السباون
        public ParticleSystem spawnParticle; // البارتكل الخاص بهذا السباون
        public bool isActive = true; // إذا كان السباون نشط
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        StartCoroutine(EndlessWaves());
    }

    IEnumerator EndlessWaves()
    {
        while (true)
        {
            Wave currentWave = CreateWave(currentWaveNumber);
            PlayWaveStartSound();
            yield return StartCoroutine(SpawnWave(currentWave));
            currentWaveNumber++;
            PlayWaveCompleteSound();
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

        // الحصول على نقطة سباون مع بارتكلها
        SpawnPoint selectedSpawnPoint = GetSpawnPoint();

        if (selectedSpawnPoint != null && selectedSpawnPoint.isActive)
        {
            Vector3 spawnPosition = selectedSpawnPoint.pointTransform.position;

            // تشغيل البارتكل الخاص بهذه النقطة
            PlaySpawnParticle(selectedSpawnPoint);

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            ApplyWaveDifficulty(enemy, waveLevel);
            RegisterEnemyDeathEvent(enemy);

            Debug.Log($"🎯 Spawned enemy at {selectedSpawnPoint.pointTransform.name}");
        }
    }

    SpawnPoint GetSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return null;

        List<SpawnPoint> activeSpawnPoints = new List<SpawnPoint>();

        // جمع جميع نقاط السباون النشطة
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.isActive && spawnPoint.pointTransform != null)
            {
                activeSpawnPoints.Add(spawnPoint);
            }
        }

        if (activeSpawnPoints.Count == 0)
            return null;

        if (randomSpawnPoints)
        {
            // اختيار عشوائي من النقاط النشطة
            return activeSpawnPoints[Random.Range(0, activeSpawnPoints.Count)];
        }
        else
        {
            // تناوب بين النقاط النشطة
            return activeSpawnPoints[enemiesAlive % activeSpawnPoints.Count];
        }
    }

    void ApplyWaveDifficulty(GameObject enemy, int waveLevel)
    {
        int levelIndex = Mathf.Min(waveLevel - 1, MAX_LEVEL - 1);

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

        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.attackDamage = Mathf.RoundToInt(enemyAI.attackDamage * damageMultipliers[levelIndex]);
            enemyAI.walkSpeed *= speedMultipliers[levelIndex];
            enemyAI.runSpeed *= speedMultipliers[levelIndex];

            if (waveLevel >= 3)
            {
                enemyAI.enableMerge = true;

                // تفعيل نقاط السباون الإضافية من الموجة 3
                for (int i = 2; i < spawnPoints.Length; i++)
                {
                    if (i < spawnPoints.Length)
                    {
                        spawnPoints[i].isActive = true;
                    }
                }

                if (waveLevel >= 4)
                {
                    enemyAI.mergeDamageBonus = 15;
                    enemyAI.mergeSizeMultiplier = 1.3f;
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

    // 🔥 تشغيل البارتكل الخاص بنقطة السباون
    void PlaySpawnParticle(SpawnPoint spawnPoint)
    {
        if (spawnPoint.spawnParticle != null)
        {
            // تشغيل البارتكل الموجود مسبقاً على نقطة السباون
            spawnPoint.spawnParticle.Play();
            Debug.Log($"✨ Playing particle for {spawnPoint.pointTransform.name}");
        }
    }

    void PlayWaveStartSound()
    {
        if (waveStartSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(waveStartSound);
        }
    }

    void PlayWaveCompleteSound()
    {
        if (waveCompleteSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(waveCompleteSound);
        }
    }

    public string GetSpawnPointsInfo()
    {
        int activeCount = 0;
        foreach (SpawnPoint sp in spawnPoints)
        {
            if (sp.isActive) activeCount++;
        }
        return $"Spawn Points: {activeCount}/{spawnPoints.Length} active";
    }

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
        return $"Wave: {currentWaveNumber} | Level: {currentWaveLevel}/5 | {GetSpawnPointsInfo()}";
    }
}