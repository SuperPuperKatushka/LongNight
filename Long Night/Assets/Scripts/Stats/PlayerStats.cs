using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    [System.Serializable]

    public class EquipmentData
    {
        public List<SlotItemData> equippedItems = new List<SlotItemData>();
    }

    public EquipmentData equipmentData = new EquipmentData();

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
    public bool TakeDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
        Debug.Log($"Игрок получил {damage} урона. Осталось HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            //Die();
            return true;
        }
        return false;
    }

    public void Heal(int healAmount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + healAmount);
        Debug.Log($"Игрок восстановил {healAmount} HP. Теперь HP: {currentHP}/{maxHP}");
    }

    public void Heal(float percent)
    {
        if (percent <= 0)
        {
            Debug.LogWarning("Процент лечения должен быть больше 0");
            return;
        }

        int healAmount = Mathf.RoundToInt(maxHP * Mathf.Clamp01(percent));
        Heal(healAmount); // Используем основную версию метода
    }

    public bool SpendMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            Debug.Log($"Потрачено {amount} маны. Осталось: {currentMana}/{maxMana}");
            return true;
        }
        Debug.Log($"Недостаточно маны! Нужно: {amount}, есть: {currentMana}");
        return false;
    }

    public void RestoreMana(int amount)
    {
        currentMana = Mathf.Min(maxMana, currentMana + amount);
        Debug.Log($"Восстановлено {amount} маны. Теперь маны: {currentMana}/{maxMana}");
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

    public void ApplyLevelStats()
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

    // Новый метод для получения экипированных навыков
    public List<ItemData> GetEquippedSkills()
    {
        List<ItemData> skills = new List<ItemData>();

        foreach (var slotItem in equipmentData.equippedItems)
        {
            if (slotItem.isEquipped)
            {
                // Здесь нужно загрузить ItemData из сохраненных данных
                GameObject itemPrefab = Resources.Load<GameObject>($"Items/{slotItem.prefabName}");
                if (itemPrefab != null)
                {
                    ItemData itemData = itemPrefab.GetComponent<ItemData>();
                    if (itemData != null)
                    {
                        skills.Add(itemData);
                    }
                }
            }
        }

        return skills;
    }

    // Метод для сохранения экипированных предметов
    public void SaveEquippedItem(int slotIndex, ItemData itemData)
    {
        var existing = equipmentData.equippedItems.FirstOrDefault(x => x.slotIndex == slotIndex);
        if (existing != null)
        {
            existing.itemID = itemData.itemID;
            existing.prefabName = itemData.gameObject.name.Replace("(Clone)", "");
        }
        else
        {
            equipmentData.equippedItems.Add(new SlotItemData
            {
                slotIndex = slotIndex,
                itemID = itemData.itemID,
                isEquipped = true,
                prefabName = itemData.gameObject.name.Replace("(Clone)", "")
            });
        }
    }

    [System.Serializable]
    public class PlayerStatsData
    {
        public int level;
        public int maxHP;
        public int currentHP;
        public int maxMana;
        public int currentMana;
        public int attackPower;
        public int currentEXP;
        public int expToLevelUp;
    }

    public PlayerStatsData GetSaveData()
    {
        return new PlayerStatsData
        {
            level = level,
            maxHP = maxHP,
            currentHP = currentHP,
            maxMana = maxMana,
            currentMana = currentMana,
            attackPower = attackPower,
            currentEXP = currentEXP,
            expToLevelUp = expToLevelUp
        };
    }
    public void LoadSaveData(PlayerStatsData data)
    {
        level = data.level;
        maxHP = data.maxHP;
        currentHP = data.currentHP;
        maxMana = data.maxMana;
        currentMana = data.currentMana;
        attackPower = data.attackPower;
        currentEXP = data.currentEXP;
        expToLevelUp = data.expToLevelUp;
    }

    public void ResetToDefault()
    {
        level = 1;
        ApplyLevelStats();
        currentHP = maxHP;
        currentMana = maxMana;
        currentEXP = 0;
        inventoryData.slotItems.Clear();
        equipmentData.equippedItems.Clear();
    }
}

