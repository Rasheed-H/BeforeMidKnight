using UnityEngine;

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
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing from DemonScythe!");
        }
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
        Debug.Log($"Scythe Initialized with Direction: {direction}");
    }

    /// <summary>
    /// Handles collision events for bouncing or destroying the scythe.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
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

            Debug.Log($"Bounce {currentBounces + 1}: New Direction: {direction}");
            currentBounces++; 
        }
    }
}
