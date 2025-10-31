using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    HealthComponent healthComponent;
    [SerializeField] float projectileDamage = 10f;


    float lifeTime = 5f;

    [SerializeField]
    
    string layerName = "Enemy";

    [SerializeField] float projectileDamage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            
            Destroy(gameObject);
            healthComponent = other.GetComponent<HealthComponent>();

            if (healthComponent != null)
            {
                healthComponent.TakeDamage(projectileDamage);
                healthComponent.TakeDamage(projectileDamage);
            }
        }
        else
        {
            Destroy(gameObject,lifeTime);
        }

    }

 
}
