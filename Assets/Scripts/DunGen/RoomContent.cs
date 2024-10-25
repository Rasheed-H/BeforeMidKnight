using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is responsible for spawning objects and enemies inside a room. It selects random objects
/// and enemies from predefined arrays and positions them randomly within the room bounds. The enemies
/// spawned are registered with the RoomController to manage room states.
/// </summary>
public class RoomContent : MonoBehaviour
{
    [Header("Objects Settings")]
    public List<Spawnable> possibleObjects;  // List of objects with spawn chance
    public int minObjects = 1;
    public int maxObjects = 5;

    [Header("Enemies Settings")]
    public List<Spawnable> possibleEnemies;  // List of enemies with spawn chance
    public int minEnemies = 1;
    public int maxEnemies = 5;

    [Header("Room Size Settings")]
    private float roomWidth = 16f;
    private float roomHeight = 9f;

    public List<Enemy> spawnedEnemies = new List<Enemy>();

    /// <summary>
    /// Called when the scene starts. Spawns the objects and enemies in the room
    /// and registers them with the RoomController.
    /// </summary>
    void Start()
    {
        SpawnObjects();
        SpawnEnemies();

        RoomController roomController = GetComponent<RoomController>();
        roomController.RegisterEnemies(spawnedEnemies);
    }

    /// <summary>
    /// Spawns a random number of objects in the room based on the min and max object count.
    /// Objects are selected from the possibleObjects list with spawn chances.
    /// </summary>
    void SpawnObjects()
    {
        int objectCount = Random.Range(minObjects, maxObjects + 1);

        for (int i = 0; i < objectCount; i++)
        {
            GameObject objectToSpawn = GetRandomSpawn(possibleObjects);

            if (objectToSpawn != null)
            {
                Vector2 spawnPosition = GetRandomPositionInRoom();
                Instantiate(objectToSpawn, spawnPosition, Quaternion.identity, transform);
            }
        }
    }

    /// <summary>
    /// Spawns a random number of enemies in the room based on the min and max enemy count.
    /// Enemies are selected from the possibleEnemies list with spawn chances.
    /// </summary>
    void SpawnEnemies()
    {
        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyToSpawn = GetRandomSpawn(possibleEnemies);

            if (enemyToSpawn != null)
            {
                Vector2 spawnPosition = GetRandomPositionInRoom();
                GameObject spawnedEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity, transform);
                spawnedEnemy.SetActive(false);

                // Register the enemy in the list for the RoomController
                Enemy enemyScript = spawnedEnemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    spawnedEnemies.Add(enemyScript);
                }
            }
        }
    }

    /// <summary>
    /// Returns a random GameObject from the list based on spawn chances.
    /// </summary>
    /// <param name="spawnItems">List of spawn items with their spawn chances.</param>
    /// <returns>The GameObject to spawn or null if none was chosen.</returns>
    GameObject GetRandomSpawn(List<Spawnable> spawnItems)
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

        return null; // No item selected
    }

    /// <summary>
    /// Calculates and returns a random position within the room. The position is adjusted with margins
    /// to ensure objects are not too close to the room's edges.
    /// </summary>
    /// <returns>A random position within the room bounds as a Vector2.</returns>
    Vector2 GetRandomPositionInRoom()
    {
        float marginX = roomWidth * 0.15f;
        float marginY = roomHeight * 0.15f;

        float minX = transform.position.x - (roomWidth / 2) + marginX;
        float maxX = transform.position.x + (roomWidth / 2) - marginX;
        float minY = transform.position.y - (roomHeight / 2) + marginY;
        float maxY = transform.position.y + (roomHeight / 2) - marginY;

        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);

        return new Vector2(x, y);
    }
}

/// <summary>
/// Represents a spawnable item (object or enemy) with a prefab and a chance to spawn.
/// </summary>
[System.Serializable]
public class Spawnable
{
    public GameObject objectPrefab;  // The prefab of the object or enemy to spawn
    public float spawnChance;      // The chance (in percentage) of this item being selected
}
