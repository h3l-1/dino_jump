using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnableObject
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float spawnChance;
    }

    public SpawnableObject[] obstacles;
    
    [Header("Settings")]
    public Transform spawnPoint; 
    
    [Range(0f, 0.5f)] 
    public float noSpawnChance = 0.05f; // 5% chance to skip a spawn cycle
    
    [Header("Spawn Position Y-Coordinates")]
    private const float GroundY = -1.8f;
    private const float SkyY = 1.8f;
    private const float MeteorY = 19.7f;
    
    // Chrome Logic: Minimum time between spawns decreases as speed increases,
    // BUT distance must remain jumpable.
    public float minSpawnDistance = 10f; // Minimum world units between obstacles
    public float distanceVariance = 5f;  // Random extra distance
    
    private float nextSpawnTime;

    void Start()
    {
        ScheduleNextSpawn();
    }

    void Update()
    {
        if (GameManager.Instance.isGameOver) return;

        if (Time.time >= nextSpawnTime)
        {
            Spawn();
            ScheduleNextSpawn();
        }
    }

    void Spawn()
    {
        // Safety Check: We MUST have a spawnPoint assigned in the Inspector
        if (obstacles == null || obstacles.Length == 0 || spawnPoint == null) 
        {
            ScheduleNextSpawn();
            return;
        }

        GameObject prefabToSpawn = PickRandomPrefab();
        
        if (prefabToSpawn != null)
        {
            // Use the X-position from the assigned spawnPoint
            Vector3 spawnPosition = spawnPoint.position;
            
            // Determine Y-position based on prefab name (Cactus/Ground, Bird/Sky, Meteor)
            string prefabName = prefabToSpawn.name.ToLower();
            
            if (prefabName.Contains("cactus") || prefabName.Contains("ground"))
            {
                spawnPosition.y = GroundY;
            }
            else if (prefabName.Contains("bird") || prefabName.Contains("sky"))
            {
                spawnPosition.y = SkyY;
            }
            else if (prefabName.Contains("meteor"))
            {
                spawnPosition.y = MeteorY;
            }
            else
            {
                // Default to GroundY if name is ambiguous
                spawnPosition.y = GroundY; 
            }

            // Instantiate at the calculated position
            GameObject instance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            
            // Ensure it has the Obstacle script
            if (instance.GetComponent<Obstacle>() == null)
            {
                instance.AddComponent<Obstacle>();
            }
        }
    }

    void ScheduleNextSpawn()
    {
        // Time = Distance / Speed
        float currentSpeed = GameManager.Instance.gameSpeed;
        if (currentSpeed <= 0) currentSpeed = 1f; // Safety

        // Calculate required distance to be playable
        float distance = minSpawnDistance + Random.Range(0, distanceVariance);
        
        // Calculate how long that distance takes at current speed
        float timeDelay = distance / currentSpeed;
        
        nextSpawnTime = Time.time + timeDelay;
    }

    GameObject PickRandomPrefab()
    {
        if (obstacles.Length == 0) return null;

        float totalObstacleChance = 0;
        foreach (var obj in obstacles) totalObstacleChance += obj.spawnChance;

        // The total probability space is 1.0 (totalObstacleChance + noSpawnChance)
        float totalSpace = totalObstacleChance + noSpawnChance;
        
        // Pick a random point in the combined probability space
        float randomPoint = Random.Range(0, totalSpace);
        
        // If the random point falls into the 'no spawn' range, return null (5% skip chance)
        if (randomPoint >= totalObstacleChance)
        {
            return null;
        }
        
        // Otherwise, iterate through obstacles based on the random point
        float cumulativeChance = 0;
        for (int i = 0; i < obstacles.Length; i++)
        {
            cumulativeChance += obstacles[i].spawnChance;
            if (randomPoint < cumulativeChance)
            {
                return obstacles[i].prefab;
            }
        }
        
        // Fallback: If calculation precision causes issues, spawn the first item.
        return obstacles[0].prefab;
    }
}