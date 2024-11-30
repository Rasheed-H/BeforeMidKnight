using System.Collections;
using UnityEngine;

public class Ghast : Enemy
{
    [Header("Ghast Behavior")]
    public float waitTime = 3f; 
    [SerializeField] private float stoppingDistance = 0.2f; 

    private Vector2 targetPosition; 
    private Vector2 movementDirection; 

    private bool isWaiting = true; 
    private bool isMovingToTarget = false; 
    private Coroutine ghastCoroutine;

    protected override void Awake()
    {
        base.Awake();
        speed = 15f; 
        damage = 2;
        maxHealth = 1;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (!isActive || isDefeated)
            return;

        if (isWaiting)
        {
            if (playerTransform != null)
            {
                Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
                UpdateAnimationDirection(directionToPlayer);
            }
        }
        else if (isMovingToTarget)
        {
            MoveToTargetPosition();

            if (Vector2.Distance(transform.position, targetPosition) <= stoppingDistance)
            {
                rb.velocity = Vector2.zero; 
                isMovingToTarget = false;
                isWaiting = true;
                StartWaiting();
            }
        }
    }

    /// <summary>
    /// Activates the Ghast and starts its behavior loop.
    /// </summary>
    public override void Activate()
    {
        base.Activate();
        if (ghastCoroutine == null)
        {
            ghastCoroutine = StartCoroutine(GhastBehavior());
        }
    }

    /// <summary>
    /// Handles the main behavior loop of the Ghast.
    /// </summary>
    private IEnumerator GhastBehavior()
    {
        while (!isDefeated)
        {
            yield return new WaitForSeconds(waitTime);

            if (playerTransform != null)
            {
                targetPosition = playerTransform.position;
                movementDirection = (targetPosition - (Vector2)transform.position).normalized;
                isWaiting = false;
                isMovingToTarget = true;
            }
        }
    }

    /// <summary>
    /// Moves the Ghast toward the target position.
    /// </summary>
    private void MoveToTargetPosition()
    {
        rb.velocity = movementDirection * speed; 
        UpdateAnimationDirection(movementDirection);
    }

    /// <summary>
    /// Starts the waiting period, resetting the velocity and animation.
    /// </summary>
    private void StartWaiting()
    {
        rb.velocity = Vector2.zero; 
        StartCoroutine(WaitBeforeNextMove());
    }

    /// <summary>
    /// Waits for the specified time before allowing the Ghast to move again.
    /// </summary>
    private IEnumerator WaitBeforeNextMove()
    {
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
    }

    /// <summary>
    /// Updates the Ghast's animation direction based on movement.
    /// </summary>
    /// <param name="direction">Direction vector for movement or focus.</param>
    private void UpdateAnimationDirection(Vector2 direction)
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
    /// Handles collision events for the Ghast. Damages the player upon collision.
    /// </summary>
    /// <param name="collision">Collision object.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive || isDefeated)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage); 
            }
        }
    }

    /// <summary>
    /// Defeats the Ghast and handles its destruction.
    /// </summary>
    protected override void Defeat()
    {
        base.Defeat(); 
    }
}
