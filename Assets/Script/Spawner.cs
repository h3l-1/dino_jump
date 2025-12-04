using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnableObject
    {
        public GameObject prefab;
        [Range(0f, 1f)]
        public float spawnChance;
    }
    
    [Header("Spawn Objects")]
    public SpawnableObject[] objects;
    
    [Header("Spawn Timing")]
    public float minSpawnRate = 1f;
    public float maxSpawnRate = 2f;
    
    [Header("Spawn Heights")]
    private const float GroundY = -1.8f;
    private const float SkyY = 1.6f;
    private const float MeteorY = 8f;
    
    private void OnEnable()
    {
        Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));
    }
    
    private void OnDisable()
    {
        CancelInvoke();
    }
    
    private void Spawn()
    {
        float spawnChance = Random.value;
        
        foreach (SpawnableObject obj in objects)
        {
            if (spawnChance < obj.spawnChance)
            {
                GameObject obstacle = Instantiate(obj.prefab);
                
                // Spawn at certain height depending on type
                Vector3 spawnPos = transform.position;
                
                // Determine Y position based on prefab name
                string prefabName = obj.prefab.name.ToLower();
                
                if (prefabName.Contains("bird"))
                {
                    spawnPos.y = SkyY;
                }
                else if (prefabName.Contains("meteor"))
                {
                    spawnPos.y = MeteorY;
                }
                else
                {
                    spawnPos.y = GroundY;
                }
                
                obstacle.transform.position = spawnPos;
                break;
            }
            
            spawnChance -= obj.spawnChance;
        }
        
        Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));
    }
}