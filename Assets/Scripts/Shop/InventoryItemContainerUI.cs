using UnityEngine;
using UnityEngine.UI;
using TMPro;

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