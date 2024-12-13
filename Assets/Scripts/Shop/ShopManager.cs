using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the shop system, including generating daily and weekly items, displaying available items,
/// and handling item purchases and UI interactions.
/// </summary>
public class ShopManager : MonoBehaviour
{
    [Header("Rarity Chances")]
    [Range(0f, 1f)] public float commonChance = 0.6f;
    [Range(0f, 1f)] public float rareChance = 0.3f;
    [Range(0f, 1f)] public float mythicalChance = 0.1f;

    [Header("Item Pools")]
    public List<ItemData> commonItems = new List<ItemData>();
    public List<ItemData> rareItems = new List<ItemData>();
    public List<ItemData> mythicalItems = new List<ItemData>();

    [Header("Shop UI References")]
    public Transform dailyItemsContainer;
    public Transform weeklyItemContainer;
    public GameObject itemContainerPrefab;
    public GameObject shopMenuUI;


    [Header("Item Info Panel References")]
    public GameObject itemInfoPanel;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public Transform selectedItemContainer;
    public Button purchaseButton;

    private ItemData selectedItem;
    public TMP_Text totalCoinsText;

    [SerializeField] private AudioClip purchaseSound;
    [SerializeField] private AudioClip deniedSound;
    [SerializeField] private AudioClip buttonClickSound;


    /// <summary>
    /// Initializes the shop by generating daily and weekly items if they are not already set,
    /// and saves the game state.
    /// </summary>
    private void Start()
    {
        if (GameManager.Instance.dailyItems == null || GameManager.Instance.dailyItems.Count == 0)
        {
            GameManager.Instance.dailyItems = GenerateDailyItems().Select(item => item.itemName).ToList();
            GameManager.Instance.SaveGameState();
        }

        if (string.IsNullOrEmpty(GameManager.Instance.weeklyItem))
        {
            GameManager.Instance.weeklyItem = GenerateWeeklyItem()?.itemName;
            GameManager.Instance.SaveGameState();
        }
    }

    /// <summary>
    /// Generates the daily item selection based on rarity chances and unpurchased items,
    /// ensuring unique items are displayed each day.
    /// </summary>
    private List<ItemData> GenerateDailyItems()
    {
        List<ItemData> dailyItems = new List<ItemData>();
        int totalSlots = 4;

        List<ItemData> allItems = commonItems.Concat(rareItems).Concat(mythicalItems).ToList();

        bool allItemsPurchased = allItems.All(item => GameManager.Instance.purchasedItems.Contains(item.itemName));

        if (allItemsPurchased)
        {
            for (int i = 0; i < totalSlots; i++)
            {
                dailyItems.Add(allItems[Random.Range(0, allItems.Count)]);
            }
            return dailyItems;
        }

        var unpurchasedItems = allItems
            .Where(item => !GameManager.Instance.purchasedItems.Contains(item.itemName))
            .ToList();

        unpurchasedItems = unpurchasedItems.OrderBy(_ => Random.value).ToList();

        foreach (var item in unpurchasedItems)
        {
            if (dailyItems.Count >= totalSlots) break;
            if (!dailyItems.Any(d => d.itemName == item.itemName))
            {
                dailyItems.Add(item);
            }
        }

        while (dailyItems.Count < totalSlots)
        {
            var remainingItems = allItems
                .Where(item => !dailyItems.Any(d => d.itemName == item.itemName))
                .ToList();

            if (remainingItems.Count == 0)
            {
                remainingItems = allItems; 
            }

            dailyItems.Add(remainingItems[Random.Range(0, remainingItems.Count)]);
        }

        return dailyItems;
    }


    /// <summary>
    /// Selects a weekly item from the pool of mythical items, prioritizing unpurchased items.
    /// </summary>
    private ItemData GenerateWeeklyItem()
    {
        var unpurchased = mythicalItems
            .Where(item => !GameManager.Instance.purchasedItems.Contains(item.itemName))
            .ToList();

        if (unpurchased.Count > 0)
        {
            return unpurchased[Random.Range(0, unpurchased.Count)];
        }

        return mythicalItems.Count > 0 ? mythicalItems[Random.Range(0, mythicalItems.Count)] : null;
    }

