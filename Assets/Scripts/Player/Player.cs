using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the player's health, movement, attacks, and interactions with the game world.
/// Manages player animations, invincibility after taking damage, and death behavior.
/// </summary>
public class Player : MonoBehaviour
{
    public int maxHealth => (int)GameManager.Instance.GetStat("playerMaxHealth");
    public int currentHealth;
    private float moveSpeed => GameManager.Instance.GetStat("playerMoveSpeed");
    public float invincibilityDurationAfterHit => GameManager.Instance.GetStat("playerInvincibilityDuration");

    private bool isAlive = true;
    public bool isInvincible = false;
    private Vector2 moveInput;
    public Vector2 LastFacedDirection { get; private set; } = Vector2.down;

    public PlayerAttack playerAttack;

    public AudioClip playerDieSound;
    public AudioClip playerHurtSound;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;
    public HeartsManager heartsManager;

    [SerializeField] private AudioClip dieSound;


    /// <summary>
    /// Initializes the player's health, references, and special effects. Sets up hearts and handles visual changes based on special effects.
    /// </summary>
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        heartsManager.InitializeHearts(maxHealth);
        heartsManager.UpdateHearts(currentHealth);

        if (GameManager.Instance.IsSpecialEffectActive("CoolBlueCloak"))
        {
            spriteRenderer.color = new Color32(20, 119, 255, 255); 
        }
    }

    /// <summary>
    /// Updates the player's state each frame, including handling movement if the player is alive.
    /// </summary>
    void Update()
    {
        if (!isAlive) return;
        HandleMovement();
    }

    /// <summary>
    /// Handles player movement based on input, updates the Rigidbody velocity,
    /// and manages animations for movement and idle states.
    /// </summary>
    private void HandleMovement()
    {
        moveInput = UserInput.Instance.MoveInput;
        rb.velocity = moveInput * moveSpeed;

        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool("isRunning", isMoving);

        if (isMoving)
        {
            animator.SetFloat("MoveX", moveInput.x);
            animator.SetFloat("MoveY", moveInput.y);
            LastFacedDirection = moveInput.normalized;
        }
    }

    /// <summary>
    /// Reduces the player's health when taking damage. Triggers invincibility and hurt animations,
    /// and handles death if health reaches zero.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (!isAlive || isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        heartsManager.UpdateHearts(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            SoundEffects.Instance.PlaySound(playerHurtSound);
            animator.SetTrigger("Hurt");
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    /// <summary>
    /// Enables invincibility for a set duration, making the player immune to further damage.
    /// </summary>
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        StartCoroutine(FlashSprite());
        yield return new WaitForSeconds(invincibilityDurationAfterHit);
        isInvincible = false;
    }

    /// <summary>
    /// Temporarily flashes the player's sprite to indicate invincibility after taking damage.
    /// </summary>
    private IEnumerator FlashSprite()
    {
        while (isInvincible)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// Disables all attack hitboxes. Called from animation event.
    /// </summary>
    public void DisableAttackHitboxes()
    {
        playerAttack.DisableAllHitboxes();
    }

    /// <summary>
    /// Restores the player's health by a specified amount, ensuring it does not exceed the maximum health.
    /// Updates the heart UI to reflect the new health.
    /// </summary>
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        heartsManager.UpdateHearts(currentHealth);
    }

    /// <summary>
    /// Handles player death by triggering animations, disabling movement, freezing the player,
    /// and showing the death screen.
    /// </summary>
    public void Die()
    {
        SoundEffects.Instance.PlaySound(dieSound);
        animator.SetTrigger("Die");
        SoundEffects.Instance.PlaySound(playerDieSound);
        isAlive = false;
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        UIController.Instance.ShowDieScreen();
    }

}
