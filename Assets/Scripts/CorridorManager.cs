using UnityEngine;

public class CorridorManager : MonoBehaviour
{
    [Header("Corridor Settings")]
    [SerializeField] private GameObject corridorSegmentPrefab;
    [SerializeField] private int segmentsAhead = 5; // Number of segments to keep ahead of player
    [SerializeField] private float segmentLength = 20f; // Length of each corridor segment
    
    [Header("References")]
    [SerializeField] private Transform player;
    
    private float nextSpawnZ = 0f;
    private int segmentsSpawned = 0;
    
    void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        
        // Spawn initial segments
        for (int i = 0; i < segmentsAhead + 2; i++)
        {
            SpawnSegment();
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        // Spawn new segments as player moves forward
        if (player.position.z + (segmentsAhead * segmentLength) > nextSpawnZ)
        {
            SpawnSegment();
        }
    }
    
    void SpawnSegment()
    {
        if (corridorSegmentPrefab == null) return;
        
        Vector3 spawnPosition = new Vector3(0, 0, nextSpawnZ);
        GameObject segment = Instantiate(corridorSegmentPrefab, spawnPosition, Quaternion.identity, transform);
        segment.name = $"CorridorSegment_{segmentsSpawned}";
        
        // Toggles walls every 750 meters. Even blocks (0-750, 1500-2250) have walls.
        bool hasWalls = Mathf.FloorToInt(nextSpawnZ / 750f) % 2 == 0;
        if (!hasWalls)
        {
            Transform wallLeft = segment.transform.Find("WallLeft");
            if (wallLeft != null) wallLeft.gameObject.SetActive(false);
            
            Transform wallRight = segment.transform.Find("WallRight");
            if (wallRight != null) wallRight.gameObject.SetActive(false);
        }
        
        nextSpawnZ += segmentLength;
        segmentsSpawned++;
        
        // Add cleanup component to remove old segments
        SegmentCleanup cleanup = segment.AddComponent<SegmentCleanup>();
        cleanup.Initialize(player, segmentLength * 2);
    }
}

// Helper component to clean up old segments
public class SegmentCleanup : MonoBehaviour
{
    private Transform player;
    private float destroyDistance;
    
    public void Initialize(Transform playerTransform, float distance)
    {
        player = playerTransform;
        destroyDistance = distance;
    }
    
    void Update()
    {
        if (player == null) return;
        
        // Destroy segment if it's far behind the player
        if (transform.position.z < player.position.z - destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}
