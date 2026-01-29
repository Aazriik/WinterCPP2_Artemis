using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Pickup script for handling various item pickups in the game

    // Variables
    [Header("References")]
    public GameObject player;

    [Header("Pickup Settings")]
    public PickupType pickupType;
    public enum PickupType
    {
        Health,
        Ammo,
        JumpBoost
    }

    [Header("Health Pickup Settings")]
    public int healthAmount = 20;

    [Header("Ammo Pickup Settings")]
    public int ammoAmount = 10;

    [Header("Jump Boost Pickup Settings")]
    public float jumpBoostAmount = 2.0f;

    [Header("Bobbing / Rotation")]
    public float bobHeight = 0.25f;
    public float bobSpeed = 2f;
    public float rotationSpeed = 50f;

    [Header("Ground Detection")]
    public LayerMask groundLayers = ~0; // default: everything
    public float groundCheckMargin = 0.05f; // small margin for raycast

    private Vector3 startPosition;
    private Rigidbody rb;
    private Collider col;

    // State used to allow gravity while falling, then switch to controlled bobbing after landing
    private bool hasLanded = false;
    private Vector3 landPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (rb != null)
        {
            // Start with gravity enabled so pickups fall
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
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
            // Check if we've contacted the ground so we can switch to bobbing relative to that position
            if (IsGrounded())
            {
                // Determine a stable land position (respecting collider extents)
                Vector3 bottomOffset = Vector3.zero;
                if (col != null)
                {
                    bottomOffset = Vector3.up * col.bounds.extents.y;
                }

                // Try a raycast to get exact contact point for more precise placement
                RaycastHit hit;
                float rayDistance = (col != null ? col.bounds.extents.y + 0.5f : 1f);
                if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance, groundLayers, QueryTriggerInteraction.Ignore))
                {
                    landPosition = new Vector3(transform.position.x, hit.point.y + bottomOffset.y, transform.position.z);
                }
                else
                {
                    landPosition = transform.position;
                }

                // Switch to kinematic so we can control bobbing via MovePosition/MoveRotation without gravity interference.
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                rb.isKinematic = true;
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
        // Bob relative to the recorded landing position
        float y = landPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
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
