using UnityEngine;

/// <summary>
/// Controls the behavior of the spike trap, including damage and animation-based activation.
/// </summary>
public class Spike : MonoBehaviour
{
    [Header("Spike Properties")]
    [SerializeField] private int damage = 1; 
    private BoxCollider2D spikeCollider; 

    private void Awake()
    {
        spikeCollider = GetComponent<BoxCollider2D>();

        if (spikeCollider == null)
        {
            Debug.LogError("Spike Collider is missing!");
        }

        spikeCollider.enabled = false;
    }

    /// <summary>
    /// Activates the spike's collider. Called from an animation event.
    /// </summary>
    public void ActivateCollider()
    {
        spikeCollider.enabled = true;
    }

    /// <summary>
    /// Deactivates the spike's collider. Called from an animation event.
    /// </summary>
    public void DeactivateCollider()
    {
        spikeCollider.enabled = false;
    }

    /// <summary>
    /// Detects collisions with the player and applies damage.
    /// </summary>
    /// <param name="collision">The collider of the object that entered the spike's trigger.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
