using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CastleBar_UI : MonoBehaviour
{
    [Header("Health Bar Components")]
    [SerializeField] private Scrollbar castleHealthBar;
    [SerializeField] private Image barHandle;
    [SerializeField] private Image castleIcon;
    
    [Header("Castle Sprites")]
    [SerializeField] private Sprite castleHealthySprite;
    [SerializeField] private Sprite castleDamagedSprite;
    
    [Header("Health Bar Colors")]
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private Color criticalHealthColor = new Color(1f, 0.3f, 0f); // Dark red
    
    [Header("Damage Feedback")]
    [SerializeField] private GameObject damagePopupPrefab;
    [SerializeField] private Transform popupSpawnPoint;
    [SerializeField] private float shakeIntensity = 0.5f;
    [SerializeField] private float shakeDuration = 0.3f;
    
    [Header("Health Thresholds")]
    [SerializeField] private float lowHealthThreshold = 0.3f;
    [SerializeField] private float criticalHealthThreshold = 0.1f;
    
    // Private variables
    private float currentHealth = 1f;
    private float maxHealth = 1f;
    private Vector3 originalPosition;
    private Coroutine healthUpdateCoroutine;
    
    void Start()
    {
        originalPosition = transform.localPosition;
        InitializeHealthBar();
    }
    
    private void InitializeHealthBar()
    {
        SetHealthBarValue(1f);
        castleIcon.sprite = castleHealthySprite;
    }
    
    /// <summary>
    /// Main method to update the castle health bar
    /// </summary>
    /// <param name="healthPercentage">Health value between 0 and 1</param>
    /// <param name="damage">Amount of damage taken (for popup animation)</param>
    public void UpdateCastleHealth(float healthPercentage, float damage = 0f)
    {
        healthPercentage = Mathf.Clamp01(healthPercentage);
        
        // Stop any ongoing health update animation
        if (healthUpdateCoroutine != null)
        {
            StopCoroutine(healthUpdateCoroutine);
        }
        
        // Start the health update animation
        healthUpdateCoroutine = StartCoroutine(AnimateHealthChange(healthPercentage, damage));
    }
    
    private IEnumerator AnimateHealthChange(float targetHealth, float damage)
    {
        float startHealth = currentHealth;
        float animationTime = 0.5f;
        float elapsed = 0f;
        
        // Show damage popup if damage was taken
        if (damage > 0f)
        {
            ShowDamagePopup(damage);
            TriggerShakeAnimation();
        }
        
        // Animate the health bar change
        while (elapsed < animationTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationTime;
            
            // Use easing for smooth animation
            t = Mathf.SmoothStep(0f, 1f, t);
            
            float newHealth = Mathf.Lerp(startHealth, targetHealth, t);
            SetHealthBarValue(newHealth);
            
            yield return null;
        }
        
        // Ensure final value is set
        SetHealthBarValue(targetHealth);
        currentHealth = targetHealth;
    }
    
    private void SetHealthBarValue(float healthPercentage)
    {
        // Update scrollbar size (bar physically shrinks/grows based on health)
        castleHealthBar.size = healthPercentage;
        
        // Update colors based on health
        UpdateHealthBarColor(healthPercentage);
        
        // Update castle icon based on health
        UpdateCastleIcon(healthPercentage);
    }
    
    private void UpdateHealthBarColor(float healthPercentage)
    {
        Color targetColor;
        
        if (healthPercentage > lowHealthThreshold)
        {
            targetColor = healthyColor;
        }
        else if (healthPercentage > criticalHealthThreshold)
        {
            // Interpolate between healthy and low health color
            float t = (healthPercentage - criticalHealthThreshold) / (lowHealthThreshold - criticalHealthThreshold);
            targetColor = Color.Lerp(lowHealthColor, healthyColor, t);
        }
        else
        {
            targetColor = criticalHealthColor;
        }
        
        // Apply color to bar handle
        if (barHandle != null)
            barHandle.color = targetColor;
    }
    
    private void UpdateCastleIcon(float healthPercentage)
    {
        if (healthPercentage <= 0f)
        {
            castleIcon.sprite = castleDamagedSprite;
        }
        else
        {
            castleIcon.sprite = castleHealthySprite;
        }
    }
    
    private void ShowDamagePopup(float damage)
    {
        if (damagePopupPrefab == null || popupSpawnPoint == null) return;
        
        GameObject popup = Instantiate(damagePopupPrefab, popupSpawnPoint.position, Quaternion.identity, transform.parent);
        
        // Get the text component and set damage value
        TextMeshProUGUI damageText = popup.GetComponent<TextMeshProUGUI>();
        if (damageText != null)
        {
            damageText.text = "-" + damage.ToString("F0");
            damageText.color = Color.red;
        }
        
        // Start popup animation coroutine
        StartCoroutine(AnimateDamagePopup(popup));
    }
    
    private IEnumerator AnimateDamagePopup(GameObject popup)
    {
        if (popup == null) yield break;
        
        Vector3 startPos = popup.transform.position;
        Vector3 endPos = startPos + Vector3.up * 50f;
        
        // Get or add CanvasGroup for fading
        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = popup.AddComponent<CanvasGroup>();
        
        float duration = 1f;
        float elapsed = 0f;
        
        // Start with scale 0 and grow
        popup.transform.localScale = Vector3.zero;
        
        // Animation loop
        while (elapsed < duration && popup != null)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Scale animation (quick pop-in)
            if (t < 0.2f)
            {
                float scaleT = t / 0.2f;
                // Ease out back effect
                scaleT = 1f - Mathf.Pow(1f - scaleT, 3f);
                popup.transform.localScale = Vector3.one * scaleT;
            }
            else
            {
                popup.transform.localScale = Vector3.one;
            }
            
            // Position animation (move up)
            popup.transform.position = Vector3.Lerp(startPos, endPos, t);
            
            // Fade out after half duration
            if (t > 0.5f)
            {
                float fadeT = (t - 0.5f) / 0.5f;
                canvasGroup.alpha = 1f - fadeT;
            }
            
            yield return null;
        }
        
        if (popup != null)
            Destroy(popup);
    }
    
    private void TriggerShakeAnimation()
    {
        StartCoroutine(ShakeAnimation());
    }
    
    private IEnumerator ShakeAnimation()
    {
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            
            // Generate random offset
            Vector3 randomOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0f
            );
            
            transform.localPosition = originalPosition + randomOffset;
            
            yield return null;
        }
        
        // Reset to original position
        transform.localPosition = originalPosition;
    }
    
    // Public methods for external access
    public float GetCurrentHealthPercentage()
    {
        return currentHealth;
    }
    
    public bool IsLowHealth()
    {
        return currentHealth <= lowHealthThreshold;
    }
    
    public bool IsCriticalHealth()
    {
        return currentHealth <= criticalHealthThreshold;
    }
    
    // Method to directly set health without animation (for initialization)
    public void SetHealthInstant(float healthPercentage)
    {
        healthPercentage = Mathf.Clamp01(healthPercentage);
        currentHealth = healthPercentage;
        SetHealthBarValue(healthPercentage);
    }
}
