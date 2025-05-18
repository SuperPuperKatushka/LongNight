using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryTests
{
    private GameObject inventoryObj;
    private Inventory inventory;
    private GameObject testItemPrefabSkill;
    private GameObject testItemPrefab;


    [SetUp]
    public void Setup()
    {
        // Создаем объект инвентаря
        inventoryObj = new GameObject("Inventory");
        inventory = inventoryObj.AddComponent<Inventory>();

        // Инициализируем PlayerStats
        var playerStatsObj = new GameObject("PlayerStats");
        playerStatsObj.AddComponent<PlayerStats>();

        // Настройка тестовых слотов
        inventory.slots = new GameObject[3];
        inventory.equipmentSlots = new GameObject[2];
        inventory.isFull = new bool[inventory.slots.Length];

        for (int i = 0; i < inventory.slots.Length; i++)
        {
            inventory.slots[i] = new GameObject($"Slot_{i}");
            inventory.slots[i].transform.SetParent(inventoryObj.transform);
        }

        for (int i = 0; i < inventory.equipmentSlots.Length; i++)
        {
            inventory.equipmentSlots[i] = new GameObject($"EqSlot_{i}");
            inventory.equipmentSlots[i].transform.SetParent(inventoryObj.transform);
        }

        // Создаем тестовый префаб предмета c типом Regular
        testItemPrefab = new GameObject("TestItem");
        var itemData = testItemPrefab.AddComponent<ItemData>();
        itemData.itemID = ItemID.GuardianShieldRuna;
        itemData.itemType = ItemType.Skill;

        // Создаем тестовый префаб предмета c типом Regular
        testItemPrefabSkill = new GameObject("TestItem2");
        var itemDataSkill = testItemPrefabSkill.AddComponent<ItemData>();
        itemDataSkill.itemID = ItemID.HealthPotion;
        itemDataSkill.itemType = ItemType.Regular;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(inventoryObj);
        Object.DestroyImmediate(testItemPrefab);
        Object.DestroyImmediate(GameObject.Find("PlayerStats"));
    }

    [Test]
    public void AddItem_AddsItemToEmptySlot()
    {
        // Act
        bool result = inventory.AddItem(testItemPrefab);

        // Assert
        Assert.IsTrue(result, "Предмет не был добавлен");
        Assert.IsTrue(inventory.isFull[0], "Первый слот должен быть заполнен");
        Assert.AreEqual(1, CountItemsInSlots(), "Должен быть 1 предмет в слотах");
    }


    [Test]
    public void EquipItem_MovesItemToEquipmentSlot()
    {
        // Arrange
        inventory.AddItem(testItemPrefab);
        var item = inventory.slots[0].GetComponentInChildren<ItemData>();

        // Act
        bool result = inventory.EquipItem(item, inventory.slots[0]);

        // Assert
        Assert.IsTrue(result, "Предмет должен экипироваться");
        Assert.AreEqual(0, CountItemsInSlots(), "Предмет должен исчезнуть из обычных слотов");
        Assert.AreEqual(1, CountEquippedItems(), "Предмет должен появиться в слотах экипировки");
    }


    [Test]
    public void EquipSkillItem_MovesItemToEquipmentSlot()
    {
        // Arrange
        inventory.AddItem(testItemPrefabSkill);
        var item = inventory.slots[0].GetComponentInChildren<ItemData>();

        // Act
        bool result = inventory.EquipItem(item, inventory.slots[0]);

        // Assert
        Assert.IsFalse(result, "Предмет не должен экипироваться");
        Assert.AreEqual(1, CountItemsInSlots(), "Предмет не должен исчезнуть из обычных слотов");
        Assert.AreEqual(0, CountEquippedItems(), "Предмет не должен появиться в слотах экипировки");
    }


    private int CountItemsInSlots()
    {
        int count = 0;
        foreach (var slot in inventory.slots)
        {
            if (slot.transform.childCount > 0) count++;
        }
        return count;
    }

    private int CountEquippedItems()
    {
        int count = 0;
        foreach (var slot in inventory.equipmentSlots)
        {
            if (slot.transform.childCount > 0) count++;
        }
        return count;
    }
}