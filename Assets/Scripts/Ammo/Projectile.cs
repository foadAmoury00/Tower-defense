//using Unity.VisualScripting;
//using UnityEngine;

//public class Projectile : MonoBehaviour
//{

//    HealthComponent healthComponent;

//    float lifeTime = 5f;

//    [SerializeField]

//    string layerName = "Enemy";

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
//        {
//            Debug.Log("Hit");
//            Destroy(gameObject);
//            healthComponent = other.GetComponent<HealthComponent>();

//            if (healthComponent != null)
//            {
//                healthComponent.TakeDamage(10);
//            }
//        }
//        else
//        {
//            Destroy(gameObject,lifeTime);
//        }

//    }


//}
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            var healthComponent = other.GetComponentInParent<HealthComponent>(); // works if collider is on child
            if (healthComponent != null)
            {
                healthComponent.TakeDamage(projectileDamage);
                healthComponent.TakeDamage(projectileDamage);
            }
            Destroy(gameObject); // destroy after applying damage
        }
    }
}