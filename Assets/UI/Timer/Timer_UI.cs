using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer_UI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private RectTransform sunPivot;
    [SerializeField] private Image sunImage;

    [Header("Timer Settings")]
    [SerializeField] private float maxAngle = 0f;        // Starting position (top)
    [SerializeField] private float minAngle = 360f;      // Ending position (full circle)
    [SerializeField] private float warningThreshold = 10f; // Time in seconds when warning effects start
    
    [Header("Sun Effects")]
    [SerializeField] private Color normalSunColor = Color.yellow;
    [SerializeField] private Color warningSunColor = Color.red;
    [SerializeField] private float glowPulseSpeed = 2f;
    [SerializeField] private float maxGlowIntensity = 1.5f;
    
    [Header("Sand Swirl Effect (Optional)")]
    [SerializeField] private ParticleSystem sandSwirlEffect; // Assign in inspector if available
    
    // Private variables
    private float currentTime;
    private float maxTime;
    private bool isWarning = false;
    private Coroutine glowCoroutine;
    private Color originalSunColor;
    
    void Start()
    {
        originalSunColor = sunImage.color;
        if (sandSwirlEffect != null)
        {
            sandSwirlEffect.Stop();
        }
    }
    
    /// <summary>
    /// Main method to update the timer from controlling layer
    /// </summary>
    /// <param name="timeRemaining">Time remaining in seconds</param>
    /// <param name="totalTime">Total time for this countdown</param>
    public void UpdateTimer(float timeRemaining, float totalTime)
    {
        currentTime = timeRemaining;
        maxTime = totalTime;
        
        UpdateTimerDisplay();
        UpdateSunPosition();
        UpdateSunEffects();
    }
    
    /// <summary>
    /// Call this when time is extended (dice roll effect)
    /// </summary>
    /// <param name="newTimeRemaining">New time after extension</param>
    /// <param name="totalTime">Total time</param>
    public void ExtendTime(float newTimeRemaining, float totalTime)
    {
        StartCoroutine(TimeExtensionEffect(newTimeRemaining, totalTime));
    }
    
    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    private void UpdateSunPosition()
    {
        if (maxTime <= 0) return;
        
        // Calculate progress (0 = start, 1 = end)
        float progress = 1f - (currentTime / maxTime);
        
        // Calculate angle based on progress
        float targetAngle = Mathf.Lerp(maxAngle, minAngle, progress);
        
        // Apply rotation to sun pivot
        sunPivot.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
    
    private void UpdateSunEffects()
    {
        bool shouldWarn = currentTime <= warningThreshold;
        
        if (shouldWarn && !isWarning)
        {
            // Start warning effects
            isWarning = true;
            if (glowCoroutine != null)
                StopCoroutine(glowCoroutine);
            glowCoroutine = StartCoroutine(PulseSunGlow());
        }
        else if (!shouldWarn && isWarning)
        {
            // Stop warning effects
            isWarning = false;
            if (glowCoroutine != null)
                StopCoroutine(glowCoroutine);
            sunImage.color = originalSunColor;
        }
    }
    
    private IEnumerator PulseSunGlow()
    {
        while (isWarning)
        {
            // Pulse between normal and warning colors with intensity
            float pulse = (Mathf.Sin(Time.time * glowPulseSpeed) + 1f) / 2f;
            Color targetColor = Color.Lerp(normalSunColor, warningSunColor, pulse);
            
            // Add glow effect by increasing intensity
            targetColor *= (1f + pulse * (maxGlowIntensity - 1f));
            
            sunImage.color = targetColor;
            yield return null;
        }
    }
    
    private IEnumerator TimeExtensionEffect(float newTimeRemaining, float totalTime)
    {
        // Trigger sand swirl effect if available
        if (sandSwirlEffect != null)
        {
            sandSwirlEffect.Play();
        }
        
        // Store current angle
        float currentAngle = sunPivot.rotation.eulerAngles.z;
        
        // Calculate new target angle
        float progress = 1f - (newTimeRemaining / totalTime);
        float newTargetAngle = Mathf.Lerp(maxAngle, minAngle, progress);
        
        // Animate sundial rotating backward
        float animationDuration = 1f;
        float elapsedTime = 0f;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            
            // Use ease-out curve for smooth animation
            t = 1f - Mathf.Pow(1f - t, 3f);
            
            float animatedAngle = Mathf.LerpAngle(currentAngle, newTargetAngle, t);
            sunPivot.rotation = Quaternion.Euler(0, 0, animatedAngle);
            
            yield return null;
        }
        
        // Update the timer values
        currentTime = newTimeRemaining;
        maxTime = totalTime;
        
        // Stop sand swirl effect
        if (sandSwirlEffect != null)
        {
            sandSwirlEffect.Stop();
        }
        
        // Update display and effects
        UpdateTimerDisplay();
        UpdateSunEffects();
    }
    
    /// <summary>
    /// Reset timer to initial state
    /// </summary>
    public void ResetTimer()
    {
        currentTime = 0;
        maxTime = 0;
        isWarning = false;
        
        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
            glowCoroutine = null;
        }
        
        sunImage.color = originalSunColor;
        sunPivot.rotation = Quaternion.Euler(0, 0, maxAngle);
        timerText.text = "00:00";
        
        if (sandSwirlEffect != null)
        {
            sandSwirlEffect.Stop();
        }
    }
}
