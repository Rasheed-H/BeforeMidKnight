using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject startRoomPrefab;
    public GameObject treasureRoomPrefab;
    public GameObject endRoomPrefab;
    public List<GameObject> emptyRoomPrefabs;

    private int dungeonCrawlerCount = 2;

    public float roomWidth = 25f;
    public float roomHeight = 15f;

    private Dictionary<Vector2Int, GameObject> spawnedRooms = new Dictionary<Vector2Int, GameObject>();

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

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        int maxIterations = GameManager.Instance.dungeonRoomCount; 
        float treasureRoomChance = GameManager.Instance.GetStat("treasureRoomChance");

        Vector2Int startPosition = Vector2Int.zero;
        SpawnRoom(startPosition, startRoomPrefab);

        List<Crawler> crawlers = new List<Crawler>();
        List<Vector2Int> allRoomPositions = new List<Vector2Int>();
        allRoomPositions.Add(startPosition);

        int stepsPerCrawler = maxIterations / dungeonCrawlerCount;
        Vector2Int lastRoomPosition = startPosition;

        for (int i = 0; i < dungeonCrawlerCount; i++)
        {
            crawlers.Add(new Crawler(startPosition));
        }

        foreach (Crawler crawler in crawlers)
        {
            int step = 0;

            while (step < stepsPerCrawler)
            {
                Vector2Int direction = GetDirection(crawler.lastDirection);
                Vector2Int newPosition = crawler.position + direction;

                crawler.position = newPosition;
                crawler.lastDirection = direction;

                if (!spawnedRooms.ContainsKey(newPosition))
                {
                    GameObject roomPrefab = GetRandomRoomPrefab(treasureRoomChance);
                    SpawnRoom(newPosition, roomPrefab);
                    allRoomPositions.Add(newPosition);
                    lastRoomPosition = newPosition;


                    step++;
                }
            }
        }

        ReplaceLastRoomWithEndRoom(lastRoomPosition);
        UpdateRoomDoors();
    }

    void SpawnRoom(Vector2Int position, GameObject roomPrefab)
    {
        Vector3 worldPosition = new Vector3(position.x * roomWidth, position.y * roomHeight, 0);
        GameObject newRoom = Instantiate(roomPrefab, worldPosition, Quaternion.identity);
        newRoom.name = $"Room_{position.x}_{position.y}";
        spawnedRooms.Add(position, newRoom);
    }

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

    GameObject GetRandomRoomPrefab(float treasureRoomChance)
    {
        if (Random.value <= treasureRoomChance)
        {
            return treasureRoomPrefab;
        }
        else
        {
            // Select a random empty room prefab
            int randomIndex = Random.Range(0, emptyRoomPrefabs.Count);
            return emptyRoomPrefabs[randomIndex];
        }
    }

    void ReplaceLastRoomWithEndRoom(Vector2Int lastRoomPosition)
    {
        if (spawnedRooms.ContainsKey(lastRoomPosition))
        {
            Destroy(spawnedRooms[lastRoomPosition]);
            spawnedRooms.Remove(lastRoomPosition);

            SpawnRoom(lastRoomPosition, endRoomPrefab);
        }
    }

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
