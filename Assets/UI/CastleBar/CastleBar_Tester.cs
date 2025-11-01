using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CastleBar_Tester : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CastleBar_UI castleBarUI;
    
    [Header("Test Buttons (Assign in Inspector)")]
    [SerializeField] private Button healButton;
    [SerializeField] private Button smallDamageButton;
    [SerializeField] private Button mediumDamageButton;
    [SerializeField] private Button largeDamageButton;
    [SerializeField] private Button resetHealthButton;
    [SerializeField] private Button autoTestButton;
    [SerializeField] private Button setLowHealthButton;
    [SerializeField] private Button setCriticalHealthButton;
    
    [Header("Test Settings")]
    [SerializeField] private float smallDamage = 10f;
    [SerializeField] private float mediumDamage = 25f;
    [SerializeField] private float largeDamage = 50f;
    [SerializeField] private float healAmount = 20f;
    [SerializeField] private float autoTestSpeed = 2f; // Seconds between auto test actions
    
    // Private variables
    private float currentHealthPercentage = 1f;
    private bool isAutoTesting = false;
    private Coroutine autoTestCoroutine;
    
    void Start()
    {
        SetupButtons();
        
        // Initialize castle bar to full health
        if (castleBarUI != null)
        {
            castleBarUI.SetHealthInstant(1f);
            currentHealthPercentage = 1f;
        }
    }
    
    void Update()
    {
        // Manual keyboard controls for testing
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ApplySmallDamage();
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            ApplyMediumDamage();
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            ApplyLargeDamage();
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            HealCastle();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetHealth();
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetToLowHealth();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            SetToCriticalHealth();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleAutoTest();
        }
    }
    
    private void SetupButtons()
    {
        if (healButton != null)
            healButton.onClick.AddListener(HealCastle);
            
        if (smallDamageButton != null)
            smallDamageButton.onClick.AddListener(ApplySmallDamage);
            
        if (mediumDamageButton != null)
            mediumDamageButton.onClick.AddListener(ApplyMediumDamage);
            
        if (largeDamageButton != null)
            largeDamageButton.onClick.AddListener(ApplyLargeDamage);
            
        if (resetHealthButton != null)
            resetHealthButton.onClick.AddListener(ResetHealth);
            
        if (autoTestButton != null)
            autoTestButton.onClick.AddListener(ToggleAutoTest);
            
        if (setLowHealthButton != null)
            setLowHealthButton.onClick.AddListener(SetToLowHealth);
            
        if (setCriticalHealthButton != null)
            setCriticalHealthButton.onClick.AddListener(SetToCriticalHealth);
    }
    
    public void ApplySmallDamage()
    {
        ApplyDamage(smallDamage);
        Debug.Log($"Applied {smallDamage} damage");
    }
    
    public void ApplyMediumDamage()
    {
        ApplyDamage(mediumDamage);
        Debug.Log($"Applied {mediumDamage} damage");
    }
    
    public void ApplyLargeDamage()
    {
        ApplyDamage(largeDamage);
        Debug.Log($"Applied {largeDamage} damage");
    }
    
    public void HealCastle()
    {
        float previousHealth = currentHealthPercentage;
        currentHealthPercentage = Mathf.Clamp01(currentHealthPercentage + (healAmount / 100f));
        
        if (castleBarUI != null)
        {
            castleBarUI.UpdateCastleHealth(currentHealthPercentage, 0f); // No damage popup for healing
        }
        
        Debug.Log($"Healed castle by {healAmount}%. Health: {previousHealth:P0} → {currentHealthPercentage:P0}");
    }
    
    public void ResetHealth()
    {
        currentHealthPercentage = 1f;
        
        if (castleBarUI != null)
        {
            castleBarUI.SetHealthInstant(1f);
        }
        
        Debug.Log("Castle health reset to 100%");
    }
    
    public void SetToLowHealth()
    {
        currentHealthPercentage = 0.25f; // 25% health
        
        if (castleBarUI != null)
        {
            castleBarUI.UpdateCastleHealth(currentHealthPercentage, 0f);
        }
        
        Debug.Log("Set castle to low health (25%)");
    }
    
    public void SetToCriticalHealth()
    {
        currentHealthPercentage = 0.05f; // 5% health
        
        if (castleBarUI != null)
        {
            castleBarUI.UpdateCastleHealth(currentHealthPercentage, 0f);
        }
        
        Debug.Log("Set castle to critical health (5%)");
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
    
    private void ApplyDamage(float damageAmount)
    {
        float damagePercentage = damageAmount / 100f;
        currentHealthPercentage = Mathf.Clamp01(currentHealthPercentage - damagePercentage);
        
        if (castleBarUI != null)
        {
            castleBarUI.UpdateCastleHealth(currentHealthPercentage, damageAmount);
        }
    }
    
    private IEnumerator AutoTestSequence()
    {
        Debug.Log("=== Auto Test Sequence Started ===");
        
        while (isAutoTesting)
        {
            // Phase 1: Reset to full health
            Debug.Log("Phase 1: Reset to full health");
            ResetHealth();
            yield return new WaitForSeconds(autoTestSpeed);
            
            if (!isAutoTesting) break;
            
            // Phase 2: Apply small damage multiple times
            Debug.Log("Phase 2: Multiple small damages");
            for (int i = 0; i < 3; i++)
            {
                ApplySmallDamage();
                yield return new WaitForSeconds(autoTestSpeed * 0.5f);
                if (!isAutoTesting) break;
            }
            
            if (!isAutoTesting) break;
            yield return new WaitForSeconds(autoTestSpeed);
            
            // Phase 3: Apply medium damage
            Debug.Log("Phase 3: Medium damage");
            ApplyMediumDamage();
            yield return new WaitForSeconds(autoTestSpeed);
            
            if (!isAutoTesting) break;
            
            // Phase 4: Heal a bit
            Debug.Log("Phase 4: Healing");
            HealCastle();
            yield return new WaitForSeconds(autoTestSpeed);
            
            if (!isAutoTesting) break;
            
            // Phase 5: Large damage to trigger low health
            Debug.Log("Phase 5: Large damage (low health)");
            ApplyLargeDamage();
            yield return new WaitForSeconds(autoTestSpeed);
            
            if (!isAutoTesting) break;
            
            // Phase 6: More damage to trigger critical health
            Debug.Log("Phase 6: Critical health");
            ApplyMediumDamage();
            yield return new WaitForSeconds(autoTestSpeed);
            
            if (!isAutoTesting) break;
            
            // Phase 7: Finish with large damage (near death)
            Debug.Log("Phase 7: Near death");
            ApplyLargeDamage();
            yield return new WaitForSeconds(autoTestSpeed * 2);
            
            if (!isAutoTesting) break;
            
            Debug.Log("=== Auto Test Sequence Completed ===");
            yield return new WaitForSeconds(autoTestSpeed);
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
    public void SetSmallDamage(float damage)
    {
        smallDamage = damage;
        Debug.Log($"Small damage set to {damage}");
    }
    
    public void SetMediumDamage(float damage)
    {
        mediumDamage = damage;
        Debug.Log($"Medium damage set to {damage}");
    }
    
    public void SetLargeDamage(float damage)
    {
        largeDamage = damage;
        Debug.Log($"Large damage set to {damage}");
    }
    
    public void SetHealAmount(float heal)
    {
        healAmount = heal;
        Debug.Log($"Heal amount set to {heal}%");
    }
    
    // Status display for debugging
    void OnGUI()
    {
        if (!Application.isPlaying) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 350, 300));
        GUILayout.Label("Castle Bar Tester Status", new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold });
        
        // Health status
        GUILayout.Label($"Current Health: {currentHealthPercentage:P1}");
        
        // Health state indicators
        if (castleBarUI != null)
        {
            GUILayout.Label($"Is Low Health: {castleBarUI.IsLowHealth()}");
            GUILayout.Label($"Is Critical Health: {castleBarUI.IsCriticalHealth()}");
        }
        
        GUILayout.Label($"Auto Testing: {isAutoTesting}");
        
        GUILayout.Space(10);
        
        // Damage settings
        GUILayout.Label("Damage Settings:", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        GUILayout.Label($"Small Damage: {smallDamage}");
        GUILayout.Label($"Medium Damage: {mediumDamage}");
        GUILayout.Label($"Large Damage: {largeDamage}");
        GUILayout.Label($"Heal Amount: {healAmount}%");
        
        GUILayout.Space(10);
        
        // Keyboard controls
        GUILayout.Label("Keyboard Controls:", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        GUILayout.Label("Q - Small Damage");
        GUILayout.Label("W - Medium Damage");
        GUILayout.Label("E - Large Damage");
        GUILayout.Label("H - Heal Castle");
        GUILayout.Label("R - Reset Health");
        GUILayout.Label("L - Set Low Health");
        GUILayout.Label("C - Set Critical Health");
        GUILayout.Label("T - Toggle Auto Test");
        
        GUILayout.EndArea();
    }
}
