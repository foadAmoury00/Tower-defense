using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Optional explicit refs (leave blank to auto-find)")]
    public GameObject optionsPanel;
    public GameObject creditsPanel;
    public Slider volumeSlider;

    [Header("Buttons (optional)")]
    public Button btnNewGame, btnOptions, btnCredits, btnQuit, btnOptionsBack, btnCreditsBack;

    [Header("Config")]
    [Tooltip("Scene to load for New Game (add to Build Settings). If empty, will try index 1.")]
    public string newGameScene = "GameScene";
    [Range(0f, 1f)] public float defaultVolume = 0.8f;

    const string VolumeKey = "MasterVolume";

    void Awake()
    {
        EnsureEventSystem();
        AutoFindAll();

        // Panels start hidden
        if (optionsPanel) optionsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);

        // Volume load & bind
        float v = PlayerPrefs.GetFloat(VolumeKey, defaultVolume);
        AudioListener.volume = v;
        if (volumeSlider)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = v;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // Wire buttons (if found)
        if (btnNewGame) btnNewGame.onClick.AddListener(OnNewGame);
        if (btnOptions) btnOptions.onClick.AddListener(() => SetPanel(optionsPanel, true));
        if (btnCredits) btnCredits.onClick.AddListener(() => SetPanel(creditsPanel, true));
        if (btnQuit) btnQuit.onClick.AddListener(OnQuit);
        if (btnOptionsBack) btnOptionsBack.onClick.AddListener(() => SetPanel(optionsPanel, false));
        if (btnCreditsBack) btnCreditsBack.onClick.AddListener(() => SetPanel(creditsPanel, false));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsPanel && optionsPanel.activeSelf) optionsPanel.SetActive(false);
            else if (creditsPanel && creditsPanel.activeSelf) creditsPanel.SetActive(false);
        }
    }

    // ---------- Button handlers ----------
    public void OnNewGame()
    {
        if (!string.IsNullOrEmpty(newGameScene))
        {
            if (Application.CanStreamedLevelBeLoaded(newGameScene))
                SceneManager.LoadScene(newGameScene);
            else
                Debug.LogError($"[MainMenu] Scene '{newGameScene}' is not in Build Settings.");
        }
        else
        {
            // Fallback: try build index 1
            int targetIndex = 1;
            if (SceneManager.sceneCountInBuildSettings > targetIndex)
                SceneManager.LoadScene(targetIndex);
            else
                Debug.LogError("[MainMenu] Build Settings has no scene at index 1.");
        }
    }

    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
    }

    // ---------- Helpers ----------
    void SetPanel(GameObject panel, bool state) { if (panel) panel.SetActive(state); }

    void EnsureEventSystem()
    {
        var es = FindObjectOfType<EventSystem>();
        if (es == null)
        {
            var go = new GameObject("EventSystem");
            es = go.AddComponent<EventSystem>();

            // Try add the right input module depending on your project
            var inputSystemType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemType != null && go.GetComponent(inputSystemType) == null)
            {
                go.AddComponent(inputSystemType);
            }
            else if (go.GetComponent<StandaloneInputModule>() == null)
            {
                go.AddComponent<StandaloneInputModule>();
            }
        }
    }

    void AutoFindAll()
    {
        // Panels by name
        if (!optionsPanel) optionsPanel = GameObject.Find("OptionsPanel");
        if (!creditsPanel) creditsPanel = GameObject.Find("CreditsPanel");
        if (!volumeSlider && optionsPanel) volumeSlider = optionsPanel.GetComponentInChildren<Slider>(true);

        // Buttons under any Canvas
        var canvas = GetComponentInChildren<Canvas>(true);
        if (!canvas) canvas = FindObjectOfType<Canvas>();

        if (!canvas)
        {
            Debug.LogWarning("[MainMenu] No Canvas found. Create one (GameObject → UI → Canvas).");
            return;
        }

        btnNewGame = btnNewGame ? btnNewGame : FindButton(canvas.transform, "Btn_NewGame");
        btnOptions = btnOptions ? btnOptions : FindButton(canvas.transform, "Btn_Options");
        btnCredits = btnCredits ? btnCredits : FindButton(canvas.transform, "Btn_Credits");
        btnQuit = btnQuit ? btnQuit : FindButton(canvas.transform, "Btn_Quit");
        btnOptionsBack = btnOptionsBack ? btnOptionsBack : FindButton(canvas.transform, "Btn_OptionsBack");
        btnCreditsBack = btnCreditsBack ? btnCreditsBack : FindButton(canvas.transform, "Btn_CreditsBack");
    }

    Button FindButton(Transform root, string name)
    {
        var t = FindDeep(root, name);
        return t ? t.GetComponent<Button>() : null;
    }

    Transform FindDeep(Transform root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }
}
