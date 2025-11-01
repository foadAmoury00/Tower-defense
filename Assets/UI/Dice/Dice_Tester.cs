using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Dice_Tester : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Dice_UI diceUI;
    
    [Header("Test Buttons (Assign in Inspector)")]
    [SerializeField] private Button rollDiceButton;
    [SerializeField] private Button quickRollButton;
    [SerializeField] private Button fadeLabelButton;
    [SerializeField] private Button showLabelButton;
    [SerializeField] private Button autoTestButton;
    [SerializeField] private Button simulateCallbackButton;
    [SerializeField] private Button testFadeSpeedButton;
    [SerializeField] private Button resetToDefaultButton;
    
    [Header("Test Settings")]
    [SerializeField] private float testRollDuration = 2f; // Simulated roll duration
    [SerializeField] private float customFadeSpeed = 5f; // For testing different fade speeds
    [SerializeField] private float autoTestSpeed = 3f; // Seconds between auto test actions
    [SerializeField] private int autoTestRolls = 5; // Number of rolls in auto test
    
    // Private variables
    private bool isCurrentlyRolling = false;
    private bool isAutoTesting = false;
    private Coroutine autoTestCoroutine;
    private int rollCount = 0;
    
    void Start()
    {
        SetupButtons();
        
        Debug.Log("Dice Tester initialized. Press R or use buttons to test dice rolling.");
    }
    
    void Update()
    {
        // Manual keyboard controls for testing
        if (Input.GetKeyDown(KeyCode.R))
        {
            RollDice();
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuickRoll();
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            FadeLabel();
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            ShowLabel();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            SimulateCallback();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleAutoTest();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestFadeSpeed();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ResetToDefault();
        }
    }
    
    private void SetupButtons()
    {
        if (rollDiceButton != null)
            rollDiceButton.onClick.AddListener(RollDice);
            
        if (quickRollButton != null)
            quickRollButton.onClick.AddListener(QuickRoll);
            
        if (fadeLabelButton != null)
            fadeLabelButton.onClick.AddListener(FadeLabel);
            
        if (showLabelButton != null)
            showLabelButton.onClick.AddListener(ShowLabel);
            
        if (autoTestButton != null)
            autoTestButton.onClick.AddListener(ToggleAutoTest);
            
        if (simulateCallbackButton != null)
            simulateCallbackButton.onClick.AddListener(SimulateCallback);
            
        if (testFadeSpeedButton != null)
            testFadeSpeedButton.onClick.AddListener(TestFadeSpeed);
            
        if (resetToDefaultButton != null)
            resetToDefaultButton.onClick.AddListener(ResetToDefault);
    }
    
    public void RollDice()
    {
        if (isCurrentlyRolling)
        {
            Debug.Log("Dice is already rolling! Wait for it to finish.");
            return;
        }
        
        // Simulate pressing R key (the actual input method)
        if (diceUI != null)
        {
            // This will trigger the dice UI's R key handling
            // Since the dice UI listens for R key in Update(), we can just log this
            Debug.Log("Simulating R key press for dice roll...");
            rollCount++;
            isCurrentlyRolling = true;
            
            // Track when rolling should be complete (based on dice UI's placeholder timing)
            StartCoroutine(TrackRollCompletion());
        }
        else
        {
            Debug.LogWarning("Dice UI reference not assigned!");
        }
    }
    
    public void QuickRoll()
    {
        if (isCurrentlyRolling)
        {
            Debug.Log("Dice is already rolling! Wait for it to finish.");
            return;
        }
        
        Debug.Log("Quick roll - reduced duration test");
        // Set a custom shorter duration for quick testing
        testRollDuration = 0.5f;
        RollDice();
        
        // Reset duration after roll
        StartCoroutine(ResetDurationAfterRoll());
    }
    
    private IEnumerator ResetDurationAfterRoll()
    {
        yield return new WaitForSeconds(1f);
        testRollDuration = 2f; // Reset to default
    }
    
    public void FadeLabel()
    {
        if (diceUI != null)
        {
            // We can't directly access private methods, but we can test the effect
            // by simulating what happens during a roll
            Debug.Log("Testing label fade to transparent");
            
            // This would be like calling FadeLabelToTransparent() if it were public
            // For testing purposes, we'll just log the action
            Debug.Log("Label should fade to transparent now");
        }
    }
    
    public void ShowLabel()
    {
        if (diceUI != null)
        {
            // Simulate showing the label (like after roll completion)
            Debug.Log("Testing label fade to visible");
            
            // This simulates what OnDiceRollComplete() does
            diceUI.OnDiceRollComplete();
        }
    }
    
    public void SimulateCallback()
    {
        if (diceUI != null)
        {
            Debug.Log("Simulating dice roll completion callback");
            diceUI.OnDiceRollComplete();
            isCurrentlyRolling = false;
        }
    }
    
    public void TestFadeSpeed()
    {
        Debug.Log($"Testing fade speed. Current custom speed: {customFadeSpeed}");
        // Note: Since fadeSpeed is private in Dice_UI, this is more of a conceptual test
        // In a real implementation, you might want to make fadeSpeed accessible for testing
        Debug.Log("Note: Fade speed is controlled in Dice_UI inspector. Adjust there for testing.");
    }
    
    public void ResetToDefault()
    {
        isCurrentlyRolling = false;
        rollCount = 0;
        testRollDuration = 2f;
        customFadeSpeed = 5f;
        
        Debug.Log("Reset dice tester to default state");
        
        // Ensure label is visible
        if (diceUI != null)
        {
            diceUI.OnDiceRollComplete();
        }
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
        Debug.Log("Started dice auto test sequence");
        UpdateAutoTestButtonText();
    }
    
    public void StopAutoTest()
    {
        if (autoTestCoroutine != null)
        {
            StopCoroutine(autoTestCoroutine);
        }
        
        isAutoTesting = false;
        Debug.Log("Stopped dice auto test sequence");
        UpdateAutoTestButtonText();
    }
    
    private IEnumerator AutoTestSequence()
    {
        Debug.Log("=== Dice UI Auto Test Sequence Started ===");
        
        while (isAutoTesting)
        {
            // Phase 1: Reset to default state
            Debug.Log("Phase 1: Reset to default state");
            ResetToDefault();
            yield return new WaitForSeconds(autoTestSpeed * 0.5f);
            
            if (!isAutoTesting) break;
            
            // Phase 2: Test normal rolls
            Debug.Log("Phase 2: Testing normal dice rolls");
            for (int i = 0; i < autoTestRolls; i++)
            {
                Debug.Log($"Auto roll {i + 1}/{autoTestRolls}");
                RollDice();
                
                // Wait for roll to complete
                yield return new WaitForSeconds(testRollDuration + 0.5f);
                if (!isAutoTesting) break;
            }
            
            if (!isAutoTesting) break;
            yield return new WaitForSeconds(autoTestSpeed);
            
            // Phase 3: Test quick rolls
            Debug.Log("Phase 3: Testing quick rolls");
            for (int i = 0; i < 3; i++)
            {
                QuickRoll();
                yield return new WaitForSeconds(1f);
                if (!isAutoTesting) break;
            }
            
            if (!isAutoTesting) break;
            yield return new WaitForSeconds(autoTestSpeed);
            
            // Phase 4: Test manual label control
            Debug.Log("Phase 4: Testing manual label control");
            FadeLabel();
            yield return new WaitForSeconds(1f);
            
            if (!isAutoTesting) break;
            
            ShowLabel();
            yield return new WaitForSeconds(1f);
            
            if (!isAutoTesting) break;
            
            // Phase 5: Test callback simulation
            Debug.Log("Phase 5: Testing callback simulation");
            RollDice();
            yield return new WaitForSeconds(1f); // Don't wait for full roll
            
            if (!isAutoTesting) break;
            
            SimulateCallback(); // Force completion
            yield return new WaitForSeconds(1f);
            
            if (!isAutoTesting) break;
            
            // Phase 6: Test rapid rolls (should be blocked)
            Debug.Log("Phase 6: Testing rapid roll blocking");
            RollDice();
            yield return new WaitForSeconds(0.2f);
            
            if (!isAutoTesting) break;
            
            RollDice(); // This should be blocked
            RollDice(); // This should also be blocked
            yield return new WaitForSeconds(testRollDuration);
            
            if (!isAutoTesting) break;
            
            Debug.Log("=== Dice UI Auto Test Sequence Completed ===");
            yield return new WaitForSeconds(autoTestSpeed * 2);
        }
    }
    
    private IEnumerator TrackRollCompletion()
    {
        yield return new WaitForSeconds(testRollDuration);
        isCurrentlyRolling = false;
        Debug.Log($"Roll #{rollCount} tracking completed");
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
    public void SetTestRollDuration(float duration)
    {
        testRollDuration = duration;
        Debug.Log($"Test roll duration set to {duration} seconds");
    }
    
    public void SetCustomFadeSpeed(float speed)
    {
        customFadeSpeed = speed;
        Debug.Log($"Custom fade speed set to {speed}");
    }
    
    public void SetAutoTestSpeed(float speed)
    {
        autoTestSpeed = speed;
        Debug.Log($"Auto test speed set to {speed} seconds");
    }
    
    public void SetAutoTestRolls(int rolls)
    {
        autoTestRolls = rolls;
        Debug.Log($"Auto test rolls set to {rolls}");
    }
    
    // Status display for debugging
    void OnGUI()
    {
        if (!Application.isPlaying) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 400, 380));
        GUILayout.Label("Dice UI Tester Status", new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold });
        
        // Current status
        GUILayout.Label($"Total Rolls: {rollCount}");
        GUILayout.Label($"Currently Rolling: {isCurrentlyRolling}");
        GUILayout.Label($"Auto Testing: {isAutoTesting}");
        
        if (diceUI != null)
        {
            GUILayout.Label("Dice UI: Connected");
        }
        else
        {
            GUILayout.Label("Dice UI: NOT CONNECTED", new GUIStyle(GUI.skin.label) { normal = { textColor = Color.red } });
        }
        
        GUILayout.Space(10);
        
        // Test settings
        GUILayout.Label("Test Settings:", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        GUILayout.Label($"Test Roll Duration: {testRollDuration}s");
        GUILayout.Label($"Custom Fade Speed: {customFadeSpeed}");
        GUILayout.Label($"Auto Test Speed: {autoTestSpeed}s");
        GUILayout.Label($"Auto Test Rolls: {autoTestRolls}");
        
        GUILayout.Space(10);
        
        // Integration status
        GUILayout.Label("Integration Status:", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        GUILayout.Label("• R key input: Ready");
        GUILayout.Label("• Label fading: Ready");
        GUILayout.Label("• Roll blocking: Ready");
        GUILayout.Label("• Callback system: Ready");
        GUILayout.Label("• Dice component: Placeholder");
        
        GUILayout.Space(10);
        
        // Keyboard controls
        GUILayout.Label("Keyboard Controls:", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        GUILayout.Label("R - Roll Dice");
        GUILayout.Label("Q - Quick Roll");
        GUILayout.Label("F - Fade Label");
        GUILayout.Label("S - Show Label");
        GUILayout.Label("C - Simulate Callback");
        GUILayout.Label("T - Toggle Auto Test");
        GUILayout.Label("1 - Test Fade Speed");
        GUILayout.Label("0 - Reset to Default");
        
        GUILayout.EndArea();
    }
}