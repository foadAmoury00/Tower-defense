using UnityEngine;

public class Death : MonoBehaviour
{
    public HealthComponent healthComponent;


    private void OnEnable()
    {
        healthComponent.OnDeath += Die;
    }

    public void Die()
    {
        Destroy(gameObject);
        
    }

    private void OnDisable()
    {
        healthComponent.OnDeath -= Die;
    }
}
