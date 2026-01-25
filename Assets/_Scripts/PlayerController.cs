using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    // Variables
    private InputSystem_Actions m_Actions;
    CharacterController cc;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float timeToJumpApex = 0.4f;

    private float gravity;
    private float initialJumpVelocity;

    private bool jumpPressed = false;

    [Header("Movement Settings")]
    private Vector2 moveInput = Vector2.zero;
    private Vector3 velocity = Vector3.zero;

    [Header("References")]
    public Transform respawnPoint;                  // Reference to the respawn point Transform


    #region Input Handling

    // Called when the script instance is being loaded
    void Awake()
    {
        m_Actions = new InputSystem_Actions();
        m_Actions.Player.SetCallbacks(this);
        m_Actions.Player.Enable();
    }

    // Called when the MonoBehaviour will be destroyed
    void OnDestroy()
    {
        m_Actions.Dispose();
    }

    // Called when the object becomes enabled and active
    void OnEnable()
    {
        m_Actions.Enable();
    }

    // Called when the behaviour becomes disabled or inactive
    void OnDisable()
    {
        m_Actions.Disable();
    }




    // Attack input handling
    public void OnAttack(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }


    // Crouch input handling
    public void OnCrouch(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }


    // Interact input handling
    public void OnInteract(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }


    // Jump input handling
    public void OnJump(InputAction.CallbackContext context)
    {
        jumpPressed = context.ReadValueAsButton();
    }


    // Camera look input handling
    public void OnLook(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }


    // Movement input handling
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            moveInput = context.ReadValue<Vector2>();
            return;
        }

        moveInput = Vector2.zero;
    }


    // Inventory navigation input handling
    public void OnNext(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }


    // Previous inventory navigation input handling
    public void OnPrevious(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }


    // Sprint input handling
    public void OnSprint(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();

        CalculateJumpVariables();
    }



    // Update is called once per frame
    void FixedUpdate()
    {

        UpdateCharacterVelocity();

        cc.Move(velocity * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Player detected a collision with: " + hit.gameObject.name);

        if (hit.gameObject.CompareTag("Killzone"))
        {
            Debug.Log("Player has entered the killzone. Respawning...");
            // Respawn the player at the respawn point
            transform.position = respawnPoint.position;
        }
    }

    // Called when the script is loaded or a value is changed in the inspector (Editor only)
    void OnValidate()
    {
        CalculateJumpVariables();
    }



    void CalculateJumpVariables()
    {
        // Debugging and Logging Statements for Validation for Jump Variables
        if (timeToJumpApex <= 0f)
            throw new System.ArgumentOutOfRangeException("timeToJumpApex must be greater than zero.");

        if (jumpHeight <= 0f)
            throw new System.ArgumentOutOfRangeException("jumpHeight must be greater than zero.");


        // Calculate gravity and initial jump velocity based on jump height and time to apex
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        initialJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }



    void UpdateCharacterVelocity()
    {

        velocity.x = moveInput.x * 5f;
        velocity.z = moveInput.y * 5f;
        
        if (cc.isGrounded)
        {
            velocity.y = cc.skinWidth;
            if (jumpPressed)
            {
                velocity.y = initialJumpVelocity;
            }

        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

    }
}
