using System.Collections;
using UnityEngine;

public class Wizard : Enemy
{
    [Header("Wizard Properties")]
    public AudioClip teleportSound;

    [Header("Attack Properties")]
    [SerializeField] private GameObject book; 
    [SerializeField] private GameObject blueBallPrefab;
    [SerializeField] private float fireRate = 2f; 
    private float lastFireTime = 0f;

    [Header("Visual Effects")]
    [SerializeField] private GameObject teleportEffectPrefab;
    [SerializeField] private Color damageFlashColor = new Color(1f, 0.45f, 0.45f); 
    private Color normalColor = Color.white;

    private RoomContent roomContent;
    private float pushThreshold = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        damage = 1;
        maxHealth = 15;
        currentHealth = maxHealth;
        roomContent = GetComponentInParent<RoomContent>();
        if (roomContent == null)
        {
            Debug.LogError("RoomContent not found in Wizard's parent!");
        }
    }

    private void Update()
    {
        if (!isActive || isDefeated)
            return;

        if (rb.velocity.magnitude > pushThreshold)
        {
            rb.velocity = Vector2.zero;
        }

        if (playerTransform != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            UpdateAnimationDirection(directionToPlayer);
            UpdateBookPosition(directionToPlayer);
        }

        if (Time.time >= lastFireTime + fireRate)
        {
            Attack(); 
        }
    }

    /// <summary>
    /// Updates the wizard's idle animation direction based on the player's position.
    /// </summary>
    /// <param name="direction">Direction vector towards the player.</param>
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
    /// Updates the book's position and sorting order based on the wizard's facing direction.
    /// </summary>
    private void UpdateBookPosition(Vector2 direction)
    {
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY) 
        {
            if (direction.x > 0) 
            {
                book.transform.localPosition = new Vector3(0.5f, -0.5f, 0); 
            }
            else 
            {
                book.transform.localPosition = new Vector3(-0.5f, -0.5f, 0); 
            }
            book.GetComponent<SpriteRenderer>().sortingOrder = 1; 
        }
        else // Vertical
        {
            if (direction.y > 0) // Facing up
            {
                book.transform.localPosition = new Vector3(0, 0, 0); 
                book.GetComponent<SpriteRenderer>().sortingOrder = -1; 
            }
            else // Facing down
            {
                book.transform.localPosition = new Vector3(0, -1f, 0); 
                book.GetComponent<SpriteRenderer>().sortingOrder = 1; 
            }
        }
    }

    /// <summary>
    /// Triggers the attack animation and shooting behavior.
    /// </summary>
    private void Attack()
    {
        lastFireTime = Time.time;

        // Play attack animations
        animator.SetTrigger("Shoot");
        book.GetComponent<Animator>().SetTrigger("Shoot");
    }

    /// <summary>
    /// Fires the projectile. This should be triggered by an animation event.
    /// </summary>
    public void FireProjectile()
    {
        if (playerTransform == null)
            return;

        // Instantiate and direct the projectile
        GameObject blueBall = Instantiate(blueBallPrefab, book.transform.position, Quaternion.identity);
        Vector2 directionToPlayer = (playerTransform.position - book.transform.position).normalized;
        blueBall.GetComponent<Rigidbody2D>().velocity = directionToPlayer * 5f; 

        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        blueBall.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// Handles teleporting the wizard to a random position within the room.
    /// </summary>
    private IEnumerator Teleport()
    {

        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.3f);

        if (roomContent != null)
        {
            Bounds roomBounds = roomContent.GetRoomBounds();

            float marginX = roomBounds.size.x * 0.15f;
            float marginY = roomBounds.size.y * 0.15f;

            float minX = roomBounds.min.x + marginX;
            float maxX = roomBounds.max.x - marginX;
            float minY = roomBounds.min.y + marginY;
            float maxY = roomBounds.max.y - marginY;

            Vector2 newPosition = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            transform.position = newPosition;
        }


        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
        }


        spriteRenderer.color = normalColor;
    }


    /// <summary>
    /// Handles damage taken by the wizard and initiates teleporting.
    /// </summary>
    /// <param name="damageAmount">Amount of damage dealt to the wizard.</param>
    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
        if (!isDefeated)
        {
            spriteRenderer.color = damageFlashColor;
            StartCoroutine(Teleport());
        }
    }

    /// <summary>
    /// Defeats the wizard and triggers the death animation.
    /// </summary>
    protected override void Defeat()
    {
        base.Defeat();
    }
}
