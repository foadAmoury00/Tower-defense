using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("PlayerDeath script is active");
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Player died");
            Destroy(gameObject);
        }
       
    }
}
