//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;


//public class AutoAttack : MonoBehaviour
//{


//    public GameObject projectilePrefab;
//    public float attackRange = 10f;
//    public float timeBetweenShots = 2f;

//    private float shootTimer;
//    private Transform currentTarget;

//    public float projectileSpeed = 10f;

//    void Update()
//    {
//        FindTarget();

//        shootTimer -= Time.deltaTime;
//        if (currentTarget != null && shootTimer <= 0)
//        {
//            ShootTarget();
//            shootTimer = timeBetweenShots;
//        }
//    }

//    void FindTarget()
//    {
//        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));
//        if (hitColliders.Length > 0)
//        {
//            currentTarget = hitColliders[0].transform;
//        }
//        else
//        {
//            currentTarget = null;
//        }
//    }

//    void ShootTarget()
//    {
//        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);

//        Rigidbody projectRB = projectile.GetComponent<Rigidbody>();

//        projectile.GetComponent<Rigidbody>().linearVelocity = (currentTarget.position - transform.position).normalized * projectileSpeed;

//        projectile.transform.LookAt(currentTarget);
//    }
//    // making the enemy script invoke this method when in range
//    void AttackEnemy()
//    {
//        Debug.Log("Attacking Enemy");
//    }

//    void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, attackRange);
//    }
//}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float attackRange = 10f;
    public float timeBetweenShots = 2f;
    public float projectileSpeed = 10f;

    [Header("Input")]
    public bool fireOnClick = false; // true for player, false for defenders

    [Header("Targeting")]
    public LayerMask enemyMask;  // NEW (set this in Inspector to "Enemy")

    private float shootTimer;
    private Transform currentTarget;

    void Update()
    {
        FindTarget();

        shootTimer -= Time.deltaTime;

        if (fireOnClick)
        {
            if (Input.GetMouseButtonDown(1) && currentTarget != null && shootTimer <= 0f)
            {
                ShootTarget();
                shootTimer = timeBetweenShots;
            }
        }
        else
        {
            if (currentTarget != null && shootTimer <= 0f)
            {
                ShootTarget();
                shootTimer = timeBetweenShots;
            }
        }
    }

    void FindTarget()
    {
        // if you don�t set enemyMask in the Inspector, this falls back to layer name "Enemy"
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int mask = enemyMask.value != 0 ? enemyMask.value : LayerMask.GetMask("Enemy");

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, mask);
        currentTarget = (hits.Length > 0) ? hits[0].transform : null;
    }

    void ShootTarget()
    {
        if (!projectilePrefab || currentTarget == null) return;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = (currentTarget.position - transform.position).normalized * projectileSpeed; // FIXED
        }

        projectile.transform.LookAt(currentTarget);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
