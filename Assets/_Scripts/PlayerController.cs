using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    // Variables
    private InputSystem_Actions m_Actions;                          // Input action asset reference
    CharacterController cc;                                         // Reference to the CharacterController component

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2f;                 // Desired jump height
    [SerializeField] private float timeToJumpApex = 0.4f;           // Time to reach the apex of the jump

    private float gravity;                                          // Calculated gravity value
    private float initialJumpVelocity;                              // Calculated initial jump velocity

    private bool jumpPressed = false;                               // Flag to track if jump input is pressed

    [Header("Movement Settings")]
    private Vector2 moveInput = Vector2.zero;                       // Movement input vector
    private Vector3 velocity = Vector3.zero;                        // Character velocity vector

    [Header("References")]
    public Transform respawnPoint;                                  // Reference to the respawn point Transform


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
        //Debug.Log("Player detected a collision with: " + hit.gameObject.name);


        // Handle killzone collisions
        if (hit.gameObject.CompareTag("Killzone"))
        {
            Debug.Log("Player has entered the killzone. Respawning...");
            // Respawn the player at the respawn point
            transform.position = respawnPoint.position;
        }


        // Handle pickup collisions
        if (hit.gameObject.CompareTag("Pickup"))
        {

            // Check for Pickup component
            Debug.Log("Player has collided with a pickup item.");
            Pickup pickup = hit.gameObject.GetComponent<Pickup>();

            // If Pickup component exists, handle the pickup
            if (pickup != null)
            {
                PickUpType(pickup.pickupType.ToString());
                Destroy(hit.gameObject);
            }

            // If Pickup component is missing, log a warning
            else
            {
                Debug.LogWarning("Pickup component not found on the collided object.");
            }
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

    #region Pickup Handling
    void PickUpType(string type)
    {
        // Handle different pickup types
        switch (type)
        {
            case "Health":
                Debug.Log("Picked up Health!");
                break;

            case "Ammo":
                Debug.Log("Picked up Ammo!");
                break;

            case "JumpBoost":
                Debug.Log("Picked up Jump Boost! JumpHeight increased to 4 for 10 seconds!");

                // Effect: Increase jump height to 4 for 10 seconds
                StartCoroutine(JumpBoostEffect(4f, 10f));
                break;

            default:
                Debug.Log("Unknown pickup type.");
                break;

        }
    }
    

    System.Collections.IEnumerator JumpBoostEffect(float boostedJumpHeight, float duration)
    {
        float originalJumpHeight = jumpHeight;
        jumpHeight = boostedJumpHeight;
        CalculateJumpVariables();
        yield return new WaitForSeconds(duration);
        jumpHeight = originalJumpHeight;
        CalculateJumpVariables();
    }
    #endregion
}
