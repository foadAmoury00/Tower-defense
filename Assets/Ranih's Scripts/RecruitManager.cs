using UnityEngine;

public class RecruitManager : MonoBehaviour
{
    public static RecruitManager Instance { get; private set; }

    [Header("Recruitment")]
    [Tooltip("Starting max number of recruited defenders.")]
    public int maxAllies = 3;
    [SerializeField] private int currentAllies = 0;

    [Header("Defender Prefab")]
    [Tooltip("Prefab that stays static and auto-shoots enemies.")]
    public GameObject defenderPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public bool TryRecruitFromEnemy(EnemyAI deadEnemy)
    {
        if (currentAllies >= maxAllies || defenderPrefab == null || deadEnemy == null) return false;

        // Spawn at exact death spot, preserve Y-rotation so it faces roughly the same direction
        Vector3 pos = deadEnemy.transform.position;
        Quaternion rot = Quaternion.Euler(0f, deadEnemy.transform.eulerAngles.y, 0f);

        GameObject defender = Instantiate(defenderPrefab, pos, rot);

        // Optional: copy some stats from the enemy
        var turret = defender.GetComponent<DefenderTurret>();
        if (turret != null)
        {
            turret.attackRange = Mathf.Max(turret.attackRange, deadEnemy.detectionRange * 0.6f);
            turret.timeBetweenShots = Mathf.Max(0.2f, 1f / Mathf.Max(0.001f, deadEnemy.attackRate)); // inverse of enemy rate
            turret.projectileDamage = Mathf.Max(1, deadEnemy.attackDamage); // reuse your projectile damage system
        }

        currentAllies++;
        return true;
    }

    public void IncreaseCap(int amount)
    {
        maxAllies = Mathf.Max(0, maxAllies + amount);
    }

    // Use if you ever want to remove a defender at runtime
    public void OnDefenderDestroyed()
    {
        currentAllies = Mathf.Max(0, currentAllies - 1);
    }
}
