//using UnityEngine;
//using System;

//public class HealthComponent : MonoBehaviour, IDamageable
//{
//    private float currentHealth;
//    [SerializeField] float maxHealth;
//    private bool isDead;

//    public event Action OnDeath;

//    private void Awake()
//    {
//        isDead = false;

//        currentHealth = maxHealth;
//    }



//    public void RenewHealth()
//    {
//        isDead = false;
//        currentHealth = maxHealth;
//    }

//    public void setMaxHealth(float maxHealth)
//    {
//        this.maxHealth = maxHealth;
//    }

//    public void TakeDamage(float damage)
//    {
//        if (isDead) return;

//        currentHealth -= damage;

//        if (currentHealth <= 0)
//        {
//            Die();
//        }
//    }




//    private void Die()
//    {
//        if (isDead) return; // Prevent multiple death calls

//        isDead = true;

//        if (OnDeath != null) OnDeath?.Invoke();

//        Destroy(gameObject);

//    }
//}using UnityEngine;
using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamageable
{
    public CastleBar_UI castleBarUI;  // Reference to CastleBar_UI
    private float currentHealth;
    [SerializeField] float maxHealth;
    private bool isDead;

    [Header("Death Behavior")]
    [SerializeField] private bool destroyOnDeath = true;   // NEW

    public event Action OnDeath;

    private void Awake()
    {
        isDead = false;
        currentHealth = maxHealth;
        if (castleBarUI == null)
            castleBarUI = GameObject.FindGameObjectWithTag("CastleUI").GetComponent<CastleBar_UI>();
    }

    public void SetDestroyOnDeath(bool value)   // NEW (so EnemyAI can switch behavior)
    {
        destroyOnDeath = value;
    }

    public void RenewHealth()
    {
        //isDead = false;
        currentHealth = maxHealth;
    }

    public void setMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        
            
        
            

        castleBarUI.UpdateCastleHealth(currentHealth / maxHealth, damage);

        

        if (currentHealth <= 0)
        {
            Die();

        }
    }

   

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDeath?.Invoke();

        if (destroyOnDeath)                   // only destroy if still set to true
            Destroy(gameObject);
    }
}