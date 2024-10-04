using UnityEngine;

/// <summary>
/// Represents an arrow projectile that travels in a given direction and deals damage upon hitting a player.
/// The arrow is destroyed after a set time or upon collision with a player or a wall.
/// </summary>
public class Arrow : MonoBehaviour
{
    public int damage;              
    public float speed = 10f;       
    public float lifetime = 3f;     

    private Vector2 direction;      

    /// <summary>
    /// Sets the direction for the arrow to travel.
    /// </summary>
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized; 
    }

    /// <summary>
    /// Called when the arrow is instantiated. It schedules the arrow for destruction after its lifetime has passed.
    /// </summary>
    private void Start()
    {
        Destroy(gameObject, lifetime); 
    }

    /// <summary>
    /// Moves the arrow in the set direction every frame.
    /// </summary>
    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime); 
    }

    /// <summary>
    /// Handles collisions with other objects. If the arrow hits a player, it deals damage and is destroyed.
    /// If it hits a wall or another object (like a slash), it is also destroyed.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage); 
            }

            Destroy(gameObject); 
        }
        else if (collision.CompareTag("Wall") || collision.CompareTag("Slash"))
        {
            Destroy(gameObject); 
        }
    }
}
