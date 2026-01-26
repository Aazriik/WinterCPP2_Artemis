using System;
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

    }

    // Update is called once per frame
    void Update()
    {
        // Rotate item on Y-Axis, and bob up and down for visual effect
        transform.Rotate(0, 50 * Time.deltaTime, 0);
        float bobHeight = 0.25f;
        transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time * 2) * bobHeight + 1, transform.position.z);

    }
}
