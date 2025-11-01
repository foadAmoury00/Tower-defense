#region Commented Enemy Controller
//using UnityEngine;
//using UnityEngine.AI;
//using System.Collections;

//public class EnemyAI : MonoBehaviour
//{
//    [Header("=== NAVMESH SETTINGS ===")]
//    private NavMeshAgent agent;
//    public Transform targetTower;

//    [Header("=== MOVEMENT SETTINGS ===")]
//    public float walkSpeed = 3.5f;
//    public float runSpeed = 5f;
//    public float rotationSpeed = 5f;

//    [Header("=== COMBAT SETTINGS ===")]
//    public float attackRange = 2f;
//    public float detectionRange = 10f;
//    public int attackDamage = 15;
//    public float attackRate = 1f;
//    public float nextAttackTime = 0f;

//    [Header("=== MERGE SETTINGS ===")]
//    public bool enableMerge = true;
//    public int mergeDamageBonus = 10;
//    public float mergeSizeMultiplier = 1.2f;
//    public ParticleSystem mergeEffect;

//    [Header("=== ANIMATION SETTINGS ===")]
//    public Animator animator;
//    private string currentState;

//    // Animation States
//    const string ENEMY_IDLE = "Idle";
//    const string ENEMY_WALK = "Walk";
//    const string ENEMY_RUN = "Run";
//    const string ENEMY_ATTACK = "Attack";
//    const string ENEMY_DEATH = "Death";

//    [Header("=== SOUND SETTINGS ===")]
//    public AudioSource audioSource;
//    public AudioClip spawnSound;
//    public AudioClip attackSound;
//    public AudioClip deathSound;
//    public AudioClip walkSound;

//    [Header("=== VISUAL EFFECTS ===")]
//    public ParticleSystem deathEffect;

//    private bool isDead = false;

//    private void Start()
//    {
//        InitializeEnemy();
//    }

//    void InitializeEnemy()
//    {
//        // تهيئة NavMesh Agent
//        agent = GetComponent<NavMeshAgent>();
//        if (agent == null)
//        {
//            agent = gameObject.AddComponent<NavMeshAgent>();
//        }

//        // إعدادات NavMesh Agent
//        agent.speed = walkSpeed;
//        agent.angularSpeed = 360f;
//        agent.acceleration = 8f;
//        agent.stoppingDistance = attackRange - 0.2f;
//        agent.radius = 0.5f;
//        agent.height = 2f;

//        // البحث عن البرج الهدف
//        if (targetTower == null)
//        {
//            FindTargetTower();
//        }

//        // تشغيل صوت الظهور
//        PlaySound(spawnSound);

//        // بدء الانيميشن
//        ChangeAnimation(ENEMY_WALK);

//        Debug.Log("Enemy initialized - Speed: " + agent.speed);
//    }

//    private void Update()
//    {
//        if (isDead) return;

//        if (targetTower != null)
//        {
//            float distanceToTarget = Vector3.Distance(transform.position, targetTower.position);

//            // تحديث حالة العدو بناءً على المسافة
//            UpdateEnemyState(distanceToTarget);
//        }
//        else
//        {
//            // البحث عن برج جديد إذا فقد الهدف
//            FindTargetTower();
//            ChangeAnimation(ENEMY_IDLE);
//        }
//    }

//    void UpdateEnemyState(float distanceToTarget)
//    {
//        if (distanceToTarget <= attackRange)
//        {
//            // في مدى الهجوم
//            agent.isStopped = true;
//            ChangeAnimation(ENEMY_ATTACK);

//            // الهجوم
//            if (Time.time >= nextAttackTime)
//            {
//                AttackTarget();
//                nextAttackTime = Time.time + 1f / attackRate;
//            }
//        }
//        else if (distanceToTarget <= detectionRange)
//        {
//            // في مدى الكشف - الركض نحو الهدف
//            agent.isStopped = false;
//            agent.speed = runSpeed;
//            agent.SetDestination(targetTower.position);
//            ChangeAnimation(ENEMY_RUN);
//        }
//        else
//        {
//            // خارج مدى الكشف - المشي العادي
//            agent.isStopped = false;
//            agent.speed = walkSpeed;
//            agent.SetDestination(targetTower.position);
//            ChangeAnimation(ENEMY_WALK);
//        }
//    }

