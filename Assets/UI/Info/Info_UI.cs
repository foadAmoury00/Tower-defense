using TMPro;
using UnityEngine;
using System.Collections;

public class Info_UI : MonoBehaviour
{
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color maxReachedColor = Color.red;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeIntensity = 5f;
    
    private int currentCount = 0;
    private int maxCount = 10;
    private Vector3 originalPosition;
    private Color originalColor;
    private bool isMaxReached = false;

    void Start()
    {
        originalPosition = counterText.transform.localPosition;
        originalColor = counterText.color;
        UpdateCounterDisplay();
    }

    void Update()
    {
        
    }
    
    /// <summary>
    /// Updates the counter with new values
    /// </summary>
    /// <param name="count">Current count value</param>
    /// <param name="max">Maximum count value</param>
    public void UpdateCounter(int count, int max)
    {
        int previousCount = currentCount;
        int previousMax = maxCount;
        
        currentCount = Mathf.Clamp(count, 0, max);
        maxCount = max;
        
        // Check if we tried to add beyond max
        if (count > max && previousCount == previousMax)
        {
            TriggerMaxReachedEffect();
        }
        // Check if we just reached max
        else if (currentCount == maxCount && previousCount < previousMax)
        {
            TriggerMaxReachedEffect();
        }
        
        UpdateCounterDisplay();
        UpdateColorBasedOnCount();
    }
    
    /// <summary>
    /// Attempts to add to the current count
    /// </summary>
    /// <param name="amount">Amount to add</param>
    /// <returns>True if successfully added, false if max reached</returns>
    public bool TryAddToCount(int amount)
    {
        if (currentCount >= maxCount)
        {
            TriggerMaxReachedEffect();
            return false;
        }
        
        int newCount = currentCount + amount;
        UpdateCounter(newCount, maxCount);
        return true;
    }
    
    /// <summary>
    /// Sets the maximum count value
    /// </summary>
    /// <param name="max">New maximum value</param>
    public void SetMaxCount(int max)
    {
        maxCount = max;
        currentCount = Mathf.Clamp(currentCount, 0, maxCount);
        UpdateCounterDisplay();
        UpdateColorBasedOnCount();
    }
    
    private void UpdateCounterDisplay()
    {
        if (counterText != null)
        {
            counterText.text = $"{currentCount}/{maxCount}";
        }
    }
    
    private void UpdateColorBasedOnCount()
    {
        if (counterText == null) return;
        
        bool wasMaxReached = isMaxReached;
        isMaxReached = currentCount >= maxCount;
        
        if (isMaxReached && !wasMaxReached)
        {
            // Just reached max, trigger color change
            counterText.color = maxReachedColor;
        }
        else if (!isMaxReached)
        {
            // Not at max, use normal color
            counterText.color = normalColor;
        }
    }
    
    private void TriggerMaxReachedEffect()
    {
        // Stop any ongoing effects
        StopAllCoroutines();
        
        // Start flash and shake effects
        StartCoroutine(FlashEffect());
        StartCoroutine(ShakeEffect());
    }
    
    private IEnumerator FlashEffect()
    {
        Color startColor = counterText.color;
        float elapsedTime = 0f;
        
        while (elapsedTime < flashDuration)
        {
            float t = elapsedTime / flashDuration;
            counterText.color = Color.Lerp(warningColor, maxReachedColor, Mathf.PingPong(t * 4, 1));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Restore appropriate color
        UpdateColorBasedOnCount();
    }
    
    private IEnumerator ShakeEffect()
    {
        Vector3 startPosition = originalPosition;
        float elapsedTime = 0f;
        
        while (elapsedTime < shakeDuration)
        {
            float t = elapsedTime / shakeDuration;
            float intensity = shakeIntensity * (1 - t); // Fade out the shake
            
            Vector3 randomOffset = new Vector3(
                Random.Range(-intensity, intensity),
                Random.Range(-intensity, intensity),
                0
            );
            
            counterText.transform.localPosition = startPosition + randomOffset;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Reset position
        counterText.transform.localPosition = originalPosition;
    }
    
    /// <summary>
    /// Gets the current count value
    /// </summary>
    public int GetCurrentCount() => currentCount;
    
    /// <summary>
    /// Gets the maximum count value
    /// </summary>
    public int GetMaxCount() => maxCount;
    
    /// <summary>
    /// Checks if the counter has reached its maximum
    /// </summary>
    public bool IsAtMax() => currentCount >= maxCount;
}
