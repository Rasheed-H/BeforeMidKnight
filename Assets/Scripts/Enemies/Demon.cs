using System.Collections;
using UnityEngine;

/// <summary>
/// The Demon boss enemy with multiple states (Idle, Shoot, Chase). 
/// Manages state transitions, animations, and interactions with the player.
/// </summary>
public class Demon : Enemy
{
    public enum DemonState { Idle, Shoot, Chase }

    [Header("Idle State")]
    public DemonState currentState = DemonState.Idle;
    public float idleDuration = 6.5f;

    [Header("Shoot State")]
    public float fireRate = 1f;
    public int shotsPerCycle = 2;

    [Header("Chase State")]
    public float stoppingDistance = 0.2f;
    public float pauseTime = 1.5f;
    public float chaseDuration = 4f;

    [Header("Stat Limits")]
    private const float MinIdleDuration = 3.5f;
    private const int MaxShotsPerCycle = 5;
    private const float MinPauseTime = 0.5f;
    private const float MaxChaseDuration = 7f;

    [Header("Demon Components")]
    [SerializeField] private GameObject demonScythePrefab;
    [SerializeField] private Transform scytheSpawnPoint;
    [SerializeField] private GameObject portalPrefab;
    public AudioClip takeDamageSound;
    public AudioClip deathSound;
    public AudioClip shootSound;

    private Coroutine stateCoroutine;
    private Vector2 targetPosition;

    private Vector2 centerOffset = new Vector2(-0.49f, -0.5f);

    /// <summary>
    /// Initializes the Demon's health and prepares it for gameplay based on GameManager settings.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        maxHealth = GameManager.Instance.demonHealth;
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Updates the Demon's state and animation direction relative to the player's position.
    /// </summary>
    private void Update()
    {
        if (!isActive || isDefeated)
            return;

        if (playerTransform != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            UpdateAnimationDirection(directionToPlayer);
        }
    }

    /// <summary>
    /// Activates the Demon and starts the state machine.
    /// </summary>
    public override void Activate()
    {
        base.Activate();
        if (stateCoroutine == null)
        {
            stateCoroutine = StartCoroutine(StateMachine());
        }
    }

    /// <summary>
    /// The main state machine for the Demon.
    /// </summary>
    private IEnumerator StateMachine()
    {
        while (!isDefeated)
        {
            switch (currentState)
            {
                case DemonState.Idle:
                    yield return SafeHandleState(HandleIdleState);
                    break;

                case DemonState.Shoot:
                    yield return SafeHandleState(HandleShootState);
                    break;

                case DemonState.Chase:
                    yield return SafeHandleState(HandleChaseState);
                    break;
            }

            AdjustStats();
        }
    }

    /// <summary>
    /// Adjusts the Demon's stats after each cycle.
    /// </summary>
    private void AdjustStats()
    {
        idleDuration = Mathf.Max(idleDuration - 1f, MinIdleDuration);
        shotsPerCycle = Mathf.Min(shotsPerCycle + 1, MaxShotsPerCycle);
        pauseTime = Mathf.Max(pauseTime - 0.5f, MinPauseTime);
        chaseDuration = Mathf.Min(chaseDuration + 1f, MaxChaseDuration);
    }

    /// <summary>
    /// Safely executes a state coroutine
    /// </summary>
    private IEnumerator SafeHandleState(System.Func<IEnumerator> stateHandler)
    {
        IEnumerator stateCoroutine = null;

        try
        {
            stateCoroutine = stateHandler();
        }
        catch (System.Exception)
        {
            yield break;
        }

        while (true)
        {
            bool moveNext;
            object current;

            try
            {
                if (stateCoroutine == null) yield break;

                moveNext = stateCoroutine.MoveNext();
                current = stateCoroutine.Current;
            }
            catch (System.Exception)
            {
                yield break;
            }

            if (!moveNext) yield break;

            yield return current;
        }
    }

    /// <summary>
    /// Handles the Idle state, moving the Demon to the center and waiting for a specified duration.
    /// </summary>
    private IEnumerator HandleIdleState()
    {
        ResetAnimationParameters();
        yield return StartCoroutine(MoveToCenter());
        yield return new WaitForSeconds(idleDuration);
        currentState = DemonState.Shoot;
    }

