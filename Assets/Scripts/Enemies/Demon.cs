using System.Collections;
using UnityEngine;

/// <summary>
/// The Demon boss enemy with multiple states: Idle, Shoot, and Chase.
/// </summary>
public class Demon : Enemy
{
    public enum DemonState { Idle, Shoot, Chase }

    [Header("Demon Properties")]
    public DemonState currentState = DemonState.Idle;
    public float idleDuration = 5f;
    public float chaseDuration = 8f;
    public float fireRate = 1f; 
    public int shotsPerCycle = 3;

    [Header("Attack Properties")]
    [SerializeField] private GameObject demonScythePrefab; 
    [SerializeField] private Transform scytheSpawnPoint;

    [Header("Chase Properties")]
    public float stoppingDistance = 0.2f;
    public float pauseTime = 1f; 

    private Coroutine stateCoroutine; 
    private Vector2 targetPosition; 
    private bool isMoving = false; 

    private Vector2 centerOffset = new Vector2(-0.49f, -0.5f); 

    protected override void Awake()
    {
        base.Awake();
        damage = 1;
        maxHealth = 50;
        currentHealth = maxHealth;
        speed = 15f;

        if (scytheSpawnPoint == null)
        {
            Debug.LogError("Scythe Spawn Point is not assigned!");
        }

        if (demonScythePrefab == null)
        {
            Debug.LogError("Demon Scythe Prefab is not assigned!");
        }
    }

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
        }
    }

    /// <summary>
    /// Safely executes a state coroutine and logs exceptions if they occur.
    /// </summary>
    private IEnumerator SafeHandleState(System.Func<IEnumerator> stateHandler)
    {
        IEnumerator stateCoroutine = null;

        try
        {
            stateCoroutine = stateHandler();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error preparing state '{currentState}': {ex.Message}\n{ex.StackTrace}");
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
            catch (System.Exception ex)
            {
                Debug.LogError($"Error during execution of state '{currentState}': {ex.Message}\n{ex.StackTrace}");
                yield break;
            }

            if (!moveNext) yield break;

            yield return current;
        }
    }

    /// <summary>
    /// Handles the Demon in the Idle state.
    /// </summary>
    private IEnumerator HandleIdleState()
    {
        DebugState("Idle");
        ResetAnimationParameters();
        yield return StartCoroutine(MoveToCenter());
        yield return new WaitForSeconds(idleDuration);
        currentState = DemonState.Shoot;
    }

    /// <summary>
    /// Handles the Demon in the Shoot state.
    /// </summary>
    private IEnumerator HandleShootState()
    {
        DebugState("Shoot");
        for (int i = 0; i < shotsPerCycle; i++)
        {
            animator.SetTrigger("Shoot");
            yield return new WaitForSeconds(fireRate);
        }
        currentState = DemonState.Chase;
    }

    /// <summary>
    /// Fires the scythe projectile.
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
    /// Handles the Demon in the Chase state.
    /// </summary>
    private IEnumerator HandleChaseState()
    {
        DebugState("Chase");
        float chaseTimer = 0f;

        while (chaseTimer < chaseDuration)
        {
            if (playerTransform == null)
                yield break;

            targetPosition = playerTransform.position;

            while (Vector2.Distance(transform.position, targetPosition) > stoppingDistance)
            {
                isMoving = true;
                MoveTowardsTarget(targetPosition);
                yield return null;
            }

            isMoving = false;
            rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(pauseTime);

            chaseTimer += pauseTime;
        }

        currentState = DemonState.Idle;
    }

    /// <summary>
    /// Moves the Demon towards the specified target position.
    /// </summary>
    private void MoveTowardsTarget(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;
        animator.SetBool("isMoving", true);
    }

    /// <summary>
    /// Moves the Demon to the center of the room.
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
    /// Updates the Demon's animation direction.
    /// </summary>
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
    /// Detects trigger collisions with the player and applies damage.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage); 
                Debug.Log("Player hit by Demon!");
            }
        }
    }

    /// <summary>
    /// Resets animation parameters to idle.
    /// </summary>
    private void ResetAnimationParameters()
    {
        animator.SetBool("isMoving", false);
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveY", 0);
    }

    /// <summary>
    /// Logs the current state of the Demon.
    /// </summary>
    private void DebugState(string stateName)
    {
        Debug.Log($"Demon State: {stateName}, Position: {transform.position}");
    }
}
