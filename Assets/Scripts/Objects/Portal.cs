using System.Collections;
using UnityEngine;

/// <summary>
/// Handles player interactions with a portal, including tracking how long the player
/// stays inside the portal and triggering the escape sequence.
/// </summary>
public class Portal : MonoBehaviour
{
    private float playerTimer = 0f; 
    private bool playerInside = false;
    private Coroutine portalCoroutine = null;

    /// <summary>
    /// Detects when the player enters the portal's trigger area, starts the escape coroutine, 
    /// and sets the player inside state.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                playerInside = true;
                portalCoroutine = StartCoroutine(HandlePlayerEscape(player));
            }
        }
    }

    /// <summary>
    /// Detects when the player exits the portal's trigger area, resets the player timer,
    /// stops the escape coroutine, and updates the player inside state.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                playerInside = false;
                playerTimer = 0f; 
                if (portalCoroutine != null)
                {
                    StopCoroutine(portalCoroutine);
                    portalCoroutine = null;
                }
            }
        }
    }

    /// <summary>
    /// Handles the escape process for the player, triggering the escape sequence if the player
    /// remains in the portal for the required duration.
    /// </summary>
    private IEnumerator HandlePlayerEscape(Player player)
    {
        playerTimer = 0f;
        while (playerInside)
        {
            playerTimer += Time.deltaTime;
            if (playerTimer >= 1f)
            {
                UIController.Instance.ShowEscapeScreen();
                yield break; 
            }
            yield return null;
        }
    }
}
