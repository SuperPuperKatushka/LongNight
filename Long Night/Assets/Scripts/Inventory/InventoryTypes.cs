using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Regular,
    Skill,
    Equipment
    // Добавьте другие типы по необходимости
}

[System.Serializable]


public enum ItemID
{
    None = 0,
    HealthPotion = 1,
    ManaPotion = 2,
    Sword = 3,
    Bow = 4,
    GuardianShieldRuna = 5,
    FlashFury = 6,
    coin = 7,
    keyMainQuest = 8,

    // Добавьте свои предметы
}

[System.Serializable]
public class SlotItem
{
    public int slotIndex;
    public ItemID itemID;
    public bool isEquipped;
}

[System.Serializable]
public class InventoryData
{
    public List<SlotItemData> slotItems = new List<SlotItemData>();
    public Dictionary<int, string> equippedItems = new Dictionary<int, string>(); // <slotIndex, itemID>
}

[System.Serializable]
public class SlotItemData
{
    public int slotIndex;
    public ItemID itemID;
    public bool isEquipped;
    public string prefabName;
}