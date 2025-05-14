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
        inventoryUI.SetActive(false);
        LoadInventory();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (var slotItem in PlayerStats.Instance.GetInventoryState())
                Debug.Log("ААААААА " + slotItem);
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;
        inventoryUI.SetActive(inventoryOpen);

        if (!inventoryOpen)
        {
            UpdateSlotStatus();
            SaveInventory();
        }
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

    public void LoadInventory()
    {
        ClearAllSlots();
        var slotItems = PlayerStats.Instance.GetInventoryState();
        Debug.Log($"Загружаем инвентарь, найдено {slotItems.Count} предметов");

        foreach (var slotItem in slotItems)
        {
            // Проверка корректности данных
            if (string.IsNullOrEmpty(slotItem.prefabName))
            {
                Debug.LogWarning("Обнаружен предмет с пустым именем префаба");
                continue;
            }

            // Загрузка префаба с учетом возможных вариантов именования
            GameObject itemPrefab = Resources.Load<GameObject>($"Items/{slotItem.prefabName}") ??
                                  Resources.Load<GameObject>($"Items/{slotItem.prefabName.Replace("(Clone)", "")}");

            if (itemPrefab == null)
            {
                Debug.LogError($"Не удалось загрузить префаб: Items/{slotItem.prefabName}");
                continue;
            }

            // Создание экземпляра предмета
            GameObject itemObj = Instantiate(itemPrefab);
            ItemData itemData = itemObj.GetComponent<ItemData>();

            if (itemData == null)
            {
                Debug.LogError($"У префаба {slotItem.prefabName} отсутствует компонент ItemData");
                Destroy(itemObj);
                continue;
            }

            // Размещение в соответствующем слоте
            if (slotItem.isEquipped)
            {
                if (slotItem.slotIndex < equipmentSlots.Length)
                {
                    itemObj.transform.SetParent(equipmentSlots[slotItem.slotIndex].transform);
                    itemObj.transform.localPosition = Vector3.zero;
                    itemObj.transform.localScale = Vector3.one; // Важно!
                    equippedItems[slotItem.slotIndex] = itemData;
                    Debug.Log($"Экипирован предмет {slotItem.prefabName} в слот {slotItem.slotIndex}");
                }
            }
            else
            {
                if (slotItem.slotIndex < slots.Length)
                {
                    itemObj.transform.SetParent(slots[slotItem.slotIndex].transform);
                    itemObj.transform.localPosition = Vector3.zero;
                    itemObj.transform.localScale = Vector3.one; // Важно!
                    isFull[slotItem.slotIndex] = true;
                    Debug.Log($"Добавлен предмет {slotItem.prefabName} в слот {slotItem.slotIndex}");
                }
            }
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
}