using UnityEngine;

public class Box : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public GameObject itemPrefab;
        public float dropChance; // Percentage chance for this item to drop
    }

    [Header("Box Properties")]
    public bool damaged = false; // Tracks if the box is already damaged
    public Drops[] itemDrops; // List of possible items to drop
    public Transform itemSpawnPoint; // Set in the inspector for item spawn position

    [Header("Drop Settings")]
    public int minDropItems = 1;
    public int maxDropItems = 3;

    private Animator animator;
    private Collider2D boxCollider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<Collider2D>();

        if (itemSpawnPoint == null)
        {
            Debug.LogError("Item Spawn Point is not assigned for the box.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Trigger detected with {collision.tag}");

        // Check for Slash, Dash, or Dagger tags and if not already damaged
        if ((collision.CompareTag("Slash") || collision.CompareTag("Dash") || collision.CompareTag("Dagger")) && !damaged)
        {
            Debug.Log("Triggering Damage animation");
            damaged = true;
            animator.SetTrigger("Damage");
        }
        else if (damaged)
        {
            Debug.Log("Triggering Destroy animation");
            animator.SetTrigger("Destroy");
            boxCollider.enabled = false; // Disable the collider to prevent further interactions
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

        if (totalDropChance != 100f)
        {
            Debug.LogError("Total drop chance for items does not add up to 100%.");
            return;
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
                    // Instantiate item at the defined spawn point
                    Instantiate(item.itemPrefab, itemSpawnPoint.position, Quaternion.identity);
                    break;
                }
            }
        }
    }

}
