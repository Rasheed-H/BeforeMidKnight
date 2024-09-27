using UnityEngine;

/// <summary>
/// DashHitbox handles the detection of enemies within the player's dash hitbox during a dash and applies damage to them.
/// </summary>
public class DashHitbox : MonoBehaviour
{
    private PlayerController playerController;

    /// <summary>
    /// Called when the script is initialized. Attempts to get a reference to the PlayerController in the parent object.
    /// </summary>
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found in parent.");
        }
    }

    /// <summary>
    /// Called when another object enters the dash hitbox. Applies damage to enemies that collide with the hitbox.
    /// </summary>
    /// <param name="collision">The Collider2D object that has entered the hitbox.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Get the Enemy script from the collided object
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Apply slash damage from PlayerController
                enemy.TakeDamage(playerController.dashDamage);
                Debug.Log("Enemy hit by slash! Damage applied: " + playerController.dashDamage);
            }
        }
    }
}