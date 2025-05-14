using UnityEngine;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int level = 1;
    public int maxLevel = 10;
    public int maxHP;
    public int currentHP;
    public int maxMana;
    public int currentMana;
    public int attackPower;
    public int currentEXP;
    public int expToLevelUp = 100;

    [System.Serializable]
    public class InventoryData
    {
        public List<SlotItemData> slotItems = new List<SlotItemData>();
    }

    public InventoryData inventoryData = new InventoryData();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ApplyLevelStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GainEXP(int amount)
    {
        currentEXP += amount;
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        while (currentEXP >= expToLevelUp && level < maxLevel)
        {
            currentEXP -= expToLevelUp;
            LevelUp();
        }
    }

    public void LevelUp()
    {
        level++;
        ApplyLevelStats();
        Debug.Log($"Level Up! New level: {level}");
    }

    void ApplyLevelStats()
    {
        maxHP = 200 + (level - 1) * 50;
        attackPower = 10 + (level - 1) * 5;
        maxMana = 5 + (level - 1) * 5;
    }

    // Методы для работы с инвентарем
    public void SaveInventoryState(List<SlotItemData> slotItems)
    {
        inventoryData.slotItems = new List<SlotItemData>(slotItems);
    }

    public List<SlotItemData> GetInventoryState()
    {
        return inventoryData.slotItems;
    }
}

