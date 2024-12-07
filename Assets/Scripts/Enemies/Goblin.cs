using UnityEngine;

/// <summary>
/// Represents a specific type of enemy, the Goblin, which inherits from the base Enemy class. 
/// This class adds specific behavior for the Goblin, such as movement towards the player, sound effects 
/// for taking damage and dying, and handling player collision.
/// </summary>
public class Goblin : Enemy
{
    public AudioClip takeDamageSound;
    public AudioClip deathSound;
    private Vector2 movementDirection;

    protected override void Awake()
    {
        base.Awake();
        speed = GameManager.Instance.goblinSpeed;
        maxHealth = GameManager.Instance.goblinHealth;
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Called every frame to update the Goblin's behavior. If the Goblin is active and not defeated, 
    /// it moves towards the player.
    /// </summary>
    protected void Update()
    {
        if (!isActive || isDefeated)
            return;

        MoveTowardsPlayer();
    }


    /// <summary>
    /// Moves the Goblin towards the player's position. The movement direction is calculated by normalizing 
    /// the vector between the Goblin's position and the player's position. The Goblin's movement is then 
    /// updated using Rigidbody2D, and the movement direction is reflected in the animation and sprite flipping.
    /// </summary>
    void MoveTowardsPlayer()
    {
        if (playerTransform == null)
            return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        movementDirection = direction;

        if (rb != null)
        {
            rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
        }

        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);
        float moveX = 0, moveY = 0;

        if (absX > absY)
        {
            moveX = direction.x > 0 ? 1 : -1;
            moveY = 0;
        }
        else
        {
            moveX = 0;
            moveY = direction.y > 0 ? 1 : -1;
        }

        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", moveY);
        spriteRenderer.flipX = moveX < 0;

    }


    /// <summary>
    /// Handles collision events for the Goblin. If the Goblin collides with the player, it deals damage to the player.
    /// </summary>
    /// <param name="collision">The Collision2D object representing the collision.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isActive || isDefeated)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage); // The player script will handle invincibility frames
            }
        }
    }


    /// <summary>
    /// Reduces the Goblin's health when taking damage. It also plays a sound effect for taking damage, which 
    /// is triggered via the RoomController's audio system.
    /// </summary>
    /// <param name="damageAmount">The amount of damage to apply to the Goblin.</param>
    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
        SoundEffects.Instance.PlaySound(takeDamageSound);
    }


    /// <summary>
    /// Defeats the Goblin by setting its state as defeated, stopping movement, and triggering the death animation. 
    /// It also plays a death sound effect via the RoomController's audio system.
    /// </summary>
    protected override void Defeat()
    {
        base.Defeat();
        SoundEffects.Instance.PlaySound(deathSound);
    }
}