    /// <summary>
    /// Handles the Shoot state, triggering attack animations and firing projectiles at the player.
    /// </summary>
    private IEnumerator HandleShootState()
    {
        for (int i = 0; i < shotsPerCycle; i++)
        {
            SoundEffects.Instance.PlaySound(shootSound);
            animator.SetTrigger("Shoot");
            yield return new WaitForSeconds(fireRate);
        }
        currentState = DemonState.Chase;
    }

    /// <summary>
    /// Instantiates and launches a projectile (scythe) in the direction of the player.
    /// </summary>
    public void FireProjectile()
    {
        if (playerTransform == null || demonScythePrefab == null || scytheSpawnPoint == null)
            return;

        Vector2 directionToPlayer = (playerTransform.position - scytheSpawnPoint.position).normalized;

        GameObject scythe = Instantiate(demonScythePrefab, scytheSpawnPoint.position, Quaternion.identity);

        DemonScythe demonScythe = scythe.GetComponent<DemonScythe>();
        if (demonScythe != null)
        {
            demonScythe.Initialize(directionToPlayer);
        }

        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        scythe.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// Handles the Chase state, moving the Demon towards the player and pausing periodically.
    /// </summary>
    private IEnumerator HandleChaseState()
    {
        float chaseTimer = 0f;

        while (chaseTimer < chaseDuration)
        {
            if (playerTransform == null)
                yield break;

            targetPosition = playerTransform.position;

            while (Vector2.Distance(transform.position, targetPosition) > stoppingDistance)
            {
                MoveTowardsTarget(targetPosition);
                yield return null;
            }

            rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(pauseTime);

            chaseTimer += pauseTime;
        }

        currentState = DemonState.Idle;
    }

    /// <summary>
    /// Moves the Demon towards a specified target position while updating its velocity and animations.
    /// </summary>
    /// <param name="target">The target position to move towards.</param>
    private void MoveTowardsTarget(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;
        animator.SetBool("isMoving", true);
    }

    /// <summary>
    /// Moves the Demon to the center of the room during the Idle state.
    /// </summary>
    private IEnumerator MoveToCenter()
    {
        targetPosition = (Vector2)transform.parent.position + centerOffset;

        while (Vector2.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            MoveTowardsTarget(targetPosition);
            yield return null;
        }

        rb.velocity = Vector2.zero;
        ResetAnimationParameters();
    }

    /// <summary>
    /// Detects trigger collisions with the player and applies damage.
    /// </summary>
    private void OnTriggerStay2D(Collider2D collision)
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

    /// <summary>
    /// Updates the Demon's animation direction based on its current movement vector.
    /// </summary>
    /// <param name="direction">Direction vector indicating the movement or facing direction.</param>
    private void UpdateAnimationDirection(Vector2 direction)
    {
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY)
        {
            animator.SetFloat("MoveX", direction.x > 0 ? 1 : -1);
            animator.SetFloat("MoveY", 0);
            spriteRenderer.flipX = direction.x < 0;
        }
        else
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", direction.y > 0 ? 1 : -1);
            spriteRenderer.flipX = false;
        }
    }

    /// <summary>
    /// Resets the Demon's animation parameters to their default values, stopping movement animations.
    /// </summary>
    private void ResetAnimationParameters()
    {
        animator.SetBool("isMoving", false);
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveY", 0);
    }

    /// <summary>
    /// Reduces the Demon's health when damaged, playing a sound effect and triggering damage animations.
    /// </summary>
    /// <param name="damageAmount">The amount of damage dealt to the Demon.</param>
    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
        SoundEffects.Instance.PlaySound(takeDamageSound);
    }

    /// <summary>
    /// Handles the Demon's defeat, including spawning a portal.
    /// </summary>
    protected override void Defeat()
    {
        Instantiate(portalPrefab, transform.position, Quaternion.identity);
        base.Defeat();
        SoundEffects.Instance.PlaySound(deathSound);
        GameManager.Instance.IncrementKillCounter("demon");
    }

}