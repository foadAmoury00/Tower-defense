
using UnityEngine;

public class Projectile : MonoBehaviour
{

    HealthComponent healthComponent;
    [SerializeField] float projectileDamage = 10f;


    float lifeTime = 5f;

    [SerializeField]
    
    string layerName = "Enemy";


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(layerName))
        
        {
            HealthComponent healthComponent = new HealthComponent();
            if (other.transform.childCount > 0)
            {
                healthComponent = other.GetComponentInParent<HealthComponent>();
            }
            else 
            {
                healthComponent = other.GetComponent<HealthComponent>();

            }
            // works if collider is on child




            if (healthComponent != null)
            {
                healthComponent.TakeDamage(projectileDamage);
            }
            Destroy(gameObject); 
        }
        else
        {
            Destroy(gameObject, lifeTime);
        }
    }
}