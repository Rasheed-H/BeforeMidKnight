using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is responsible for spawning objects or enemies inside a room. It selects random objects 
/// from a predefined array and positions them randomly within the room bounds. The enemies spawned 
/// are registered with the RoomController to manage room states.
/// </summary>
public class RoomContent : MonoBehaviour
{
    public GameObject[] possibleObjects;
    public int minObjects = 1;
    public int maxObjects = 5;
    private float roomWidth = 16f;
    private float roomHeight = 9f;
    public List<Enemy> spawnedEnemies = new List<Enemy>();


    /// <summary>
    /// Called when the scene starts. Spawns the objects or enemies in the room 
    /// and registers them with the RoomController.
    /// </summary>
    void Start()
    {
        SpawnObjects(); 
        RoomController roomController = GetComponent<RoomController>();
        roomController.RegisterEnemies(spawnedEnemies);
    }

    /// <summary>
    /// Spawns a random number of objects (or enemies) in the room between the minimum and maximum count.
    /// Each spawned object is positioned randomly within the room.
    /// </summary>
    void SpawnObjects()
    {
        int objectCount = Random.Range(minObjects, maxObjects + 1);

        for (int i = 0; i < objectCount; i++)
        {
            GameObject objectToSpawn = possibleObjects[Random.Range(0, possibleObjects.Length)];
            Vector2 spawnPosition = GetRandomPositionInRoom();
            GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity, transform);
            spawnedObject.SetActive(false);
            Enemy enemyScript = spawnedObject.GetComponent<Enemy>();
            spawnedEnemies.Add(enemyScript);
            
        }
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
