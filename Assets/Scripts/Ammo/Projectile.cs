using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    HealthComponent healthComponent;
    [SerializeField] float projectileDamage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Hit");
            Destroy(gameObject);
            healthComponent = other.GetComponent<HealthComponent>();

            if (healthComponent != null)
            {
                healthComponent.TakeDamage(projectileDamage);
            }
        }

    }

 
}
