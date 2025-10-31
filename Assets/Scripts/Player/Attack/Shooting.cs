using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    public GameObject projectilePrefab;

    public float TimeBetweenShots = 0.3333f;
    private float m_timeStamp = 0f;
    public float projectileSpeed = 10f;



    private PlayerInputActions playerInputActions;
    
  
    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
    }

    void FixedUpdate()
    {
        if ((Time.time >= m_timeStamp) && playerInputActions.Player.Fire.IsPressed())
        {
            
            Fire();
            m_timeStamp = Time.time + TimeBetweenShots;

        }

    }

    void Fire()
    {
       
       Debug.Log("Firing"); 

        Vector2 mouseXY = playerInputActions.Player.MouseMovement.ReadValue<Vector2>();


        Vector3 mousePos = Camera.main.ScreenToWorldPoint(
            new Vector3(mouseXY.x,
            mouseXY.y, Camera.main.transform.position.y));

        Vector3 direction = (mousePos - transform.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        projectile.transform.LookAt(mousePos);

        projectile.GetComponent<Rigidbody>().linearVelocity = direction * projectileSpeed;

        


    }
    





  

   
  
  
}
