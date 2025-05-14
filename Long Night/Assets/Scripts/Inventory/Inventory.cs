using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    public static event System.Action<string> OnItemPickUp;
    [Header("Slots")]
    public bool[] isFull;
    public GameObject[] slots;
    public GameObject[] equipmentSlots;
    public GameObject inventoryUI;
    private bool inventoryOpen;
    private static Inventory instance;

    private Dictionary<int, ItemData> equippedItems = new Dictionary<int, ItemData>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public static Inventory Instance => instance;

    private void Start()
    {
        inventoryOpen = false;
        inventoryUI.SetActive(false);
        LoadInventory();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;
        inventoryUI.SetActive(inventoryOpen);

        // Обновляем статус слотов при закрытии
        if (!inventoryOpen)
        {
            UpdateSlotStatus();
        }
    }

    public bool AddItem(GameObject itemPrefab)
    {

        UpdateSlotStatus(); // Обязательно обновляем перед добавлением
        ItemData itemData = itemPrefab.GetComponent<ItemData>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (!isFull[i])
            {
                GameObject newItem = Instantiate(itemPrefab, slots[i].transform);
                isFull[i] = true;
                SaveInventory();

                // Вызываем событие после успешного добавления
                OnItemPickUp?.Invoke(itemData.itemID.ToString());
                return true;
            }
        }
        return false;
    }

    public bool EquipItem(ItemData itemData, GameObject fromSlot)
    {
        if (itemData.itemType != ItemType.Skill && itemData.itemType != ItemType.Equipment)
            return false;

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].transform.childCount == 0)
            {
                itemData.transform.SetParent(equipmentSlots[i].transform);
                itemData.transform.localPosition = Vector3.zero;

                equippedItems[i] = itemData;

                int slotIndex = System.Array.IndexOf(slots, fromSlot);
                if (slotIndex >= 0) isFull[slotIndex] = false;

                ApplyItemEffects(itemData, true);
                SaveInventory();
                return true;
            }
        }
        return false;
    }

    public bool UnequipItem(ItemData itemData, GameObject fromEquipmentSlot)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!isFull[i])
            {
                itemData.transform.SetParent(slots[i].transform);
                itemData.transform.localPosition = Vector3.zero;

                int slotIndex = System.Array.IndexOf(equipmentSlots, fromEquipmentSlot);
                if (slotIndex >= 0) equippedItems.Remove(slotIndex);

                isFull[i] = true;

                ApplyItemEffects(itemData, false);
                SaveInventory();
                return true;
            }
        }
        return false;
    }

    private void ApplyItemEffects(ItemData itemData, bool apply)
    {
        int modifier = apply ? 1 : -1;

        //switch (itemData.itemID)
        //{
        //    case ItemID.FireballSkill:
        //        PlayerStats.Instance.AddDamageBonus(10 * modifier);
        //        break;
        //    case ItemID.HealSkill:
        //        PlayerStats.Instance.AddHealthRegen(2 * modifier);
        //        break;
        //    case ItemID.Sword:
        //        PlayerStats.Instance.AddAttackPower(15 * modifier);
        //        break;
        //    case ItemID.Shield:
        //        PlayerStats.Instance.AddDefense(10 * modifier);
        //        break;
        //}
    }

    public void SaveInventory()
    {
        InventoryData data = new InventoryData();

        // Сохраняем обычные слоты
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].transform.childCount > 0)
            {
                var item = slots[i].transform.GetChild(0).GetComponent<ItemData>();
                if (item != null)
                {
                    data.slotItems.Add(new SlotItemData
                    {
                        slotIndex = i,
                        itemID = item.itemID,
                        isEquipped = false,
                        prefabName = item.gameObject.name.Replace("(Clone)", "") // Сохраняем имя префаба
                    });
                }
            }
        }

        // Сохраняем слоты экипировки
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].transform.childCount > 0)
            {
                var item = equipmentSlots[i].transform.GetChild(0).GetComponent<ItemData>();
                if (item != null)
                {
                    data.slotItems.Add(new SlotItemData
                    {
                        slotIndex = i,
                        itemID = item.itemID,
                        isEquipped = true,
                        prefabName = item.gameObject.name.Replace("(Clone)", "")
                    });
                }
            }
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("InventoryData", json);
        PlayerPrefs.Save();
    }
    private void UpdateSlotStatus()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            isFull[i] = slots[i].transform.childCount > 0;
        }
    }
    public void LoadInventory()
    {
        //if (PlayerPrefs.HasKey("InventoryData"))
        //{
        //    string json = PlayerPrefs.GetString("InventoryData");
        //    InventoryData data = JsonUtility.FromJson<InventoryData>(json);

        //    foreach (var slotItem in data.slotItems)
        //    {
        //        // Загружаем префаб по имени
        //        GameObject itemPrefab = Resources.Load<GameObject>("Items/" + slotItem.prefabName);
        //        if (itemPrefab != null)
        //        {
        //            GameObject itemObj = Instantiate(itemPrefab);
        //            ItemData itemData = itemObj.GetComponent<ItemData>();

        //            if (slotItem.isEquipped)
        //            {
        //                if (slotItem.slotIndex < equipmentSlots.Length)
        //                {
        //                    itemObj.transform.SetParent(equipmentSlots[slotItem.slotIndex].transform);
        //                    itemObj.transform.localPosition = Vector3.zero;
        //                    equippedItems[slotItem.slotIndex] = itemData;
        //                    ApplyItemEffects(itemData, true);
        //                }
        //            }
        //            else
        //            {
        //                if (slotItem.slotIndex < slots.Length)
        //                {
        //                    itemObj.transform.SetParent(slots[slotItem.slotIndex].transform);
        //                    itemObj.transform.localPosition = Vector3.zero;
        //                    isFull[slotItem.slotIndex] = true;
        //                }
        //            }
        //        }
        //    }
        //}
    }
}

