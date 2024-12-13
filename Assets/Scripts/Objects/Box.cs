using UnityEngine;

/// <summary>
/// Represents a destructible box that can drop loot when damaged or destroyed.
/// Tracks its damaged state and handles loot dropping based on predefined drop chances.
/// </summary>
public class Box : MonoBehaviour
{
    /// <summary>
    /// Represents an item drop with a specific prefab and drop chance percentage.
    /// </summary>
    [System.Serializable]
    public class Drops
    {
        public GameObject itemPrefab;
        public float dropChance; 
    }

    [Header("Box Properties")]
    public bool damaged = false; 
    public Drops[] itemDrops; 
    public Transform itemSpawnPoint; 

    [Header("Drop Settings")]
    public int minDropItems = 1;
    public int maxDropItems = 3;

    private Animator animator;
    private Collider2D boxCollider;

    /// <summary>
    /// Initializes the box, setting references and applying the "BoxBreaker" special effect
    /// if it is active.
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<Collider2D>();
        if (GameManager.Instance.IsSpecialEffectActive("BoxBreaker"))
        {
            damaged = true;
        }
    }

    /// <summary>
    /// Detects collisions with player attacks (slash, dash, or dagger) and triggers damage or destroy animations
    /// based on the box's current state.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if ((collision.CompareTag("Slash") || collision.CompareTag("Dash") || collision.CompareTag("Dagger")) && !damaged)
        {
            damaged = true;
            animator.SetTrigger("Damage");
        }
        else if (damaged)
        {
            animator.SetTrigger("Destroy");
            boxCollider.enabled = false; 
        }
    }

    /// <summary>
    /// Drops loot items based on defined drop chances.
    /// Called by an animation event during the destroy animation.
    /// </summary>
    public void DropLoot()
    {
        float totalDropChance = 0f;
        foreach (Drops item in itemDrops)
        {
            totalDropChance += item.dropChance;
        }

        int itemsToDrop = Random.Range(minDropItems, maxDropItems + 1);

        for (int i = 0; i < itemsToDrop; i++)
        {
            float randomValue = Random.Range(0f, 100f);
            float cumulativeChance = 0f;

            foreach (Drops item in itemDrops)
            {
                cumulativeChance += item.dropChance;

                if (randomValue <= cumulativeChance)
                {
                    Instantiate(item.itemPrefab, itemSpawnPoint.position, Quaternion.identity);
                    break;
                }
            }
        }
    }

}
