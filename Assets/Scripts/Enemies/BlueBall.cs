using UnityEngine;

/// <summary>
/// Represents a projectile fired by the wizard enemy, dealing damage to the player on contact
/// and destroying itself after a set lifespan or upon collision with walls.
/// </summary>
public class BlueBall : MonoBehaviour
{
    public int damage = 1;
    public float speed;
    public float lifespan = 5f;

    /// <summary>
    /// Initializes the projectile's speed based on the GameManager and sets a timer to destroy it after its lifespan expires.
    /// </summary>
    private void Start()
    {
        speed = GameManager.Instance.wizardProjSpeed;
        Destroy(gameObject, lifespan); 
    }

    /// <summary>
    /// Handles collision logic for the projectile, dealing damage to the player or destroying itself upon hitting a wall.
    /// </summary>
    /// <param name="collision">The collider the projectile interacts with.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject); 
        }
        else if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
