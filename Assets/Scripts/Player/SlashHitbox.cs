using UnityEngine;

/// <summary>
/// SlashHitbox handles detecting enemies that enter the player's slash hitbox and applies damage to them.
/// </summary>
public class SlashHitbox : MonoBehaviour
{
    private PlayerAttack playerAttack;

    /// <summary>
    /// Called when the script is initialized. Attempts to get a reference to the PlayerController in the parent object.
    /// </summary>
    void Start()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
    }

    /// <summary>
    /// Called when another object enters the slash hitbox. Applies damage to enemies that collide with the hitbox.
    /// </summary>
    /// <param name="collision">The Collider2D object that has entered the hitbox.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(playerAttack.slashDamage);
            }
        }
        else if (collision.CompareTag("EnemyProjectile") && GameManager.Instance.IsSpecialEffectActive("ShotSlayer"))
        {
            Destroy(collision.gameObject);
        }
    }
}