using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controls room behaviors including managing doors, enemies, and camera transitions. 
/// It handles room state when the player enters and exits a room, including locking doors 
/// if enemies are present and unlocking them when all enemies are defeated.
/// </summary>
public class RoomController : MonoBehaviour
{

    public GameObject[] doors; 
    public GameObject[] doorGapColliders;
    private List<Enemy> enemies = new List<Enemy>();
    private AudioSource audioSource;
    private Camera mainCamera;
    public float cameraTransitionDuration = 0.5f;


    /// <summary>
    /// Called when the room controller is initialized. 
    /// Sets up the camera and audio source references.
    /// </summary>
    void Awake()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
    }


    /// <summary>
    /// Called when the room starts.
    /// Unlocks all doors in the room at the start.
    /// </summary>
    void Start()
    {
        UnlockDoors();
    }


    /// <summary>
    /// Called when the player enters the room.
    /// Moves the camera to the current room, activates the enemies,
    /// and locks the doors if there are enemies alive.
    /// </summary>
    /// <param name="other">The collider that triggered this event (in this case, the player).</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MoveCameraToRoom();
            ActivateEnemies();

            if (AreEnemiesAlive())
            {
                Debug.Log("Enemies alive in room: " + gameObject.name + ", locking doors.");
                LockDoors();
            }
            else
            {
                Debug.Log("No enemies alive in room: " + gameObject.name + ", doors remain unlocked.");
                UnlockDoors();
            }
        }
    }


    /// <summary>
    /// Called when the player exits the room.
    /// Deactivates enemies and unlocks the doors when the player leaves.
    /// </summary>
    /// <param name="other">The collider that triggered this event (in this case, the player).</param>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DeactivateEnemies();
            UnlockDoors();
        }
    }


    /// <summary>
    /// Registers the enemies in the room by adding them to the list of enemies.
    /// </summary>
    /// <param name="roomEnemies">A list of enemies to register in the room.</param>
    public void RegisterEnemies(List<Enemy> roomEnemies)
    {
        enemies.AddRange(roomEnemies);
    }


    /// <summary>
    /// Called when an enemy is defeated.
    /// Unlocks the doors if there are no remaining alive enemies in the room.
    /// </summary>
    public void OnEnemyDefeated()
    {
        if (!AreEnemiesAlive())
        {
            UnlockDoors();
        }
    }


    /// <summary>
    /// Checks if any enemies in the room are still alive.
    /// </summary>
    /// <returns>Returns true if any enemies are alive, false otherwise.</returns>
    public bool AreEnemiesAlive()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy && !enemy.IsDefeated())
            {
                return true; 
            }
        }
        return false; 
    }


    /// <summary>
    /// Moves the camera smoothly to the current room's position.
    /// </summary>
    void MoveCameraToRoom()
    {
        StartCoroutine(SmoothCameraTransition());
    }


    /// <summary>
    /// Coroutine that moves the camera to the room's position over a set duration.
    /// </summary>
    /// <returns>An IEnumerator for Unity's coroutine system.</returns>
    IEnumerator SmoothCameraTransition()
    {
        Vector3 startPos = mainCamera.transform.position;
        Vector3 endPos = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
        float elapsedTime = 0f;

        while (elapsedTime < cameraTransitionDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPos, endPos, (elapsedTime / cameraTransitionDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = endPos;
    }


    /// <summary>
    /// Locks all doors in the room, preventing player movement between rooms.
    /// </summary>
    void LockDoors()
    {
        foreach (GameObject door in doors)
        {
            if (!door.activeInHierarchy)
                continue;

            DoorController doorController = door.GetComponent<DoorController>();
            doorController.SetDoorState(false); 
        }
    }


    /// <summary>
    /// Unlocks all doors in the room, allowing player movement between rooms.
    /// </summary>
    void UnlockDoors()
    {
        foreach (GameObject door in doors)
        {
            if (!door.activeInHierarchy)
                continue;
            DoorController doorController = door.GetComponent<DoorController>();
            doorController.SetDoorState(true); 
        }
    }


    /// <summary>
    /// Activates all enemies in the room, enabling their behavior and movement.
    /// </summary>
    void ActivateEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && !enemy.IsDefeated())
            {
                enemy.Activate();
            }
        }
    }


    /// <summary>
    /// Deactivates all enemies in the room, stopping their behavior and movement.
    /// </summary>
    void DeactivateEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && !enemy.IsDefeated())
            {
                enemy.Deactivate();
            }
        }
    }


    /// <summary>
    /// Activates or deactivates doors based on the direction and whether adjacent rooms exist.
    /// </summary>
    /// <param name="direction">The direction of the door to activate (e.g., "Up", "Down").</param>
    /// <param name="isActive">Whether to activate or deactivate the door.</param>
    public void SetDoorActive(string direction, bool isActive)
    {
        foreach (GameObject door in doors)
        {
            if (door.name.Contains(direction))
            {
                door.SetActive(isActive);
            }
        }

        foreach (GameObject doorGapCollider in doorGapColliders)
        {
            if (doorGapCollider.name.Contains(direction))
            {
                Collider2D collider = doorGapCollider.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = !isActive; 
                }
            }
        }
    }


    /// <summary>
    /// Plays a sound clip using the room's audio source.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip); 
        }
    }
}
