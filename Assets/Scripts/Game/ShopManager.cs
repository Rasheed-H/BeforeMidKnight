using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

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

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

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

    private List<ItemData> GenerateDailyItems()
    {
        List<ItemData> dailyItems = new List<ItemData>();
        int totalSlots = 4;

        var allItems = commonItems.Concat(rareItems).Concat(mythicalItems).ToList();

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

    public ItemData ResolveItemByName(string itemName)
    {
        foreach (var item in commonItems)
            if (item.itemName == itemName) return item;

        foreach (var item in rareItems)
            if (item.itemName == itemName) return item;

        foreach (var item in mythicalItems)
            if (item.itemName == itemName) return item;

        Debug.LogError($"Item '{itemName}' not found in any item pool.");
        return null;
    }

    private void OpenItemInfoPanel(ItemData item)
    {
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

    public void PurchaseSelectedItem()
    {
        if (selectedItem == null) return;

        if (GameManager.Instance.coinsDeposited >= selectedItem.itemPrice)
        {
            GameManager.Instance.coinsDeposited -= selectedItem.itemPrice;
            GameManager.Instance.purchasedItems.Add(selectedItem.itemName);
            GameManager.Instance.SaveGameState();

            Debug.Log($"Item '{selectedItem.itemName}' purchased for {selectedItem.itemPrice} coins.");
            PopulateShop();
            CloseItemInfoPanel();
        }
        else
        {
            Debug.LogWarning($"Not enough coins to purchase '{selectedItem.itemName}'. Need {selectedItem.itemPrice - GameManager.Instance.coinsDeposited} more coins.");
        }

    }

    public void CloseItemInfoPanel()
    {
        // Close the ItemInfoPanel
        itemInfoPanel.SetActive(false);
        selectedItem = null;
    }

    public void OpenShopMenu()
    {
        shopMenuUI.SetActive(true);
        PopulateShop();
    }

    public void CloseShopMenu()
    {
        shopMenuUI.SetActive(false);
    }
}