//    void AttackTarget()
//    {
//        Debug.Log("Enemy attacked tower!"); 
//        // تشغيل انيميشن الهجوم
//        ChangeAnimation(ENEMY_ATTACK);

//        // تشغيل صوت الهجوم
//        PlaySound(attackSound);

//        // إلحاق الضرر بالبرج
//        HealthComponent towerHealth = targetTower.GetComponent<HealthComponent>();

//        if (towerHealth != null)
//        {
//            towerHealth.TakeDamage(attackDamage);
//            Debug.Log("Enemy attacked tower for " + attackDamage + " damage!");
//        }


//    }


//    public void Die()
//    {
//        isDead = true;

//        // إيقاف NavMesh Agent
//        if (agent != null)
//            agent.isStopped = true;

//        // تشغيل انيميشن الموت
//        ChangeAnimation(ENEMY_DEATH);

//        // تشغيل صوت الموت
//        PlaySound(deathSound);

//        // تأثيرات الموت
//        if (deathEffect != null)
//            deathEffect.Play();

//        // تعطيل الكوليدر
//        Collider collider = GetComponent<Collider>();
//        if (collider != null)
//            collider.enabled = false;

//        Debug.Log("Enemy died!");

//        // تدمير الكائن بعد 3 ثواني
//        Destroy(gameObject, 3f);
//    }

//    void OnTriggerEnter(Collider other)
//    {
//        if (isDead) return;

//        // الدمج مع اللاعب
//        if (other.CompareTag("Player"))
//        {
//            MergeWithPlayer();
//        }

//        // الدمج مع الأعداء الآخرين
//        if (enableMerge && other.CompareTag("Enemy"))
//        {
//            TryMergeWithEnemy(other.gameObject);
//        }
//    }

//    void MergeWithPlayer()
//    {
//        // عندما يلمس اللاعب العدو - يموت العدو
//        Die();
//        Debug.Log("💥 اللاعب دمر العدو!");
//    }

//    void TryMergeWithEnemy(GameObject otherEnemyObj)
//    {
//        EnemyAI otherEnemy = otherEnemyObj.GetComponent<EnemyAI>();

//        if (otherEnemy != null && !otherEnemy.isDead && otherEnemy.enableMerge)
//        {
//            StartCoroutine(MergeWithEnemy(otherEnemy));
//        }
//    }

//    IEnumerator MergeWithEnemy(EnemyAI otherEnemy)
//    {
//        // منع كلا العدوين من الحركة أثناء الدمج
//        agent.isStopped = true;
//        otherEnemy.agent.isStopped = true;

//        Debug.Log("🔄 بدء دمج عدوين!");

//        // تأثيرات الدمج
//        if (mergeEffect != null)
//            Instantiate(mergeEffect, transform.position, Quaternion.identity);

//        PlaySound(attackSound);

//        // الانتظار قليلاً للتأثير البصري
//        yield return new WaitForSeconds(0.5f);

//        // تطبيق ترقيات الدمج على هذا العدو
//        ApplyMergeUpgrades();

//        // تدمير العدو الآخر
//        otherEnemy.DestroyAfterMerge();

//        // استئناف الحركة
//        agent.isStopped = false;

//        Debug.Log($"🎉 تم الدمج! الهجوم: {attackDamage}");
//    }

//    void ApplyMergeUpgrades()
//    {
//        // زيادة الضرر
//        attackDamage += mergeDamageBonus;

//        // تكبير الحجم
//        transform.localScale *= mergeSizeMultiplier;

//        // زيادة سرعة الحركة
//        walkSpeed *= 1.1f;
//        runSpeed *= 1.1f;
//        agent.speed = walkSpeed;
//    }

//    public void DestroyAfterMerge()
//    {
//        // تعطيل المكونات قبل التدمير
//        if (agent != null)
//            agent.isStopped = true;

//        // تعطيل الكوليدر
//        Collider collider = GetComponent<Collider>();
//        if (collider != null)
//            collider.enabled = false;

//        // إخفاء الكائن
//        MeshRenderer renderer = GetComponent<MeshRenderer>();
//        if (renderer != null)
//            renderer.enabled = false;

//        // تدمير الكائن
//        Destroy(gameObject);
//    }

//    void ChangeAnimation(string newState)
//    {
//        if (animator == null) return;

//        // إيقاف الانيميشن الحالي
//        if (currentState == newState) return;

