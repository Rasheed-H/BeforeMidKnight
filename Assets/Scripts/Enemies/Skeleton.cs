using UnityEngine;

/// <summary>
/// Represents the Skeleton enemy, which moves towards the player, shoots arrows when in range,
/// and flees from the player when they get too close. The Skeleton can also instantiate arrows
/// to shoot towards the player.
/// </summary>
public class Skeleton : Enemy
{
    public AudioClip takeDamageSound;
    public AudioClip deathSound;
    public AudioClip shootSound;

    public float shootingRange = 10f;    
    public float fleeingDistance = 3f;   
    public float fleeDuration = 1f;     
    public float fleeSpeedMultiplier = 2.5f; 
    public float fleeCooldown = 8f;       

    public GameObject arrowPrefab;        
    public Transform arrowSpawnPoint;     
    public float fireRate = 1.5f;         

    private float lastFireTime = 0f;
    private Vector2 movementDirection;
    private bool isInShootingRange = false;
    private bool isFleeing = false;
    private float fleeTimer = 0f;
    private bool isFleeTimerActive = false;
    private float fleeCooldownTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        fireRate = GameManager.Instance.skeletonFireRate;
        maxHealth = GameManager.Instance.skeletonHealth;
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Updates the skeleton's state every frame. 
    /// Handles movement, shooting, and fleeing behavior based on player distance and timers.
    /// </summary>
    protected void Update()
    {
        if (!isActive || isDefeated)
            return;

        if (isFleeTimerActive)
        {
            fleeTimer += Time.deltaTime;
            if (fleeTimer >= fleeDuration)
            {
                isFleeing = false;
                isFleeTimerActive = false;
                fleeTimer = 0f;
                fleeCooldownTimer = fleeCooldown; 
            }
        }
        else if (fleeCooldownTimer > 0)
        {
            fleeCooldownTimer -= Time.deltaTime;
        }

        CheckPlayerDistance();

        if (isFleeing)
        {
            FleeFromPlayer();
        }
        else if (isInShootingRange)
        {
            animator.SetBool("isMoving", false);
            rb.velocity = Vector2.zero; 

            if (Time.time >= lastFireTime + fireRate) 
            {
                ShootAtPlayer();
                lastFireTime = Time.time; 
            }
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    /// <summary>
    /// Moves the skeleton toward the player's position.
    /// </summary>
    void MoveTowardsPlayer()
    {
        movementDirection = (playerTransform.position - transform.position).normalized;
        rb.MovePosition(rb.position + movementDirection * speed * Time.deltaTime);
        animator.SetBool("isMoving", true);
        UpdateAnimationDirection(movementDirection);
    }

    /// <summary>
    /// Makes the skeleton flee from the player's position.
    /// </summary>
    void FleeFromPlayer()
    {
        movementDirection = (transform.position - playerTransform.position).normalized;
        rb.MovePosition(rb.position + movementDirection * speed * fleeSpeedMultiplier * Time.deltaTime);
        animator.SetBool("isMoving", true);
        UpdateAnimationDirection(movementDirection);
    }

    /// <summary>
    /// Updates the skeleton's animation direction based on movement.
    /// </summary>
    /// <param name="direction">Direction vector in which the skeleton is moving.</param>
    void UpdateAnimationDirection(Vector2 direction)
    {
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);
        float moveX = 0, moveY = 0;

        if (absX > absY)
        {
            moveX = direction.x > 0 ? 1 : -1;
            moveY = 0;
            spriteRenderer.flipX = moveX < 0; 
        }
        else
        {
            moveX = 0;
            moveY = direction.y > 0 ? 1 : -1;
        }

        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", moveY);
    }

    /// <summary>
    /// Checks the distance between the skeleton and the player.
    /// Determines if the skeleton should flee or shoot based on range.
    /// </summary>
    void CheckPlayerDistance()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= fleeingDistance && !isFleeTimerActive && fleeCooldownTimer <= 0) 
        {
            isFleeing = true;
            isFleeTimerActive = true;
            fleeTimer = 0f; 
        }
        else if (distanceToPlayer <= shootingRange) 
        {
            isInShootingRange = true;
        }
        else 
        {
            isInShootingRange = false;
        }
    }

    /// <summary>
    /// Shoots an arrow at the player by instantiating the arrow prefab and applying a force in the player's direction.
    /// </summary>
    void ShootAtPlayer()
    {
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        UpdateAnimationDirection(directionToPlayer);
        animator.SetTrigger("Shoot");
    }

    /// <summary>
    /// Instantiates the arrow at the spawn point and shoots it toward the player.
    /// This is called during the shoot animation.
    /// </summary>
    public void FireArrow()
    {
        SoundEffects.Instance.PlaySound(shootSound);
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
        Vector2 direction = (playerTransform.position - arrowSpawnPoint.position).normalized;
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        arrowRb.velocity = direction * 10f; 
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        arrowScript.damage = damage;
    }

    /// <summary>
    /// Reduces the skeleton's health when taking damage and plays the damage sound.
    /// </summary>
    /// <param name="damageAmount">Amount of damage to apply to the skeleton.</param>
    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
        SoundEffects.Instance.PlaySound(takeDamageSound);
    }

    /// <summary>
    /// Defeats the skeleton by playing the death animation and sound.
    /// </summary>
    protected override void Defeat()
    {
        base.Defeat();
        SoundEffects.Instance.PlaySound(deathSound);
    }
}
