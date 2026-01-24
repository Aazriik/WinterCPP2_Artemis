using UnityEngine;

public class Killzone : MonoBehaviour
{
    [Header("References")]
    public GameObject player;           // Reference to the player GameObject
    public Transform respawnPoint;      // Reference to the respawn point Transform

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            player.transform.position = respawnPoint.position;
        }
    }
}
