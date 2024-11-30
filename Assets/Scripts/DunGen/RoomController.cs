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

    private AudioSource audioSource;
    private Camera mainCamera;
    public float cameraTransitionDuration = 0.5f;

    public DoorController doorTop;
    public DoorController doorBottom;
    public DoorController doorLeft;
    public DoorController doorRight;

    private List<Enemy> enemies = new List<Enemy>();

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
    /// Called when the player enters the room.
    /// </summary>
    /// <param name="other">The collider that triggered this event.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MoveCameraToRoom();
            ActivateEnemies();

            if (AreEnemiesAlive())
            {
                SetAllDoorsState("closed");
            }
            else
            {
                SetAllDoorsState("open");
            }
        }
    }

    /// <summary>
    /// Called when the player exits the room.
    /// </summary>
    /// <param name="other">The collider that triggered this event.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DeactivateEnemies();
        }
    }

    /// <summary>
    /// Registers the enemies in the room.
    /// </summary>
    /// <param name="roomEnemies">List of enemies to register.</param>
    public void RegisterEnemies(List<Enemy> roomEnemies)
    {
        enemies.AddRange(roomEnemies);
    }

    /// <summary>
    /// Called when an enemy is defeated.
    /// Unlocks doors if all enemies are defeated.
    /// </summary>
    public void OnEnemyDefeated()
    {
        if (!AreEnemiesAlive())
        {
            SetAllDoorsState("open");
        }
    }

    /// <summary>
    /// Checks if any enemies in the room are still alive.
    /// </summary>
    /// <returns>True if enemies are alive, false otherwise.</returns>
    private bool AreEnemiesAlive()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && !enemy.IsDefeated())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Activates all enemies in the room.
    /// </summary>
    private void ActivateEnemies()
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
    /// Deactivates all enemies in the room.
    /// </summary>
    private void DeactivateEnemies()
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
    /// Moves the camera smoothly to the room's position.
    /// </summary>
    void MoveCameraToRoom()
    {
        Vector3 offset = new Vector3(-0.49f, -0.5f, 0);
        StartCoroutine(SmoothCameraTransition(offset));
    }

    /// <summary>
    /// Coroutine for moving the camera.
    /// </summary>
    IEnumerator SmoothCameraTransition(Vector3 offset)
    {
        Vector3 startPos = mainCamera.transform.position;
        Vector3 endPos = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z) + offset; 
        float elapsedTime = 0f;

        while (elapsedTime < cameraTransitionDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / cameraTransitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = endPos;
    }


    /// <summary>
    /// Sets the state of all doors in the room.
    /// </summary>
    /// <param name="state">The state to set: "open", "closed", or "none".</param>
    private void SetAllDoorsState(string state)
    {
        doorTop?.SetDoorState(state);
        doorBottom?.SetDoorState(state);
        doorLeft?.SetDoorState(state);
        doorRight?.SetDoorState(state);
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
