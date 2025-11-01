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

        Vector2 mouseXY = playerInputActions.Player.MouseMovement.ReadValue<Vector2>();


        Ray ray = Camera.main.ScreenPointToRay(new Vector3(mouseXY.x, mouseXY.y, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 mousePos = hit.point;
            // Keep the projectile on the same Y level as the player, useful for 2.5D top-down
            mousePos.y = transform.position.y;

            Vector3 direction = (mousePos - transform.position).normalized;

            // ... rest of your firing code
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.transform.LookAt(mousePos); // Use LookAt the target point
            projectile.GetComponent<Rigidbody>().linearVelocity = direction * projectileSpeed;
        }


    }











}
