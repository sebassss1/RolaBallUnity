using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private float spawnDistance = 50f; // How far ahead to spawn
    [SerializeField] private float baseSpawnInterval = 2f; // Base time between spawns
    [SerializeField] private float minSpawnInterval = 0.5f; // Minimum time between spawns
    
    [Header("Spawn Area")]
    [SerializeField] private float spawnRangeX = 3f; // Random X position range
    [SerializeField] private float spawnHeight = 0.5f; // Y position for obstacles
    
    [Header("Obstacle Variety")]
    [SerializeField] private Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private Vector3 maxScale = new Vector3(1.5f, 2f, 1.5f);
    
    private Transform player;
    private float nextSpawnTime;
    private float currentSpawnInterval;
    
    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        currentSpawnInterval = baseSpawnInterval;
        nextSpawnTime = Time.time + currentSpawnInterval;
    }
    
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;
        if (player == null) return;
        
        // Update spawn interval based on difficulty
        float difficulty = GameManager.Instance != null ? GameManager.Instance.GetCurrentDifficulty() : 1f;
        currentSpawnInterval = Mathf.Max(baseSpawnInterval / difficulty, minSpawnInterval);
        
        // Spawn obstacles at intervals
        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacle();
            nextSpawnTime = Time.time + currentSpawnInterval;
        }
    }
    
    void SpawnObstacle()
    {
        if (obstaclePrefab == null || player == null) return;
        
        // Calculate spawn position ahead of player
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPosition = new Vector3(
            randomX,
            spawnHeight,
            player.position.z + spawnDistance
        );
        
        // Instantiate obstacle
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        
        // Random scale for variety
        Vector3 randomScale = new Vector3(
            Random.Range(minScale.x, maxScale.x),
            Random.Range(minScale.y, maxScale.y),
            Random.Range(minScale.z, maxScale.z)
        );
        obstacle.transform.localScale = randomScale;
        
        // Random rotation for visual variety
        obstacle.transform.rotation = Quaternion.Euler(
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f)
        );
    }
}
