using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates a dungeon by placing rooms in a grid-based layout.
/// Rooms are placed using crawlers that traverse the grid, and the dungeon layout is randomized.
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    public GameObject startRoomPrefab;    
    public GameObject emptyRoomPrefab;    
    public GameObject treasureRoomPrefab; 
    public GameObject endRoomPrefab;      

    public int minIterations = 10;      
    public int maxIterations = 20;      
    public int numberOfCrawlers = 2;   
    public float treasureRoomChance = 0.1f; 

    public float roomWidth = 25f;       
    public float roomHeight = 15f;       

    // Dictionary to track spawned rooms using their grid positions
    private Dictionary<Vector2Int, GameObject> spawnedRooms = new Dictionary<Vector2Int, GameObject>();

    /// <summary>
    /// A helper class representing a crawler that moves through the grid to place rooms.
    /// </summary>
    private class Crawler
    {
        public Vector2Int position;       
        public Vector2Int lastDirection;    

        public Crawler(Vector2Int startPos)
        {
            position = startPos;
            lastDirection = Vector2Int.zero; 
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
                Vector2Int direction = GetDirection(crawler.lastDirection);
                Vector2Int newPosition = crawler.position + direction;

                crawler.position = newPosition;
                crawler.lastDirection = direction; 

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
    Vector2Int GetDirection(Vector2Int lastDirection)
    {
        List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        if (lastDirection != Vector2Int.zero && Random.value <= 0.6f)
        {
            return lastDirection;
        }

        return directions[Random.Range(0, directions.Count)];
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
    private void UpdateRoomDoors()
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

            roomController.doorTop.SetDoorState(hasUpRoom ? "open" : "none");
            roomController.doorBottom.SetDoorState(hasDownRoom ? "open" : "none");
            roomController.doorLeft.SetDoorState(hasLeftRoom ? "open" : "none");
            roomController.doorRight.SetDoorState(hasRightRoom ? "open" : "none");
        }
    }
}
