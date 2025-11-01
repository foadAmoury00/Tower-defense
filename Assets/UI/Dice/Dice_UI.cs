using System.Collections;
using UnityEngine;

public class Dice_UI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private CanvasGroup labelCanvasGroup;
    [SerializeField] private Dice dice;

    [Header("Animation Settings")]
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isRolling = false;
    private bool isFading = false;
    private float targetAlpha = 1f;

    // TODO: Replace with actual Dice class reference when available from other branch
    // private Dice diceComponent;

    private PlayerInputActions playerInputActions;

    void OnEnable()
    {
        dice.DiceCompleted += OnDiceRollComplete;
    }
    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
    }

    private void Start()
    {
        // Initialize label as visible
        if (labelCanvasGroup != null)
        {
            labelCanvasGroup.alpha = 1f;
        }

        // TODO: Get dice component when available
        // diceComponent = diceExteranalObject?.GetComponent<Dice>();
    }

    private void Update()
    {
        // Handle R key input for dice rolling
        HandleInput();

        // Handle label fading animation
        HandleLabelFading();
    }

    private void HandleInput()
    {
        if (playerInputActions.Player.RollDice.triggered && !isRolling)
        {
            Debug.Log("R key pressed - starting dice roll.");
            StartDiceRoll();
        }
    }

    private void StartDiceRoll()
    {
        if (dice == null)
        {
            Debug.LogWarning("Dice component not assigned!");
            return;
        }


        isRolling = true;

        // Fade label to transparent quickly
        FadeLabelToTransparent();

        // TODO: Call dice rolling method when Dice class is available
        // Example: diceComponent?.StartRoll();
        // For now, we'll simulate the roll with a placeholder
        StartDiceRollPlaceholder();

        Debug.Log("Dice rolling started!");
    }

    private void FadeLabelToTransparent()
    {
        targetAlpha = 0f;
        isFading = true;
    }

    private IEnumerator FadeLabelToVisible()
    {
        yield return new WaitForSeconds(fadeDuration);
        targetAlpha = 1f;
        isFading = true;
    }

    private void HandleLabelFading()
    {
        if (!isFading || labelCanvasGroup == null)
        {
            return;
        }

        // Lerp towards target alpha
        float currentAlpha = labelCanvasGroup.alpha;
        float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);

        labelCanvasGroup.alpha = newAlpha;

        // Check if fading is complete
        if (Mathf.Approximately(newAlpha, targetAlpha))
        {
            isFading = false;
        }
    }

    void OnDisable()
    {
        dice.DiceCompleted -= OnDiceRollComplete;
    }

    // TODO: Replace this placeholder with actual dice integration
    private void StartDiceRollPlaceholder()
    {
        // Simulate dice roll duration (replace with actual dice roll time)
        dice.RollDice();
    }

    // This method should be called by the dice component when rolling is complete
    public void OnDiceRollComplete()
    {
        isRolling = false;

        // Fade label back to visible
        StartCoroutine( FadeLabelToVisible());

        Debug.Log("Dice rolling completed! Label fading back in.");
    }

    // TODO: Integration methods for when Dice class becomes available
    /*
    private void SetupDiceCallbacks()
    {
        if (diceComponent != null)
        {
            // Example callback setup:
            // diceComponent.OnRollComplete += OnDiceRollComplete;
        }
    }
    
    private void OnDestroy()
    {
        // Clean up callbacks
        if (diceComponent != null)
        {
            // diceComponent.OnRollComplete -= OnDiceRollComplete;
        }
    }
    */
}