    /// <summary>
    /// Populates the shop UI with daily and weekly items, resolving their data and
    /// marking purchased items appropriately.
    /// </summary>
    public void PopulateShop()
    {
        foreach (Transform child in dailyItemsContainer)
            Destroy(child.gameObject);
        foreach (Transform child in weeklyItemContainer)
            Destroy(child.gameObject);

        foreach (var itemName in GameManager.Instance.dailyItems)
        {
            var item = ResolveItemByName(itemName);
            if (item != null)
            {
                var itemUI = Instantiate(itemContainerPrefab, dailyItemsContainer).GetComponent<ItemContainerUI>();
                itemUI.Setup(item, OpenItemInfoPanel);
                itemUI.SetPurchased(GameManager.Instance.purchasedItems.Contains(itemName));
            }
        }

        if (!string.IsNullOrEmpty(GameManager.Instance.weeklyItem))
        {
            var weeklyItem = ResolveItemByName(GameManager.Instance.weeklyItem);
            if (weeklyItem != null)
            {
                var weeklyItemUI = Instantiate(itemContainerPrefab, weeklyItemContainer).GetComponent<ItemContainerUI>();
                weeklyItemUI.transform.localScale = new Vector3(1.15f, 1.15f, 1f);
                weeklyItemUI.Setup(weeklyItem, OpenItemInfoPanel);
                weeklyItemUI.SetPurchased(GameManager.Instance.purchasedItems.Contains(GameManager.Instance.weeklyItem));
            }
        }
    }

    /// <summary>
    /// Resolves an item's data by its name, searching through all item pools.
    /// </summary>
    public ItemData ResolveItemByName(string itemName)
    {
        foreach (var item in commonItems)
            if (item.itemName == itemName) return item;

        foreach (var item in rareItems)
            if (item.itemName == itemName) return item;

        foreach (var item in mythicalItems)
            if (item.itemName == itemName) return item;

        return null;
    }

    /// <summary>
    /// Opens the item information panel and displays details of the selected item,
    /// including its name, description, and stats.
    /// </summary>
    private void OpenItemInfoPanel(ItemData item)
    {
        SoundEffects.Instance.PlaySound(buttonClickSound);
        selectedItem = item;

        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.itemDescription.Replace("\\n", "\n"); ;

        foreach (Transform child in selectedItemContainer)
            Destroy(child.gameObject);

        var itemUI = Instantiate(itemContainerPrefab, selectedItemContainer).GetComponent<ItemContainerUI>();
        itemUI.Setup(item, null); 
        Destroy(itemUI.purchaseButton.gameObject); 

        itemInfoPanel.SetActive(true);
    }

    /// <summary>
    /// Attempts to purchase the currently selected item, updating the shop UI and coins if successful.
    /// Plays a success or denial sound based on the purchase result.
    /// </summary>
    public void PurchaseSelectedItem()
    {
        if (selectedItem == null) return;

        if (GameManager.Instance.PurchaseItem(selectedItem))
        {
            SoundEffects.Instance.PlaySound(purchaseSound);
            totalCoinsText.text = $"{GameManager.Instance.coinsDeposited}";
            PopulateShop();
            CloseItemInfoPanel();
        }
        else
        {
            SoundEffects.Instance.PlaySound(deniedSound);
        }
    }

    /// <summary>
    /// Closes the item information panel and resets the selected item.
    /// </summary>
    public void CloseItemInfoPanel()
    {
        itemInfoPanel.SetActive(false);
        selectedItem = null;
    }

    /// <summary>
    /// Opens the shop menu and populates it with available items.
    /// </summary>
    public void OpenShopMenu()
    {
        shopMenuUI.SetActive(true);
        PopulateShop();
    }

    /// <summary>
    /// Closes the shop menu.
    /// </summary>
    public void CloseShopMenu()
    {
        shopMenuUI.SetActive(false);
    }
}