//        // تشغيل الانيميشن الجديد
//        animator.Play(newState);
//        currentState = newState;
//    }

//    void PlaySound(AudioClip clip)
//    {
//        if (audioSource != null && clip != null)
//        {
//            audioSource.PlayOneShot(clip);
//        }
//    }

//    void FindTargetTower()
//    {
//        GameObject tower = GameObject.FindGameObjectWithTag("Tower");
//        if (tower != null)
//        {
//            targetTower = tower.transform;
//            Debug.Log("Target tower found: " + tower.name);
//        }
//        else
//        {
//            Debug.LogWarning("No tower found with tag 'Tower'!");
//        }
//    }

//    // رسم نطاقات الكشف والهجوم في المحرر
//    void OnDrawGizmosSelected()
//    {
//        // نطاق الهجوم - أحمر
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, attackRange);

//        // نطاق الكشف - أصفر
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireSphere(transform.position, detectionRange);
//    }
//}using UnityEngine;
#endregion



using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("=== TARGET ===")]
    public Transform targetTower;

    [Header("=== MOVEMENT ===")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 5f;
    public float detectionRange = 10f;

    [Header("=== COMBAT ===")]
    public float attackRange = 2f;
    public int attackDamage = 15;
    public float attackRate = 1f;

    [Header("=== ANIMATION ===")]
    public Animator animator;
    const string ENEMY_IDLE = "Idle";
    const string ENEMY_WALK = "Walk";
    const string ENEMY_RUN = "Run";
    const string ENEMY_ATTACK = "Attack";

    [Header("=== AUDIO/VFX (optional) ===")]
    public AudioSource audioSource;
    public AudioClip spawnSound, attackSound;

    [Header("=== DEFENDER MELEE (on conversion) ===")]
    public string allyLayerName = "Ally";
    public string enemyLayerName = "Enemy";
    public float defenderRadius = 3.5f;
    public float defenderDamage = 10f;
    public float defenderHitInterval = 0.6f;
    public LayerMask defenderEnemyMask;   // set to Enemy in Inspector (or leave empty)


    [Header("=== MERGE SETTINGS ===")]
    public bool enableMerge = true;
    public int mergeDamageBonus = 10;
    public float mergeSizeMultiplier = 1.2f;
    public ParticleSystem mergeEffect;


    // --- internals ---
    private NavMeshAgent agent;
    private HealthComponent health;
    private float nextAttackTime = 0f;
    private bool converted = false;


    //detecting target either its enemy or castle
    float dist;
    GameObject target;



    void Start()
    {
        // NavMesh
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        agent.angularSpeed = 360f;
        agent.acceleration = 8f;
        agent.stoppingDistance = Mathf.Max(0.05f, attackRange - 0.2f);

        // Health
        health = GetComponent<HealthComponent>();
        if (health != null)
        {
            // IMPORTANT: do not auto-destroy so we can convert this instance
            health.SetDestroyOnDeath(false);
            health.OnDeath += HandleDeath;
        }

        // Find tower if not assigned
        if (!targetTower)
        {
            var tower = GameObject.FindWithTag("Tower");
            if (tower) targetTower = tower.transform;
        }

        PlaySound(spawnSound);
        ChangeAnimation(ENEMY_WALK);
    }

    void Update()
    {
        if (converted) return;      // once converted, this AI is disabled

        if (!targetTower)
        {
            ChangeAnimation(ENEMY_IDLE);
            return;
        }

        DetectTarget();

        if (dist <= attackRange)
        {
            Debug.Log("🔎 In distance of the attack range...");

            agent.isStopped = true;
            ChangeAnimation(ENEMY_ATTACK);

            if (Time.time >= nextAttackTime)
            {
                
                    AttackTarget(targetTower.gameObject);
                

                nextAttackTime = Time.time + 1f / Mathf.Max(0.0001f, attackRate);
            }
        }
        else
        {
            agent.isStopped = false;
            agent.speed = (dist <= detectionRange) ? runSpeed : walkSpeed;
            agent.SetDestination(targetTower.position);
            ChangeAnimation((dist <= detectionRange) ? ENEMY_RUN : ENEMY_WALK);
        }
    }

    private void DetectTarget()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            dist = Vector3.Distance(transform.position, targetTower.position);
        }
        
    }

    void AttackTarget(GameObject target)
    {
        PlaySound(attackSound);

        // Damage HealthComponent or TowerHealth (you have both in your project)
        var hc = targetTower.GetComponent<HealthComponent>();
        if (hc) { hc.TakeDamage(attackDamage); return; }

   
    }

   

    // If player touches enemy -> kill via HealthComponent so OnDeath fires and conversion happens
    void OnTriggerEnter(Collider other)
    {
        if (converted) return;

        if (other.CompareTag("Player"))
        {
            if (health != null) health.TakeDamage(999999f);
        }
    }

    // --- conversion path ---
    private void HandleDeath()
    {
        // Wave bookkeeping
        var wm = FindFirstObjectByType<WaveManager>();
        if (wm != null) wm.OnEnemyDeath();

        if (converted) return;

        // Recruit-cap check
        bool canRecruit = RecruitManager.Instance == null || RecruitManager.Instance.TryReserveSlot();
        if (!canRecruit)
        {
            if (health != null) health.SetDestroyOnDeath(true);
            Destroy(gameObject);
            return;
        }

        converted = true;

        // Stop Nav/AI
        if (agent)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.enabled = false;
        }

        // Switch to Ally layer
        int allyLayer = LayerMask.NameToLayer(allyLayerName);
        if (allyLayer >= 0) gameObject.layer = allyLayer;
        gameObject.tag = "Untagged";

        // Stop root motion & idle pose
        if (animator)
        {
            animator.applyRootMotion = false;
            ChangeAnimation(ENEMY_IDLE);
        }

        // Make it a standing, non-sinking turret
        EnsureStandingComponents();
        SnapToGround();

        // Make ally indestructible (optional)
        if (health) health.enabled = false;

        // --- attach/configure melee AOE defender (no projectiles) ---
        var melee = GetComponent<DefenderMelee>();
        if (!melee) melee = gameObject.AddComponent<DefenderMelee>();

        int mask = (defenderEnemyMask.value != 0)
            ? defenderEnemyMask.value
            : LayerMask.GetMask(enemyLayerName);



        melee.enemyMask = mask;
        melee.radius = defenderRadius;
        melee.damagePerHit = defenderDamage;
        melee.timeBetweenHits = defenderHitInterval;

        // Disable this AI script (converted)
        enabled = false;

        Debug.Log("⚔️ Enemy converted into ally melee defender (AOE).");
    }

    // --- helpers ---
    void ChangeAnimation(string state)
    {
        if (!animator) return;
        animator.CrossFade(state, 0.1f);
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource && clip) audioSource.PlayOneShot(clip);
    }

    void OnDisable()
    {
        if (health != null) health.OnDeath -= HandleDeath;
    }

    // If someone else calls Die(), unify through HealthComponent
    public void Die()
    {
        if (health != null) health.TakeDamage(999999f);
        else Destroy(gameObject);
    }
    private void EnsureStandingComponents()
    {
        // Rigidbody: make it stay put and not sink
        if (!TryGetComponent<Rigidbody>(out var rb))
            rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Collider: ensure there is a solid collider so it rests on ground
        var col = GetComponent<Collider>();
        if (col == null)
        {
            var cap = gameObject.AddComponent<CapsuleCollider>();
            cap.height = 2f;
            cap.radius = 0.4f;
            cap.center = new Vector3(0f, 1f, 0f);
            cap.isTrigger = false;
        }
        else
        {
            col.isTrigger = false; // must be solid to stand on ground
        }

        // NavMeshObstacle so other agents avoid this ally
        var obs = GetComponent<NavMeshObstacle>();
        if (obs == null) obs = gameObject.AddComponent<NavMeshObstacle>();
        obs.shape = NavMeshObstacleShape.Capsule;
        obs.carving = true;

        // Optional: make ally indestructible (we already converted it)
        if (health) health.enabled = false;
    }

    private void SnapToGround()
    {
        // First try a physics ray down
        Vector3 start = transform.position + Vector3.up * 3f;
        if (Physics.Raycast(start, Vector3.down, out var hit, 10f, ~0, QueryTriggerInteraction.Ignore))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            return;
        }

        // Fallback: try NavMesh position (if present)
        if (NavMesh.SamplePosition(transform.position, out var nHit, 2f, NavMesh.AllAreas))
        {
            transform.position = nHit.position;
        }
    }

 
}
