using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private float playerTimer = 0f; 
    private bool playerInside = false;
    private Coroutine portalCoroutine = null;

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

    private IEnumerator HandlePlayerEscape(Player player)
    {
        playerTimer = 0f;
        while (playerInside)
        {
            playerTimer += Time.deltaTime;
            if (playerTimer >= 1.5f)
            {
                Debug.Log("Player escaped through the portal!");
                UIController.Instance.ShowEscapeScreen();
                yield break; 
            }
            yield return null;
        }
    }
}
