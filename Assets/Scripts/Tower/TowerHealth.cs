using UnityEngine;

public class TowerHealth : MonoBehaviour
{
    public int maxHealth = 200;
    public int currentHealth;
    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        Debug.Log("Tower took " + damage + " damage! Remaining: " + currentHealth);

        if (currentHealth <= 0)
        {
            TowerDestroyed();
        }
    }

    void TowerDestroyed()
    {
        Debug.Log("Tower has been destroyed!");
        // ÅÖÇÝÉ ÊÃËíÑÇÊ ÇáÊÏãíÑ åäÇ
        gameObject.SetActive(false);
    }
}