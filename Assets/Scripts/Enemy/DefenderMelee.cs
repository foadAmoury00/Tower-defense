using System.Linq;
using UnityEngine;

public class DefenderMelee : MonoBehaviour
{
    [Header("Melee / AOE")]
    public float radius = 3.5f;
    public float damagePerHit = 10f;
    public float timeBetweenHits = 0.6f;
    public LayerMask enemyMask; // set to "Enemy" in Inspector (or we set it from EnemyAI)

    private float timer;

    void Start()
    {
        name = "Ally(Clone)";
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Hit();
            timer = timeBetweenHits;
        }
    }

    private void Hit()
    {
        int mask =  LayerMask.GetMask("Enemy");

        // include trigger colliders (many enemies use triggers)
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            radius,
            mask,
            QueryTriggerInteraction.Collide
        );

        Debug.Log("enemies ally is attacking "+hits.Length);
        for (int i = 0; i < hits.Length; i++)
        {
            Debug.Log("enemies ally is attacking "+hits[i].name);
            var hc = hits[i].GetComponentInParent<HealthComponent>();
            
            if (hc != null)
            {
                hc.TakeDamage(damagePerHit);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
