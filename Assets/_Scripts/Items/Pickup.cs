using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Pickup script for handling various item pickups in the game

    #region Variables
    [Header("References")]
    [SerializeField] private GameObject player;                 // Reference to the player GameObject
    private Vector3 startPosition;                              // Initial position for bobbing calculation
    private Rigidbody rb;                                       // Reference to the Rigidbody component
    private Collider col;

    [Header("Pickup Settings")]
    public PickupType pickupType;                               // Type of pickup from enum
    public enum PickupType
    {
        Health,
        Ammo,
        JumpBoost
    }

    [Header("Health Pickup Settings")]
    [SerializeField] private int healthAmount = 20;             // Amount of health restored

    [Header("Ammo Pickup Settings")]
    [SerializeField] private int ammoAmount = 10;               // Amount of ammo restored

    [Header("Jump Boost Pickup Settings")]
    [SerializeField] private float jumpBoostAmount = 2.0f;      // Amount of jump boost

    [Header("Bobbing / Rotation")]
    [SerializeField] private float bobHeight = 0.22f;           // Height of the bobbing motion
    [SerializeField] private float bobSpeed = 2f;               // Speed of the bobbing motion
    [SerializeField] private float rotationSpeed = 50f;         // Rotation speed in degrees per second

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayers = ~0;       // Default: everything
    private float groundCheckMargin = 0.05f;                    // Small margin for raycast

    // State used to allow gravity while falling, then switch to controlled bobbing after landing
    private bool hasLanded = false;                             // Whether the pickup has landed on the ground
    private Vector3 landPosition;                               // Position where the pickup landed
    private float bobStartTime;                                 // Ensures bob begins with zero phase on landing

    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;             // Record initial position for bobbing
        rb = GetComponent<Rigidbody>();                 // Get Rigidbody component
        col = GetComponent<Collider>();                 // Get Collider component

        // Configure Rigidbody for initial falling behavior
        if (rb != null)
        {
            // Allow gravity so pickups fall initially
            rb.isKinematic = false;
            rb.useGravity = true;
            // Interpolate for smoother motion
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            // Keep the item upright while it falls (prevent toppling)
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    // Detect the first collision with ground and start bobbing immediately.
    void OnCollisionEnter(Collision collision)
    {
        if (hasLanded || rb == null)
            return;

        // Only react to configured ground layers
        if (((1 << collision.gameObject.layer) & groundLayers) == 0)
            return;

        // Use the first contact point for placement
        ContactPoint contact = collision.GetContact(0);
        Vector3 contactPoint = contact.point;



        // Compute a robust bottom offset: distance from transform.y to collider bottom in world space
        float bottomOffsetY = 0f;
        if (col != null)
        {
            bottomOffsetY = transform.position.y - col.bounds.min.y + 0.5f;    // Add small margin to avoid clipping
        }


        // Place the object so its bottom sits at the contact point
        landPosition = new Vector3(transform.position.x, contactPoint.y + bottomOffsetY, transform.position.z);

        // Stop motion, prevent further physics-driven rotation, and snap upright
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Switch off gravity and make kinematic to control bobbing with MovePosition/MoveRotation
        rb.useGravity = false;
        rb.isKinematic = true;

        // Snap rotation to upright while preserving current yaw
        float yaw = transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // Use MovePosition to avoid unexpected physics corrections (ensure kinematic before MovePosition)
        rb.MovePosition(landPosition);

        // Start bob phase at zero so there is no initial upward jump from the bob sine
        bobStartTime = Time.time;

        hasLanded = true;
    }

    // Use FixedUpdate for physics checks and movement
    void FixedUpdate()
    {
        if (rb == null)
        {
            // No Rigidbody: fallback to transform-driven bobbing/rotation
            ItemBobbingFallback();
            ItemRotateFallback();
            return;
        }

        if (!hasLanded)
        {
            // Fallback grounded detection in case collision is missed (keeps previous behavior)
            if (IsGrounded())
            {
                // Determine stable land position using collider bottom (more robust than extents alone)
                float bottomOffsetY = 0f;
                if (col != null)
                {
                    bottomOffsetY = transform.position.y - col.bounds.min.y;
                }

                RaycastHit hit;
                float rayDistance = (col != null ? col.bounds.extents.y + 0.5f : 1f);
                if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance, groundLayers, QueryTriggerInteraction.Ignore))
                {
                    landPosition = new Vector3(transform.position.x, hit.point.y + bottomOffsetY, transform.position.z);
                }
                else
                {
                    landPosition = transform.position;
                }

                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                rb.isKinematic = true;

                // Snap upright and position using MovePosition
                float yaw = transform.eulerAngles.y;
                transform.rotation = Quaternion.Euler(0f, yaw, 0f);
                rb.MovePosition(landPosition);

                // Start bob phase at zero so initial bob offset is zero
                bobStartTime = Time.time;

                hasLanded = true;
            }
            // else: still falling - let physics handle it
        }
        else
        {
            // Physics-safe controlled movement for landed pickup (kinematic)
            PhysicsBobbingAndRotation();
        }
    }

    // Update left empty on purpose: physics is handled in FixedUpdate
    void Update()
    {
    }

    void PhysicsBobbingAndRotation()
    {
        // Bob relative to the recorded landing position, using local bob time so first sample is zero
        float t = Time.time - bobStartTime;
        float y = landPosition.y + Mathf.Sin(t * bobSpeed) * bobHeight;
        Vector3 targetPosition = new Vector3(landPosition.x, y, landPosition.z);

        // MovePosition / MoveRotation is appropriate for kinematic rigidbodies
        rb.MovePosition(targetPosition);

        float angle = rotationSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0f, angle, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    bool IsGrounded()
    {
        if (col == null)
        {
            // Conservative check using small downward ray
            return Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayers, QueryTriggerInteraction.Ignore);
        }

        // Raycast from center down to just beyond the collider bottom to see if ground is right below
        float rayDistance = col.bounds.extents.y + groundCheckMargin;
        return Physics.Raycast(transform.position, Vector3.down, rayDistance, groundLayers, QueryTriggerInteraction.Ignore);
    }

    // Fallback methods for non-Rigidbody pickups
    void ItemBobbingFallback()
    {
        float y = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, y, startPosition.z);
    }

    void ItemRotateFallback()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.World);
    }
}
