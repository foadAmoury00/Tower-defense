using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class Info_Tester : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Info_UI infoUI;
    
    [Header("Test Buttons (Assign in Inspector)")]
    [SerializeField] private Button addOneButton;
    [SerializeField] private Button addFiveButton;
    [SerializeField] private Button subtractOneButton;
    [SerializeField] private Button resetCountButton;
    [SerializeField] private Button setMaxButton;
    [SerializeField] private Button setNearMaxButton;
    [SerializeField] private Button setToMaxButton;
    [SerializeField] private Button autoTestButton;
    [SerializeField] private Button tryAddWhenMaxButton;
    
    [Header("Test Settings")]
    [SerializeField] private int defaultMaxCount = 10;
    [SerializeField] private int largeAddAmount = 5;
    [SerializeField] private float autoTestSpeed = 1.5f; // Seconds between auto test actions
    
    // Private variables
    private int currentTestCount = 0;
    private int currentTestMax = 10;
    private bool isAutoTesting = false;
    private Coroutine autoTestCoroutine;
    
    void Start()
    {
        SetupButtons();
        
        // Initialize info UI
        currentTestMax = defaultMaxCount;
        currentTestCount = 0;
        
        if (infoUI != null)
        {
            infoUI.UpdateCounter(currentTestCount, currentTestMax);
        }
    }
    
    void Update()
    {
        // Manual keyboard controls for testing
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddOne();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddFive();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SubtractOne();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCount();
        }
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            SetToMax();
        }
        
        if (Input.GetKeyDown(KeyCode.N))
        {
            SetNearMax();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleAutoTest();
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryAddWhenMax();
        }
        
        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals))
        {
            IncreaseMaxCount();
        }
        
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            DecreaseMaxCount();
        }
    }
    
    private void SetupButtons()
    {
        if (addOneButton != null)
            addOneButton.onClick.AddListener(AddOne);
            
        if (addFiveButton != null)
            addFiveButton.onClick.AddListener(AddFive);
            
        if (subtractOneButton != null)
            subtractOneButton.onClick.AddListener(SubtractOne);
            
        if (resetCountButton != null)
            resetCountButton.onClick.AddListener(ResetCount);
            
        if (setMaxButton != null)
            setMaxButton.onClick.AddListener(SetMax);
            
        if (setNearMaxButton != null)
            setNearMaxButton.onClick.AddListener(SetNearMax);
            
        if (setToMaxButton != null)
            setToMaxButton.onClick.AddListener(SetToMax);
            
        if (autoTestButton != null)
            autoTestButton.onClick.AddListener(ToggleAutoTest);
            
        if (tryAddWhenMaxButton != null)
            tryAddWhenMaxButton.onClick.AddListener(TryAddWhenMax);
    }

    

    public void AddOne()
    {
        bool success = false;
        if (infoUI != null)
        {
            success = infoUI.TryAddToCount(1);
        }
        
        if (success)
        {
            currentTestCount = infoUI.GetCurrentCount();
            Debug.Log($"Added 1. Count: {currentTestCount}/{currentTestMax}");
        }
        else
        {
            Debug.Log("Failed to add 1 - max reached!");
        }
    }
    
    public void AddFive()
    {
        bool success = false;
        if (infoUI != null)
        {
            success = infoUI.TryAddToCount(largeAddAmount);
        }
        
        if (success)
        {
            currentTestCount = infoUI.GetCurrentCount();
            Debug.Log($"Added {largeAddAmount}. Count: {currentTestCount}/{currentTestMax}");
        }
        else
        {
            Debug.Log($"Failed to add {largeAddAmount} - max reached!");
        }
    }
    
    public void SubtractOne()
    {
        if (currentTestCount > 0)
        {
            currentTestCount--;
            if (infoUI != null)
            {
                infoUI.UpdateCounter(currentTestCount, currentTestMax);
            }
            Debug.Log($"Subtracted 1. Count: {currentTestCount}/{currentTestMax}");
        }
        else
        {
            Debug.Log("Cannot subtract - count is already 0");
        }
    }
    
    public void ResetCount()
    {
        currentTestCount = 0;
        if (infoUI != null)
        {
            infoUI.UpdateCounter(currentTestCount, currentTestMax);
        }
        Debug.Log($"Reset count to 0. Count: {currentTestCount}/{currentTestMax}");
    }

    private void SetMax()
    {
        currentTestMax = defaultMaxCount;
        currentTestMax = Mathf.Clamp(currentTestMax, 1, 50);
        if (infoUI != null)
        {
            infoUI.SetMaxCount(currentTestMax);
        }
        Debug.Log($"Set max to {currentTestMax}. Count: {currentTestCount}/{currentTestMax}");
    }
    
    public void SetToMax()
    {
        currentTestCount = currentTestMax;
        if (infoUI != null)
        {
            infoUI.UpdateCounter(currentTestCount, currentTestMax);
        }
        Debug.Log($"Set to max. Count: {currentTestCount}/{currentTestMax}");
    }
    
    public void SetNearMax()
    {
        currentTestCount = Mathf.Max(0, currentTestMax - 1);
        if (infoUI != null)
        {
            infoUI.UpdateCounter(currentTestCount, currentTestMax);
        }
        Debug.Log($"Set near max. Count: {currentTestCount}/{currentTestMax}");
    }
    
    public void TryAddWhenMax()
    {
        // First set to max
        SetToMax();
        
        // Wait a moment then try to add more
        StartCoroutine(DelayedAddAttempt());
    }
    
    private IEnumerator DelayedAddAttempt()
    {
        yield return new WaitForSeconds(0.5f);
        
        bool success = false;
        if (infoUI != null)
        {
            success = infoUI.TryAddToCount(1);
        }
        
        Debug.Log($"Attempted to add when at max. Success: {success}");
    }
    
    public void IncreaseMaxCount()
    {
        currentTestMax = Mathf.Min(currentTestMax + 5, 50); // Cap at 50
        if (infoUI != null)
        {
            infoUI.SetMaxCount(currentTestMax);
        }
        currentTestCount = infoUI != null ? infoUI.GetCurrentCount() : currentTestCount;
        Debug.Log($"Increased max to {currentTestMax}. Count: {currentTestCount}/{currentTestMax}");
    }
    
    public void DecreaseMaxCount()
    {
        currentTestMax = Mathf.Max(currentTestMax - 5, 1); // Minimum of 1
        if (infoUI != null)
        {
            infoUI.SetMaxCount(currentTestMax);
        }
        currentTestCount = infoUI != null ? infoUI.GetCurrentCount() : currentTestCount;
        Debug.Log($"Decreased max to {currentTestMax}. Count: {currentTestCount}/{currentTestMax}");
    }
    
    public void ToggleAutoTest()
    {
        if (isAutoTesting)
        {
            StopAutoTest();
        }
        else
        {
            StartAutoTest();
        }
    }
    
    public void StartAutoTest()
    {
        if (autoTestCoroutine != null)
        {
            StopCoroutine(autoTestCoroutine);
        }
        
        isAutoTesting = true;
        autoTestCoroutine = StartCoroutine(AutoTestSequence());
        Debug.Log("Started auto test sequence");
        UpdateAutoTestButtonText();
    }
    
    public void StopAutoTest()
    {
        if (autoTestCoroutine != null)
        {
            StopCoroutine(autoTestCoroutine);
        }
        
        isAutoTesting = false;
        Debug.Log("Stopped auto test sequence");
        UpdateAutoTestButtonText();
    }
    
    private IEnumerator AutoTestSequence()
    {
        Debug.Log("=== Info UI Auto Test Sequence Started ===");
        
        while (isAutoTesting)
        {
            // Phase 1: Reset counter
            Debug.Log("Phase 1: Reset counter");
            ResetCount();
            yield return new WaitForSeconds(autoTestSpeed);
            
            if (!isAutoTesting) break;
            
            // Phase 2: Add one by one to show progression
            Debug.Log("Phase 2: Adding one by one");
            for (int i = 0; i < currentTestMax - 1; i++)
            {
                AddOne();
                yield return new WaitForSeconds(autoTestSpeed * 0.3f);
                if (!isAutoTesting) break;
            }
            
            if (!isAutoTesting) break;
            yield return new WaitForSeconds(autoTestSpeed);
            
            // Phase 3: Add final one to reach max (trigger effects)
            Debug.Log("Phase 3: Reaching max");
            AddOne();
            yield return new WaitForSeconds(autoTestSpeed);
            
            if (!isAutoTesting) break;
            
            // Phase 4: Try to add more when at max (trigger effects)
            Debug.Log("Phase 4: Attempting to exceed max");
            for (int i = 0; i < 3; i++)
            {
                AddOne();
                yield return new WaitForSeconds(autoTestSpeed * 0.5f);
                if (!isAutoTesting) break;
            }
            
            if (!isAutoTesting) break;
            yield return new WaitForSeconds(autoTestSpeed);
            
            // Phase 5: Test with different max values
            Debug.Log("Phase 5: Testing different max values");
            IncreaseMaxCount();
            yield return new WaitForSeconds(autoTestSpeed * 0.5f);
            
            if (!isAutoTesting) break;
            
            AddFive();
            yield return new WaitForSeconds(autoTestSpeed);
            
            if (!isAutoTesting) break;
            
            DecreaseMaxCount();
            yield return new WaitForSeconds(autoTestSpeed);
            
            if (!isAutoTesting) break;
            
            // Phase 6: Test large additions
            Debug.Log("Phase 6: Testing large additions");
            ResetCount();
            yield return new WaitForSeconds(autoTestSpeed * 0.5f);
            
            if (!isAutoTesting) break;
            
            AddFive();
            yield return new WaitForSeconds(autoTestSpeed);
            
            if (!isAutoTesting) break;
            
            AddFive(); // This should trigger max reached
            yield return new WaitForSeconds(autoTestSpeed);
            
            if (!isAutoTesting) break;
            
            Debug.Log("=== Info UI Auto Test Sequence Completed ===");
            yield return new WaitForSeconds(autoTestSpeed * 2);
        }
    }
    
    private void UpdateAutoTestButtonText()
    {
        if (autoTestButton != null)
        {
            var tmpText = autoTestButton.GetComponentInChildren<TMPro.TMP_Text>();
            var uiText = autoTestButton.GetComponentInChildren<UnityEngine.UI.Text>();
            
            string newText = isAutoTesting ? "Stop Auto Test" : "Start Auto Test";
            
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
    public void SetDefaultMaxCount(int max)
    {
        defaultMaxCount = max;
        Debug.Log($"Default max count set to {max}");
    }
    
    public void SetLargeAddAmount(int amount)
    {
        largeAddAmount = amount;
        Debug.Log($"Large add amount set to {amount}");
    }
    
    // Status display for debugging
    void OnGUI()
    {
        if (!Application.isPlaying) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 400, 350));
        GUILayout.Label("Info UI Tester Status", new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold });
        
        // Current status
        if (infoUI != null)
        {
            GUILayout.Label($"Current Count: {infoUI.GetCurrentCount()}");
            GUILayout.Label($"Max Count: {infoUI.GetMaxCount()}");
            GUILayout.Label($"Is At Max: {infoUI.IsAtMax()}");
            GUILayout.Label($"Progress: {(float)infoUI.GetCurrentCount() / infoUI.GetMaxCount() * 100f:F1}%");
        }
        else
        {
            GUILayout.Label($"Current Count: {currentTestCount}");
            GUILayout.Label($"Max Count: {currentTestMax}");
        }
        
        GUILayout.Label($"Auto Testing: {isAutoTesting}");
        
        GUILayout.Space(10);
        
        // Test settings
        GUILayout.Label("Test Settings:", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        GUILayout.Label($"Default Max Count: {defaultMaxCount}");
        GUILayout.Label($"Large Add Amount: {largeAddAmount}");
        GUILayout.Label($"Auto Test Speed: {autoTestSpeed}s");
        
        GUILayout.Space(10);
        
        // Keyboard controls
        GUILayout.Label("Keyboard Controls:", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        GUILayout.Label("1 - Add One");
        GUILayout.Label("2 - Add Five");
        GUILayout.Label("3 - Subtract One");
        GUILayout.Label("R - Reset Count");
        GUILayout.Label("M - Set to Max");
        GUILayout.Label("N - Set Near Max");
        GUILayout.Label("F - Try Add When Max");
        GUILayout.Label("T - Toggle Auto Test");
        GUILayout.Label("+ - Increase Max Count");
        GUILayout.Label("- - Decrease Max Count");
        
        GUILayout.EndArea();
    }
}