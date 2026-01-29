using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2f;                 // Desired jump height
    [SerializeField] private float timeToJumpApex = 0.4f;           // Time to reach the apex of the jump

    private float gravity;                                          // Calculated gravity value
    private float initalJumpVelocity;                               // Calculated initial jump velocity

    private bool jumpPressed = false;                               // Flag to track if jump input is pressed

    [Header("Movement Settings")]
    private Vector2 moveInput = Vector2.zero;                       // Movement input vector
    private Vector3 velocity = Vector3.zero;                        // Character velocity vector

    [Header("References")]
    CharacterController cc;                                         // Reference to the CharacterController component
    public Transform respawnPoint;                                  // Reference to the respawn point Transform

    private LayerMask stairsLayer;


    #region Input Handling
    void OnEnable()
    {
        InputManager.Instance.OnMoveEvent += OnMove;
        InputManager.Instance.OnJumpEvent += OnJump;
    }
    void OnDisable()
    {
        InputManager.Instance.OnMoveEvent -= OnMove;
        InputManager.Instance.OnJumpEvent -= OnJump;
    }

    void OnMove(Vector2 input) => moveInput = input;
    void OnJump(bool pressed) => jumpPressed = pressed;
    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
        CalculateJumpVariables();

        stairsLayer = LayerMask.GetMask("Stairs");
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
        initalJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    // Update is called once per frame
    private void Update()
    {
        Ray newRay = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        Debug.DrawRay(newRay.origin, newRay.direction * 10.0f, Color.red, 0.1f);
        bool hitSomething = Physics.Raycast(newRay, out hitInfo, 10.0f, stairsLayer);
        if (hitSomething)
        {
            Debug.Log("Stairs detected: " + hitInfo.collider.gameObject.name);
        }

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        initalJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateCharacterVelocity();

        cc.Move(velocity * Time.fixedDeltaTime);
    }

    void UpdateCharacterVelocity()
    {
        velocity.x = moveInput.x * 5f;
        velocity.z = moveInput.y * 5f;

        if (cc.isGrounded)
        {
            velocity.y = -cc.skinWidth;
            if (jumpPressed)
            {
                velocity.y = initalJumpVelocity;
            }
        }
        else
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
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
