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

    /*[Header("Audio")]
    public AudioClip pickupSound;
    private AudioSource audioSource; */


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
        ItemBobbing();
        ItemRotate();

    }

    // Update is called once per frame
    void Update()
    {   

    }

    void ItemBobbing()
    {
        // Implement bobbing logic if needed
        // Keep position in the world, but bob up and down.
        float bobHeight = 0.25f;
        transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time * 2) * bobHeight + 1.0f, transform.position.z);
    }

    void ItemRotate()
    {
        // Implement rotation logic if needed
        transform.Rotate(Vector3.up * Time.deltaTime * 50f);

    }
}
