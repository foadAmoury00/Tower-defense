using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    public GameObject projectilePrefab;

    public float TimeBetweenShots = 0.3333f;
    private float m_timeStamp = 0f;
    public float projectileSpeed = 10f;

    public PlayerInput playerInput;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    void FixedUpdate()
    {
        if ((Time.time >= m_timeStamp) && playerInputActions.Player.Fire.triggered)
        {
            Fire();
            m_timeStamp = Time.time + TimeBetweenShots;
        }
    }

    void Fire()
    {
        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        projectile.GetComponent<Rigidbody>().linearVelocity = projectile.transform.forward * projectileSpeed;

        

    }
    





  

   
  
  
}
