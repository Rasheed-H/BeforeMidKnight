using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void SetPurchased(bool isPurchased)
    {
        purchaseButton.interactable = !isPurchased;
        if (isPurchased)
        {
            purchasedText.text = "PURCHASED";
        }
    }

}
