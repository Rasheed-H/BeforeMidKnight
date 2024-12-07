using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    [TextArea(3, 10)]
    public string itemDescription;
    public int itemPrice;
    public string itemRarity;
    public List<StatModifier> statModifiers = new List<StatModifier>();
    public List<string> specialEffects = new List<string>();
}

[System.Serializable]
public class StatModifier
{
    public string statName;
    public float value;
}
