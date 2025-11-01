using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer_Tester : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Timer_UI timerUI;
    
    [Header("Test Buttons (Assign in Inspector)")]
    [SerializeField] private Button startTimerButton;
    [SerializeField] private Button pauseTimerButton;
    [SerializeField] private Button resetTimerButton;
    [SerializeField] private Button addTimeButton;
    [SerializeField] private Button setWarningButton;
    [SerializeField] private Button quickTestButton;
    
    [Header("Test Settings")]
    [SerializeField] private float testTimerDuration = 60f; // 1 minute default
    [SerializeField] private float timeToAdd = 15f; // 15 seconds to add when extending
    [SerializeField] private bool autoDecrease = true;
    
    // Private variables
    private float currentTime;
    private float maxTime;
    private bool isRunning = false;
    private Coroutine timerCoroutine;
    
    void Start()
    {
        SetupButtons();
        currentTime = testTimerDuration;
        maxTime = testTimerDuration;
        
        // Initialize timer display
        if (timerUI != null)
        {
            timerUI.UpdateTimer(currentTime, maxTime);
        }
    }
    
    void Update()
    {
        // Manual controls for testing (if no buttons assigned)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleTimer();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTimer();
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddTime();
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetToWarningTime();
        }
    }
    
    private void SetupButtons()
    {
        if (startTimerButton != null)
            startTimerButton.onClick.AddListener(ToggleTimer);
            
        if (pauseTimerButton != null)
            pauseTimerButton.onClick.AddListener(ToggleTimer);
            
        if (resetTimerButton != null)
            resetTimerButton.onClick.AddListener(ResetTimer);
            
        if (addTimeButton != null)
            addTimeButton.onClick.AddListener(AddTime);
            
        if (setWarningButton != null)
            setWarningButton.onClick.AddListener(SetToWarningTime);
            
        if (quickTestButton != null)
            quickTestButton.onClick.AddListener(QuickTest);
    }
    
    public void ToggleTimer()
    {
        if (isRunning)
        {
            PauseTimer();
        }
        else
        {
            StartTimer();
        }
    }
    
    public void StartTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        
        isRunning = true;
        timerCoroutine = StartCoroutine(TimerCountdown());
        
        Debug.Log("Timer Started");
        UpdateButtonTexts();
    }
    
    public void PauseTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        
        isRunning = false;
        Debug.Log("Timer Paused");
        UpdateButtonTexts();
    }
    
    public void ResetTimer()
    {
        PauseTimer();
        currentTime = testTimerDuration;
        maxTime = testTimerDuration;
        
        if (timerUI != null)
        {
            timerUI.ResetTimer();
            timerUI.UpdateTimer(currentTime, maxTime);
        }
        
        Debug.Log("Timer Reset");
        UpdateButtonTexts();
    }
    
    public void AddTime()
    {
        float newTime = currentTime + timeToAdd;
        
        if (timerUI != null)
        {
            timerUI.ExtendTime(newTime, maxTime);
        }
        
        currentTime = newTime;
        Debug.Log($"Added {timeToAdd} seconds. New time: {currentTime:F1}");
    }
    
    public void SetToWarningTime()
    {
        currentTime = 8f; // Set to 8 seconds to trigger warning
        
        if (timerUI != null)
        {
            timerUI.UpdateTimer(currentTime, maxTime);
        }
        
        Debug.Log("Set timer to warning time (8 seconds)");
    }
    
    public void QuickTest()
    {
        StartCoroutine(QuickTestSequence());
    }
    
    private IEnumerator QuickTestSequence()
    {
        Debug.Log("Starting Quick Test Sequence...");
        
        // Reset and start with 30 seconds
        currentTime = 30f;
        maxTime = 30f;
        if (timerUI != null)
        {
            timerUI.UpdateTimer(currentTime, maxTime);
        }
        
        yield return new WaitForSeconds(1f);
        
        // Start countdown
        StartTimer();
        Debug.Log("Phase 1: Normal countdown");
        
        yield return new WaitForSeconds(3f);
        
        // Add time (test extension effect)
        Debug.Log("Phase 2: Adding time");
        AddTime();
        
        yield return new WaitForSeconds(2f);
        
        // Jump to warning time
        Debug.Log("Phase 3: Testing warning effects");
        SetToWarningTime();
        
        yield return new WaitForSeconds(4f);
        
        // Add time during warning
        Debug.Log("Phase 4: Adding time during warning");
        AddTime();
        
        yield return new WaitForSeconds(2f);
        
        Debug.Log("Quick test completed!");
    }
    
    private IEnumerator TimerCountdown()
    {
        while (currentTime > 0 && isRunning)
        {
            yield return new WaitForSeconds(0.1f); // Update 10 times per second
            
            currentTime -= 0.1f;
            
            if (currentTime < 0)
            {
                currentTime = 0;
            }
            
            if (timerUI != null)
            {
                timerUI.UpdateTimer(currentTime, maxTime);
            }
            
            // Stop when time reaches 0
            if (currentTime <= 0)
            {
                isRunning = false;
                Debug.Log("Timer reached 0!");
                UpdateButtonTexts();
                break;
            }
        }
    }
    
    private void UpdateButtonTexts()
    {
        if (startTimerButton != null)
        {
            var tmpText = startTimerButton.GetComponentInChildren<TMPro.TMP_Text>();
            var uiText = startTimerButton.GetComponentInChildren<UnityEngine.UI.Text>();
            
            string newText = isRunning ? "Pause" : "Start";
            
            if (tmpText != null)
            {
                tmpText.text = newText;
            }
            else if (uiText != null)
            {
                uiText.text = newText;
            }
        }
    }
    
    // Public methods for setting test parameters
    public void SetTestDuration(float duration)
    {
        testTimerDuration = duration;
        Debug.Log($"Test duration set to {duration} seconds");
    }
    
    public void SetTimeToAdd(float timeAmount)
    {
        timeToAdd = timeAmount;
        Debug.Log($"Time to add set to {timeAmount} seconds");
    }
    
    // Display current status in inspector
    void OnGUI()
    {
        if (!Application.isPlaying) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Timer Tester Status", new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold });
        GUILayout.Label($"Current Time: {currentTime:F1}s");
        GUILayout.Label($"Max Time: {maxTime:F1}s");
        GUILayout.Label($"Is Running: {isRunning}");
        GUILayout.Label($"Progress: {(1f - currentTime/maxTime)*100f:F1}%");
        
        GUILayout.Space(10);
        GUILayout.Label("Keyboard Controls:", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        GUILayout.Label("SPACE - Start/Pause Timer");
        GUILayout.Label("R - Reset Timer");
        GUILayout.Label("A - Add Time");
        GUILayout.Label("W - Set to Warning Time");
        
        GUILayout.EndArea();
    }
}
