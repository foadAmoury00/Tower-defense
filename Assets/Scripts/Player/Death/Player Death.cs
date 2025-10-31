using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Player died");
            Destroy(gameObject);
        }
    }
}
