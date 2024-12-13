using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// This script is responsible for spawning and managing objects and enemies inside a room. 
/// It ensures that objects and enemies are activated and deactivated based on the player's presence in the room.
/// </summary>
public class RoomContent : MonoBehaviour
{
    [Header("Objects Settings")]
    public List<Spawnable> possibleObjects;
    public int minObjects = 1;
    public int maxObjects = 5;

    [Header("Enemies Settings")]
    public List<Spawnable> possibleEnemies;
    public int minEnemies = 1;
    public int maxEnemies = 5;

    [Header("Room Size Settings")]
    private float roomWidth = 25f;
    private float roomHeight = 15f;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<Enemy> spawnedEnemies = new List<Enemy>();


    /// <summary>
    /// Initializes and spawns objects and enemies in the room.
    /// </summary>
    private void Start()
    {
        SpawnObjects();
        SpawnEnemies();
    }

    /// <summary>
    /// Spawns a random number of objects in the room based on the min and max object count.
    /// </summary>
    private void SpawnObjects()
    {
        int objectCount = Random.Range(minObjects, maxObjects + 1);

        for (int i = 0; i < objectCount; i++)
        {
            GameObject objectToSpawn = GetRandomSpawn(possibleObjects);

            if (objectToSpawn != null)
            {
                Vector2 spawnPosition = GetRandomGroundPosition();
                if (spawnPosition != Vector2.zero) 
                {
                    GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity, transform);
                    spawnedObject.SetActive(false); 
                    spawnedObjects.Add(spawnedObject);
                }
            }
        }
    }

    /// <summary>
    /// Spawns a random number of enemies in the room based on the min and max enemy count.
    /// </summary>
    private void SpawnEnemies()
    {
        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyToSpawn = GetRandomSpawn(possibleEnemies);

            if (enemyToSpawn != null)
            {
                Vector2 spawnPosition = GetRandomGroundPosition();
                if (spawnPosition != Vector2.zero) 
                {
                    GameObject spawnedEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity, transform);
                    spawnedEnemy.SetActive(false); 

                    // Register the enemy
                    Enemy enemyScript = spawnedEnemy.GetComponent<Enemy>();
                    if (enemyScript != null)
                    {
                        spawnedEnemies.Add(enemyScript);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Activates all enemies and objects in the room.
    /// </summary>
    public void ActivateContent()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            obj?.SetActive(true);
        }

        StartCoroutine(ActivateEnemiesWithDelay());
    }

    /// <summary>
    /// Coroutine to activate enemies one by one with a delay.
    /// </summary>
    private IEnumerator ActivateEnemiesWithDelay()
    {
        yield return new WaitForSeconds(0.7f);

        foreach (Enemy enemy in spawnedEnemies)
        {
            if (enemy != null && !enemy.IsDefeated())
            {
                yield return new WaitForSeconds(0.2f);
                enemy.Activate();
            }
        }
    }

    /// <summary>
    /// Deactivates all enemies and objects in the room.
    /// </summary>
    public void DeactivateContent()
    {
        foreach (Enemy enemy in spawnedEnemies)
        {
            if (enemy != null && !enemy.IsDefeated())
            {
                enemy.Deactivate();
            }
        }

        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                if (obj.CompareTag("Spike"))
                {
                    obj.SetActive(false); 
                }
            }
        }
    }

    /// <summary>
    /// Returns a random GameObject from the list based on spawn chances.
    /// </summary>
    private GameObject GetRandomSpawn(List<Spawnable> spawnItems)
    {
        float randomValue = Random.Range(0f, 100f);
        float cumulativeChance = 0f;

        foreach (Spawnable item in spawnItems)
        {
            cumulativeChance += item.spawnChance;
            if (randomValue <= cumulativeChance)
            {
                return item.objectPrefab;
            }
        }

        return null;
    }

    /// <summary>
    /// Calculates and returns a random position within the room on the Ground layer.
    /// </summary>
    /// <returns>A random position within the room bounds as a Vector2, or Vector2.zero if no valid position is found.</returns>
    private Vector2 GetRandomGroundPosition()
    {
        int maxAttempts = 10;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float marginX = roomWidth * 0.20f;
            float marginY = roomHeight * 0.20f;

            float minX = transform.position.x - (roomWidth / 2) + marginX;
            float maxX = transform.position.x + (roomWidth / 2) - marginX;
            float minY = transform.position.y - (roomHeight / 2) + marginY;
            float maxY = transform.position.y + (roomHeight / 2) - marginY;

            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);

            Vector2 position = new Vector2(x, y);

            if (Physics2D.OverlapPoint(position, LayerMask.GetMask("Ground")) &&
                !Physics2D.OverlapPoint(position, LayerMask.GetMask("Air")))
            {
                return position;
            }
        }

        return Vector2.zero;
    }

    public Bounds GetRoomBounds()
    {
        Vector2 roomCenter = transform.position;
        Vector2 size = new Vector2(roomWidth, roomHeight);
        return new Bounds(roomCenter, size);
    }

    /// <summary>
    /// Provides the list of spawned enemies for external use.
    /// </summary>
    public List<Enemy> GetEnemies()
    {
        return spawnedEnemies;
    }
}

/// <summary>
/// Represents a spawnable item (object or enemy) with a prefab and a chance to spawn.
/// </summary>
[System.Serializable]
public class Spawnable
{
    public GameObject objectPrefab;
    public float spawnChance;
}


