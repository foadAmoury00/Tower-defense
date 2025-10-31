using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using static Utilities.SpringMotion;

[SelectionBase]
public class IsometricCharacterController : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform playerMesh;

    public PlayerInput playerInput;
    private PlayerInputActions playerInputActions;

    private ControlType currentControlType;
    private enum ControlType
    {
        KeyboardAndMouse,
        Gamepad,
    }

    [Header("General Controls")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float speed = 12.0f;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float turnSpeed = 10.0f;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float turnRotationSpeed = 45.0f;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float tiltAngle = 15.0f;

    [Header("Jump Settings")]
    [SerializeField]
    private float gravity = -9.81f;



    [SerializeField]
    [Range(0.0f, 30.0f)]
    private float terminalVelocity = 15.0f;


    [Space]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float coyoteTime;
    private float coyoteTimeCounter;

    [Header("Input Damping")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float inputDampingRotation = 0.1f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float inputDampingMovementBasic = 0.1f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float inputDampingMovementAccel = 0.1f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float inputDampingMovementDecel = 0.05f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float inputDampingMovementTurn = 0.1f;




    [SerializeField]
   
    private Vector3 velocity;
    private Vector3 velocityAfterGlideStart;
    private float additionalVelocity;
    private Vector3 prevVelocity;

    private Vector3 inputVectorRot;
    private Vector3 inputVelocityRot;
    private Vector3 inputVectorMove;
    private Vector3 inputVelocityMove;

   

  
    private float currentDampingMove;
    private float currentDampingRot;

    private float time;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        getCurrentControlType(playerInput);
    }

    private void OnEnable()
    {
        playerInput.onControlsChanged += onControlsChanged;
        playerInputActions.Player.Enable();

        playerInputActions.Player.Move.started += MoveStarted;
    }

    private void OnDisable()
    {
        playerInput.onControlsChanged -= onControlsChanged;
        playerInputActions.Player.Move.started -= MoveStarted;
        playerInputActions.Player.Disable();
    }

    void Update()
    {
        // Check if grounded.
        currentDampingRot = inputDampingRotation;

        // WASD movement.
        Vector3 input = new Vector3(playerInputActions.Player.Move.ReadValue<Vector2>().x, 0.0f, playerInputActions.Player.Move.ReadValue<Vector2>().y);

        if (!playerInputActions.Player.Move.IsInProgress()) // Decelerating
        {
            currentDampingMove = inputDampingMovementDecel;
        }

        if (playerInputActions.Player.Move.IsInProgress() && inputVectorMove.toIso().normalized != Vector3.zero)
        {
            float dot = Vector3.Dot(input.toIso().normalized, inputVectorMove.toIso().normalized);
          
        }
        if (!playerInputActions.Player.Move.IsInProgress() && inputVectorMove.magnitude < 0.01f)
        {
            currentDampingMove = 0;
        }

        // Smoothly interpolate rotation.
        inputVectorRot = Vector3.SmoothDamp(inputVectorRot, input, ref inputVelocityRot, currentDampingRot,  Mathf.Infinity);

        // Smoothly interpolate movement.
        inputVectorMove = Vector3.SmoothDamp(inputVectorMove, input, ref inputVelocityMove, currentDampingMove, turnSpeed);

 
        Debug.DrawRay(transform.position, velocity, Color.yellow);

        // Don't move the player if not movement buttons are being pressed and the movement is not being smoothed.
        if (inputVectorRot != Vector3.zero)
        {
            // .toIso() transforms the input vector to align with the isometric view.
            // LookRotation line will rotate player around the global up axis. This might cause problems when
            // implementing climbing.
            Quaternion targetRot = Quaternion.LookRotation(inputVectorRot.toIso(), Vector3.up); // Returns target rotation.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnRotationSpeed * Time.deltaTime);

            //Calculates the tilt based on user input
            Quaternion targetTilt = Quaternion.Euler(
                inputVectorRot.magnitude * tiltAngle ,
                0.0f,
                (inputVelocityRot.magnitude * 0.2f) *
                (Mathf.Abs(Vector3.Dot(inputVectorRot.normalized, inputVelocityRot.normalized)) >= 0.95f ? 0 : 1) *
                -angleDir(inputVectorRot.normalized, inputVelocityRot.normalized, transform.up) *
                tiltAngle
            );
            playerMesh.rotation = Quaternion.RotateTowards(playerMesh.rotation, targetRot * targetTilt, 100.0f * Time.deltaTime);
        }
        velocity.x = inputVectorMove.toIso().x * speed;
        velocity.z = inputVectorMove.toIso().z * speed;

        // Keep history of grounded in the last x seconds. Called Coyote time.
        coyoteTimeCounter -= Time.deltaTime;
      
       

       

     

      

        velocity.y = Mathf.Clamp(velocity.y, -terminalVelocity, terminalVelocity);

        controller.Move(velocity * Time.deltaTime);
    }

    // Test is the targetDir is pointing either on the left or right side of a transform relative to its forward.
    float angleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 right = Vector3.Cross(up, fwd); // right vector
        float dir = Vector3.Dot(right, targetDir);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

    // Not used
    private void getCurrentControlType(PlayerInput input)
    {
        if (input.currentControlScheme == "Gamepad")
        {
            currentControlType = ControlType.Gamepad;
        }
        else if (input.currentControlScheme == "Keyboard&Mouse")
        {
            currentControlType = ControlType.KeyboardAndMouse;
        }
    }

    private void MoveStarted(InputAction.CallbackContext ctx)
    {
        currentDampingMove = inputDampingMovementAccel; // When starting to move.
    }

 
    private void onControlsChanged(PlayerInput obj)
    {
        getCurrentControlType(obj);
    }
}
