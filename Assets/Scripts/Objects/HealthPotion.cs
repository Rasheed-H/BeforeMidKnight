using UnityEngine;

/// <summary>
/// Represents a health potion that restores a specific amount of health to the player upon collision,
/// provided the player's health is not already full.
/// </summary>
public class HealthPotion : MonoBehaviour
{

    public int healthRestored = 2;
    [SerializeField] private AudioClip healSound;

    /// <summary>
    /// Detects when the player collides with the health potion. Restores health if the player is not at max health,
    /// plays a heal sound effect, and destroys the potion game object.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();

            if (player != null)
            {

                if (player.currentHealth < player.maxHealth)
                {
                    player.Heal(healthRestored);
                    SoundEffects.Instance.PlaySound(healSound);
                    Destroy(gameObject);  
                }
            }
        }
    }
}
