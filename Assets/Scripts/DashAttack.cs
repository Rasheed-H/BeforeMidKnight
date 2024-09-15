using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashAttack : MonoBehaviour
{
    public Collider2D playerCollider;   // Player's collider for invincibility during dash
    public Collider2D dashHitbox;       // Hitbox to deal damage during dash
    public float damage = 50f;      // Damage dealt during dash
    public float speed = 80f;       // Speed of the dash
    public float dashDuration = 0.2f;   // How long the dash lasts
    public float invincibilityDuration = 0.3f; // How long the player is invincible during the dash
    private bool isDashing = false;     // Flag to prevent multiple dashes
    private Rigidbody2D rb;             // Rigidbody2D for controlling player movement
    private Vector2 dashEndPoint;       // Endpoint for the dash

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Called from PlayerController to perform the dash attack
    public void PerformDashAttack()
    {
        if (isDashing) return;  // Prevent multiple dashes at once

        // Get mouse position and calculate dash direction
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f; // Keep Z-axis at 0 for 2D

        Vector2 dashDirection = (mousePos - transform.position).normalized;
        dashEndPoint = mousePos;

        // Start the dash
        StartCoroutine(Dash(dashDirection));
    }

    // Coroutine to handle the dash mechanics
    private IEnumerator Dash(Vector2 direction)
    {
        playerCollider.enabled = false;
        isDashing = true;
        dashHitbox.enabled = true;

        float elapsedTime = 0f;

        // Dash towards the end point over the duration of the dash
        while (elapsedTime < dashDuration)
        {
            rb.MovePosition(Vector2.Lerp(rb.position, dashEndPoint, elapsedTime / dashDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure player reaches the exact final dash position
        rb.MovePosition(dashEndPoint);

        // Disable the dash hitbox after the dash ends
        dashHitbox.enabled = false;

        // Wait for the invincibility duration to end before re-enabling the player's normal collider
        yield return new WaitForSeconds(invincibilityDuration);
        playerCollider.enabled = true;

        isDashing = false;
    }

    // Check for collisions with enemies while the dash hitbox is active
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dashHitbox.enabled && collision.CompareTag("Enemy"))
        {
            // Apply damage to the enemy
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Enemy hit by dash! Damage applied: " + damage);
            }
        }
    }
}
