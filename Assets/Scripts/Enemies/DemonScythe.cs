using UnityEngine;

/// <summary>
/// Represents the projectile fired by the Demon boss. 
/// The scythe moves in a straight line, bounces off walls up to a maximum number of times, and damages the player on contact.
/// </summary>
public class DemonScythe : MonoBehaviour
{
    private Rigidbody2D rb; 
    [SerializeField] private float speed = 8f; 
    [SerializeField] private int maxBounces = 3; 
    [SerializeField] private float lifetime = 8f; 

    private int currentBounces = 0; 
    private Vector2 direction; 


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Initializes the scythe's movement direction and starts its lifetime countdown.
    /// </summary>
    /// <param name="initialDirection">Initial movement direction of the scythe.</param>
    public void Initialize(Vector2 initialDirection)
    {
        direction = initialDirection.normalized;
        rb.velocity = direction * speed; 
        Destroy(gameObject, lifetime); 
    }

    /// <summary>
    /// Handles collision events for bouncing or destroying the scythe.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(1); 
            }
            Destroy(gameObject); 
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            if (currentBounces >= maxBounces)
            {
                Destroy(gameObject);
                return;
            }
            ContactPoint2D contact = collision.contacts[0];
            direction = Vector2.Reflect(direction, contact.normal);
            rb.velocity = direction * speed;

            currentBounces++; 
        }
    }
}
