using UnityEngine;

public class PlayerDeath : MonoBehaviour
{

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Player died");
            Destroy(gameObject);
        }
    }

  
}
