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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Hit");
            var healthComponent = other.GetComponentInParent<HealthComponent>(); // <- change here
            if (healthComponent != null)
            {
                healthComponent.TakeDamage(10);
            }
            Destroy(gameObject);
        }
    }
}