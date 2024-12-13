using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the player's inventory, including equipping and unequipping items,
/// updating item stats, and interacting with the inventory and equip slots UI.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform inventoryContainer; 
    public Transform equipSlotsContainer; 
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public TMP_Text actionButtonText; 
    public Button actionButton; 

    public ShopManager shopManager; 
    private ItemData selectedItem; 
    private bool isEquippedItem; 

    public GameObject inventoryMenuUI;

    [SerializeField] private AudioClip equipSound;
    [SerializeField] private AudioClip unequipSound;
    [SerializeField] private AudioClip deniedSound;
    [SerializeField] private AudioClip buttonClickSound;


    /// <summary>
    /// Initializes the inventory manager, ensuring dependencies are set and the UI is updated.
    /// </summary>
    private void Start()
    {
        RefreshUI();
        UpdateItemInfo(null); 
    }

    /// <summary>
    /// Refreshes the entire UI by populating inventory and equipped slots.
    /// </summary>
    private void RefreshUI()
    {
        PopulateInventory();
        PopulateEquipSlots();
    }

    /// <summary>
    /// Populates the inventory container with all purchased items.
    /// </summary>
    private void PopulateInventory()
    {
        for (int i = 0; i < inventoryContainer.childCount; i++)
        {
            var inventoryItem = inventoryContainer.GetChild(i).GetComponent<InventoryItemContainerUI>();
            if (i < GameManager.Instance.purchasedItems.Count)
            {
                var itemName = GameManager.Instance.purchasedItems[i];
                var item = shopManager.ResolveItemByName(itemName);

                inventoryItem.gameObject.SetActive(true);
                inventoryItem.Setup(item, SelectItem);

                bool isEquipped = GameManager.Instance.equippedItems.Contains(item.itemName);
                inventoryItem.SetEquipped(isEquipped);
            }
            else
            {
                inventoryItem.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Populates the equip slots container with equipped items.
    /// </summary>
    private void PopulateEquipSlots()
    {
        for (int i = 0; i < equipSlotsContainer.childCount; i++)
        {
            var equipSlot = equipSlotsContainer.GetChild(i).GetComponent<InventoryItemContainerUI>();
            if (i < GameManager.Instance.equippedItems.Count)
            {
                var itemName = GameManager.Instance.equippedItems[i];
                var item = shopManager.ResolveItemByName(itemName);

                equipSlot.gameObject.SetActive(true);
                equipSlot.Setup(item, SelectItem);

                equipSlot.SetEquipped(false);
            }
            else
            {
                equipSlot.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Updates the ItemInfoPanel when an item is selected.
    /// </summary>
    private void SelectItem(ItemData item)
    {
        SoundEffects.Instance.PlaySound(buttonClickSound);
        selectedItem = item;
        isEquippedItem = GameManager.Instance.equippedItems.Contains(item.itemName);

        UpdateItemInfo(item);
    }

    /// <summary>
    /// Updates the ItemInfoPanel with the selected item's details.
    /// </summary>
    private void UpdateItemInfo(ItemData item)
    {
        if (item == null)
        {
            itemNameText.text = "Select an item";
            itemDescriptionText.text = "";
            actionButton.interactable = false;
            actionButtonText.text = "Equip";
            return;
        }

        itemNameText.text = item.itemName;

        itemDescriptionText.text = item.itemDescription.Replace(@"\n", "\n");

        actionButton.interactable = true;
        if (GameManager.Instance.equippedItems.Contains(item.itemName))
        {
            actionButtonText.text = "Unequip";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(UnequipSelectedItem);
        }
        else
        {
            actionButtonText.text = "Equip";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(EquipSelectedItem);
        }
    }

    /// <summary>
    /// Equips the selected item and updates stats.
    /// </summary>
    public void EquipSelectedItem()
    {
        if (selectedItem == null) return;

        if (GameManager.Instance.equippedItems.Count >= 6)
        {
            SoundEffects.Instance.PlaySound(deniedSound);
            return;
        }

        GameManager.Instance.equippedItems.Add(selectedItem.itemName);
        SoundEffects.Instance.PlaySound(equipSound);

        foreach (var modifier in selectedItem.statModifiers)
        {
            GameManager.Instance.ModifyStat(modifier.statName, modifier.value);
        }

        foreach (var effect in selectedItem.specialEffects)
        {
            GameManager.Instance.AddSpecialEffect(effect);
        }

        GameManager.Instance.SaveGameState();

        RefreshUI();
        UpdateItemInfo(selectedItem);
    }

    /// <summary>
    /// Unequips the selected item, updates stats, and removes special effects.
    /// </summary>
    public void UnequipSelectedItem()
    {
        if (selectedItem == null) return;

        GameManager.Instance.equippedItems.Remove(selectedItem.itemName);
        SoundEffects.Instance.PlaySound(unequipSound);

        foreach (var modifier in selectedItem.statModifiers)
        {
            GameManager.Instance.ModifyStat(modifier.statName, -modifier.value);
        }

        foreach (var effect in selectedItem.specialEffects)
        {
            GameManager.Instance.RemoveSpecialEffect(effect);
        }

        GameManager.Instance.SaveGameState();

        RefreshUI();
        UpdateItemInfo(selectedItem);
    }

    /// <summary>
    /// Opens the inventory menu and refreshes the UI to reflect the current inventory state.
    /// </summary>
    public void OpenInventoryMenu()
    {
        RefreshUI(); 
        inventoryMenuUI.SetActive(true);
    }

    /// <summary>
    /// Closes the inventory menu.
    /// </summary>
    public void CloseInventoryMenu()
    {
        inventoryMenuUI.SetActive(false);
    }
}
