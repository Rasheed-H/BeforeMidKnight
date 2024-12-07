using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DaggerProjectile class handles the behavior of the dagger projectile, including its lifespan and interactions with enemies and walls.
/// </summary
public class DaggerProjectile : MonoBehaviour
{
    public int damage;
    private float lifeTime = 3f;


    /// <summary>
    /// Called when the dagger projectile is instantiated. Starts a coroutine to destroy the dagger after a set time.
    /// </summary>
    private void Awake()
    {
        StartCoroutine(DestroyAfterTime());
    }


    /// <summary>
    /// Triggered when the dagger collides with another object. Damages enemies and destroys the projectile upon hitting walls or enemies.
    /// </summary>
    /// <param name="collision">The collider that the dagger projectile has interacted with.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Enemy hit by dagger! Damage applied: " + damage);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall") || collision.CompareTag("Box"))
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Coroutine to destroy the dagger projectile after a specified lifetime.
    /// </summary>
    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
