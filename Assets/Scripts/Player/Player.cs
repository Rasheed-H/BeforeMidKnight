using System;
using System.Collections;
using UnityEngine;

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

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        heartsManager.InitializeHearts(maxHealth);
        heartsManager.UpdateHearts(currentHealth);
    }

    void Update()
    {
        if (!isAlive) return;
        HandleMovement();
    }


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

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        StartCoroutine(FlashSprite());
        yield return new WaitForSeconds(invincibilityDurationAfterHit);
        isInvincible = false;
    }

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

    public void DisableAttackHitboxes()
    {
        playerAttack.DisableAllHitboxes();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        heartsManager.UpdateHearts(currentHealth);
    }

    public void Die()
    {
        animator.SetTrigger("Die");
        SoundEffects.Instance.PlaySound(playerDieSound);
        isAlive = false;
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        UIController.Instance.ShowDieScreen();
    }

}
