using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    // Item Spawner that controls what items spawn and when.
    // Variables
    [Header("Spawn Settings")]
    [SerializeField] private bool spawnMultipleItems = false;       // If false -> spawn one item at every spawn point. If true -> spawn multiple items per spawn point over time.
    [SerializeField] private int itemsPerSpawnPoint = 2;            // Number of items to spawn per spawn point when spawnMultipleItems is true (adjustable in inspector).
    [SerializeField] private float spawnInterval = 5.0f;            // Time interval between spawns in seconds
    [SerializeField] private GameObject[] itemPrefabs;              // Array of item prefabs to spawn
    [SerializeField] private Transform[] spawnPoints;               // Array of spawn points

    // optional spread to avoid exact overlap when spawning multiple items at a point
    [Header("Spawn Layout")]
    [SerializeField] private float spawnSpread = 0.5f;

    [Header("Coroutine")]
    private Coroutine spawnCoroutine;
    private int[] spawnedCounts;

    // Called when the script instance is being loaded
    private void Awake()
    {
        // Auto populate spawn points from children if none are assigned
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                spawnPoints[i] = transform.GetChild(i);
            }
        }

        // initialize per-spawn-point counters
        spawnedCounts = new int[(spawnPoints != null) ? spawnPoints.Length : 0];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spawnMultipleItems)
        {
            // start periodic spawning; coroutine will stop automatically once each spawn point reaches itemsPerSpawnPoint
            if (spawnCoroutine == null)
                spawnCoroutine = StartCoroutine(SpawnRoutine());
        }
        else
        {
            // initial one-per-spawn-point behavior
            SpawnOneAtEachPoint();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Spawns exactly one item at every spawn point (used when spawnMultipleItems == false)
    private void SpawnOneAtEachPoint()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0 || spawnPoints == null || spawnPoints.Length == 0)
            return;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            SpawnIntoPoint(i);
        }
    }

    // Coroutine that spawns items every spawnInterval seconds; only runs when spawnMultipleItems == true.
    // Stops automatically when every spawn point has spawned itemsPerSpawnPoint items.
    private IEnumerator SpawnRoutine()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0 || spawnPoints == null || spawnPoints.Length == 0)
            yield break;

        // Ensure counters are reset when routine starts
        for (int i = 0; i < spawnedCounts.Length; i++)
            spawnedCounts[i] = 0;

        // Keep spawning until each spawn point has reached the configured limit
        while (true)
        {
            bool allReached = true;

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnedCounts[i] < Mathf.Max(1, itemsPerSpawnPoint))
                {
                    SpawnIntoPoint(i);
                    spawnedCounts[i]++;
                }

                if (spawnedCounts[i] < Mathf.Max(1, itemsPerSpawnPoint))
                    allReached = false;
            }

            if (allReached)
                break;

            yield return new WaitForSeconds(spawnInterval);
        }

        spawnCoroutine = null;
    }

    // Helper that spawns a single item at the given spawn point index with a small randomized offset and rotation.
    private void SpawnIntoPoint(int pointIndex)
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0 || spawnPoints == null || pointIndex < 0 || pointIndex >= spawnPoints.Length)
            return;

        GameObject itemToSpawn = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

        // small randomized spread to reduce overlap when multiple items spawn at same point
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(0f, spawnSpread);
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
        Vector3 spawnPos = spawnPoints[pointIndex].position + offset;

        // Randomize yaw so spawned items don't all face the same way
        Quaternion spawnRot = spawnPoints[pointIndex].rotation * Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        Instantiate(itemToSpawn, spawnPos, spawnRot);
    }

    // Optional: call this to stop the periodic spawning at runtime
    public void StopPeriodicSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
}
