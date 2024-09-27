using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates a dungeon by placing rooms in a grid-based layout.
/// Rooms are placed using crawlers that traverse the grid, and the dungeon layout is randomized.
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    // Room Prefabs
    public GameObject startRoomPrefab;    
    public GameObject emptyRoomPrefab;    
    public GameObject treasureRoomPrefab; 
    public GameObject endRoomPrefab;      

    // Dungeon Generation Variables
    public int minIterations = 10;      
    public int maxIterations = 20;      
    public int numberOfCrawlers = 2;   
    public float treasureRoomChance = 0.1f; 

    public float roomWidth = 16f;       
    public float roomHeight = 9f;       

    // Dictionary to track spawned rooms using their grid positions
    private Dictionary<Vector2Int, GameObject> spawnedRooms = new Dictionary<Vector2Int, GameObject>();

    /// <summary>
    /// A helper class representing a crawler that moves through the grid to place rooms.
    /// </summary>
    private class Crawler
    {
        public Vector2Int position; 

        public Crawler(Vector2Int startPos)
        {
            position = startPos;
        }
    }


    /// <summary>
    /// Called when the scene starts, initiates the dungeon generation process.
    /// </summary>
    void Start()
    {
        GenerateDungeon(); 
    }


    /// <summary>
    /// Main method to generate the dungeon layout by placing rooms in the grid using crawlers.
    /// </summary>
    void GenerateDungeon()
    {
        Vector2Int startPosition = Vector2Int.zero; 
        SpawnRoom(startPosition, startRoomPrefab);  
        List<Crawler> crawlers = new List<Crawler>();              
        List<Vector2Int> allRoomPositions = new List<Vector2Int>(); 
        allRoomPositions.Add(startPosition);                       

        int totalIterations = Random.Range(minIterations, maxIterations + 1);
        int stepsPerCrawler = totalIterations / numberOfCrawlers;
        Vector2Int lastRoomPosition = startPosition;

        for (int i = 0; i < numberOfCrawlers; i++)
        {
            crawlers.Add(new Crawler(startPosition));
        }


        foreach (Crawler crawler in crawlers)
        {
            for (int step = 0; step < stepsPerCrawler; step++)
            {
                Vector2Int direction = GetRandomDirection();         
                Vector2Int newPosition = crawler.position + direction; 

                crawler.position = newPosition;

                if (!spawnedRooms.ContainsKey(newPosition))
                {
                    GameObject roomPrefab = GetRandomRoomPrefab(); 
                    SpawnRoom(newPosition, roomPrefab);            
                    allRoomPositions.Add(newPosition);             
                    lastRoomPosition = newPosition;
                }
            }
        }

        ReplaceLastRoomWithEndRoom(lastRoomPosition);
        UpdateRoomDoors();
    }


    /// <summary>
    /// Spawns a room at a given grid position.
    /// </summary>
    /// <param name="position">Grid position where the room will be spawned.</param>
    /// <param name="roomPrefab">The room prefab to instantiate.</param>
    void SpawnRoom(Vector2Int position, GameObject roomPrefab)
    {
        Vector3 worldPosition = new Vector3(position.x * roomWidth, position.y * roomHeight, 0);
        GameObject newRoom = Instantiate(roomPrefab, worldPosition, Quaternion.identity);
        newRoom.name = $"Room_{position.x}_{position.y}";
        spawnedRooms.Add(position, newRoom);
    }


    /// <summary>
    /// Returns a random direction (up, down, left, right) for the crawler to move in the grid.
    /// </summary>
    /// <returns>A Vector2Int representing the direction to move in the grid.</returns>
    Vector2Int GetRandomDirection()
    {
        List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,    // (0, 1)
            Vector2Int.down,  // (0, -1)
            Vector2Int.left,  // (-1, 0)
            Vector2Int.right  // (1, 0)
        };

        int index = Random.Range(0, directions.Count);
        return directions[index];
    }


    /// <summary>
    /// Returns a random room prefab based on the predefined chance for a treasure room.
    /// </summary>
    /// <returns>The room prefab to spawn.</returns>
    GameObject GetRandomRoomPrefab()
    {
        float randValue = Random.value; 

        if (randValue <= treasureRoomChance)
        {
            return treasureRoomPrefab; 
        }
        else
        {
            return emptyRoomPrefab; 
        }
    }


    /// <summary>
    /// Replaces the last placed room with the end room (boss room).
    /// </summary>
    /// <param name="lastRoomPosition">The grid position of the last placed room.</param>
    void ReplaceLastRoomWithEndRoom(Vector2Int lastRoomPosition)
    {
        if (spawnedRooms.ContainsKey(lastRoomPosition))
        {
            Destroy(spawnedRooms[lastRoomPosition]);
            spawnedRooms.Remove(lastRoomPosition);

            SpawnRoom(lastRoomPosition, endRoomPrefab);
        }
    }


    /// <summary>
    /// Connects all rooms by activating doors between adjacent rooms.
    /// </summary>
    void UpdateRoomDoors()
    {
        foreach (KeyValuePair<Vector2Int, GameObject> roomEntry in spawnedRooms)
        {
            Vector2Int roomPos = roomEntry.Key;
            GameObject roomObj = roomEntry.Value;
            RoomController roomController = roomObj.GetComponent<RoomController>();

            bool hasUpRoom = spawnedRooms.ContainsKey(roomPos + Vector2Int.up);
            bool hasDownRoom = spawnedRooms.ContainsKey(roomPos + Vector2Int.down);
            bool hasLeftRoom = spawnedRooms.ContainsKey(roomPos + Vector2Int.left);
            bool hasRightRoom = spawnedRooms.ContainsKey(roomPos + Vector2Int.right);

            roomController.SetDoorActive("Up", hasUpRoom);
            roomController.SetDoorActive("Down", hasDownRoom);
            roomController.SetDoorActive("Left", hasLeftRoom);
            roomController.SetDoorActive("Right", hasRightRoom);
        }
    }


    /// <summary>
    /// Adjusts the treasure room chance dynamically.
    /// </summary>
    /// <param name="newChance">The new chance for spawning treasure rooms (value between 0.0 and 1.0).</param>
    public void SetTreasureRoomChance(float newChance)
    {
        treasureRoomChance = newChance;
    }
}
