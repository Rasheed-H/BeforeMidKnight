using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the chest's opening behavior, item drop logic, and interaction with the player.
/// The chest can drop random loot items based on predefined chances when opened.
/// </summary>
public class Chest : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public GameObject itemPrefab;
        public float dropChance;
    }

    public Drops[] itemDrops;
    public int minDropItems = 1;
    public int maxDropItems = 3;

    private Animator animator;
    private bool isOpened = false;

    /// <summary>
    /// Initializes the animator component.
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Detects when the player collides with the chest and initiates the opening if not already opened.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !isOpened)
        {
            OpenChest();
        }
    }

    /// <summary>
    /// Handles chest opening, triggers the animation, and prevents re-opening.
    /// </summary>
    private void OpenChest()
    {
        isOpened = true;
        animator.SetTrigger("open");
    }

    /// <summary>
    /// Spawns loot items based on defined drop chances. Called by an animation event to synchronize with chest opening.
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
                    Vector2 dropPosition = (Vector2)transform.position + Random.insideUnitCircle * 1.5f;
                    Instantiate(item.itemPrefab, dropPosition, Quaternion.identity);
                    break;
                }
            }
        }

        GetComponent<Collider2D>().isTrigger = true;
    }
}
