using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the UI for a single item in the shop or inventory, including its appearance,
/// cost, rarity frame, and purchase interactions.
/// </summary>
public class ItemContainerUI : MonoBehaviour
{
    public Image frame; 
    public Image itemSprite; 
    public TextMeshProUGUI itemName; 
    public TextMeshProUGUI itemCost; 
    public Button purchaseButton; 
    public TextMeshProUGUI purchasedText;

    public Sprite commonFrame; 
    public Sprite rareFrame;   
    public Sprite mythicalFrame;

    /// <summary>
    /// Configures the UI elements of the item container, including the item's sprite, name, cost,
    /// rarity frame, and the on-click action for the purchase button.
    /// </summary>
    public void Setup(ItemData item, System.Action<ItemData> onSelect)
    {

        itemSprite.sprite = item.itemSprite;
        itemName.text = item.itemName;
        itemCost.text = $"x{item.itemPrice}";

        switch (item.itemRarity.ToLower())
        {
            case "common":
                frame.sprite = commonFrame;
                break;
            case "rare":
                frame.sprite = rareFrame;
                break;
            case "mythical":
                frame.sprite = mythicalFrame;
                break;
        }

        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(() => onSelect(item));
    }

    /// <summary>
    /// Updates the item container to reflect whether the item has been purchased, disabling
    /// the purchase button and showing the "PURCHASED" text if applicable.
    /// </summary>
    public void SetPurchased(bool isPurchased)
    {
        purchaseButton.interactable = !isPurchased;
        if (isPurchased)
        {
            purchasedText.text = "PURCHASED";
        }
    }

}
