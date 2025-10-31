using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("=== NAVMESH SETTINGS ===")]
    private NavMeshAgent agent;
    public Transform targetTower;

    [Header("=== MOVEMENT SETTINGS ===")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 5f;
    public float rotationSpeed = 5f;

    [Header("=== COMBAT SETTINGS ===")]
    public float attackRange = 2f;
    public float detectionRange = 10f;
    public int attackDamage = 15;
    public float attackRate = 1f;
    public float nextAttackTime = 0f;

    [Header("=== MERGE SETTINGS ===")]
    public bool enableMerge = true;
    public int mergeDamageBonus = 10;
    public float mergeSizeMultiplier = 1.2f;
    public ParticleSystem mergeEffect;

    [Header("=== ANIMATION SETTINGS ===")]
    public Animator animator;
    private string currentState;

    // Animation States
    const string ENEMY_IDLE = "Idle";
    const string ENEMY_WALK = "Walk";
    const string ENEMY_RUN = "Run";
    const string ENEMY_ATTACK = "Attack";
    const string ENEMY_DEATH = "Death";

    [Header("=== SOUND SETTINGS ===")]
    public AudioSource audioSource;
    public AudioClip spawnSound;
    public AudioClip attackSound;
    public AudioClip deathSound;
    public AudioClip walkSound;

    [Header("=== VISUAL EFFECTS ===")]
    public ParticleSystem deathEffect;

    private bool isDead = false;

    private void Start()
    {
        InitializeEnemy();
    }

    void InitializeEnemy()
    {
        // تهيئة NavMesh Agent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        // إعدادات NavMesh Agent
        agent.speed = walkSpeed;
        agent.angularSpeed = 360f;
        agent.acceleration = 8f;
        agent.stoppingDistance = attackRange - 0.2f;
        agent.radius = 0.5f;
        agent.height = 2f;

        // البحث عن البرج الهدف
        if (targetTower == null)
        {
            FindTargetTower();
        }

        // تشغيل صوت الظهور
        PlaySound(spawnSound);

        // بدء الانيميشن
        ChangeAnimation(ENEMY_WALK);

        Debug.Log("Enemy initialized - Speed: " + agent.speed);
    }

    private void Update()
    {
        if (isDead) return;

        if (targetTower != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetTower.position);

            // تحديث حالة العدو بناءً على المسافة
            UpdateEnemyState(distanceToTarget);
        }
        else
        {
            // البحث عن برج جديد إذا فقد الهدف
            FindTargetTower();
            ChangeAnimation(ENEMY_IDLE);
        }
    }

    void UpdateEnemyState(float distanceToTarget)
    {
        if (distanceToTarget <= attackRange)
        {
            // في مدى الهجوم
            agent.isStopped = true;
            ChangeAnimation(ENEMY_ATTACK);

            // الهجوم
            if (Time.time >= nextAttackTime)
            {
                AttackTarget();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        else if (distanceToTarget <= detectionRange)
        {
            // في مدى الكشف - الركض نحو الهدف
            agent.isStopped = false;
            agent.speed = runSpeed;
            agent.SetDestination(targetTower.position);
            ChangeAnimation(ENEMY_RUN);
        }
        else
        {
            // خارج مدى الكشف - المشي العادي
            agent.isStopped = false;
            agent.speed = walkSpeed;
            agent.SetDestination(targetTower.position);
            ChangeAnimation(ENEMY_WALK);
        }
    }

    void AttackTarget()
    {
        Debug.Log("Enemy attacked tower!"); 
        // تشغيل انيميشن الهجوم
        ChangeAnimation(ENEMY_ATTACK);

        // تشغيل صوت الهجوم
        PlaySound(attackSound);

        // إلحاق الضرر بالبرج
        HealthComponent towerHealth = targetTower.GetComponent<HealthComponent>();

        if (towerHealth != null)
        {
            towerHealth.TakeDamage(attackDamage);
            Debug.Log("Enemy attacked tower for " + attackDamage + " damage!");
        }


    }


    public void Die()
    {
        isDead = true;

        // إيقاف NavMesh Agent
        if (agent != null)
            agent.isStopped = true;

        // تشغيل انيميشن الموت
        ChangeAnimation(ENEMY_DEATH);

        // تشغيل صوت الموت
        PlaySound(deathSound);

        // تأثيرات الموت
        if (deathEffect != null)
            deathEffect.Play();

        // تعطيل الكوليدر
        Collider collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = false;

        Debug.Log("Enemy died!");

        // تدمير الكائن بعد 3 ثواني
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        // الدمج مع اللاعب
        if (other.CompareTag("Player"))
        {
            MergeWithPlayer();
        }

        // الدمج مع الأعداء الآخرين
        if (enableMerge && other.CompareTag("Enemy"))
        {
            TryMergeWithEnemy(other.gameObject);
        }
    }

    void MergeWithPlayer()
    {
        // عندما يلمس اللاعب العدو - يموت العدو
        Die();
        Debug.Log("💥 اللاعب دمر العدو!");
    }

    void TryMergeWithEnemy(GameObject otherEnemyObj)
    {
        EnemyAI otherEnemy = otherEnemyObj.GetComponent<EnemyAI>();

        if (otherEnemy != null && !otherEnemy.isDead && otherEnemy.enableMerge)
        {
            StartCoroutine(MergeWithEnemy(otherEnemy));
        }
    }

    IEnumerator MergeWithEnemy(EnemyAI otherEnemy)
    {
        // منع كلا العدوين من الحركة أثناء الدمج
        agent.isStopped = true;
        otherEnemy.agent.isStopped = true;

        Debug.Log("🔄 بدء دمج عدوين!");

        // تأثيرات الدمج
        if (mergeEffect != null)
            Instantiate(mergeEffect, transform.position, Quaternion.identity);

        PlaySound(attackSound);

        // الانتظار قليلاً للتأثير البصري
        yield return new WaitForSeconds(0.5f);

        // تطبيق ترقيات الدمج على هذا العدو
        ApplyMergeUpgrades();

        // تدمير العدو الآخر
        otherEnemy.DestroyAfterMerge();

        // استئناف الحركة
        agent.isStopped = false;

        Debug.Log($"🎉 تم الدمج! الهجوم: {attackDamage}");
    }

    void ApplyMergeUpgrades()
    {
        // زيادة الضرر
        attackDamage += mergeDamageBonus;

        // تكبير الحجم
        transform.localScale *= mergeSizeMultiplier;

        // زيادة سرعة الحركة
        walkSpeed *= 1.1f;
        runSpeed *= 1.1f;
        agent.speed = walkSpeed;
    }

    public void DestroyAfterMerge()
    {
        // تعطيل المكونات قبل التدمير
        if (agent != null)
            agent.isStopped = true;

        // تعطيل الكوليدر
        Collider collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = false;

        // إخفاء الكائن
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.enabled = false;

        // تدمير الكائن
        Destroy(gameObject);
    }

    void ChangeAnimation(string newState)
    {
        if (animator == null) return;

        // إيقاف الانيميشن الحالي
        if (currentState == newState) return;

        // تشغيل الانيميشن الجديد
        animator.Play(newState);
        currentState = newState;
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void FindTargetTower()
    {
        GameObject tower = GameObject.FindGameObjectWithTag("Tower");
        if (tower != null)
        {
            targetTower = tower.transform;
            Debug.Log("Target tower found: " + tower.name);
        }
        else
        {
            Debug.LogWarning("No tower found with tag 'Tower'!");
        }
    }

    // رسم نطاقات الكشف والهجوم في المحرر
    void OnDrawGizmosSelected()
    {
        // نطاق الهجوم - أحمر
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // نطاق الكشف - أصفر
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}