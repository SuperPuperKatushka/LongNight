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
    public GameMessage gameMessage;
    private static Inventory instance;

    private bool inventoryOpen;
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
        LoadInventory();
    }



    public bool AddItem(GameObject itemPrefab)
    {
        UpdateSlotStatus();
        ItemData itemData = itemPrefab.GetComponent<ItemData>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (!isFull[i])
            {
                GameObject newItem = Instantiate(itemPrefab, slots[i].transform);
                isFull[i] = true;
                SaveInventory();
                OnItemPickUp?.Invoke(itemData.itemID.ToString());

                if (gameMessage != null)
                {
                    string text = "Предмет " + itemData.itemName + " добавлен в инвентарь";
                    gameMessage.ShowItemMessage(text);
                }
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

                int slotIndex = System.Array.IndexOf(slots, fromSlot);
                if (slotIndex >= 0) isFull[slotIndex] = false;

                PlayerStats.Instance.SaveEquippedItem(i, itemData);
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
                if (slotIndex >= 0)
                {
                    PlayerStats.Instance.equipmentData.equippedItems.RemoveAll(x => x.slotIndex == slotIndex);
                }

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
        // Реализуйте эффекты предметов здесь
    }

    public void SaveInventory()
    {
        List<SlotItemData> slotItems = new List<SlotItemData>();

        // Сохраняем обычные слоты
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].transform.childCount > 0)
            {
                var item = slots[i].transform.GetChild(0).GetComponent<ItemData>();
                if (item != null)
                {
                    slotItems.Add(new SlotItemData
                    {
                        slotIndex = i,
                        itemID = item.itemID,
                        isEquipped = false,
                        prefabName = item.gameObject.name.Replace("(Clone)", "")
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
                    slotItems.Add(new SlotItemData
                    {
                        slotIndex = i,
                        itemID = item.itemID,
                        isEquipped = true,
                        prefabName = item.gameObject.name.Replace("(Clone)", "")
                    });
                    // Сохраняем в equipmentData
                    PlayerStats.Instance.SaveEquippedItem(i, item);
                }
            }
        }

        PlayerStats.Instance.SaveInventoryState(slotItems);
    }

    private void UpdateSlotStatus()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            isFull[i] = slots[i].transform.childCount > 0;
        }
    }

    private void LoadInventory()
    {
        ClearAllSlots();

        // Загружаем обычные предметы
        var slotItems = PlayerStats.Instance.GetInventoryState();
        foreach (var slotItem in slotItems)
        {
            if (!slotItem.isEquipped)
            {
                LoadItemToSlot(slotItem, slots);
            }
        }

        // Загружаем экипированные предметы из equipmentData
        foreach (var equippedItem in PlayerStats.Instance.equipmentData.equippedItems)
        {
            LoadItemToSlot(equippedItem, equipmentSlots);
        }
    }

    private void LoadItemToSlot(SlotItemData slotItem, GameObject[] targetSlots)
    {
        if (slotItem.slotIndex >= targetSlots.Length) return;

        GameObject itemPrefab = Resources.Load<GameObject>($"Items/{slotItem.prefabName}");
        if (itemPrefab == null) return;

        GameObject itemObj = Instantiate(itemPrefab, targetSlots[slotItem.slotIndex].transform);
        itemObj.transform.localPosition = Vector3.zero;

        if (targetSlots == slots)
        {
            isFull[slotItem.slotIndex] = true;
        }
    }

    private void ClearAllSlots()
    {
        foreach (var slot in slots)
        {
            if (slot.transform.childCount > 0)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }
        }
        foreach (var eqSlot in equipmentSlots)
        {
            if (eqSlot.transform.childCount > 0)
            {
                Destroy(eqSlot.transform.GetChild(0).gameObject);
            }
        }
        equippedItems.Clear();
    }


    public List<ItemData> GetEquippedSkills()
    {
        List<ItemData> equippedSkills = new List<ItemData>();

        // Проверяем все слоты экипировки
        foreach (var slot in equipmentSlots)
        {
            // Если в слоте есть предмет и у него тип Skill
            if (slot.transform.childCount > 0)
            {
                ItemData item = slot.transform.GetChild(0).GetComponent<ItemData>();
                if (item != null && item.itemType == ItemType.Skill)
                {
                    equippedSkills.Add(item);
                }
            }
        }

        return equippedSkills;
    }
}