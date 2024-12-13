using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents an item in the game, including its name, sprite, description, price, rarity,
/// stat modifiers, and special effects.
/// </sum
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

/// <summary>
/// Represents a modification to a specific stat (stat's name and the value)
/// </summary>
[System.Serializable]
public class StatModifier
{
    public string statName;
    public float value;
}
