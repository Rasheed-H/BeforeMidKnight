using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the UI for displaying an inventory item, including its appearance, rarity frame,
/// and interaction for equipping or selecting the item.
/// </summary>
public class InventoryItemContainerUI : MonoBehaviour
{
    public Image frame;                     
    public Image itemSprite;               
    public TextMeshProUGUI itemName;        
    public Button selectButton;             
    public TextMeshProUGUI statusText;      

    public Sprite commonFrame;              
    public Sprite rareFrame;            
    public Sprite mythicalFrame;

    /// <summary>
    /// Configures the UI for an inventory item, setting its sprite, name, rarity frame, and the 
    /// on-click action for the select button.
    /// </summary>
    public void Setup(ItemData item, System.Action<ItemData> onEquip)
    {
        itemSprite.sprite = item.itemSprite;
        itemName.text = item.itemName;

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

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onEquip(item));
    }

    /// <summary>
    /// Updates the UI to reflect whether the item is equipped, disabling the select button
    /// and showing the equipped status if applicable.
    /// </summary>
    public void SetEquipped(bool isEquipped)
    {
        selectButton.interactable = !isEquipped;
        if (isEquipped)
        {
            statusText.text = "EQUIPPED";
        }
        else
        {
            statusText.text = "";
        }
    }
}