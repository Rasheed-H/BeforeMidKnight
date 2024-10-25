using UnityEngine;

[System.Serializable]
public class Drops
{
    public GameObject itemPrefab;  // The item prefab (coin, etc.)
    public float dropChance;       // The chance of dropping this item
}

/// <summary>
/// The base class for enemies in the game. This class handles common enemy behavior, such as taking damage,
/// activation/deactivation, and defeat. It also provides a structure for future enemy scaling (adjusting stats).
/// Derived enemy classes can inherit from this base and extend its functionality.
/// </summary>
public class Enemy : MonoBehaviour
{
    
    [SerializeField] protected float speed = 1f;       
    [SerializeField] protected int damage = 1;         
    [SerializeField] protected int maxHealth = 1;      

    protected int currentHealth;
    protected bool isActive = false;
    protected bool isDefeated = false;
    protected RoomController roomController;           
    protected Transform playerTransform;               
    protected Rigidbody2D rb;                         
    protected Animator animator;                     
    protected SpriteRenderer spriteRenderer;

    [Header("Drop Settings")]
    public Drops[] itemDrops;  // Array of potential drops (coins)


    /// <summary>
    /// Called when the enemy is initialized. It sets up basic components such as Rigidbody, Animator, 
    /// and the player's reference. This method ensures that each enemy has access to the RoomController 
    /// and is aware of the player’s position.
    /// </summary>
    protected virtual void Awake()
    {
        roomController = GetComponentInParent<RoomController>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found.");
        }
    }


    /// <summary>
    /// Activates the enemy, making it visible and able to perform actions. This method is called when the 
    /// player enters the room, and enemies start reacting to player movement.
    /// </summary>
    public virtual void Activate()
    {
        isActive = true;
        gameObject.SetActive(true);
    }


    /// <summary>
    /// Deactivates the enemy, making it inactive and hidden from the scene. This is triggered when the player 
    /// leaves the room, causing the enemies to stop all movement and actions.
    /// </summary>
    public virtual void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
    }


    /// <summary>
    /// Reduces the enemy’s health by a specified amount and triggers a 'TakeDamage' animation. If the health reaches zero, 
    /// the enemy is defeated.
    /// </summary>
    /// <param name="damageAmount">The amount of damage to apply to the enemy.</param>
    public virtual void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        animator.SetTrigger("TakeDamage");
        if (currentHealth <= 0)
        {
            Defeat();
        }
    }


    /// <summary>
    /// Defeats the enemy, stopping all movement and interactions. This method triggers the 'Die' animation, 
    /// disables the enemy's collider, and informs the RoomController that the enemy has been defeated.
    /// </summary>
    protected virtual void Defeat()
    {
        isDefeated = true;
        isActive = false;
        animator.SetTrigger("Die");
        rb.velocity = Vector2.zero;
        Collider2D col = GetComponent<Collider2D>();
        col.enabled = false;
        roomController.OnEnemyDefeated();
        DropItem();
        Destroy(gameObject, 1f);
    }

    /// <summary>
    /// Randomly chooses a coin to drop based on drop chances.
    /// </summary>
    public void DropItem()
    {
        float randomValue = Random.Range(0f, 100f);  // Random value between 0 and 100
        float cumulativeChance = 0f;

        foreach (Drops drop in itemDrops) // Loop through each possible drop
        {
            cumulativeChance += drop.dropChance;

            if (randomValue <= cumulativeChance)  // If within this drop's chance, drop it
            {
                Instantiate(drop.itemPrefab, transform.position, Quaternion.identity);
                return;  // Exit after dropping one item
            }
        }

        // No coin will drop if the randomValue is greater than the cumulative chances of all items
        // This means the chance for nothing to drop is the remaining percent (100% - cumulativeChance)
    }



    /// <summary>
    /// Checks whether the enemy has been defeated. Returns true if the enemy is dead, false otherwise.
    /// </summary>
    /// <returns>Boolean indicating the defeated status of the enemy.</returns>
    public bool IsDefeated()
    {
        return isDefeated;
    }

    /// <summary>
    /// Scales the enemy's stats based on a multiplier. This can be used to dynamically adjust enemy difficulty
    /// as the player progresses through the game. The method adjusts the enemy’s health, damage, and speed.
    /// </summary>
    /// <param name="multiplier">The factor by which to scale the enemy's stats.</param>
    public virtual void ScaleStats(float multiplier)
    {
        maxHealth = Mathf.RoundToInt(maxHealth * multiplier);
        currentHealth = maxHealth;                            
        damage = Mathf.RoundToInt(damage * multiplier);       
        speed *= multiplier;                                  

        Debug.Log($"{gameObject.name} scaled! New stats - Health: {maxHealth}, Damage: {damage}, Speed: {speed}");
    }
}
