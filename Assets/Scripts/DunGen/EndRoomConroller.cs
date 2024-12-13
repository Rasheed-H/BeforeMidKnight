using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the portal in the end room. Enables the portal if no demon is spawned.
/// This was added due to a rare bug when the demon doesn't spawn in the room but we 
/// still need a way for the player to escape in case this happens.
/// </summary>
public class EndRoomController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The portal prefab that will be enabled if no demon is spawned.")]
    public GameObject portal;

    [Header("Room Settings")]
    [Tooltip("Indicates whether a demon was successfully spawned in the room.")]
    public bool demonSpawned = false;

    [Tooltip("Delay (in seconds) before checking for the demon.")]
    public float checkDelay = 0.5f;

    /// <summary>
    /// Called when the player enters the room. Checks the status of the demon and enables the portal if needed.
    /// </summary>
    /// <param name="other">The collider that triggered this event.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || !demonSpawned)
        {
            StartCoroutine(CheckForDemonAfterDelay());
        }
    }

    /// <summary>
    /// Delays the check for the demon by a specified amount of time.
    /// </summary>
    private IEnumerator CheckForDemonAfterDelay()
    {
        yield return new WaitForSeconds(checkDelay);
        CheckForDemon();
    }

    /// <summary>
    /// Checks if the demon is present as a child of this room and sets the demonSpawned flag.
    /// </summary>
    private void CheckForDemon()
    {

        foreach (Transform child in transform)
        {
            if (child.CompareTag("Enemy"))
            {
                demonSpawned = true;
                return;
            }
        }

        if (!demonSpawned)
        {
            portal.SetActive(true);
            return;
        }


    }

}
