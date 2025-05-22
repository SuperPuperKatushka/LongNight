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

    // Применение урона к игроку
    public bool TakeDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);

        if (currentHP <= 0)
        {
            return true;
        }
        return false;
    }

    // Проверяет, мёртв ли игрок (HP равен 0 или меньше)
    public bool IsDeath()
    {
        return currentHP <= 0;
    }


    // Восстановление здоровья на фиксированное значение
    public void Heal(int healAmount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + healAmount);
    }

    // Восстановление здоровья в процентах от максимального
    public void Heal(float percent)
    {
        if (percent <= 0)
        {
            return;
        }

        int healAmount = Mathf.RoundToInt(maxHP * Mathf.Clamp01(percent));
        Heal(healAmount); // Используем основную версию метода
    }

    // Трата маны
    public bool SpendMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            return true;
        }
        return false;
    }

    // Восстановление маны на указанное количество
    public void RestoreMana(int amount)
    {
        currentMana = Mathf.Min(maxMana, currentMana + amount);
    }

    // Получение опыта и проверка на повышение уровня
    public void GainEXP(int amount)
    {
        currentEXP += amount;
        CheckLevelUp();
    }

    // Проверка и выполнение повышения уровня при достаточном опыте
    void CheckLevelUp()
    {
        while (currentEXP >= expToLevelUp && level < maxLevel)
        {
            currentEXP -= expToLevelUp;
            LevelUp();
        }
    }

    // Повышение уровня игрока и перерасчет характеристик
    public void LevelUp()
    {
        level++;
        ApplyLevelStats();
    }

    // Применение новых характеристик на основе уровня
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

    // Метод для получения экипированных навыков
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
    // Возвращает объект со всеми текущими параметрами игрока для сохранения
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
    // Загружает параметры игрока из сохраненного состояния
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
    // Сброс всех характеристик игрока до стартовых значений
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
}
