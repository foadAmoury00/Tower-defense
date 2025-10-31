using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AutoAttack : MonoBehaviour
{
    
 
    public GameObject projectilePrefab;
    public float attackRange = 10f;
    public float timeBetweenShots = 2f;

    private float shootTimer;
    private Transform currentTarget;

    public float projectileSpeed = 10f;

    void Update()
    {
        FindTarget();

        shootTimer -= Time.deltaTime;
        if (currentTarget != null && shootTimer <= 0)
        {
            ShootTarget();
            shootTimer = timeBetweenShots;
        }
    }

    void FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));
        if (hitColliders.Length > 0)
        {
            currentTarget = hitColliders[0].transform;
        }
        else
        {
            currentTarget = null;
        }
    }

    void ShootTarget()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);

        Rigidbody projectRB = projectile.GetComponent<Rigidbody>();

        projectile.GetComponent<Rigidbody>().linearVelocity = (currentTarget.position - transform.position).normalized * projectileSpeed;

        projectile.transform.LookAt(currentTarget);
    }
    // making the enemy script invoke this method when in range
    void AttackEnemy()
    {
        Debug.Log("Attacking Enemy");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

    

